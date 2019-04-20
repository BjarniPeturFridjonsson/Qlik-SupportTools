using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Eir.Common.IO;
using DirectoryInfo = System.IO.DirectoryInfo;


// ReSharper disable InconsistentNaming

namespace Gjallarhorn.SenseLogReading
{
    [Flags]
    public enum LogFilePatterns
    {
        None = 0,
        NodeJs_AppMigractionAndGuid = 1,
        NodeJs_AppMigrationGlobal = 2,
        NodeJs_AppMigrationRequest = 4,
        //engine 
        [Description("Audit Activity")]
        AuditActivity_Engine_Audit,
        QixCounter_Engine_Audit,
        Service_Engine = 8,
        System_Engine_Trace = 16,
        Performance_Engine_Trace,
        Session_Engine_Trace,
        //printing
        Service_Printing,
        Security_Printing,
        System_Printing,
        //proxy
        System_Proxy,//obsolete??
        Service_Proxy,
        AuditActivity_Proxy,
        AuditSecurity_Proxy,
        Audit_Proxy,
        Performance_Proxy,
        //repository
        AuditActivity_Repository,
        AuditSecurity_Repository,
        Service_Repository,
        Audit_Repository,
        Security_Repository,
        License_Repository,
        System_Repository,
        UserManagement_Repository,
        Synchronization_Repository,
        //scheduler
        AuditActivity_Scheduler,
        Service_Scheduler,
        Security_Scheduler,
        System_Scheduler,
        TaskExecution_Scheduler,
        //scriptin
        Scripting
    }

    public abstract class BasicLogFinder
    {
        public SenseLogMasterBaseType LogMasterBaseType { get; }
        public SenseLogBaseTypes LogBaseTypes { get; }
        public SenseLogSubTypes LogSubTypes { get; }
        public DirectorySetting Directory { get; }
        public abstract List<SenseLogSubTypes> BaseTypeSubTypeSupport { get; }
        public List<LogFilePatterns> FilePatterns { get; set;} 
        

        public List<FileInfo> Files { get; protected set; }

        protected BasicLogFinder(SenseLogMasterBaseType logMasterBaseType, SenseLogBaseTypes senseLogBaseTypes, SenseLogSubTypes logSubType, DirectorySetting directory)
        {
            LogMasterBaseType = logMasterBaseType;
            LogBaseTypes = senseLogBaseTypes;
            LogSubTypes = logSubType;
            Directory = directory;
        }

        public BasicLogFinder AddFilePatterns(params LogFilePatterns[] patterns)
        {
            var a = new List<LogFilePatterns>();
            foreach (var var in patterns)
            {
                a.Add(var);
            }
            FilePatterns = a;
            return this;
        }

        protected List<FileInfo> GetFiles(DateTime from, DateTime to)
        {
            var ret = new List<FileInfo>();
            DirectoryInfo info = new DirectoryInfo(Directory.Path);
            var files = info.GetFiles().OrderBy(p => p.CreationTime);
            foreach (FileInfo file in files)
            {
                if (file.CreationTime >= from && file.CreationTime <= to)
                    ret.Add(file);
            }
            return ret;
        }

        public abstract List<FileInfo> FindFiles(DateTime from, DateTime to, SenseLogBaseTypes acceptedFiles);
    }

    public class SenseV1LogFolder : BasicLogFinder
    {
        public override List<SenseLogSubTypes> BaseTypeSubTypeSupport { get; } = new List<SenseLogSubTypes> { SenseLogSubTypes.Audit, SenseLogSubTypes.System, SenseLogSubTypes.Trace, SenseLogSubTypes.Engine }; 
        public SenseV1LogFolder(DirectorySetting directory, SenseLogBaseTypes senseLogBaseTypes, SenseLogSubTypes logSubType) : base(SenseLogMasterBaseType.SenseV1, senseLogBaseTypes, logSubType, directory)
        {
            
        }

        public override List<FileInfo> FindFiles(DateTime from, DateTime to, SenseLogBaseTypes acceptedFiles)
        {
            var dateMatched = GetFiles(from, to);
            var ret = new List<FileInfo>();

            if (acceptedFiles == SenseLogBaseTypes.Unknown)
                return dateMatched;

           
            foreach (FileInfo fi in dateMatched)
            {
                ret.Add(fi);
            }
            Files = ret;
            return ret;
        }
    }

    public class ScriptV1LogFolder : BasicLogFinder
    {
        public override List<SenseLogSubTypes> BaseTypeSubTypeSupport { get; } = new List<SenseLogSubTypes> { SenseLogSubTypes.RootFolder};
        public ScriptV1LogFolder(DirectorySetting directory, SenseLogBaseTypes senseLogBaseTypes, SenseLogSubTypes logSubType) : base(SenseLogMasterBaseType.ScriptV1, senseLogBaseTypes, logSubType, directory)
        {

        }

        public override List<FileInfo> FindFiles(DateTime from, DateTime to, SenseLogBaseTypes acceptedFiles)
        {
            var dateMatched = GetFiles(from, to);
            var ret = new List<FileInfo>();

            if (acceptedFiles == SenseLogBaseTypes.Unknown)
                return dateMatched;
            
            foreach (FileInfo fi in dateMatched)
            {
                ret.Add(fi);
            }
            Files = ret;
            return ret;
        }
    }

    public class NodeJsV1LogFolder : BasicLogFinder
    {
        public override List<SenseLogSubTypes> BaseTypeSubTypeSupport { get; } = new List<SenseLogSubTypes> { SenseLogSubTypes.RootFolder };
        public NodeJsV1LogFolder(DirectorySetting directory, SenseLogBaseTypes senseLogBaseTypes, SenseLogSubTypes logSubType) : base(SenseLogMasterBaseType.NodeJsV1, senseLogBaseTypes, logSubType, directory)
        {
            //Severity= info
            //Date =2015-10-15T15:12:37.304Z
            //1444921956216_969c2051-f159-4bac-a233-33bab93e0798.log 
            //1444921956216_Global.log
            //1444921956216_Request.log
            //Is file within date span
        }
       

        public override List<FileInfo> FindFiles(DateTime from, DateTime to, SenseLogBaseTypes acceptedFiles)
        {
            var dateMatched = GetFiles(from,to);
            var ret = new List<FileInfo>();

            if (acceptedFiles == SenseLogBaseTypes.Unknown)
                return dateMatched;
            foreach (FileInfo fi in dateMatched)
            {
                //if(!(IsEpochAtStart(fi.Name) && fi.Extension == ".log"))
                //    continue;
                //if (acceptedFiles == LogFilePatterns.NodeJs_AppMigractionAndGuid  && fi.Name.Substring(23) == "-"
                //    ||
                //    acceptedFiles == LogFilePatterns.NodeJs_AppMigrationGlobal && fi.Name.Substring(13, 7) == "_Global"
                //    ||
                //    acceptedFiles == LogFilePatterns.NodeJs_AppMigrationRequest  && fi.Name.Substring(13, 8) == "_Request")
                {
                    ret.Add(fi);
                }
            }
            Files = ret;
            return ret;
        }

        //private bool IsEpochAtStart(string name)
        //{
        //    double n;
        //    return double.TryParse(name.Substring(0, 13), out n);//whoahhahah we are supticle to the year 2286 problem... 
        //}
    }
}
