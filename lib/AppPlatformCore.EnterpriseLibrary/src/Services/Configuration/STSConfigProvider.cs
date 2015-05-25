using System;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Services.CustomBindings;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Configuration
{
    public class STSConfigProvider
    {
        public static void GetSTSConfigurationForEndpoint(ServiceEndpoint endpoint, out Binding STSBinding, out EndpointAddress STSAddress)
        {
            Assert.ArgumentIsNotNull(endpoint, "Endpoint can't be null");

            var endpoints = ((ClientSection)ConfigurationManager.GetSection("system.serviceModel/client")).Endpoints.Cast<ChannelEndpointElement>();

            var foundEndpoint = endpoints.FirstOrDefault(ep => ep.Contract == endpoint.Contract.ContractType.FullName && ep.Address == endpoint.Address.Uri);

            //var STSFedBinding = this.BindingByTypeAndConfigurationName<>(foundEndpoint.Binding, foundEndpoint.BindingConfiguration);
            //var fedBinding = STSFedBinding as WS2007FederationHttpBinding;

            //var fedBinding = GetSTSFederationBinding(foundEndpoint);
            var fedBinding = endpoint.Binding is WS2007FederationHttpBinding
                             ? endpoint.Binding as WS2007FederationHttpBinding
                             : endpoint.Binding is Custom2007FederationBinding
                                   ? (endpoint.Binding as Custom2007FederationBinding).InnerWS2007FederationHttpBinding
                                   : null;

            Assert.IsNotNull(fedBinding, string.Format("STS binding for endpoint {0} must be of type {1}", endpoint.Name, typeof(WS2007FederationHttpBinding).FullName));

            STSBinding = fedBinding.Security.Message.IssuerBinding;
            STSAddress = fedBinding.Security.Message.IssuerAddress;
        }

        private static WS2007FederationHttpBinding GetSTSFederationBinding(ChannelEndpointElement endpoint)
        {
            if (endpoint == null)
            {
                return null;
            }

            switch (endpoint.Binding)
            {
                case "ws2007FederationHttpBinding":
                    return new WS2007FederationHttpBinding(endpoint.BindingConfiguration);
                case "custom2007FederationBinding":
                    return new Custom2007FederationBinding(endpoint.BindingConfiguration).InnerWS2007FederationHttpBinding;
                default:
                    throw new ApplicationException(string.Format("Binding type {0} is not supported as federation binding", endpoint.Binding));
            }
        }

        //private Binding BindingByTypeAndConfigurationName(string bindingType, string bindingConfigurationName)
        //{
        //    var bindingsSection = (BindingsSection)ConfigurationManager.GetSection("system.serviceModel/bindings");

        //    var bindingElem = bindingsSection[bindingType].ConfiguredBindings.ToList().Find(binding => binding.Name == bindingConfigurationName);
            
        //    return bindingElem != null ? bindingElem as Binding : null;
        //}
    }
}