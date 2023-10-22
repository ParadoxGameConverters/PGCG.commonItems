using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable InconsistentNaming

namespace commonItems;

[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public static class Logger {
	private static readonly ILog log = LogManager.GetLogger("mainLogger");

	static Logger() {
		Configure();
	}

	public static void Configure(bool logToConsole = true, bool logToFile = false) {
		var repository = LogManager.GetRepository();
		if (repository.Configured) {
			return;
		}

		var hierarchy = (Hierarchy)repository;
		hierarchy.Root.RemoveAllAppenders();

		// Add custom "PROGRESS" level.
		repository.LevelMap.Add(LogExtensions.ProgressLevel);

		var layout = new PatternLayout {
			ConversionPattern = "%date{yyyy'-'MM'-'dd HH':'mm':'ss} [%level] %message%newline",
		};
		layout.ActivateOptions();
		if (logToConsole) {
			var consoleAppender = new ConsoleAppender {
				Threshold = Level.All, Target = "Console.Out", Layout = layout,
			};
			consoleAppender.ActivateOptions();
			hierarchy.Root.AddAppender(consoleAppender);
		}
		if (logToFile) {
			var fileAppender = new RollingFileAppender {
				Name = "file",
				File = "log.txt",
				AppendToFile = true,
				RollingStyle = RollingFileAppender.RollingMode.Size,
				MaxSizeRollBackups = 5,
				MaximumFileSize = "100MB",
				StaticLogFileName = true,
				Layout = layout,
				Threshold = Level.All,
			};
			fileAppender.ActivateOptions();
			hierarchy.Root.AddAppender(fileAppender);
		}

		hierarchy.Root.Level = Level.All;
		hierarchy.Configured = true;
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