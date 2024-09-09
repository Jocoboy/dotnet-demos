using ABPDemo.Components;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Principal;

namespace ABPDemo.Web.Extensions
{
    /// <summary>
    /// 用户Token校验扩展
    /// </summary>
    public static class ApplicationBuilderTokenVerifyMiddlewareExtension
    {
        public static IApplicationBuilder UseTokenVerifyMiddleware(this IApplicationBuilder app, string schema = JwtBearerDefaults.AuthenticationScheme)
        {
            return app.Use(async (ctx, next) =>
            {
                if (ctx.User.Identity?.IsAuthenticated ?? false)
                {
                    var forbiddenUserCache = ctx.RequestServices.GetRequiredService<ForbiddenUserCache>();

                    var userId = ctx.User.FindUserId();

                    var isForbidden = await forbiddenUserCache.IsUserForbiddenAsync(userId.Value);

                    if (isForbidden) ctx.User = null;
                }

                await next();
            });
        }
    }
}
