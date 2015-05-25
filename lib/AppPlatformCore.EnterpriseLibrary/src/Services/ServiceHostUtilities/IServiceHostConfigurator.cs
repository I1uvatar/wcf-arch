using System.ServiceModel;

namespace AppPlatform.Core.EnterpriseLibrary.Services.ServiceHostUtilities
{
    public interface IServiceHostConfigurator
    {
        void Configure(ServiceHost serviceHost);
    }
}