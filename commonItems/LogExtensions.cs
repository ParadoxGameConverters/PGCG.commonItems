using log4net;
using log4net.Core;
using System;

namespace commonItems; 

public static class LogExtensions {
	public static Level ProgressLevel { get; } = new(35000, "PROGRESS");

	public static int CurrentProgress { get; private set; } = 0;

	[Obsolete("Use Progress(this ILog log, int progress)")]
	public static void Progress(this ILog log, string message) {
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, message, null);
		}
	}
	public static void Progress(this ILog log, int progress) {
		CurrentProgress = progress;

		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, $"{CurrentProgress}%", null);
		}
	}
	public static void IncrementProgress(this ILog log, int progressLimit = 99) {
		if (CurrentProgress >= progressLimit) {
			return;
		}

		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, $"{++CurrentProgress}%", null);
		}
	}

	public static void Notice(this ILog log, string message) {
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, Level.Notice, message, null);
		}
	}
	public static void NoticeFormat(this ILog log, string message, params object[] args) {
		string formattedMessage = string.Format(message, args);
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, Level.Notice, formattedMessage, null);
		}
	}
	public static void Log(this ILog log, Level level, string message) {
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, level, message, null);
		}
	}
}