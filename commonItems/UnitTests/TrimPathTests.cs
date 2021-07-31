using Xunit;

namespace commonItems.UnitTests {
    public class TrimPathTests {
        [Fact]
        public void TrimPathTrimsSlashes() {
            var input = @"/this/is/a/path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsBackslashes() {
            var input = @"c:\this\is\a\path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsMixedSlashes() {
            var input = @"c:\this\is/a/path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathTrimsReversedMixedSlashes() {
            var input = @"/this/is\a\path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
        [Fact]
        public void TrimPathDoesNotAffectRawFiles() {
            var input = @"path.txt";
            Assert.Equal("path.txt", CommonFunctions.TrimPath(input));
        }
    }
}
