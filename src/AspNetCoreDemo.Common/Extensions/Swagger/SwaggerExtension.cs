using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AspNetCoreDemo.Common.Extensions.Swagger
{
    /// <summary>
    /// Swagger扩展
    /// </summary>
    public static class SwaggerExtension
    {
        /// <summary>
        /// 添加扩展方法
        /// </summary>
        /// <param name="builder"></param>
        public static void AddSwaggerGenExt(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                #region 自定义Api分版本展示配置项
                typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
                {
                    options.SwaggerDoc(version, new OpenApiInfo()
                    {
                        Title = $"{version} Api文档",
                        Version = version,
                        Description = $"Api版本{version}"
                    });
                });
                #endregion

                #region 通过组件支持Api分版本展示
                // 根据API版本信息生成API文档
                var provider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Contact = new OpenApiContact()
                        {
                            Name = "Jocoboy",
                            Email = "Jocoboy@outlook.com"
                        },
                        Description = "WebApi 文档",
                        Title = "WebApi 文档",
                        Version = description.ApiVersion.ToString()
                    });
                }
                // 在Swagger文档显示的API地址中将版本信息参数替换为实际的版本号
                options.DocInclusionPredicate((version, apiDescription) =>
                {
                    if (!version.Equals(apiDescription.GroupName))
                        return false;
                    var values = apiDescription.RelativePath!.Split("/").Select(
                        v => v.Replace("v{version}", apiDescription.GroupName));
                    apiDescription.RelativePath = string.Join("/", values);
                    return true;
                });
                // 参数使用驼峰命名法
                options.DescribeAllParametersInCamelCase();
                // 取消API文档的版本信息参数传入
                options.OperationFilter<RemoveVersionFromParameter>();
                #endregion

                #region Api注释展示配置项
                // 显示控制器注释
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AspNetCoreDemo.Web.xml"), true);
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "AspNetCoreDemo.Model.xml"), true);
                //options.OrderActionsBy(o => o.RelativePath);
                #endregion

                #region JWT Token配置项
                // 添加安全定义
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "请输入token，格式为Bearer xxxxxxx（注意中间有空格）",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // 添加安全要求
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                        {
                            new OpenApiSecurityScheme{
                                Reference = new OpenApiReference {
                                            Type = ReferenceType.SecurityScheme,
                                            Id = "Bearer"}
                           },new string[] { }
                        }
                 });
                #endregion

                #region 文件上传配置项
                options.OperationFilter<FileUploadFilter>();
                #endregion
            });

            #region 添加API版本控制支持
            builder.Services.AddApiVersioning(options =>
            {
                // 是否在响应的header信息中返回API版本信息
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // 未指定API版本时，设置API版本为默认的版本
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            #endregion

            #region 配置API版本信息
            builder.Services.AddVersionedApiExplorer(options =>
            {
                // API版本分组名称
                options.GroupNameFormat = "'v'VVVV";
                // 未指定API版本时，设置API版本为默认的版本
                options.AssumeDefaultVersionWhenUnspecified = true;
            });
            #endregion
        }

        /// <summary>
        /// 使用扩展方法
        /// </summary>
        /// <param name="app"></param>
        public static void UseSwaggerExt(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                #region 自定义Api分版本展示配置项
                foreach (string version in typeof(ApiVersions).GetEnumNames())
                {
                    options.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"版本：{version}");
                }
                #endregion

                #region 调用第三方程序包支持版本控制
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"api {description.GroupName.ToUpperInvariant()}");
                }
                #endregion
            });
        }
    }

    /// <summary>
    /// 版本枚举
    /// </summary>
    public enum ApiVersions
    {
        /// <summary>
        /// 版本一
        /// </summary>
        V1,
        /// <summary>
        /// 版本二
        /// </summary>
        V2,
        /// <summary>
        /// 版本三
        /// </summary>
        V3
    }
}
