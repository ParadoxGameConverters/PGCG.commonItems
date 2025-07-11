using Xunit;

namespace commonItems.UnitTests;

public sealed class CommonRegexesTests {
	private sealed class TestParser : Parser;

	[Fact]
	public void VariableRegexMatchesVariables() {
		var reader = new BufferedReader("@ai_aggressiveness = 70");
		var instance = new TestParser();
		instance.ParseStream(reader);
		Assert.Collection(reader.Variables,
			pair => {
				var (key, value) = pair;
				Assert.Equal("ai_aggressiveness", key);
				Assert.Equal(70, value);
			}
		);
	}
	[Fact]
	public void VariableRegexDoesNotMatchInterpolatedExpressions() {
		var reader = new BufferedReader("@[100-ai_aggressiveness] = 70");
		var instance = new TestParser();
		instance.ParseStream(reader);
		Assert.Empty(reader.Variables);
	}

	[Fact]
	public void InterpolatedExpressionRegexMatchesInterpolatedExpressions() {
		Assert.Matches(CommonRegexes.InterpolatedExpression, "@[100-ai_aggressiveness]");
	}
	[Fact]
	public void InterpolatedExpressionRegexMatchesVariables() {
		Assert.Matches(CommonRegexes.InterpolatedExpression, "@ai_aggressiveness");
	}

	[Fact]
	public void CatchallRegexMatchesStrings() {
		Assert.Matches(CommonRegexes.Catchall, "1234-abcd");
	}

	[Fact]
	public void CatchallRegexMatchesQuotedStrings() {
		Assert.Matches(CommonRegexes.Catchall, "\"1234-abcd\"");
	}

	[Fact]
	public void CatchallRegexDoesntMatchCurlyBrackets() {
		Assert.DoesNotMatch(CommonRegexes.Catchall, "1234-abcd{");
		Assert.DoesNotMatch(CommonRegexes.Catchall, "1234-abcd}");
	}
	
	[Fact]
	public void CatchallRegexMatchesQuotedCurlyBrackets() {
		Assert.Matches(CommonRegexes.Catchall, "\"1234-abcd{\"");
		Assert.Matches(CommonRegexes.Catchall, "\"1234-abcd}\"");
	}

	[Fact]
	public void CatchallRegexMatchesInternalQuotes() {
		Assert.Matches(CommonRegexes.Catchall, @"1234-""abcd");
	}

	[Fact]
	public void CatchallRegexMatchesQuotedInternalQuotes() {
		Assert.Matches(CommonRegexes.Catchall, @"""1234-""abcd""");
	}

	[Fact]
	public void CatchallRegexDoesntMatchQuestionSign() {
		Assert.DoesNotMatch(CommonRegexes.Catchall, "1234-abcd?");
	}

	[Fact]
	public void CatchallRegexDoesntMatchEquals() {
		Assert.DoesNotMatch(CommonRegexes.Catchall, "1234-abcd=");
	}

	[Fact]
	public void CatchallRegexMatchesQuotedEquals() {
		Assert.Matches(CommonRegexes.Catchall, @"""1234-abcd=""");
	}

	[Fact]
	public void IntegerRegexMatchesIntegers() {
		Assert.Matches(CommonRegexes.Integer, "123456");
	}

	[Fact]
	public void IntegerRegexMatchesNegativeIntegers() {
		Assert.Matches(CommonRegexes.Integer, "-123456");
	}

	[Fact]
	public void IntegerRegexDoesntMatchQuotedIntegers() {
		Assert.DoesNotMatch(CommonRegexes.Integer, "\"123456\"");
	}

	[Fact]
	public void IntegerRegexDoesntMatchFloats() {
		Assert.DoesNotMatch(CommonRegexes.Integer, "123.456");
	}

	[Fact]
	public void IntegerRegexDoesntMatchStrings() {
		Assert.DoesNotMatch(CommonRegexes.Integer, "a123456");
		Assert.DoesNotMatch(CommonRegexes.Integer, "123456a");
	}

	[Fact]
	public void QuotedIntegerRegexMatchesIntegers() {
		Assert.Matches(CommonRegexes.QuotedInteger, "\"123456\"");
	}

	[Fact]
	public void QuotedIntegerRegexMatchesNegativeIntegers() {
		Assert.Matches(CommonRegexes.QuotedInteger, "\"-123456\"");
	}

	[Fact]
	public void QuotedIntegerRegexDoesntMatchUnquotedIntegers() {
		Assert.DoesNotMatch(CommonRegexes.QuotedInteger, "123456");
	}

	[Fact]
	public void QuotedIntegerRegexesDoesntMatchFloats() {
		Assert.DoesNotMatch(CommonRegexes.QuotedInteger, "\"123.456\"");
	}

	[Fact]
	public void QuotedIntegerRegexDoesntMatchStrings() {
		Assert.DoesNotMatch(CommonRegexes.QuotedInteger, "\"a123456\"");
		Assert.DoesNotMatch(CommonRegexes.QuotedInteger, "\"123456a\"");
	}

	[Fact]
	public void FloatRegexMatchesFloats() {
		Assert.Matches(CommonRegexes.Float, "123.456");
	}

	[Fact]
	public void FloatRegexMatchesNegativeFloats() {
		Assert.Matches(CommonRegexes.Float, "-123.456");
	}

	[Fact]
	public void FloatRegexMatchesIntegers() {
		Assert.Matches(CommonRegexes.Float, "123456");
	}

	[Fact]
	public void FloatRegexDoesntMatchQuotedFloats() {
		Assert.DoesNotMatch(CommonRegexes.Float, "\"123.456\"");
	}

	[Fact]
	public void FloatRegexDoesntMatchStrings() {
		Assert.DoesNotMatch(CommonRegexes.Float, "a12345");
		Assert.DoesNotMatch(CommonRegexes.Float, "123456a");
	}

	[Fact]
	public void StringRegexMatchesStrings() {
		Assert.Matches(CommonRegexes.String, "1234-abcd");
	}

	[Fact]
	public void StringRegexDoesntMatchQuotedStrings() {
		Assert.DoesNotMatch(CommonRegexes.String, "\"1234-abcd\"");
	}

	[Fact]
	public void StringRegexDoesntMatchCurlyBrackets() {
		Assert.DoesNotMatch(CommonRegexes.String, "1234-abcd{");
		Assert.DoesNotMatch(CommonRegexes.String, "1234-abcd}");
	}

	[Fact]
	public void StringRegexDoesntMatchBrackets() {
		Assert.DoesNotMatch(CommonRegexes.String, "1234-abcd[");
		Assert.DoesNotMatch(CommonRegexes.String, "1234-abcd]");
	}

	[Fact]
	public void StringRegexDoesntMatchInternalQuotes() {
		Assert.DoesNotMatch(CommonRegexes.String, @"1234-abcd""");
	}

	[Fact]
	public void StringRegexDoesntMatchEquals() {
		Assert.DoesNotMatch(CommonRegexes.String, "1234-abcd=");
	}

	[Fact]
	public void StringRegexMatchesSingleCharacters() {
		Assert.Matches(CommonRegexes.String, "a");
		Assert.Matches(CommonRegexes.String, "1");
	}

	[Fact]
	public void QuotedStringRegexMatchesQuotedStrings() {
		Assert.Matches(CommonRegexes.QuotedString, @"""1234-abcd""");
	}

	[Fact]
	public void QuotedStringRegexMatchesCurlyBrackets() {
		Assert.Matches(CommonRegexes.QuotedString, @"""1234-abcd{""");
		Assert.DoesNotMatch(CommonRegexes.QuotedString, "1234-abcd}");
	}

	[Fact]
	public void QuotedStringRegexMatchesBrackets() {
		Assert.Matches(CommonRegexes.QuotedString, @"""1234-abcd[""");
		Assert.DoesNotMatch(CommonRegexes.QuotedString, "1234-abcd]");
	}

	[Fact]
	public void QuotedStringRegexDoesntMatchInternalQuotes() {
		Assert.DoesNotMatch(CommonRegexes.QuotedString, @"1234-abcd""");
	}

	[Fact]
	public void QuotedStringRegexMatchesQuotedEquals() {
		Assert.Matches(CommonRegexes.QuotedString, @"""1234-abcd=""");
	}
	
	[Fact]
	public void DateRegexMatchesDates() {
		Assert.Matches(CommonRegexes.Date, "1918.11.11");
		Assert.Matches(CommonRegexes.Date, "-1918.11.11");
	}

	[Fact]
	public void DateRegexMatchesIncompleteDates() {
		Assert.Matches(CommonRegexes.Date, "1918.11.");
		Assert.Matches(CommonRegexes.Date, "1918.11");
		Assert.Matches(CommonRegexes.Date, "1918.");
		Assert.Matches(CommonRegexes.Date, "1918");

		Assert.Matches(CommonRegexes.Date, "-1918.11.");
		Assert.Matches(CommonRegexes.Date, "-1918.11");
		Assert.Matches(CommonRegexes.Date, "-1918.");
		Assert.Matches(CommonRegexes.Date, "-1918");
	}
	
	[Fact]
	public void DateRegexDoesNotMatchDatesWithCharacters() {
		Assert.DoesNotMatch(CommonRegexes.Date, "1918a.11.11");
		Assert.DoesNotMatch(CommonRegexes.Date, "1918.11a.11");
		Assert.DoesNotMatch(CommonRegexes.Date, "1918.11.11a");
	}
}