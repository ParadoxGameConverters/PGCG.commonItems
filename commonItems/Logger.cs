using System.IO;
using log4net;
using log4net.Config;

namespace commonItems {
    public static class Logger {
        private static readonly ILog log = LogManager.GetLogger("mainLogger");
        static Logger() {
            // add custom "PROGRESS" level
            LogManager.GetRepository().LevelMap.Add(LogExtensions.progressLevel);

            // configure log4net
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
