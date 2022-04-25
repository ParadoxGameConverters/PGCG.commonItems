using Xunit;

namespace commonItems.UnitTests; 

public class CardinalToRomanTests {
	[Fact]
	public void NegativeNumberGivesEmptyString() {
		Assert.Equal(string.Empty, CommonFunctions.CardinalToRoman(-5));
	}
	[Fact]
	public void ZeroGivesEmptyString() {
		Assert.Equal(string.Empty, CommonFunctions.CardinalToRoman(0));
	}
	[Fact]
	public void OneConvertsToI() {
		Assert.Equal("I", CommonFunctions.CardinalToRoman(1));
	}
	[Fact]
	public void FourConvertsToIV() {
		Assert.Equal("IV", CommonFunctions.CardinalToRoman(4));
	}
	[Fact]
	public void FiveConvertsToV() {
		Assert.Equal("V", CommonFunctions.CardinalToRoman(5));
	}
	[Fact]
	public void NineConvertsToIX() {
		Assert.Equal("IX", CommonFunctions.CardinalToRoman(9));
	}
	[Fact]
	public void TenConvertsToX() {
		Assert.Equal("X", CommonFunctions.CardinalToRoman(10));
	}
	[Fact]
	public void FortyConvertsToXL() {
		Assert.Equal("XLI", CommonFunctions.CardinalToRoman(41));
	}
	[Fact]
	public void FiftyConvertsTol() {
		Assert.Equal("LII", CommonFunctions.CardinalToRoman(52));
	}
	[Fact]
	public void NinetyConvertsToXC() {
		Assert.Equal("XCIII", CommonFunctions.CardinalToRoman(93));
	}
	[Fact]
	public void HundredConvertsToC() {
		Assert.Equal("CIV", CommonFunctions.CardinalToRoman(104));
	}
	[Fact]
	public void FourHundredConvertsToCD() {
		Assert.Equal("CDX", CommonFunctions.CardinalToRoman(410));
	}
	[Fact]
	public void FiveHundredConvertsToCD() {
		Assert.Equal("DXI", CommonFunctions.CardinalToRoman(511));
	}
	[Fact]
	public void NineHundredConvertsToCM() {
		Assert.Equal("CML", CommonFunctions.CardinalToRoman(950));
	}
	[Fact]
	public void MultipleThousandsConvertToMultipleMs() {
		Assert.Equal("MMMV", CommonFunctions.CardinalToRoman(3005));
	}
}