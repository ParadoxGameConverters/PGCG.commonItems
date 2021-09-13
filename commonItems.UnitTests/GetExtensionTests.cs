using System;
using Xunit;

namespace commonItems.UnitTests {
    public class GetExtensionTests {
        [Fact]
        public void GetExtensionGetsPostDot() {
            const string? input = "file.extension";
            Assert.Equal("extension", CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionGetsPostLastDot() {
            const string? input = "file.name.with.extension";
            Assert.Equal("extension", CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionReturnsEmptyStringForNoExtension() {
            const string? input = "filename";
            Assert.Empty(CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionDoesNotAffectDirectories() {
            const string? input = "/path/with.extension/directoryname";
            Assert.Equal(string.Empty, CommonFunctions.GetExtension(input));
        }
        [Fact]
        public void GetExtensionWorksOnAbsolutePaths() {
            const string? input = @"c:\path/with.extension/filename.mod";
            Assert.Equal("mod", CommonFunctions.GetExtension(input));
        }
    }
}
