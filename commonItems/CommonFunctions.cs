using GameFinder.Common;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using GameFinder.Wine;
using IcgSoftware.IntToOrdinalNumber;
using NexusMods.Paths;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace commonItems;

public static class CommonFunctions {
	private static readonly char[] PathSeparators = ['/', '\\'];

	public static string[] SplitPath(string path) {
		return path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);
	}
	
	public static string TrimPath(string fileName) {
		ReadOnlySpan<char> span = fileName.AsSpan();
		int lastSlash = Math.Max(span.LastIndexOf('/'), span.LastIndexOf('\\'));
		return lastSlash >= 0 ? span[(lastSlash + 1)..].ToString() : fileName;
	}

	public static string GetPath(string fileName) {
		ReadOnlySpan<char> span = fileName.AsSpan();
		int lastSlash = Math.Max(span.LastIndexOf('/'), span.LastIndexOf('\\'));
		return lastSlash >= 0 ? fileName[..(lastSlash + 1)] : string.Empty;
	}

	public static string TrimExtension(string fileName) {
		ReadOnlySpan<char> span = fileName.AsSpan();
		int lastSlash = Math.Max(span.LastIndexOf('/'), span.LastIndexOf('\\'));
		int dotPos = span.LastIndexOf('.');
		if (dotPos == -1 || dotPos < lastSlash) {
			return fileName;
		}

		return fileName[..dotPos];
	}

	public static string GetExtension(string fileName) {
		ReadOnlySpan<char> span = fileName.AsSpan();
		int lastSlash = Math.Max(span.LastIndexOf('/'), span.LastIndexOf('\\'));
		int dotPos = span.LastIndexOf('.');
		if (dotPos == -1 || dotPos < lastSlash || dotPos == span.Length - 1) {
			return string.Empty;
		}

		return span[(dotPos + 1)..].ToString();
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
		CultureInfo cultureInfo;
		try {
			cultureInfo = CultureInfo.GetCultureInfo(languageTag);
		} catch(CultureNotFoundException e) {
			Logger.Warn($"Failed to get ordinal number suffix for culture {languageTag}, defaulting to English. Details: {e.Message}");
			cultureInfo = CultureInfo.GetCultureInfo(LanguageNameToIetfTag("english"));
		}
		
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

		var normalizedLength = asciiPath.Length;
		foreach (var c in asciiPath) {
			if (c == '\t') {
				normalizedLength--;
			}
		}

		return string.Create(normalizedLength, asciiPath, static (destination, source) => {
			var writeIndex = 0;
			foreach (var c in source) {
				if (c == '\t') {
					continue;
				}

				destination[writeIndex++] = c switch {
					'/' or '\\' or ':' or '*' or '?' or '"' or '<' or '>' or '|' => '_',
					_ => c,
				};
			}
		});
	}

	/// <summary>
	///  Given a Steam AppId, returns the install path for the corresponding game.
	/// </summary>
	/// <returns>Install path for the corresponding game, or null</returns>
	public static string? GetSteamInstallPath(uint steamId) {
		var handler = new SteamHandler(FileSystem.Shared, OperatingSystem.IsWindows() ? WindowsRegistry.Shared : null);

		try {
			var game = handler.FindOneGameById(AppId.From(steamId), out ErrorMessage[] errors);
			if (game is not null) {
				return game.Path.GetFullPath();
			}

			foreach (var error in errors) {
				Logger.Debug($"Error occurred when locating Steam game {steamId}: {error}");
			}
		} catch (Exception e) {
			Logger.Warn($"Exception was raised when locating Steam game {steamId}: {e.Message}");
		}

		return null;
	}
	
	/// <summary>
	///  Given a GOG game ID, returns the install path for the corresponding game.
	///	 Game ID can be found here: https://www.gogdb.org/
	/// </summary>
	/// <returns>Install path for the corresponding game, or null</returns>
	public static string? GetGOGInstallPath(long gogId) {
		try {
			GOGHandler? handler = null;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				handler = new GOGHandler(WindowsRegistry.Shared, FileSystem.Shared);
			} else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
				var prefixManager = new DefaultWinePrefixManager(FileSystem.Shared);

				foreach (var result in prefixManager.FindPrefixes()) {
					result.Switch(prefix => {
						Logger.Debug($"Found wine prefix at {prefix.ConfigurationDirectory}");
						
						var wineFileSystem = prefix.CreateOverlayFileSystem(FileSystem.Shared);
						var wineRegistry = prefix.CreateRegistry(FileSystem.Shared);

						handler = new GOGHandler(wineRegistry, wineFileSystem);
					}, error => {
						Logger.Debug(error.Message);
					});
				}
			}
			if (handler is null) {
				Logger.Debug($"Failed to init GOGHandler on system: {RuntimeInformation.OSDescription}");
				return null;
			}

			var gameId = GOGGameId.From(gogId);
			var game = handler.FindOneGameById(gameId, out ErrorMessage[] errors);

			if (game is not null && game.Id == gameId) {
				return game.Path.GetFullPath();
			}

			foreach (var error in errors) {
				Logger.Debug($"Error occurred when locating GOG game {gogId}: {error}");
			}
		} catch (Exception e) {
			Logger.Warn($"Exception was raised when locating GOG game {gogId}: {e.Message}");
		}

		return null;
	}
}