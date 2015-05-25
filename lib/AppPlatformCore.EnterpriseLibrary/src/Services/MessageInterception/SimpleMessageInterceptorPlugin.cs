using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace AppPlatform.Core.EnterpriseLibrary.Services.MessageInterception
{
    /// <summary>
    /// Extension to WCF Service Model. It is used as an injector for simple message pre/post processor.
    /// </summary>
    public class SimpleMessageInterceptorPlugin : IEndpointBehavior, IServiceBehavior
    {
        private readonly List<Type> interceptorTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interceptorTypes">
        /// List of interceptor types.
        /// It is expected that every interceptor has a default constructor and implements IDispatchMessageInspector
        /// </param>
        public SimpleMessageInterceptorPlugin(List<Type> interceptorTypes)
        {
            this.interceptorTypes = interceptorTypes;
        }

        ///<summary>
        ///Implement to confirm that the endpoint meets some intended criteria.
        ///</summary>
        ///<param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {
            // No special endpoint criteria applicable
        }

        ///<summary>
        ///Implement to pass data at runtime to bindings to support custom behavior.
        ///</summary>
        ///<param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        ///<param name="endpoint">The endpoint to modify.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // No special parameters
        }

        ///<summary>
        ///Implements a modification or extension of the service across an endpoint.
        ///</summary>
        ///
        ///<param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        ///<param name="endpoint">The endpoint that exposes the contract.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (Type interceptorType in this.interceptorTypes)
            {
                var messageInterceptor = (IDispatchMessageInspector)Activator.CreateInstance(interceptorType);
                endpointDispatcher.DispatchRuntime.MessageInspectors.Add(messageInterceptor);
            }
        }

        ///<summary>
        ///Implements a modification or extension of the client across an endpoint.
        ///</summary>
        ///
        ///<param name="endpoint">The endpoint that is to be customized.</param>
        ///<param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (Type interceptorType in this.interceptorTypes)
            {
                var messageInterceptor = (IClientMessageInspector)Activator.CreateInstance(interceptorType);
                clientRuntime.MessageInspectors.Add(messageInterceptor);
            }
        }

        /// <summary>
        /// Provides the ability to inspect the service host and the service description to confirm that the service can run successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        { }

        /// <summary>
        /// Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        { }

        /// <summary>
        /// Provides the ability to change run-time property values or insert custom extension objects such as error handlers, message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatch in serviceHostBase.ChannelDispatchers)
            {
                foreach (var endpointDispatch in channelDispatch.Endpoints)
                {
                    foreach (var interceptorType in this.interceptorTypes)
                    {
                        var messageInterceptor = (IDispatchMessageInspector)Activator.CreateInstance(interceptorType);
                        endpointDispatch.DispatchRuntime.MessageInspectors.Add(messageInterceptor);
                    }
                }
            }
        }
    }
}
