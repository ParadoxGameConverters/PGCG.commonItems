using System;
using Xunit;

namespace commonItems.UnitTests {
    public class GetExtensionTests {
        [Fact]
        public void GetExtensionGetsPostDot() {
            var input = @"file.extension";
            Assert.Equal(@"extension", CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionGetsPostLastDot() {
            var input = @"file.name.with.extension";
            Assert.Equal(@"extension", CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionReturnsEmptyStringForNoExtension() {
            var input = @"filename";
            Assert.Empty(CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionDoesNotAffectDirectories() {
            var input = @"/path/with.extension/directoryname";
            Assert.Equal(String.Empty, CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionWorksOnAbsolutePaths() {
            var input = @"c:\path/with.extension/filename.mod";
            Assert.Equal("mod", CommonFunctions.GetExtension(input));
        }
    }
}
