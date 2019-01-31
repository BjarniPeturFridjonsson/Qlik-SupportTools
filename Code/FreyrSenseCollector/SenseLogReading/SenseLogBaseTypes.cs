using System;

namespace FreyrSenseCollector.SenseLogReading
{

    /// <summary>
    /// These are the correct folder names for each service being logged.
    /// </summary>
    [Flags]
    public enum SenseLogBaseTypes
    {
        Unknown =0,
        All =1,
        AppMigration = 1 << 1 ,
        DataProfiling = 1 << 2,
        Engine = 1 << 3,
        Printing = 1 << 4,
        Proxy = 1 << 5,
        Repository = 1 << 6,
        Scheduler = 1 << 7,
        Script = 1 << 8,
        BrokerService = 1 << 9,
        HubService = 1 << 10,
        AboutService = 1 << 11,
        CapabilityService = 1 << 12,
        ConnectorRegistryProxy = 1 << 13,
        ConverterService = 1 << 14,
        DepGraphService = 1 << 15,
        DownloadPrepService = 1 << 16,
        OdagService = 1 << 17,  
        WebExtensionService = 1 << 18,
        AppDistributionService = 1 << 19,
        EntitlementProvisioningService = 1 << 20,
        HybridDeploymentService = 1 << 21,
        HybridSetupConsoleBff = 1 << 22,
        ResourceDistributionService = 1 << 23,
        DeploymentBasedWarningsService = 1 << 24,


    }

    /// <summary>
    /// These are the subFolders for the log base types. 
    /// <para>The rootFolder value is used for the node.js logs which don't have the same structure but dumps their logs in the root of the folder</para>
    /// </summary>
    [Flags]
    public enum SenseLogSubTypes
    {
        Unknown = 0,
        Audit = 1,
        System = 1<<1,
        Trace = 1<<2,
        Engine = 1 << 3,
        //sense nodeJs and such which do not confirm to the default type of subfolders
        RootFolder = 1<<4,
    }

    public enum SenseLogMasterBaseType
    {//we estimate that these will change over time
        SenseV1, NodeJsV1, ScriptV1
    }

    //// ReSharper disable InconsistentNaming
    ///// <summary>
    ///// the name reflects the correct file name in the Log directory
    ///// <para>We don't have the extension because it will change when rolling</para>
    ///// <para>We dont have the start of the file becuase that is the </para>
    ///// </summary>
    //public enum KnownLogBaseNames
    //{
    //    //engine 
    //    Service_Engine, System_Engine,
    //    //proxy
    //    System_Proxy,
    //    //repository
    //    Service_Repository, License_Repository, System_Repository, UserManagement_Repository, Synchronization_Repository
    //}
}
