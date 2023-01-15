using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;
using IcgSoftware.IntToOrdinalNumber;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace commonItems;

public static class CommonFunctions {
	public static string[] SplitPath(string path) {
		return path.Split(new[]{Path.AltDirectorySeparatorChar,
			Path.DirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
	}
	
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
		}

		return fileName.Substring(0, fileName.IndexOf(rawFile, StringComparison.Ordinal) + dotPos);
	}

	public static string GetExtension(string fileName) {
		var rawFile = TrimPath(fileName);
		var dotPos = rawFile.LastIndexOf('.');
		if (dotPos == -1) {
			return string.Empty;
		}

		return rawFile.Substring(dotPos + 1);
	}
	public static string ReplaceCharacter(string fileName, char character) {
		return fileName.Replace(character, '_');
	}

	public static string LanguageNameToIetfTag(string languageName) {
		return languageName switch {
			"catalan" => "ca",
			"chinese" => "za",
			"dutch" => "nl",
			"english" => "en",
			"french" => "fr",
			"italian" => "it",
			"japanese" => "ja",
			"portuguese" => "pt",
			"simp_chinese" => "za",
			"spanish" => "es",
			_ => "en"
		};
	}
	
	public static string ToOrdinalSuffix(this int number) {
		return number.ToOrdinalSuffix("english");
	}
	public static string ToOrdinalSuffix(this int number, string languageName) {
		var languageTag = LanguageNameToIetfTag(languageName);
		var cultureInfo = CultureInfo.GetCultureInfo(languageTag);
		
		return number.ToOrdinalNumber(cultureInfo).Replace(number.ToString(CultureInfo.InvariantCulture), "");
	}
	
	[Obsolete($"Use {nameof(ToRomanNumeral)}() extension method instead")]
	public static string CardinalToRoman(int number) {
		return number.ToRomanNumeral();
	}
	public static string ToRomanNumeral(this int number) {
		var numbers = new[] { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
		var symbols = new[] { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
		int i = 12; // (length of symbols array) - 1
		var sb = new StringBuilder();
		while (number > 0) {
			var div = number / numbers[i];
			number %= numbers[i];
			while (div-- > 0) {
				sb.Append(symbols[i]);
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
		var handler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
			? new SteamHandler(new WindowsRegistry())
			: new SteamHandler(null);

		try {
			var game = handler.FindOneGameById(steamId, out string[] errors);

			if (game is not null && game.AppId == steamId) {
				return game.Path;
			}

			foreach (var error in errors) {
				Logger.Debug($"Error occurred when locating Steam game {steamId}: {error}");
			}
		} catch (Exception e) {
			Logger.Warn($"Exception was raised when locating Steam game {steamId}: {e.Message}");
		}

		return null;
	}
}