using System.Collections.Generic;

namespace commonItems;

public class ModParser : Parser {
	public string Name { get; private set; } = "";
	public string Path { get; set; } = "";

	private bool compressed = false;
	public SortedSet<string> Dependencies { get; } = new();

	public void ParseMod(BufferedReader reader) {
		RegisterKeys();
		ParseStream(reader);
		ClearRegisteredRules();
		CheckIfCompressed();
	}
	public void ParseMod(string fileName) {
		RegisterKeys();
		ParseFile(fileName);
		ClearRegisteredRules();
		CheckIfCompressed();
	}
	private void CheckIfCompressed() {
		if (string.IsNullOrEmpty(Path)) {
			return;
		}

		var ending = CommonFunctions.GetExtension(Path);
		compressed = ending is "zip" or "bin";
	}
	public bool IsValid() {
		return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Path);
	}
	public bool IsCompressed() {
		return compressed;
	}

	private void RegisterKeys() {
		RegisterKeyword("name", reader => Name = reader.GetString());
		RegisterRegex("path|archive", reader => Path = reader.GetString());
		RegisterKeyword("dependencies", reader => Dependencies.UnionWith(reader.GetStrings()));
		RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreItem);
	}
}