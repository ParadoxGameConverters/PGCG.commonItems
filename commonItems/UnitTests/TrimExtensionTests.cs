using Xunit;

namespace commonItems.UnitTests {
    public class TrimExtensionTests {
        [Fact]
        public void TrimExtensionTrimsDot() {
            var input = @"file.extension";
            Assert.Equal(@"file", CommonFunctions.TrimExtension(input));
        }
        [Fact]
        public void TrimExtensionTrimsLastDot() {
            var input = @"file.name.with.extension";
            Assert.Equal(@"file.name.with", CommonFunctions.TrimExtension(input));
        }
        [Fact]
        public void TrimExtensionDoesNotAffectDirectories() {
            var input = @"/path/with.extension/filename";
            Assert.Equal(@"/path/with.extension/filename", CommonFunctions.TrimExtension(input));
        }
    }
}
