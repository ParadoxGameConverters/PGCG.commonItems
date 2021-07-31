using Xunit;

namespace commonItems.UnitTests {
    public class NormalizeStringPathTests {
        [Fact]
        public void NonAllowedCharactersAreReplaced() {
            var str = @"/\:*?""<>|- ";
            var expected = "___________";
            Assert.Equal(expected.Length, str.Length);
            Assert.Equal(expected, CommonFunctions.NormalizeStringPath(str));
        }
        [Fact]
        public void AllowedCharactersAreNotReplaced() {
            var str = "string123";
            Assert.Equal("string123", CommonFunctions.NormalizeStringPath(str));
        }
        [Fact]
        public void TabsAreSkipped() {
            var str = "\tstring123\t";
            Assert.Equal("string123", CommonFunctions.NormalizeStringPath(str));
        }
    }
}
