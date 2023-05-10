using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class GetSteamInstallPathTests {
    [Fact]
    public void NullIsReturnedWhenGameIsNotFound() {
        const int fakeSteamId = 420;
        var gamePath = CommonFunctions.GetSteamInstallPath(fakeSteamId);
        
        Assert.Null(gamePath);
    }

    [Fact]
    public void ErrorIsLoggedWhenSteamGameIdIsInvalid() {
	    var output = new StringWriter();
	    Console.SetOut(output);
	    
	    const int fakeSteamId = -1;
	    var gamePath = CommonFunctions.GetSteamInstallPath(fakeSteamId);
	    Assert.Null(gamePath);
	    
	    Assert.Contains($"Error occurred when locating Steam game {fakeSteamId}: ", output.ToString());
    }
}