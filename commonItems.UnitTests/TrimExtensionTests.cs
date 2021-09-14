using Xunit;

namespace commonItems.UnitTests {
    public class TrimExtensionTests {
        [Fact]
        public void TrimExtensionTrimsDot() {
            const string input = "file.extension";
            Assert.Equal("file", CommonFunctions.TrimExtension(input));
        }
        [Fact]
        public void TrimExtensionTrimsLastDot() {
            const string input = "file.name.with.extension";
            Assert.Equal("file.name.with", CommonFunctions.TrimExtension(input));
        }
        [Fact]
        public void TrimExtensionDoesNotAffectDirectories() {
            const string input = "/path/with.extension/filename";
            Assert.Equal("/path/with.extension/filename", CommonFunctions.TrimExtension(input));
        }
    }
}
