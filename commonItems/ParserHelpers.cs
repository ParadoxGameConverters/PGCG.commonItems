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
					state = HandleInQuotes(currentChar, ref previousChar, ref tokenLength);
					break;
				case State.InLiteralQuote:
					state = HandleInLiteralQuote(currentChar, ref previousChar, ref tokenLength);
					break;
				case State.InInterpolatedExpression:
					state = HandleInInterpolatedExpression(currentChar, ref previousChar, ref tokenLength);
					break;
				default:
					state = HandleDefaultChar(currentChar, reader, ref previousChar, ref tokenLength, ref braceDepth);
					break;
			}
			if (state == State.Exited) return;
		}
	}

	private static State HandleInQuotes(char currentChar, ref char previousChar, ref int tokenLength) {
		if (currentChar == '"' && previousChar != '\\') {
			tokenLength = 1;
			previousChar = currentChar;
			return State.Normal;
		}
		previousChar = currentChar;
		return State.InQuotes;
	}

	private static State HandleInLiteralQuote(char currentChar, ref char previousChar, ref int tokenLength) {
		if (currentChar == '"' && previousChar == ')') {
			tokenLength = 1;
			previousChar = currentChar;
			return State.Normal;
		}
		previousChar = currentChar;
		return State.InLiteralQuote;
	}

	private static State HandleInInterpolatedExpression(char currentChar, ref char previousChar, ref int tokenLength) {
		if (currentChar == ']') {
			tokenLength = 1;
			previousChar = currentChar;
			return State.Normal;
		}
		previousChar = currentChar;
		return State.InInterpolatedExpression;
	}

	private static State HandleDefaultChar(char currentChar, BufferedReader reader, ref char previousChar, ref int tokenLength, ref int braceDepth) {
		if (currentChar == '#') {
			reader.SkipRestOfLine();
			previousChar = '\n';
			tokenLength = 0;
			return State.Normal;
		}

		if (currentChar == '"') {
			if (tokenLength == 0) {
				previousChar = currentChar;
				return State.InQuotes;
			}
			if (tokenLength == 1 && previousChar == 'R') {
				previousChar = currentChar;
				return State.InLiteralQuote;
			}
			++tokenLength;
			previousChar = currentChar;
			return State.Normal;
		}

		if (currentChar == '[' && previousChar == '@') {
			previousChar = currentChar;
			return State.InInterpolatedExpression;
		}

		if (char.IsWhiteSpace(currentChar)) {
			tokenLength = 0;
			previousChar = currentChar;
			return State.Normal;
		}

		if (currentChar == '{') {
			++braceDepth;
			tokenLength = 0;
			previousChar = currentChar;
			return State.Normal;
		}

		if (currentChar == '}') {
			--braceDepth;
			if (braceDepth == 0) return State.Exited;
			tokenLength = 0;
			previousChar = currentChar;
			return State.Normal;
		}

		if (currentChar is '=' or '?') {
			tokenLength = 0;
			previousChar = currentChar;
			return State.Normal;
		}

		++tokenLength;
		previousChar = currentChar;
		return State.Normal;
	}

	private enum State {
		Normal,
		InQuotes,
		InLiteralQuote,
		InInterpolatedExpression,
		Exited
	}

	public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
		IgnoreItem(sr);
		Logger.Debug($"Ignoring keyword: {keyword}");
	}
}