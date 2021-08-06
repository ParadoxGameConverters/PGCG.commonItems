using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using Name = System.String;
using Path = System.String;
using Mods = System.Collections.Generic.List<Mod>;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class ModLoaderTests {
        string testFilesPath = "UnitTests/TestFiles";
        [Fact] public void ModsCanBeLocatedUnpackedAndUpdated() {
            var incomingMods = new Mods {
                new Mod("Some mod", "mod/themod.mod") // mod's in fact named "The Mod" in the file
            };

            var modLoader = new ModLoader();
            modLoader.LoadMods(testFilesPath, incomingMods);
            var mods = modLoader.UsableMods;

            Assert.Collection(mods,
                item => Assert.Equal(new Mod("The Mod", System.IO.Path.Combine(testFilesPath, "mod", "themod")), item));
            Assert.Collection(mods[0].dependencies,
                item => Assert.Equal("Missing Mod", item),
                item => Assert.Equal("Packed Mod", item)
            );
        }
        [Fact] public void BrokenMissingAndNonexistentModsAreDiscarded() {
            var incomingMods = new Mods {
                new Mod("", "mod/themod.mod"), // no name given
                new Mod("Broken mod", "mod/brokenmod.mod"), // no path
                new Mod("Missing mod", "mod/missingmod.mod"), // missing directory
                new Mod("Nonexistent mod", "mod/nonexistentmod.mod") // doesn't exist.
            };

            var modLoader = new ModLoader();
            modLoader.LoadMods(testFilesPath, incomingMods);
            var mods = modLoader.UsableMods;

            Assert.Collection(mods,
                item => Assert.Equal(new Mod("The Mod", System.IO.Path.Combine(testFilesPath, "mod", "themod")), item));
        }
        [Fact]
        public void CompressedModsCanBeUnpacked() {
            var incomingMods = new Mods {
                new Mod("some packed mod", "mod/packedmod.mod")
            };

            var modLoader = new ModLoader();
            modLoader.LoadMods(testFilesPath, incomingMods);
            var mods = modLoader.UsableMods;

            Assert.Collection(mods,
                item => Assert.Equal(new Mod("Packed Mod", System.IO.Path.Combine("mods", "packedmod")), item));
            Assert.True(Directory.Exists(System.IO.Path.Combine("mods", "packedmod")));
        }
        [Fact]
        public void BrokenCompressedModsAreNotSkippedEvenThoughTheyShouldBe() {
            var incomingMods = new Mods {
                new Mod("broken packed mod", "mod/brokenpacked.mod")
            };

            var modLoader = new ModLoader();
            modLoader.LoadMods(testFilesPath, incomingMods);
            var mods = modLoader.UsableMods;

            Assert.Collection(mods,
                item => Assert.Equal(new Mod("Broken Packed Mod", System.IO.Path.Combine("mods", "brokenpacked")), item));
            Assert.True(Directory.Exists(System.IO.Path.Combine("mods", "brokenpacked")));
        }
    }
}
