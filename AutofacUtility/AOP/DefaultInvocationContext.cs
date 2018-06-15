using AutofacMiddleware;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 默认调度上下文
    /// </summary>
    internal class DefaultInvocationContext : IInvocationContext
    {
        #region 私有字段
        /// <summary>
        /// 核心调度上下文
        /// </summary>
        private IInvocation m_coreInvocation = null;

        /// <summary>
        /// 拦截器管道
        /// </summary>
        private List<IInvocationInterceptor> m_lstInterceptor = null;

        /// <summary>
        /// 使用的拦截器迭代器
        /// </summary>
        private List<IInvocationInterceptor>.Enumerator m_useEnumerator; 
        #endregion

        #region 代理接口

        public object[] Arguments => m_coreInvocation.Arguments;

        public Type[] GenericArguments => m_coreInvocation.GenericArguments;

        public object InvocationTarget => m_coreInvocation.InvocationTarget;

        public MethodInfo Method => m_coreInvocation.Method;

        public object ReturnValue { get => m_coreInvocation.ReturnValue; set => m_coreInvocation.ReturnValue = value; }

        public Type TargetType => m_coreInvocation.TargetType; 
        #endregion

        /// <summary>
        /// 构造上下文
        /// </summary>
        /// <param name="inputCoreInvocation"></param>
        /// <param name="inputLstInterceptor"></param>
        internal DefaultInvocationContext( IInvocation inputCoreInvocation,List<IInvocationInterceptor> inputLstInterceptor)
        {
            m_coreInvocation = inputCoreInvocation;
            m_lstInterceptor = inputLstInterceptor;
            //获得迭代器
            m_useEnumerator = m_lstInterceptor.GetEnumerator();
        }

        /// <summary>
        /// 执行管道
        /// </summary>
        public void Proceed()
        {
            //执行管道方法
            if (m_useEnumerator.MoveNext())
            {
                m_useEnumerator.Current.Interceptor(this);
            }
            //递归到底则调用核心方法
            else
            {
                m_coreInvocation.Proceed();
            }
        }
    }
}
