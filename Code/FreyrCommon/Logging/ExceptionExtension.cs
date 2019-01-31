using System;

namespace FreyrCommon.Logging
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

        public static string GetStackTrace(this Exception exception)
        {
            while (exception != null)
            {
                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    return exception.StackTrace;
                }

                exception = exception.InnerException;
            }

            return null;
        }
    }
}