using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;

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
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreDemo API", Version = "v1" });

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
        }
    }
}
