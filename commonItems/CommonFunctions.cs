﻿using System;
using System.Text;
using GameFinder.StoreHandlers.Steam;
using Microsoft.Win32;

namespace commonItems {
	public static class CommonFunctions {
		public static string TrimPath(string fileName) {
			string trimmedFileName = fileName;
			var lastSlash = trimmedFileName.LastIndexOf('\\');
			if (lastSlash != -1) {
				trimmedFileName = trimmedFileName.Substring(lastSlash + 1);
			}
			lastSlash = trimmedFileName.LastIndexOf('/');
			if (lastSlash != -1) {
				trimmedFileName = trimmedFileName.Substring(lastSlash + 1);
			}
			return trimmedFileName;
		}

		public static string GetPath(string fileName) {
			var rawFile = TrimPath(fileName);
			var filePos = fileName.IndexOf(rawFile, StringComparison.Ordinal);
			return fileName.Substring(0, filePos);
		}

		public static string TrimExtension(string fileName) {
			var rawFile = TrimPath(fileName);
			var dotPos = rawFile.LastIndexOf('.');
			if (dotPos == -1) {
				return fileName;
			} else {
				return fileName.Substring(0, fileName.IndexOf(rawFile, StringComparison.Ordinal) + dotPos);
			}
		}

		public static string GetExtension(string fileName) {
			var rawFile = TrimPath(fileName);
			var dotPos = rawFile.LastIndexOf('.');
			if (dotPos == -1) {
				return string.Empty;
			} else {
				return rawFile.Substring(dotPos + 1);
			}
		}
		public static string ReplaceCharacter(string fileName, char character) {
			return fileName.Replace(character, '_');
		}
		public static string CardinalToOrdinal(int cardinal) {
			var hundredRemainder = cardinal % 100;
			var tenRemainder = cardinal % 10;
			if (hundredRemainder - tenRemainder == 10) {
				return "th";
			}

			return tenRemainder switch {
				1 => "st",
				2 => "nd",
				3 => "rd",
				_ => "th",
			};
		}
		public static string CardinalToRoman(int number) {
			var num = new[] { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
			var sym = new[] { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
			int i = 12;
			var sb = new StringBuilder();
			while (number > 0) {
				var div = number / num[i];
				number %= num[i];
				while (div-- > 0) {
					sb.Append(sym[i]);
				}
				--i;
			}
			return sb.ToString();
		}

		public static string NormalizeStringPath(string stringPath) {
			var toReturn = NormalizeUTF8Path(stringPath);
			toReturn = ReplaceCharacter(toReturn, '-');
			toReturn = ReplaceCharacter(toReturn, ' ');
			return toReturn;
		}

		// from C++ commonItems version's OSCommonLayer
		public static string NormalizeUTF8Path(string utf8Path) {
			string asciiPath = EncodingConversions.ConvertUTF8ToASCII(utf8Path);
			asciiPath = asciiPath.Replace('/', '_');
			asciiPath = asciiPath.Replace('\\', '_');
			asciiPath = asciiPath.Replace(':', '_');
			asciiPath = asciiPath.Replace('*', '_');
			asciiPath = asciiPath.Replace('?', '_');
			asciiPath = asciiPath.Replace('\"', '_');
			asciiPath = asciiPath.Replace('<', '_');
			asciiPath = asciiPath.Replace('>', '_');
			asciiPath = asciiPath.Replace('|', '_');
			asciiPath = asciiPath.Replace("\t", string.Empty);

			return asciiPath;
		}

		/// <summary>
		///  Given a Steam AppId, returns the install path for the corresponding game.
		/// </summary>
		/// <returns>Install path for the corresponding game, or null</returns>
		public static string? GetSteamInstallPath(int steamId) {
			// try to find the game without specifying Steam path (may work on Linux)
			var handler = new SteamHandler();

			// if not found, construct handler with Steam path from registry 
			if (!OperatingSystem.IsWindows()) {
				return null;
			}

			const string steamPath32Bit = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam";
			var steamInstallPath = Registry.GetValue(steamPath32Bit, "InstallPath", null);
			if (steamInstallPath is string steam32BitPath) {
				handler = new SteamHandler(steam32BitPath);
				if (handler.TryGetByID(steamId, out var foundGameFrom32Bit)) {
					return foundGameFrom32Bit?.Path;
				}
			}

			const string steamPath64Bit = "HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam";
			steamInstallPath = Registry.GetValue(steamPath64Bit, "InstallPath", null);
			if (steamInstallPath is string steam64BitPath) {
				handler = new SteamHandler(steam64BitPath);
				if (handler.TryGetByID(steamId, out var foundGameFrom64Bit)) {
					return foundGameFrom64Bit?.Path;
				}
			}

			return null;
		}
	}
}
