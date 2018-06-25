using Autofac;
using AutofacMiddleware;
using Microsoft.AspNetCore.Http;
using System;

namespace AutofacMiddlewarePrepare
{
    /// <summary>
    /// HttpContextAccessor准备接口
    /// </summary>
    public class AutofacHttpContextAccessorPrepare : IAutofacContainerPrepare
    {
        public void Prepare(ContainerBuilder builder)
        {
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        }
    }
}
