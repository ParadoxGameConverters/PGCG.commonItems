using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace commonItems; 

public static class DebugInfo {
	public static void LogSystemInfo() {
		OperatingSystem os = Environment.OSVersion;
		Logger.DebugFormat("Operating system: {0}", os.VersionString);
		CultureInfo ci = CultureInfo.InstalledUICulture;
		Logger.DebugFormat("Installed UI language: {0}", ci.Name);
	}

	public static void LogExecutableLocation() {
		var location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
		Logger.Debug($"Executable location: {location}");
	}

	public static void LogEverything() {
		LogSystemInfo();
		LogExecutableLocation();
	}
}