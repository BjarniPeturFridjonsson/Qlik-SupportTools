using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FreyrViewer.Enums;
using FreyrViewer.Extensions;
using FreyrViewer.Ui;
using FreyrViewer.Ui.MdiForms;
using FreyrViewer.Ui.Splashes;
using WeifenLuo.WinFormsUI.Docking;

namespace FreyrViewer.Common
{
    public class FormsCache
    {
        public ConcurrentDictionary<string, FormWrapper> ImmediateFormWrappers { get; } = new ConcurrentDictionary<string, FormWrapper>();

        private readonly CommonResources _getCommonResources;
        private readonly DockPanel _dockPanel;
        private readonly FrmMain _main;
        
        public FormsCache(DockPanel dockPanel, FrmMain main, CommonResources commonResources)
        {
            _getCommonResources = commonResources;
            _dockPanel = dockPanel;
            _main = main;
        }

        public MenuItemWrapper CreateFormItem<T>(string formKey, ApplicationMenuIcon icon, object param) where T : FrmBaseForm
        {
            return new MenuItemWrapper(formKey, () => OpenFormOrActivate<T>(formKey, param), typeof(T), icon);
        }


        private async Task<FrmBaseForm> CreateNewForm<T>(Type formType, string key, CancellationTokenSource cts)
            where T : FrmBaseForm
        {
            T newForm = null;
            Splash splash = null;

            FormWrapper removedImmediateFormWrapper;

            try
            {
                newForm = CreateNewForm<T>(formType, key);
                splash = SplashManager.ShowEmbeddedSplash(newForm);
                CancelEventHandler newFormOnClosing = null;
                newForm.Closing += newFormOnClosing = (s, e) =>
                {

                    ImmediateFormWrappers.TryRemove(key, out removedImmediateFormWrapper);

                    WinformExtensions.ExecuteActionSwallowExceptions(cts.Cancel);
                    WinformExtensions.ExecuteActionSwallowExceptions(cts.Dispose);
                    splash?.Dispose();
                    newForm.Closing -= newFormOnClosing;
                };

                newForm.OnUiThread(() => newForm.Show(_dockPanel, DockState.Document));

                await newForm.InitAsync(cts.Token);

                //if (cache != _formsCache)
                //{
                //    throw new Exception("The form was opened for another customer (but wasn't initialized until now). Kill it!");
                //}

                splash?.Dispose();
            }
            catch (TaskCanceledException)
            {
                ImmediateFormWrappers.TryRemove(key, out removedImmediateFormWrapper);
                WinformExtensions.ExecuteActionSwallowExceptions(newForm.Close);
                splash?.Dispose();
                throw;
            }
            catch (Exception)
            {
                ImmediateFormWrappers.TryRemove(key, out removedImmediateFormWrapper);
                WinformExtensions.ExecuteActionSwallowExceptions(cts.Cancel);
                WinformExtensions.ExecuteActionSwallowExceptions(cts.Dispose);
                if (newForm != null) WinformExtensions.ExecuteActionSwallowExceptions(newForm.Close);
                splash?.Dispose();
                throw;
            }

            return newForm;
        }

        private T CreateNewForm<T>(Type formType, string title) where T : FrmBaseForm
        {
            Type[] constructorParamTypes = {typeof(CommonResources), typeof(string)};
            ConstructorInfo constructor = formType.GetConstructor(constructorParamTypes);
            return (T) constructor?.Invoke(new object[] {_getCommonResources, title});
        }

        private FormWrapper CreateFormWrapper<T>(Type formType, string key) where T : FrmBaseForm
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task<FrmBaseForm> formTask = _main.OnUiThread(() => CreateNewForm<T>(formType, key, cts));

            if (formTask.IsFaulted)
            {
                throw formTask.Exception ?? new Exception("Unknown empty exception in create Form Wrapper");
            }

            return new FormWrapper(formTask, cts);
        }

        private async Task<T> OpenFormOrActivate<T>(string key, object param) where T : FrmBaseForm
        {
            try
            {
                Trace.WriteLine($"Open form {key}");


                var formWrapper = ImmediateFormWrappers.GetOrAdd(key, x => CreateFormWrapper<T>(typeof(T), key));
                if (!(await formWrapper.FormTask is T form)) return null;
                form.MdiParent = _main;
                form.Tag = param;
                form.Activate();
                if(param !=null)
                    form.ManualActivateEvent(param);
                return form;
            }
            catch 
            {
                //Log.To.Main.AddException($"{nameof(LoadPanelHelper)}.OpenForm. Failed opening form {typeof(T).Name}.", exception);
                return null;
            }
        }
    }
}