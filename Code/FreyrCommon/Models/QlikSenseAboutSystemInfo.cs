using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreyrCommon.Models
{
    public class QlikSenseAboutSystemInfo
    {
        public string SenseId { get; set; }
        public string Version { get; set; }
        public string DeploymentType { get; set; }
        public string ReleaseLabel { get; set; }
        public string ProductName { get; set; }
        public string CopyrightYearRange { get; set; }
    }
}
