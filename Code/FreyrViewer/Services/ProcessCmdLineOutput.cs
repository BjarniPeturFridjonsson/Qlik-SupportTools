using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using FreyrCommon.Models;
using Newtonsoft.Json;

namespace FreyrViewer.Services
{
    public class ProcessCmdLineOutput
    {
        public ProcessFixedWidthOutput SystemInformation { get; set; }
        public List<CmdLineResult> GroupedInfoNetwork { get; set; }
        private List<CmdLineResult> CmdResult { get; set; }
        public List<CmdLineResult> GroupedFirewall { get; set; }
        public List<CmdLineResult> GroupedServices { get; set; }
        public List<CmdLineResult> GroupedUsersAndSec { get; set; }
        public List<CmdLineResult> GroupedProcesses { get; set; }
        public List<CmdLineResult> GroupedCertifictes { get; set; }
        public List<CmdLineResult> GroupedServerInfo { get; set; }

        public ProcessCmdLineOutput ProcessJson(string json)
        {
            CmdResult = JsonConvert.DeserializeObject<List<CmdLineResult>>(json);
            ProcessAll();
            return this;
        }

        private void ProcessAll()
        {
            GroupedInfoNetwork = new List<CmdLineResult>();
            GroupedFirewall = new List<CmdLineResult>();
            GroupedServerInfo =new List<CmdLineResult>();
            GroupedServices = new List<CmdLineResult>();
            GroupedUsersAndSec = new List<CmdLineResult>();
            GroupedCertifictes = new List<CmdLineResult>();
            GroupedProcesses = new List<CmdLineResult>();

            CmdResult.ForEach(p =>
            {
                switch (p.Name)
                {
                    case "WhoAmI":
                    case "Port status":
                    case "Network Info":
                    case "Internet Connection":
                    case "Proxy Activated":
                    case "Proxy Server":
                    case "Proxy AutoConfig":
                    case "Proxy Override":
                    case "Hosts file":
                    case "UrlAclList":
                    case "PortCertList":
                    {
                        GroupedInfoNetwork.Add(p);
                       break;
                    }
                    case "Process Info":
                    case "IIS Status":
                    case "Program List":
                    case "Service List":
                    {
                        GroupedServices.Add(p);
                        break;
                    }
                    case "Firewall Info":
                    {
                       GroupedFirewall.Add(p);
                        break;
                    }
                    case "Drive Mappings":
                    case "Drive Info":
                    case "HotFixes":
                    case "PageFile":
                        {
                        GroupedServerInfo.Add(p);
                        break;
                    }
                    case "Group Policy":
                    case "Localgroup Administrators":
                    case "Localgroup Qv Administrators":
                    case "Localgroup Sense Service Users":
                    case "Localgroup Performance Monitor Users":
                    case "Localgroup Qv Api":
                    case "Local Policies - User Rights Assignment":
                    case "Local Policies - Security Options":
                    {
                        GroupedUsersAndSec.Add(p);
                        break;
                    }
                    case "Certificate - Current User(Personal)":
                    case "Certificate - Current User(Trusted Root)":
                    case "Certificate - Local Computer(Personal)":
                    case "Certificate - Local Computer(Trusted Root)":
                    {
                        GroupedCertifictes.Add(p);
                        break;
                    }

                    case "Lef file QlikTech":
                    case "Lef file QlikView":
                        {
                        break;
                    }
                    case "System Information":
                    {
                        var data = new ProcessFixedWidthOutput();
                        SystemInformation = data.ProcessFileData(p.Result);
                        break;
                    }
                }
            });
        }

        public List<SuperSimpleColumnTypes.TwoColumnType> GetResutlAsSystemInfo(string json)
        {
            

            if (string.IsNullOrEmpty(json))
                return new List<SuperSimpleColumnTypes.TwoColumnType> { new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "This file is empty." } };

            var output = JsonConvert.DeserializeObject<List<CmdLineResult>>(json);

            if (!output.Any())
                return new List<SuperSimpleColumnTypes.TwoColumnType> { new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "This file contains not data" } };

            if (string.IsNullOrEmpty(output[0].Result))
                return new List<SuperSimpleColumnTypes.TwoColumnType> { new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "There are no values in this file.", ColumnTwo = output[0].Error } };

            var ret = new List<SuperSimpleColumnTypes.TwoColumnType>();
            using (TextReader sr = new StringReader(output[0].Result))
            {
                var sHeader = sr.ReadLine()+"";
                var sValues = sr.ReadLine()+"";

                var header = sHeader.Split(new []{ "\",\"" },StringSplitOptions.None);
                var values = sValues.Split(new[] { "\",\"" }, StringSplitOptions.None);
                
                for (int i = 0; i < header.Length; i++)
                {
                    string val = values.Length < i+1 ? "" : values[i];
                    val = val.Replace("\"", "");
                    ret.Add(new SuperSimpleColumnTypes.TwoColumnType
                    {
                        ColumnOne = header[i],
                        ColumnTwo = val
                    });
                } 
            }
            return ret;
        }

        //We need to change this, because we need to datamine the values in this and therefore need to know excatly the values.
        public List<SuperSimpleColumnTypes.TwoColumnType> GetResultAsColumnsValueList<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new List<SuperSimpleColumnTypes.TwoColumnType>{new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "This file is empty." }};

            var output = JsonConvert.DeserializeObject<List<CmdLineResult>>(json);

            if (!output.Any())
                return new List<SuperSimpleColumnTypes.TwoColumnType> { new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "This file contains not data" } };

            if(string.IsNullOrEmpty(output[0].Result))
                return new List<SuperSimpleColumnTypes.TwoColumnType> { new SuperSimpleColumnTypes.TwoColumnType { ColumnOne = "There are no values in this file.",ColumnTwo = output[0].Error} };

            using (TextReader sr = new StringReader(output[0].Result))
            {
                var csv = new CsvReader(sr);//- () :
                csv.Configuration.PrepareHeaderForMatch = header => header.Replace(" ", string.Empty)
                    .Replace("_", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace(":", string.Empty)
                    .Replace("-", string.Empty);

                csv.Read();
                csv.ReadHeader();
                csv.ValidateHeader<T>();
                csv.Read();

                var record = csv.GetRecord<T>();

                PropertyInfo[] fieldInfos = typeof(T).GetProperties();

                var fields = fieldInfos.Where(fi => fi.PropertyType ==  typeof(string)).ToList();
                var ret = new List<SuperSimpleColumnTypes.TwoColumnType>();

                foreach (PropertyInfo fi in fields)
                {
                    ret.Add(new SuperSimpleColumnTypes.TwoColumnType
                    {
                        ColumnOne = fi.Name,
                        ColumnTwo = fi.GetValue(record) as string
                    });
                    
                }
               return ret;
            }
        }

        public List<T> GetCsvListResults<T>(string json)
        {
            var output = JsonConvert.DeserializeObject<List<CmdLineResult>>(json);
            using (TextReader sr = new StringReader(output[0].Result))
            {
                var csv = new CsvReader(sr);//- () :
                csv.Configuration.PrepareHeaderForMatch = header => header.Replace(" ", string.Empty)
                    .Replace("_", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty)
                    .Replace(":", string.Empty)
                    .Replace("-", string.Empty);

                csv.Read();
                csv.ReadHeader();
                csv.ValidateHeader<T>();
                var ret = new List<T>();
                while (csv.Read())
                {
                    ret.Add(csv.GetRecord<T>());
                }
                //var record = csv.GetRecord<T>();
                return ret;
            }
        }
    }
}
