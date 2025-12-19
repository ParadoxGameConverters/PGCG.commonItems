using Xunit;

namespace commonItems.UnitTests;

public sealed class ToRomanNumeralTests {
	[Fact]
	public void NegativeNumberGivesEmptyString() {
		Assert.Equal(string.Empty, (-5).ToRomanNumeral());
	}
	[Fact]
	public void ZeroGivesEmptyString() {
		Assert.Equal(string.Empty, 0.ToRomanNumeral());
	}

	[Theory]
	[InlineData(1, "I")]
	[InlineData(2, "II")]
	[InlineData(4, "IV")]
	[InlineData(5, "V")]
	[InlineData(7, "VII")]
	[InlineData(9, "IX")]
	[InlineData(10, "X")]
	[InlineData(11, "XI")]
	[InlineData(31, "XXXI")]
	[InlineData(41, "XLI")]
	[InlineData(51, "LI")]
	[InlineData(52, "LII")]
	[InlineData(93, "XCIII")]
	[InlineData(101, "CI")]
	[InlineData(104, "CIV")]
	[InlineData(410, "CDX")]
	[InlineData(501, "DI")]
	[InlineData(511, "DXI")]
	[InlineData(950, "CML")]
	[InlineData(1001, "MI")]
	[InlineData(3005, "MMMV")]
	public void PositiveNumbersAreCorrectlyConverted(int number, string expectedNumeral) {
		Assert.Equal(expectedNumeral, number.ToRomanNumeral());
	}
}