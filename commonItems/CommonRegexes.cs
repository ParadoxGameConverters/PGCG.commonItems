namespace commonItems {
    public static class CommonRegexes {
        // catchall:
        //		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
        //		in the parser.
        public static string Catchall { get; } = @""".+""|[^={}]+";

        // numbers
        public static string Integer { get; } = @"-?\d+";
        public static string QuotedInteger { get; } = @"""-?\d+""";
        public static string Float { get; } = @"-?\d+(.\d+)?";
        public static string QuotedFloat { get; } = @"""-?\d+(.\d+)?""";

        // strings
        public static string String { get; } = @"[^\s^=^\{^\}^\""]+";
        public static string QuotedString { get; } = @"""[^\n^=^\{^\}^\""]+""";

        // dates
        public static string Date { get; } = @"\d+[.]\d+[.]\d+";
    }
}
