// http://devplanet.com/blogs/brianr/archive/2008/09/29/thread-safe-dictionary-update.aspx
// (C) Brian Rudolph

using System;
using System.Threading;

namespace AppPlatform.Core.EnterpriseLibrary.Threading
{
    public static class Locks
    {
        private const int LockRetry = 5000; // 5 seconds

        public static void GetReadLock(ReaderWriterLockSlim locks)
        {
            bool lockAcquired = false;
            var i = 0;
            while (!lockAcquired && ++i < LockRetry)
            {
                lockAcquired = locks.TryEnterUpgradeableReadLock(1);
            }

            if (!lockAcquired)
            {
                throw new InvalidOperationException("ReadLock failed!");
            }
        }


        public static void GetReadOnlyLock(ReaderWriterLockSlim locks)
        {
            bool lockAcquired = false;
            var i = 0;
            while (!lockAcquired && ++i < LockRetry)
            {
                lockAcquired = locks.TryEnterReadLock(1);
            }

            if (!lockAcquired)
            {
                throw new InvalidOperationException("ReadOnlyLock failed!");
            }
        }


        public static void GetWriteLock(ReaderWriterLockSlim locks)
        {
            bool lockAcquired = false;
            var i = 0;
            while (!lockAcquired && ++i < LockRetry)
            {
                lockAcquired = locks.TryEnterWriteLock(1);
            }

            if (!lockAcquired)
            {
                throw new InvalidOperationException("WriteLock failed!");
            }
        }


        public static void ReleaseReadOnlyLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsReadLockHeld)
                locks.ExitReadLock();
        }


        public static void ReleaseReadLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsUpgradeableReadLockHeld)
                locks.ExitUpgradeableReadLock();
        }


        public static void ReleaseWriteLock(ReaderWriterLockSlim locks)
        {
            if (locks.IsWriteLockHeld)
                locks.ExitWriteLock();
        }


        public static void ReleaseLock(ReaderWriterLockSlim locks)
        {
            ReleaseWriteLock(locks);
            ReleaseReadLock(locks);
            ReleaseReadOnlyLock(locks);
        }


        public static ReaderWriterLockSlim GetLockInstance()
        {
            return GetLockInstance(LockRecursionPolicy.SupportsRecursion);
        }


        public static ReaderWriterLockSlim GetLockInstance(LockRecursionPolicy recursionPolicy)
        {
            return new ReaderWriterLockSlim(recursionPolicy);
        }
    }


    public abstract class BaseLock : IDisposable
    {
        protected ReaderWriterLockSlim _Locks;
        
        protected BaseLock(ReaderWriterLockSlim locks)
        {
            _Locks = locks;
        }


        public abstract void Dispose();
    }


    public class ReadLock : BaseLock
    {
        public ReadLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadLock(this._Locks);
        }


        public override void Dispose()
        {
            Locks.ReleaseReadLock(this._Locks);
        }
    }


    public class ReadOnlyLock : BaseLock
    {
        public ReadOnlyLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetReadOnlyLock(this._Locks);
        }


        public override void Dispose()
        {
            Locks.ReleaseReadOnlyLock(this._Locks);
        }
    }


    public class WriteLock : BaseLock
    {
        public WriteLock(ReaderWriterLockSlim locks)
            : base(locks)
        {
            Locks.GetWriteLock(this._Locks);
        }


        public override void Dispose()
        {
            Locks.ReleaseWriteLock(this._Locks);
        }
    }
}
