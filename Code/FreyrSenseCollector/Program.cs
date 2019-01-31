using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrCommon.Logging;

namespace FreyrSenseCollector
{
   static class Program
    {
        private static bool _isDisposed;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegisterUnhandledErrors();

            //var s = File.ReadAllText();
            Application.Run(new FrmMain());
            Dispose();
        }

        private static void RegisterUnhandledErrors()
        {
            //Making sure we find out what task is not being handled. This is connected to the Dispose func below with gc collection.
            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                eventArgs.SetObserved();
                eventArgs.Exception.Handle(ex =>
                {
                    Log.Add($"UnobservedTaskException type: {ex.GetType()}", ex);
                    return true;
                });
            };

            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($@"We have an unhandled thread exception. Sorry about that.{Environment.NewLine}{e.Exception}", @"Unhandled exception");
                Log.Add("Unhandled ThreadException", e.Exception);
                Dispose();
                Environment.Exit(-1);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MessageBox.Show($@"We have an unhandled exception. Sorry about that.{Environment.NewLine}{e.ExceptionObject as Exception}", @"Unhandled exception");
                Log.Add($"Unhandled Exception with {(e.IsTerminating ? "Terminating" : "Non - Terminating")}", e.ExceptionObject as Exception);
                Dispose();
                Environment.Exit(-1);
            };

            Log.Add("Application is up and running.");
        }

        public static void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            Log.Add("Application is stopped and closing.");
            //this is done to try to force the gc to collect to be able to catch unhandled task exceptions.
            Thread.Sleep(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);//make sure we have last loglines
            Log.Shutdown();
        }
    }
}
