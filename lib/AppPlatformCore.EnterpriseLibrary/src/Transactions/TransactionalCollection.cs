using System.Collections;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Transactions;

namespace AppPlatform.Core.EnterpriseLibrary.Transactions
{
    public abstract class TransactionalCollection<C,T> : Transactional<C>,IEnumerable<T> where C : IEnumerable<T>
    {
        public TransactionalCollection(C collection)
        {
            Value = collection;
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<T> enumerable = this;
            return enumerable.GetEnumerator();
        }
    }
}