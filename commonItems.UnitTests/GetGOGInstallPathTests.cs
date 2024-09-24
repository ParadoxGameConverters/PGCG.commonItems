using Xunit;

namespace commonItems.UnitTests; 

public sealed class GetGOGInstallPathTests {
	[Fact]
	public void NullIsReturnedWhenGameIsNotFound() {
		const int fakeGOGId = 420;
		var gamePath = CommonFunctions.GetGOGInstallPath(fakeGOGId);
        
		Assert.Null(gamePath);
	}
}