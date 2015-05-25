using System;
using System.Collections.Generic;

namespace AppPlatform.Core.EnterpriseLibrary.Types
{
    /// <summary>
    /// Pair
    /// </summary>
    [Serializable]
    public class Pair<T>
    {
        public const string FIRST = "First";
        public const string SECOND = "Second";

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public Pair(T first, T second)
        {
            First = first;
            Second = second;
        }

        public Pair()
        { }

        /// <summary>
        /// Gets or sets the first.
        /// </summary>
        /// <value>The first.</value>
        public T First { get; set; }

        /// <summary>
        /// Gets or sets the second.
        /// </summary>
        /// <value>The second.</value>
        public T Second { get; set; }
    }

    public class StringPairComparer : IEqualityComparer<Pair<string>>
    {
        public bool Equals(Pair<string> x, Pair<string> y)
        {
            return (x.First == y.First && x.Second == y.Second);
        }

        public int GetHashCode(Pair<string> obj)
        {
            return String.Concat(obj.First, obj.Second).GetHashCode();
        }
    }

    /// <summary>
    /// Pair
    /// </summary>
    public class Pair<T, U>
    {
        public const string FIRST = "First";
        public const string SECOND = "Second";

        /// <summary>
        /// Initializes a new instance of the <see cref="Pair&lt;T, U&gt;"/> class.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public Pair()
        { }

        /// <summary>
        /// Gets or sets the first.
        /// </summary>
        /// <value>The first.</value>
        public T First { get; set; }

        /// <summary>
        /// Gets or sets the second.
        /// </summary>
        /// <value>The second.</value>
        public U Second { get; set; }

    }
}
