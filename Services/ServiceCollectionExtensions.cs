using System;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SignalIRServerTest.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInterceptedSingleton<TInterface, TImplementation, TInterceptor>(
            this IServiceCollection services)
            where TInterface : class
            where TImplementation : class, TInterface
            where TInterceptor : class, IInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.AddSingleton<TImplementation>();
            services.TryAddTransient<TInterceptor>();
            services.AddSingleton(provider =>
            {
                var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                var implementation = provider.GetRequiredService<TImplementation>();
                var interceptor = provider.GetRequiredService<TInterceptor>();
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation, interceptor);
            });
        }

        public static void AddInterceptedScoped<TClass, TInterceptor>(
            this IServiceCollection services)
            where TClass : class
            where TInterceptor : class, IInterceptor
        {
            services.TryAddSingleton<IProxyGenerator, ProxyGenerator>();
            services.TryAddTransient<TInterceptor>();
            services.AddScoped(provider =>
            {
                var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
                var implementation = Activator.CreateInstance<TClass>();
                var interceptor = provider.GetRequiredService<TInterceptor>();
                return proxyGenerator.CreateClassProxyWithTarget(implementation, interceptor);
            });
            //services.AddSingleton(provider =>
            //{
            //    var proxyGenerator = provider.GetRequiredService<IProxyGenerator>();
            //    var implementation = provider.GetRequiredService<TClass>();
            //    var interceptor = provider.GetRequiredService<TInterceptor>();
            //    return proxyGenerator.CreateClassProxyWithTarget(implementation, interceptor);
            //});
        }
    }
}