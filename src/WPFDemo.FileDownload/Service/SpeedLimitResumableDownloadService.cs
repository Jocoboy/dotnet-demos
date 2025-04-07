using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo.FileDownload.Service
{
    public class SpeedLimitResumableDownloadService
    {
        private readonly HttpClient _httpClient;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _activeDownloads;
        private const int BufferSize = 8192; // 缓冲区 8KB
        private const int ChunkSize = 1024 * 1024; // 1MB 每块
        private readonly int _maxParallelism;
        private readonly int _maxRetryCount;
        private int _maxSpeedKBps; // 0表示不限速
        private Stopwatch _stopWatch;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);  // 使用SemaphoreSlim实现异步锁，初始化时设置最大并发数为 1

        // 进度和状态事件
        public event Action<long, long> ProgressChanged; // 当前下载量, 总大小
        public event Action<int> DownloadCompleted;     // 下载ID
        public event Action<int, Exception> DownloadFailed; // 下载ID, 异常
        public event Action<double> SpeedChanged;      // 下载速度(KB/s)

        public SpeedLimitResumableDownloadService(int maxParallelism = 4, int maxRetryCount = 10, int maxSpeedKBps = 0)
        {
            _httpClient = new HttpClient();
            _stopWatch = new Stopwatch();
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
            _activeDownloads = new ConcurrentDictionary<int, CancellationTokenSource>();
            _maxParallelism = maxParallelism;
            _maxRetryCount = maxRetryCount;
            _maxSpeedKBps = maxSpeedKBps;
        }

        public async Task<int> StartParallelDownloadAsync(string fileUrl, string destDir)
        {
            var downloadId = fileUrl.GetHashCode();
            var cts = new CancellationTokenSource();
            _activeDownloads.TryAdd(downloadId, cts);
            try
            {
                // 获取文件总大小
                var fileSize = await GetFileSizeAsync(fileUrl);

                // 创建目标文件
                var destFile = fileUrl.Split("/").Last();
                var destinationPath = $"{destDir}\\{destFile}";
                var tempDir = $"{destDir}\\temp";
                var tempPath = $"{tempDir}\\{Path.GetFileNameWithoutExtension(destFile)}.tmp";
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                {
                    fs.SetLength(fileSize);
                }

                // 计算分片
                var chunks = CalculateChunks(fileSize, _maxParallelism);

                // 并行下载
                var retryCount = 0;
                var completed = false;
                while (!completed && retryCount < _maxRetryCount)
                {
                    try
                    {
                        await Task.Run(() => Parallel.ForEachAsync(chunks, new ParallelOptions
                        {
                            MaxDegreeOfParallelism = _maxParallelism,
                            CancellationToken = cts.Token
                        }, async (chunk, ct) =>
                        {
                            await _semaphoreSlim.WaitAsync();
                            await DownloadChunkAsync(downloadId, fileUrl, tempPath, chunk.Start, chunk.End, fileSize, ct);
                            _semaphoreSlim.Release();
                        }), cts.Token);

                        completed = true;
                    }
                    catch
                    {
                        retryCount++;
                        if (retryCount >= _maxRetryCount) throw;
                        await Task.Delay(100 * retryCount); // 指数退避
                    }
                }
                // 下载完成后重命名临时文件
                File.Move(tempPath, destinationPath, true);
                DownloadCompleted?.Invoke(downloadId);
                return downloadId;
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不视为错误
                return downloadId;
            }
            catch (Exception ex)
            {
                DownloadFailed?.Invoke(downloadId, ex);
                throw;
            }
            finally
            {
                _activeDownloads.TryRemove(downloadId, out _);
            }
        }

        public void PauseDownload(int downloadId)
        {
            if (_activeDownloads.TryGetValue(downloadId, out var cts))
            {
                cts.Cancel();
            }
        }

        private async Task DownloadChunkAsync(int downloadId, string fileUrl, string destinationPath, long start, long end, long fileSize, CancellationToken cancellationToken)
        {
            try
            {
                // 使用 Range 头请求从断点处继续下载
                using var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                request.Headers.Range = new RangeHeaderValue(start, end);

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var fileStream = new FileStream(destinationPath, FileMode.Open, FileAccess.Write);

                fileStream.Seek(start, SeekOrigin.Begin);

                // 使用固定大小缓冲区，避免内存问题
                var buffer = new byte[BufferSize];
                int bytesRead;
                long totalRead = 0;
                while ((bytesRead = await ReadWithSpeedLimit(contentStream, buffer, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    // 更新下载进度
                    totalRead += bytesRead;
                    ProgressChanged?.Invoke(start + totalRead, fileSize);
                    // 更新下载速度
                    var speed = bytesRead / 1024.0 / _stopWatch.Elapsed.TotalSeconds;
                    SpeedChanged?.Invoke(speed);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不视为错误
            }
            catch (Exception ex)
            {
                throw new Exception($"分片下载失败({start}-{end}): {ex.Message}", ex);
            }
            finally
            {
                _stopWatch.Stop();
            }
        }

        private async Task<int> ReadWithSpeedLimit(Stream stream, byte[] buffer, CancellationToken cancellationToken)
        {
            _stopWatch.Restart();
            var bytesRead = 0;
            if (_maxSpeedKBps <= 0)
            {
                // 不限速
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                return bytesRead;
            }

            // 计算本次读取的最大字节数
            var maxBytes = (int)(_maxSpeedKBps * 1024 * 0.1); // 每100ms的限额
            var readSize = Math.Min(buffer.Length, maxBytes);

            bytesRead = await stream.ReadAsync(buffer, 0, readSize, cancellationToken);

            // 速度控制
            if (bytesRead > 0 && _maxSpeedKBps > 0)
            {
                var expectedTime = (bytesRead / 1024.0) / _maxSpeedKBps * 1000; // 毫秒
                var actualTime = _stopWatch.ElapsedMilliseconds;
                var delayTime = (int)(expectedTime - actualTime);

                if (delayTime > 0)
                {
                    await Task.Delay(delayTime, cancellationToken);
                }
            }

            return bytesRead;
        }

        private static IEnumerable<(long Start, long End)> CalculateChunks(long fileSize, int threadCount)
        {
            var chunkSize = Math.Max(ChunkSize, fileSize / threadCount);
            for (long i = 0; i < fileSize; i += chunkSize + 1)
            {
                var end = Math.Min(i + chunkSize, fileSize - 1);
                yield return (i, end);
            }
        }


        private async Task<long> GetFileSizeAsync(string fileUrl)
        {
            using var request = new HttpRequestMessage(HttpMethod.Head, fileUrl);
            using var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            return response.Content.Headers.ContentLength ??
                   throw new Exception("无法获取文件大小");
        }
    }
}
