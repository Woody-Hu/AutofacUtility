using AutofacMiddleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace AutofacAopImp
{
    /// <summary>
    /// 参数型EF事务上下文拦截
    /// </summary>
    internal class EFCoreTransactionInterceptorWithParam : IInvocationInterceptor
    {
        public void Interceptor(IInvocationContext inputContext)
        {
            DbContext useDbcontext = null;

            //寻找Db上下文参数
            foreach (var oneArgument in inputContext.Arguments)
            {
                if (null != oneArgument && oneArgument is DbContext)
                {
                    useDbcontext = oneArgument as DbContext;
                }
            }

            //若没有上下文
            if (null == useDbcontext)
            {
                inputContext.Proceed();
            }

            IDbContextTransaction tempTransaction = null;

            try
            {
                //开启事务
                tempTransaction = useDbcontext.Database.BeginTransaction();

                //执行
                inputContext.Proceed();

                //事务提交
                tempTransaction.Commit();
            }
            catch (Exception)
            {
                //若事务发起失败
                if(null == tempTransaction)
                {
                    //执行
                    inputContext.Proceed();
                }
                else
                {
                    //事务回滚
                    tempTransaction.Rollback();
                }
            }

        }
    }
}
