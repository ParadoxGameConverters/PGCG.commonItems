using System;
using System.Collections.Generic;
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
        [Fact] public void ModsCanBeLocatedUnpackedAndUpdated() {
            var incomingMods = new Mods {
                new Mod("Some mod", "mod/themod.mod") // mod's in fact named "The Mod" in the file
            };

            var modLoader = new ModLoader();
            modLoader.LoadMods("UnitTests/TestFiles", incomingMods);
            var mods = modLoader.UsableMods;

            Assert.Collection(mods, item => Assert.Equal(new Mod("The Mod", "TestFiles/mod/themod/"), item));
            Assert.Collection(mods[0].dependencies,
                item => Assert.Equal("Packed Mod", item),
                item => Assert.Equal("Missing Mod", item)
            );
        }
    }
}
