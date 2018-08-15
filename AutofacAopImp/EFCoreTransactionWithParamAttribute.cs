/*----------------------------------------------------------------
// Copyright (C) 2015 新鸿业科技有限公司
// 版权所有。 
// 自动化上下文持久层操作服务 - 参数型EF事务特性
// 创建标识：胡迪 2018.07.03
//----------------------------------------------------------------*/
using AutofacMiddleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacAopImp
{
    /// <summary>
    /// 参数型EF事务特性
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
