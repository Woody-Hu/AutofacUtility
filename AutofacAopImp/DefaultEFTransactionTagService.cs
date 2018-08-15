using AutofacMiddleware;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace AutofacAopImp
{
    [Component(IfByClass =false,LifeScope = LifeScopeKind.Request)]
    public class DefaultEFTransactionTagService : IEFTransactionTagService
    {
        private ConcurrentDictionary<Type, bool> m_useDic = new ConcurrentDictionary<Type, bool>();

        public bool IfContextHasBeenStartTraction(Type inputType)
        {
            return m_useDic.ContainsKey(inputType);
        }

        public void SetContextUseTraction(Type inputType)
        {
            m_useDic.GetOrAdd(inputType, true);
        }
    }
}
