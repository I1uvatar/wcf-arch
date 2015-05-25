using System;
using System.Configuration;
using System.Globalization;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Services.CustomBindings
{
    /// <summary>
    /// Collection, holding Custom2007FederationBinding configuration elements
    /// </summary>
    public class Custom2007FederationBindingCollectionElement : StandardBindingCollectionElement<Custom2007FederationBinding, Custom2007FederationBindingElement>
    { }

    /// <summary>
    /// Configuration element describing the Custom2007FederationBinding configuration.
    /// </summary>
    public class Custom2007FederationBindingElement : StandardBindingElement
    {
        protected override void OnApplyConfiguration(Binding binding)
        {
            if ((binding == null))
            {
                throw new ArgumentNullException("binding");
            }
            if (!(binding is Custom2007FederationBinding))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid type for binding. Expected type: {0}. Type passed in: {1}.", typeof(Custom2007FederationBinding).AssemblyQualifiedName, binding.GetType().AssemblyQualifiedName));
            }

            var federationBinding = binding as Custom2007FederationBinding;

            federationBinding.Encoding = this.Encoding;
            federationBinding.WsFederationBindingConfiguration = this.WsFederationBindingConfiguration;
            federationBinding.EstablishSecurityContext = this.EstablishSecurityContext;
            federationBinding.ServiceCredentials = this.ServiceCredentials;
        }

        /// <summary>
        /// Message encoding. Possible values are "soap" and "binary"
        /// </summary>
        [ConfigurationProperty("encoding", DefaultValue = "soap")]
        public string Encoding
        {
            get
            {
                var result = string.Empty;
                try
                {
                    result = ((string)(base["encoding"])).ToLowerInvariant();
                }
                catch {}

                return result;
            }
            set
            {
                base["encoding"] = value;
            }
        }

        /// <summary>
        /// Template WS2007FederationBinding configuration name.
        /// </summary>
        [ConfigurationProperty("ws2007FederationBindingConfiguration", DefaultValue = "")]
        public string WsFederationBindingConfiguration
        {
            get
            {
                var result = string.Empty;
                try
                {
                    result = (string)base["ws2007FederationBindingConfiguration"];
                }
                catch { }

                return result;
            }
            set
            {
                base["ws2007FederationBindingConfiguration"] = value;
            }
        }

        /// <summary>
        /// Establish security context for the session. Same semantics as in the case of WSHttpBinding.
        /// </summary>
        [ConfigurationProperty("establishSecurityContext", DefaultValue = false)]
        public bool EstablishSecurityContext
        {
            get
            {
                var result = false;
                try
                {
                    result = (bool)base["establishSecurityContext"];
                }
                catch { }

                return result;
            }
            set
            {
                base["establishSecurityContext"] = value;
            }
        }

        /// <summary>
        /// Service credentials type
        /// </summary>
        [ConfigurationProperty("serviceCredentials", DefaultValue = "none")]
        public string ServiceCredentials
        {
            get
            {
                var result = "none";
                try
                {
                    var tryResult = base["serviceCredentials"].ToString();
                    if (!String.IsNullOrEmpty(tryResult))
                    {
                        result = tryResult;
                    }
                }
                catch {}

                return result;
            }
            set
            {
                base["serviceCredentials"] = value;
            }
        }


        protected override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            var federationBinding = binding as Custom2007FederationBinding;
            if(federationBinding == null)
            {
                return;
            }

            this.Encoding = federationBinding.Encoding;
            this.WsFederationBindingConfiguration = federationBinding.WsFederationBindingConfiguration;
            this.EstablishSecurityContext = federationBinding.EstablishSecurityContext;
            this.ServiceCredentials = federationBinding.ServiceCredentials;
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                ConfigurationPropertyCollection properties = base.Properties;
                properties.Add(new ConfigurationProperty("encoding", typeof(string)));
                properties.Add(new ConfigurationProperty("ws2007FederationBindingConfiguration", typeof(string)));
                properties.Add(new ConfigurationProperty("establishSecurityContext", typeof(bool)));
                properties.Add(new ConfigurationProperty("serviceCredentials", typeof(string)));

                return properties;
            }
        }

        protected override Type BindingElementType
        {
            get { return typeof(Custom2007FederationBinding); }
        }
    }
}
