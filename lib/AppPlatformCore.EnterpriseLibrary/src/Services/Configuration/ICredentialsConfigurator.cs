using System.ServiceModel;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Configuration
{
    public interface ICredentialsConfigurator
    {
        void SetupClientCredentials<IContract>(ClientBase<IContract> clientBase) where IContract : class;
    }
}
