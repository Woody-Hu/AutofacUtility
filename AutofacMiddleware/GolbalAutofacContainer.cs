using Autofac;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutofacMiddleware
{
    /// <summary>
    /// 全局Autofac容器
    /// </summary>
    public class GolbalAutofacContainer
    {
        /// <summary>
        /// 使用的容器
        /// </summary>
        private static IContainer m_useContainer = null;

        /// <summary>
        /// 使用的读写锁
        /// </summary>
        private static ReaderWriterLockSlim m_useLocker = new ReaderWriterLockSlim();

        /// <summary>
        /// 读写Autofac容器(读写分离)
        /// </summary>
        public static IContainer UseContainer
        {   set
            {
                try
                {
                    m_useLocker.EnterWriteLock();

                    if (null == m_useContainer)
                    {
                        m_useContainer = value;
                    }
                }
                finally
                {
                    m_useLocker.ExitWriteLock();
                }
            }

            get
            {
                try
                {
                    m_useLocker.EnterReadLock();

                    return m_useContainer;

                }
                finally
                {
                    m_useLocker.ExitReadLock();
                }
                
            }
        }

        /// <summary>
        /// 获得当前的HttpContext
        /// </summary>
        /// <returns></returns>
        public static HttpContext GetCurrentHttpContext()
        {
            //获得全局Autofac应用
            var useAutofacContainer = GolbalAutofacContainer.UseContainer;

            //判断是否可解析
            if (null == useAutofacContainer)
            {
                return null;
            }

            IHttpContextAccessor tempHttpAccessor = null;

            try
            {
                tempHttpAccessor = useAutofacContainer.Resolve(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            }
            catch (Exception)
            {
                return null;
            }

            return tempHttpAccessor.HttpContext;
        }
    }
}
