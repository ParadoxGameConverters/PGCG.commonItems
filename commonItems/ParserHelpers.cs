namespace commonItems;

public static class ParserHelpers {
	public static void IgnoreItem(BufferedReader reader) {
		var next = Parser.GetNextLexeme(reader);
		if (next is "=" or "?=") {
			next = Parser.GetNextLexeme(reader);
		}
		if (next is "rgb" or "hsv") { // Needed for ignoring color. Example: "color = rgb { 2 4 8 }"
			if ((char)reader.Peek() == '{') {
				next = Parser.GetNextLexeme(reader);
			} else { // don't go further in cases like "type = rgb"
				return;
			}
		}
		if (next == "{") {
			var braceDepth = 1;
			while (true) {
				if (reader.EndOfStream) {
					return;
				}
				var lexeme = Parser.GetNextLexeme(reader);
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