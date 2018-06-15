using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.AttributeFilters;
using AutofacMiddleware;
using AutofacUtility;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;



namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var tempApp = AutofacApplication.GetApplication();

            using (var tempContext = tempApp.GetContainer().BeginLifetimeScope())
            {
                var tempA = tempContext.Resolve<IA>();
                var tempB = tempContext.Resolve<B>();
                tempB.TestMehtod();
                //var tempC = tempContext.Resolve<C>();
            }

            Console.Read();


        }
    }

    public class UseInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override IInvocationInterceptor CreatInterceptor()
        {
            return new UseInvocationInterceptor();
        }
    }

    public class UseInterceptor2Attribute : AbstractInterceptorAttribute
    {
        public override IInvocationInterceptor CreatInterceptor()
        {
            return new UseInvocationInterceptor2();
        }
    }

    public class UseInvocationInterceptor : IInvocationInterceptor
    {
        public void Interceptor(IInvocationContext inputContext)
        {
            Console.WriteLine("aa");

            inputContext.Proceed();

            Console.WriteLine("aa");

        }
    }

    public class UseInvocationInterceptor2 : IInvocationInterceptor
    {
        public void Interceptor(IInvocationContext inputContext)
        {
            Console.WriteLine("BB");

            inputContext.Proceed();

            Console.WriteLine("BB");

        }
    }



    public interface IA
    { }

    [Component(Name = "A", IfByClass = false)]
    public class A1 : IA
    { }

    [Component(Name = "B", IfByClass = false)]
    public class A2:IA
    { }

    public interface ITest
    {
        [UseInterceptor]
        void TestMehtod();
    }

    [Component(IfByClass = true)]
    public class B
    {
        [Denpency(Name = "A")]
        public IA UseA { set; get; }

        IA m_useA;

        //如果AOP的话构造注入用不了
        public B([KeyFilter("A")] IA inputA)
        {
            m_useA = inputA;
        }


        public virtual void TestMehtod()
        {
            Console.WriteLine("Core");
        }
    }



}
