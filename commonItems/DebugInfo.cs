using Hardware.Info;
using System;
using System.Globalization;

namespace commonItems; 

public static class DebugInfo {
	public static void LogSystemInfo() {
		OperatingSystem os = Environment.OSVersion;
		Logger.DebugFormat("Operating system: {0}", os.VersionString);
		CultureInfo ci = CultureInfo.InstalledUICulture;
		Logger.DebugFormat("Installed UI language: {0}", ci.Name);
	}
	
	public static void LogCPUInfo() {
		HardwareInfo hardwareInfo;
		try {
			hardwareInfo = new();
			hardwareInfo.RefreshCPUList(includePercentProcessorTime: false);
		} catch (Exception e) {
			Logger.Debug($"Exception was raised when detecting CPUs: {e.Message}");
			return;
		}

		foreach (var cpu in hardwareInfo.CpuList) {
			Logger.Debug($"CPU: {cpu.Name}");
		}
	}

	public static void LogExecutableDirectory() {
		Logger.Debug($"Executable directory: {AppDomain.CurrentDomain.BaseDirectory}");
	}

	public static void LogEverything() {
		LogSystemInfo();
		LogCPUInfo();
		LogExecutableDirectory();
	}
}