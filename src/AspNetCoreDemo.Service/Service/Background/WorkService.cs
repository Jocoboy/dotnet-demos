using AspNetCoreDemo.Service.IService.Background;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCoreDemo.Service.Service.Background
{
    public class WorkService : IWorkService
    {
        private int executionCount = 0;
        private readonly ILogger<WorkService> _logger;
        private DateTime nextDateTime;
        private readonly IWebHostEnvironment _env;
        private readonly BackgroundJobOptions _backgroundJobOptions;

        public WorkService(ILogger<WorkService> logger, IWebHostEnvironment env, IOptions<BackgroundJobOptions> backgroundJobOptions)
        {
            _logger = logger;
            _env = env;
            _backgroundJobOptions = backgroundJobOptions.Value;
        }

        public async Task TaskWorkAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // 计算下一个时间节点
                var now = DateTime.Now;
                var firstDateTime = new DateTime(now.Year, now.Month, now.Day, _backgroundJobOptions.StartHour, _backgroundJobOptions.StartMinute, _backgroundJobOptions.StartSecond);
                if (executionCount == 0)
                {
                    nextDateTime = firstDateTime;
                }
                else
                {
                    nextDateTime = nextDateTime.AddMinutes(_backgroundJobOptions.IntervalMinute);
                }

                if (nextDateTime < now)
                {
                    var delay = nextDateTime.AddDays(1) - now;
                    await Task.Delay(delay, cancellationToken);
                }
                else
                {
                    var delay = nextDateTime - now;
                    await Task.Delay(delay, cancellationToken);
                     CleanupOldUploads(_backgroundJobOptions.CleanUpDaysAgo);

                    var count = Interlocked.Increment(ref executionCount);
                    _logger.LogInformation("已完成分片自动清理. 累计清理次数: {Count}", count);
                }

            }
        }

        private void CleanupOldUploads(int daysAgo)
        {
            var cutoff = DateTime.Now.AddDays(-daysAgo);
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            foreach (var dir in Directory.GetDirectories(uploadPath))
            {
                var dirName = dir.Split('\\').Last();
                if (!"completed".Equals(dirName) && Directory.GetCreationTime(dir) < cutoff)
                {
                    Directory.Delete(dir, true);
                }
            }
        }
    }

    public class BackgroundJobOptions
    {
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int StartSecond { get; set; }
        public int IntervalMinute { get; set; }
        public int CleanUpDaysAgo { get; set; }
    }
}
