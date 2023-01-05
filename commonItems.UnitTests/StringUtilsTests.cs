using Xunit;

namespace commonItems.UnitTests; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class StringUtilsTests {
	[Theory]
	[InlineData("abc def", false)]
	[InlineData("\"abc def", false)]
	[InlineData("abc def\"", false)]
	[InlineData("\"abc def\"", true)]
	public void IsQuotedReturnsCorrectValue(string str, bool expectedOutput) {
		Assert.Equal(expectedOutput, str.IsQuoted());
	}
	
	[Fact]
	public void RemQuotesRemovesQuotes() {
		const string quotedString = "\"Quoted string\"";
		// ReSharper disable once InvokeAsExtensionMethod
		string unquotedString = StringUtils.RemQuotes(quotedString);
		Assert.Equal("Quoted string", unquotedString);
	}

	[Fact]
	public void RemQuotesRequiresStartingQuotes() {
		const string quotedString = "Quoted string\"";
		string unquotedString = quotedString.RemQuotes();
		Assert.Equal("Quoted string\"", unquotedString);
	}

	[Fact]
	public void RemQuotesRequiresEndingQuotes() {
		const string quotedString = "\"Quoted string";
		string unquotedString = quotedString.RemQuotes();
		Assert.Equal("\"Quoted string", unquotedString);
	}

	[Fact]
	public void RemQuotesLeavesSingleQuote() {
		const string quotedString = "\"";
		string unquotedString = quotedString.RemQuotes();
		Assert.Equal("\"", unquotedString);
	}

	[Fact]
	public void AddQuotesTurnsEmptyStringIntoQuoted() {
		const string unquotedString = "";
		// ReSharper disable once InvokeAsExtensionMethod
		string quotedString = StringUtils.AddQuotes(unquotedString);
		Assert.Equal("\"\"", quotedString);
	}

	[Fact]
	public void AddQuotesLeavesAlreadyQuotedStringAlone() {
		const string alreadyQuotedString = "\"already quoted\"";
		string quotedString = alreadyQuotedString.AddQuotes();
		Assert.Equal("\"already quoted\"", quotedString);
	}

	[Fact]
	public void AddQuotesAddsQuotesToUnquotedString() {
		const string unqQuotedString = "not quoted";
		string quotedString = unqQuotedString.AddQuotes();
		Assert.Equal("\"not quoted\"", quotedString);
	}

	[Fact]
	public void AddQuotesLeavesSingleQuote() {
		const string singleQuoteString = "\"";
		string quotedString = singleQuoteString.AddQuotes();
		Assert.Equal("\"", quotedString);
	}

	[Fact]
	public void AddQuotesLeavesImproperlyQuotedStringAsIs() {
		const string improperlyQuotedString = "\"c";
		string quotedString = improperlyQuotedString.AddQuotes();
		Assert.Equal("\"c", quotedString);
	}

	[Theory]
	// ReSharper disable StringLiteralTypo
	[InlineData("Łódź", "Lodz")] // Polish
	[InlineData("Łękołody", "Lekolody")] // Polish
	[InlineData("Żur", "Zur")] // Polish
	[InlineData("āăąēîïĩíĝġńñšŝśûůŷ", "aaaeiiiiggnnsssuuy")]
	[InlineData("Ý", "Y")]
	// https://en.wikipedia.org/wiki/List_of_Latin-script_letters
	[InlineData("ᴀ", "A")] // Small capital A
	[InlineData("Ɐɐ", "Aa")] // Turned A
	[InlineData("Ĉĉ", "Cc")] // C with circumflex
	[InlineData("C̃c̃", "Cc")] // C with tilde
	[InlineData("C̄c̄", "Cc")] // C with macron
	[InlineData("C̄́c̄́", "Cc")] // C with macron and acute
	[InlineData("C̆c̆", "Cc")] // C with breve
	// ReSharper restore StringLiteralTypo
	public void StringCanBeFoldedToASCII(string strWithAccents, string expectedStr) {
		Assert.Equal(expectedStr, strWithAccents.FoldToASCII());
	}
}