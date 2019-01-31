using System;
using System.Reflection;
using System.Windows.Forms;

namespace FreyrViewer.Extensions
{
    public static class WinformExtensions
    {
        public static void ExecuteActionSwallowExceptions(Action action)
        {
            try
            {
                action();
            }
            catch
            {
                // Slurp...
            }
        }

        public static void OnUiThread(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                try
                {
                    control.Invoke(action);
                }
                catch (ObjectDisposedException)
                {
                    // we have timers and shit and when we change customers. this can actually happen..
                }
            }
            else
            {
                action();
            }
        }

        public static T OnUiThread<T>(this Control control, Func<T> action)
        {
            return control.InvokeRequired
                ? (T)control.Invoke(action)
                : action();
        }

        public static T OnUiThread<T>(this Control control, Func<Control, T> action)
        {
            if (control.InvokeRequired)
            {
                return (T)control.Invoke((Func<T>)(() => action(control)));
            }
            return action(control);
        }

        public static bool IsControlValid(Control myControl)
        {
            if (myControl == null) return false;
            if (myControl.IsDisposed) return false;
            if (myControl.Disposing) return false;
            if (!myControl.IsHandleCreated) return false;
            return true;
        }
        public static T Clone<T>(this T controlToClone)
            where T : Control
        {
            PropertyInfo[] controlProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            T instance = Activator.CreateInstance<T>();

            foreach (PropertyInfo propInfo in controlProperties)
            {
                if (propInfo.CanWrite)
                {
                    if (propInfo.Name != "WindowTarget")
                        propInfo.SetValue(instance, propInfo.GetValue(controlToClone, null), null);
                }
            }

            return instance;
        }
    }
}
