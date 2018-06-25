using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacMiddleware
{
    /// <summary>
    /// 元素特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ComponentAttribute : Attribute, INameAndCalssAttribute
    {
        /// <summary>
        /// 使用的名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 是否以类型注册
        /// </summary>
        public bool IfByClass { set; get; }

        /// <summary>
        /// 是否已单例形式注册
        /// </summary>
        public LifeScopeKind LifeScope { set; get; } = LifeScopeKind.Transient;
    }
}
