using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WPFDemo.FileUpload.Serivce
{
    public class ParallelUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUri;
        private const string UploadChunkApiUri = "/api/Upload/UploadChunk";
        private const string GetUploadStatusApiUri = "/api/Upload/GetUploadStatus";
        private const int ChunkSize = 1024 * 1024; // 1MB 每块
        private readonly int _maxParallelism;

        // 进度和状态事件
        public event Action<int> ProgressChanged; // 上传百分比
        public event Action<string> StatusChanged; // 状态消息
        public event Action<bool> UploadCompleted; // 完成状态

        public ParallelUploadService(string serviceUri, int maxParallelism = 4)
        {
            _serviceUri = serviceUri;
            _httpClient = new HttpClient();
            _maxParallelism = maxParallelism;
        }

        public async Task ParallelUploadFileAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                StatusChanged?.Invoke("正在准备上传...");

                var fileInfo = new FileInfo(filePath);
                var totalChunks = (int)Math.Ceiling((double)fileInfo.Length / ChunkSize);
                using var fileStream = File.OpenRead(filePath);
                var fileId = GetFileHash(fileStream, HashAlgorithmType.Sha256); // 基于文件内容生成唯一ID

                StatusChanged?.Invoke("正在检查已上传分片...");

                // 获取已上传的分片信息
                var uploadedChunks = await GetUploadedChunksAsync(fileId, cancellationToken);

                var chunksToUpload = Enumerable.Range(0, totalChunks).Where(chunk => !uploadedChunks.Contains(chunk)).ToList();

                var progressLock = new object();
                var uploadedCount = 0;
                var totalToUpload = chunksToUpload.Count;

                StatusChanged?.Invoke($"准备上传 {totalToUpload} 个分片...");

                var parallelOptions = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _maxParallelism,
                    CancellationToken = cancellationToken
                };

                await Parallel.ForEachAsync(chunksToUpload, parallelOptions, async (chunkNumber, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    fileStream.Seek(chunkNumber * ChunkSize, SeekOrigin.Begin);

                    var chunkData = new byte[ChunkSize];
                    var bytesRead = await fileStream.ReadAsync(chunkData, 0, ChunkSize, cancellationToken);

                    var actualChunkData = new byte[bytesRead];
                    Array.Copy(chunkData, actualChunkData, bytesRead);

                    StatusChanged?.Invoke($"正在上传分片 {chunkNumber + 1}/{totalChunks}");

                    await UploadChunkAsync(fileId, chunkNumber, totalChunks, fileInfo.Name, actualChunkData, cancellationToken);

                    // 使用锁保证进度更新的原子性
                    lock (progressLock)
                    {
                        uploadedCount++;
                        var progress = (int)((double)uploadedCount / totalToUpload * 100);
                        ProgressChanged?.Invoke(progress);
                    }
                });

                StatusChanged?.Invoke("上传完成！");
                UploadCompleted?.Invoke(true);
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

        private async Task<List<int>> GetUploadedChunksAsync(string fileId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_serviceUri}{GetUploadStatusApiUri}?fileId={fileId}", cancellationToken);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(cancellationToken);
                return JsonSerializer.Deserialize<List<int>>(json);
            }
            catch
            {
                return new List<int>();
            }
        }

        private async Task UploadChunkAsync(string fileId, int chunkNumber, int totalChunks, string fileName, byte[] chunkData, CancellationToken cancellationToken)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(fileId), "fileId" },
                { new StringContent(chunkNumber.ToString()), "chunkNumber" },
                { new StringContent(totalChunks.ToString()), "totalChunks" },
                { new StringContent(fileName), "fileName" },
                { new ByteArrayContent(chunkData), "chunk", "chunk.dat" }
            };

            var response = await _httpClient.PostAsync($"{_serviceUri}{UploadChunkApiUri}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
