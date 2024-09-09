using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.AspNetCore.Mvc.ApiExploring;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;

namespace ABPDemo.Web.Filters
{
    /// <summary>
    /// 过滤ABP自带的部分控制器
    /// </summary>
    public class DisableApiExplorerFilter : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            var filterTypes = new Type[] { typeof(AbpApplicationConfigurationController), typeof(AbpApiDefinitionController), typeof(AbpApplicationLocalizationController) };
            var filterApis = application.Controllers
                .Where(x => filterTypes.Contains(x.ControllerType))
                .ToList();
            application.Controllers.RemoveAll(filterApis);
        }
    }
}
