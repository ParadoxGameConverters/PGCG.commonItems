using System.Text.RegularExpressions;

namespace commonItems; 

public static partial class CommonRegexes {
	// catchall:
	//		We grab everything that's NOT ?, =, { or }, OR we grab everything within quotes, except newlines, which we already drop
	//		in the parser.
	public static Regex Catchall => GetCatchallRegex();

	// variables and interpolated expressions
	public static Regex Variable => GetVariableRegex();
	public static Regex InterpolatedExpression => GetInterpolatedExpressionRegex();

	// numbers
	public static Regex Integer => GetIntegerRegex();
	public static Regex QuotedInteger => GetQuotedIntegerRegex();
	public static Regex Float => GetFloatRegex();
	public static Regex QuotedFloat => GetQuotedFloatRegex();

	// strings
	// Characters that can't be part of an unquoted string: \s = \{\} \[ \] \"
	public static Regex String => GetStringRegex();
	public static Regex QuotedString => GetQuotedStringRegex();

	// dates
	public static Regex Date => GetDateRegex();


	[GeneratedRegex("^\".+\"|[^?={}]+$")]
	private static partial Regex GetCatchallRegex();
	[GeneratedRegex("^-?\\d+$")]
	private static partial Regex GetIntegerRegex();
	[GeneratedRegex("^\"-?\\d+\"$")]
	private static partial Regex GetQuotedIntegerRegex();
	[GeneratedRegex(@"^-?\d+(.\d+)?$")]
	private static partial Regex GetFloatRegex();
	[GeneratedRegex("^\"-?\\d+(.\\d+)?\"$")]
	private static partial Regex GetQuotedFloatRegex();
	[GeneratedRegex(@"^-?\d+([.]\d+)?([.]\d+)?\.?$")]
	private static partial Regex GetDateRegex();
	
	[GeneratedRegex("^(?!@).*[^\\s=\\{\\}\\[\\]\\\"]+$")]
	private static partial Regex GetStringRegex();
	
	[GeneratedRegex(@"^""[^\n\""]+""$")]
	private static partial Regex GetQuotedStringRegex();
	
	[GeneratedRegex("^@[^\\s=\\{\\}\\[\\]\\\"]+$")]
	private static partial Regex GetVariableRegex();
	
	[GeneratedRegex(@"^@([\s\S]+)|(\[[\s\S]*\])$")]
	private static partial Regex GetInterpolatedExpressionRegex();
}