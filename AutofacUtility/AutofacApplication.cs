using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.AttributeFilters;
using AutofacMiddleware;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace AutofacUtility
{
    /// <summary>
    /// Autofac应用 单例模式
    /// </summary>
    public class AutofacApplication:IDisposable
    {
        #region 私有字段
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
        /// bean扫描特性
        /// </summary>
        private Type m_useBeanScanType = typeof(BeanScanAttribute);

        /// <summary>
        /// Bean特性
        /// </summary>
        private Type m_useBeanType = typeof(BeanAttribute);

        /// <summary>
        /// 使用的拦截器基类
        /// </summary>
        private Type m_useBaseInterceptor = typeof(BaseInterceptor);

        /// <summary>
        /// 使用的Aop特性类
        /// </summary>
        private Type m_useBaseInterceptorCreaterType = typeof(AbstractInterceptorAttribute);

        /// <summary>
        /// 使用的默认代理拦截设置
        /// </summary>
        private ProxyGenerationOptions m_useDefaultProxyOptions = new ProxyGenerationOptions(new DefaultProxyGenerationHook());

        #endregion

        #region 私有构造方法

        /// <summary>
        /// 单例模式构造方法
        /// </summary>
        private AutofacApplication()
        {
            m_containerBuilder = new ContainerBuilder();
            PrepareData();
        }

        /// <summary>
        /// 单例模式构造方法    
        /// </summary>
        /// <param name="services"></param>
        private AutofacApplication(IServiceCollection services)
        {
            m_containerBuilder = new ContainerBuilder();
            //放置上层服务
            m_containerBuilder.Populate(services);
            PrepareData();
        }

        /// <summary>
        /// Moduel形式的注册
        /// </summary>
        /// <param name="builder"></param>
        private AutofacApplication(ContainerBuilder builder)
        {
            m_containerBuilder = builder;
            PrepareData();
        }
        #endregion

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
        /// 获取临时Application
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static AutofacApplication PrepareApplication(ContainerBuilder builder)
        {
            return new AutofacApplication(builder);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (null != m_coreContainer)
            {
                m_coreContainer.Dispose();
            }
        }

        /// <summary>
        /// 获得容器
        /// </summary>
        /// <returns></returns>
        public IContainer GetContainer()
        {
            if (null == m_coreContainer)
            {
                m_coreContainer = m_containerBuilder.Build();
            }

            return m_coreContainer;
        }

        #region 私有方法
        /// <summary>
        /// 准备数据
        /// </summary>
        private void PrepareData()
        {
            //注册生成事件
            m_containerBuilder.RegisterBuildCallback(k => GolbalAutofacContainer.UseContainer = k);

            //准备拦截器基类
            PrepareBaseInterceptor();
            //获取应用程序域中的程序集实例
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            //循环全部程序集
            foreach (var oneAssemblies in allAssemblies)
            {
                PrepareOneAssembly(oneAssemblies);
            }

        }

        /// <summary>
        /// 准备基础类拦截器
        /// </summary>
        private void PrepareBaseInterceptor()
        {
            //注册基础AOP拦截器
            m_containerBuilder.Register(c => new BaseInterceptor());
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
                //注册Component
                RegiestComponent(oneType);

                //注册Bean
                RegestBeans(oneType);
            }


        }

        /// <summary>
        /// 注册Bean
        /// </summary>
        /// <param name="oneType"></param>
        private void RegestBeans(Type oneType)
        {
            //若有Bean特性
            if (null != oneType.GetCustomAttribute(m_useBeanScanType))
            {
                foreach (var oneMethod in oneType.GetMethods(BindingFlags.Static|BindingFlags.Public))
                {
                    //若符合要求
                    if (null != oneMethod.GetCustomAttribute(m_useBeanType) && 0 == oneMethod.GetParameters().Length)
                    {
                        object tempObj = null;

                        //创建Bean对象
                        try
                        {
                            tempObj = oneMethod.Invoke(null, null);
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        RegiestBean(tempObj, oneMethod.GetCustomAttribute(m_useBeanType) as INameAndCalssAttribute);
                    }
                }
            }
        }

        /// <summary>
        /// 注册Bean
        /// </summary>
        /// <param name="inputObj"></param>
        /// <param name="tempAttribute"></param>
        private void RegiestBean(object inputObj, INameAndCalssAttribute tempAttribute)
        {
            var tempBuilder = m_containerBuilder.RegisterInstance(inputObj);

            //按name与class注册
            tempBuilder = PrepareCalssAndName(inputObj.GetType(), tempAttribute, tempBuilder);
        }

        /// <summary>
        /// 注册一个Component
        /// </summary>
        /// <param name="oneType"></param>
        /// <param name="inputObj">注册的对象</param>
        private void RegiestComponent(Type oneType)
        {
            //获取Component特性
            if (null != oneType.GetCustomAttribute(m_useCompentType))
            {
                ComponentAttribute tempComponentAttribute = oneType.GetCustomAttribute(m_useCompentType) as ComponentAttribute;
                if (oneType.IsGenericType)
                {
                    RegiestComponentByGeneric(oneType, tempComponentAttribute);
                }
                else
                {
                    RegiestComponentByNoneGeneric(oneType, tempComponentAttribute);
                }


            }
        }

        /// <summary>
        /// 非泛型注册Component
        /// </summary>
        /// <param name="oneType"></param>
        /// <param name="tempComponentAttribute"></param>
        private void RegiestComponentByNoneGeneric(Type oneType, ComponentAttribute tempComponentAttribute)
        {
            var tempBuilder = m_containerBuilder.RegisterType(oneType);

            //设置生命周期
            tempBuilder = SetLifeScope(tempComponentAttribute, tempBuilder);

            tempBuilder = PrepareCalssAndName(oneType, tempComponentAttribute, tempBuilder);

            //后续行为
            ActionMethod(oneType, tempBuilder);
        }

        /// <summary>
        /// 泛型注册一个Component
        /// </summary>
        /// <param name="inputType"></param>
        /// <param name="tempComponentAttribute"></param>
        private void RegiestComponentByGeneric(Type inputType, ComponentAttribute tempComponentAttribute)
        {
            var tempBuilder = m_containerBuilder.RegisterGeneric(inputType);

            //设置生命周期
            tempBuilder = SetLifeScope(tempComponentAttribute, tempBuilder);

            tempBuilder = PrepareCalssAndName(inputType, tempComponentAttribute, tempBuilder);

            //后续行为
            ActionMethod(inputType, tempBuilder);
        }

        /// <summary>
        /// 后续拦截行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="inputType"></param>
        /// <param name="tempBuilder"></param>
        private void ActionMethod<T, X>(Type inputType, IRegistrationBuilder<object, T, X> tempBuilder)
            where T : ReflectionActivatorData
        {
            //获取激活后事件方法
            var tempAction = ExpressionUtility.GetActivedAction(inputType);

            if (null != tempAction)
            {
                //绑定解析方法
                tempBuilder.OnActivated(tempAction);
            }

            //key过滤
            tempBuilder.WithAttributeFiltering();

            //若类需要拦截
            if (IfTypeUseInterceptor(inputType))
            {
                //设置类型拦截
                tempBuilder.EnableInterfaceInterceptors(m_useDefaultProxyOptions).InterceptedBy(m_useBaseInterceptor);
            }

            //key过滤
            tempBuilder.WithAttributeFiltering();
        }

        /// <summary>
        /// 设置生命周期行为
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="tempComponentAttribute"></param>
        /// <param name="tempBuilder"></param>
        /// <returns></returns>
        private IRegistrationBuilder<object, T, X> SetLifeScope<T, X>
            (ComponentAttribute tempComponentAttribute, IRegistrationBuilder<object, T, X> tempBuilder)
            where T : ReflectionActivatorData
        {
            //设置生命周期
            switch (tempComponentAttribute.LifeScope)
            {
                //在.net core Request 用 InstancePerLifetimeScope http://docs.autofac.org/en/latest/integration/aspnetcore.html
                case LifeScopeKind.Request:
                    tempBuilder = tempBuilder.InstancePerLifetimeScope();
                    break;
                case LifeScopeKind.Singleton:
                    tempBuilder = tempBuilder.SingleInstance();
                    break;
                default:
                    break;
            }

            return tempBuilder;
        }

        /// <summary>
        /// 判断类是否拦截
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        private bool IfTypeUseInterceptor(Type inputType)
        {
            foreach (var oneInterface in inputType.GetInterfaces())
            {
                foreach (var oneMethod in oneInterface.GetMethods())
                {

                    //获得方法aop特性
                    var methodAttributes = oneMethod.GetCustomAttributes(m_useBaseInterceptorCreaterType, false);

                    if (methodAttributes.Length != 0)
                    {
                        return true;
                    }

                }
            }
            

            return false;
        }

        /// <summary>
        /// 注册类型与名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oneType"></param>
        /// <param name="tempAttribute"></param>
        /// <param name="tempBuilder"></param>
        /// <returns></returns>
        private IRegistrationBuilder<object, T, SingleRegistrationStyle> PrepareCalssAndName<T>
            (Type oneType, INameAndCalssAttribute tempAttribute, IRegistrationBuilder<object, T, SingleRegistrationStyle> tempBuilder)
            where T:IConcreteActivatorData
        {
            //类型注册/接口注册
            if (tempAttribute.IfByClass)
            {
                tempBuilder = tempBuilder.As(oneType);
            }
            else
            {
                tempBuilder = tempBuilder.AsImplementedInterfaces();
            }

            //设置注册名称
            if (!string.IsNullOrWhiteSpace(tempAttribute.Name))
            {
                //按类型注册
                if (tempAttribute.IfByClass)
                {
                    tempBuilder = tempBuilder.Keyed(tempAttribute.Name, oneType);
                }
                //按接口注册
                else
                {
                    foreach (var oneInterfaceType in oneType.GetInterfaces())
                    {
                        tempBuilder = tempBuilder.Keyed(tempAttribute.Name, oneInterfaceType);
                    }
                }
            }

            return tempBuilder;
        }

        /// <summary>
        /// 泛型注册类型与名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oneType"></param>
        /// <param name="tempAttribute"></param>
        /// <param name="tempBuilder"></param>
        /// <returns></returns>
        private IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> PrepareCalssAndName
           (Type oneType, INameAndCalssAttribute tempAttribute, IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> tempBuilder)
        {
            //类型注册/接口注册
            if (tempAttribute.IfByClass)
            {
                tempBuilder = tempBuilder.As(oneType);
            }
            else
            {
                foreach (var oneInterface in oneType.GetInterfaces())
                {
                    if (oneInterface.IsGenericType)
                    {
                        tempBuilder.As(oneInterface);
                    }
                }
            }

            //设置注册名称
            if (!string.IsNullOrWhiteSpace(tempAttribute.Name))
            {
                //按类型注册
                if (tempAttribute.IfByClass)
                {
                    tempBuilder = tempBuilder.Keyed(tempAttribute.Name, oneType);
                }
                //按接口注册
                else
                {
                    foreach (var oneInterfaceType in oneType.GetInterfaces())
                    {
                        tempBuilder = tempBuilder.Keyed(tempAttribute.Name, oneInterfaceType);
                    }
                }
            }

            return tempBuilder;
        }
        #endregion
    }
}
