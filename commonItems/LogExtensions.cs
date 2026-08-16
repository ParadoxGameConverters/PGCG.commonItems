using log4net;
using log4net.Core;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace commonItems;

[SuppressMessage("ReSharper", "IntroduceOptionalParameters.Global")]
public static class LogExtensions {
	public static Level ProgressLevel { get; } = new(35000, "PROGRESS");

	public static int CurrentProgress { get; private set; } = 0;

	public static void Progress(this ILog log, int progressValue) {
		CurrentProgress = progressValue;

		var callerStackBoundaryDeclaringType = MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(LogExtensions);
		log.Logger.Log(callerStackBoundaryDeclaringType, ProgressLevel, $"{CurrentProgress}%", null);
	}

	public static void IncrementProgress(this ILog log) {
		IncrementProgress(log, 99);
	}

	public static void IncrementProgress(this ILog log, int progressLimit) {
		if (CurrentProgress >= progressLimit) {
			Logger.Debug($"Can't increment progress above {progressLimit}.");
			return;
		}

		var callerStackBoundaryDeclaringType = MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(LogExtensions);
		log.Logger.Log(callerStackBoundaryDeclaringType, ProgressLevel, $"{++CurrentProgress}%", null);
	}

	public static void Notice(this ILog log, string message) {
		var callerStackBoundaryDeclaringType = MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(LogExtensions);
		log.Logger.Log(callerStackBoundaryDeclaringType, Level.Notice, message, null);
	}

	public static void NoticeFormat(this ILog log, string message, params object[] args) {
		string formattedMessage = string.Format(message, args);
		var callerStackBoundaryDeclaringType = MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(LogExtensions);
		log.Logger.Log(callerStackBoundaryDeclaringType, Level.Notice, formattedMessage, null);
	}

	public static void Log(this ILog log, Level level, string message) {
		var callerStackBoundaryDeclaringType = MethodBase.GetCurrentMethod()?.DeclaringType ?? typeof(LogExtensions);
		log.Logger.Log(callerStackBoundaryDeclaringType, level, message, null);
	}
}