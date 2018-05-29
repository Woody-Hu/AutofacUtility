using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacUtility
{
    /// <summary>
    /// 名称类型特性接口
    /// </summary>
    interface INameAndCalssAttribute
    {
        /// <summary>
        /// 是否以类型注册
        /// </summary>
       bool IfByClass { set; get; }

        /// <summary>
        /// 名称
        /// </summary>
      string Name { set; get; }
    }
}
