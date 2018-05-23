using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// Bean特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BeanAttribute:Attribute
    {
        /// <summary>
        /// 是否以类型注册
        /// </summary>
        public bool IfByClass { set; get; }
    }
}
