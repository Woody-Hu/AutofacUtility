using Autofac;
using System;

namespace AutofacMiddleware
{
    /// <summary>
    /// Autofac中间操作接口
    /// </summary>
    public interface IAutofacContainerPrepare
    {
        void Prepare(ContainerBuilder builder);
    }
}
