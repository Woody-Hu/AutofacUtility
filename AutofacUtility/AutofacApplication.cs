using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.AttributeFilters;
using Microsoft.Extensions.DependencyInjection;

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
        /// 使用的核心库
        /// </summary>
        private ContainerBuilder m_containerBuilder = null;

        /// <summary>
        /// 使用的核心容器
        /// </summary>
        private IContainer m_coreContainer = null;

        /// <summary>
        /// component特性
        /// </summary>
        private Type m_useCompentType = typeof(ComponentAttribute);

        /// <summary>
        /// 单例模式构造方法
        /// </summary>
        private AutofacApplication()
        {
            if (null != m_coreContainer)
            {
                m_coreContainer.Dispose();
            }
            m_containerBuilder = new ContainerBuilder();
        }

        /// <summary>
        /// 单例模式构造方法    
        /// </summary>
        /// <param name="services"></param>
        private AutofacApplication(IServiceCollection services)
        {
            if (null != m_coreContainer)
            {
                m_coreContainer.Dispose();
            }
            m_containerBuilder = new ContainerBuilder();
            //放置上层服务
            m_containerBuilder.Populate(services);


        }

        /// <summary>
        /// 单例模式获取器 双重检查锁
        /// </summary>
        /// <returns></returns>
        public static AutofacApplication GetApplication(IServiceCollection services = null)
        {
            if (null == m_singleTag)
            {
                lock (typeof(AutofacApplication))
                {
                    if (null == m_singleTag)
                    {
                        if (null == services)
                        {
                            m_singleTag = new AutofacApplication();
                        }
                        else
                        {
                            m_singleTag = new AutofacApplication(services);
                        }
                    }
                }
            }

            return m_singleTag;
        }

        /// <summary>
        /// 准备数据
        /// </summary>
        private void PrepareData()
        {
            //获取应用程序域中的程序集实例
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            //循环全部程序集
            foreach (var oneAssemblies in allAssemblies)
            {
                PrepareOneAssembly(oneAssemblies);
            }

        }

        /// <summary>
        /// 准备一个程序集
        /// </summary>
        /// <param name="inputAssembly"></param>
        private void PrepareOneAssembly(Assembly inputAssembly)
        {
            Type[] allTypes = null;

            try
            {
                allTypes = inputAssembly.GetTypes();
            }
            catch (Exception)
            {
                return;
            }

            //循环全部类型
            foreach (var oneType in allTypes)
            {
                RegiestComponent(oneType);
            }


        }

        /// <summary>
        /// 注册一个Component
        /// </summary>
        /// <param name="oneType"></param>
        private void RegiestComponent(Type oneType)
        {
            //获取Component特性
            if (null != oneType.GetCustomAttribute(m_useCompentType))
            {
                ComponentAttribute tempComponentAttribute = oneType.GetCustomAttribute(m_useCompentType) as ComponentAttribute;

                var tempBuilder = m_containerBuilder.RegisterType(oneType);

                //若是单例模式
                if (tempComponentAttribute.IfSingelton)
                {
                    tempBuilder = tempBuilder.SingleInstance();
                }

                //类型注册/接口注册
                if (tempComponentAttribute.IfByClass)
                {
                    tempBuilder = tempBuilder.AsSelf();
                }
                else
                {
                    tempBuilder = tempBuilder.AsImplementedInterfaces();
                }

                //设置注册名称
                if (!string.IsNullOrWhiteSpace(tempComponentAttribute.Name))
                {
                    //按类型注册
                    if (tempComponentAttribute.IfByClass)
                    {
                        tempBuilder = tempBuilder.Keyed(tempComponentAttribute.Name, oneType);
                    }
                    //按接口注册
                    else
                    {
                        foreach (var oneInterfaceType in oneType.GetInterfaces())
                        {
                            tempBuilder = tempBuilder.Keyed(tempComponentAttribute.Name, oneInterfaceType);
                        }
                    }
                }

                //key过滤
                tempBuilder.WithAttributeFiltering();
            }
        }
    }
}
