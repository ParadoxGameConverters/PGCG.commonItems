namespace commonItems {
	public static class CommonRegexes {
		// catchall:
		//		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
		//		in the parser.
		public static string Catchall => @""".+""|[^={}]+";

		// variables
		public static string Variable => $"@{String}";

		// numbers
		public static string Integer => @"-?\d+";
		public static string QuotedInteger => @"""-?\d+""";
		public static string Float => @"-?\d+(.\d+)?";
		public static string QuotedFloat => @"""-?\d+(.\d+)?""";

		// strings
		public static string String => $"[^{NonStringCharacters}]+";
		public static string QuotedString => @"""[^\n^=^\{^\}^\""]+""";

		// dates
		public static string Date => @"\d+[.]\d+[.]\d+";

		// characters that can't be part of an unquoted string
		private static string NonStringCharacters => @"\s=\{\}\[\]\""";
	}
}
