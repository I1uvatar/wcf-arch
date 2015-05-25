using System;
using System.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Execution
{
    public class RetryConfiguration : ConfigurationSection
    {
        private const string SectionName = "RetryOnExceptionConfiguration";

        public static RetryConfiguration GetConfig()
        {
            return (RetryConfiguration)ConfigurationManager.GetSection(SectionName) ?? new RetryConfiguration();
        }

        [ConfigurationProperty("policies")]
        [ConfigurationCollection(typeof(RetryOnExceptionPoliciesCollection), AddItemName = "add")]
        public RetryOnExceptionPoliciesCollection Policies
        {
            get
            {
                return (RetryOnExceptionPoliciesCollection)this["policies"] ?? new RetryOnExceptionPoliciesCollection();
            }
        }
    }

    public class RetryOnExceptionPoliciesCollection : ConfigurationElementCollection
    {
        public RetryOnExceptionPolicyElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as RetryOnExceptionPolicyElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        new public RetryOnExceptionPolicyElement this[string Name]
        {
            get
            {
                return (RetryOnExceptionPolicyElement)BaseGet(Name);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RetryOnExceptionPolicyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RetryOnExceptionPolicyElement)element).Name;
        }
    }

    public class RetryOnExceptionPolicyElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
        }

        [ConfigurationProperty("exceptions", IsRequired = true)]
        public PolicyExceptionsCollection Exceptions
        {
            get
            {
                return (PolicyExceptionsCollection)this["exceptions"] ?? new PolicyExceptionsCollection();
            }
        }
    }

    public class PolicyExceptionsCollection : ConfigurationElementCollection
    {
        public PolicyExceptionElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as PolicyExceptionElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PolicyExceptionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PolicyExceptionElement)element).TypeName;
        }

        public PolicyExceptionElement this[Type type]
        {
            get
            {
                return base.BaseGet(type.AssemblyQualifiedName) as PolicyExceptionElement;
            }
        }
    }

    public class PolicyExceptionElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string TypeName
        {
            get
            {
                return this["type"] as string;
            }
        }

        private Type exceptionType;

        public Type ExceptionType
        {
            get
            {
                if (exceptionType == null)
                {
                    exceptionType = Type.GetType(this.TypeName);
                }

                return exceptionType;
            }
        }

        [ConfigurationProperty("retryCount", IsRequired = false, DefaultValue = 0)]
        public int RetryCount
        {
            get
            {
                return (int)this["retryCount"];
            }
        }

        [ConfigurationProperty("delay", IsRequired = false)]
        public TimeSpan? Delay
        {
            get
            {
                return (TimeSpan?)this["delay"];
            }
        }
    }
}
