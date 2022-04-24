using log4net;
using log4net.Core;

namespace commonItems; 

public static class LogExtensions {
	public static readonly Level ProgressLevel = new(35000, "PROGRESS");

	public static void Progress(this ILog log, string message) {
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, message, null);
		}
	}
	public static void ProgressFormat(this ILog log, string message, params object[] args) {
		string formattedMessage = string.Format(message, args);
		var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
		if (currentMethod is not null) {
			log.Logger.Log(currentMethod.DeclaringType, ProgressLevel, formattedMessage, null);
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
