using commonItems.Mods;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests;

public sealed class ScriptValueCollectionTests {
	private const string GameRoot = "TestFiles/CK3/game";
	private static readonly List<Mod> Mods = [new("Cool Mod", "TestFiles/mod/themod")];
	private readonly ModFilesystem modFS = new(GameRoot, Mods);

	[Fact]
	public void SimpleScriptValuesAreReadFromGameAndMods() {
		var defines = new Defines();
		defines.LoadDefines(modFS);
		
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, defines);

		scriptValueCollection.Keys.Should()
			.BeEquivalentTo(
				"value_using_value_defined_below",
				"value1",
				"value2",
				"value3",
				"value4",
				"mod_value",
				"common_value",
				"value_using_value",
				"value_using_variable",
				"value_using_expression",
				"bool_value_yes",
				"bool_value_no",
				"cheap_building_tier_1_cost",
				"value_using_define"
			);

		Assert.Equal(-0.4d, scriptValueCollection["value_using_value_defined_below"]); // same as value2
		Assert.Equal(0.4d, scriptValueCollection["value1"]);
		Assert.Equal(-0.4d, scriptValueCollection["value2"]);
		Assert.Equal(1d, scriptValueCollection["value3"]);
		Assert.Equal(-3d, scriptValueCollection["value4"]);
		Assert.Equal(3.2d, scriptValueCollection["mod_value"]);
		Assert.Equal(69d, scriptValueCollection["common_value"]); // 68 in game, overridden by 69 in mod
		Assert.Equal(0.4d, scriptValueCollection["value_using_value"]);
		Assert.Equal(420, scriptValueCollection["value_using_variable"]);
		Assert.Equal(69, scriptValueCollection["value_using_expression"]);
		Assert.Equal(1, scriptValueCollection["bool_value_yes"]);
		Assert.Equal(0, scriptValueCollection["bool_value_no"]);
		Assert.Equal(100, scriptValueCollection["cheap_building_tier_1_cost"]);
		Assert.Equal(-30, scriptValueCollection["value_using_define"]);
	}

	[Fact]
	public void ComplexValuesAreIgnored() {
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, new Defines());

		Assert.DoesNotContain("clan_government_tax_max_possible", scriptValueCollection.Keys);
	}

	[Fact]
	public void IndexerThrowsExceptionWhenKeyIsNotFound() {
		var scriptValueCollection = new ScriptValueCollection();
		Assert.Throws<KeyNotFoundException>(() => _ = scriptValueCollection["missing_key"]);
	}

	[Fact]
	public void GetValueForStringReturnsNumberForValidNumberString() {
		var scriptValueCollection = new ScriptValueCollection();

		Assert.Equal(5.5f, scriptValueCollection.GetValueForString("5.5"));
	}

	[Fact]
	public void GetValueForStringReturnsNullForInvalidNumberString() {
		var scriptValueCollection = new ScriptValueCollection();
		const string invalidNumberString = "2e";

		Assert.Null(scriptValueCollection.GetValueForString(invalidNumberString));
	}

	[Fact]
	public void GetValueForStringReturnsNumberForExistingScriptValue() {
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, new Defines());

		Assert.Equal(0.4d, scriptValueCollection.GetValueForString("value1"));
	}

	[Fact]
	public void GetValueForStringReturnsNullForUndefinedScriptValue() {
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, new Defines());

		Assert.Null(scriptValueCollection.GetValueForString("undefined_value"));
	}

	[Fact]
	public void ContainsKeyReturnsCorrectValues() {
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, new Defines());

		Assert.True(scriptValueCollection.ContainsKey("value1"));
		Assert.True(scriptValueCollection.ContainsKey("mod_value"));
		Assert.True(scriptValueCollection.ContainsKey("common_value"));

		Assert.False(scriptValueCollection.ContainsKey("missing_value"));
	}

	[Fact]
	public void Values() {
		var scriptValueCollection = new ScriptValueCollection();
		scriptValueCollection.LoadScriptValues(modFS, new Defines());

		// value4 should be found when iterating over the collection
		bool value4Found = false;
		using var enumerator = scriptValueCollection.GetEnumerator();
		foreach (var (key, value) in scriptValueCollection) {
			if (key != "value4") {
				continue;
			}

			value4Found = true;
			Assert.Equal(-3d, value);
		}

		Assert.True(value4Found);
	}
}