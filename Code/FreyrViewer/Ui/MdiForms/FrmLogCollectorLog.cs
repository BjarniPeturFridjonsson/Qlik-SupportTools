using System;
using System.Reflection;
using System.Windows.Forms;
using FreyrCommon.Models;
using FreyrViewer.Common;

namespace FreyrViewer.Ui.MdiForms
{
  
    public partial class FrmLogCollectorLog : FrmBaseForm
    {
       
       

        public FrmLogCollectorLog(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
            WireDragNdrop();
        }

        private void FrmGenericTextViewer_Load(object sender, EventArgs e)
        {
            DoLoad();
        }


        private void DoLoad()
        {
            try
            {

                txtRawData.Text = Switchboard.Instance.LogCollectorOutput?.LogCollectorLog;
                var info = Switchboard.Instance.LogCollectorOutput?.LogCollectorSettings;
                ResizeAllToCurrentSize();
                if (info == null)
                    return;
                int i = 0;
                ctrlTableLayout.SuspendLayout();
                GetValue(info, i);
                ctrlTableLayout.ResumeLayout();
                







            }
            catch (Exception  e)
            {
                txtRawData.Text = $@"Something bad happened?\r\n{e}";
            }
            
        }

        private void GetValue(object info, int i)
        {
            //ctrlTableLayout.AutoSize = true;
            foreach (PropertyInfo propertyInfo in info.GetType().GetProperties())
            {
                if (propertyInfo.CanRead)
                {
                    object val = propertyInfo.GetValue(info, null);
                    if (val == null)
                        continue;
                    if (val is CommonCollectorOutput)
                    {
                        GetValue(val,i);
                        continue;
                    }
                    Label lblTitle = new Label();
                    lblTitle.Text = propertyInfo.Name;
                    lblTitle.TabIndex = (i * 2);
                    lblTitle.Margin = new Padding(0);
                    lblTitle.Dock = DockStyle.Fill;
                    ctrlTableLayout.Controls.Add(lblTitle, 0, i);

                    TextBox txtValue = new TextBox();
                    txtValue.TabIndex = (i * 2) + 1;
                    txtValue.Margin = new Padding(0);
                    txtValue.Dock = DockStyle.Fill;
                    txtValue.Text = val.ToString();
                    ctrlTableLayout.Controls.Add(txtValue, 1, i);
                    i++;
                }
            }
        }
    }
}
