//using Microsoft.Data.Sqlite;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Storage;
//using Microsoft.Extensions.DependencyInjection;
//using Volo.Abp;
//using Volo.Abp.EntityFrameworkCore;
//using Volo.Abp.EntityFrameworkCore.Sqlite;
//using Volo.Abp.Modularity;
//using Volo.Abp.PermissionManagement;
//using Volo.Abp.SettingManagement;
//using Volo.Abp.Uow;

//namespace ABPDemo.EntityFrameworkCore;

//[DependsOn(
//    typeof(ABPDemoApplicationTestModule),
//    typeof(ABPDemoEntityFrameworkCoreModule),
//    typeof(AbpEntityFrameworkCoreSqliteModule)
//    )]
//public class ABPDemoEntityFrameworkCoreTestModule : AbpModule
//{
//    private SqliteConnection? _sqliteConnection;

//    public override void ConfigureServices(ServiceConfigurationContext context)
//    {
//        Configure<FeatureManagementOptions>(options =>
//        {
//            options.SaveStaticFeaturesToDatabase = false;
//            options.IsDynamicFeatureStoreEnabled = false;
//        });
//        Configure<PermissionManagementOptions>(options =>
//        {
//            options.SaveStaticPermissionsToDatabase = false;
//            options.IsDynamicPermissionStoreEnabled = false;
//        });
//        Configure<SettingManagementOptions>(options =>
//        {
//            options.SaveStaticSettingsToDatabase = false;
//            options.IsDynamicSettingStoreEnabled = false;
//        });
//        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

//        ConfigureInMemorySqlite(context.Services);
//    }

//    private void ConfigureInMemorySqlite(IServiceCollection services)
//    {
//        _sqliteConnection = CreateDatabaseAndGetConnection();

//        services.Configure<AbpDbContextOptions>(options =>
//        {
//            options.Configure(context =>
//            {
//                context.DbContextOptions.UseSqlite(_sqliteConnection);
//            });
//        });
//    }

//    public override void OnApplicationShutdown(ApplicationShutdownContext context)
//    {
//        _sqliteConnection?.Dispose();
//    }

//    private static SqliteConnection CreateDatabaseAndGetConnection()
//    {
//        var connection = new AbpUnitTestSqliteConnection("Data Source=:memory:");
//        connection.Open();

//        var options = new DbContextOptionsBuilder<ABPDemoDbContext>()
//            .UseSqlite(connection)
//            .Options;

//        using (var context = new ABPDemoDbContext(options))
//        {
//            context.GetService<IRelationalDatabaseCreator>().CreateTables();
//        }

//        return connection;
//    }
//}
