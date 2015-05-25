using System;
using Microsoft.Practices.Unity;

namespace AppPlatform.Core.EnterpriseLibrary.Unity
{
    /// <summary>
    /// Extends IUnityContainer with a couple of handy helpers
    /// </summary>
    public interface IExtendedUnityContainer : IUnityContainer
    {
        /// <summary>
        /// Registers a type with detailed constructor parameters specification
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="constructionInstructions">Values to be injected into constructor</param>
        /// <returns></returns>
        IExtendedUnityContainer ConfigureConstructor<T>(params object[] constructionInstructions);

        /// <summary>
        /// Configures the constructor for registration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurationName">Name of the configuration.</param>
        /// <param name="constructionInstructions">The construction instructions.</param>
        /// <returns></returns>
        IExtendedUnityContainer ConfigureConstructorForRegistration<T>(string configurationName, params object[] constructionInstructions);

        /// <summary>
        /// Registers a type with detailed constructor parameters specification
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="constructionInstructions">Values to be injected into constructor</param>
        /// <returns></returns>
        IExtendedUnityContainer ConfigureConstructorForConfiguration<T>(string configurationName, params object[] constructionInstructions);


        /// <summary>
        /// Registers a type with detailed initializing function arguments specification
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        IExtendedUnityContainer ConfigureInitialisation<T>(string functionName, params object[] arguments);

        /// <summary>
        /// Registers a type as a singleton (with a ContainerControlledLifetimeManager)
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <returns></returns>
        IExtendedUnityContainer RegisterTypeAsSingleton<S, T>() where T : S;

        /// <summary>
        /// Registers a type as a singleton (with a ContainerControlledLifetimeManager)
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation</typeparam>
        /// <param name="configurationName">Configuration name</param>
        /// <returns></returns>
        IExtendedUnityContainer RegisterTypeAsSingleton<S, T>(string configurationName) where T : S;

        /// <summary>
        /// Registers a type as a transient (every time new instance) type (with a TransientLifetimeManager)
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <returns></returns>
        IExtendedUnityContainer RegisterTransientType<S, T>() where T : S;

        /// <summary>
        /// Registers a type as a transient (every time new instance) type (with a TransientLifetimeManager )
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <param name="configurationName">Configuration name</param>
        /// <returns></returns>
        IExtendedUnityContainer RegisterTransientType<S, T>(string configurationName) where T : S; 
    }
}
