using System;
using System.Xml;
using System.Globalization;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public class TokenRequestParameterBuilder
    {
        public static XmlElement GetParameterXml(TokenRequestParamDescription paramDescription, object value)
        {
            Assert.ArgumentIsNotNull(paramDescription, "Parameter description can't be null");

            var doc = new XmlDocument();
            
            var customElement = doc.CreateElement(
                paramDescription.Prefix, 
                paramDescription.Name,
                paramDescription.NamespaceUri
                );

            customElement.InnerText = Convert.ToString(value, CultureInfo.InvariantCulture);

            return customElement;
        }
    }
}
