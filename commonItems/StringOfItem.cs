using commonItems.Serialization;
using System.Text;

namespace commonItems;

public sealed class StringOfItem : IPDXSerializable {
	public StringOfItem(string itemString) {
		str = itemString;
	}
	public StringOfItem(BufferedReader reader) {
		var next = Parser.GetNextLexeme(reader);
		var sb = new StringBuilder();
		if (next is "=" or "?=") {
			next = Parser.GetNextLexeme(reader);
		}
		sb.Append(next);

		if (next == "{") {
			AppendBracedContent(reader, sb);
		}
		str = sb.ToString();
	}

	private static void AppendBracedContent(BufferedReader reader, StringBuilder sb) {
		bool inQuotes = false;
		int braceDepth = 1;
		int backslashRun = 0; // consecutive backslashes immediately before the current character
		while (!reader.EndOfStream) {
			char inputChar = (char)reader.Read();
			sb.Append(inputChar);

			if (inputChar == '"') {
				if (backslashRun % 2 == 0) { // even number of backslashes means quote is not escaped
					inQuotes = !inQuotes;
				}
				backslashRun = 0;
			} else if (inputChar == '\\') {
				++backslashRun;
			} else {
				backslashRun = 0;
			}
			if (inputChar == '{' && !inQuotes) {
				++braceDepth;
			} else if (inputChar == '}' && !inQuotes) {
				--braceDepth;
				if (braceDepth == 0) {
					return;
				}
			}
		}
	}

	public bool IsArrayOrObject() {
		var indexOfBracket = str.IndexOf('{');
		return indexOfBracket != -1 && (!str.Contains('"') || str.IndexOf('"') > indexOfBracket);
	}

	public string Serialize(string indent, bool withBraces) => ToString();
	public override string ToString() => str;

	private readonly string str;
}