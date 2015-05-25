using System;

namespace AppPlatform.Core.EnterpriseLibrary.Patterns.Gof
{
    /// <summary>
    /// Gof Decorator Pattern
    /// </summary>
    public interface IDecorator
    {
        /// <summary>
        /// Wraps target into its own implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        T Decorate<T>(T target);
    }
}