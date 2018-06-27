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


            //尝试解析服务若失败则直接执行
            try
            {
                useDbContext = tempHttpContext.RequestServices.GetService(useResloveType) as DbContext;
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