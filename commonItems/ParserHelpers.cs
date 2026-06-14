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
		int tokenLength = 0;
		var state = State.Normal;

		while (!reader.EndOfStream) {
			var currentChar = (char)reader.Read();
			switch (state) {
				case State.InQuotes:
					if (currentChar == '"' && previousChar != '\\') {
						state = State.Normal;
						tokenLength = 1;
					}
					previousChar = currentChar;
					break;
				case State.InLiteralQuote:
					if (currentChar == '"' && previousChar == ')') {
						state = State.Normal;
						tokenLength = 1;
					}
					previousChar = currentChar;
					break;
				case State.InInterpolatedExpression:
					if (currentChar == ']') {
						state = State.Normal;
						tokenLength = 1;
					}
					previousChar = currentChar;
					break;
				default:
					if (currentChar == '#') {
						reader.SkipRestOfLine();
						previousChar = '\n';
						tokenLength = 0;
						break;
					}

					if (currentChar == '"') {
						if (tokenLength == 0) {
							state = State.InQuotes;
						} else if (tokenLength == 1 && previousChar == 'R') {
							state = State.InLiteralQuote;
						} else {
							++tokenLength;
						}
						previousChar = currentChar;
						break;
					}

					if (currentChar == '[' && previousChar == '@') {
						state = State.InInterpolatedExpression;
						previousChar = currentChar;
						break;
					}

					if (char.IsWhiteSpace(currentChar)) {
						tokenLength = 0;
						previousChar = currentChar;
						break;
					}

					if (currentChar == '{') {
						++braceDepth;
						tokenLength = 0;
					} else if (currentChar == '}') {
						--braceDepth;
						if (braceDepth == 0) return;
						tokenLength = 0;
					} else if (currentChar is '=' or '?') {
						tokenLength = 0;
					} else {
						++tokenLength;
					}

					previousChar = currentChar;
					break;
			}
		}
	}

	private enum State {
		Normal,
		InQuotes,
		InLiteralQuote,
		InInterpolatedExpression
	}

	public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
		IgnoreItem(sr);
		Logger.Debug($"Ignoring keyword: {keyword}");
	}
}