using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public class CustomRequestConfigHelper
    {
        public static void ConfigureWSTrustClient(WSTrustClient client, ITokenCustomRequestProvider requestProvider)
        {
            Assert.ArgumentIsNotNull(client, "WSTrustClient to configure can't be null");

            if (requestProvider == null)
            {
                return;
            }

            client.WSTrustRequestSerializer = requestProvider.GetCustomRequestSerializer();
        }

        public static void ConfigureFederationBinding(Binding binding, ITokenCustomRequestProvider requestProvider)
        {
            Assert.ArgumentIsNotNull(binding, "Binding to configure can't be null");

            if (requestProvider == null)
            {
                return;
            }

            var fedBinding = binding as WS2007FederationHttpBinding;

            if (fedBinding == null)
            {
                Debug.WriteLine("Using WsTrust configuration with non WS2007FederationHttpBinding");
                return;
            }

            var requestParam = requestProvider.GetTokenRequestParametersAsXml();

            if (!Safe.HasItems(requestParam))
            {
                return;
            }

            foreach (var param in requestParam)
            {
                fedBinding.Security.Message.TokenRequestParameters.Add(param);
            }
        }

        public static void ConfigureCustomTokenRequest(RequestSecurityToken request, ITokenCustomRequestProvider requestProvider)
        {
            Assert.ArgumentIsNotNull(request, "Request to configure can't be null");

            if (requestProvider == null)
            {
                return;
            }

            var properties = requestProvider.GetTokenRequestParameters();

            if (!Safe.HasItems(properties))
            {
                return;
            }

            foreach (var additionalProperty in properties)
            {
                request.Properties.Add(additionalProperty.Key, additionalProperty.Value);
            }
        }
    }
}
