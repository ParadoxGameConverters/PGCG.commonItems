using Xunit;

namespace commonItems.UnitTests {
    public class GetPathTests {
        [Fact]
        public void GetPathGetsSlashedPath() {
            const string input = "/this/is/a/path.txt";
            Assert.Equal("/this/is/a/", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsBackslashedPath() {
            const string input = @"c:\this\is\a\path.txt";
            Assert.Equal(@"c:\this\is\a\", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsMixedSlashedPath() {
            const string input = @"c:\this\is/a/path.txt";
            Assert.Equal(@"c:\this\is/a/", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsReversedMixedSlashedPath() {
            const string input = @"/this/is\a\path.txt";
            Assert.Equal(@"/this/is\a\", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathReturnsBlankStringForRawFiles() {
            const string input = "path.txt";
            Assert.Empty(CommonFunctions.GetPath(input));
        }
    }
}
