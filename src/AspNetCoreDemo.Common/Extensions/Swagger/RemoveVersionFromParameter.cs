using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions.Swagger
{
    /// <summary>
    /// 移除API版本参数
    /// </summary>
    public class RemoveVersionFromParameter : IOperationFilter
    {
        /// <summary>
        /// 扩展方法
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.FirstOrDefault(p => p.Name == "version");
            if (versionParameter != null)
            {
                operation.Parameters.Remove(versionParameter);
            }
        }
    }
}
