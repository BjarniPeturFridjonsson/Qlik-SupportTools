using System.ComponentModel;
using System.Configuration.Install;

namespace Gjallarhorn
{
    [RunInstaller(true)]
    public partial class MonitorServiceInstaller : Installer
    {
        public MonitorServiceInstaller()
        {
            InitializeComponent();
        }

        public MonitorServiceInstaller(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}