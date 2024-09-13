using Microsoft.AspNetCore.Http;
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
    /// Swagger文件上传扩展
    /// </summary>
    public class FileUploadFilter : IOperationFilter
    {
        /// <summary>
        /// Swagger扩展文件上传，展示选择文件
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            const string FileUploadContentType = "multipart/form-data";
            if (operation.RequestBody == null || !operation.RequestBody.Content.Any(x => x.Key.Equals(FileUploadContentType,
                StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }
            if (context.ApiDescription.ActionDescriptor.Parameters.Any(n => n.ParameterType == typeof(IFormCollection)))
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Description = "文件上传",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            FileUploadContentType,
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Required = new HashSet<string>{"file"},
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        {
                                            "file",
                                            new OpenApiSchema()
                                            {
                                                Type = "string",
                                                Format = "binary"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                };
            }
        }
    }
}
