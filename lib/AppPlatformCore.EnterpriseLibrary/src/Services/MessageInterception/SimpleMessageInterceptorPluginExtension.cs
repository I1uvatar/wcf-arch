using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Services.MessageInterception
{
    /// <summary>
    /// Extension to WCF Service Model. It is used as configuration handler for SimpleMessageInterceptorPlugin class.
    /// </summary>
    public class SimpleMessageInterceptorPluginExtension : BehaviorExtensionElement
    {
        ///<summary>
        ///Creates a behavior extension based on the current configuration settings.
        ///</summary>
        ///<returns>
        ///The behavior extension.
        ///</returns>
        protected override object CreateBehavior()
        {
            var typeList = new List<Type>();
            foreach (TypeConfigElement type in this.Types)
            {
                typeList.Add(Type.GetType(type.TypeName));
            }

            return new SimpleMessageInterceptorPlugin(typeList);
        }

        ///<summary>
        ///Gets the type of behavior.
        ///</summary>
        ///<returns>
        ///A <see cref="T:System.Type"></see>.
        ///</returns>
        public override Type BehaviorType
        {
            get { return typeof(SimpleMessageInterceptorPlugin); }
        }

        /// <summary>
        /// Gets a list of simple message interceptor types
        /// </summary>
        [ConfigurationProperty("types", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(TypeCollection), AddItemName = "add")]
        public TypeCollection Types
        {
            get
            {
                var typesCollection = (TypeCollection)base["types"];
                return typesCollection;
            }
        }
    }

    /// <summary>
    /// Extension to WCF Service Model. It is used as configuration handler for SimpleMessageInterceptorPlugin class.
    /// </summary>
    public class TypeCollection : ConfigurationElementCollection
    {
        ///<summary>
        ///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</summary>
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new TypeConfigElement();
        }

        ///<summary>
        ///Gets the element key for a specified configuration element when overridden in a derived class.
        ///</summary>
        ///<returns>
        ///An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///<param name="element">The <see cref="T:System.Configuration.ConfigurationElement"></see> to return the key for. </param>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TypeConfigElement)element).TypeName;
        }
    }

    /// <summary>
    /// Extension to WCF Service Model. It is used as configuration handler for SimpleMessageInterceptorPlugin class.
    /// </summary>
    public class TypeConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the message interceptor type to be instanciated.
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = null, IsRequired = true, IsKey = false)]
        public string TypeName
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }
    }
}
