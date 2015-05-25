using System;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;

namespace AppPlatform.Core.EnterpriseLibrary.Services.PolicyInjection
{
    public class PolicyInjectionInstanceProvider : IInstanceProvider
    {
        private readonly Type serviceContractType;
        private static readonly PolicyInjectorFactory fac = new PolicyInjectorFactory();
        private static PolicyInjector injector;

        public PolicyInjectionInstanceProvider(Type t)
        {
            this.serviceContractType = t;
            if (injector == null)
                injector = fac.Create();
        }

        public object GetInstance(InstanceContext instanceContext,
          System.ServiceModel.Channels.Message message)
        {
            var type = instanceContext.Host.Description.ServiceType;

            return serviceContractType != null 
                ? injector.Create(type, serviceContractType) 
                : injector.Create(type);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
