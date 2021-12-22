using System;
using System.Collections.Generic;
using System.IO;

namespace commonItems.Localization {
	public delegate string LocDelegate(string baseLoc, string modifyingLoc);
	public class LocDB {
		private readonly Dictionary<string, LocBlock> locBlocks = new();
		private readonly string baseLanguage;
		private readonly string[] otherLanguages;

		public LocDB(string baseLanguage, params string[] otherLanguages) {
			this.baseLanguage = baseLanguage;
			this.otherLanguages = otherLanguages;
		}

		public void ScrapeLocalizations(string sourceGamePath, IEnumerable<Mod> mods) {
			Logger.Info("Reading Localization");
			var scrapingPath = Path.Combine(sourceGamePath, "game", "localization");
			ScrapeLanguage("english", scrapingPath);
			ScrapeLanguage("french", scrapingPath);
			ScrapeLanguage("german", scrapingPath);
			ScrapeLanguage("russian", scrapingPath);
			ScrapeLanguage("simp_chinese", scrapingPath);
			ScrapeLanguage("spanish", scrapingPath);
			var vanillaLineCount = locBlocks.Count;
			Logger.Info($"{vanillaLineCount} vanilla localization lines read.");

			foreach (var mod in mods) {
				var modLocPath = Path.Combine(mod.Path, "localization");
				if (Directory.Exists(modLocPath)) {
					Logger.Info($"Found some localization in [{mod.Name}]");
					ScrapeLanguage("english", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("french", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("german", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("russian", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("simp_chinese", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("spanish", Path.Combine(mod.Path, "localization"));
					ScrapeLanguage("english", Path.Combine(mod.Path, "localization", "replace"));
					ScrapeLanguage("french", Path.Combine(mod.Path, "localization", "replace"));
					ScrapeLanguage("german", Path.Combine(mod.Path, "localization", "replace"));
					ScrapeLanguage("russian", Path.Combine(mod.Path, "localization", "replace"));
					ScrapeLanguage("simp_chinese", Path.Combine(mod.Path, "localization", "replace"));
					ScrapeLanguage("spanish", Path.Combine(mod.Path, "localization", "replace"));
				}
			}
			Logger.Info($"{locBlocks.Count - vanillaLineCount} mod localization lines read.");
		}
		private void ScrapeLanguage(string language, string path) {
			var languagePath = Path.Combine(path, language);
			if (!Directory.Exists(languagePath)) {
				return;
			}
			var fileNames = SystemUtils.GetAllFilesInFolderRecursive(languagePath);
			foreach (var fileName in fileNames) {
				var filePath = Path.Combine(languagePath, fileName);
				try {
					var stream = File.OpenText(filePath);
					var reader = new BufferedReader(stream);
					ScrapeStream(reader, language);
					stream.Close();
				} catch (Exception e) {
					Logger.Warn($"Could not parse localization file {filePath}: {e}");
				}
			}
		}
		public void ScrapeStream(BufferedReader reader, string language) {
			while (!reader.EndOfStream) {
				var (key, loc) = DetermineKeyLocalizationPair(reader.ReadLine());
				if (key is null || loc is null) {
					continue;
				}

				if (locBlocks.TryGetValue(key, out var locBlock)) {
					locBlock[language] = loc;
				} else {
					var newBlock = new LocBlock(baseLanguage, otherLanguages);
					newBlock[language] = loc;
					locBlocks.Add(key, newBlock);
				}
			}
		}
		private static KeyValuePair<string?, string?> DetermineKeyLocalizationPair(string? line) {
			if (line == null || line.Length < 4 || line[0] == '#' || line[1] == '#') {
				return new KeyValuePair<string?, string?>();
			}
			var sepLoc = line.IndexOf(':');
			if (sepLoc == -1) {
				return new KeyValuePair<string?, string?>();
			}
			var key = line.Substring(1, sepLoc - 1);
			var newLine = line.Substring(sepLoc + 1);
			var quoteLoc = newLine.IndexOf('\"');
			var quote2Loc = newLine.LastIndexOf('\"');
			if (quoteLoc == -1 || quote2Loc == -1 || quote2Loc - quoteLoc == 0) {
				return new KeyValuePair<string?, string?>(key, null);
			}
			var value = newLine.Substring(quoteLoc + 1, quote2Loc - quoteLoc - 1);
			return new KeyValuePair<string?, string?>(key, value);
		}
		public LocBlock? GetLocBlockForKey(string key) {
			if (!locBlocks.TryGetValue(key, out var locBlock)) {
				return null;
			}

			if (!string.IsNullOrEmpty(locBlock.english) &&
				(string.IsNullOrEmpty(locBlock.french) ||
				string.IsNullOrEmpty(locBlock.german) ||
				string.IsNullOrEmpty(locBlock.russian) ||
				string.IsNullOrEmpty(locBlock.simp_chinese) ||
				string.IsNullOrEmpty(locBlock.spanish))
			) {
				locBlock.FillEmptyLocsWithBaseLanguageLoc();
				return locBlock;
			}
			// Either all is well, or we're missing english. Can't do anything about the latter.
			return locBlock;
		}
		public LocBlock AddLocBlock(string key) {
			locBlocks[key] = new(baseLanguage, otherLanguages);
			return locBlocks[key];
		}
	}
}
