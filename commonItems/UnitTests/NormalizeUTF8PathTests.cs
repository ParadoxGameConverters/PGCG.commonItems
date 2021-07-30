using Xunit;

namespace commonItems.UnitTests {
    public class NormalizeUTF8PathTests {
        [Fact] public void NonAllowedCharactersAreReplaced() {
            var str = @"/\:*?""<>|";
            Assert.Equal("_________", CommonFunctions.NormalizeUTF8Path(str));
        }
        [Fact]
        public void AllowedCharactersAreNotReplaced() {
            var str = "string123";
            Assert.Equal("string123", CommonFunctions.NormalizeUTF8Path(str));
        }
        [Fact]
        public void TabsAreSkipped() {
            var str = "\tstring123\t";
            Assert.Equal("string123", CommonFunctions.NormalizeUTF8Path(str));
        }
    }
}
