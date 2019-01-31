using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace FreyrCollectorCommon.Winform
{

    public enum MessageLevels
    {
        Undefined = 0,
        Error = 1,
        Ok = 2,
        Warning = 3,
        Animate = 10,//we use anything below as non animated and default to ok image.
    }

    public class AnimationHelper
    {
        private readonly Form _frm;
        private int _roundRobinLen = 3;
        private string _ctrlRoundRobinName = "running_";
        public AnimationHelper(Form frm)
        {
            _frm = frm;
        }

        public void ImageFromMsgLevel(PictureBox pic, MessageLevels msgLevel)
        {
           switch (msgLevel)
            {
                case MessageLevels.Error:
                    pic.Image = Properties.Resources.Checkbox_Error;
                    pic.Name = "";
                    break;
                case MessageLevels.Warning:
                    pic.Image = Properties.Resources.Checkbox_Warning;
                    pic.Name = "";
                    break;
                case MessageLevels.Ok:
                    pic.Image = Properties.Resources.Checkbox_Ok;
                    pic.Name = "";
                    break;
                case MessageLevels.Animate:
                    if (pic.Name.StartsWith(_ctrlRoundRobinName)) break;
                    pic.Image = Properties.Resources.Checkbox_Work0;
                    pic.Name = CreateAnimControlName(0);
                    break;
                default:
                    pic.Image = Properties.Resources.Checkbox_Ok;
                    break;
            }
        }

        public void GetFirstImage(PictureBox pic,string modkey, MessageLevels msgLevel)
        {
            //no mod key. no anime for you buddy.
            if (string.IsNullOrEmpty(modkey) && msgLevel == MessageLevels.Animate && Debugger.IsAttached)
            {
                throw new Exception("Developer mind melt. this is wrong");
            }
            ImageFromMsgLevel(pic, msgLevel);            
        }

        public void AnimationTick()
        {
            bool found = false;
            int firstAnimStage = -1;
            foreach (Control ctrl in _frm.Controls)
            {
                if (ctrl.Tag != null && ctrl is PictureBox pic && pic.Name.StartsWith(_ctrlRoundRobinName))
                {
                    //all animations will sync to please the eye....
                    if (firstAnimStage < 0)
                    {
                        var curr = int.Parse(pic.Name.Substring(8));
                        curr++;
                        if (curr > _roundRobinLen) curr = 0;
                        firstAnimStage = curr;
                    }
                   
                    pic.Name = CreateAnimControlName(firstAnimStage);
                    pic.Image = GetImageFromTick(firstAnimStage);
                    found = true;
                }
            }
            if (found)
                Application.DoEvents();
        }

        private string CreateAnimControlName(int roundRobinId)
        {
            return _ctrlRoundRobinName + roundRobinId;
        }

        private Image GetImageFromTick(int roundRobinId)
        {
            switch (roundRobinId)
            {
                case 0: return Properties.Resources.Checkbox_Work0;
                case 1: return Properties.Resources.Checkbox_Work1;
                case 2: return Properties.Resources.Checkbox_Work2;
                case 3: return Properties.Resources.Checkbox_Work3;
                default:
                    if (Debugger.IsAttached) throw new Exception("tutt tutt.. Animation fail.");
                    return Properties.Resources.Checkbox_Work0;
            }
        }
    }
}
