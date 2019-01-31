using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using Eir.Common.IO;
using FreyrCommon.Models;
using FreyrViewer.Common;
using FreyrViewer.Extensions;
using FreyrViewer.Ui.Grids;
using FreyrViewer.Ui.Helpers;


namespace FreyrViewer.Ui.MdiForms
{
    public partial class FrmSenseNodes : FrmBaseForm
    {
        private GridHelper<SuperSimpleColumnTypes.TwoColumnType> _gridHelper;

        public FrmSenseNodes(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
        }

        private void FrmSenseNodes_Load(object sender, EventArgs e)
        {
            treeInfo.BeginUpdate();
            var infos = Switchboard.Instance?.LogCollectorOutput?.GroupedServerInfo;
            if (infos == null)
                return;
            infos.Sort((p1,p2) => string.Compare(p1.QlikSenseMachineInfo.HostName, p2.QlikSenseMachineInfo?.HostName, StringComparison.Ordinal));
            infos.ForEach(p =>
            {
                CreateNode(treeInfo,p);
            });
            var node = treeInfo.Nodes.Add("Local Server info");
            CreateNode(node, "Qrs about", Switchboard.Instance.LogCollectorOutput.QrsAbout);
            CreateNode(node, "Cal info", Switchboard.Instance.LogCollectorOutput.CalInfo);
            CreateNode(node, "License info", Switchboard.Instance.LogCollectorOutput.LicenseAgent);
            CreateNode(node, "Network", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedInfoNetwork);
            CreateNode(node, "Firewall", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedFirewall);
            CreateNode(node, "Users & Security", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedUsersAndSec);
            CreateNode(node, "Processes & Services", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedServices);
            CreateNode(node, "Certificates", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedCertifictes);
            CreateNode(node, "Server info", Switchboard.Instance.LogCollectorOutput.CmdLineOutput?.GroupedServerInfo);

            treeInfo.EndUpdate();
            treeInfo.ExpandAll();
            ResizeAllToCurrentSize();
        }

     

        private void CreateNode(TreeNode reveiver, string name, object tag )
        {
            if (tag == null)
                return;
            reveiver.Nodes.Add( new TreeNode
            {
                Name = name,
                Tag = tag,
                //ImageIndex = (int)menuItem.MenuIcon,
                Text = name,
                SelectedImageIndex = 0,
                //ForeColor = foreColor
            });
        }

        private void CreateNode(TreeView reveiver, GroupedServerInfo info)
        {
            var font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            var ret = new TreeNode
            {
                Name = info.QlikSenseMachineInfo.HostName,
                Tag = info,
                NodeFont = font,
                //ImageIndex = (int)menuItem.MenuIcon,
                Text = info.QlikSenseMachineInfo.HostName + (info.QlikSenseMachineInfo.IsCentral ? " (Central node)" :""),
                SelectedImageIndex = 0,
                //ForeColor = foreColor,
                ContextMenuStrip = ctrlContext,
            };
            font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular);
            info.QlikSenseServiceInfo.ForEach(p =>
            {
                if (!p.ServiceState.Equals("Disabled", StringComparison.InvariantCultureIgnoreCase))
                {
                    var s = $"{p.ServiceType} ({p.ServiceState}) {p.ModifiedDate.ToString("yyyy-MM-dd")}";
                    ret.Nodes.Add(new TreeNode { Text = s, NodeFont = font, });

                }
            });

            reveiver.Nodes.Add(ret);
        }

        private void ShowDataPane(IEnumerable<SuperSimpleColumnTypes.TwoColumnType> info)
        {
            CtrlGrid.Dock = DockStyle.Fill;
            CtrlGrid.Visible = true;
            ctrlFlow.Visible = false;

            if (_gridHelper == null)
            {
                _gridHelper = new GridHelper<SuperSimpleColumnTypes.TwoColumnType>(CtrlGrid, () => LoadStaticData(info),DisposalHelper);
                _gridHelper.UseAlternatingBackColor = true;
                _gridHelper.Columns.CreateColumn("Name", x => x.ColumnOne);
                _gridHelper.Columns.CreateColumn("Values", x => x.ColumnTwo);
                _gridHelper.ReloadOnce(true);
            }
            else
            {
                _gridHelper.ChangeData(()=>LoadStaticData(info));
            }
        }

        private Task<IEnumerable<SuperSimpleColumnTypes.TwoColumnType>> LoadStaticData(IEnumerable<SuperSimpleColumnTypes.TwoColumnType> info)
        {
            if (info == null)
            {
                var stuff = new List<SuperSimpleColumnTypes.TwoColumnType>{new SuperSimpleColumnTypes.TwoColumnType{ColumnOne = "No Data found"}};
                return Task.FromResult(stuff as IEnumerable<SuperSimpleColumnTypes.TwoColumnType>);
            }
                
            var ret = Task.FromResult(info.Where(p=> !string.IsNullOrWhiteSpace(p.ColumnOne )));
            return ret;
        }

        private void treeInfo_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                var menuItem = e.Node?.Tag as GroupedServerInfo;
                if (menuItem != null)
                {
                    ShowDataPane(menuItem.SystemInfo);
                    return;
                }
                var simpleNode = e.Node?.Tag as List<SuperSimpleColumnTypes.TwoColumnType>;
                if (simpleNode != null)
                {
                    ShowDataPane(simpleNode);
                    return;
                }
                var cmdLineNode = e.Node?.Tag as List<CmdLineResult>;
                if (cmdLineNode != null)
                {
                    ShowStaticInfo(cmdLineNode);
                   
                }

            }
            catch (Exception ex)
            {
                Mbox.Error(ex, $"Failed to load '{e.Node?.Text ?? "Unknown"}'");
            }
        }

        private void ShowStaticInfo(List<CmdLineResult> cmdLineNode)
        {
            ctrlFlow.Dock = DockStyle.Fill;
            ctrlFlow.Visible = true;
            CtrlGrid.Visible = false;

            var lbl = lblData1.Clone();
            var txt = txtData1.Clone();
            ctrlFlow.Controls.Clear();
            var i = 0;
            foreach (var item in cmdLineNode)
            {
                i++;

                var lblInner = lbl.Clone();
                lblInner.Name = "lblData" + i;
                lblInner.Text = $@"{item.Name} - ({item.Cmd}) " ;

                var txtInner = txt.Clone();
                txtInner.Name = "txtData" + i;
                txtInner.Text = string.IsNullOrEmpty(item.Result) ? "<no data received>" : item.Result.Replace("\r\n",Environment.NewLine);
                var size = txtInner.Font.GetTextSize(txtInner.Text);
                txtInner.Height = (int)size.Height + 10;

                ctrlFlow.Controls.Add(lblInner);
                ctrlFlow.Controls.Add(txtInner);
            }
            ResizeResultPaneCtrls();
        }

        private void ResizeResultPaneCtrls()
        {
            foreach (Control ctrl in ctrlFlow.Controls)
            {
                ctrl.Width = ctrlFlow.ClientSize.Width - 10;
            }
        }

        private void ctrlFlow_Resize(object sender, EventArgs e)
        {
            ResizeResultPaneCtrls();
        }

        private void treeInfo_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag == null)
                e.Cancel = true;
        }

        private void mnuItemOpenLogFolder_Click(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var contextMenu = menuItem?.GetCurrentParent() as ContextMenuStrip;
            var node = contextMenu?.SourceControl as TreeView;
            var a = node?.SelectedNode?.Tag as GroupedServerInfo;
            string path = a?.LogFolderPath;
            Process.Start(path);
        }

        private void FrmSenseNodes_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
