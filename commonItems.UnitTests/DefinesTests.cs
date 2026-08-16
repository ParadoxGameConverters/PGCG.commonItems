using commonItems.Mods;
using Xunit;

namespace commonItems.UnitTests;

public class DefinesTests {
	private const string GameRoot = "TestFiles/CK3/game";
	private readonly ModFilesystem modFS = new(GameRoot, []);

	[Fact]
	public void DefinesAreCorrectlyLoaded() {
		var defines = new Defines();
		defines.LoadDefines(modFS);

		Assert.Equal("-30", defines.GetValue("NDomicile", "TEMPERAMENT_THRESHOLD_LOW"));
		Assert.Equal("\"event:/SFX/UI/Actions/sfx_ui_action_sacrifice\"", defines.GetValue("NTest", "RELIGION_SACRIFICE"));
		Assert.Equal("\"snapshot:/States/MainMenu\"", defines.GetValue("NTest", "FRONTEND_HANDLER_SNAPSHOT"));
	}
}