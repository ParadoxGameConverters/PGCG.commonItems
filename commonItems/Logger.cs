using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace commonItems {
    public enum LogLevel {
        Error,
        Warning,
        Info,
        Debug,
        Progress
    }

    public static class Logger {
        static Logger() {
            FileInfo logConfiguration = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logConfiguration);
        }
        private static bool logFileCreated = false;
        public static void Log(LogLevel level, string message) {
            StringBuilder logLine = new();
            logLine.Append(logLevelStrings[level]);
            logLine.Append(message);
            Console.WriteLine(logLine);

            if (!logFileCreated) {
                File.WriteAllText("log.txt", string.Empty);
                logFileCreated = true;
            }

            using (StreamWriter logFile = File.AppendText("log.txt"))
            {
                logFile.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                logFile.Write(logLine);
                logFile.Write(Environment.NewLine);
            }
        }

        private static readonly Dictionary<LogLevel, string> logLevelStrings = new() {
            { LogLevel.Error, " [ERROR] " },
            { LogLevel.Warning, " [WARNING] " },
            { LogLevel.Info, " [INFO] " },
            { LogLevel.Debug, " [DEBUG] " },
            { LogLevel.Progress, " [PROGRESS] " }
        };
    }
}
