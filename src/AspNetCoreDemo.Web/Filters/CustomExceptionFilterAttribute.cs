using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreDemo.Common;
using AspNetCoreDemo.Model.Enums;

namespace AspNetCoreDemo.Web.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;
        public CustomExceptionFilterAttribute(IModelMetadataProvider modelMetadataProvider, ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                _logger.LogError(filterContext.Exception.Message + Environment.NewLine + filterContext.Exception.StackTrace);

                string msg = "获取接口信息时发生了错误，请刷新页面后重试";
                if (filterContext.Exception is CustomException)
                {
                    msg = filterContext.Exception.Message;
                }

                filterContext.Result = new JsonResult(ResultHelper<string>.GetResult(ErrorEnum.DataError, null, msg));

                filterContext.ExceptionHandled = true;
            }
        }
    }
}
