using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public interface ISTSConfigProvider
    {
        void GetSTSConfigurationForEndpoint(ServiceEndpoint endpoint, out Binding STSBinding, out EndpointAddress STSAddress);
    }
}
