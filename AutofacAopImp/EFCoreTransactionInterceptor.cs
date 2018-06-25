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

        internal EFCoreTransactionInterceptor(Type inputType)
        {
            useResloveType = inputType;
        }


        public void Interceptor(IInvocationContext inputContext)
        {

            //获得全局Autofac应用
            var useAutofacContainer = GolbalAutofacContainer.UseContainer;

            //判断是否可解析
            if (null == useAutofacContainer || null == useResloveType)
            {
                inputContext.Proceed();
            }

            

            DbContext useDbContext = null;

            //打开临时声明空间
            using (var tempContext = useAutofacContainer.BeginLifetimeScope())
            {

                //尝试解析服务若失败则直接执行
                if (!(tempContext.Resolve(typeof(IHttpContextAccessor)) is IHttpContextAccessor useAccessor))
                {
                    inputContext.Proceed();
                    //执行完返回
                    return;
                }

                //尝试解析服务若失败则直接执行
                try
                {
                    useDbContext = useAccessor.HttpContext.RequestServices.GetService(useResloveType) as DbContext;
                }
                catch (Exception)
                {
                    inputContext.Proceed();
                    //执行完返回
                    return;
                }

                //若会话失败
                if (null == useDbContext)
                {
                    inputContext.Proceed();
                    //执行完返回
                    return;
                }

                IDbContextTransaction tempTransaction = null;

                //尝试打开事务并提交
                try
                {
                    tempTransaction = useDbContext.Database.BeginTransaction();
                    inputContext.Proceed();
                    AsyncMethod(useDbContext).Wait();
                    tempTransaction.Commit();
                }
                //异常回滚
                catch (Exception)
                {
                    if (null != tempTransaction)
                    {
                        tempTransaction.Rollback();
                    }
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