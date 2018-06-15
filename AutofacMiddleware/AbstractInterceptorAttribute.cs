using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutofacMiddleware
{
    /// <summary>
    /// 抽象拦截器特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public abstract class AbstractInterceptorAttribute : Attribute
    {
        public abstract IInvocationInterceptor CreatInterceptor();
    }
}