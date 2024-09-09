using ABPDemo.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace ABPDemo.Web.Filters.StringTrim
{
    /// <summary>
    /// 过滤字符串空格
    /// </summary>
    public class StringTrimFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (ControllerParameterDescriptor parameter in context.ActionDescriptor.Parameters)
            {
                var hasValue = context.ActionArguments.ContainsKey(parameter.Name) && context.ActionArguments[parameter.Name] != null;
                var hasTrimAttribute = parameter.ParameterInfo.CustomAttributes.Where(x => x.AttributeType == typeof(StringTrimAttribute)).Any();
                if (hasValue && hasTrimAttribute)
                {
                    context.ActionArguments[parameter.Name] = StringTrimmer.TrimValue(context.ActionArguments[parameter.Name]);
                }
            }
            await next();
        }
    }
}
