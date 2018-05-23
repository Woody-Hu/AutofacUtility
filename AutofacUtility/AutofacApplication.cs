using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// Autofac应用 单例模式
    /// </summary>
    public class AutofacApplication
    {
        /// <summary>
        /// 单例模式标签
        /// </summary>
        private static AutofacApplication m_singleTag = null;

        /// <summary>
        /// 单例模式构造方法
        /// </summary>
        private AutofacApplication()
        {

        }

        /// <summary>
        /// 单例模式获取器 双重检查锁
        /// </summary>
        /// <returns></returns>
        public static AutofacApplication GetApplication()
        {
            if (null == m_singleTag)
            {
                lock (typeof(AutofacApplication))
                {
                    if (null == m_singleTag)
                    {
                        m_singleTag = new AutofacApplication();
                    }
                }
            }

            return m_singleTag;
        }
    }
}
