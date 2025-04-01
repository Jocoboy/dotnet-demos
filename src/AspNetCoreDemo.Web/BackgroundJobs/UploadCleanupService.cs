using AspNetCoreDemo.Service.IService.Background;

namespace AspNetCoreDemo.Web.BackgroundJobs
{
    public class UploadCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public UploadCleanupService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _services.CreateScope();
            //获取服务类
            var taskWorkService = scope.ServiceProvider.GetRequiredService<IWorkService>();
            //执行服务类的定时任务
            await taskWorkService.TaskWorkAsync(stoppingToken);
        }
    }

}
