using System;

namespace Gjallarhorn.Monitors.QmsApi
{
    public class QvCalAgentDto
    {  /// <summary>
       /// The datapoint "CalAgent.QvsName"
       /// </summary>
        public string QvsName { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.NamedCalsAssigned"
        /// </summary>
        public Double NamedCalsAssigned { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.NamedCalsInLicense"
        /// </summary>
        public Double NamedCalsInLicense { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.NamedCalsUtilizationPercent"
        /// </summary>
        public Double NamedCalsUtilizationPercent { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.DocumentCalsAssigned"
        /// </summary>
        public Double DocumentCalsAssigned { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.DocumentCalsInLicense"
        /// </summary>
        public Double DocumentCalsInLicense { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.DocumentCalsUtilizationPercent"
        /// </summary>
        public Double DocumentCalsUtilizationPercent { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.SessionCalsAssigned"
        /// </summary>
        public Double SessionCalsAssigned { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.SessionCalsInLicense"
        /// </summary>
        public Double SessionCalsInLicense { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.SessionCalsUtilizationPercent"
        /// </summary>
        public Double SessionCalsUtilizationPercent { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.UsageCalsAssigned"
        /// </summary>
        public Double UsageCalsAssigned { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.UsageCalsInLicense"
        /// </summary>
        public Double UsageCalsInLicense { get; set;}

        /// <summary>
        /// The datapoint "CalAgent.UsageCalsUtilizationPercent"
        /// </summary>
        public Double UsageCalsUtilizationPercent { get; set;}
    }
}
