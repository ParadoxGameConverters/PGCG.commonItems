using System.IO;
using log4net;
using log4net.Config;
using log4net.Core;

namespace commonItems {
    public static class Logger {
        static readonly ILog log = LogManager.GetLogger("mainLogger");
        static Logger() {
            // add custom "Progress" level
            var progressLevel = new Level(35000, "PROGRESS");
            LogManager.GetRepository().LevelMap.Add(progressLevel);

            var logConfiguration = new FileInfo("log4net.config");
            XmlConfigurator.Configure(logConfiguration);
        }
        public static void Error(string message) {
            log.Error(message);
        }
        public static void Warn(string message) {
            log.Warn(message);
        }
        public static void Info(string message) {
            log.Info(message);
        }
        public static void Debug(string message) {
            log.Debug(message);
        }
        public static void Progress(string message) {
            log.Progress(message);
        }
    }
}
