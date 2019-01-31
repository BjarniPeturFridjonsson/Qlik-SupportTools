using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class QvLicenceDto
    {
        /// <summary>
        /// The datapoint "LicenseAgent.ExpireDate"
        /// </summary>
        public  DateTime ExpireDate { get; set; }

        /// <summary>
        /// The datapoint "LicenseAgent.LicenseSerialNo"
        /// </summary>
        public  string LicenseSerialNo { get; set; }

        /// <summary>
        /// The datapoint "LicenseAgent.LicenseType"
        /// </summary>
        public  string LicenseType { get; set; }
    }
}
