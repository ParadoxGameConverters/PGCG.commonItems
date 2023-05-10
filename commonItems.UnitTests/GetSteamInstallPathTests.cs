using Xunit;

namespace commonItems.UnitTests;

public class GetSteamInstallPathTests {
    [Fact]
    public void NullIsReturnedWhenGameIsNotFound() {
        const int fakeSteamId = 420;
        var gamePath = CommonFunctions.GetSteamInstallPath(fakeSteamId);
        
        Assert.Null(gamePath);
    }
}