using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace commonItems.UnitTests {
    public class CommonFunctionsTests {
        [Fact]
        public void LastDigitOneGivesSt() {
            Assert.Equal("st", CommonFunctions.CardinalToOrdinal(1));
        }
        [Fact]
        public void LastDigitTwoGivesNd() {
            Assert.Equal("nd", CommonFunctions.CardinalToOrdinal(2));
        }
        [Fact]
        public void LastDigitThreeGivesRd() {
            Assert.Equal("rd", CommonFunctions.CardinalToOrdinal(3));
        }
        [Fact]
        public void RemainingDigitsGiveTh() {
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(4));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(5));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(6));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(7));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(8));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(9));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(0));
        }
        [Fact]
        public void TeensGiveTh() {
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(10));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(11));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(12));
            Assert.Equal("th", CommonFunctions.CardinalToOrdinal(13));
        }
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
        [Fact]
        public void ReplaceCharacterCanReplaceSpaces() {
            var input = @"a file name.eu4";
            Assert.Equal("a_file_name.eu4", CommonFunctions.ReplaceCharacter(input, ' '));
        }
        [Fact]
        public void ReplaceCharacterCanReplaceMinuses() {
            var input = @"a file-with-name.eu4";
            Assert.Equal("a file_with_name.eu4", CommonFunctions.ReplaceCharacter(input, '-'));
        }
    }
}
