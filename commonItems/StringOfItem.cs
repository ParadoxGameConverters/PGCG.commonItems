using commonItems.Serialization;
using System.Text;

namespace commonItems;

public class StringOfItem : IPDXSerializable {
	public StringOfItem(string itemString) {
		str = itemString;
	}
	public StringOfItem(BufferedReader reader) {
		var next = Parser.GetNextLexeme(reader);
		var sb = new StringBuilder();
		if (next == "=") {
			next = Parser.GetNextLexeme(reader);
		}
		sb.Append(next);

		if (next == "{") {
			var braceDepth = 1;
			while (!reader.EndOfStream) {
				char inputChar = (char)reader.Read();
				sb.Append(inputChar);

				if (inputChar == '{') {
					++braceDepth;
				} else if (inputChar == '}') {
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
	
	public string Serialize(string indent, bool withBraces) => ToString();
	public override string ToString() => str;

	private readonly string str;
}