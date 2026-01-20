using commonItems.Collections;
using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;

namespace commonItems.Localization;

public class LocDB : IdObjectCollection<string, LocBlock> {
	private readonly string baseLanguage;
	private readonly string[] otherLanguages;
	private readonly string baseLanguageHeader;
	private readonly string[] otherLanguageHeaders;
	private readonly string baseLanguageFileSuffix;
	private readonly string[] otherLanguageFileSuffixes;

	public LocDB(string baseLanguage, params string[] otherLanguages) {
		this.baseLanguage = baseLanguage;
		this.otherLanguages = otherLanguages;
		baseLanguageHeader = $"l_{baseLanguage}:";
		otherLanguageHeaders = new string[otherLanguages.Length];
		for (var i = 0; i < otherLanguages.Length; ++i) {
			otherLanguageHeaders[i] = $"l_{otherLanguages[i]}:";
		}
		baseLanguageFileSuffix = $"l_{baseLanguage}";
		otherLanguageFileSuffixes = new string[otherLanguages.Length];
		for (var i = 0; i < otherLanguages.Length; ++i) {
			otherLanguageFileSuffixes[i] = $"l_{otherLanguages[i]}";
		}
	}

	public void ScrapeLocalizations(ModFilesystem modFS) {
		Logger.Info("Reading Localization...");

		var locLinesCount = 0;
		foreach (var file in modFS.GetAllFilesInFolderRecursive("localization")) {
			if (!"yml".Equals(CommonFunctions.GetExtension(file.RelativePath), StringComparison.Ordinal)) {
				continue;
			}
			locLinesCount += ScrapeFile(file.AbsolutePath);
		}
		
		Logger.Info($"{locLinesCount} localization lines read.");
	}
	
	/// <summary>
	/// Scrapes file for localization lines.
	/// </summary>
	/// <param name="filePath">Path to the file to be read.</param>
	/// <returns>Count of read loc lines.</returns>
	public int ScrapeFile(string filePath) {
		try {
			using var stream = File.OpenText(filePath);
			var reader = new BufferedReader(stream);
			return ScrapeStream(reader, DetermineLanguageFromFileName(filePath));
		} catch (Exception e) {
			Logger.Warn($"Could not parse localization file {filePath}: {e}");
			return 0;
		}
	}
	public int ScrapeStream(BufferedReader reader, string? currentLanguage = null) {
		var linesRead = 0;
		bool languageSpecified = currentLanguage != null;

		while (!reader.EndOfStream) {
			var line = reader.ReadLine();
			var (key, loc) = DetermineKeyLocalizationPair(line, ref currentLanguage, ref languageSpecified);
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
	private KeyValuePair<string?, string?> DetermineKeyLocalizationPair(
		string? line,
		ref string? currentLanguage,
		ref bool languageSpecified
	) {
		if (line == null || line.Length < 4) {
			return new(null, null);
		}

		ReadOnlySpan<char> span = line.AsSpan();
		var end = span.Length;
		while (end > 0 && char.IsWhiteSpace(span[end - 1])) {
			--end;
		}
		if (end == 0) {
			return new(null, null);
		}
		span = span[..end];

		var start = 0;
		while (start < span.Length && char.IsWhiteSpace(span[start])) {
			++start;
		}
		if (start >= span.Length) {
			return new(null, null);
		}
		if (span[start] == '#') {
			return new(null, null);
		}

		var separatorIndex = span[start..].IndexOf(':');
		if (separatorIndex == -1) {
			return new(null, null);
		}
		separatorIndex += start;

		if (start == 0 && span.StartsWith("l_", StringComparison.Ordinal)) {
			if (span.SequenceEqual(baseLanguageHeader.AsSpan())) {
				currentLanguage = baseLanguage;
			}
			for (var i = 0; i < otherLanguageHeaders.Length; ++i) {
				if (!span.SequenceEqual(otherLanguageHeaders[i].AsSpan())) {
					continue;
				}
				currentLanguage = otherLanguages[i];
			}
			languageSpecified = true;

			return new(null, null);
		}

		if (!languageSpecified) {
			Logger.Warn($"Scraping loc line [{line}] without language specified!");
			return new(null, null);
		}

		// Fail fast when line is malformed, to avoid allocating the key string.
		var valueSpan = span[(separatorIndex + 1)..];
		var quoteIndex = valueSpan.IndexOf('"');
		var quote2Index = valueSpan.LastIndexOf('"');
		if (quoteIndex == -1 || quote2Index == -1 || quote2Index - quoteIndex == 0) {
			return new(null, null);
		}

		var keySpan = span[..separatorIndex];
		var keyStart = 0;
		while (keyStart < keySpan.Length && char.IsWhiteSpace(keySpan[keyStart])) {
			++keyStart;
		}
		if (keyStart >= keySpan.Length) {
			return new(null, null);
		}
		var key = new string(keySpan[keyStart..]);

		var value = new string(valueSpan[(quoteIndex + 1)..(quote2Index)]);
		return new(key, value);
	}
	
	private string? DetermineLanguageFromFileName(string fileName) {
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

		if (fileNameWithoutExtension.EndsWith(baseLanguageFileSuffix, StringComparison.Ordinal)) {
			return baseLanguage;
		}
		for (var i = 0; i < otherLanguageFileSuffixes.Length; ++i) {
			if (fileNameWithoutExtension.EndsWith(otherLanguageFileSuffixes[i], StringComparison.Ordinal)) {
				return otherLanguages[i];
			}
		}
		
		return null;
	}
	
	public LocBlock? GetLocBlockForKey(string key) {
		return dict.TryGetValue(key, out var locBlock) ? locBlock : null;
	}
	public LocBlock AddLocBlock(string key) {
		dict[key] = new LocBlock(key, baseLanguage);
		return dict[key];
	}
	
	public void AddLocForKeyAndLanguage(string key, string language, string loc) {
		if (dict.TryGetValue(key, out var locBlock)) {
			locBlock[language] = loc;
		} else {
			var newBlock = new LocBlock(key, baseLanguage) { [language] = loc };
			dict.Add(key, newBlock);
		}
	}
}