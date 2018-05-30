using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.AttributeFilters;

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
            //var tempApp = AutofacApplication.GetApplication();

            //using (var tempContext = tempApp.GetContainer().BeginLifetimeScope())
            //{
            //    var tempB = tempContext.Resolve<B>();
            //    var tempC = tempContext.Resolve<C>();
            //}

            //ProxyGenerator generator = new ProxyGenerator();

            //var builder = new ContainerBuilder();
            //builder.RegisterType<A1>()
            //       .EnableClassInterceptors()
            //       .InterceptedBy(typeof(TempInterceptor));

            var testPacker = new TestPacker(new List<ITestInterceptor>() { new TempInterceptor1(), new TempInterceptor2() }, new CoreHanlder());

            testPacker.Process();

            Console.Read();
        }
    }

    public interface ITestPacker
    {
        void Process();
    }

    public class CoreHanlder
    {
        public void CoreMethod()
        {
            Console.WriteLine("Core");
        }
    }

    public interface ITestInterceptor
    {
        void Intercept(ITestPacker inputPacker);
    }

    public class TempInterceptor1 : ITestInterceptor
    {
        public void Intercept(ITestPacker invocation)
        {
            Console.WriteLine("1 start");
            invocation.Process();
            Console.WriteLine("1 end");

        }
    }


    public class TempInterceptor2 : ITestInterceptor
    {
        public void Intercept(ITestPacker invocation)
        {
            Console.WriteLine("2 start");
            invocation.Process();
            Console.WriteLine("2 end");

        }
    }


    public class TestPacker : ITestPacker
    {
        private List<ITestInterceptor> m_lstInterceptor;

        private CoreHanlder m_Hanlder;

        private List<ITestInterceptor>.Enumerator m_useenumerator;

        public TestPacker(List<ITestInterceptor> lstInputInterceptor, CoreHanlder inputHanlder)
        {
            m_lstInterceptor = lstInputInterceptor;

            m_Hanlder = inputHanlder;

            m_useenumerator = m_lstInterceptor.GetEnumerator();
        }

        public void Process()
        {
            if (m_useenumerator.MoveNext())
            {
                m_useenumerator.Current.Intercept(this);
            }
            else
            {
                m_Hanlder.CoreMethod();
            }
        }
    }





    public interface IA
    { }

    [Component(Name = "A",IfByClass = false)]
    public class A1 : IA
    { }

    [Component(Name = "B", IfByClass = false)]
    public class A2:IA
    { }

    [Component(IfByClass = true)]
    public class B
    {
        IA m_useA;

        public B([KeyFilter("A")] IA inputA)
        {
            m_useA = inputA;
        }
    }

    [Component(IfByClass = true)]
    public class C
    {
       [Denpency(Name = "B")]
       public IA UseA { set; get; }
    }


}
