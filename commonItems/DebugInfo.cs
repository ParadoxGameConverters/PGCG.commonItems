using Hardware.Info;
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
	
	public static void LogCPUInfo() {
		HardwareInfo hardwareInfo;
		try {
			hardwareInfo = new();
			hardwareInfo.RefreshCPUList();
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

	/// <summary>
	/// Based on https://stackoverflow.com/a/27168374/10249243.
	/// </summary>
	public static void LogAntivirusInfo() {
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			return;
		}

		try {
			var searcherPreVista = new ManagementObjectSearcher($@"\\{Environment.MachineName}\root\SecurityCenter", "SELECT * FROM AntivirusProduct");
			var searcherPostVista = new ManagementObjectSearcher($@"\\{Environment.MachineName}\root\SecurityCenter2", "SELECT * FROM AntivirusProduct");
			var preVistaResult = searcherPreVista.Get().OfType<ManagementObject>();
			var postVistaResult = searcherPostVista.Get().OfType<ManagementObject>();

			var instances = preVistaResult.Concat(postVistaResult);

#pragma warning disable CA1416
			var installedAntiviruses = instances
				.Select(i => i.Properties.OfType<PropertyData>())
				.Where(pd => pd.Any(p => p.Name == "displayName"))
				.Select(pd => pd.Single(p => p.Name == "displayName").Value.ToString())
				.ToArray();
#pragma warning restore CA1416

			foreach (var antivirusName in installedAntiviruses) {
				Logger.Debug($"Found antivirus: {antivirusName}");
			}
		} catch (Exception e) {
			Logger.Debug($"Exception was raised when locating antiviruses: {e.Message}");
		}
	}

	public static void LogEverything() {
		LogSystemInfo();
		LogCPUInfo();
		LogExecutableDirectory();
		LogAntivirusInfo();
	}
}