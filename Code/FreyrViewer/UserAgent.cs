using System;
using Eir.Common.Common;

namespace FreyrViewer
{
    internal static class UserAgent
    {
        public static string Compose(string name)
        {
            return $"{ApplicationName.QlikCockpit.Short}/{Environment.Version}; ({name})";
        }
    }
}
