using System;
using AppPlatform.Core.EnterpriseLibrary.Transactions;

namespace AppPlatform.Core.EnterpriseLibrary.Transactions
{
    public class TransactionalValueType<T> : Transactional<T>
    {
        public TransactionalValueType(T t) : base(t){}

        static TransactionalValueType()
        {
            Type type = typeof(T);
            if (!type.IsValueType && type != typeof(string))
            {
                throw new InvalidOperationException("The type " + type + " is not nullable");
            }
        }
        protected override T Clone(T t)
        {
            return t;
        }
    }
}