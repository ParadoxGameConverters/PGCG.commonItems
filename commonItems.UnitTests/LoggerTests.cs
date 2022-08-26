using log4net.Core;
using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class LoggerTests {
	[Fact]
	public void ErrorMessagesLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Error("Error message");
		Assert.Contains("[ERROR] Error message", output.ToString().TrimEnd());
	}
	[Fact]
	public void ErrorMessagesFormatLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.ErrorFormat("Error {0}", "message");
		Assert.Contains("[ERROR] Error message", output.ToString().TrimEnd());
	}

	[Fact]
	public void WarningMessagesLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Warn("Warning message");
		Assert.Contains("[WARN] Warning message", output.ToString());
	}
	[Fact]
	public void WarningMessagesFormatLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.WarnFormat("Warning {0}", "message");
		Assert.Contains("[WARN] Warning message", output.ToString());
	}

	[Fact]
	public void InfoMessagesLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Info("Info message");
		Assert.Contains("[INFO] Info message", output.ToString().TrimEnd());
	}
	[Fact]
	public void InfoMessagesFormatLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.InfoFormat("Info {0}", "message");
		Assert.Contains("[INFO] Info message", output.ToString().TrimEnd());
	}

	[Fact]
	public void NoticeMessagesLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Notice("Notice message");
		Assert.Contains("[NOTICE] Notice message", output.ToString());
	}
	[Fact]
	public void NoticeMessagesFormatLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.NoticeFormat("Notice {0}", "message");
		Assert.Contains("[NOTICE] Notice message", output.ToString());
	}

	[Fact]
	public void DebugMessagesLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Debug("Debug message");
		Assert.Contains("[DEBUG] Debug message", output.ToString());
	}
	[Fact]
	public void DebugMessagesFormatLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.DebugFormat("Debug {0}", "message");
		Assert.Contains("[DEBUG] Debug message", output.ToString());
	}

	[Fact]
	public void ProgressMessagesAreLogged() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Progress(42);
		Assert.Contains("[PROGRESS] 42%", output.ToString());
	}
	[Fact]
	public void ProgressCanBeIncremented() {
		var output = new StringWriter();
		Console.SetOut(output);
		
		Logger.Progress(20);
		Assert.Contains("[PROGRESS] 20%", output.ToString());
		
		Logger.IncrementProgress();
		Assert.Contains("[PROGRESS] 21%", output.ToString());
	}

	[Fact]
	public void ProgressIncrementationCanBeLimited() {
		var output = new StringWriter();
		Console.SetOut(output);
		
		Logger.Progress(20);
		Assert.Contains("[PROGRESS] 20%", output.ToString());

		const int incrementLimit = 21;
		
		Logger.IncrementProgress(progressLimit: incrementLimit); // fits the limit
		Assert.Contains("[PROGRESS] 21%", output.ToString());
		
		Logger.IncrementProgress(progressLimit: incrementLimit); // doesn't fit the limit
		Assert.DoesNotContain("[PROGRESS] 22%", output.ToString());
		Assert.Contains("[DEBUG] Can't increment progress above 21.", output.ToString());
	}
	
	[Fact]
	public void LevelCanBePassedAsArgument() {
		var output = new StringWriter();
		Console.SetOut(output);
		Logger.Log(Level.Debug, "1");
		Logger.Log(Level.Info, "2");
		Logger.Log(Level.Warn, "3");
		Logger.Log(Level.Error, "4");
		Logger.Log(Level.Notice, "5");
		Logger.Log(LogExtensions.ProgressLevel, "6");
		Logger.Log(Level.Alert, "7");

		var outStr = output.ToString();
		Assert.Contains("[DEBUG] 1", outStr);
		Assert.Contains("[INFO] 2", outStr);
		Assert.Contains("[WARN] 3", outStr);
		Assert.Contains("[ERROR] 4", outStr);
		Assert.Contains("[NOTICE] 5", outStr);
		Assert.Contains("[PROGRESS] 6", outStr);
		Assert.Contains("[ALERT] 7", outStr);
	}
}