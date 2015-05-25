using System;
using AppPlatform.Core.EnterpriseLibrary.Patterns.Gof;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;

namespace AppPlatform.Core.EnterpriseLibrary.PolicyInjectionExtension
{
    public class PolicyWrapDecorator : IDecorator
    {
        /// <summary>
        /// Wraps target into its own implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public T Decorate<T>(T target)
        {
            T wrapped = PolicyInjection.Wrap<T>(target);
            return wrapped;
        }
    }
}
