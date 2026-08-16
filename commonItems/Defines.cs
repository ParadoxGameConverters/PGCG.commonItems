using commonItems.Mods;
using System.Collections.Generic;

namespace commonItems;

/// <summary>
/// Class for reading and storing defines from common/defines/ files in Jomini games.
/// </summary>
public class Defines {
	public void LoadDefines(ModFilesystem modFS) {
		var categoriesParser = new Parser();
		categoriesParser.RegisterRegex(CommonRegexes.String, (categoryReader, categoryName) => {
			if (!defines.TryGetValue(categoryName, out var category)) {
				category = new();
				defines[categoryName] = category;
			}
			
			var definesParser = new Parser();
			definesParser.RegisterKeyword(";", reader => { }); // Ignore the semicolons.
			definesParser.RegisterRegex(CommonRegexes.String, (reader, key) => {
				var stfOfItem = reader.GetStringOfItem();
				category[key] = stfOfItem.ToString();
			});
			definesParser.IgnoreAndLogUnregisteredItems();
			definesParser.ParseStream(categoryReader);
		});
		categoriesParser.IgnoreAndLogUnregisteredItems();
		categoriesParser.ParseGameFolder("common/defines", modFS, "txt", recursive: true);
	}

	public string? GetValue(string category, string key) {
		if (!defines.TryGetValue(category, out var categoryDict)) {
			return null;
		}

		return categoryDict.GetValueOrDefault(key);
	}
	
	private readonly Dictionary<string, Dictionary<string, string>> defines = new();
}