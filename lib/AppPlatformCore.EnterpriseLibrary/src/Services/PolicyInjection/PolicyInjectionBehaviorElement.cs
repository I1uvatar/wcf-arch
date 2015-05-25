using System;
using System.ServiceModel.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.Services.PolicyInjection
{
    public class PolicyInjectionBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(PolicyInjectionBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new PolicyInjectionBehavior();
        }
    }
}
