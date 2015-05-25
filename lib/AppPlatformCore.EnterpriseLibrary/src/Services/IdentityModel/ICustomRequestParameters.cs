namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public interface ICustomRequestParameters
    {
        TokenRequestParamDescription GetParametersDescription(string parameterPrefix);
        TokenRequestParamDescription GetParametersDescription(string parameterPrefix, string localName, string namespaceUri);
        bool IsParameter(string localName);
        bool IsParameter(string parameterPrefix, string localName, string namespaceUri);
    }
}
