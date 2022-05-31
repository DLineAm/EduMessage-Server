using System;

namespace SignalIRServerTest.Services
{
    public static class StringDecorator
    {
        private static string DecorateLogString(string message)
        {
            var verticalBorder = "";
            for (int i = 0; i < message.Length + 6; i++)
            {
                verticalBorder += "=";
            }
            string result = "\n== " + message + " ==\n" + verticalBorder + "\n";
            return result;
        }

        public static string GetDecoratedLogString(Type exceptionType, string methodName)
        {
            var logString = "Method " + methodName + " throws an " + exceptionType.Name + " in " + DateTime.Now;
            logString = DecorateLogString(logString);
            return logString;
        }
    }
}