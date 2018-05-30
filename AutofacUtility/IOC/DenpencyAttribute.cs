using System;

namespace AutofacUtility
{
    /// <summary>
    /// 属性注入特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false,Inherited = false)]
    public class DenpencyAttribute:Attribute
    {
        /// <summary>
        /// 使用的名称
        /// </summary>
        public string Name { set; get; }
    }
}
