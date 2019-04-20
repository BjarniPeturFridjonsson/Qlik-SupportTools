using System;
using Eir.Common.IO;

namespace Gjallarhorn.SenseLogReading
{
    public class SenseLogFolderFinder
    {
        public string KnownLogFoldersAsString { get; }
        public SenseLogFolderFinder()
        {
            KnownLogFoldersAsString = GetKnownLogFoldersAsString();
        }

        private SenseLogSubTypes GetSenseLogSubTypes(string dirName )
        {
            var subType = SenseLogSubTypes.Unknown;
            if (("audit,trace,system").Contains(dirName.ToLower()))
                Enum.TryParse(dirName, true, out subType);
            return subType;
        }

        public SenseLogBaseTypes GetSenseLogBaseTypes(DirectorySetting dir)
        {
            if (KnownLogFoldersAsString.Contains("," + dir.Name + ","))
            {
                SenseLogBaseTypes type;
                if (Enum.TryParse(dir.Name, true, out type))
                    return type;
            }
            return SenseLogBaseTypes.Unknown;
        }



        public BasicLogFinder Get(DirectorySetting dir)
        {
            var savedDir = dir;
            var subType = GetSenseLogSubTypes(dir.Name);
            if (subType != SenseLogSubTypes.Unknown)
                dir = dir.ParentDirectory;//we've found him jim!

            var type = GetSenseLogBaseTypes(dir);
            if(type != SenseLogBaseTypes.Unknown)
                return Get(type, subType, savedDir);

            return null;
        }

        //do something cool to remove this.. this is just a temp hack.. Im tired today.. 
        //I did!!! finally 1 year afterwards...
        public BasicLogFinder Get(SenseLogBaseTypes baseType, SenseLogSubTypes subType, DirectorySetting dir)
        {
           
            switch (baseType)
            {
                case SenseLogBaseTypes.AppMigration:
                case SenseLogBaseTypes.Script:
                case SenseLogBaseTypes.BrokerService:
                case SenseLogBaseTypes.DataProfiling:
                case SenseLogBaseTypes.HubService:
                case SenseLogBaseTypes.AboutService:
                case SenseLogBaseTypes.CapabilityService:
                case SenseLogBaseTypes.ConnectorRegistryProxy:
                case SenseLogBaseTypes.ConverterService:
                case SenseLogBaseTypes.DepGraphService:
                case SenseLogBaseTypes.DownloadPrepService:
                case SenseLogBaseTypes.OdagService:
                case SenseLogBaseTypes.WebExtensionService:
                case SenseLogBaseTypes.HybridSetupConsoleBff:
                case SenseLogBaseTypes.ResourceDistributionService:
                case SenseLogBaseTypes.DeploymentBasedWarningsService:

                    //1444921956216_969c2051-f159-4bac-a233-33bab93e0798.log whoahhahah we are susceptible to the year 2286 problem... 
                    //1444921956216_Global.log
                    //1444921956216_Request.log
                    return new NodeJsV1LogFolder(dir, baseType, SenseLogSubTypes.RootFolder);

                case SenseLogBaseTypes.Engine:
                case SenseLogBaseTypes.Proxy:
                case SenseLogBaseTypes.Printing:
                case SenseLogBaseTypes.Repository:
                case SenseLogBaseTypes.Scheduler:
                case SenseLogBaseTypes.AppDistributionService:// Todo: Bugreport this log. Its just horrible.
                case SenseLogBaseTypes.EntitlementProvisioningService:// Todo: Bugreport this log. Its just horrible. 
                case SenseLogBaseTypes.HybridDeploymentService:// Todo: Bugreport this log. Its just horrible. 
                    switch (subType)
                    {
                        case SenseLogSubTypes.Audit:
                            return new SenseV1LogFolder(dir, baseType, subType);
                        case SenseLogSubTypes.System:
                            return new SenseV1LogFolder(dir, baseType, subType);
                        case SenseLogSubTypes.Trace:
                            return new SenseV1LogFolder(dir, baseType, subType);
                        default:
                            return new SenseV1LogFolder(dir, SenseLogBaseTypes.Unknown, subType); 
                    }

                default:
                    return null;
            }
        }

        private readonly string _knownFolders = "," + string.Join(",", Enum.GetNames(typeof(SenseLogBaseTypes))) + ",";
        private string GetKnownLogFoldersAsString()
        {
            return _knownFolders;
        }
    }

   
}
