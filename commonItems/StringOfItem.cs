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
			bool inQuotes = false;
			int braceDepth = 1;
			while (!reader.EndOfStream) {
				char inputChar = (char)reader.Read();
				sb.Append(inputChar);

				if (inputChar == '\"' && (sb.Length < 2 || sb[^2] != '\\')) {
					inQuotes = !inQuotes;
				}
				if (inputChar == '{' && !inQuotes) {
					++braceDepth;
				} else if (inputChar == '}' && !inQuotes) {
					--braceDepth;
					if (braceDepth == 0) {
						str = sb.ToString();
						return;
					}
				}
			}
		}
		str = sb.ToString();
	}

	public bool IsArrayOrObject() {
		var indexOfBracket = str.IndexOf('{');
		return indexOfBracket != -1 && (!str.Contains('"') || str.IndexOf('"') > indexOfBracket);
	}

	public string Serialize(string indent, bool withBraces) => ToString();
	public override string ToString() => str;

	private readonly string str;
}