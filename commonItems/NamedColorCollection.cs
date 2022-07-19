using commonItems.Mods;
using System.Collections.Generic;

namespace commonItems; 

public class NamedColorCollection : SortedDictionary<string, Color> {
	public void LoadNamedColors(string relativePath, ModFilesystem modFilesystem) {
		var colorsParser = new Parser();
		colorsParser.RegisterRegex(CommonRegexes.String, (reader, colorName) => {
			this[colorName] = new ColorFactory().GetColor(reader);
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