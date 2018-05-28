using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 表达式树工具
    /// </summary>
    internal class ExpressionUtility
    {
        /// <summary>
        /// 使用的Actived事件对象
        /// </summary>
        private static Type m_typeofEventArg = typeof(IActivatedEventArgs<object>);

        /// <summary>
        /// 属性注入特性
        /// </summary>
        private static Type m_typeofDepency = typeof(DenpencyAttribute);

        /// <summary>
        /// 获得扩展方法类型
        /// </summary>
        private static Type m_useAutofacExtensionType = typeof(ResolutionExtensions);

        /// <summary>
        /// 使用的实例属性接口
        /// </summary>
        private const string m_strInstance = "Instance";

        /// <summary>
        /// 使用的上下文属性接口
        /// </summary>
        private const string m_strContext = "Context";

        /// <summary>
        /// 使用的Instance属性对象
        /// </summary>
        private static PropertyInfo m_useInstanceProperty;

        /// <summary>
        /// 使用的Context属性对象
        /// </summary>
        private static PropertyInfo m_useContextProperty;

        /// <summary>
        /// 静态构造
        /// </summary>
        static ExpressionUtility()
        {
            //获取Instance属性
            m_useInstanceProperty = m_typeofEventArg.GetProperty(m_strInstance);

            //获取context属性
            m_useContextProperty = m_typeofEventArg.GetProperty(m_strContext);
        }

        /// <summary>
        /// 获得激活后的属性注入事件
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        internal static Action<IActivatedEventArgs<object>> GetActivedAction(Type inputType)
        {
            List<PropertyInfo> lstProperty = new List<PropertyInfo>();

            //获得需要处理的属性
            foreach (var oneProperty in inputType.GetProperties())
            {
                if (!oneProperty.CanRead || !oneProperty.CanWrite|| null == oneProperty.GetCustomAttribute(m_typeofDepency))
                {
                    continue;
                }
                else
                {
                    lstProperty.Add(oneProperty);
                }
            }

            //若没有则直接返回
            if (0 == lstProperty.Count)
            {
                return null;
            }

            //输入参数
            ParameterExpression inputParameter = Expression.Parameter(m_typeofEventArg);

            //获得属性实例
            var propertyInstance = Expression.Property(inputParameter, m_useInstanceProperty);

            //类型转换
            var realUseInstance = Expression.TypeAs(propertyInstance, inputType);

            List<Expression> lstExpression = new List<Expression>();

            foreach (var oneProperty in lstProperty)
            {
                var tempProperty = Expression.Property(realUseInstance, oneProperty);
            }

            //主体表达式
            BlockExpression useBlockExpression = Expression.Block(lstExpression);

            //编译返回
            return Expression.Lambda<Action<IActivatedEventArgs<object>>>(useBlockExpression, inputParameter).Compile();
        }
    }
}
