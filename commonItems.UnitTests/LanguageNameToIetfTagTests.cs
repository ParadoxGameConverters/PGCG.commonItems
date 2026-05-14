using Xunit;

namespace commonItems.UnitTests;

public sealed class LanguageNameToIetfTagTests {
	[Theory]
	[InlineData("catalan", "ca")]
	[InlineData("chinese", "za")]
	[InlineData("dutch", "nl")]
	[InlineData("english", "en")]
	[InlineData("french", "fr")]
	[InlineData("italian", "it")]
	[InlineData("japanese", "ja")]
	[InlineData("portuguese", "pt")]
	[InlineData("simp_chinese", "za")]
	[InlineData("spanish", "es")]
	public void LanguageNameMapsToExpectedTag(string languageName, string expectedTag) {
		Assert.Equal(expectedTag, CommonFunctions.LanguageNameToIetfTag(languageName));
	}

	[Fact]
	public void UnknownLanguageDefaultsToEnglishTag() {
		Assert.Equal("en", CommonFunctions.LanguageNameToIetfTag("klingon"));
	}
}