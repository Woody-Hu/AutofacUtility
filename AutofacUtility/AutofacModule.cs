using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;

namespace AutofacUtility
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //获取临时module应用
            var tempAutoApplication = AutofacApplication.PrepareApplication(builder);
        }
    }
}
