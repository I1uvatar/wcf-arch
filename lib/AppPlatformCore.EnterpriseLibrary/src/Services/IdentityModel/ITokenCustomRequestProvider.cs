using System.Collections.Generic;
using System.Xml;
using Microsoft.IdentityModel.Protocols.WSTrust;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public interface ITokenCustomRequestProvider
    {
        List<XmlElement> GetTokenRequestParametersAsXml();
        Dictionary<string, object> GetTokenRequestParameters();
        T GetRequestParameter<T>(string parameterName);

        WSTrust13RequestSerializer GetCustomRequestSerializer();
    }
}
