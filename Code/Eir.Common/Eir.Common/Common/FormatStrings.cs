using System;

namespace Eir.Common.Common
{
    public static class FormatStrings
    {
        public const string DATE = "yyyy'-'MM'-'dd";
        public const string TIME = "HH':'mm':'ss";
        public const string TIME_WITH_MILLISECONDS = TIME + "'.'fff";
        public const string DATE_AND_TIME = DATE + " " + TIME;
        public const string DATE_AND_TIME_WITH_MILLISECONDS = DATE + " " + TIME_WITH_MILLISECONDS;
        public const string UTC_DATE_AND_TIME_WITH_MILLISECONDS = DATE + "T" + TIME_WITH_MILLISECONDS + "Z";


       
    }
}