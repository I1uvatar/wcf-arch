using System;
using Microsoft.Practices.Unity;

namespace AppPlatform.Core.EnterpriseLibrary.Unity
{
    /// <summary>
    /// Provider for the IUnityContainer implementation
    /// </summary>
    public static class IocProvider
    {
        private static IExtendedUnityContainer ioc;

        /// <summary>
        /// Returns inversion of control container
        /// </summary>
        public static IExtendedUnityContainer Container
        {
            get { return ioc; }
        }

        /// <summary>
        /// Initializes inversion of control container
        /// </summary>
        /// <param name="iocValue"></param>
        public static void Initialize(IExtendedUnityContainer iocValue)
        {
            ioc = iocValue;
        }
    }
}