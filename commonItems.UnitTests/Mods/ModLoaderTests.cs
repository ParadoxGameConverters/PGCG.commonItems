using commonItems.Exceptions;
using commonItems.Mods;
using System;
using System.IO;
using Xunit;
using ModList = System.Collections.Generic.List<commonItems.Mods.Mod>;

namespace commonItems.UnitTests.Mods;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class ModLoaderTests {
	private const string TestFilesPath = "TestFiles";
	private readonly GameVersion installedGameVersion = new("1.31");

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
		var output = new StringWriter();
		Console.SetOut(output);

		var incomingMods = new ModList {
			new("broken packed mod", "mod/brokenpacked.mod"),
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var usableMods = modLoader.UsableMods;

		Assert.Empty(usableMods);
		Assert.False(Directory.Exists(Path.Combine("mods", "brokenpacked")));
	}

	[Fact]
	public void LoadModsLogsWhenNoMods() {
		var output = new StringWriter();
		Console.SetOut(output);

		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, [], installedGameVersion, throwForOutOfDateMods: false);
		var usableMods = modLoader.UsableMods;

		Assert.Empty(usableMods);
		Assert.Contains("[INFO] No mods were detected in savegame. Skipping mod processing.", output.ToString());
	}

	[Fact]
	public void LoadModsLogsWarningForOutOfDateMods() {
		var output = new StringWriter();
		Console.SetOut(output);

		var incomingMods = new ModList {
			new("Outdated Mod", "mod/outdated.mod"), // supports up to 1.30
		};
		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var usableMods = modLoader.UsableMods;

		// Should still be added to usable mods.
		var mod = Assert.Single(usableMods);
		Assert.Equal(new("Outdated Mod", Path.Combine(TestFilesPath, "mod", "outdated")), mod);
		var consoleOutput = output.ToString();
		Assert.Contains("[WARN] \t\tMod [Outdated Mod] supports game version 1.30.*, but the installed version is 1.31. " +
		                "Proceeding anyway, but this can cause issues.", consoleOutput);
	}

	[Fact]
	public void LoadModsThrowsForOutOfDateModsWhenConfigured() {
		var incomingMods = new ModList {
			new("Outdated Mod", "mod/outdated.mod"), // supports up to 1.30
		};
		var modLoader = new ModLoader();
		var exception = Assert.Throws<UserErrorException>(() =>
			modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: true)
		);

		Assert.Equal("\t\tMod [Outdated Mod] supports game version 1.30.*, but the installed version is 1.31. Cannot continue.", exception.Message);
	}

	[Fact]
	public void SteamWorkshopNameCanBeRetrievedForModMissingFromDisk() {
		var output = new StringWriter();
		Console.SetOut(output);

		var incomingMods = new ModList {
			new(name: string.Empty, path: "mod/ugc_2845446001.mod"), // Timeline Extension for Invictus
		};
		var modLoader = new ModLoader();
		modLoader.LoadMods(TestFilesPath, incomingMods, installedGameVersion, throwForOutOfDateMods: false);
		var usableMods = modLoader.UsableMods;

		Assert.Empty(usableMods);
		var expectedModDetails = $"mod at {incomingMods[0].Path} " +
		                         "(probable Steam Workshop name: Timeline Extension for Invictus)";
		var consoleOutput = output.ToString();
		Assert.Contains($"Savegame uses {expectedModDetails}, which is not present on disk. " +
		                "Skipping at your risk, but this can greatly affect conversion.", consoleOutput);
	}
}