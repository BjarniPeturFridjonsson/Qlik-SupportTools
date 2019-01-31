using System;
using System.Drawing;
using FreyrViewer.Properties;

namespace FreyrViewer.Ui.Grids
{
    internal enum GridImageKind
    {
        Check,
        Cross,
        Blank16,
        Snooze,
        Timer,
        User,
        Service,
        Unknown,

        RuleThemeCompute,
        RuleThemeResources,
        RuleThemeHealth,
        RuleThemeEvents,
        RuleThemeLicensing,
    }

    internal static class GridImageHelper
    {
        public static Image GetImage(GridImageKind key)
        {
            switch (key)
            {
                //case GridImageKind.Check: return Resources.check;
                //case GridImageKind.Cross: return Resources.cross;
                //case GridImageKind.Snooze: return Resources.snooze;
                //case GridImageKind.Blank16: return Resources.blank16;
                //case GridImageKind.Timer: return Resources.timer;
                //case GridImageKind.User: return Resources.user_16x16;
                //case GridImageKind.Service: return Resources.service_16x16;
                //case GridImageKind.Unknown: return Resources.Unknown;

                //case GridImageKind.RuleThemeCompute: return Resources.RuleThemeCompute;
                //case GridImageKind.RuleThemeResources: return Resources.RuleThemeResources;
                //case GridImageKind.RuleThemeHealth: return Resources.RuleThemeHealth;
                //case GridImageKind.RuleThemeEvents: return Resources.RuleThemeEvents;
                //case GridImageKind.RuleThemeLicensing: return Resources.RuleThemeLicensing;

                default: throw new IndexOutOfRangeException($"Missing image in {nameof(GridImageHelper)}.GetImage: {key}");
            }
        }
    }
}
