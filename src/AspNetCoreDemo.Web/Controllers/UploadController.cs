using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class UploadController: ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _uploadPath;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
            _uploadPath = Path.Combine(_env.WebRootPath, "uploads");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetUploadStatus(string fileId)
        {
            var uploadedChunks = new List<int>();

            if (Directory.Exists(_uploadPath))
            {
                var chunkFiles = Directory.GetFiles(_uploadPath, $"{fileId}_*");
                foreach (var chunkFile in chunkFiles)
                {
                    if (int.TryParse(Path.GetFileName(chunkFile).Split('_').Last(), out var chunkNumber))
                    {
                        uploadedChunks.Add(chunkNumber);
                    }
                }
            }

            return Ok(uploadedChunks);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadChunkAsync(CancellationToken cancellationToken)
        {
            var form = await Request.ReadFormAsync(cancellationToken);
            var fileId = form["fileId"].ToString();
            var chunkNumber = int.Parse(form["chunkNumber"].ToString());
            var totalChunks = int.Parse(form["totalChunks"].ToString());
            var fileName = form["fileName"].ToString();
            var chunk = form.Files["chunk"];

            // 确保上传目录存在
            Directory.CreateDirectory(_uploadPath);

            // 临时保存分片
            var chunkPath = Path.Combine(_uploadPath, $"{fileId}_{chunkNumber}");
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream, cancellationToken);
            }

            // 如果是最后一个分片，合并文件
            if (chunkNumber == totalChunks - 1)
            {
                await MergeChunksAsync(fileId, totalChunks, fileName, cancellationToken);
                return Ok(new { Message = "Upload complete", FileName = fileName });
            }

            return Ok(new { Message = "Chunk uploaded", ChunkNumber = chunkNumber });
        }

        private async Task MergeChunksAsync(string fileId, int totalChunks, string fileName, CancellationToken cancellationToken)
        {
            var finalPath = Path.Combine(_uploadPath, fileName);

            using (var finalStream = new FileStream(finalPath, FileMode.Create))
            {
                for (int i = 0; i < totalChunks; i++)
                {
                    var chunkPath = Path.Combine(_uploadPath, $"{fileId}_{i}");
                    using (var chunkStream = System.IO.File.OpenRead(chunkPath))
                    {
                        await chunkStream.CopyToAsync(finalStream, cancellationToken);
                    }
                    System.IO.File.Delete(chunkPath); // 合并后删除分片
                }
            }
        }
    }
}
