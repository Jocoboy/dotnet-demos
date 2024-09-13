using AspNetCoreDemo.Model.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions
{
    /// <summary>
    /// 模型自动验证扩展类
    /// </summary>
    public static class ModelStateValidateExtension
    {
        /// <summary>
        /// 模型自动验证扩展方法
        /// </summary>
        /// <param name="builder"></param>
        public static void AddModelStateValidateExt(this WebApplicationBuilder builder)
        {
            #region 模型自动验证
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .Select(e => e.Value.Errors.First().ErrorMessage)
                    .ToList();
                    var str = string.Join("|", errors);
                    var res = ResultHelper<string>.GetResult(ErrorEnum.DataError, "", str);
                    return new BadRequestObjectResult(res);
                };
            });
            #endregion
        }
    }
}
