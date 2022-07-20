using commonItems.Mods;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests; 

public class NamedColorCollectionTests {
	private const string GameRoot = "TestFiles/CK3/game";
	private static readonly List<Mod> mods = new() { new("Cool Mod", "TestFiles/mod/themod") };
	private readonly ModFilesystem modFS = new(GameRoot, mods);
	private readonly ColorFactory referenceColorFactory = new();
	
	[Fact]
	public void CollectionDefaultsToEmpty() {
		// ReSharper disable once CollectionNeverUpdated.Local
		var namedColors = new NamedColorCollection();
		
		Assert.Empty(namedColors);
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