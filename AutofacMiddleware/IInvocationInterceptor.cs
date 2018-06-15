using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacMiddleware
{
    /// <summary>
    /// 方法拦截器接口
    /// </summary>
    public interface IInvocationInterceptor
    {
        /// <summary>
        /// 方法拦截
        /// </summary>
        /// <param name="inputContext"></param>
        void Interceptor(IInvocationContext inputContext);
    }
}
