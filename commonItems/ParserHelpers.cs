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
			SkipBracedItem(reader);
		}
	}

	private static void SkipBracedItem(BufferedReader reader) {
		int braceDepth = 1;
		char previousChar = '\0';
		bool inQuotes = false;
		bool inLiteralQuote = false;
		bool inInterpolatedExpression = false;
		int tokenLength = 0;

		while (!reader.EndOfStream) {
			var currentChar = (char)reader.Read();

			if (inQuotes) {
				if (currentChar == '"' && previousChar != '\\') {
					inQuotes = false;
					tokenLength = 1;
				}
				previousChar = currentChar;
				continue;
			}

			if (inLiteralQuote) {
				if (currentChar == '"' && previousChar == ')') {
					inLiteralQuote = false;
					tokenLength = 1;
				}
				previousChar = currentChar;
				continue;
			}

			if (inInterpolatedExpression) {
				if (currentChar == ']') {
					inInterpolatedExpression = false;
					tokenLength = 1;
				}
				previousChar = currentChar;
				continue;
			}

			if (currentChar == '#') {
				reader.SkipRestOfLine();
				previousChar = '\n';
				tokenLength = 0;
				continue;
			}

			if (currentChar == '"') {
				if (tokenLength == 0) {
					inQuotes = true;
				} else if (tokenLength == 1 && previousChar == 'R') {
					inLiteralQuote = true;
				} else {
					++tokenLength;
				}
				previousChar = currentChar;
				continue;
			}

			if (currentChar == '[' && previousChar == '@') {
				inInterpolatedExpression = true;
				previousChar = currentChar;
				continue;
			}

			if (char.IsWhiteSpace(currentChar)) {
				tokenLength = 0;
				previousChar = currentChar;
				continue;
			}

			if (currentChar == '{') {
				++braceDepth;
				tokenLength = 0;
			} else if (currentChar == '}') {
				--braceDepth;
				if (braceDepth == 0) {
					return;
				}
				tokenLength = 0;
			} else if (currentChar is '=' or '?') {
				tokenLength = 0;
			} else {
				++tokenLength;
			}

			previousChar = currentChar;
		}
	}

	public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
		IgnoreItem(sr);
		Logger.Debug($"Ignoring keyword: {keyword}");
	}
}