using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AppPlatform.Core.EnterpriseLibrary.Threading
{
    public class Checks
    {
        private static readonly Checks instance = new Checks();
        private const string MAIN_THREAD_NAME = "MainThread";
        private static Thread mainThread;
        private Control mainThreadControl;

        private Checks() {}
        public static Checks Instance
        {
            get { return instance; }
        }

        public void SetAsMainThread()
        {
            Thread.CurrentThread.Name = MAIN_THREAD_NAME;
            mainThread = Thread.CurrentThread;
        }

        public void SetMainThreadControl(Control mainThreadControl)
        {
            this.mainThreadControl = mainThreadControl;
        }

        public Control GetMainThreadControl()
        {
            return this.mainThreadControl;
        }

        public bool IsThisMainThread()
        {
            return (Thread.CurrentThread.Name == MAIN_THREAD_NAME);
        }

        public Thread GetMainThread()
        {
            return mainThread;
        }
    }
}
