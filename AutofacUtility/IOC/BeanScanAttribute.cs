using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    /// <summary>
    /// Bean扫描特性
    /// </summary>
    public class BeanScanAttribute: Attribute
    {

    }
}
