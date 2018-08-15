using System;
using System.Collections.Generic;
using System.Text;

namespace AutofacAopImp
{
    /// <summary>
    /// EF事务标签服务
    /// </summary>
    public interface IEFTransactionTagService
    {
        /// <summary>
        /// 判断一个上下文是否使用过事务
        /// </summary>
        /// <param name="inputType"></param>
        /// <returns></returns>
        bool IfContextHasBeenStartTraction(Type inputType);

        /// <summary>
        /// 设置一个上下文为事务使用状态
        /// </summary>
        /// <param name="inputType"></param>
        void SetContextUseTraction(Type inputType);
    }
}
