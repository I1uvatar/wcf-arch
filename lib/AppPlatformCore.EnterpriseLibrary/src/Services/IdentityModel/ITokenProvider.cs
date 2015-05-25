using System.IdentityModel.Tokens;
using System.ServiceModel.Description;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    /// <summary>
    /// Interface of a service that provides tokens for a given endpoint.
    /// The tokens may be cached, reused or taken care of on some other way.
    /// Calling this methods does not necesserily trigger a remote call to STS.
    /// </summary>
    public interface ITokenProvider
    {
        SecurityToken GetToken(ServiceEndpoint endpoint);
        SecurityToken GetToken(ServiceEndpoint endpoint, ITokenCustomRequestProvider customRequestProvider);
    }
}
