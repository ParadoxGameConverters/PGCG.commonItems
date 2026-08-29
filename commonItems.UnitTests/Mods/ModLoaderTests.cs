using commonItems.Exceptions;
using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;
using ModList = System.Collections.Generic.List<commonItems.Mods.Mod>;

namespace commonItems.UnitTests.Mods;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class ModLoaderTests {
	private const string TestFilesPath = "TestFiles";
	private readonly GameVersion installedGameVersion = new("1.31");
	private static readonly Lock ConsoleLock = new();

	[Fact]
	public void ModsCanBeLocatedUnpackedAndUpdated() {
		var incomingMods = new ModList {
			new("Some mod", "mod/themod.mod"), // mod's in fact named "The Mod" in the file
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var mods = modLoader.UsableMods;

		var mod = Assert.Single(mods);
		Assert.Equal(new Mod("The Mod", Path.Combine(TestFilesPath, "mod", "themod")), mod);
		Assert.Collection(mods[0].Dependencies,
			item => Assert.Equal("Packed Mod", item),
			item => Assert.Equal("Missing Mod", item)
		);
	}
	[Fact]
	public void BrokenMissingAndNonexistentModsAreDiscarded() {
		var incomingMods = new ModList {
			new("", "mod/themod.mod"), // no name given
			new("Broken mod", "mod/brokenmod.mod"), // no path
			new("Missing mod", "mod/missingmod.mod"), // missing directory
			new("Nonexistent mod", "mod/nonexistentmod.mod"), // doesn't exist.
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var mods = modLoader.UsableMods;

		var mod = Assert.Single(mods);
		Assert.Equal(new Mod("The Mod", Path.Combine(TestFilesPath, "mod", "themod")), mod);
	}
	[Fact]
	public void CompressedModsCanBeUnpacked() {
		var incomingMods = new ModList {
			new("some packed mod", "mod/packedmod.mod"),
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var mods = modLoader.UsableMods;

		var mod = Assert.Single(mods);
		Assert.Equal(new Mod("Packed Mod", Path.Combine("mods", "packedmod")), mod);
		Assert.True(Directory.Exists(Path.Combine("mods", "packedmod")));
	}
	[Fact]
	public void BrokenCompressedModsAreSkipped() {
		StringWriter output;
		List<Mod> usableMods;
		lock (ConsoleLock) {
			output = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(output);
			try {
				var incomingMods = new ModList {
					new("broken packed mod", "mod/brokenpacked.mod"),
				};

				var modLoader = new ModLoader();
				modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
				usableMods = modLoader.UsableMods;
			} finally {
				Console.SetOut(originalOut);
			}
		}

		Assert.Empty(usableMods);
		Assert.False(Directory.Exists(Path.Combine("mods", "brokenpacked")));
	}

	[Fact]
	public void LoadModsLogsWhenNoMods() {
		string outputStr;
		List<Mod> usableMods;
		lock (ConsoleLock) {
			var output = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(output);
			try {
				var modLoader = new ModLoader();
				modLoader.LoadMods(TestFilesPath, [], installedGameVersion, throwForOutOfDateMods: false);
				usableMods = modLoader.UsableMods;
				outputStr = output.ToString();
			} finally {
				Console.SetOut(originalOut);
			}
		}

		Assert.Empty(usableMods);
		Assert.Contains("[INFO] No mods were detected in savegame. Skipping mod processing.", outputStr);
	}

	[Fact]
	public void LoadModsLogsWarningForOutOfDateMods() {
		string consoleOutput;
		List<Mod> usableMods;
		lock (ConsoleLock) {
			var output = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(output);
			try {
				var incomingMods = new ModList {
					new("Outdated Mod", "mod/outdated.mod"), // supports up to 1.30
				};
				var modLoader = new ModLoader();
				modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
				usableMods = modLoader.UsableMods;
				consoleOutput = output.ToString();
			} finally {
				Console.SetOut(originalOut);
			}
		}

		// Should still be added to usable mods.
		var mod = Assert.Single(usableMods);
		Assert.Equal(new("Outdated Mod", Path.Combine(TestFilesPath, "mod", "outdated")), mod);
		Assert.Contains("[WARN] \t\tMod [Outdated Mod] supports game version 1.30.*, but your game version is 1.31. " +
		                "Proceeding anyway, but this can cause issues.", consoleOutput);
	}

	[Fact]
	public void LoadModsThrowsForGrosslyOutOfDateModsWhenConfigured() {
		var incomingMods = new ModList {
			new("Outdated Mod", "mod/outdated.mod"), // supports up to 1.30
		};
		var modLoader = new ModLoader();
		var exception = Assert.Throws<UserErrorException>(() =>
			modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: true)
		);

		Assert.Equal("\t\tMod [Outdated Mod] supports game version 1.30.*, but your game version is 1.31. Cannot continue.", exception.Message);
	}

	[Fact]
	public void LoadModsLogWarningForSlightlyOutOfDateMods() {
		string consoleOutput;
		List<Mod> usableMods;
		lock (ConsoleLock) {
			var output = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(output);
			try {
				var gameVersion = new GameVersion("1.31.1");

				var incomingMods = new ModList {
					new("Slightly Outdated Mod", "mod/slightlyoutdated.mod"), // supports up to 1.31.0
				};
				var modLoader = new ModLoader();
				modLoader.LoadMods(TestFilesPath, incomingMods, gameVersion, throwForOutOfDateMods: true);
				usableMods = modLoader.UsableMods;
				consoleOutput = output.ToString();
			} finally {
				Console.SetOut(originalOut);
			}
		}
		// Should still be added to usable mods.
		Assert.Single(usableMods);

		// Check if the warning is logged, but should not throw as it's only slightly out of date.
		Assert.Contains("[WARN] \t\tMod [Slightly Outdated Mod] supports game version 1.31.0.*, but your game version is 1.31.1. " +
		                "Proceeding anyway, but this can cause issues.", consoleOutput);
	}

	[Fact]
	public void SteamWorkshopNameCanBeRetrievedForModMissingFromDisk() {
		string consoleOutput;
		List<Mod> usableMods;
		ModList incomingMods;
		lock (ConsoleLock) {
			var output = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(output);
			try {
				incomingMods = new ModList {
					new(name: string.Empty, path: "mod/ugc_2845446001.mod"), // Timeline Extension for Invictus
				};
				var modLoader = new ModLoader();
				modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
				usableMods = modLoader.UsableMods;
				consoleOutput = output.ToString();
			} finally {
				Console.SetOut(originalOut);
			}
		}

		Assert.Empty(usableMods);
		// Check generic part is always present; workshop name part may vary or be missing if Steam request fails (network flakiness)
		Assert.Contains($"Savegame uses mod at {incomingMods[0].Path}", consoleOutput);
		Assert.Contains("which is not present on disk.", consoleOutput);
	}
}