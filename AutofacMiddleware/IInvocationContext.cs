using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AutofacMiddleware
{
    /// <summary>
    /// 使用的上下文仿写接口
    /// </summary>
    public interface IInvocationContext
    {
        //
        // 摘要:
        //     Gets the arguments that the Castle.DynamicProxy.IInvocation.Method has been invoked
        //     with.
        object[] Arguments { get; }

        //
        // 摘要:
        //     Gets the generic arguments of the method.
        Type[] GenericArguments { get; }

        //
        // 摘要:
        //     Gets the object on which the invocation is performed. This is different from
        //     proxy object because most of the time this will be the proxy target object.
        object InvocationTarget { get; }

        //
        // 摘要:
        //     Gets the System.Reflection.MethodInfo representing the method being invoked on
        //     the proxy.
        MethodInfo Method { get; }

        //
        // 摘要:
        //     Gets or sets the return value of the method.
        object ReturnValue { get; set; }

        //
        // 摘要:
        //     Gets the type of the target object for the intercepted method.
        Type TargetType { get; }

        //
        // 摘要:
        //     Proceeds the call to the next interceptor in line, and ultimately to the target
        //     method.
        //
        // 备注:
        //     Since interface proxies without a target don't have the target implementation
        //     to proceed to, it is important, that the last interceptor does not call this
        //     method, otherwise a System.NotImplementedException will be thrown.
        void Proceed();
    }
}
