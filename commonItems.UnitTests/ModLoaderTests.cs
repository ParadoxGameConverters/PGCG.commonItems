using System;
using System.IO;
using Xunit;
using Mods = System.Collections.Generic.List<commonItems.Mod>;

namespace commonItems.UnitTests; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class ModLoaderTests {
	private const string testFilesPath = "TestFiles";

	[Fact]
	public void ModsCanBeLocatedUnpackedAndUpdated() {
		var incomingMods = new Mods {
			new("Some mod", "mod/themod.mod") // mod's in fact named "The Mod" in the file
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(testFilesPath, incomingMods);
		var mods = modLoader.UsableMods;

		Assert.Collection(mods,
			item => Assert.Equal(new Mod("The Mod", Path.Combine(testFilesPath, "mod", "themod")), item));
		Assert.Collection(mods[0].Dependencies,
			item => Assert.Equal("Missing Mod", item),
			item => Assert.Equal("Packed Mod", item)
		);
	}
	[Fact]
	public void BrokenMissingAndNonexistentModsAreDiscarded() {
		var incomingMods = new Mods {
			new("", "mod/themod.mod"), // no name given
			new("Broken mod", "mod/brokenmod.mod"), // no path
			new("Missing mod", "mod/missingmod.mod"), // missing directory
			new("Nonexistent mod", "mod/nonexistentmod.mod") // doesn't exist.
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(testFilesPath, incomingMods);
		var mods = modLoader.UsableMods;

		Assert.Collection(mods,
			item => Assert.Equal(new Mod("The Mod", Path.Combine(testFilesPath, "mod", "themod")), item));
	}
	[Fact]
	public void CompressedModsCanBeUnpacked() {
		var incomingMods = new Mods {
			new("some packed mod", "mod/packedmod.mod")
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(testFilesPath, incomingMods);
		var mods = modLoader.UsableMods;

		Assert.Collection(mods,
			item => Assert.Equal(new Mod("Packed Mod", Path.Combine("mods", "packedmod")), item));
		Assert.True(Directory.Exists(Path.Combine("mods", "packedmod")));
	}
	[Fact]
	public void BrokenCompressedModsAreSkipped() {
		var output = new StringWriter();
		Console.SetOut(output);

		var incomingMods = new Mods {
			new("broken packed mod", "mod/brokenpacked.mod")
		};

		var modLoader = new ModLoader();
		modLoader.LoadMods(testFilesPath, incomingMods);
		var usableMods = modLoader.UsableMods;

		Assert.Empty(usableMods);
		Assert.False(Directory.Exists(Path.Combine("mods", "brokenpacked")));
	}
}