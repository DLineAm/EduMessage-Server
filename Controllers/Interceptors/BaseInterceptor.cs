using System;
using System.Diagnostics;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using SignalIRServerTest.Services;

namespace SignalIRServerTest.Controllers.Interceptors
{
    public class BaseInterceptor : IInterceptor
    {
        private readonly ILogger<BaseInterceptor> _logger;

        public BaseInterceptor(ILogger<BaseInterceptor> logger)
        {
            _logger = logger;
        }


        public void Intercept(IInvocation invocation)
        {
            var sw = new Stopwatch();
            _logger.LogInformation("Method " + invocation.Method.Name + " from " + invocation.TargetType.Name + " started");
            try
            {
                invocation.Proceed();
            }
            catch (Exception e)
            {
                _logger.LogError(StringDecorator.GetDecoratedLogString(e.GetType(), invocation.Method.Name));
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation("Method " + invocation.Method.Name + " took " + sw.ElapsedMilliseconds + " ms");
            }
        }
    }

    public interface IAboba
    {
        bool GetResult();
    }

    public class Aboba : IAboba
    {
        public bool GetResult()
        {
            return true;
        }
    }
}