using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreyrViewer.Common;

namespace FreyrViewer.Ui.MdiForms
{
    public partial class GeneralEnvironment : FrmBaseForm
    {
        public GeneralEnvironment(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
        }
    }
}
