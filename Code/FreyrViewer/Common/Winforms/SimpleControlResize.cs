using System;
using System.Drawing;
using System.Windows.Forms;

namespace FreyrViewer.Common.Winforms
{

    //todo: Create a dict of controls and store org size so we can do more fancy resizing.
    public class SimpleControlResize
    {
        private Single _currentResizeBy;
        private Single _delta;

        public void SetCorrectFontSize(Control.ControlCollection controls)
        {
            if (_delta != 0f)
            {
                ResizeAllControls(controls, _delta);
                return;
            }
            if(_currentResizeBy != 0f)
                ResizeAllControls(controls, _currentResizeBy);
        }

        public void ResetDelta()
        {
            _delta = 0f;
        }

        public void Smaller()
        {
            _delta = -2f;
            _currentResizeBy -= 2f;
        }

        public void Larger()
        {
            _delta = 2f;
            _currentResizeBy += 2f;
        }

        private void ResizeAllControls(Control.ControlCollection controls, Single resizeBy)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is Button)
                    continue;
                //var a = ctrl.ShouldSerializeForeColor();
                //var test = Attribute.GetCustomAttributes(typeof(Control).GetProperty("Font"));
                //var a = Attribute.GetCustomAttributes(typeof(Control).GetProperty("Font")).FirstOrDefault(p=> p.TypeId.ToString().Equals("System.ComponentModel.AmbientValueAttribute"));
                //if(a != null)
                //    continue;
                if(!ctrl.Font.Equals(ctrl.Parent.Font))//ok bad. but hard to fix. your font could be inherited.
                    ctrl.Font = new Font(ctrl.Font.FontFamily, ctrl.Font.Size + resizeBy);

                if (ctrl.HasChildren)
                    ResizeAllControls(ctrl.Controls, resizeBy);

            }

            if (controls[0].Parent is Form frmParent)
            {
                frmParent.Font = new Font(frmParent.Font.FontFamily, frmParent.Font.Size + resizeBy);
            }
        }
    }
}
