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