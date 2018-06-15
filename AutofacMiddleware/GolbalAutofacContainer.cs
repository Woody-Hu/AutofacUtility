using Autofac;
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
    }
}
