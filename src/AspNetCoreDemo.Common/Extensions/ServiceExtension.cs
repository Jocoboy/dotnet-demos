using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AspNetCoreDemo.Common.Extensions
{
    /// <summary>
    /// builder.Services扩展方法，反射注册服务
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// 依赖注入仓储与服务
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection DIRegisterService(this IServiceCollection service)
        {
            service.RegisterAssembly("AspNetCoreDemo.Repository");
            service.RegisterAssembly("AspNetCoreDemo.Service");
            return service;
        }

        /// <summary>
        /// 用DI批量注入接口程序集中对应的实现类。
        /// <para>
        /// 需要注意的是，这里有如下约定：
        /// IUserService --> UserService, IUserRepository --> UserRepository.
        /// </para>
        /// </summary>
        /// <param name="service"></param>
        /// <param name="AssemblyName">实现程序集的名称（不包含文件扩展名）</param>
        /// <param name="serviceLifetime">生命周期</param>
        /// <returns></returns>
        public static IServiceCollection RegisterAssembly(this IServiceCollection service, string AssemblyName, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            // 获取接口程序集与实现程序集中的类
            Assembly Assembly = Assembly.Load(AssemblyName);
            Type[] implementTypes = Assembly.GetTypes()
                .Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && !t.GetTypeInfo().IsGenericType)
                .ToArray();
            Type[] interfaceTypes = Assembly.GetTypes()
            .Where(t => t.GetTypeInfo().IsInterface && !t.GetTypeInfo().IsGenericType)
            .ToArray();
            foreach (var type in interfaceTypes)
            {
                string implementTypeName = type.Name.Substring(1);
                Type implementType = implementTypes.SingleOrDefault(t => t.Name == implementTypeName);
                if (implementType != null && type.IsAssignableFrom(implementType))
                {
                    switch (serviceLifetime)
                    {
                        //根据生命周期选择注册依赖
                        case ServiceLifetime.Scoped: service.AddScoped(type, implementType); break;
                        case ServiceLifetime.Singleton: service.AddSingleton(type, implementType); break;
                        case ServiceLifetime.Transient: service.AddTransient(type, implementType); break;
                    }
                }
            }
            return service;
        }

    }
}
