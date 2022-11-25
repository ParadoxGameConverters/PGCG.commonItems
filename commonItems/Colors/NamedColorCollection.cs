using commonItems.Mods;
using System;
using System.Collections.Generic;

namespace commonItems.Colors; 

public class NamedColorCollection : SortedDictionary<string, Color> {
	public void LoadNamedColors(string relativePath, ModFilesystem modFilesystem) {
		var colorsParser = new Parser();
		colorsParser.RegisterRegex(CommonRegexes.String, (reader, colorName) => {
			try {
				this[colorName] = new ColorFactory().GetColor(reader);
			} catch (FormatException e) {
				Logger.Warn($"Failed to read color {colorName}: {e.Message}");
			}
		});
		colorsParser.IgnoreAndLogUnregisteredItems();
		
		var parser = new Parser();
		parser.RegisterKeyword("colors", reader => {
			colorsParser.ParseStream(reader);
		});
		parser.IgnoreAndLogUnregisteredItems();
		
		parser.ParseGameFolder(relativePath, modFilesystem, "txt", recursive: true);
	}
}