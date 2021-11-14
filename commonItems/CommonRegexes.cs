using System.Text.RegularExpressions;

namespace commonItems {
	public static class CommonRegexes {
		// catchall:
		//		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
		//		in the parser.
		public static Regex Catchall => new(@"^"".+""|[^={}]+$", RegexOptions.Compiled);

		// variables and interpolated expressions
		public static Regex Variable => new($"^@[^{NonStringCharacters}]+$", RegexOptions.Compiled);
		public static Regex InterpolatedExpression => new($@"@([^{NonStringCharacters}]+)|(\[[^{NonStringCharacters}]+\])$", RegexOptions.Compiled);

		// numbers
		public static Regex Integer => new(@"^-?\d+$", RegexOptions.Compiled);
		public static Regex QuotedInteger => new(@"^""-?\d+""$", RegexOptions.Compiled);
		public static Regex Float => new(@"^-?\d+(.\d+)?$", RegexOptions.Compiled);
		public static Regex QuotedFloat => new(@"^""-?\d+(.\d+)?""$", RegexOptions.Compiled);

		// strings
		public static Regex String => new($"^(?!@).+[^{NonStringCharacters}]+$", RegexOptions.Compiled);
		public static Regex QuotedString => new(@"^""[^\n=\{\}\""]+""$", RegexOptions.Compiled);

		// dates
		public static Regex Date => new(@"^\d+[.]\d+[.]\d+$", RegexOptions.Compiled);

		// characters that can't be part of an unquoted string
		private const string NonStringCharacters = @"\s=\{\}\[\]\""";
	}
}
