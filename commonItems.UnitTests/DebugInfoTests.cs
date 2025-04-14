using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
public class DebugInfoTests {
	[Fact]
	public void LogSystemInfoLogsOSAndLanguage() {
		var output = new StringWriter();
		Console.SetOut(output);

		DebugInfo.LogSystemInfo();
		var result = output.ToString();
		Assert.Contains("Operating system: ", result);
		Assert.Contains("Installed UI language: ", result);
	}

	[Fact]
	public void LogCPUInfoLogsCPU() {
		var output = new StringWriter();
		Console.SetOut(output);

		DebugInfo.LogCPUInfo();
		var result = output.ToString();
		Assert.Contains("CPU: ", result);
	}

	[SkippableFact]
	public void LogAntivirusInfoLogsNothingOnNonWindows() {
		Skip.If(OperatingSystem.IsWindows(), "This test is only for non-Windows platforms.");

		var output = new StringWriter();
		Console.SetOut(output);

		DebugInfo.LogAntivirusInfo();
		var result = output.ToString();
		Assert.Empty(result);
	}

	[Fact]
	public void LogEverythingLogsAllInfo() {
		var output = new StringWriter();
		Console.SetOut(output);

		DebugInfo.LogEverything();
		var result = output.ToString();

		Assert.Contains("Operating system: ", result);
		Assert.Contains("Installed UI language: ", result);
		Assert.Contains("CPU: ", result);
		Assert.Contains("Executable directory: ", result);
		if (OperatingSystem.IsWindows()) {
			Assert.Contains("Found antivirus: ", result);
		} else {
			// Antivirus info is not collected on non-Windows platforms.
			Assert.DoesNotContain("Found antivirus: ", result);
		}
	}
}