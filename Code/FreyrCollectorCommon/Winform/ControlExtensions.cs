using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace FreyrCollectorCommon.Winform
{
    public static class ControlExtensions
    {
        public static void MakeLabelTransparent(this Form frm, Label label, Control parent)
        {
            var pos = frm.PointToScreen(label.Location);
            pos = parent.PointToClient(pos);
            label.Parent = parent;
            label.Location = pos;
            label.BackColor = Color.Transparent;
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

        public static DialogResult ShowDialogueCenterParent(this Form dialogue, Form parent)
        {
            dialogue.StartPosition = FormStartPosition.CenterParent;
            return dialogue.ShowDialog(parent);
        }
    }
}
