using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 公共方法
    /// </summary>
    internal class UtilityMethod
    {
        /// <summary>
        /// 使用的属性注入特性
        /// </summary>
        private static Type m_useDenpencyAttributeType = typeof(DenpencyAttribute);

        /// <summary>
        /// 判断属性是否需要注入
        /// </summary>
        /// <param name="inputPropertyInfo">输入的属性信息</param>
        /// <param name="inputObj">输入的对象</param>
        /// <returns></returns>
        internal static bool PropertyDenpencyCheck(PropertyInfo inputPropertyInfo, object inputObj)
        {
            return null != inputPropertyInfo.GetCustomAttribute(m_useDenpencyAttributeType);
        }
    }
}
