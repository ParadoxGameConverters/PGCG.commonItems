using Xunit;

namespace commonItems.UnitTests {
    public class TrimPathTests {
        [Fact]
        public void TrimPathTrimsSlashes() {
            const string input = "/this/is/a/path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsBackslashes() {
            const string input = @"c:\this\is\a\path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsMixedSlashes() {
            const string input = @"c:\this\is/a/path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsReversedMixedSlashes() {
            const string input = @"/this/is\a\path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathDoesNotAffectRawFiles() {
            const string input = "path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
    }
}
