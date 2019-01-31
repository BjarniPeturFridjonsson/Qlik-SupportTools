using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Common;
using Eir.Common.Logging;
using Gjallarhorn.Common;

namespace Gjallarhorn
{
    public partial class Service : ServiceBase
    {
        private static Main _main;
        //private static bool _isDisposed;

        public Service()
        {
            InitializeComponent();
        }

        [MTAThread]
        public static void Main(string[] args)
        {
            bool standaloneExe = false;
            try
            {
                foreach (string arg in args)
                {
                    string argKey = arg.Split('=')[0].ToLower();
                    //string argVal = arg.IndexOf('=') > 0 ? arg.Split('=')[1].ToLower() : "";
                    switch (argKey)
                    {
                        case "-standalone":
                        case "/standalone":
                        case "-debug":
                        case "/debug":
                            standaloneExe = true;
                            break;
                    }
                }

                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Log.DEFAULT_DIR_NAME);
                Log.Init(new Logs(logDir, LogLevel.Info, () => false, ApplicationName.Gjallarhorn));
                Log.To.InitializeDynamicLog(NotificationLogItem.NotificationLogName, NotificationLogItem.Header);
                RegisterUnhandledErrors();
                _main = new Main(standaloneExe);
                if (standaloneExe)
                    _main.Run();
                else
                {
                    var dashboardServerService = new ServiceBase[] { new Service() };
                    Run(dashboardServerService);
                }
            }
            catch (Exception ex)
            {
                Log.To.WindowsEvent.Error(ex);
                Log.To.Main.AddException("Error in Main", ex);
            }
            finally
            {
                Environment.Exit(0);
            }
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


            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {

                Log.To.Main.AddException($"Unhandled Exception with {(e.IsTerminating ? "Terminating" : "Non - Terminating")}", e.ExceptionObject as Exception);
                Log.Shutdown();
                Thread.Sleep(1000);
                Environment.Exit(-1);
            };


        }

        protected override void OnStart(string[] args)
        {
            Log.To.Main.Add("Service start signal received");
            if (_main != null) _main.Run();
            base.OnStart(args);
            Log.To.Main.Add("Service start sequence completed");
        }


        protected override void OnStop()
        {
            Log.To.Main.Add("Service stop signal received");

            if (_main != null) _main.Stop();

            Log.To.Main.Add("Service stop sequence completed");
            Log.Shutdown();
            Thread.Sleep(500);

        }
        //public static void Dispose()
        //{
        //    if (_isDisposed)
        //    {
        //        return;
        //    }
        //    _isDisposed = true;
        //    Log.To.Main.Add("Application is stopped and closing.");
        //    //this is done to try to force the gc to collect to be able to catch unhandled task exceptions.
        //    Thread.Sleep(100);
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    Thread.Sleep(100);//make sure we have last loglines
        //    Log.Shutdown();
        //}
    }
}