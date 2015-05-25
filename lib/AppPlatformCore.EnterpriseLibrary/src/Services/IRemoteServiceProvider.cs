using System.ServiceModel.Channels;

namespace AppPlatform.Core.EnterpriseLibrary.Services
{
    /// <summary>
    /// Declares methods for retrieving service proxy
    /// </summary>
    public interface IRemoteServiceProvider
    {
        /// <summary>
        /// Gets the service by specifying contract type.
        /// </summary>
        /// <typeparam name="IContract">The type of the contract.</typeparam>
        /// <remarks>Can be used if only one endpoint for that contract type is defined in configuration</remarks>
        /// <returns></returns>
        IRemoteServiceProxy<IContract> GetService<IContract>()
            where IContract : class;
        /// <summary>
        /// Get service proxy by endpoint name
        /// </summary>
        /// <typeparam name="TService">Type of service</typeparam>
        /// <param name="endpointConfigurationName">Endpoint configuration name</param>
        /// <returns></returns>
        IRemoteServiceProxy<TService> GetServiceByEndPointName<TService>(string endpointConfigurationName) where TService : class;
        
        /// <summary>
        /// Get service proxy at defined mex address
        /// </summary>
        /// <typeparam name="TService">Type of service</typeparam>
        /// <param name="metadataExchangeEndpointAddress">EnAddress of mex endpoint</param>
        /// <returns></returns>
        IRemoteServiceProxy<TService> GetService<TService>(string metadataExchangeEndpointAddress) where TService : class;

        /// <summary>
        /// Get service proxy at defined mex address
        /// </summary>
        /// <typeparam name="TService">Type of service</typeparam>
        /// <param name="metadataExchangeEndpointAddress">EnAddress of mex endpoint</param>
        /// <param name="expectedBinding">The expected binding.</param>
        /// <returns></returns>
        IRemoteServiceProxy<TService> GetService<TService>(string metadataExchangeEndpointAddress, Binding expectedBinding) where TService : class;
    }
}