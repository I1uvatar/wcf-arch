using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Services.Configuration;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;
using SecurityTokenElement = Microsoft.IdentityModel.SecurityTokenService.SecurityTokenElement;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    /// <summary>
    /// Interface of a service that provides tokens for a given endpoint.
    /// Invoking these methods always results in a remote call to STS.
    /// </summary>
    public interface IRemoteTokenProvider
    {
        /// <summary>
        /// Gets a token from a remote STS
        /// </summary>
        /// <param name="relyingPartyEndpoint"></param>
        /// <param name="STSBinding"></param>
        /// <param name="STSAddress"></param>
        /// <param name="customRequestProvider"></param>
        /// <returns></returns>
        SecurityToken GetToken(EndpointAddress relyingPartyEndpoint, Binding STSBinding, EndpointAddress STSAddress, ITokenCustomRequestProvider customRequestProvider);
        
        /// <summary>
        /// Gets a token from a remote STS
        /// </summary>
        /// <param name="relyingPartyEndpoint"></param>
        /// <param name="STSBinding"></param>
        /// <param name="STSAddress"></param>
        /// <param name="customRequestProvider"></param>
        /// <param name="actAs"></param>
        /// <returns></returns>
        SecurityToken GetToken(EndpointAddress relyingPartyEndpoint, Binding STSBinding, EndpointAddress STSAddress, ITokenCustomRequestProvider customRequestProvider, SecurityToken actAs);
    }

    public class RemoteTokenProvider : IRemoteTokenProvider
    {
        private readonly ICredentialsConfigurator credentialsConfigurator;

        public RemoteTokenProvider() { }

        public RemoteTokenProvider(ICredentialsConfigurator credentialsConfigurator)
        {
            this.credentialsConfigurator = credentialsConfigurator;
        }

        public SecurityToken GetToken(EndpointAddress relyingPartyEndpoint, Binding STSBinding, EndpointAddress STSAddress, ITokenCustomRequestProvider customRequestProvider)
        {
            return this.GetToken(relyingPartyEndpoint, STSBinding, STSAddress, customRequestProvider, null);
        }

        public SecurityToken GetToken(EndpointAddress relyingPartyEndpoint, Binding STSBinding, EndpointAddress STSAddress, ITokenCustomRequestProvider customRequestProvider, SecurityToken actAs)
        {
            Assert.ArgumentIsNotNull(STSBinding, "STSBinding can't be null");
            Assert.ArgumentIsNotNull(STSAddress, "STSAddress can't be null");

            var trustClient = new WSTrustClient(STSBinding, STSAddress);

            if (customRequestProvider != null)
            {
                trustClient.WSTrustRequestSerializer = customRequestProvider.GetCustomRequestSerializer();
            }

            try
            {
                var rst = new RequestSecurityToken(WSTrust13Constants.RequestTypes.Issue)
                {
                    AppliesTo = relyingPartyEndpoint,
                    ActAs = actAs != null ? new SecurityTokenElement(actAs) : null
                };

                CustomRequestConfigHelper.ConfigureCustomTokenRequest(rst, customRequestProvider);

                if (this.credentialsConfigurator != null)
                {
                    this.credentialsConfigurator.SetupClientCredentials(trustClient);
                }

                return trustClient.Issue(rst);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                trustClient.Close();
            }
        }
    }
}
