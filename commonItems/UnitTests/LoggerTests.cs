using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class LoggerTests {
        [Fact]
        public void ErrorMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Log(LogLevel.Error, "Error message");
            Assert.Equal("    [ERROR] Error message", output.ToString().TrimEnd());
        }

        [Fact]
        public void WarningMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Log(LogLevel.Warning, "Warning message");
            Assert.Equal("  [WARNING] Warning message", output.ToString().TrimEnd());
        }

        [Fact]
        public void InfoMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Log(LogLevel.Info, "Info message");
            Assert.Equal("     [INFO] Info message", output.ToString().TrimEnd());
        }

        [Fact]
        public void DebugMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Log(LogLevel.Debug, "Debug message");
            Assert.Equal("    [DEBUG]         Debug message", output.ToString().TrimEnd());
        }

        [Fact]
        public void ProgressMessagesLogged() {
            var output = new StringWriter();
            Console.SetOut(output);
            Logger.Log(LogLevel.Progress, "Progress message");
            Assert.Equal(" [PROGRESS] Progress message", output.ToString().TrimEnd());
        }
    }
}
