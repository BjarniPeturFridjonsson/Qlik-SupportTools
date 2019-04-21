using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eir.Common.Extensions;
using Eir.Common.IO;
using Eir.Common.Logging;

namespace Gjallarhorn.SenseLogReading.FileMiners
{
    class AuditActivityProxyMiner : BaseDataMiner, IDataMiner
    {
        public string MinerName => "AuditActivity_Proxy";
        private int _userIdColumnNr = -1;
        private int _objectIdColumnNr = -1;
        private readonly Dictionary<string, CountTheSessions> _sessionLenght = new Dictionary<string, CountTheSessions>();
        private int _descriptionColumnNr;
        private bool _stopSessionDetected;
        private int _nrOfColumns;
        private string _currentUserName;
        private int _proxySessionIdColumnNr;
        private string _proxySessionId;


        [DebuggerDisplay("{DebugDisplay}.")]
        private class CountTheSessions
        {
            public string UserId { get; set; }
            public DateTime SessionStart { get; set; }
            public DateTime SessionEnd { get; set; }
            private string DebugDisplay => $"UserId {UserId}. Start = {SessionStart.ToString("yy-MM-dd HH:mm")}. End = {SessionEnd.ToString("yy-MM-dd HH:mm")}";
        }

        public AuditActivityProxyMiner()
        {
            base.DataMinerSettings.NeedDatePerRow = true;
        }

        public string MineFromThisLocation(string basePath, IFileSystem fileSystem)
        {
            var pathToMine = fileSystem.Path.Combine(basePath, @"Proxy\Audit");
            return fileSystem.DirectoryExists(pathToMine) ? pathToMine : "";
        }

        public void Mine(string line)
        {
            _stopSessionDetected = false;
            _currentUserName = "";
            base.MineFile(line, Analyze);
        }

        public override void InitializeNewFile(string headerLine, BasicDataFromCase basicDataFromCase, string path)
        {
            base.InitializeNewFile(headerLine, basicDataFromCase, path);

            _nrOfColumns = base.ColumnNames.Count;

            if (!base.ColumnNames.TryGetValue("userid", out _userIdColumnNr))
            {
                Log.To.Main.Add($"Failed finding userId column in log {base.CurrentFilePath}");
            }
            if (!base.ColumnNames.TryGetValue("objectid", out _objectIdColumnNr))
            {
                Log.To.Main.Add($"Failed finding objectid column in log {base.CurrentFilePath}");
            }
            if (!base.ColumnNames.TryGetValue("description", out _descriptionColumnNr))
            {
                Log.To.Main.Add($"Failed finding Description column in log {base.CurrentFilePath}");
            }
            if (!base.ColumnNames.TryGetValue("proxysessionid", out _proxySessionIdColumnNr))
            {
                Log.To.Main.Add($"Failed finding Proxy Session Id column in log {base.CurrentFilePath}");
            }
        }

        public void FinaliseStatistics()
        {
            if (base.BasicDataFromCase == null || _sessionLenght == null) return;
            var sessionLengths = new List<int>();
            foreach (var item in _sessionLenght)
            {
                if (item.Value.SessionEnd == DateTime.MinValue)
                {
                    item.Value.SessionEnd = base.DataMinerRowValues.RowDate;
                }

                var min = (int)(item.Value.SessionEnd - item.Value.SessionStart).TotalMinutes;
                if (min == 0)
                {
                    continue;
                }
                sessionLengths.Add(min);
            }

            base.BasicDataFromCase.SessionLengthAvgInMinutes = (int)Math.Round(sessionLengths.Any() ? sessionLengths.Average() : 0, MidpointRounding.AwayFromZero);
            base.BasicDataFromCase.SessionLengthMedInMinutes = (int)Math.Round(sessionLengths.Any() ? sessionLengths.Median() : 0, MidpointRounding.AwayFromZero);
            base.BasicDataFromCase.TotalNrOfSessions = sessionLengths.Count;
        }

        private void Analyze(int colNr, string value)
        {
            if (colNr == _userIdColumnNr)
            {
                _currentUserName = value;

                if (base.BasicDataFromCase.TotalUniqueActiveUsersList.ContainsKey(value))
                    base.BasicDataFromCase.TotalUniqueActiveUsersList[value]++;
                else
                    base.BasicDataFromCase.TotalUniqueActiveUsersList[value] = 1;
            }
            else if (colNr == _objectIdColumnNr)
            {
                if (base.BasicDataFromCase.TotalUniqueActiveAppsList.ContainsKey(value))
                    base.BasicDataFromCase.TotalUniqueActiveAppsList[value]++;
                else
                    base.BasicDataFromCase.TotalUniqueActiveAppsList[value] = 0;
            }
            else if (colNr == _descriptionColumnNr)
            {
                if (value.StartsWith("Command=Stop session;"))
                {
                    _stopSessionDetected = true;
                }
            }
            else if (colNr == _proxySessionIdColumnNr)
            {
                _proxySessionId = value;
            }

            if (_nrOfColumns == colNr + 1)
            {
                if (!_sessionLenght.ContainsKey(_proxySessionId))
                {
                    _sessionLenght[_proxySessionId] = new CountTheSessions()
                    {
                        UserId = _currentUserName,
                        SessionStart = base.DataMinerRowValues.RowDate
                    };
                }
                else if (_stopSessionDetected)
                {
                    _sessionLenght[_proxySessionId].SessionEnd = base.DataMinerRowValues.RowDate;
                }
            }
        }
    }
}
