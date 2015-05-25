using System;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Patterns.Gof;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Utility;

namespace AppPlatform.Core.EnterpriseLibrary.Unity
{
    public class ExtendedUnityContainer : UnityContainer, IExtendedUnityContainer
    {
        private readonly IDecorator wrapper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrapper"></param>
        public ExtendedUnityContainer(IDecorator wrapper)
        {
            this.wrapper = wrapper;
        }

        public new void Dispose()
        {
            base.Dispose();
            if (this.wrapper != null && this.wrapper is IDisposable)
            {
                ((IDisposable)this.wrapper).Dispose();
            }
        }

        private T Wrap<T> (T target)
        {
            if (this.wrapper != null)
            {
                return this.wrapper.Decorate(target);
            }

            return target;
        }

        #region IExtendedUnityContainer

        /// <summary>
        /// Registers a type with detailed constructor parameters specification
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="constructionInstructions">Values to be injected into constructor</param>
        /// <returns></returns>
        public IExtendedUnityContainer ConfigureConstructor<T>(params object[] constructionInstructions)
        {
            object[] configuredValues = this.PrepareInjectionParameters(constructionInstructions);

            this.Configure<InjectedMembers>()
                .ConfigureInjectionFor<T>(
                    new InjectionConstructor(configuredValues)
                );

            return this;
        }

        /// <summary>
        /// Registers a type with detailed constructor parameters specification
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="configurationName">Name of the configuration.</param>
        /// <param name="constructionInstructions">Values to be injected into constructor</param>
        /// <returns></returns>
        public IExtendedUnityContainer ConfigureConstructorForRegistration<T>(string configurationName,  params object[] constructionInstructions)
        {
            object[] configuredValues = this.PrepareInjectionParameters(constructionInstructions);

            this.Configure<InjectedMembers>()
                .ConfigureInjectionFor<T>(
                    configurationName,
                    new InjectionConstructor(configuredValues)
                );

            return this;
        }

        /// <summary>
        /// Registers a type with detailed constructor parameters specification
        /// </summary>
        /// <typeparam name="T">Implementation type</typeparam>
        /// <param name="configurationName">Configuration name</param>
        /// <param name="constructionInstructions">Values to be injected into constructor</param>
        /// <returns></returns>
        public IExtendedUnityContainer ConfigureConstructorForConfiguration<T>(string configurationName, params object[] constructionInstructions)
        {
            object[] configuredValues = this.PrepareInjectionParameters(constructionInstructions);

            this.Configure<InjectedMembers>()
                .ConfigureInjectionFor<T>(
                    configurationName,
                    new InjectionConstructor(configuredValues)
                );

            return this;
        }


        /// <summary>
        /// Registers a type with detailed initializing function arguments specification
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="functionName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public IExtendedUnityContainer ConfigureInitialisation<T>(string functionName, params object[] arguments)
        {
            object[] configuredValues = this.PrepareInjectionParameters(arguments);

            this.Configure<InjectedMembers>()
                .ConfigureInjectionFor<T>(
                new InjectionMethod(functionName, configuredValues)
                );

            return this;
        }

        private object[] PrepareInjectionParameters(object[] constructionInstructions)
        {
            if (constructionInstructions == null || constructionInstructions.Length < 1)
            {
                return new object[0];
            }

            var configuredValues = new object[constructionInstructions.Length];
            for (int i = 0; i < constructionInstructions.Length; i++)
            {
                var configurationValue = constructionInstructions[i];
                if (configurationValue is Type)
                {
                    configurationValue = new ResolvedParameter(configurationValue as Type);
                }

                configuredValues[i] = configurationValue;
            }
            return configuredValues;
        }


        /// <summary>
        /// Registers a type as a singleton (with a ContainerControlledLifetimeManager)
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <returns></returns>
        public IExtendedUnityContainer RegisterTypeAsSingleton<S, T>() where T : S
        {
            this.RegisterType<S, T>(new ContainerControlledLifetimeManager());
            return this;
        }

        public IExtendedUnityContainer RegisterTypeAsSingleton<S, T>(string configurationName) where T : S
        {
            this.RegisterType<S, T>(configurationName, new ContainerControlledLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a type as a transient (every time new instance) type (with a TransientLifetimeManager)
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <returns></returns>
        public IExtendedUnityContainer RegisterTransientType<S, T>() where T : S
        {
            this.RegisterType<S, T>(new TransientLifetimeManager());
            return this;
        }

        /// <summary>
        /// Registers a type as a transient (every time new instance) type (with a TransientLifetimeManager )
        /// </summary>
        /// <typeparam name="S">Service</typeparam>
        /// <typeparam name="T">Implementation Type</typeparam>
        /// <param name="configurationName">Configuration name</param>
        /// <returns></returns>
        public IExtendedUnityContainer RegisterTransientType<S, T>(string configurationName) where T : S
        {
            this.RegisterType<S, T>(configurationName, new TransientLifetimeManager());
            return this;
        }

        #endregion

        #region UnityContainer

        /// <summary>
        /// Get an instance of the default requested type from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>
        /// <returns>
        /// The retrieved object.
        /// </returns>
        public new T Resolve<T>()
        {
            return this.Wrap(base.Resolve<T>());
        }

        /// <summary>
        ///Get an instance of the requested type with the given name from the container.
        /// </summary>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to get from the container.</typeparam>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <returns>
        /// The retrieved object.
        /// </returns>
        public new T Resolve<T>(string name)
        {
            return this.Wrap(base.Resolve<T>(name));
        }

        /// <summary>
        /// Get an instance of the requested type with the given name from the container.
        /// </summary>
        /// <param name="t"><see cref="T:System.Type" /> of object to get from the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <returns>
        /// The retrieved object.
        /// </returns>
        public new object Resolve(Type t, string name)
        {
            return this.Wrap(base.Resolve(t, name));
        }

        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="T:System.Type" /> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type requested.</typeparam>
        /// <returns>
        /// Set of objects of type <typeparamref name="T" />.
        /// </returns>
        public new IEnumerable<T> ResolveAll<T>()
        {
            IEnumerable<T> containedObjects = base.ResolveAll<T>();

            foreach (T containedObject in containedObjects)
            {
                yield return this.Wrap(containedObject);
            }
        }

        /// <summary>
        /// Return instances of all registered types requested.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful if you've registered multiple types with the same
        /// <see cref="T:System.Type" /> but different names.
        /// </para>
        /// <para>
        /// Be aware that this method does NOT return an instance for the default (unnamed) registration.
        /// </para>
        /// </remarks>
        /// <param name="t">The type requested.</param>
        /// <returns>
        /// Set of objects of type <paramref name="t" />.
        /// </returns>
        public new IEnumerable<object> ResolveAll(Type t)
        {
            foreach (object o in this.ResolveAllHeleper(t))
            {
                yield return this.Wrap(o);
            }
        }

        private IEnumerable<object> ResolveAllHeleper(Type t)
        {
            List<object> all = new List<object>(base.ResolveAll(t));
            return all;
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to perform injection on.</typeparam>
        /// <param name="existing">Instance to build up.</param>
        /// <returns>
        /// The resulting object. By default, this will be <paramref name="existing" />, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T" />).
        /// </returns>
        public new T BuildUp<T>(T existing)
        {
            return this.Wrap(base.BuildUp(existing));
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// </remarks>
        /// <typeparam name="T"><see cref="T:System.Type" /> of object to perform injection on.</typeparam>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <returns>
        /// The resulting object. By default, this will be <paramref name="existing" />, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <typeparamref name="T" />).
        /// </returns>
        public new T BuildUp<T>(T existing, string name)
        {
            return this.Wrap(base.BuildUp(existing, name));
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// <para>
        /// This overload uses the default registrations.
        /// </para>
        /// </remarks>
        /// <param name="t"><see cref="T:System.Type" /> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <returns>
        /// The resulting object. By default, this will be <paramref name="existing" />, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="t" />).
        /// </returns>
        public new object BuildUp(Type t, object existing)
        {
            return this.Wrap(base.BuildUp(t, existing));
        }

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para>
        /// </remarks>
        /// <param name="t"><see cref="T:System.Type" /> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <returns>
        /// The resulting object. By default, this will be <paramref name="existing" />, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="t" />).
        /// </returns>
        public new object BuildUp(Type t, object existing, string name)
        {
            return this.Wrap(base.BuildUp(t, existing, name));
        }        

        #endregion
    }
}
