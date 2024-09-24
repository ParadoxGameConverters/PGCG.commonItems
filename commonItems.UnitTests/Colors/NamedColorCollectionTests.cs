using commonItems.Colors;
using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests.Colors;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class NamedColorCollectionTests {
	private const string GameRoot = "TestFiles/CK3/game";
	private static readonly List<Mod> Mods = new() { new("Cool Mod", "TestFiles/mod/themod") };
	private readonly ModFilesystem modFS = new(GameRoot, Mods);
	private readonly ColorFactory referenceColorFactory = new();

	[Fact]
	public void CollectionDefaultsToEmpty() {
		// ReSharper disable once CollectionNeverUpdated.Local
		var namedColors = new NamedColorCollection();

		Assert.Empty(namedColors);
	}

	[Fact]
	public void FormatExceptionIsCaught() {
		var output = new StringWriter();
		Console.SetOut(output);

		var namedColors = new NamedColorCollection();
		var mods = new List<Mod> {new("broken mod", "TestFiles/mod/mod_with_broken_colors")};
		var modFSWithBrokenColors = new ModFilesystem(GameRoot, mods);
		namedColors.LoadNamedColors("common/named_colors", modFSWithBrokenColors);

		var outStr = output.ToString();
		Assert.Contains("[WARN] Failed to read color ck2_black: Color has wrong number of components", outStr);
		// delian_league_gold is also malformed, but wrong number of RGB components is gracefully handled.
		Assert.DoesNotContain("[WARN] Failed to read color delian_league_gold", outStr);
	}

	[Fact]
	public void NamedColorsCanBeLoadedFromGameAndMods() {
		var namedColors = new NamedColorCollection();
		namedColors.LoadNamedColors("common/named_colors", modFS);

		Assert.Collection(namedColors,
			kvp => {
				Assert.Equal("antigonid_yellow", kvp.Key); // from game
				var expectedColorReader = new BufferedReader("= rgb { 246 223 15 }");
				Assert.Equal(referenceColorFactory.GetColor(expectedColorReader), kvp.Value);
			},
			kvp => {
				Assert.Equal("ck2_black", kvp.Key); // from mod
				var expectedColorReader = new BufferedReader("= hsv { 0 0 0.12 }");
				Assert.Equal(referenceColorFactory.GetColor(expectedColorReader), kvp.Value);
			},
			kvp => {
				Assert.Equal("delian_league_gold", kvp.Key); // from mod
				var expectedColorReader = new BufferedReader("= rgb { 250 250 210 }");
				Assert.Equal(referenceColorFactory.GetColor(expectedColorReader), kvp.Value);
			},
			kvp => {
				Assert.Equal("pitch_black", kvp.Key); // from game, overwritten by mod
				var expectedColorReader = new BufferedReader("= hsv { 0 0 0.15 }");
				Assert.Equal(referenceColorFactory.GetColor(expectedColorReader), kvp.Value);
			}
		);
	}
}