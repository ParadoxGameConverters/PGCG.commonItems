using log4net;
using log4net.Config;
using System.IO;

namespace commonItems;

public enum LogLevel {
	Debug,
	Info,
	Warn,
	Error,
	Notice,
	Progress
}

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
	public static void ErrorFormat(string message, params object[] args) {
		log.ErrorFormat(message, args);
	}
	public static void Warn(string message) {
		log.Warn(message);
	}
	public static void WarnFormat(string message, params object[] args) {
		log.WarnFormat(message, args);
	}
	public static void Info(string message) {
		log.Info(message);
	}
	public static void InfoFormat(string message, params object[] args) {
		log.InfoFormat(message, args);
	}
	public static void Notice(string message) {
		log.Notice(message);
	}
	public static void NoticeFormat(string message, params object[] args) {
		log.NoticeFormat(message, args);
	}
	public static void Debug(string message) {
		log.Debug(message);
	}
	public static void DebugFormat(string message, params object[] args) {
		log.DebugFormat(message, args);
	}
	public static void Progress(string message) {
		log.Progress(message);
	}
	public static void ProgressFormat(string message, params object[] args) {
		log.ProgressFormat(message, args);
	}

	public static void Log(LogLevel level, string message) {
		switch (level) {
			case LogLevel.Debug: log.Debug(message); break;
			case LogLevel.Info: log.Info(message); break;
			case LogLevel.Warn: log.Warn(message); break;
			case LogLevel.Error: log.Error(message); break;
			case LogLevel.Notice: log.Notice(message); break;
			case LogLevel.Progress: log.Progress(message); break;
			default: log.Debug(message); break;
		}
	}
}