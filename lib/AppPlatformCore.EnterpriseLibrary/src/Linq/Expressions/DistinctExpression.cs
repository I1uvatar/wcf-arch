using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace AppPlatform.Core.EnterpriseLibrary.Linq.Expressions
{
    public static class DistincExpression
    {
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, object> uniqueCheckerMethod)
        {
            return source.Distinct(new GenericComparer<T>(uniqueCheckerMethod));
        }

        class GenericComparer<T> : IEqualityComparer<T>
        {
            public GenericComparer(Func<T, object> uniqueCheckerMethod)
            {
                this._uniqueCheckerMethod = uniqueCheckerMethod;
            }

            private Func<T, object> _uniqueCheckerMethod;

            bool IEqualityComparer<T>.Equals(T x, T y)
            {
                return this._uniqueCheckerMethod(x).Equals(this._uniqueCheckerMethod(y));
            }

            int IEqualityComparer<T>.GetHashCode(T obj)
            {
                return this._uniqueCheckerMethod(obj).GetHashCode();
            }
        }
    }


}
