using System;
using System.Transactions;

namespace AppPlatform.Core.EnterpriseLibrary.Transactions
{
    /// <summary>
    /// System.Transaction.TransactionScope functionality
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Votes for successfull transaction outcome
        /// </summary>
        void Complete();
    }

    /// <summary>
    /// Wrapper around the System.Transactions.TransactionScope
    /// </summary>
    public class TransactionScopeProvider : ITransactionScope
    {
        public delegate void Command();

        private readonly TransactionScope scope;
        private bool disposed;

        /// <summary>
        /// Constructor.
        /// Uses TransactionScopeOption.Required
        /// </summary>
        private TransactionScopeProvider() : this(TransactionScopeOption.Required) { }

        /// <summary>
        /// Constructor
        /// Uses default timeout
        /// </summary>
        /// <param name="option"></param>
        private TransactionScopeProvider(TransactionScopeOption option)
        {
            this.disposed = false;
            this.scope = new TransactionScope(option);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="option"></param>
        /// <param name="timeout"></param>
        private TransactionScopeProvider(TransactionScopeOption option, TimeSpan timeout)
        {
            this.disposed = false;
            this.scope = new TransactionScope(option, timeout);
        }

        public static void ExecuteOutsideTransactionScope(Command aCommand)
        {
            if (Transaction.Current == null || Transaction.Current.TransactionInformation.Status != TransactionStatus.Active)
            {
                aCommand();
                return;
            }

            using (TransactionScope suppressedScope = new TransactionScope(TransactionScopeOption.Suppress))
            {
                aCommand();
                suppressedScope.Complete();
            }
        }

        #region ITransactionScope Members

        /// <summary>
        /// Votes for successfull transaction outcome
        /// </summary>
        public void Complete()
        {
            this.scope.Complete();
        }

        #endregion

        #region IDisposable Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.scope.Dispose();
            GC.SuppressFinalize(this);
            this.disposed = true;
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~TransactionScopeProvider()
        {
            if (!this.disposed && this.scope != null)
            {
                this.scope.Dispose();
            }
        }

        #endregion

        #region Factory

        /// <summary>
        /// Creates a transaction scope.
        /// In debug version the timeout is set to 5 minutes.
        /// Transaction option is set to TransactionScopeOption.Required
        /// </summary>
        /// <returns></returns>
        public static ITransactionScope GetTransactionScope()
        {
            return GetTransactionScope(TransactionScopeOption.Required);
        }

        /// <summary>
        /// Creates a transaction scope.
        /// In debug version the timeout is set to 5 minutes.
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public static ITransactionScope GetTransactionScope(TransactionScopeOption option)
        {
#if DEBUG
            // This line can be used to turn off the transactions
            //return new FakeTransactionScope();
            return new TransactionScopeProvider(option, TimeSpan.FromMinutes(15));
#else
    // This line can be used to turn off the transactions
    // return new FakeTransactionScope();
            return new TransactionScopeProvider(option);
#endif
            #endregion
        }
    }
}