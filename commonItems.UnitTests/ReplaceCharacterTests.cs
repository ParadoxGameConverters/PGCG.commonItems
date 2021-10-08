using Xunit;

namespace commonItems.UnitTests {
	public class ReplaceCharacterTests {
		[Fact]
		public void ReplaceCharacterCanReplaceSpaces() {
			const string input = "a file name.eu4";
			Assert.Equal("a_file_name.eu4", CommonFunctions.ReplaceCharacter(input, ' '));
		}
		[Fact]
		public void ReplaceCharacterCanReplaceMinuses() {
			const string input = "a file-with-name.eu4";
			Assert.Equal("a file_with_name.eu4", CommonFunctions.ReplaceCharacter(input, '-'));
		}
	}
}
