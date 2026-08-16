using Xunit;

namespace commonItems.UnitTests; 

public sealed class NormalizeUTF8PathTests {
	[Fact]
	public void NonAllowedCharactersAreReplaced() {
		const string str = @"/\:*?""<>|";
		const string expected = "_________";
		Assert.Equal(expected.Length, str.Length);
		Assert.Equal(expected, CommonFunctions.NormalizeUTF8Path(str));
	}
	[Fact]
	public void AllowedCharactersAreNotReplaced() {
		const string str = "string123";
		Assert.Equal("string123", CommonFunctions.NormalizeUTF8Path(str));
	}
	[Fact]
	public void TabsAreSkipped() {
		const string str = "\tstring123\t";
		Assert.Equal("string123", CommonFunctions.NormalizeUTF8Path(str));
	}
}