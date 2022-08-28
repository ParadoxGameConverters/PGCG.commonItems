using log4net;
using log4net.Core;
using System.Diagnostics.CodeAnalysis;

namespace commonItems; 

[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public static class LogExtensions {
	public static Level ProgressLevel { get; } = new(35000, "PROGRESS");

	public static int CurrentProgress { get; private set; } = 0;
	
	public static void Progress(this ILog log, int progressValue) {
		CurrentProgress = progressValue;

		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, $"{CurrentProgress}%", null);
		}
	}
	public static void IncrementProgress(this ILog log) {
		IncrementProgress(log, 99);
	}
	public static void IncrementProgress(this ILog log, int progressLimit) {
		if (CurrentProgress >= progressLimit) {
			Logger.Debug($"Can't increment progress above {progressLimit}.");
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