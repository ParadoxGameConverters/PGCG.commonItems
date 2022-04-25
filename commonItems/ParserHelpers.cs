using commonItems.Serialization;
using System.Collections.Generic;
using System.Text;

namespace commonItems;

public static class ParserHelpers {
	public static void IgnoreItem(BufferedReader sr) {
		var next = Parser.GetNextLexeme(sr);
		if (next == "=") {
			next = Parser.GetNextLexeme(sr);
		}
		if (next is "rgb" or "hsv") // Needed for ignoring color. Example: "color = rgb { 2 4 8 }"
		{
			if ((char)sr.Peek() == '{') {
				next = Parser.GetNextLexeme(sr);
			} else { // don't go further in cases like "type = rgb"
				return;
			}
		}
		if (next == "{") {
			var braceDepth = 1;
			while (true) {
				if (sr.EndOfStream) {
					return;
				}
				var lexeme = Parser.GetNextLexeme(sr);
				if (lexeme == "{") {
					++braceDepth;
				} else if (lexeme == "}") {
					--braceDepth;
					if (braceDepth == 0) {
						return;
					}
				}
			}
		}
	}

	public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
		IgnoreItem(sr);
		Logger.Debug($"Ignoring keyword: {keyword}");
	}
}

public class StringOfItem : IPDXSerializable {
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

public class BlobList : Parser {
	public List<string> Blobs { get; } = new();
	public BlobList(BufferedReader reader) {
		var next = GetNextLexeme(reader);
		if (next == "=") {
			next = GetNextLexeme(reader);
		}
		if (next != "{") {
			return;
		}

		var braceDepth = 0;
		var sb = new StringBuilder();
		while (!reader.EndOfStream) {
			char inputChar = (char)reader.Read();
			if (inputChar == '{') {
				if (braceDepth > 0) {
					sb.Append(inputChar);
				}
				++braceDepth;
			} else if (inputChar == '}') {
				--braceDepth;
				if (braceDepth > 0) {
					sb.Append(inputChar);
				} else if (braceDepth == 0) {
					Blobs.Add(sb.ToString());
					sb.Clear();
				} else if (braceDepth == -1) {
					return;
				}
			} else if (braceDepth == 0) {
				// Ignore this character. Only look for blobs.
			} else {
				sb.Append(inputChar);
			}
		}
	}
}