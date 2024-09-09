using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.XPath;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.Web.Filters
{
    /// <summary>
    /// 过滤枚举类型
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        private readonly IXPathNavigatorCache _navigatorCache;

        public EnumSchemaFilter(IXPathNavigatorCache navigatorCache)
        {
            _navigatorCache = navigatorCache;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                var dllName = type.Assembly.GetName().Name;

                if (_navigatorCache.ContainsKey(dllName))
                {
                    var navigator = _navigatorCache[dllName];

                    var enumNames = type.GetEnumNames()
                        .Select(name =>
                        {
                            var value = (int)Enum.Parse(type, name);
                            var nodeName = XmlCommentsNodeNameHelper.GetMemberNameForFieldOrProperty(type.GetMember(name)[0]);
                            var node = navigator.SelectSingleNode($"/doc/members/member[@name='{nodeName}']/summary");
                            return node != null ? $"{value}-{XmlCommentsTextHelper.Humanize(node.InnerXml)}" : null;
                        })
                        .Where(x => x != null);

                    var pre = string.IsNullOrWhiteSpace(schema.Description) ? "" : $"{schema.Description}:";

                    schema.Description = $"{pre}{string.Join(",", enumNames)}";
                }
            }
        }
    }

    public interface IXPathNavigatorCache : IDictionary<string, XPathNavigator>
    {
    }

    public class XPathNavigatorCache : Dictionary<string, XPathNavigator>, IXPathNavigatorCache, ITransientDependency
    {
        private readonly SwaggerGenOptions _options;
        public XPathNavigatorCache(IOptions<SwaggerGenOptions> options)
        {
            _options = options.Value;
            LoadCache();
        }

        private void LoadCache()
        {
            var documents = _options.SchemaFilterDescriptors.SelectMany(x => x.Arguments.Where(a => a is XPathDocument).Cast<XPathDocument>());

            var navigators = documents.Select(x => x.CreateNavigator());

            foreach (var navigator in navigators)
            {
                this.Add(Path.GetFileNameWithoutExtension(navigator.BaseURI), navigator);
            }
        }
    }
}
