using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using static AspNetCoreDemo.Web.Controllers.UploadController;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class UploadController: ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadPath;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment env, ILogger<UploadController> logger)
        {
            _env = env;
            _uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            _logger = logger;
        }

        #region For Chunked/Resumable/Parallel UploadService Used
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult GetUploadStatus(string fileId)
        //{
        //    var uploadedChunks = new List<int>();

        //    if (Directory.Exists(_uploadPath))
        //    {
        //        var chunkFiles = Directory.GetFiles(_uploadPath, $"{fileId}_*");
        //        foreach (var chunkFile in chunkFiles)
        //        {
        //            if (int.TryParse(Path.GetFileName(chunkFile).Split('_').Last(), out var chunkNumber))
        //            {
        //                uploadedChunks.Add(chunkNumber);
        //            }
        //        }
        //    }

        //    return Ok(uploadedChunks);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //public async Task<IActionResult> UploadChunkAsync(CancellationToken cancellationToken)
        //{
        //    var form = await Request.ReadFormAsync(cancellationToken);
        //    var fileId = form["fileId"].ToString();
        //    var chunkNumber = int.Parse(form["chunkNumber"].ToString());
        //    var totalChunks = int.Parse(form["totalChunks"].ToString());
        //    var fileName = form["fileName"].ToString();
        //    var chunk = form.Files["chunk"];

        //    // 确保上传目录存在
        //    Directory.CreateDirectory(_uploadPath);

        //    // 临时保存分片
        //    var chunkPath = Path.Combine(_uploadPath, $"{fileId}_{chunkNumber}");
        //    using (var stream = new FileStream(chunkPath, FileMode.Create))
        //    {
        //        await chunk.CopyToAsync(stream, cancellationToken);
        //    }

        //    // 如果是最后一个分片，合并文件
        //    if (chunkNumber == totalChunks - 1)
        //    {
        //        await MergeChunksAsync(fileId, totalChunks, fileName, cancellationToken);
        //        return Ok(new { Message = "Upload complete", FileName = fileName });
        //    }

        //    return Ok(new { Message = "Chunk uploaded", ChunkNumber = chunkNumber });
        //}

        //private async Task MergeChunksAsync(string fileId, int totalChunks, string fileName, CancellationToken cancellationToken)
        //{
        //    var finalPath = Path.Combine(_uploadPath, fileName);

        //    using (var finalStream = new FileStream(finalPath, FileMode.Create))
        //    {
        //        for (int i = 0; i < totalChunks; i++)
        //        {
        //            var chunkPath = Path.Combine(_uploadPath, $"{fileId}_{i}");
        //            using (var chunkStream = System.IO.File.OpenRead(chunkPath))
        //            {
        //                await chunkStream.CopyToAsync(finalStream, cancellationToken);
        //            }
        //            System.IO.File.Delete(chunkPath); // 合并后删除分片
        //        }
        //    }
        //}
        #endregion

        #region For AdaptiveResumable UploadService Used

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok(DateTime.UtcNow);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetUploadStatus(string fileId)
        {
            var uploadDir = Path.Combine(_uploadPath, fileId);
            if (!Directory.Exists(uploadDir))
            {
                return Ok(new { UploadedChunks = Array.Empty<UploadedChunk>() });
            }

            var chunks = Directory.GetFiles(uploadDir, "chunk_*")
                .Select(f => new FileInfo(f))
                .Select(f => new UploadedChunk
                {
                    Offset = long.Parse(f.Name.Split('_')[1]),
                    Size = (int)f.Length
                })
                .OrderBy(c => c.Offset)
                .ToList();

            return Ok(new { UploadedChunks = chunks });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadChunkAsync(CancellationToken cancellationToken)
        {
            var form = await Request.ReadFormAsync(cancellationToken);
            var fileId = form["fileId"].ToString();
            var chunkIndex = int.Parse(form["chunkIndex"].ToString());
            var chunkOffset = int.Parse(form["chunkOffset"].ToString());
            var chunkSize = int.Parse(form["chunkSize"].ToString());
            var chunkHash = form["chunkHash"].ToString();
            var fileSize = int.Parse(form["fileSize"].ToString());
            var fileName = form["fileName"].ToString();
            var chunk = form.Files["chunk"];

            try
            {
                // 验证分片
                if (chunk == null || chunk.Length == 0)
                {
                    return BadRequest("无效的分片数据");
                }

                // 创建上传目录
                var uploadDir = Path.Combine(_uploadPath, fileId);
                Directory.CreateDirectory(uploadDir);

                // 保存分片
                var chunkPath = Path.Combine(uploadDir, $"chunk_{chunkOffset}");
                using (var stream = new FileStream(chunkPath, FileMode.Create))
                {
                    await chunk.CopyToAsync(stream, cancellationToken);
                }

                using var chunkStream = System.IO.File.OpenRead(chunkPath);
                var revievedChunkHash = GetFileHash(chunkStream, HashAlgorithmType.Md5);
                if (!chunkHash.Equals(revievedChunkHash))
                {
                    chunkStream.Close();
                    System.IO.File.Delete(chunkPath);
                    var errorMsg = $"分片完整性校验失败，原始分片哈希为{chunkHash}, 接收到的分片哈希为{revievedChunkHash}, 分片 {chunkIndex} 已被删除，请重新上传！";
                    _logger.LogError(errorMsg);
                    return BadRequest(errorMsg);
                }

                _logger.LogInformation($"已接收分片 {chunkIndex} (偏移: {chunkOffset}, 大小: {chunk.Length}, 剩余大小: {fileSize - chunkOffset})");

                // 检查是否完成
                if (IsUploadComplete(fileId, fileSize))
                {
                    _logger.LogInformation($"文件 {fileName} 所有分片已上传");
                    return Ok(new { Completed = true });
                }

                return Ok(new { Completed = false });
            }
            catch(OperationCanceledException ex)
            {
                // 客户端主动取消上传，最后一个分片可能未完整上传，需要进行删除，否则客户端断点续传时，在对分片信息的更新过程中会发生错误
                System.IO.File.Delete(Path.Combine(Path.Combine(_uploadPath, fileId), $"chunk_{chunkOffset}"));
                var errorMsg = $"分片上传失败:{ex.Message}, 分片 chunk_{chunkOffset} 已删除！";
                _logger.LogError(errorMsg);
                return BadRequest(errorMsg);
            }
            catch (Exception ex)
            {
                var errorMsg = $"分片上传失败:{ex.Message}";
                _logger.LogError(errorMsg);
                return BadRequest(errorMsg);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> MergeChunksAsync(MergeFile mergeFile, CancellationToken cancellationToken)
        {
            var uploadDir = Path.Combine(_uploadPath, mergeFile.FileId);
            var finalDir = Path.Combine(_uploadPath, "completed");
            var finalPath = Path.Combine(finalDir, mergeFile.FileName);
            if (!Directory.Exists(finalDir))
            {
                Directory.CreateDirectory(finalDir);
            }

            // 获取所有分片并按偏移量排序
            var chunkFiles = Directory.GetFiles(uploadDir, "chunk_*")
                .Select(f => new
                {
                    Path = f,
                    Offset = long.Parse(Path.GetFileName(f).Split('_')[1])
                })
                .OrderBy(x => x.Offset)
                .ToList();

            // 合并文件
            using (var finalStream = new FileStream(finalPath, FileMode.Create))
            {
                foreach (var chunk in chunkFiles)
                {
                    using (var chunkStream = System.IO.File.OpenRead(chunk.Path))
                    {
                        await chunkStream.CopyToAsync(finalStream, cancellationToken);
                    }
                    System.IO.File.Delete(chunk.Path);
                }
            }

            // 清理临时目录
            Directory.Delete(uploadDir);

            using var stream = System.IO.File.OpenRead(finalPath);
            var mergeFileId = GetFileHash(stream, HashAlgorithmType.Sha256);
            if (!mergeFile.FileId.Equals(mergeFileId))
            {
                stream.Close();
                System.IO.File.Delete(finalPath);
                var errorMsg = $"合并后的文件内容哈希不正确，文件可能已损坏，合并前内容哈希为{mergeFile.FileId}, 合并后内容哈希为{mergeFileId}, 合并后的文件已被删除，请重新上传！";
                _logger.LogError(errorMsg);
                return BadRequest(errorMsg);
            }

            _logger.LogInformation($"文件合并完成: {finalPath}");

            return Ok();
        }

        #region private methods
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

        private bool IsUploadComplete(string fileId, long fileSize)
        {
            var status = GetUploadStatus(fileId) as OkObjectResult;
            var data = status?.Value as dynamic;
            var chunks = data?.UploadedChunks as IEnumerable<UploadedChunk>;

            if (chunks == null || !chunks.Any())
            {
                return false;
            }

            // 获取文件总大小(最后一个分片的结束位置)
            // 请勿使用chunks.Max(c => c.Offset + c.Size), 该值在单线程环境中可以代表所有分片都已上传完毕, 但在多线程环境中是错误的
            long currentTotalSize = chunks.Sum(c => c.Size);

            // 检查是否覆盖了所有字节
            return currentTotalSize >= fileSize;
        }

        #endregion

        #region public class
        public class UploadedChunk
        {
            public long Offset { get; set; }
            public int Size { get; set; }
        }

        public class MergeFile
        {
            public string FileId { get; set; }
            public string FileName { get; set; }
        }
        #endregion
        #endregion
    }
}
