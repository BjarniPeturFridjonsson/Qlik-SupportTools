using System;

namespace Eir.Common.Logging
{
    public interface ILogFileNameComposer
    {
        string GetDatePart(DateTime timestamp);

        string GetFileName(string datePart);

        string GetFileName(string datePart, string roll);

        bool TryParse(string fileName, out string datePart, out string roll);
    }
}