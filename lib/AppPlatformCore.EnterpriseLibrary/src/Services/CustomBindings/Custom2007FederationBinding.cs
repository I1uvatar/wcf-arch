using System;
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Security.Tokens;
using System.ServiceModel;
using System.IdentityModel.Tokens;

namespace AppPlatform.Core.EnterpriseLibrary.Services.CustomBindings
{
    /// <summary>
    /// Custom binding, based on WS2007HttpBinding.
    /// Allows communication without security context.
    /// Allows soap or binary message encoding.
    /// </summary>
    public class Custom2007FederationBinding : CustomBinding
    {
        public bool EstablishSecurityContext { get; set; }

        private string wsFederationBindingConfiguration;

        public string WsFederationBindingConfiguration
        {
            get
            {
                return this.wsFederationBindingConfiguration;
            }
            set
            {
                this.wsFederationBindingConfiguration = value;
                this.PrepareInnerWS2007FederationHttpBinding();
            }
        }

        private void PrepareInnerWS2007FederationHttpBinding()
        {
            this.InnerWS2007FederationHttpBinding = this.PrepareBinding<WS2007FederationHttpBinding>(this.WsFederationBindingConfiguration);
        }

        public string Encoding { get; set; }

        public string ServiceCredentials { get; set; }

        public WS2007FederationHttpBinding InnerWS2007FederationHttpBinding { get; private set; }

        /// <summary>
        /// .ctor
        /// </summary>
        public Custom2007FederationBinding()
        {
            this.EstablishSecurityContext = true;
            this.Encoding = "soap";
        }

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="configurationName"></param>
        public Custom2007FederationBinding(string configurationName)
            : this()
        {
            this.ApplyConfiguration(configurationName);
        }

        private void ApplyConfiguration(string configurationName)
        {
            var bindings = ((BindingsSection)(ConfigurationManager.GetSection("system.serviceModel/bindings")));
            var section = (Custom2007FederationBindingCollectionElement)bindings["custom2007FederationBinding"];
            Custom2007FederationBindingElement element = section.Bindings[configurationName];
            if (element == null)
            {
                throw new ConfigurationErrorsException(string.Format("There is no binding named {0} at {1}.", configurationName, section.BindingName));
            }

            element.ApplyConfiguration(this);

            if (this.InnerWS2007FederationHttpBinding == null)
            {
                this.PrepareInnerWS2007FederationHttpBinding();
            }
        }

        /// <summary>
        /// Creates a collection with the binding elements for the binding.
        /// </summary>
        /// <returns></returns>
        public override BindingElementCollection CreateBindingElements()
        {
            return this.CreateBindingElements(this.EstablishSecurityContext, this.Encoding);
        }

        private BindingElementCollection CreateBindingElements(bool securityContext, string encoding)
        {
            var defaultElements = this.InnerWS2007FederationHttpBinding.CreateBindingElements();

            var securityElementPosition = defaultElements.IndexOf(defaultElements.Find<SymmetricSecurityBindingElement>());
            var defaultSecurityElement = (SymmetricSecurityBindingElement)defaultElements[securityElementPosition];

            BindingElement securityElement;
            switch (this.ServiceCredentials)
            {
                case "none":
                    securityElement = this.PrepareSecurityElementWithNoServiceCheck(securityContext, this.InnerWS2007FederationHttpBinding);
                    break;
                case "negotiate":
                    securityElement = this.PrepareSecurityElementWithNegotiation(securityContext, defaultSecurityElement);
                    break;
                case "certificate":
                    securityElement = this.PrepareSecurityElementWithServiceCertificate(securityContext, defaultSecurityElement);
                    break;
                default:
                    throw new ArgumentException("Wrong serviceCredentials value: " + this.ServiceCredentials);
            }

            var encodingElementPosition = defaultElements.IndexOf(defaultElements.Find<TextMessageEncodingBindingElement>());
            var encodingElement = this.PrepareEncodingElement(encoding, (TextMessageEncodingBindingElement)defaultElements[encodingElementPosition]);

            defaultElements[securityElementPosition] = securityElement;
            defaultElements[encodingElementPosition] = encodingElement;

            return defaultElements;
        }

        public override string Scheme
        {
            get
            {
                return this.InnerWS2007FederationHttpBinding != null 
                    ? this.InnerWS2007FederationHttpBinding.Scheme 
                    : new HttpTransportBindingElement().Scheme;
            }
        }

        private BindingElement PrepareEncodingElement(string encoding, TextMessageEncodingBindingElement textMessageEncoding)
        {
            if (encoding.Equals("soap"))
            {
                return textMessageEncoding;
            }
            if (encoding.Equals("binary"))
            {
                var tcpBinding = new NetTcpBinding(SecurityMode.Message)
                {
                    ReaderQuotas = textMessageEncoding.ReaderQuotas
                };
                var encodingElement = tcpBinding.CreateBindingElements().Find<BinaryMessageEncodingBindingElement>();                
                return encodingElement;
            }

            throw new InvalidOperationException("Unknown encoding: " + encoding ?? string.Empty);
        }

        private BindingElement PrepareSecurityElementWithNegotiation(bool securityContext, SymmetricSecurityBindingElement element)
        {
            if (securityContext)
            {
                return element;
            }

            var wsBinding = new WS2007HttpBinding();
            wsBinding.Security.Mode = SecurityMode.Message;
            wsBinding.Security.Message.ClientCredentialType = MessageCredentialType.IssuedToken;
            wsBinding.Security.Message.EstablishSecurityContext = false;
            var wsSecurityElement = wsBinding.CreateBindingElements().Find<SymmetricSecurityBindingElement>();

            wsSecurityElement.EndpointSupportingTokenParameters.Endorsing[0] =
                ((SecureConversationSecurityTokenParameters)element.ProtectionTokenParameters)
                .BootstrapSecurityBindingElement
                .EndpointSupportingTokenParameters
                .Endorsing[0];

            return wsSecurityElement;
        }


        private BindingElement PrepareSecurityElementWithServiceCertificate(bool securityContext, SymmetricSecurityBindingElement element)
        {
            if (securityContext)
            {
                return element;
            }

            var foo = (SecureConversationSecurityTokenParameters)element.ProtectionTokenParameters;
            return foo.BootstrapSecurityBindingElement;
        }


        private BindingElement PrepareSecurityElementWithNoServiceCheck(bool securityContext, WSFederationHttpBinding federatedBinding)
        {
            if (securityContext)
            {
                return federatedBinding.CreateBindingElements().Find<SymmetricSecurityBindingElement>();
            }

            var param = new IssuedSecurityTokenParameters(
                @"http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1",
                federatedBinding.Security.Message.IssuerAddress,
                federatedBinding.Security.Message.IssuerBinding);

            param.IssuerMetadataAddress = federatedBinding.Security.Message.IssuerMetadataAddress;
            param.DefaultMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;            
            param.InclusionMode = SecurityTokenInclusionMode.AlwaysToRecipient;
            param.KeyType = SecurityKeyType.SymmetricKey;

            var noServiceSecurity = SecurityBindingElement.CreateIssuedTokenBindingElement(param);
            return noServiceSecurity;
        }

        private T PrepareBinding<T>(string configurationName) where T : class, new()
        {
            return !string.IsNullOrEmpty(configurationName) ? typeof(T).GetConstructor(new[] { typeof(string) }).Invoke(new[] { configurationName }) as T : new T();
        }
    }
}
