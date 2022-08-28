using log4net;
using log4net.Config;
using log4net.Core;
using System.Diagnostics.CodeAnalysis;
using System.IO;
// ReSharper disable InconsistentNaming

namespace commonItems;

[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public static class Logger {
	private static readonly ILog log = LogManager.GetLogger("mainLogger");
	static Logger() {
		Configure("log4net.config");
	}

	public static void Configure(string log4netConfigPath) {
		var repository = LogManager.GetRepository();
		if (repository.Configured) {
			return;
		}

		// add custom "PROGRESS" level
		repository.LevelMap.Add(LogExtensions.ProgressLevel);

		// configure log4net
		var logConfiguration = new FileInfo(log4netConfigPath);
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

	public static void Progress(int progressValue) {
		log.Progress(progressValue);
	}
	public static void IncrementProgress() {
		log.IncrementProgress();
	}
	public static void IncrementProgress(int progressLimit) {
		log.IncrementProgress(progressLimit);
	}

	public static void Log(Level level, string message) {
		log.Log(level, message);
	}
}
