using Autofac;
using Autofac.Features.AttributeFilters;
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
            A tempa = new A();

            TestPacker<object> tempPacker = new TestPacker<object>();
            tempPacker.Instance = tempa;

            var tempAction = GetAction(typeof(A));

            tempAction(tempPacker);

            Console.Read();
        }

       
        public static Action<TestPacker<object>> GetAction(Type inputType)
        {
            ParameterExpression inputParameter = Expression.Parameter(typeof(TestPacker<object>));

            var propertyInstance = Expression.Property(inputParameter, "Instance");

            var realUseInstance = Expression.TypeAs(propertyInstance, inputType);

            List<Expression> lstExpersion = new List<Expression>();

            foreach (var oneProperty in inputType.GetProperties())
            {
                var tempProperty = Expression.Call(realUseInstance, oneProperty.SetMethod, Expression.Constant(5));
                lstExpersion.Add(tempProperty);
            }

            BlockExpression useBlockExpression = Expression.Block(lstExpersion);
            return Expression.Lambda<Action<TestPacker<object>>>(useBlockExpression, inputParameter).Compile();
        }
    }

    public class TestPacker<T>
    {
        public T Instance { set; get; }
    }

    public class A
    {
        public int TempValue { set; get; }
    }





}
