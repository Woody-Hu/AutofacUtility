/*----------------------------------------------------------------
// Copyright (C) 2015 新鸿业科技有限公司
// 版权所有。 
// 自动化上下文持久层操作服务 - 参数无关EF事务拦截器
// 创建标识：胡迪 2018.07.03
//----------------------------------------------------------------*/
using Autofac;
using AutofacMiddleware;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacAopImp
{
    /// <summary>
    /// 使用的参数无关EF事务拦截器
    /// </summary>
    internal class EFCoreTransactionInterceptor: IInvocationInterceptor
    {
        private Type useResloveType = null;

        private static Type m_useBoolType = typeof(bool);

        private static Type m_useObjectType = typeof(object);

        internal EFCoreTransactionInterceptor(Type inputType)
        {
            useResloveType = inputType;
        }


        public void Interceptor(IInvocationContext inputContext)
        {

            //获得当前HttpContext
            var tempHttpContext = GolbalAutofacContainer.GetCurrentHttpContext();

            //判断是否可解析
            if (null == tempHttpContext || null == useResloveType)
            {
                inputContext.Proceed();
            }


            DbContext useDbContext = null;

            IEFTransactionTagService useTagService = null;


            //尝试解析服务若失败则直接执行
            try
            {
                useDbContext = tempHttpContext.RequestServices.GetService(useResloveType) as DbContext;
                useTagService = tempHttpContext.RequestServices.GetService(typeof(IEFTransactionTagService)) as IEFTransactionTagService;
            }
            catch (Exception)
            {
                inputContext.Proceed();
                //执行完返回
                return;
            }

            //若获取会话失败
            if (null == useDbContext)
            {
                inputContext.Proceed();
                //执行完返回
                return;
            }

            //是否使用过事务标签
            bool ifNeedTansaction = true;

            if (null != useTagService)
            {
                ifNeedTansaction = !useTagService.IfContextHasBeenStartTraction(useResloveType);
            }

            IDbContextTransaction tempTransaction = null;

            //尝试打开事务并提交
            try
            {
                //若需打开事务
                if (ifNeedTansaction)
                {
                    tempTransaction = useDbContext.Database.BeginTransaction();

                    if (null != useTagService)
                    {
                        useTagService.SetContextUseTraction(useResloveType);
                    }
                }

                inputContext.Proceed();
                AsyncMethod(useDbContext).Wait();

                //若需打开事务
                if (ifNeedTansaction)
                {
                    tempTransaction.Commit();
                }
            }
            //异常回滚
            catch (Exception)
            {
                if (null != tempTransaction && ifNeedTansaction)
                {
                    tempTransaction.Rollback();
                }

                //若返回值是bool类型的
                if (inputContext.Method.ReturnType == m_useBoolType)
                {
                    inputContext.ReturnValue = false;
                }
                //若返回值是Object类型的
                else if (m_useObjectType.IsAssignableFrom(inputContext.Method.ReturnType))
                {
                    inputContext.ReturnValue = null;
                }
            }
        }

        /// <summary>
        /// 内部NIO封装
        /// </summary>
        /// <param name="useDbContext"></param>
        /// <returns></returns>
        private static async Task AsyncMethod(DbContext useDbContext)
        {
            await useDbContext.SaveChangesAsync();
        }
    }
}