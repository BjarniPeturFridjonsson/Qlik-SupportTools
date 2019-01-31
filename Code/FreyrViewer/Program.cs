using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eir.Common.Common;
using Eir.Common.Logging;
using FreyrViewer.Ui;
using System.Threading;

namespace FreyrViewer
{
    static class Program
    {
        private static bool _isDisposed;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static  void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            RegisterUnhandledErrors();

            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Qlik", ApplicationName.QlikCockpit.Short, "Log");
            Log.Init(new Logs(logDir, LogLevel.Verbose, () => false, ApplicationName.QlikCockpit));

            Application.Run(new FrmMain());
            
        }

        private static void RegisterUnhandledErrors()
        {
            //Making sure we find out what task is not being handled. This is connected to the Dispose func below with gc collection.
            TaskScheduler.UnobservedTaskException += (sender, eventArgs) =>
            {
                eventArgs.SetObserved();
                eventArgs.Exception.Handle(ex =>
                {
                    Log.To.Main.AddException($"UnobservedTaskException type: {ex.GetType()}", ex);
                    return true;
                });
            };

            Application.ThreadException += (sender, e) =>
            {
                MessageBox.Show($@"We have an unhandled thread exception. Sorry about that.{Environment.NewLine}{e.Exception}", @"Unhandled exception");
                Log.To.Main.AddException("Unhandled ThreadException", e.Exception);
                Dispose();
                Environment.Exit(-1);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                MessageBox.Show($@"We have an unhandled exception. Sorry about that.{Environment.NewLine}{e.ExceptionObject as Exception}", @"Unhandled exception");
                Log.To.Main.AddException($"Unhandled Exception with {(e.IsTerminating ? "Terminating" : "Non - Terminating")}", e.ExceptionObject as Exception);
                Dispose();
                Environment.Exit(-1);
            };

            Log.To.Main.Add("Application is up and running.");
        }

        public static void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            Log.To.Main.Add("Application is stopped and closing.");
            Log.Shutdown();
            //this is done to try to force the gc to collect to be able to catch unhandled task exceptions.
            Thread.Sleep(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);//make sure we have last loglines
            
        }
    }
}
