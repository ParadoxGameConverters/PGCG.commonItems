using log4net;
using log4net.Core;

namespace commonItems {
    public static class LogExtensions {
        static readonly Level progressLevel = new(35000, "PROGRESS");

        public static void Progress(this ILog log, string message) {
            var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
            if (currentMethod is not null) {
                log.Logger.Log(currentMethod.DeclaringType,
                progressLevel, message, null);
            }

        }

        public static void ProgressFormat(this ILog log, string message, params object[] args) {
            string formattedMessage = string.Format(message, args);
            var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
            if (currentMethod is not null) {
                log.Logger.Log(currentMethod.DeclaringType,
                progressLevel, formattedMessage, null);
            }
        }
    }
}
