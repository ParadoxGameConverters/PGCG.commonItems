using Xunit;

namespace commonItems.UnitTests {
    public class NormalizeUTF8PathTests {
        [Fact] public void NonAllowedCharactersAreReplaced() {
            var str = @"/\:*?""<>|";
            var expected = "_________";
            Assert.Equal(expected.Length, str.Length);
            Assert.Equal(expected, CommonFunctions.NormalizeUTF8Path(str));
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
