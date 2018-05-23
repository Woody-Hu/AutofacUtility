using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 元素特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false, Inherited = false)]
    public class ComponentAttribute:Attribute
    {
        /// <summary>
        /// 是否以类型注册
        /// </summary>
        public bool IfByClass { set; get; }

        /// <summary>
        /// 是否已单例形式注册
        /// </summary>
        public bool IfSingelton { set; get; }
    }
}
