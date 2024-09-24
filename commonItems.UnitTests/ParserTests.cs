using commonItems.Mods;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
// ReSharper disable InconsistentNaming

namespace commonItems.UnitTests; 

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public sealed class ParserTests {
	[Fact]
	public void AbsorbBOMAbsorbsBOM() {
		var stream = new BufferedReader("\xEF\xBB\xBF" + "More text");
		Parser.AbsorbBOM(stream);
		Assert.Equal("More text", stream.ReadToEnd());
	}

	[Fact]
	public void AbsorbBOMDoesNotAbsorbNonBOM() {
		var stream = new BufferedReader("More text");
		Parser.AbsorbBOM(stream);
		Assert.Equal("More text", stream.ReadToEnd());
	}

	private sealed class Test : Parser {
		public string? key;
		public string? value;
		public Test(BufferedReader bufferedReader) {
			RegisterKeyword("key", (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void KeywordsAreMatched() {
		var bufferedReader = new BufferedReader("key = value");
		var test = new Test(bufferedReader);
		Assert.Equal("key", test.key);
		Assert.Equal("value", test.value);
	}

	[Fact]
	public void QuotedKeywordsAreMatched() {
		var bufferedReader = new BufferedReader("\"key\" = value");
		var test = new Test(bufferedReader);
		Assert.Equal("\"key\"", test.key);
		Assert.Equal("value", test.value);
	}

	[Fact]
	public void WrongQuotedKeywordsAreMatched() {
		var bufferedReader = new BufferedReader("\"key_wrong\" = value");
		var test = new Test(bufferedReader);
		Assert.Null(test.key);
		Assert.Null(test.value);
	}

	private sealed class Test2 : Parser {
		public string key = "";
		public string? value;
		public Test2(BufferedReader bufferedReader) {
			RegisterKeyword("\"key\"", (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void QuotedKeywordsAreQuotedlyMatched() {
		var bufferedReader = new BufferedReader("\"key\" = value");
		var test = new Test2(bufferedReader);
		Assert.Equal("\"key\"", test.key);
		Assert.Equal("value", test.value);
	}

	[Fact]
	public void QuotedValuesAreParsed() {
		var bufferedReader = new BufferedReader(@"key = ""value quote""");
		var test = new Test(bufferedReader);
		Assert.Equal("key", test.key);
		Assert.Equal("value quote", test.value);
	}

	[Fact]
	public void QuotedValuesWithEscapedQuotesAreParsed() {
		var bufferedReader = new BufferedReader(@"key = ""value \""quote\"" string""");
		var test = new Test(bufferedReader);
		Assert.Equal("key", test.key);
		Assert.Equal(@"value \""quote\"" string", test.value);
	}

	[Fact]
	public void StringLiteralsAreParsed() {
		var bufferedReader = new BufferedReader(@"key = R""(value ""quote"" string)""");
		var test = new Test(bufferedReader);
		Assert.Equal("key", test.key);
		Assert.Equal(@"value ""quote"" string", test.value);
	}

	[Fact]
	public void WrongKeywordsAreIgnored() {
		var bufferedReader = new BufferedReader("wrongKey = value");
		var test = new Test(bufferedReader);
		Assert.True(string.IsNullOrEmpty(test.key));
		Assert.True(string.IsNullOrEmpty(test.value));
	}

	private sealed class Test3 : Parser {
		public string key = "";
		public string? value;
		public Test3(BufferedReader bufferedReader) {
			RegisterRegex("[key]+", (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void QuotedRegexesAreMatched() {
		var bufferedReader = new BufferedReader("\"key\" = value");
		var test = new Test3(bufferedReader);
		Assert.Equal("\"key\"", test.key);
		Assert.Equal("value", test.value);
	}

	private sealed class Test4 : Parser {
		public string key = "";
		public string? value;
		public Test4(BufferedReader bufferedReader) {
			RegisterRegex("[k\"ey]+", (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void QuotedRegexesAreQuotedlyMatched() {
		var bufferedReader = new BufferedReader("\"key\" = value");
		var test = new Test4(bufferedReader);
		Assert.Equal("\"key\"", test.key);
		Assert.Equal("value", test.value);
	}

	private sealed class Test5 : Parser {
		public string key = "";
		public string? value;
		public Test5(BufferedReader bufferedReader) {
			RegisterRegex(CommonRegexes.Catchall, (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void CatchAllCatchesQuotedKeys() {
		var bufferedReader = new BufferedReader("\"key\" = value");
		var test = new Test5(bufferedReader);
		Assert.Equal("\"key\"", test.key);
		Assert.Equal("value", test.value);
	}

	[Fact]
	public void CatchAllCatchesQuotedKeysWithWhitespaceInside() {
		var bufferedReader = new BufferedReader("\"this\tis a\nkey\r\n\" = value");
		var test = new Test5(bufferedReader);
		Assert.Equal("\"this\tis a key \"", test.key);
		Assert.Equal("value", test.value);
	}

	[Fact]
	public void CatchAllCatchesQuotedKeysWithFigurativeCrapInside() {
		var bufferedReader = new BufferedReader("\"this = is a silly { key\t} \" = value");
		var test = new Test5(bufferedReader);
		Assert.Equal("\"this = is a silly { key\t} \"", test.key);
		Assert.Equal("value", test.value);
	}

	private sealed class Test6 : Parser {
		public uint keyCount = 0;
		public Test6(BufferedReader bufferedReader) {
			RegisterRegex(CommonRegexes.Catchall, sr => {
				++keyCount;
				ParserHelpers.IgnoreItem(sr);
			});
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void SimpleDelegateRegexCanBeRegistered() {
		var bufferedReader = new BufferedReader("key = value key2 = value2 key3 = value3");
		var test = new Test6(bufferedReader);
		Assert.Equal((uint)3, test.keyCount);
	}

	[Fact]
	public void GetNextLexemeSkipsComments() {
		var reader = new BufferedReader("key1 = value1#this is a comment\n key2=value2");
		var lexeme1 = Parser.GetNextLexeme(reader);
		Assert.Equal("key1", lexeme1);
		var lexeme2 = Parser.GetNextLexeme(reader);
		Assert.Equal("=", lexeme2);
		var lexeme3 = Parser.GetNextLexeme(reader);
		Assert.Equal("value1", lexeme3);
		var lexeme4 = Parser.GetNextLexeme(reader);
		Assert.Equal("key2", lexeme4);
	}

	[Fact]
	public void NewLineEndsLexeme() {
		var reader = new BufferedReader("lexeme\nlexeme2");
		var lexeme = Parser.GetNextLexeme(reader);
		Assert.Equal("lexeme", lexeme);
		lexeme = Parser.GetNextLexeme(reader);
		Assert.Equal("lexeme2", lexeme);
	}

	[Fact]
	public void OpeningBraceEndsLexeme() {
		var reader = new BufferedReader("lexeme{stuff}");
		var lexeme = Parser.GetNextLexeme(reader);
		Assert.Equal("lexeme", lexeme);
	}

	[Fact]
	public void FileNotFoundIsLogged() {
		var output = new StringWriter();
		Console.SetOut(output);

		new Parser().ParseFile("missingFile.txt");
		Assert.Contains("[ERROR] Could not open missingFile.txt for parsing", output.ToString());
	}

	private sealed class FileTest : Parser {
		public string? value;
		public FileTest(string filename) {
			RegisterKeyword("key1", reader => value = reader.GetString());
			ParseFile(filename);
		}
	}

	[Fact]
	public void ParserCanParseFiles() {
		const string filename = "TestFiles/keyValuePair.txt";
		var value = new FileTest(filename).value;
		Assert.Equal("value1", value);
	}

	[Fact]
	public void RegisteredRulesCanBeCleared() {
		const string filename = "TestFiles/keyValuePair.txt";
		var parser = new FileTest(filename);
		var value = parser.value;
		Assert.Equal("value1", value);

		parser.value = null;
		parser.ClearRegisteredRules();
		parser.ParseFile(filename);
		value = parser.value;
		Assert.Null(value);
	}

	private sealed class Test7 : Parser {
		public string? key;
		public string? value;
		public string? broken;
		public Test7(BufferedReader bufferedReader) {
			RegisterKeyword("key", (reader, k) => {
				key = k;
				value = reader.GetString();
			});
			RegisterKeyword("broken", (_, k) => broken = k);
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void FastForwardTo0DepthWorksWithOpeningBrackets() {
		var bufferedReader = new BufferedReader("= { key = value = { broken } }");
		var test = new Test7(bufferedReader);
		Assert.Equal("key", test.key);
		Assert.Equal("value", test.value);
		Assert.Null(test.broken);
	}

	private sealed class TestCountry : Parser {
		public string Name { get; private set; } = string.Empty;
		public double Prestige { get; private set; }

		public TestCountry(BufferedReader reader) {
			RegisterKeyword("name", reader => Name = reader.GetString());
			RegisterKeyword("prestige", reader => Prestige = reader.GetDouble());
			ParseStream(reader);
		}
	}
	[Fact]
	public void ParserResolvesVariables() {
		var reader = new BufferedReader(
			"@best_country_on_earth_name = \"Roman Empire\"\n" +
			"name = @best_country_on_earth_name"
		);
		var country = new TestCountry(reader);
		Assert.Equal("Roman Empire", country.Name);
	}

	[Fact]
	public void ParserEvaluatesExpressions() {
		var reader = new BufferedReader(
			"@default_prestige = 50\n" +
			"prestige = @[default_prestige/2+10]"
		);
		var country = new TestCountry(reader);
		Assert.Equal(35, country.Prestige);
	}

	[Fact]
	public void InterpolatedExpressionsCanBeUsedInVariableValues() {
		var reader = new BufferedReader(
			"@cheap_cost_scale_addition_per_tier = 50\n" + // 50
			"@cheap_cost_base = 100\n" + // 100
			"@cheap_cost_tier_1 = @[cheap_cost_base]\n" + // 100
			"@cheap_cost_tier_2 = @[cheap_cost_tier_1 + cheap_cost_scale_addition_per_tier]"); // 150

		var parser = new Parser();
		parser.ParseStream(reader);

		Assert.Collection(reader.Variables,
			kvp => {
				Assert.Equal("cheap_cost_scale_addition_per_tier", kvp.Key);
				Assert.Equal(50, kvp.Value);
			},
			kvp => {
				Assert.Equal("cheap_cost_base", kvp.Key);
				Assert.Equal(100, kvp.Value);
			},
			kvp => {
				Assert.Equal("cheap_cost_tier_1", kvp.Key);
				Assert.Equal(100, kvp.Value);
			},
			kvp => {
				Assert.Equal("cheap_cost_tier_2", kvp.Key);
				Assert.Equal(150, kvp.Value);
			});
	}

	[Fact]
	public void ParserParsesGameFolderInVanillaAndMods() {
		const string gameRoot = "TestFiles/CK3/game";
		var mods = new List<Mod> { new("cool_mod", "TestFiles/mod/themod") };
		var modFS = new ModFilesystem(gameRoot, mods);

		var foundGovernments = new List<string>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.String, (reader, govName) => {
			foundGovernments.Add(govName);
			ParserHelpers.IgnoreItem(reader);
		});

		parser.ParseGameFolder("common/governments", modFS, "txt", recursive: false);
		foundGovernments.Should().Equal(
			"tribal_modded", // mod_tribal_govs.txt in mod
			"tribal_federation" // vanilla_governments.txt in game
		);
	}

	[Fact]
	public void ParserRecursivelyParsesGameFolderInVanillaAndMods() {
		const string gameRoot = "TestFiles/CK3/game";
		var mods = new List<Mod> { new("cool_mod", "TestFiles/mod/themod") };
		var modFS = new ModFilesystem(gameRoot, mods);

		var foundGovernments = new List<string>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.String, (reader, govName) => {
			foundGovernments.Add(govName);
			ParserHelpers.IgnoreItem(reader);
		});

		parser.ParseGameFolder("common/governments", modFS, "txt", recursive: true);

		foundGovernments.Should().Equal(
			"tribal_modded", // mod_tribal_govs.txt in mod
			"tribal_federation", // vanilla_governments.txt in game
			"constitutional_monarchy", // monarchy/mod_monarchy_govs.txt in mod
			"aristocratic_republic" // republic/republic_governments.txt in game
		);
	}

	[Fact]
	public void ParserParsesGameFileInVanillaAndMods() {
		const string gameRoot = "TestFiles/CK3/game";
		var mods = new List<Mod> {
			new("mod1", "TestFiles/mod/themod"),
			new("mod2", "TestFiles/mod/mod2")
		};
		var modFS = new ModFilesystem(gameRoot, mods);

		var foundAreas = new List<string>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.String, (reader, areaName) => {
			foundAreas.Add(areaName);
			ParserHelpers.IgnoreItem(reader);
		});

		parser.ParseGameFile("map_data/areas.txt", modFS);
		// File exists in game and mod1. Mod's version takes precedence.
		foundAreas.Should().Equal("themod_area");

		foundAreas.Clear();
		parser.ParseGameFile("map_data/areas2.txt", modFS);
		// File exists only in game.
		foundAreas.Should().Equal("vanilla1_area2");
	}
}