using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media;
using FreyrViewer.Models;
using Color = System.Windows.Media.Color;

namespace FreyrViewer.Services
{
    [Flags]
    public enum LogFailureLevel
    {
        [Description("Undefined value")]
        Undefined = 0,
        [Description("Log level Warning")]
        Warning = 1 << 1,
        [Description("Log level Error")]
        Error = 1 << 2,
        [Description("Log level Error (Critical)")]
        Critical = 1 << 3,
        [Description("Proactive Warning")]
        ProactiveWarning = 1<<4,
        [Description("Proactive Error")]
        ProactiveError = 1 << 5,
        [Description("Proactive Critical Warning")]
        ProactiveCritical = 1 << 6,
    }

    public enum SimplifiedFailureLevels
    {
        None = 0, Warning=1,Error=2
    }

    public class LogFileAnalyzerService
    {
        public List<LogFileAnalyzerResult> AnalyzerResults { get; } = new List<LogFileAnalyzerResult>();
        public LogFailureLevel LogFailureLevels { get; set; }
        public SimplifiedFailureLevels SimplifiedFailureLevel { get; private set; } = SimplifiedFailureLevels.None;

        public LogFileAnalyzerService()
        {
            foreach (int value in Enum.GetValues(typeof(LogFailureLevel)))
            {
                ErrorCount[(LogFailureLevel)value] = 0;
            }
        }

        public void Analyze(int rowNr, int columnNr, string columnValue)
        {
            if (columnNr == 2 || columnNr == 1)
            {
                if (columnValue.Equals("Warning", StringComparison.InvariantCultureIgnoreCase)) AnalyzerResultsAdd(LogFailureLevel.Warning, rowNr, 10 );
                if (columnValue.Equals("WARN")) AnalyzerResultsAdd(LogFailureLevel.Warning, rowNr, 10);
                if (columnValue.Equals("Error", StringComparison.InvariantCultureIgnoreCase)) AnalyzerResultsAdd(LogFailureLevel.Error, rowNr, 10);
            }
            if (columnNr == 3)
            {
                if (columnValue.Equals("WARN")) AnalyzerResultsAdd(LogFailureLevel.Warning, rowNr, 10 );
                if (columnValue.Equals("ERROR")) AnalyzerResultsAdd(LogFailureLevel.Error, rowNr, 10);
            }
        }

        private Color LogFailureLevelToColor(LogFailureLevel level)
        {
            switch (level)
            {
                case LogFailureLevel.Undefined:
                    return Color.FromRgb(SystemColors.Control.R, SystemColors.Control.G, SystemColors.Control.B);
                case LogFailureLevel.Warning:
                    return Colors.DarkOrange;
                case LogFailureLevel.Error:
                    return Colors.Red;
                case LogFailureLevel.Critical:
                    return Colors.Red;
                case LogFailureLevel.ProactiveWarning:
                    return Colors.DarkOrange;
                case LogFailureLevel.ProactiveError:
                    return Colors.Red;
                case LogFailureLevel.ProactiveCritical:
                    return Colors.Red;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        private int _highestError;
        public Dictionary<LogFailureLevel, int> ErrorCount = new Dictionary<LogFailureLevel, int>();
        private void AnalyzerResultsAdd(LogFailureLevel level, int rowNr, int pixelWidth)
        {
            ErrorCount[level] += +1;

            LogFailureLevels |= level;
            if ((int) level > _highestError)
                _highestError = (int)level;

            AnalyzerResults.Add(new LogFileAnalyzerResult { ColorRight = LogFailureLevelToColor(level), RowNumber= rowNr, PixelWith=pixelWidth });
        }

        public void OnFinished()
        {
            switch ((LogFailureLevel)_highestError)
            {
                case LogFailureLevel.Warning:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Warning;
                    break;
                case LogFailureLevel.Error:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Error;
                    break;
                case LogFailureLevel.Critical:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Error;
                    break;
                case LogFailureLevel.ProactiveWarning:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Warning;
                    break;
                case LogFailureLevel.ProactiveError:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Error;
                    break;
                case LogFailureLevel.ProactiveCritical:
                    SimplifiedFailureLevel = SimplifiedFailureLevels.Error;
                    break;
            }
        }
    }
}
