using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacMiddleware
{
    /// <summary>
    /// 生命周期枚举
    /// </summary>
    public enum LifeScopeKind
    {
        /// <summary>
        /// 瞬态
        /// </summary>
        Transient = 0,
        /// <summary>
        /// 请求
        /// </summary>
        Request,
        /// <summary>
        /// 单例
        /// </summary>
        Singleton
    }
}
