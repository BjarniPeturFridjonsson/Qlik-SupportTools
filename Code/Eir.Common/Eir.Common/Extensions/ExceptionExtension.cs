using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Eir.Common.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetNestedMessages(this Exception exception)
        {
            string message = exception.Message;
            exception = exception.InnerException;

            while (exception != null)
            {
                message += " -> " + exception.Message;
                exception = exception.InnerException;
            }

            return message;
        }

        public static string ToLogLine(this Exception exception, bool asOneLine = true)
        {
            exception = GetRealException(exception);

            string text = TryGetBodyFromWebException(exception as WebException) + exception;

            return asOneLine ? text.ReplaceControlChars() : text;
        }

        private static string TryGetBodyFromWebException(WebException webException)
        {
            try
            {
                var stream = webException?.Response?.GetResponseStream();

                if (stream == null)
                {
                    return null;
                }

                using (stream)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return "Web exception: \"" + reader.ReadToEnd() + "\"\r\n";
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private static Exception GetRealException(Exception exception)
        {
            try
            {
                var aggregateException = exception as AggregateException;
                if (aggregateException != null)
                {
                    return GetRealException(aggregateException.Flatten().InnerException);
                }

                if (exception is TargetInvocationException)
                {
                    return GetRealException(exception.InnerException);
                }
            }
            catch
            {
            }

            return exception;
        }

    }
}