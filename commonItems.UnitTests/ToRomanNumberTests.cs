using Xunit;

namespace commonItems.UnitTests; 

public class ToRomanNumberTests {
	[Fact]
	public void NegativeNumberGivesEmptyString() {
		Assert.Equal(string.Empty, (-5).ToRomanNumber());
	}
	[Fact]
	public void ZeroGivesEmptyString() {
		Assert.Equal(string.Empty, 0.ToRomanNumber());
	}
	[Fact]
	public void OneConvertsToI() {
		Assert.Equal("I", 1.ToRomanNumber());
	}
	[Fact]
	public void FourConvertsToIV() {
		Assert.Equal("IV", 4.ToRomanNumber());
	}
	[Fact]
	public void FiveConvertsToV() {
		Assert.Equal("V", 5.ToRomanNumber());
	}
	[Fact]
	public void NineConvertsToIX() {
		Assert.Equal("IX", 9.ToRomanNumber());
	}
	[Fact]
	public void TenConvertsToX() {
		Assert.Equal("X", 10.ToRomanNumber());
	}
	[Fact]
	public void FortyConvertsToXL() {
		Assert.Equal("XLI", 41.ToRomanNumber());
	}
	[Fact]
	public void FiftyConvertsTol() {
		Assert.Equal("LII", 52.ToRomanNumber());
	}
	[Fact]
	public void NinetyConvertsToXC() {
		Assert.Equal("XCIII", 93.ToRomanNumber());
	}
	[Fact]
	public void HundredConvertsToC() {
		Assert.Equal("CIV", 104.ToRomanNumber());
	}
	[Fact]
	public void FourHundredConvertsToCD() {
		Assert.Equal("CDX", 410.ToRomanNumber());
	}
	[Fact]
	public void FiveHundredConvertsToCD() {
		Assert.Equal("DXI", 511.ToRomanNumber());
	}
	[Fact]
	public void NineHundredConvertsToCM() {
		Assert.Equal("CML", 950.ToRomanNumber());
	}
	[Fact]
	public void MultipleThousandsConvertToMultipleMs() {
		Assert.Equal("MMMV", 3005.ToRomanNumber());
	}
}