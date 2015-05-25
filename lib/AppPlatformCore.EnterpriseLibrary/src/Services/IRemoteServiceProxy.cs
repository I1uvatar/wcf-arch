using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using AppPlatform.Core.EnterpriseLibrary.ExceptionHandling;
using AppPlatform.Core.EnterpriseLibrary.Services.Context;

namespace AppPlatform.Core.EnterpriseLibrary.Services
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void Execute();

    /// <summary>
    /// Defines interface for accessing a service proxy
    /// </summary>
    /// <typeparam name="IServiceContract">Contract of the service being accessed via this proxy</typeparam>
    public interface IRemoteServiceProxy<IServiceContract>
    {
        /// <summary>
        /// Get the remote service contract implementation.
        /// </summary>
        IServiceContract Contract { get; }

        /// <summary>
        /// Gets the client credentials.
        /// </summary>
        /// <value>The client credentials.</value>
        ClientCredentials ClientCredentials { get; }

        /// <summary>
        /// Gets the state of the communication object.
        /// </summary>
        CommunicationState CommunicationState { get; }

        /// <summary>
        /// "Safe" execute of the remote call sequence. Default exception policy ("Default Policy" configured 
        /// in configiration file) is used.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="pendingCommand">The command to execute.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        bool ExecuteAndRelease(Execute pendingCommand);

        /// <summary>
        /// "Safe" execute of the remote call sequence. 
        /// Default exception policy ("Default Policy" configured in configiration file) is used.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="contractAction">The contract action.</param>
        /// <returns> True if command is successfully executed, false otherwise. </returns>
        bool ExecuteAndRelease(Action<IServiceContract> contractAction);

        /// <summary>
        /// "Safe" execute of the remote call sequence using the specified exception policy.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="contractAction">The contract action.</param>
        /// <param name="exceptionPolicy">The exception policy.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        bool ExecuteAndRelease(Action<IServiceContract> contractAction, string exceptionPolicy);

        /// <summary>
        /// "Safe" execute of the remote call sequence using the specified exception policy.
        /// It mimics the "using" pattern.
        /// Because it is advised not to use the using pattern we have our own implementation.
        /// See http://msdn2.microsoft.com/en-us/library/aa355056.aspx
        /// </summary>
        /// <param name="pendingCommand">The command to execute.</param>
        /// <param name="exceptionPolicy">The exception policy.</param>
        /// <returns>True if command is successfully executed, false otherwise.</returns>
        bool ExecuteAndRelease(Execute pendingCommand, string exceptionPolicy);

        /// <summary>
        /// Sets a custom exception handling policy. If not set, EnterpriseLibrary ExceptionHandling block is used. 
        /// If <para>policy</para> returns true, the exception should be rethrown
        /// </summary>
        /// <param name="policy"></param>
        void SetExceptionHandlingPolicy(Func<Exception, string, bool> policy);

        void SetCustomExceptionProcessors(ExceptionProcessors processors);

        void SetOutgoingMessageHeaderBuilders(List<IMessageHeaderBuilder> headerBuilders);

        //void SetActionExecutionWrapper(IActionExecutionWrapper actionExecutionWrapper);
    }
}