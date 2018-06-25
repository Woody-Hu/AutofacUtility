using AutofacMiddleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacAopImp
{
    /// <summary>
    /// 利用参数开启EF事务AOP
    /// </summary>
    public class EFCoreTransactionWithParamAttribute : AbstractInterceptorAttribute
    {
        public override IInvocationInterceptor CreatInterceptor()
        {
            return _EFCoreTransactionWithParamAttribute.UseInterceptor;
        }

        /// <summary>
        /// 内部类实现单例
        /// </summary>
        private class _EFCoreTransactionWithParamAttribute
        {
            internal static IInvocationInterceptor UseInterceptor = new EFCoreTransactionInterceptorWithParam();
        }

    }
}
