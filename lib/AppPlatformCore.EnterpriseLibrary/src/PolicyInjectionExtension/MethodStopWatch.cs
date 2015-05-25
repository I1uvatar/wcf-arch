using System;
using System.Collections.Specialized;
using System.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection;
using Microsoft.Practices.EnterpriseLibrary.PolicyInjection.Configuration;

namespace AppPlatform.Core.EnterpriseLibrary.PolicyInjectionExtension
{
    /// <summary>
    /// Class uset to measure time elapsed during method call
    /// </summary>
    [ConfigurationElementType(typeof(CustomCallHandlerData))]
    public class MethodStopWatch : ICallHandler
    {
        public interface IStopWatchReport
        {
            void OnMethodCompleted(string methodName, TimeSpan timeElapsed);
        }

        private class NoReport : IStopWatchReport
        {
            public void OnMethodCompleted(string methodName, TimeSpan timeElapsed) {}
        }

        /// <summary>
        /// Ctor
        /// It needs to be here, otherwise the framework can not instantiate the handler
        /// </summary>
        /// <param name="loggingConfiguration"></param>
        public MethodStopWatch(NameValueCollection loggingConfiguration) { }

        /// <summary>
        /// Function, providing the implementation of IStopWatchReport
        /// By default it tries to find it in the IocProvider.Container.
        /// </summary>
        protected virtual IStopWatchReport Report
        {
            get
            {
                try
                {
                    return IocProvider.Container.Resolve<IStopWatchReport>();
                }
                catch
                {
                    return new NoReport();
                }
            }
        }

        /// <summary>
        /// Measures and reports the elapsed time
        /// </summary>
        /// <param name="input"></param>
        /// <param name="getNext"></param>
        /// <returns></returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            var aWatch = new Stopwatch();
            aWatch.Start();
            var msg = getNext()(input, getNext);
            aWatch.Stop();

            this.Report.OnMethodCompleted(input.MethodBase.ToString(), aWatch.Elapsed);

            return msg;
        }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set;}
    }
}
