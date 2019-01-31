using System;
using System.Linq;
using System.Threading.Tasks;
using FreyrViewer.Enums;

namespace FreyrViewer.Common
{
    public class MenuItemWrapper
    {
        public MenuItemWrapper(string text, Func<Task> menuAction, Type formType, ApplicationMenuIcon menuIcon)
        {
            Text = text;
            ApplicationMenuIcon = menuIcon;
            MenuAction = menuAction;
            FormType = formType;
            //Access = access;
            SubMenuItems = new MenuItemWrapper[0];

        }

        public MenuItemWrapper(string text, Func<Task> menuAction, object formTag, MenuItemWrapper[] subMenuItems, ApplicationMenuIcon menuIcon)
        {
            Text = text;
            ApplicationMenuIcon = menuIcon;
            //Access = access;
            MenuAction = menuAction;
            FormType = null;
            FormTagValue = formTag;
            SubMenuItems = subMenuItems.Where(x => x != null).ToArray();
        }

        public string Text { get; set; }

        //public MenuItemAccess Access { get; }

        //public MenuIcon MenuIcon { get; }

        public Func<Task> MenuAction { get; }

        public Type FormType { get; }
        public object FormTagValue { get; }
        public string Key { get; set; }
        public ApplicationMenuIcon ApplicationMenuIcon { get; set; }

        public MenuItemWrapper[] SubMenuItems { get; set; }
    }
}