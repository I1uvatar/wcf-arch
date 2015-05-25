using System;
using System.Globalization;
using System.Xml;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.WSTrust;
using Microsoft.IdentityModel.SecurityTokenService;

namespace AppPlatform.Core.EnterpriseLibrary.Services.IdentityModel
{
    public class HealthRecordTokenRequestSerializer : WSTrust13RequestSerializer
    {
        private readonly ICustomRequestParameters requestParameters;
        private readonly ITokenRequestLogger logger;

        public HealthRecordTokenRequestSerializer(ICustomRequestParameters requestParameters, ITokenRequestLogger logger)
        {
            this.requestParameters = requestParameters;
            this.logger = logger;
        }

        /// <summary>
        /// Override ReadXml method to deserialize the custom element inside the RST.
        /// </summary>
        /// <param name="reader">The xml reader to read from</param>
        /// <param name="rst">The rst object that is going to be populated with the new custom element</param>
        /// <param name="context"></param>
        public override void ReadXmlElement(XmlReader reader, RequestSecurityToken rst, WSTrustSerializationContext context)
        {
            Assert.ArgumentIsNotNull(reader, "Reader can't be null");
            Assert.ArgumentIsNotNull(rst, "RequestSecurityToken can't be null");
            Assert.ArgumentIsNotNull(context, "WSTrustSerializationContext can't be null");

            try
            {
                if (requestParameters == null)
                {
                    base.ReadXmlElement(reader, rst, context);

                    return;
                }

                reader.MoveToContent();

                if (requestParameters.IsParameter(reader.Prefix, reader.LocalName, reader.NamespaceURI))
                {
                    var param = requestParameters.GetParametersDescription(reader.Prefix, reader.LocalName, reader.NamespaceURI);

                    rst.Properties.Add(param.Name, Convert.ChangeType(reader.ReadElementContentAsString(), param.Type, CultureInfo.InvariantCulture));
                }
                else
                {
                    //
                    // The rest is just normal thing
                    //
                    base.ReadXmlElement(reader, rst, context);
                }
            }
            catch (Exception ex)
            {
                if (this.logger != null && this.logger.ShallLogFailedRequest())
                {
                    this.logger.LogCustomReqParamRead(new TokenRequestParam(reader.LocalName, reader.Prefix, reader.NamespaceURI, reader.ReadElementContentAsString()), ex);
                }
                throw;
            }
        }

        public override void WriteXmlElement(XmlWriter writer, string elementName, object elementValue, RequestSecurityToken rst, WSTrustSerializationContext context)
        {
            string prefix = null;
            string namespaceUri = null;

            try
            {
                if (requestParameters.IsParameter(elementName))
                {
                    var paramDesc = requestParameters.GetParametersDescription(elementName);

                    prefix = paramDesc.Prefix;
                    namespaceUri = paramDesc.NamespaceUri;

                    writer.WriteElementString(paramDesc.Prefix, elementName, paramDesc.NamespaceUri, Convert.ToString(elementValue, CultureInfo.InvariantCulture));
                }
                else
                {
                    base.WriteXmlElement(writer, elementName, elementValue, rst, context);
                }
            }
            catch (Exception ex)
            {
                if (this.logger != null && this.logger.ShallLogFailedRequest())
                {
                    this.logger.LogCustomReqParamWrite(new TokenRequestParam(elementName, prefix, namespaceUri, elementValue), ex);
                }
                throw;
            }
        }
    }
}