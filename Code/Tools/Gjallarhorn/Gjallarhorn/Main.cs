using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eir.Common.Logging;
using Eir.Common.Net;

namespace Gjallarhorn
{
    internal class Main
    {
        private static readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private readonly bool _standaloneExe;
        private Engine _engine;

        public Main(bool standaloneExe)
        {
            _standaloneExe = standaloneExe;
        }

        public static string GetUserAgent(string appName)
        { 
            return $"ProacticeMonitorService/{Assembly.GetExecutingAssembly().GetName().Version}; ({appName})";
        }

        public void Run()
        {
            try
            {
                Init();
                //if (_standaloneExe) StartUi();
                if (_standaloneExe)
                    Execute();
                else
                    Task.Factory.StartNew(Execute);

                Log.To.Main.Add("Service is up and running.");
            }
            catch (Exception ex)
            {
                Log.To.WindowsEvent.Error(ex);
            }
        }

        private void Execute()
        {
            try
            {
                _engine.Start();
                _shutdownEvent.WaitOne();
            }
            catch (Exception ex)
            {
                Log.To.WindowsEvent.Error(ex);
            }
            Teardown();
        }

        private void Teardown()
        {
            _engine.Stop();
            
            Log.To.Main.Add("Application is stopped and closing.");
            //this is done to try to force the gc to collect to be able to catch unhandled task exceptions.
            Thread.Sleep(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
            Log.Shutdown();
            Thread.Sleep(200);//make sure we have last loglines

        }

        private void Init()
        {
            SecurityInitiator.InitProtocols();
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            _engine = new Engine();
        }

        public void Stop()
        {
            //reserve windows services timeout
            Teardown();
            //waitfor timeout to happen
            //if timeout env.exit(1)
        }

        public static void ForceStop()
        {
            _shutdownEvent.Set();
        }
    }
}