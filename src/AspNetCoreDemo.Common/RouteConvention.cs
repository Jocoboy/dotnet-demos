using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common
{
    /// <summary>
    /// 全局路由前缀配置
    /// </summary>
    public class RouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// 路由前缀变量
        /// </summary>
        private readonly AttributeRouteModel _centralPrefix;
        /// <summary>
        /// 调用时传入指定的路由前缀
        /// </summary>
        /// <param name="routeTemplateProvider"></param>
        public RouteConvention(IRouteTemplateProvider routeTemplateProvider)
        {
            _centralPrefix = new AttributeRouteModel(routeTemplateProvider);
        }

        /// <summary>
        /// Apply接口实现
        /// </summary>
        /// <param name="application"></param>
        public void Apply(ApplicationModel application)
        {
            // 遍历所有的controller
            foreach (var controller in application.Controllers)
            {
                // 已经标记RouteAttribute的Controller
                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_centralPrefix, selectorModel.AttributeRouteModel);
                    }
                }
                // 没有标记RouteAttribute的Controller
                var unmatchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel == null).ToList();
                if (unmatchedSelectors.Any())
                {
                    foreach (var selectorModel in unmatchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = _centralPrefix;
                    }
                }
            }
        }
    }
}
