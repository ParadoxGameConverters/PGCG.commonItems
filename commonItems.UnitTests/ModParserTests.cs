using Xunit;

namespace commonItems.UnitTests {
	[Collection("Sequential")]
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class ModParserTests {
		[Fact]
		public void PrimitivesDefaultToBlank() {
			var reader = new BufferedReader(string.Empty);
			var mod = new ModParser();
			mod.ParseMod(reader);

			Assert.True(string.IsNullOrEmpty(mod.Name));
			Assert.True(string.IsNullOrEmpty(mod.Path));
			Assert.Empty(mod.Dependencies);
		}
		[Fact]
		public void PrimitivesCanBeSet() {
			var reader = new BufferedReader(
				"name = modName\n" +
				"path=modPath\n" +
				"dependencies = { dep1 dep2 }\n"
			);
			var mod = new ModParser();
			mod.ParseMod(reader);

			Assert.Equal("modName", mod.Name);
			Assert.Equal("modPath", mod.Path);
			Assert.Collection(mod.Dependencies,
				item => Assert.Equal("dep1", item),
				item => Assert.Equal("dep2", item)
			);
		}
		[Fact]
		public void PathCanBeSetFromArchive() {
			var reader = new BufferedReader("archive=modPath\n");
			var mod = new ModParser();
			mod.ParseMod(reader);

			Assert.Equal("modPath", mod.Path);
		}
		[Fact]
		public void ModIsInvalidIfPathOrNameUnSet() {
			var reader1 = new BufferedReader(string.Empty);
			var mod1 = new ModParser();
			mod1.ParseMod(reader1);
			Assert.False(mod1.IsValid());

			var reader2 = new BufferedReader("name=modName\n");
			var mod2 = new ModParser();
			mod2.ParseMod(reader2);
			Assert.False(mod2.IsValid());

			var reader3 = new BufferedReader("path=modPath\n");
			var mod3 = new ModParser();
			mod3.ParseMod(reader3);
			Assert.False(mod3.IsValid());
		}
		[Fact]
		public void ModIsValidIfNameAndPathSet() {
			var reader = new BufferedReader(
				"name=modName\n" +
				"path=modPath\n"
			);
			var mod = new ModParser();
			mod.ParseMod(reader);
			Assert.True(mod.IsValid());
		}
		[Fact]
		public void ModIsCompressedForZipAndBinPaths() {
			var reader1 = new BufferedReader("path=modPath\n");
			var mod1 = new ModParser();
			mod1.ParseMod(reader1);
			Assert.False(mod1.IsCompressed());

			var reader2 = new BufferedReader("path=modPath.zip\n");
			var mod2 = new ModParser();
			mod2.ParseMod(reader2);
			Assert.True(mod2.IsCompressed());

			var reader3 = new BufferedReader("path=modPath.bin\n");
			var mod3 = new ModParser();
			mod3.ParseMod(reader3);
			Assert.True(mod3.IsCompressed());
		}
	}
}
