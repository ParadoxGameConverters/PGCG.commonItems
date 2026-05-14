using Xunit;

namespace commonItems.UnitTests;

public sealed class SplitPathTests {
	[Fact]
	public void SplitPathSplitsOnBothSeparatorsAndRemovesEmptyEntries() {
		var splitPath = CommonFunctions.SplitPath(@"\\folder//subfolder\file/");

		Assert.Equal(["folder", "subfolder", "file"], splitPath);
	}

	[Theory]
	[InlineData("")]
	[InlineData("/")]
	[InlineData("\\")]
	[InlineData("///\\\\")]
	public void SplitPathReturnsEmptyArrayForEmptyOrSeparatorOnlyPath(string path) {
		var splitPath = CommonFunctions.SplitPath(path);

		Assert.Empty(splitPath);
	}
}