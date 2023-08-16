using System.Text.RegularExpressions;

namespace commonItems; 

public static partial class CommonRegexes {
	// catchall:
	//		We grab everything that's NOT =, { or }, OR we grab everything within quotes, except newlines, which we already drop
	//		in the parser.
	public static Regex Catchall => GetCatchallRegex();

	// variables and interpolated expressions
	public static Regex Variable => new($"^@[^{NonStringCharacters}]+$");
	public static Regex InterpolatedExpression => new(@"^@([\s\S]+)|(\[[\s\S]*\])$");

	// numbers
	public static Regex Integer => GetIntegerRegex();
	public static Regex QuotedInteger => GetQuotedIntegerRegex();
	public static Regex Float => GetFloatRegex();
	public static Regex QuotedFloat => GetQuotedFloatRegex();

	// strings
	public static Regex String => new($"^(?!@).+[^{NonStringCharacters}]+$");
	public static Regex QuotedString => new(@"^""[^\n\""]+""$");

	// dates
	public static Regex Date => GetDateRegex();

	// characters that can't be part of an unquoted string
	private const string NonStringCharacters = @"\s=\{\}\[\]\""";

	[GeneratedRegex("^\".+\"|[^={}]+$")]
	private static partial Regex GetCatchallRegex();
	[GeneratedRegex("^-?\\d+$")]
	private static partial Regex GetIntegerRegex();
	[GeneratedRegex("^\"-?\\d+\"$")]
	private static partial Regex GetQuotedIntegerRegex();
	[GeneratedRegex("^-?\\d+(.\\d+)?$")]
	private static partial Regex GetFloatRegex();
	[GeneratedRegex("^\"-?\\d+(.\\d+)?\"$")]
	private static partial Regex GetQuotedFloatRegex();
	[GeneratedRegex("^\\-?\\d+[.]\\d+[.]\\d+$")]
	private static partial Regex GetDateRegex();
}