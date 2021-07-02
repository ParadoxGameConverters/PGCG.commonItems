using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace commonItems {
    public enum LogLevel {
        Error,
        Warning,
        Info,
        Debug,
        Progress
    }

    public static class Log {
        private static bool logFileCreated = false;
        public static void WriteLine(LogLevel level, string message) {
            StringBuilder logLine = new();
            logLine.Append(logLevelStrings[level]);
            logLine.Append(message);
            Console.WriteLine(logLine);

            if (!logFileCreated) {
                System.IO.File.WriteAllText("log.txt", string.Empty);
                logFileCreated = true;
            }

            using StreamWriter logFile = File.AppendText("log.txt");
            logFile.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logFile.Write(logLine);
            logFile.Write(Environment.NewLine);
        }

        public static Dictionary<LogLevel, string> logLevelStrings = new() {
            { LogLevel.Error, "    [ERROR] " },
            { LogLevel.Warning, "  [WARNING] " },
            { LogLevel.Info, "     [INFO] " },
            { LogLevel.Debug, "    [DEBUG]         " },
            { LogLevel.Progress, " [PROGRESS] " }
        };
    }
}
