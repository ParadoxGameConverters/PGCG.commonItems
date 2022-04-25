using Xunit;

namespace commonItems.UnitTests; 

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
	public void TrimExtensionWorksOnPaths() {
		const string input = "gfx/bm_converted.png";
		Assert.Equal("gfx/bm_converted", CommonFunctions.TrimExtension(input));
	}
	[Fact]
	public void TrimExtensionDoesNotAffectDirectories() {
		const string input = "/path/with.extension/directory";
		Assert.Equal("/path/with.extension/directory", CommonFunctions.TrimExtension(input));
	}
}