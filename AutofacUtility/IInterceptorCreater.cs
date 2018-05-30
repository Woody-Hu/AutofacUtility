using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 拦截器创建器
    /// </summary>
    public interface IInterceptorCreater
    {
        /// <summary>
        /// 创建一个拦截器
        /// </summary>
        /// <returns></returns>
        IInvocationInterceptor CreatInterceptor();
    }
}
