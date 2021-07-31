using Xunit;

namespace commonItems.UnitTests {
    public class GetPathTests {
        [Fact]
        public void GetPathGetsSlashedPath() {
            var input = @"/this/is/a/path.txt";
            Assert.Equal("/this/is/a/", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsBackslashedPath() {
            var input = @"c:\this\is\a\path.txt";
            Assert.Equal(@"c:\this\is\a\", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsMixedSlashedPath() {
            var input = @"c:\this\is/a/path.txt";
            Assert.Equal(@"c:\this\is/a/", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathGetsReversedMixedSlashedPath() {
            var input = @"/this/is\a\path.txt";
            Assert.Equal(@"/this/is\a\", CommonFunctions.GetPath(input));
        }
        [Fact]
        public void GetPathReturnsBlankStringForRawFiles() {
            var input = "path.txt";
            Assert.Empty(CommonFunctions.GetPath(input));
        }
    }
}
