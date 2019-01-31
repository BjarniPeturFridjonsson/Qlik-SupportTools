using Eir.Common.Logging;
using NUnit.Framework;

namespace Eir.Common
{
    public abstract class TestBase
    {
        private Exchanger<ILogs> _logsExchanger;

        [OneTimeSetUp]
        public virtual void TestFixtureSetup()
        {
            _logsExchanger = new Exchanger<ILogs>(ConsoleLogs.Instance, Log.Testing.ExchangeInstance);
        }

        [OneTimeTearDown]
        public virtual void TestFixtureTearDown()
        {
            _logsExchanger.Dispose();
        }
    }
}