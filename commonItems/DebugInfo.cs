using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;

namespace commonItems; 

public static class DebugInfo {
	public static void LogSystemInfo() {
		OperatingSystem os = Environment.OSVersion;
		Logger.DebugFormat("Operating system: {0}", os.VersionString);
		CultureInfo ci = CultureInfo.InstalledUICulture;
		Logger.DebugFormat("Installed UI language: {0}", ci.Name);
	}

	public static void LogExecutableDirectory() {
		Logger.Debug($"Executable directory: {AppDomain.CurrentDomain.BaseDirectory}");
	}

	public static void LogAntivirusInfo() {
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			return;
		}
		
		var searcherPreVista = new ManagementObjectSearcher($@"\\{Environment.MachineName}\root\SecurityCenter", "SELECT * FROM AntivirusProduct");
		var searcherPostVista = new ManagementObjectSearcher($@"\\{Environment.MachineName}\root\SecurityCenter2", "SELECT * FROM AntivirusProduct");
		var preVistaResult = searcherPreVista.Get().OfType<ManagementObject>();
		var postVistaResult = searcherPostVista.Get().OfType<ManagementObject>();

		var instances = preVistaResult.Concat(postVistaResult);

		var installedAntiviruses = instances
			.Select(i => i.Properties.OfType<PropertyData>())
			.Where(pd => pd.Any(p => p.Name == "displayName"))
			.Select(pd => new {
				Name = pd.Single(p => p.Name == "displayName").Value
			})
			.ToArray();

		foreach (var antivirus in installedAntiviruses) {
			Logger.Debug($"Found antivirus: {antivirus.Name}");
		}

	}

	public static void LogEverything() {
		LogSystemInfo();
		LogExecutableDirectory();
		LogAntivirusInfo();
	}
}