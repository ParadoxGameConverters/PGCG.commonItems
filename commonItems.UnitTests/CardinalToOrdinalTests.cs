using Xunit;

namespace commonItems.UnitTests; 

public class CardinalToOrdinalTests {
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
	[Fact]
	public void RemainingDigitsGiveTh() {
		Assert.Equal("th", 4.ToOrdinalSuffix());
		Assert.Equal("th", 5.ToOrdinalSuffix());
		Assert.Equal("th", 6.ToOrdinalSuffix());
		Assert.Equal("th", 7.ToOrdinalSuffix());
		Assert.Equal("th", 8.ToOrdinalSuffix());
		Assert.Equal("th", 9.ToOrdinalSuffix());
		Assert.Equal("th", 0.ToOrdinalSuffix());
	}
	[Fact]
	public void TeensGiveTh() {
		Assert.Equal("th", 10.ToOrdinalSuffix());
		Assert.Equal("th", 11.ToOrdinalSuffix());
		Assert.Equal("th", 12.ToOrdinalSuffix());
		Assert.Equal("th", 13.ToOrdinalSuffix());
	}
}