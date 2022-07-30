using Xunit;

namespace commonItems.UnitTests; 

public class ToRomanNumeralTests {
	[Fact]
	public void NegativeNumberGivesEmptyString() {
		Assert.Equal(string.Empty, (-5).ToRomanNumeral());
	}
	[Fact]
	public void ZeroGivesEmptyString() {
		Assert.Equal(string.Empty, 0.ToRomanNumeral());
	}
	[Fact]
	public void OneConvertsToI() {
		Assert.Equal("I", 1.ToRomanNumeral());
	}
	[Fact]
	public void FourConvertsToIV() {
		Assert.Equal("IV", 4.ToRomanNumeral());
	}
	[Fact]
	public void FiveConvertsToV() {
		Assert.Equal("V", 5.ToRomanNumeral());
	}
	[Fact]
	public void NineConvertsToIX() {
		Assert.Equal("IX", 9.ToRomanNumeral());
	}
	[Fact]
	public void TenConvertsToX() {
		Assert.Equal("X", 10.ToRomanNumeral());
	}
	[Fact]
	public void FortyConvertsToXL() {
		Assert.Equal("XLI", 41.ToRomanNumeral());
	}
	[Fact]
	public void FiftyConvertsTol() {
		Assert.Equal("LII", 52.ToRomanNumeral());
	}
	[Fact]
	public void NinetyConvertsToXC() {
		Assert.Equal("XCIII", 93.ToRomanNumeral());
	}
	[Fact]
	public void HundredConvertsToC() {
		Assert.Equal("CIV", 104.ToRomanNumeral());
	}
	[Fact]
	public void FourHundredConvertsToCD() {
		Assert.Equal("CDX", 410.ToRomanNumeral());
	}
	[Fact]
	public void FiveHundredConvertsToCD() {
		Assert.Equal("DXI", 511.ToRomanNumeral());
	}
	[Fact]
	public void NineHundredConvertsToCM() {
		Assert.Equal("CML", 950.ToRomanNumeral());
	}
	[Fact]
	public void MultipleThousandsConvertToMultipleMs() {
		Assert.Equal("MMMV", 3005.ToRomanNumeral());
	}
}