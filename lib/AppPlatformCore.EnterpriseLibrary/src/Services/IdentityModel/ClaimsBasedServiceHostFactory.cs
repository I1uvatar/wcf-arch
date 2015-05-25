using System;
using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.IdentityModel.Tokens;
using IssuerNameRegistry = Microsoft.IdentityModel.Tokens.IssuerNameRegistry;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public class ClaimsBasedServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(
                                   string constructorString, Uri[] baseAddresses)
        {
            var host = base.CreateServiceHost(constructorString, baseAddresses);

            FederatedServiceCredentials.ConfigureServiceHost(host, new TrustedIssuerNameRegistry());
            return host;
        }

        protected override ServiceHost CreateServiceHost(
                                           Type serviceType, Uri[] baseAddresses)
        {
            var host = base.CreateServiceHost(serviceType, baseAddresses);

            FederatedServiceCredentials.ConfigureServiceHost(host, new TrustedIssuerNameRegistry());
            return host;
        }
    }

    public class ClaimsBasedServiceHost : ServiceHost
    {
        public ClaimsBasedServiceHost(object singletonInstance, params Uri[]
                           baseAddresses)
            : base(singletonInstance, baseAddresses)
        {
        }

        public ClaimsBasedServiceHost(Type serviceType, params 
                          Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }

        protected override void InitializeRuntime()
        {
            FederatedServiceCredentials.ConfigureServiceHost(this, new TrustedIssuerNameRegistry());
            base.InitializeRuntime();
        }
    }

    public class TrustedIssuerNameRegistry : IssuerNameRegistry
    {
        /// <summary>
        ///  Returns the issuer Name from the security token.
        /// </summary>
        /// <param name="securityToken">The security token that contains the STS's certificates.</param>
        /// <returns>The name of the issuer who signed the security token.</returns>
        public override string GetIssuerName(SecurityToken securityToken)
        {
            var x509Token = securityToken as X509SecurityToken;
            return  x509Token != null ? x509Token.Certificate.SubjectName.Name : null;
        }
    }

}
