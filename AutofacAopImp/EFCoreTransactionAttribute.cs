/*----------------------------------------------------------------
// Copyright (C) 2015 新鸿业科技有限公司
// 版权所有。 
// 自动化上下文持久层操作服务 - 非参数型EF事务特性
// 创建标识：胡迪 2018.07.03
//----------------------------------------------------------------*/
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
