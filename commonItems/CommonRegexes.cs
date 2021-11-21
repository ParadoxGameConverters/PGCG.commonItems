using System.Text.RegularExpressions;

namespace commonItems {
	public static class CommonRegexes {
		// catchall:
		//		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
		//		in the parser.
		public static Regex Catchall => new(@"^"".+""|[^={}]+$");

		// variables and interpolated expressions
		public static Regex Variable => new($"^@[^{NonStringCharacters}]+$");
		public static Regex InterpolatedExpression => new(@"@([\s\S]+)|(\[[\s\S]*\])$");

		// numbers
		public static Regex Integer => new(@"^-?\d+$");
		public static Regex QuotedInteger => new(@"^""-?\d+""$");
		public static Regex Float => new(@"^-?\d+(.\d+)?$");
		public static Regex QuotedFloat => new(@"^""-?\d+(.\d+)?""$");

		// strings
		public static Regex String => new($"^(?!@).+[^{NonStringCharacters}]+$");
		public static Regex QuotedString => new(@"^""[^\n=\{\}\""]+""$");

		// dates
		public static Regex Date => new(@"^\d+[.]\d+[.]\d+$");

		// characters that can't be part of an unquoted string
		private const string NonStringCharacters = @"\s=\{\}\[\]\""";
	}
}
