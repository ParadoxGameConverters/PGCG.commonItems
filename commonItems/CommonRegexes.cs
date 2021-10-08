namespace commonItems {
	public static class CommonRegexes {
		// catchall:
		//		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
		//		in the parser.
		public static string Catchall => @""".+""|[^={}]+";

		// numbers
		public static string Integer => @"-?\d+";
		public static string QuotedInteger => @"""-?\d+""";
		public static string Float => @"-?\d+(.\d+)?";
		public static string QuotedFloat => @"""-?\d+(.\d+)?""";

		// strings
		public static string String => @"[^\s^=^\{^\}^\""]+";
		public static string QuotedString => @"""[^\n^=^\{^\}^\""]+""";

		// dates
		public static string Date => @"\d+[.]\d+[.]\d+";
	}
}
