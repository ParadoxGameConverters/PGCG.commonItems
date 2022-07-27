using Xunit;

namespace commonItems.UnitTests; 

public class ToOrdinalSuffixTests {
	[Fact]
	public void LastDigitOneGivesSt() {
		Assert.Equal("st", 1.ToOrdinalSuffix());
	}
	
	[Fact]
	public void LastDigitTwoGivesNd() {
		Assert.Equal("nd", 2.ToOrdinalSuffix());
	}
	
	[Fact]
	public void LastDigitThreeGivesRd() {
		Assert.Equal("rd", 3.ToOrdinalSuffix());
	}

	[Theory]
	[InlineData(4, "th")]
	[InlineData(5, "th")]
	[InlineData(6, "th")]
	[InlineData(7, "th")]
	[InlineData(8, "th")]
	[InlineData(9, "th")]
	[InlineData(0, "th")]
	public void RemainingDigitsGiveTh(int number, string expectedSuffix) {
		Assert.Equal(expectedSuffix, number.ToOrdinalSuffix());
	}
	
	[Theory]
	[InlineData(10, "th")]
	[InlineData(11, "th")]
	[InlineData(12, "th")]
	[InlineData(13, "th")]
	public void TeensGiveTh(int number, string expectedSuffix) {
		Assert.Equal(expectedSuffix, number.ToOrdinalSuffix());
	}
}