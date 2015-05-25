using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace AppPlatform.Core.EnterpriseLibrary.Security
{
    public static class Impersonation
    {
        #region Public Methods

        public static bool CanLogonUser(string username, string domain, string password, LogonSessionType sessionType)
        {
            //Assert.ArgumentIsNotNullOrEmpty(domain, "Parameter domain is null or empty");
            Assert.ArgumentIsNotNullOrEmpty(username, "Parameter username is null or empty");
            Assert.ArgumentIsNotNullOrEmpty(password, "Parameter password is null or empty");

            SafeUserTokenHandle token = null;

            try
            {
                bool success = LogonUser(username,
                                         domain,
                                         password,
                                         sessionType,
                                         LogonProvider.Default,
                                         out token);
                return success;
            }
            catch (Exception)
            {
                return false; 
            }
            finally
            {
                if (token != null)
                    token.Dispose();
            }
        }

        public static WindowsImpersonationContext Impersonate(
            string domain, string username, string password, LogonSessionType sessionType)
        {
            //Assert.ArgumentIsNotNullOrEmpty(domain, "Parameter domain is null or empty");
            Assert.ArgumentIsNotNullOrEmpty(username, "Parameter username is null or empty");
            Assert.ArgumentIsNotNullOrEmpty(password, "Parameter password is null or empty");
            
            WindowsImpersonationContext impersonationContext = null;
            SafeUserTokenHandle token;

            if (LogonUser(
               username,
               domain,
               password,
               sessionType,
               LogonProvider.Default,
               out token))
                {
                    using (token)
                    {
                        impersonationContext = WindowsIdentity.Impersonate(
                            token.DangerousGetHandle());
                    }
                }
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            
            return impersonationContext;
        }

        #endregion

        #region Native Methods

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
          string principal,
          string authority,
          string password,
          LogonSessionType logonType,
          LogonProvider logonProvider,
          out SafeUserTokenHandle token);

        public enum LogonSessionType : uint
        {
            Interactive = 2,
            Network,
            Batch,
            Service,
            NetworkCleartext = 8,
            NewCredentials
        }

        private enum LogonProvider : uint
        {
            Default = 0, // default for platform (use this!)
            WinNT35,     // sends smoke signals to authority
            WinNT40,     // uses NTLM
            WinNT50      // negotiates Kerb or NTLM
        }

        #endregion

        #region SafeTokenHandle Class

        [SuppressUnmanagedCodeSecurity,
         HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
        private sealed class SafeUserTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal SafeUserTokenHandle() : base(true) { }

            internal SafeUserTokenHandle(IntPtr existingHandle, bool ownsHandle) :
                base(ownsHandle)
            {
                base.SetHandle(existingHandle);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success),
             DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(base.handle);
            }
        }

        #endregion
    }
}
