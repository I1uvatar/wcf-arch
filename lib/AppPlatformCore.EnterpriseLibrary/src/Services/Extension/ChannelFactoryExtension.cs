using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using Microsoft.IdentityModel.Protocols.WSTrust;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Extension
{
    public static class ChannelFactoryExtension
    {
        public static T CreateChannelWithIssuedTokenAndActingAs<T>(this ChannelFactory<T> factory, SecurityToken issuedToken, SecurityToken actAs)
        {
            var parameters = new FederatedClientCredentialsParameters
            {
                ActAs = actAs,
                IssuedSecurityToken = issuedToken
            };

            return ChannelFactoryOperations.CreateChannelWithParameters(factory, parameters);
        }

        public static T CreateChannelWithIssuedTokenAndActingAs<T>(this ChannelFactory<T> factory, EndpointAddress address, SecurityToken issuedToken, SecurityToken actAs)
        {
            var parameters = new FederatedClientCredentialsParameters
                                 {
                                     ActAs = actAs,
                                     IssuedSecurityToken = issuedToken
                                 };

            return ChannelFactoryOperations.CreateChannelWithParameters(factory, address, parameters);
        }

        public static T CreateChannelWithIssuedTokenAndActingAs<T>(this ChannelFactory<T> factory, EndpointAddress address, Uri via, SecurityToken issuedToken, SecurityToken actAs)
        {
            var parameters = new FederatedClientCredentialsParameters
                                 {
                                     ActAs = actAs,
                                     IssuedSecurityToken = issuedToken
                                 };
            return ChannelFactoryOperations.CreateChannelWithParameters(factory, address, via, parameters);
        }
    }
}
