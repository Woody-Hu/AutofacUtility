using Autofac;
using Autofac.Core;
using AutofacMiddleware;
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
        #region 私有字段
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
        /// 使用的实例属性接口方法字符串
        /// </summary>
        private const string m_strInstance = "Instance";

        /// <summary>
        /// 使用的上下文属性接口方法字符串
        /// </summary>
        private const string m_strContext = "Context";

        /// <summary>
        /// 使用的解析方法字符串
        /// </summary>
        private const string m_strResolve = "Resolve";

        /// <summary>
        /// 使用的利用Key值解析的方法字符串
        /// </summary>
        private const string m_strResolveKeyed = "ResolveKeyed";

        /// <summary>
        /// 使用的Instance属性对象
        /// </summary>
        private static PropertyInfo m_useInstanceProperty;

        /// <summary>
        /// 使用的Context属性对象
        /// </summary>
        private static PropertyInfo m_useContextProperty;

        /// <summary>
        /// 使用的解析方法
        /// </summary>
        private static MethodInfo m_useResolveMethod;

        /// <summary>
        /// 使用的利用Key解析方法
        /// </summary>
        private static MethodInfo m_useResolveByKeyMethod; 
        #endregion

        /// <summary>
        /// 静态构造
        /// </summary>
        static ExpressionUtility()
        {
            //获取Instance属性
            m_useInstanceProperty = m_typeofEventArg.GetProperty(m_strInstance);

            //获取context属性
            m_useContextProperty = m_typeofEventArg.GetProperty(m_strContext);

            //获取解析方法
            m_useResolveMethod = m_useAutofacExtensionType.GetMethod(m_strResolve, new Type[] { typeof(IComponentContext), typeof(Type) });

            //获取按Key解析方法
            m_useResolveByKeyMethod = m_useAutofacExtensionType.GetMethod(m_strResolveKeyed, new Type[] { typeof(IComponentContext), typeof(object), typeof(Type) });

        }

        /// <summary>
        /// 获得激活后的属性注入事件
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        internal static Action<IActivatedEventArgs<object>> GetActivedAction(Type inputType)
        {

            //属性字典
            Dictionary<PropertyInfo, DenpencyAttribute> dicOfProperty = new Dictionary<PropertyInfo, DenpencyAttribute>();
            DenpencyAttribute tempAttribute = null;

            //获得需要处理的属性
            foreach (var oneProperty in inputType.GetProperties())
            {
                tempAttribute = oneProperty.GetCustomAttribute(m_typeofDepency) as DenpencyAttribute;
                if (!oneProperty.CanRead || !oneProperty.CanWrite || null == tempAttribute)
                {
                    continue;
                }
                else
                {
                    dicOfProperty.Add(oneProperty, tempAttribute);
                }
            }

            //若没有则直接返回
            if (0 == dicOfProperty.Count)
            {
                return null;
            }

            //输入参数
            ParameterExpression inputParameter = Expression.Parameter(m_typeofEventArg);

            //获得属性实例
            var propertyInstance = Expression.Property(inputParameter, m_useInstanceProperty);

            //类型转换
            var realUseInstance = Expression.TypeAs(propertyInstance, inputType);

            //获取容器对象
            var useContext = Expression.Property(inputParameter, m_useContextProperty);

            List<Expression> lstExpression = new List<Expression>();

            MethodCallExpression tempMethodExpression = null;

            MethodCallExpression tempExpressionOfReslove = null;

            PrepareProperty(dicOfProperty, ref tempAttribute, realUseInstance, useContext, lstExpression, ref tempMethodExpression, ref tempExpressionOfReslove);

            //主体表达式
            BlockExpression useBlockExpression = Expression.Block(lstExpression);

            //编译返回
            return Expression.Lambda<Action<IActivatedEventArgs<object>>>(useBlockExpression, inputParameter).Compile();
        }

        /// <summary>
        /// 准备属性表达式
        /// </summary>
        /// <param name="dicOfProperty"></param>
        /// <param name="tempAttribute"></param>
        /// <param name="realUseInstance"></param>
        /// <param name="useContext"></param>
        /// <param name="lstExpression"></param>
        /// <param name="tempMethodExpression"></param>
        /// <param name="tempExpressionOfReslove"></param>
        private static void PrepareProperty(Dictionary<PropertyInfo, DenpencyAttribute> dicOfProperty, ref DenpencyAttribute tempAttribute,
            UnaryExpression realUseInstance, MemberExpression useContext, List<Expression> lstExpression, 
            ref MethodCallExpression tempMethodExpression, ref MethodCallExpression tempExpressionOfReslove)
        {
            //获取属性
            foreach (var onePropertyKVP in dicOfProperty)
            {
                //获得属性信息对象
                var tempProperty = onePropertyKVP.Key;

                tempAttribute = onePropertyKVP.Value;

                //获取类型常量
                var tempExpressionOfType = Expression.Constant(tempProperty.PropertyType, typeof(Type));

                //属性表达式
                var tempPropertyExpression = Expression.Property(realUseInstance, onePropertyKVP.Key);

                //若没有Key依赖
                if (String.IsNullOrWhiteSpace(tempAttribute.Name))
                {
                    //解析结果
                    tempExpressionOfReslove = Expression.Call(m_useResolveMethod, useContext, tempExpressionOfType);

                }
                else
                {
                    //key表达式
                    var tempKeyValueExpression = Expression.Constant(tempAttribute.Name, typeof(object));

                    //解析结果
                    tempExpressionOfReslove = Expression.Call(m_useResolveByKeyMethod, useContext, tempKeyValueExpression, tempExpressionOfType);
                }

                //属性回设
                tempMethodExpression = Expression.Call(realUseInstance, tempProperty.SetMethod, Expression.TypeAs(tempExpressionOfReslove,tempProperty.PropertyType));

                //设置表达式体
                lstExpression.Add(tempMethodExpression);
            }
        }
    }
}
