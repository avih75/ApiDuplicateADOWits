using System;

namespace WebHookServer.Models
{
    public enum LogLevel
    {
        Inform,
        Attention,
        Minor,
        Medium,
        Meijur,
        Critical
    }
    public class Logger
    {
        string LoggerFile { get; set; }
        bool WriteToLog { get; set; }
        public Logger(string loggerFilePath)
        {
            if (loggerFilePath == "")
            {
                WriteToLog = false;
            }
            else
            {
                WriteToLog = true;
            }
            LoggerFile = loggerFilePath;
            using (System.IO.StreamWriter sw = System.IO.File.AppendText(LoggerFile))
            {
                sw.WriteLine(".");
                sw.WriteLine("## ## ## Start new Log Senario " + DateTime.Now + " ## ## ##");
                sw.WriteLine(".");
            }
        }
        public void WriteLog(string message, LogLevel level)
        {
            if (WriteToLog)
            {
                using (System.IO.StreamWriter sw = System.IO.File.AppendText(LoggerFile))
                {
                    sw.WriteLine(level.ToString() + " : " + message + " at " + DateTime.Now);
                }
            }
            if (level == LogLevel.Critical)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(level.ToString() + " : " + message);
                Console.ResetColor();
            }
            else if (level == LogLevel.Meijur)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(level.ToString() + " : " + message);
                Console.ResetColor();
            }
            else if (level == LogLevel.Attention)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(level.ToString() + " : " + message);
                Console.ResetColor();
            }
            else
                Console.WriteLine(level.ToString() + " : " + message);
        }
        public void WriteLogAllExceptionError(Exception mainException)
        {
            WriteLog(" - Exception: " + mainException.Message, LogLevel.Critical);
            if (mainException.InnerException != null)
            {
                WriteLog(" - Inner exception: " + mainException.InnerException.Message, LogLevel.Meijur);
                WriteLogAllExceptionError(mainException.InnerException);
            }
        }
    }
}