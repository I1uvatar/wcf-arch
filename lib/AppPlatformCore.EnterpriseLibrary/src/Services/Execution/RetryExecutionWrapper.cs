using System;
using System.Collections.Generic;
using System.ServiceModel.Security;

namespace AppPlatform.Core.EnterpriseLibrary.Services.Execution
{
    public class RetryExecutionWrapper : IActionExecutionWrapper
    {
        private static readonly RetryOnExceptionPoliciesCollection policies;
        private readonly RetryOnExceptionPolicyElement policy;

        static RetryExecutionWrapper()
        {
            var configSection = System.Configuration.ConfigurationManager.GetSection("RetryOnExceptionConfiguration");

            if (configSection == null || !(configSection is RetryConfiguration))
            {
                return;
            }

            policies = ((RetryConfiguration)configSection).Policies;
        }

        public RetryExecutionWrapper(string policyName)
        {
            if (policies == null)
            {
                return;
            }

            this.policy = policies[policyName];
        }

        public void Execute(Execute action)
        {
            var sucedeed = false;

            var retries = new Dictionary<Type, long>();

            while (!sucedeed)
            {
                try
                {
                    action();

                    sucedeed = true;

                    return;
                }
                catch (Exception ex)
                {
                    var exceptionType = ex.GetType();

                    if (this.policy == null)
                    {
                        throw;
                    }

                    var policyExceptionDefinition = this.policy.Exceptions[exceptionType];

                    if (policyExceptionDefinition != null)
                    {
                        if (retries.ContainsKey(exceptionType))
                        {
                            if (retries[exceptionType] >= policyExceptionDefinition.RetryCount) //TODO - temp
                            {
                                throw;
                            }

                            retries[typeof(SecurityNegotiationException)]++;
                        }
                        else
                        {
                            retries.Add(typeof(SecurityNegotiationException), 1);
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
