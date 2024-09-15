using AspNetCoreDemo.Model.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        /// <summary>
        /// 检查标签错误，返回首条错误
        /// </summary>
        /// <param name="ModelState"></param>
        /// <returns></returns>
        public static string GetErrorMessage(ModelStateDictionary ModelState)
        {
            List<string> errorList = new List<string>();
            //获取所有错误的Key
            List<string> Keys = ModelState.Keys.ToList();
            //获取每一个key对应的ModelStateDictionary
            foreach (var key in Keys)
            {
                var errors = ModelState[key].Errors.ToList();
                //将错误描述添加到sb中
                foreach (var error in errors)
                {
                    errorList.Add(error.ErrorMessage);
                }
            }
            return errorList[0];
        }
    }
}
