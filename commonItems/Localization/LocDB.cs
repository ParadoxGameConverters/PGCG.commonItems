using commonItems.Collections;
using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace commonItems.Localization;

public class LocDB : IdObjectCollection<string, LocBlock> {
	private readonly string baseLanguage;
	private readonly string[] otherLanguages;

	public LocDB(string baseLanguage, params string[] otherLanguages) {
		this.baseLanguage = baseLanguage;
		this.otherLanguages = otherLanguages;
	}

	public void ScrapeLocalizations(ModFilesystem modFS) {
		Logger.Info("Reading Localization...");

		var locFiles = modFS.GetAllFilesInFolderRecursive("localization");
		var locLinesCount = locFiles.Sum(ScrapeFile);
		
		Logger.Info($"{locLinesCount} localization lines read.");
	}
	
	/// <summary>
	/// Scrapes file for localization lines, returns count of loc lines read.
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns></returns>
	private int ScrapeFile(string filePath) {
		try {
			using var stream = File.OpenText(filePath);
			var reader = new BufferedReader(stream);
			return ScrapeStream(reader);
		} catch (Exception e) {
			Logger.Warn($"Could not parse localization file {filePath}: {e}");
			return 0;
		}
	}
	public int ScrapeStream(BufferedReader reader) {
		var linesRead = 0;
		string? currentLanguage = null;

		while (!reader.EndOfStream) {
			var (key, loc) = DetermineKeyLocalizationPair(reader.ReadLine(), ref currentLanguage);
			if (currentLanguage is null) {
				continue;
			}
			if (key is null || loc is null) {
				continue;
			}

			if (dict.TryGetValue(key, out var locBlock)) {
				locBlock[currentLanguage] = loc;
			} else {
				var newBlock = new LocBlock(key, baseLanguage) { [currentLanguage] = loc };
				dict.Add(key, newBlock);
			}
			++linesRead;
		}

		return linesRead;
	}
	private KeyValuePair<string?, string?> DetermineKeyLocalizationPair(string? line, ref string? currentLanguage) {
		if (line == null || line.Length < 4 || line.TrimStart().StartsWith('#')) {
			return new(null, null);
		}

		line = line.TrimEnd();

		var sepLoc = line.IndexOf(':');
		if (sepLoc == -1) {
			return new(null, null);
		}

		if (line.StartsWith("l_", StringComparison.Ordinal)) {
			if (line == $"l_{baseLanguage}:") {
				currentLanguage = baseLanguage;
			}
			foreach (var language in otherLanguages) {
				if (line != $"l_{language}:") {
					continue;
				}
				currentLanguage = language;
			}

			return new(null, null);
		}

		if (currentLanguage is null) {
			Logger.Warn($"Scraping loc line [{line}] without language specified!");
			return new(null, null);
		}

		var key = line.Substring(1, sepLoc - 1);
		var newLine = line.Substring(sepLoc + 1);
		var quoteIndex = newLine.IndexOf('\"');
		var quote2Index = newLine.LastIndexOf('\"');
		if (quoteIndex == -1 || quote2Index == -1 || quote2Index - quoteIndex == 0) {
			return new(key, null);
		}

		var value = newLine.Substring(quoteIndex + 1, quote2Index - quoteIndex - 1);
		return new(key, value);
	}
	public LocBlock? GetLocBlockForKey(string key) {
		return dict.TryGetValue(key, out var locBlock) ? locBlock : null;
	}
	public LocBlock AddLocBlock(string key) {
		dict[key] = new LocBlock(key, baseLanguage);
		return dict[key];
	}
}