using ABPDemo.EntityFrameworkCore;
using ABPDemo.Web.Extensions;
using ABPDemo.Web.Filters;
using ABPDemo.Web.Filters.StringTrim;
using ABPDemo.Web.Menus;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Data;
using System.IO;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;
using Volo.Abp.Timing;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Uow;

namespace ABPDemo.Web;

[DependsOn(
    typeof(ABPDemoHttpApiModule),
    typeof(ABPDemoApplicationModule),
    typeof(ABPDemoEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class ABPDemoWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureNavigationServices();
        ConfigureSwaggerServices(context.Services, hostingEnvironment);
        ConfigureMvcOptions();
        ConfigureIdentityOptions();
        ConfigureClockOptions();
        ConfigureExceptionHandlerOptions();
        ConfigureDataFilterOptions();
    }

    private void ConfigureDataFilterOptions()
    {
        Configure<AbpDataFilterOptions>(options =>
        {
            options.DefaultStates[typeof(ISoftDelete)] = new DataFilterState(isEnabled: false); // 禁用软删除过滤
        });
    }

    private void ConfigureExceptionHandlerOptions()
    {
        Configure<AbpExceptionHandlingOptions>(options =>
        {
            options.SendExceptionsDetailsToClients = false;
            options.SendStackTraceToClients = false;
        });
    }

    private void ConfigureUnitOfWorkOptions()
    {
        Configure<AbpUnitOfWorkOptions>(options =>
        {
            options.IsolationLevel = IsolationLevel.ReadCommitted; // 设置事务隔离级别
        });
    }

    private void ConfigureIdentityOptions()
    {
        Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequiredLength = 6;
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        });
    }

    private void ConfigureMvcOptions()
    {
        Configure<MvcOptions>(options =>
        {
            options.Conventions.Add(new DisableApiExplorerFilter());
            options.Filters.Add(new StringTrimFilter());
            options.Filters.AddService<PostgresSqlExceptionFilter>();
        });
    }

    private void ConfigureClockOptions()
    {
        // PGSQL issues with 5.1.2 (and earlier, due to Npgsql 6+) #11437
        // Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone', only UTC is supported.
        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        Configure<AbpAntiForgeryOptions>(options => options.AutoValidate = false);
        Configure<AuthOptions>(configuration.GetSection("AuthOptions"));
        var authOptions = configuration.GetSection("AuthOptions").Get<AuthOptions>();
        context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Secret)),
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    ClockSkew = TimeSpan.FromMinutes(0)
                };
            });
    }

    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ABPDemoWebModule>();
        });
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ABPDemoMenuContributor());
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services, IWebHostEnvironment hostingEnvironment)
    {
        if (!hostingEnvironment.IsDevelopment()) return;

        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ABPDemo API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ABPDemo.Domain.Shared.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ABPDemo.Application.Contracts.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ABPDemo.HttpApi.xml"));
                options.SchemaFilter<EnumSchemaFilter>();
            }
        );
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();
        app.UseTokenVerifyMiddleware();

        app.UseAbpExceptionHandling();
        app.UseAuthorization();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseAbpSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ABPDemo API");
            });
        }

        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }
}
