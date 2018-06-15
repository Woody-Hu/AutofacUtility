using System;
using System.Collections.Generic;
using System.Text;
using AutofacMiddleware;
using Castle.DynamicProxy;

namespace AutofacUtility
{
    /// <summary>
    /// 拦截器基础类
    /// </summary>
    internal class BaseInterceptor : IInterceptor
    {
        /// <summary>
        /// 拦截器创造器类型
        /// </summary>
        private static Type m_useAttributeInterfaceType = typeof(AbstractInterceptorAttribute);

        /// <summary>
        /// 拦截器方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            //获取拦截方法
            var tempMethod = invocation.Method;

            List<IInvocationInterceptor> tempLst = new List<IInvocationInterceptor>();

            //获取拦截方法的特性表
            foreach (AbstractInterceptorAttribute oneCreater in tempMethod.GetCustomAttributes(m_useAttributeInterfaceType,false))
            {
                //获得拦截器
                var tempInterceptor = oneCreater.CreatInterceptor();
                tempLst.Add(tempInterceptor);
            }

            //若获取到方法拦截队列
            if (tempLst.Count > 0)
            {
                //迭代执行
                IInvocationContext useContext = new DefaultInvocationContext(invocation, tempLst);
                useContext.Proceed();
            }
            else
            {
                //直接执行
                invocation.Proceed();
            }

        }
    }
}
