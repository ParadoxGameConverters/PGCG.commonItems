using commonItems.Mods;
using FluentAssertions;
using Xunit;

namespace commonItems.UnitTests.Mods;

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
		Assert.Empty(mod.ReplacedPaths);

		var theModFile = new ModParser();
		theModFile.ParseMod("TestFiles/mod/empty_mod_file.mod");  // TODO: add the mod file
		Assert.Empty(theModFile.Name);
		Assert.Empty(theModFile.Path);
		Assert.Empty(theModFile.Dependencies);
		Assert.Empty(theModFile.ReplacedPaths);
	}
	[Fact]
	public void PrimitivesCanBeSet() {
		var reader = new BufferedReader(
			"name = modName\n" +
			"path=modPath\n" +
			"dependencies = { dep1 dep2 }\n" +
			"replace_path=\"replaced/path\"\n" +
			"replace_path=\"replaced/path/two\"\n"
		);
		var mod = new ModParser();
		mod.ParseMod(reader);

		Assert.Equal("modName", mod.Name);
		Assert.Equal("modPath", mod.Path);
		Assert.Collection(mod.Dependencies,
			item => Assert.Equal("dep1", item),
			item => Assert.Equal("dep2", item)
		);
		mod.ReplacedPaths.Should().Equal("replaced/path", "replaced/path/two");

		var theModFile = new ModParser();
		theModFile.ParseMod("TestFiles/mod/parseable_mod_file.mod");
		
		Assert.Equal("modName", theModFile.Name);
		Assert.Equal("modPath", theModFile.Path);
		theModFile.Dependencies.Should().Equal("dep1", "dep2");
		theModFile.ReplacedPaths.Should().Equal("replaced/path", "replaced/path/two");
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

	[Fact]
	public void PathCanBeUpdated() {
		var reader = new BufferedReader(
			"name=modName\n" +
			"path=modPath\n");

		var theModFile = new ModParser();
		theModFile.ParseMod(reader);
		theModFile.Path = "updated_path";
		
		Assert.Equal("updated_path", theModFile.Path);
	}
}