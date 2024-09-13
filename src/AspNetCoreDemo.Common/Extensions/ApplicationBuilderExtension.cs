using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// HttpContext.Current
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomHttpContext(this IApplicationBuilder app)
        {
            CustomHttpContext.Configure(app);
            return app;
        }
    }

    public class CustomHttpContext
    {
        /// <summary>
        /// Http上下文访问器
        /// </summary>
        private static IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// 当前上下文对象
        /// </summary>
        public static HttpContext Current { get { return _httpContextAccessor.HttpContext; } }

        /// <summary>
        /// Startup配置当前访问器
        /// </summary>
        /// <param name="app"></param>
        public static void Configure(IApplicationBuilder app)
        {
            _httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        }
    }
}
