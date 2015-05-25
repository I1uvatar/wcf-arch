using System;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public class TokenRequestParamDescription
    {
        public TokenRequestParamDescription(string name, string prefix, string namespaceUri, Type type)
        {
            Name = name;
            Prefix = prefix;
            NamespaceUri = namespaceUri;
            Type = type;
        }

        public string Name { get; private set; }
        public string Prefix{ get; private set; }
        public string NamespaceUri { get; private set; }
        public Type Type { get; private set; }
    }

    public class ParamName
    {
        public ParamName(string name, string prefix, string namespaceUri)
        {
            Name = name;
            Prefix = prefix;
            NamespaceUri = namespaceUri;
        }

        public string Name { get; private set; }
        public string Prefix { get; private set; }
        public string NamespaceUri { get; private set; }
    }

    [Serializable]
    public class TokenRequestParam
    {
        public TokenRequestParam() { }

        public TokenRequestParam(string name, string prefix, string namespaceUri, object value)
        {
            Value = value;
            Name = name;
            Prefix = prefix;
            NamespaceUri = namespaceUri;
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public string Prefix { get; set; }
        public string NamespaceUri { get; set; }
    }
}
