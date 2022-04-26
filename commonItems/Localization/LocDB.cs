using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace commonItems.Localization;

public class LocDB : IReadOnlyDictionary<string, LocBlock> {
	private readonly Dictionary<string, LocBlock> locBlocks = new();
	private readonly string baseLanguage;
	private readonly string[] otherLanguages;

	public LocDB(string baseLanguage, params string[] otherLanguages) {
		this.baseLanguage = baseLanguage;
		this.otherLanguages = otherLanguages;
	}

	public void ScrapeLocalizations(string sourceGamePath, IEnumerable<Mod> mods) {
		Logger.Info("Reading Localization...");

		var scrapingPath = Path.Combine(sourceGamePath, "game", "localization");
		Logger.Info($"{ScrapePath(scrapingPath)} vanilla localization lines read.");

		var modLocLinesRead = 0;
		foreach (var mod in mods) {
			var modLocPath = Path.Combine(mod.Path, "localization");
			if (!Directory.Exists(modLocPath)) {
				continue;
			}

			Logger.Info($"Found some localization in [{mod.Name}].");
			modLocLinesRead += ScrapePath(modLocPath);
		}
		Logger.Info($"{modLocLinesRead} mod localization lines read.");
	}

	private int ScrapePath(string path) {
		if (!Directory.Exists(path)) {
			return 0;
		}

		return SystemUtils.GetAllFilesInFolderRecursive(path)
			.Sum(fileName => ScrapeFile(Path.Combine(path, fileName)));
	}
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

			if (locBlocks.TryGetValue(key, out var locBlock)) {
				locBlock[currentLanguage] = loc;
			} else {
				var newBlock = new LocBlock(baseLanguage) { [currentLanguage] = loc };
				locBlocks.Add(key, newBlock);
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

		if (line.StartsWith("l_")) {
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
		return locBlocks.TryGetValue(key, out var locBlock) ? locBlock : null;
	}
	public LocBlock AddLocBlock(string key) {
		locBlocks[key] = new LocBlock(baseLanguage);
		return locBlocks[key];
	}

	public IEnumerable<string> Keys => locBlocks.Keys;
	public IEnumerable<LocBlock> Values => locBlocks.Values;
	public int Count => locBlocks.Count;
	public LocBlock this[string key] => locBlocks[key];
	public bool ContainsKey(string key) => locBlocks.ContainsKey(key);
	public bool TryGetValue(string key, [MaybeNullWhen(false)] out LocBlock value) => locBlocks.TryGetValue(key, out value);
	public IEnumerator<KeyValuePair<string, LocBlock>> GetEnumerator() => locBlocks.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => locBlocks.GetEnumerator();
}