using System.Collections.Specialized;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.PolicyInjectionExtension
{
    [ConfigurationElementType(typeof(CustomCallHandlerData))]
    public class ExampleHandler : ICallHandler
    {
        public ExampleHandler(NameValueCollection loggingConfiguration)
        {
        }

        public IMethodReturn Invoke(IMethodInvocation input,
                                    GetNextHandlerDelegate getNext)
        {
            // Declare any variables required for values used in this method here.

            // Perform any pre-processing tasks required in the custom handler here.
            // This code executes before control passes to the next handler.

            // Use the following line of code in any handler to invoke the next 
            // handler that the application block should execute. This code gets
            // the current return message that you must pass back to the caller:
            IMethodReturn msg = getNext()(input, getNext);

            // Perform any post-processing tasks required in the custom handler here.
            // This code executes after the invocation of the target object method or
            // property accessor, and before control passes back to the previous
            // handler as the Invoke call stack unwinds. You can modify the return 
            // message if required.

            // Return the message to the calling code, which may be the previous 
            // handler or, if this is the first handler in the chain, the client.   
            return msg;
        }



        #region ICallHandler Members


        public int Order
        { get; set; }

        #endregion
    }
}