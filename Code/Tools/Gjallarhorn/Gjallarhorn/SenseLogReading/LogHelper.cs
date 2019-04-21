using System;
using System.IO;

namespace Gjallarhorn.SenseLogReading
{
    public static class LogHelper
    {

        public static string GetHostNameFromFile(string filePaht)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePaht);
            var parts = fileName?.Split('_');
            if (parts == null || parts.Length < 3)
                return string.Empty;

            return parts[0];
        }
        //public static bool SetPartsFromPath(LogFileDto dto)
        //{
        //    var fileName = Path.GetFileNameWithoutExtension(dto.Path);
        //    var parts = fileName?.Split('_');
        //    if (parts == null || parts.Length < 3)
        //        return false;

        //    dto.MachineName = parts[0];

        //    var datePart = RolledOverDateFromFilePart(parts[parts.Length - 1]);
        //    int len;
        //    if (datePart == DateTime.MinValue)
        //        len = parts.Length - 1;
        //    else
        //        len = parts.Length - 2;

        //    var newArr = parts.SubArray(1, len);
        //    dto.LogBaseName = string.Join("_", newArr);
        //    return true;
        //}

        public static bool DetectRollOverByExtension(string filePath)
        {
            var ext = Path.GetExtension(filePath)+"";
            if (ext.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return true;
        }

        public static bool DetectRollOverByDateTime(string filePath)
        {
            var date = GetRolledOverDate(filePath);
            return date != DateTime.MinValue;
        }

        public static DateTime GetRolledOverDate(string filePath)
        {
            var parts = filePath.Split('_');
            if (parts.Length < 3) return DateTime.MinValue;
            string filePart = parts[parts.Length - 1];
            return RolledOverDateFromFilePart(filePart);
        }

        private static DateTime RolledOverDateFromFilePart(string filePart)
        {
            if (filePart.Length < 10) return DateTime.MinValue;
            DateTime theFileDate;
            if (DateTime.TryParse(filePart.Substring(0, 10), out theFileDate))
                return theFileDate;
            return DateTime.MinValue;
        }
    }
}
