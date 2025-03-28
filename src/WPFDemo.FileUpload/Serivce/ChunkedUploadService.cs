using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo.FileUpload.Serivce
{
    public class ChunkedUploadService
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUri;
        private const string UploadChunkApiUri = "/api/Upload/UploadChunk";
        private const int ChunkSize = 1024 * 1024; // 1MB 每块

        public ChunkedUploadService(string serviceUri)
        {
            _serviceUri = serviceUri;
            _httpClient = new HttpClient();
        }

        public async Task UploadInChunksAsync(string filePath, CancellationToken cancellationToken)
        {
            var fileInfo = new FileInfo(filePath);
            var totalChunks = (int)Math.Ceiling((double)fileInfo.Length / ChunkSize);
            var fileId = Guid.NewGuid().ToString(); // 唯一文件标识

            using var fileStream = File.OpenRead(filePath);

            for (int chunkNumber = 0; chunkNumber < totalChunks; chunkNumber++)
            {
                var chunkData = new byte[ChunkSize];
                var bytesRead = await fileStream.ReadAsync(chunkData, 0, ChunkSize, cancellationToken);

                if (bytesRead == 0) break;

                var actualChunkData = new byte[bytesRead]; // 只取实际读取的字节
                Array.Copy(chunkData, actualChunkData, bytesRead);

                await UploadChunkAsync(fileId, chunkNumber, totalChunks, fileInfo.Name, actualChunkData, cancellationToken);
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
