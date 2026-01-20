using Hardware.Info;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

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

	/// <summary>
	/// Based on https://stackoverflow.com/a/27168374/10249243.
	/// </summary>
	public static void LogAntivirusInfo() {
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
			return;
		}

		try {
			var installedAntiviruses = GetAntivirusNamesFromRegistry().ToArray();
			foreach (var antivirusName in installedAntiviruses) {
				Logger.Debug($"Found antivirus: {antivirusName}");
			}
		} catch (Exception e) {
			Logger.Debug($"Exception was raised when locating antiviruses: {e.Message}");
		}
	}

	[SupportedOSPlatform("windows")]
	private static HashSet<string> GetAntivirusNamesFromRegistry() {
		var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var view in new[] { RegistryView.Registry32, RegistryView.Registry64 }) {
			TryReadSecurityCenterKey(view, @"SOFTWARE\Microsoft\Security Center\Provider\Av", names);
			TryReadSecurityCenterKey(view, @"SOFTWARE\Microsoft\Security Center2\Provider\Av", names);
		}

		return names;
	}

	[SupportedOSPlatform("windows")]
	private static void TryReadSecurityCenterKey(RegistryView view, string subKeyPath, HashSet<string> names) {
		try {
			using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
			using var avKey = baseKey.OpenSubKey(subKeyPath, writable: false);
			if (avKey == null) {
				return;
			}

			foreach (var subKeyName in avKey.GetSubKeyNames()) {
				using var productKey = avKey.OpenSubKey(subKeyName, writable: false);
				if (productKey == null) {
					continue;
				}

				var displayName = productKey.GetValue("DisplayName") as string;
				if (!string.IsNullOrWhiteSpace(displayName)) {
					names.Add(displayName);
					continue;
				}

				var productName = productKey.GetValue("ProductName") as string;
				if (!string.IsNullOrWhiteSpace(productName)) {
					names.Add(productName);
				}
			}
		} catch {
			// Best-effort registry read; ignore failures.
		}
	}

	public static void LogEverything() {
		LogSystemInfo();
		LogCPUInfo();
		LogExecutableDirectory();
		LogAntivirusInfo();
	}
}