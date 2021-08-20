using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class LoggerTests {
        [Fact]
        public void ErrorMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Error("Error message");
            Assert.Equal("[ERROR] Error message", output.ToString().TrimEnd());
        }

        [Fact]
        public void WarningMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Warn("Warning message");
            Assert.Equal("[WARN] Warning message", output.ToString().TrimEnd());
        }

        [Fact]
        public void InfoMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Info("Info message");
            Assert.Equal("[INFO] Info message", output.ToString().TrimEnd());
        }

        [Fact]
        public void DebugMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Debug("Debug message");
            Assert.Equal("[DEBUG] Debug message", output.ToString().TrimEnd());
        }

        [Fact]
        public void ProgressMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Progress("Progress message");
            Assert.Equal("[PROGRESS] Progress message", output.ToString().TrimEnd());
        }
    }
}
