using Xunit;

namespace commonItems.UnitTests {
    public class TrimExtensionTests {
        [Fact]
        public void TrimExtensionTrimsDot() {
            var input = @"file.extension";
            Assert.Equal(@"file", CommonFunctions.TrimExtenstion(input));
        }
        [Fact]
        public void TrimExtensionTrimsLastDot() {
            var input = @"file.name.with.extension";
            Assert.Equal(@"file.name.with", CommonFunctions.TrimExtenstion(input));
        }
        [Fact]
        public void TrimExtensionDoesNotAffectDirectories() {
            var input = @"/path/with.extension/filename";
            Assert.Equal(@"/path/with.extension/filename", CommonFunctions.TrimExtenstion(input));
        }
    }
}
