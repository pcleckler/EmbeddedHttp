using CleckTech.EmbeddedHttp;
using System;

namespace CleckTech.EmbeddedHttpConsole
{
    internal class Logger : ILogger
    {
        public void Debug(string message)
        {
            this.LogMessage("DEBUG", message);
        }

        public void Error(string message)
        {
            this.LogMessage("ERROR", message);
        }

        public void Info(string message)
        {
            this.LogMessage("INFO", message);
        }

        private void LogMessage(string category, string message)
        {
            Console.WriteLine($"{DateTime.Now} - {category} - {message}");
        }
    }
}