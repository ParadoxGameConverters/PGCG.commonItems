using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class GetSteamInstallPathTests {
    [Fact]
    public void MessageIsLoggedAndNullIsReturnedWhenGameIsNotFound() {
        const int fakeSteamId = 420;

		var output = new StringWriter();
		Console.SetOut(output);

        var gamePath = CommonFunctions.GetSteamInstallPath(fakeSteamId);
        Assert.Null(gamePath);
        Assert.Contains($"Error occurred when locating Steam game {fakeSteamId}: ", output.ToString());
    }
}