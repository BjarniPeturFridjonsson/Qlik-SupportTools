using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreyrCommon.Models;
using FreyrViewer.Enums;
using FreyrViewer.Ui;
using FreyrViewer.Ui.MdiForms;
using WeifenLuo.WinFormsUI.Docking;

namespace FreyrViewer.Common
{
    public class LoadPanelHelper
    {
        private readonly DockPanel _dockPanel;
        private readonly FormsCache _formsCache;
        private MenuItemWrapper _startItem;
        private FrmMenu _frm;
        private string _lastOpenedLog;

        public LoadPanelHelper(DockPanel dock, FrmMain main, CommonResources commonResources)
        {
            _dockPanel = dock;
            _formsCache = new FormsCache(_dockPanel, main, commonResources);
        }

        public void CloseAllOpenForms()
        {
            foreach (var item in _formsCache.ImmediateFormWrappers)
            {

                item.Value.FormTask.Result.Close();
            }
            _frm.ClearDynamicMenues();
        }

        public void ShowStartPageAfterLoad()
        {
            _startItem.MenuAction.Invoke();
        }

        public async Task ShowLastLoadedLog()
        {
            await _frm.NavigateToLog(_lastOpenedLog);
        }

        public void RecalcMenuFont()
        {
            Switchboard.Instance.ControlResize.SetCorrectFontSize(_frm.Controls);
        }

        public void AddSenseLogsForHost(string name, List<SenseLogInfo> logNames)
        {
            _lastOpenedLog = logNames.LastOrDefault()?.Name;
            logNames.Sort((p1,p2)=> string.Compare(p1.Name,p2.Name,StringComparison.Ordinal));
            var items = CreateWrapper(logNames);
            var newWrapper = new MenuItemWrapper(name, null, null, items,ApplicationMenuIcon.BaseMenuServer);
            _frm.CreateDynamicMenuItems(new[] { newWrapper });
            
        }

        private MenuItemWrapper[] CreateWrapper(List<SenseLogInfo> logNames)
        {
            var ret = new List<MenuItemWrapper>();
            logNames.ForEach(p =>
            {
                var subItems = CreateWrapper(p.LogInfos);
                var newWrapper = _formsCache.CreateFormItem<FrmSenseLogs>("Sense Logs", p.IsDirectory ? ApplicationMenuIcon.BaseMenuFolder : ApplicationMenuIcon.Emtpy, p);
                newWrapper.Text = p.Name;
                newWrapper.Key = p.LogFilePath;
                newWrapper.SubMenuItems = subItems;
                _lastOpenedLog = p.LogFilePath;
                ret.Add(newWrapper);
            });
            return ret.ToArray();
        }

        public void ActivateSenseForm(object param)
        {
            _formsCache.CreateFormItem<FrmSenseLogs>("Sense Logs", ApplicationMenuIcon.Emtpy, param).MenuAction.Invoke();
        }

        public FrmMenu Start()
        {
            _frm = new FrmMenu(null, "FrmMenu");
            _frm.Show(_dockPanel, DockState.DockLeft);
            //frm.MdiParent = _main;
            _startItem = _formsCache.CreateFormItem<FrmSenseNodes>("Sense Nodes", ApplicationMenuIcon.BaseMenuNodes,null);
            var menuItems = new List<MenuItemWrapper>();
            //menuItems.Add(_formsCache.CreateFormItem<FrmSenseLogViewer>("Log"));
            menuItems.Add(_formsCache.CreateFormItem<FrmWindowsLogViewer>("Windows Logs", ApplicationMenuIcon.BaseMenuLogs, null));
            menuItems.Add(_formsCache.CreateFormItem<FrmLogCollectorLog>("LogCollector Log", ApplicationMenuIcon.BaseMenuLogs, null));
            menuItems.Add(_formsCache.CreateFormItem<FrmSenseLogs>("Log files", ApplicationMenuIcon.BaseMenuLogs, null));
            menuItems.Add(_startItem);
            _frm.ShowMenu(menuItems);
            return _frm;
        }

       
    }
}
