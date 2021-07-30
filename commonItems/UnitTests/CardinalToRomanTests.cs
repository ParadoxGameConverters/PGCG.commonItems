using Xunit;

namespace commonItems.UnitTests {
    public class CardinalToRomanTests {
        [Fact]
        public void LastDigitOneGivesSt() {
            Assert.Equal("st", CommonFunctions.CardinalToOrdinal(1));
        }
        [Fact]
        public void LastDigitTwoGivesNd() {
            Assert.Equal("nd", CommonFunctions.CardinalToOrdinal(2));
        }
        [Fact]
        public void LastDigitThreeGivesRd() {
            Assert.Equal("rd", CommonFunctions.CardinalToOrdinal(3));
        }
        [Fact]
        public void RemainingDigitsGiveTh() {
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(4));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(5));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(6));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(7));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(8));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(9));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(0));
        }
        [Fact]
        public void TeensGiveTh() {
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(10));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(11));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(12));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(13));
        }
    }
}
