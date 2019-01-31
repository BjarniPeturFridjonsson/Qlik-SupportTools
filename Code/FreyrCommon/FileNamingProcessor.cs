using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using Eir.Common.Common;
using FreyrCommon.Models;

namespace FreyrCommon
{
    public class FileNamingProcessor
    {
        [Flags]
        private enum SenseCollectorVariableTypes
        {
            Unknown = 0,
            AllowWindowsLogs = 1 << 0,
            AllowMachineInfo = 1 << 1,
            MainLogs = 1 << 2,
            ScriptLogs = 1 << 3,
            PrintingLogs = 1 << 4,
            UseOnlineDelivery = 1 << 5,
        }
        private readonly Base62NumberEncoder _encoder = new Base62NumberEncoder();
        /// <summary>
        /// This is used by Product Support for being able to embedd variables for the executable in the filename.
        /// This enables them to send generic exe that contains case number and dates needed for the tool.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public CommonCollectorServiceVariables DecodeFileName(string filename)
        {
            var ret = new CommonCollectorServiceVariables();
           
            if (filename.Length < 17 || !filename.Contains("_"))
                return ret;
            if (filename.Substring(filename.Length - 4, 1) == ".")
                filename = Path.GetFileNameWithoutExtension(filename);

            var filepart = filename.Substring(16);
            var valueString = _encoder.Decode(filepart).ToString();
            ret.StartDateForLogs = DateTime.ParseExact(valueString.Substring(0,6),"yyMMdd",CultureInfo.InvariantCulture);
            ret.StopDateForLogs = DateTime.ParseExact(valueString.Substring(6, 6), "yyMMdd", CultureInfo.InvariantCulture);
            SenseCollectorVariableTypes flags ;
            Enum.TryParse(valueString.Substring(12, 3), out flags);
            ret.Key = valueString.Substring(15);
            ret.AllowWindowsLogs = flags.HasFlag(SenseCollectorVariableTypes.AllowWindowsLogs);
            ret.AllowMachineInfo = flags.HasFlag(SenseCollectorVariableTypes.AllowMachineInfo);
            ret.UseOnlineDelivery = flags.HasFlag(SenseCollectorVariableTypes.UseOnlineDelivery);
            ret.GetLogsMain = flags.HasFlag(SenseCollectorVariableTypes.MainLogs);
            ret.GetLogsPrinting = flags.HasFlag(SenseCollectorVariableTypes.PrintingLogs);
            ret.GetLogsScripting = flags.HasFlag(SenseCollectorVariableTypes.ScriptLogs);
            return ret;
        }


        public string CreateFileNameProperties(CommonCollectorServiceVariables variables)
        {
            SenseCollectorVariableTypes flags = SenseCollectorVariableTypes.Unknown;
            if(variables.AllowWindowsLogs) flags |= SenseCollectorVariableTypes.AllowWindowsLogs;
            if (variables.AllowMachineInfo) flags |= SenseCollectorVariableTypes.AllowMachineInfo;
            if (variables.UseOnlineDelivery) flags |= SenseCollectorVariableTypes.UseOnlineDelivery;
            if (variables.GetLogsMain) flags |= SenseCollectorVariableTypes.MainLogs;
            if (variables.GetLogsPrinting) flags |= SenseCollectorVariableTypes.PrintingLogs;
            if (variables.GetLogsScripting) flags |= SenseCollectorVariableTypes.ScriptLogs;

            var str = variables.StartDateForLogs.ToString("yyMMdd") +
                    variables.StopDateForLogs.ToString("yyMMdd") +
                    ((int)flags).ToString().PadLeft(3,'0') +
                    variables.Key + "";
            
            var filepart = _encoder.Encode(BigInteger.Parse(str));
            var test = _encoder.Decode(filepart).ToString();
            if (!test.Equals(str))
            {
                throw new Exception("whow somehow our magic string thingy did not generate a correct output.");
            }
            
            return filepart;
        }
        private int ConvertBool(bool val)
        {
            return val ? 1 : 0;
        }
        private bool ConvertBool(int val)
        {
            return val == 1;
        }
    }
    
}
