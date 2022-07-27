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
	[InlineData(4)]
	[InlineData(5)]
	[InlineData(6)]
	[InlineData(7)]
	[InlineData(8)]
	[InlineData(9)]
	[InlineData(0)]
	public void RemainingDigitsGiveTh(int number) {
		Assert.Equal("th", number.ToOrdinalSuffix());
	}
	
	[Theory]
	[InlineData(10)]
	[InlineData(11)]
	[InlineData(12)]
	[InlineData(13)]
	public void TeensGiveTh(int number) {
		Assert.Equal("th", number.ToOrdinalSuffix());
	}
}