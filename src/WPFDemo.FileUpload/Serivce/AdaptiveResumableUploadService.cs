using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WPFDemo.FileUpload.Serivce
{
    public class AdaptiveResumableUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUri;
        private const string UploadChunkApiUri = "/api/Upload/UploadChunk";
        private const string GetUploadStatusApiUri = "/api/Upload/GetUploadStatus";
        private const string MergeChunksApiUri = "/api/Upload/MergeChunks";
        private const string PingApiUri = "/api/Ping";
        private readonly int _maxParallelism;
        private readonly int _minChunkSize;
        private readonly int _maxChunkSize;
        private readonly int _initialChunkSize;
        private readonly int _maxRetryCount;

        // 网络状况监测
        private double _averageUploadSpeedMbps = 1.0; // 初始假设1Mbps
        private double _averageLatencyMs = 100; // 初始延迟100ms
        private readonly object _networkLock = new object();
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        // 进度和状态事件
        public event Action<int> ProgressChanged; // 上传百分比
        public event Action<string> StatusChanged; // 状态消息
        public event Action<bool> UploadCompleted; // 完成状态

        public AdaptiveResumableUploadService(string serviceUri,
            int maxParallelism = 4,
            int minChunkSize = 256 * 1024,    // 256KB
            int maxChunkSize = 10 * 1024 * 1024, // 10MB
            int initialChunkSize = 1 * 1024 * 1024, // 1MB
            int maxRetryCount = 10)
        {
            _serviceUri = serviceUri;
            _httpClient = new HttpClient();
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
            _maxParallelism = maxParallelism;
            _minChunkSize = minChunkSize;
            _maxChunkSize = maxChunkSize;
            _initialChunkSize = initialChunkSize;
            _maxRetryCount = maxRetryCount;
        }

        // 主上传方法
        public async Task UploadFileAdaptiveAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                StatusChanged?.Invoke("正在准备上传...");

                var fileInfo = new FileInfo(filePath);
                using var fileStream = File.OpenRead(filePath);
                var fileId = GetFileHash(fileStream, HashAlgorithmType.Sha256); // 基于文件内容生成唯一ID

                StatusChanged?.Invoke("正在检查已上传分片...");

                // 准备上传任务
                var currentChunkSize = _initialChunkSize;
                var fileSize = fileInfo.Length;

                var progressLock = new object();

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxParallelism,
                    CancellationToken = cancellationToken
                };

                var retryCount = 0;
                var completed = false;
                while (!completed && retryCount < _maxRetryCount)
                {
                    // 获取已上传的分片信息
                    var uploadedChunks = await GetUploadedChunksAsync(fileId, cancellationToken);
                    var totalUploaded = uploadedChunks.Values.Sum();
                    long totalToUpload = fileSize - totalUploaded;
                    StatusChanged?.Invoke($"需要上传 {totalToUpload} 字节");
                    try
                    {
                        await Parallel.ForEachAsync(GenerateChunks(fileSize, currentChunkSize, uploadedChunks), parallelOptions, async (chunk, cancellationToken) =>
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            bool success = await UploadChunkAsync(fileId, chunk.Index, chunk.Offset, chunk.Size, fileSize, fileInfo.Name, fileStream, cancellationToken);

                            if (success)
                            {
                                // 使用锁保证进度更新的原子性
                                lock (progressLock)
                                {
                                    totalUploaded += chunk.Size;
                                    int progress = (int)((double)totalUploaded / fileSize * 100);
                                    ProgressChanged?.Invoke(progress);
                                }
                            }
                        });

                        await MergeChunksAsync(fileId, fileInfo.Name, cancellationToken);
                        completed = true;
                        ProgressChanged?.Invoke(100);
                        StatusChanged?.Invoke("上传完成！");
                        UploadCompleted?.Invoke(true);
                    }
                    catch
                    {
                        retryCount++;
                        if (retryCount >= _maxRetryCount) throw;
                        await Task.Delay(100 * retryCount, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                StatusChanged?.Invoke("上传已取消");
                UploadCompleted?.Invoke(false);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"上传失败: {ex.Message}");
                UploadCompleted?.Invoke(false);
            }
        }

        // 获取已上传的分片信息
        private async Task<ConcurrentDictionary<long, int>> GetUploadedChunksAsync(string fileId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_serviceUri}{GetUploadStatusApiUri}?fileId={fileId}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<UploadStatusResponse>(cancellationToken);
                return new ConcurrentDictionary<long, int>(result.UploadedChunks.ToDictionary(x => x.Offset, x => x.Size));
            }
            catch
            {
                return new ConcurrentDictionary<long, int>();
            }
        }

        // 上传单个分片
        private async Task<bool> UploadChunkAsync(string fileId, int chunkIndex, long chunkOffset, int chunkSize, long fileSize, string fileName, Stream fileStream, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // 测量请求延迟
                var latencyStopwatch = Stopwatch.StartNew();
                var pingResponse = await _httpClient.GetAsync($"{_serviceUri}{PingApiUri}", cancellationToken);
                latencyStopwatch.Stop();

                // 准备分片数据
                var chunkData = new byte[chunkSize];
                fileStream.Seek(chunkOffset, SeekOrigin.Begin);
                var bytesRead = await fileStream.ReadAsync(chunkData, 0, chunkSize, cancellationToken);
                using var stream = new MemoryStream(chunkData);
                var chunkHash = GetFileHash(stream, HashAlgorithmType.Md5);

                if (bytesRead == 0) return false;

                using var content = new MultipartFormDataContent
                {
                    { new StringContent(fileId), "fileId" },
                    { new StringContent(chunkIndex.ToString()), "chunkIndex" },
                    { new StringContent(chunkOffset.ToString()), "chunkOffset" },
                    { new StringContent(bytesRead.ToString()), "chunkSize"},
                    { new StringContent(chunkHash.ToString()), "chunkHash"},
                    { new StringContent(fileSize.ToString()), "fileSize"},
                    { new StringContent(fileName), "fileName" },
                    { new ByteArrayContent(chunkData, 0, bytesRead), "chunk", "chunk.dat" }
                };

                // 上传
                var uploadStopwatch = Stopwatch.StartNew();
                var response = await _httpClient.PostAsync($"{_serviceUri}{UploadChunkApiUri}", content, cancellationToken);
                uploadStopwatch.Stop();

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpRequestException(error);
                }

                // 更新网络指标
                UpdateNetworkMetrics(bytesRead, uploadStopwatch.Elapsed, latencyStopwatch.Elapsed);
                return true;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        // 上传完所有分片后合并分片
        private async Task MergeChunksAsync(string fileId, string fileName, CancellationToken cancellationToken)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(new { FileId = fileId, FileName = fileName }), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_serviceUri}{MergeChunksApiUri}", stringContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(error);
            }
        }

        private static string GetFileHash(Stream stream, HashAlgorithmType hashAlgorithmType)
        {
            switch (hashAlgorithmType)
            {
                case HashAlgorithmType.Md5:
                    using (var hashAlgorithm = MD5.Create())
                    {
                        var hash = hashAlgorithm.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                case HashAlgorithmType.Sha256:
                    using (var hashAlgorithm = SHA256.Create())
                    {
                        var hash = hashAlgorithm.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLower();
                    }
                default:
                    return string.Empty;
            }
        }

        #region 更新网络指标并动态计算当前分片大小
        // 动态计算当前分片大小
        private int CalculateDynamicChunkSize()
        {
            lock (_networkLock)
            {
                // 基于当前上传速度和延迟计算理想分片大小
                double targetUploadTimeSec = 2.0; // 目标每个分片上传时间
                double speedFactor = _averageUploadSpeedMbps * 125000; // Mbps -> bytes/sec
                double latencyFactor = Math.Max(1, _averageLatencyMs / 50);

                int idealSize = (int)(speedFactor * targetUploadTimeSec / latencyFactor);

                // 限制在最小和最大值之间
                return (int)Math.Clamp(idealSize, _minChunkSize, _maxChunkSize);
            }
        }

        // 更新网络指标
        private void UpdateNetworkMetrics(long chunkSize, TimeSpan uploadTime, TimeSpan latency)
        {
            lock (_networkLock)
            {
                // 计算速度 (Mbps)
                double speedMbps = (chunkSize * 8 / uploadTime.TotalSeconds) / 1_000_000;
                // 平滑处理速度值（加权平均）
                _averageUploadSpeedMbps = 0.7 * _averageUploadSpeedMbps + 0.3 * speedMbps;
                // 更新延迟
                _averageLatencyMs = 0.8 * _averageLatencyMs + 0.2 * latency.TotalMilliseconds;

                StatusChanged?.Invoke($"网络: {_averageUploadSpeedMbps:F1}Mbps, " +
                                    $"延迟: {_averageLatencyMs:F0}ms, " +
                                    $"分片: {CalculateDynamicChunkSize() / 1024}KB");
            }
        }

        private IEnumerable<FileChunk> GenerateChunks(long fileSize, int currentChunkSize, ConcurrentDictionary<long, int> uploadedChunks)
        {
            long position = 0;
            int chunkIndex = 0;

            while (position < fileSize)
            {
                _semaphoreSlim.WaitAsync(); // 使用异步锁确保分片信息更新时的原子性
                if (!uploadedChunks.TryGetValue(position, out var chunkSize))
                {
                    // 动态调整分片大小
                    currentChunkSize = CalculateDynamicChunkSize();
                    var actualChunkSize = (int)Math.Min(currentChunkSize, fileSize - position);

                    // 惰性求值: 迭代器代码直到开始遍历才会执行，每次迭代时返回一个值，并保持当前执行状态(局部变量、执行位置等)
                    yield return new FileChunk
                    {
                        Index = chunkIndex,
                        Offset = position,
                        Size = actualChunkSize,
                        RemainingSize = fileSize - (position + actualChunkSize)
                    };

                    position += actualChunkSize;
                }
                else
                {
                    position += chunkSize;
                }

                chunkIndex++;

                _semaphoreSlim.Release();
            }
        }
        #endregion


        #region record
        private record UploadedChunk
        {
            public long Offset { get; init; }
            public int Size { get; init; }
        }

        private record UploadStatusResponse
        {
            public List<UploadedChunk> UploadedChunks { get; init; }
        }

        private record FileChunk
        {
            public int Index { get; init; }
            public long Offset { get; init; }
            public int Size { get; init; }
            public long RemainingSize { get; init; }
        }
        #endregion
    }
}
