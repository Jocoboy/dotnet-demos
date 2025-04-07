using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WPFDemo.FileDownload.Service
{
    public class ResumableDownloadService
    {
        private readonly HttpClient _httpClient;

        // 进度和状态事件
        public event Action<int> ProgressChanged; // 上传百分比
        public event Action<string> StatusChanged; // 状态消息
        public event Action<bool> DownloadCompleted; // 完成状态

        public ResumableDownloadService()
        {
            _httpClient = new HttpClient();
        }

        public async Task DownloadWithResumeAsync(string fileUrl, string destDir, CancellationToken cancellationToken)
        {
            try
            {
                // 检查目标文件是否已存在部分下载内容
                long existingLength = 0;
                var destFile = fileUrl.Split("/").Last();
                var destinationPath = $"{destDir}\\{destFile}";
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                if (File.Exists(destinationPath))
                {
                    existingLength = new FileInfo(destinationPath).Length;
                }

                // 使用 Range 头请求从断点处继续下载
                using var request = new HttpRequestMessage(HttpMethod.Get, fileUrl);
                if (existingLength > 0)
                {
                    request.Headers.Range = new RangeHeaderValue(existingLength, null);
                }

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.RequestedRangeNotSatisfiable)
                    {
                        StatusChanged?.Invoke("下载完成！"); // Range头请求范围超出文件大小，视为下载完成
                        ProgressChanged?.Invoke(100);
                        DownloadCompleted?.Invoke(true);
                        return;
                    }
                }

                var fileTotalLength = response.Content.Headers.ContentLength;

                using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = new FileStream(destinationPath, existingLength > 0 ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None);
                var buffer = new byte[8192];
                int bytesRead;
                long totalRead = 0;

                while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    totalRead += bytesRead;

                    int progress = (int)((double)totalRead / fileTotalLength * 100);
                    ProgressChanged?.Invoke(progress);
                    StatusChanged?.Invoke($"当前上传进度: {progress}%");
                }

                StatusChanged?.Invoke("下载完成！");
                DownloadCompleted?.Invoke(true);
            }
            catch (OperationCanceledException)
            {
                StatusChanged?.Invoke("下载已取消");
                DownloadCompleted?.Invoke(false);
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"上传失败: {ex.Message}");
                DownloadCompleted?.Invoke(false);
            }
        }
    }
}
