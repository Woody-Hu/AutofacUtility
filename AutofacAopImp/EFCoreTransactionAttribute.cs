using AutofacMiddleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacAopImp
{
    /// <summary>
    /// 非参数型EF事务特性 需注册HttpContextAccessor
    /// </summary>
    public class EFCoreTransactionAttribute: AbstractInterceptorAttribute
    {
        private Type m_useType;

        public EFCoreTransactionAttribute(Type inputType)
        {
            m_useType = inputType;
        }

        /// <summary>
        /// 获得拦截器
        /// </summary>
        /// <returns></returns>
        public override IInvocationInterceptor CreatInterceptor()
        {
            return new EFCoreTransactionInterceptor(m_useType);
        }
    }
}
