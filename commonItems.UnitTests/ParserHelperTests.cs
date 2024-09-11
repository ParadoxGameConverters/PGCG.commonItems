using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests;

[Collection("Sequential")]
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class ParserHelperTests {
	private const string GameRoot = "TestFiles/CK3/game";
	private static readonly List<Mod> mods = [new("Cool Mod", "TestFiles/mod/themod")];
	private readonly ModFilesystem modFS = new(GameRoot, mods);

	[Fact]
	public void IgnoreItemIgnoresSimpleText() {
		var input = new BufferedReader("ignore_me More text");
		ParserHelpers.IgnoreItem(input);
		Assert.Equal("More text", input.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresAssignedText() {
		var input = new BufferedReader("= ignore_me More text");
		ParserHelpers.IgnoreItem(input);
		Assert.Equal("More text", input.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresBracedItem() {
		var input = new BufferedReader("= { { ignore_me } } More text");
		ParserHelpers.IgnoreItem(input);
		Assert.Equal(" More text", input.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresAssignedBracedItem() {
		var input = new BufferedReader("= { { ignore_me } } More text");
		ParserHelpers.IgnoreItem(input);
		Assert.Equal(" More text", input.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresUnclosedBlockInTheEnd() {
		var reader = new BufferedReader("= { ignore_me=");
		ParserHelpers.IgnoreItem(reader);
		Assert.True(reader.EndOfStream);
	}

	private class Test1 : Parser {
		public string? value1;
		public string? value2;

		public Test1(BufferedReader bufferedReader) {
			RegisterKeyword("key1", reader => value1 = reader.GetString());
			RegisterKeyword("key2", reader => value2 = reader.GetString());
			this.IgnoreAndLogUnregisteredItems();
			ParseStream(bufferedReader);
		}
	}

	[Fact]
	public void IgnoreAndLogItemLogsIgnoredKeyword() {
		var output = new StringWriter();
		Console.SetOut(output);

		var input = new BufferedReader("key1=val1 key2=val2 key3=mess");
		var test = new Test1(input);
		Assert.Equal("val1", test.value1);
		Assert.Equal("val2", test.value2);
		Assert.Contains("[DEBUG] Ignoring keyword: key3", output.ToString());
	}

	[Fact]
	public void GetIntsDefaultsToEmpty() {
		var reader = new BufferedReader(string.Empty);
		Assert.Empty(reader.GetInts());
	}

	[Fact]
	public void GetIntsAddsInts() {
		var reader = new BufferedReader("1 2 3");
		Assert.Equal([1, 2, 3], reader.GetInts());
	}

	[Fact]
	public void GetIntsAddsNegativeInts() {
		var reader = new BufferedReader("-1 -2 -3");
		Assert.Equal([-1, -2, -3], reader.GetInts());
	}

	[Fact]
	public void GetIntsAddsQuotedInts() {
		var reader = new BufferedReader("\"1\" \"2\" \"3\"");
		Assert.Equal([1, 2, 3], reader.GetInts());
	}

	[Fact]
	public void GetIntsAddsQuotedNegativeInts() {
		var reader = new BufferedReader("\"-1\" \"-2\" \"-3\"");
		Assert.Equal([-1, -2, -3], reader.GetInts());
	}

	[Fact]
	public void GetIntsAddsIntsFromBracedBlock() {
		var reader = new BufferedReader(" = {1 2 3} 4");
		Assert.Equal([1, 2, 3], reader.GetInts());
	}

	[Fact]
	public void GetIntGetsIntAfterEquals() {
		var reader = new BufferedReader(" = 1");
		Assert.Equal(1, reader.GetInt());
	}

	[Fact]
	public void GetIntGetsNegativeIntAfterEquals() {
		var reader = new BufferedReader(" = -1");
		Assert.Equal(-1, reader.GetInt());
	}

	[Fact]
	public void GetIntGetsQuotedIntAfterEquals() {
		var reader = new BufferedReader(" = \"-1\"");
		Assert.Equal(-1, reader.GetInt());
	}

	[Fact]
	public void GetIntLogsInvalidInput() {
		var output = new StringWriter();
		Console.SetOut(output);

		var reader = new BufferedReader("= foo");
		var theInteger = reader.GetInt();
		Assert.Contains("[WARN] Could not convert string foo to int!", output.ToString());
		Assert.Equal(0, theInteger);
	}

	[Fact]
	public void GetDoublesDefaultsToEmpty() {
		var reader = new BufferedReader(string.Empty);
		Assert.Empty(reader.GetDoubles());
	}

	[Fact]
	public void GetDoublesAddsDoubles() {
		var reader = new BufferedReader("1.25 2.5 3.75");
		Assert.Equal([1.25, 2.5, 3.75], reader.GetDoubles());
	}

	[Fact]
	public void GetDoublesAddsNegativeDoubles() {
		var reader = new BufferedReader("1.25 -2.5 -3.75");
		Assert.Equal([1.25, -2.5, -3.75], reader.GetDoubles());
	}

	[Fact]
	public void GetDoublesAddsQuotedDoubles() {
		var reader = new BufferedReader("\"1.25\" \"2.5\" \"3.75\"");
		Assert.Equal([1.25, 2.5, 3.75], reader.GetDoubles());
	}

	[Fact]
	public void GetDoublesAddsQuotedNegativeDoubles() {
		var reader = new BufferedReader("\"1.25\" \"-2.5\" \"-3.75\"");
		Assert.Equal([1.25, -2.5, -3.75], reader.GetDoubles());
	}

	[Fact]
	public void GetDoublesAddsDoublesFromBracedBlock() {
		var reader = new BufferedReader(" = {1.25 2.5 3.75} 4.5");
		Assert.Equal([1.25, 2.5, 3.75], reader.GetDoubles());
	}

	[Fact]
	public void GetDoubleGetsDoubleAfterEquals() {
		var reader = new BufferedReader(" = 1.25");
		Assert.Equal(1.25, reader.GetDouble());
	}

	[Fact]
	public void GetDoubleGetsQuotedDoubleAfterEquals() {
		var reader = new BufferedReader(" = \"1.25\"");
		Assert.Equal(1.25, reader.GetDouble());
	}

	[Fact]
	public void GetDoubleLogsNotMatchingInput() {
		var output = new StringWriter();
		Console.SetOut(output);

		var reader = new BufferedReader("= \"345.345 foo\"");
		var theDouble = reader.GetDouble();
		Assert.Contains("[WARN] Could not convert string 345.345 foo to double!", output.ToString());
		Assert.Equal(0, theDouble);
	}

	[Fact]
	public void GetDoubleCanBeUsedWithExpressions() {
		var reader = new BufferedReader("= @[variable - 2]");
		reader.Variables.Add("variable", 42.5);

		Assert.Equal(40.5, reader.GetDouble());
	}

	[Fact]
	public void GetDoubleCanBeUsedWithScriptValues() {
		var scriptValues = new ScriptValueCollection();
		scriptValues.LoadScriptValues(modFS);
		var reader = new BufferedReader(" = mod_value");
		Assert.Equal(3.2, reader.GetDouble(scriptValues));
	}

	[Fact]
	public void GetStringsDefaultsToEmptyList() {
		var reader = new BufferedReader(string.Empty);
		Assert.Empty(reader.GetStrings());
	}

	[Fact]
	public void GetStringsAddsStrings() {
		var reader = new BufferedReader("foo bar baz");
		Assert.Equal(["foo", "bar", "baz"], reader.GetStrings());
	}

	[Fact]
	public void GetStringsAddsQuotedStrings() {
		var reader = new BufferedReader("\"foo\" \"bar\" \"baz\"");
		Assert.Equal(["foo", "bar", "baz"], reader.GetStrings());
	}

	[Fact]
	public void GetStringsAddsStringsFromBracedBlock() {
		var reader = new BufferedReader(" = { foo bar baz } qux");
		Assert.Equal(["foo", "bar", "baz"], reader.GetStrings());
	}

	[Fact]
	public void GetStringsAddsSingleCharacterStrings() {
		var reader = new BufferedReader("a b c x y z");
		Assert.Equal(["a", "b", "c", "x", "y", "z"], reader.GetStrings());
	}

	[Fact]
	public void GetStringGetsSingleCharacterString() {
		var reader = new BufferedReader(" = f");
		Assert.Equal("f", reader.GetString());
	}

	[Fact]
	public void GetStringGetsStringAfterEquals() {
		var reader = new BufferedReader(" = foo");
		Assert.Equal("foo", reader.GetString());
	}

	[Fact]
	public void GetStringGetsQuotedStringAfterEquals() {
		var reader = new BufferedReader(" = \"foo\"");
		Assert.Equal("foo", reader.GetString());
	}

	[Fact]
	public void GetStringGetsQuotedCurly() {
		string input = " = \"{\" ";
		var reader = new BufferedReader(input);
		string theString = reader.GetString();
		Assert.Equal("{", theString);

		input = " = \"}\" ";
		reader = new BufferedReader(input);
		theString = reader.GetString();
		Assert.Equal("}", theString);
	}

	[Fact]
	public void GetStringLogsErrorOnTokenNotFound() {
		var output = new StringWriter();
		Console.SetOut(output);
		var reader = new BufferedReader(" =");
		_ = reader.GetString();
		Assert.Contains("[ERROR] GetString: next token not found!", output.ToString());
	}

	[Fact]
	public void GetCharGetsFirstCharAfterEquals() {
		var reader = new BufferedReader(" = foo");
		Assert.Equal('f', reader.GetChar());
	}

	[Fact]
	public void GetCharGetsFirstCharFromQuotedStringAfterEquals() {
		var reader = new BufferedReader(" = \"foo\"");
		Assert.Equal('f', reader.GetChar());
	}

	[Fact]
	public void GetCharLogsErrorOnTokenNotFound() {
		var output = new StringWriter();
		Console.SetOut(output);
		var reader = new BufferedReader(" =");
		char c = reader.GetChar();
		Assert.Contains("[ERROR] GetChar: next token not found!", output.ToString());
		Assert.Equal('\0', c);
	}

	[Fact]
	public void AssignmentItemsWithinBracesToKeyValuePairs() {
		var reader = new BufferedReader(
			" = {\n" +
			"\tid = 180\n" +
			"\ttype = 46\n" +
			"\ttype = 48\n" + // duplicates are allowed
			"}"
		);
		var assignments = reader.GetAssignments();

		var expectedAssignments = new List<KeyValuePair<string, string>> {
			new("id", "180"), new("type", "46"), new("type", "48")
		};
		Assert.Equal(expectedAssignments, assignments);
	}

	[Fact]
	public void ExceptionIsThrownOnNullValueAssignment() {
		var reader = new BufferedReader(
			"\tid = 180\n" +
			"\ttype ="
		);
		var e = Assert.Throws<FormatException>(() => reader.GetAssignments());
		Assert.StartsWith("System.FormatException: Cannot assign null to type!", e.ToString());
	}

	private class WrapperClass : Parser {
		private class TestClass : Parser {
			public TestClass(BufferedReader reader) {
				RegisterKeyword("test", reader => test = reader.GetString().Equals("yes"));
				ParseStream(reader);
			}

			public bool test = false;
		}

		public WrapperClass(BufferedReader reader) {
			RegisterRegex("[a-z]", (reader, theKey) => {
				var newTest = new TestClass(reader);
				theMap[theKey] = newTest.test;
			});
			ParseStream(reader);
		}

		public readonly Dictionary<string, bool> theMap = [];
	}

	[Fact]
	public void ParseStreamSkipsMissingKeyInBraces() {
		var output = new StringWriter();
		Console.SetOut(output);

		var reader = new BufferedReader(@"a = { test = yes }\n
                b = { = yes }\n
                c = { test = yes }");
		var wrapper = new WrapperClass(reader);
		Assert.True(wrapper.theMap["a"]);
		Assert.False(wrapper.theMap["b"]);
		Assert.True(wrapper.theMap["c"]);
	}

	[Fact]
	public void IgnoreItemIgnoresSimpleColorWithColorSpace() {
		var reader1 = new BufferedReader("rgb {6 7 15} More text");
		var reader2 = new BufferedReader("hsv {0.1 1.0 0.6} More text");
		ParserHelpers.IgnoreItem(reader1);
		ParserHelpers.IgnoreItem(reader2);

		Assert.Equal(" More text", reader1.ReadToEnd());
		Assert.Equal(" More text", reader2.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresAssignedColorWithColorSpace() {
		var reader1 = new BufferedReader("= rgb {6 7 15} More text");
		var reader2 = new BufferedReader("= hsv {0.1 1.0 0.6} More text");
		ParserHelpers.IgnoreItem(reader1);
		ParserHelpers.IgnoreItem(reader2);

		Assert.Equal(" More text", reader1.ReadToEnd());
		Assert.Equal(" More text", reader2.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresRgbAndHsvStringsWithoutBreakingParsing() {
		var reader1 = new BufferedReader("= rgb next_parameter = 69 More text");
		var reader2 = new BufferedReader("= hsv next_parameter = 420 More text");
		ParserHelpers.IgnoreItem(reader1);
		ParserHelpers.IgnoreItem(reader2);

		Assert.Equal("next_parameter = 69 More text", reader1.ReadToEnd());
		Assert.Equal("next_parameter = 420 More text", reader2.ReadToEnd());
	}

	[Fact]
	public void IgnoreItemIgnoresQuotedRgbAndHsvStringsWithoutBreakingParsing() {
		var reader1 = new BufferedReader("= \"rgb\" next_parameter = 69 More text");
		var reader2 = new BufferedReader("= \"hsv\" next_parameter = 420 More text");
		ParserHelpers.IgnoreItem(reader1);
		ParserHelpers.IgnoreItem(reader2);

		Assert.Equal(" next_parameter = 69 More text", reader1.ReadToEnd());
		Assert.Equal(" next_parameter = 420 More text", reader2.ReadToEnd());
	}

	[Fact]
	public void GetLongGetsLongAfterEquals() {
		var reader = new BufferedReader(" = 123456789012345");

		Assert.Equal(123456789012345, reader.GetLong());
	}

	[Fact]
	public void GetLongGetsNegativeLongAfterEquals() {
		var reader = new BufferedReader(" = -123456789012345");

		Assert.Equal(-123456789012345, reader.GetLong());
	}

	[Fact]
	public void GetULongGetsULongAfterEquals() {
		var reader = new BufferedReader(" = 299792458000000000");

		Assert.Equal((ulong)299792458000000000, reader.GetULong());
	}

	[Fact]
	public void GetLongGetsQuotedLongAfterEquals() {
		var reader = new BufferedReader(@"= ""123456789012345""");

		Assert.Equal(123456789012345, reader.GetLong());
	}

	[Fact]
	public void GetLongGetsQuotedNegativeLongAfterEquals() {
		var reader = new BufferedReader(@"= ""-123456789012345""");

		Assert.Equal(-123456789012345, reader.GetLong());
	}

	[Fact]
	public void GetULongGetsQuotedUlongAfterEquals() {
		var reader = new BufferedReader(@"= ""123456789012345""");

		Assert.Equal((ulong)123456789012345, reader.GetULong());
	}

	[Fact]
	public void GetLongLogsInvalidInput() {
		var reader = new BufferedReader("= foo");

		var output = new StringWriter();
		Console.SetOut(output);
		var theLong = reader.GetLong();

		Assert.Contains("Could not convert string foo to long!", output.ToString());
		Assert.Equal(0, theLong);
	}

	[Fact]
	public void GetULongLogsInvalidInput() {
		var reader = new BufferedReader("= foo");

		var output = new StringWriter();
		Console.SetOut(output);

		var theULong = reader.GetULong();

		Assert.Contains("Could not convert string foo to ulong!", output.ToString());
		Assert.Equal((ulong)0, theULong);
	}

	[Fact]
	public void GetLongsDefaultsToEmpty() {
		var reader = new BufferedReader("");
		Assert.Empty(reader.GetLongs());
	}

	[Fact]
	public void GetULongsDefaultsToEmpty() {
		var reader = new BufferedReader("");
		Assert.Empty(reader.GetULongs());
	}

	[Fact]
	public void GetLongsAddsLongs() {
		var reader = new BufferedReader("123456789012345 234567890123456 345678901234567");

		var expectedLongs = new List<long> {123456789012345, 234567890123456, 345678901234567};
		Assert.Equal(expectedLongs, reader.GetLongs());
	}

	[Fact]
	public void GetLongsAddsNegativeLongs() {
		var reader = new BufferedReader("-123456789012345 -234567890123456 -345678901234567");

		var expectedLongs = new List<long> {-123456789012345, -234567890123456, -345678901234567};
		Assert.Equal(expectedLongs, reader.GetLongs());
	}

	[Fact]
	public void GetULongsAddsLongs() {
		var reader = new BufferedReader("299792458000000000 299792458000000304 256792458000000304");

		var expectedULongs = new List<ulong> {299792458000000000, 299792458000000304, 256792458000000304};
		Assert.Equal(expectedULongs, reader.GetULongs());
	}

	[Fact]
	public void GetLongsAddsQuotedLongs() {
		var reader = new BufferedReader(@"""123456789012345"" ""234567890123456"" ""345678901234567""");

		var expectedLongs = new List<long> {123456789012345, 234567890123456, 345678901234567};
		Assert.Equal(expectedLongs, reader.GetLongs());
	}

	[Fact]
	public void GetLongsAddsQuotedNegativeLongs() {
		var reader = new BufferedReader(@"""-123456789012345"" ""-234567890123456"" ""-345678901234567""");

		var expectedLongs = new List<long> {-123456789012345, -234567890123456, -345678901234567};
		Assert.Equal(expectedLongs, reader.GetLongs());
	}

	[Fact]
	public void GetULongsAddsQuotedLongs() {
		var reader = new BufferedReader(@"""299792458000000000"" ""299792458000000304"" ""256792458000000304""");

		var expectedULongs = new List<ulong> {299792458000000000, 299792458000000304, 256792458000000304};
		Assert.Equal(expectedULongs, reader.GetULongs());
	}

	[Fact]
	public void GetLongsAddsLongsFromBracedBlock() {
		var reader = new BufferedReader(" = {123456789012345 234567890123456 345678901234567} 456789012345678");

		var expectedLongs = new List<long> {123456789012345, 234567890123456, 345678901234567};
		Assert.Equal(expectedLongs, reader.GetLongs());
	}

	[Fact]
	public void GetULongsAddsULongsFromBracedBlock() {
		var reader =
			new BufferedReader(" = {299792458000000000 299792458000000304 256792458000000304} 256796558000000304");

		var expectedULongs = new List<ulong> {299792458000000000, 299792458000000304, 256792458000000304};
		Assert.Equal(expectedULongs, reader.GetULongs());
	}

	[Theory]
	[InlineData("= yes", true)]
	[InlineData("= no", false)]
	public void GetBoolReturnsCorrectValue(string textToParse, bool expectedValue) {
		var reader = new BufferedReader(textToParse);
		Assert.Equal(expectedValue, reader.GetBool());
	}

	[Fact]
	public void GetBoolThrowsExceptionWhenValueFormatIsInvalid() {
		var reader = new BufferedReader("= perhaps");
		var e = Assert.Throws<FormatException>(() => reader.GetBool());
		Assert.Contains("Text representation of bool should be \"yes\" or \"no\", not \"perhaps\"!", e.ToString());
	}

	private class TypeClass : Parser {
		public string? str;
		public int integer;
		public long longInt;
		public ulong ulongInt;
		public double d;
		public List<string> strings = [];
		public List<int> ints = [];
		public List<long> longs = [];
		public List<ulong> ulongs = [];
		public List<double> doubles = [];
		public List<KeyValuePair<string, string>> assignments = [];

		public TypeClass(BufferedReader reader) {
			RegisterKeyword("str", reader => str = reader.GetString());
			RegisterKeyword("integer", reader => integer = reader.GetInt());
			RegisterKeyword("longInt", reader => longInt = reader.GetLong());
			RegisterKeyword("ulongInt", reader => ulongInt = reader.GetULong());
			RegisterKeyword("d", reader => d = reader.GetDouble());
			RegisterKeyword("strings", reader => strings = reader.GetStrings());
			RegisterKeyword("ints", reader => ints = reader.GetInts());
			RegisterKeyword("longs", reader => longs = reader.GetLongs());
			RegisterKeyword("ulongs", reader => ulongs = reader.GetULongs());
			RegisterKeyword("doubles", reader => doubles = reader.GetDoubles());
			RegisterKeyword("assignments", reader => assignments = reader.GetAssignments());

			ParseStream(reader);
		}
	}

	[Fact]
	public void CommonTypesWorkWithVariablesAndExpressions() {
		var reader = new BufferedReader(
			// variable definitions
			"@str_var = \"peep\"\n" +
			"@positive_var = 43\n" +
			"@negative_var = -3\n" +
			"@double_var = 0.35\n" +
			"\n" +
			// expression usage
			"str = @str_var\n" +
			"integer = @[negative_var+2]\n" +
			"longInt = @[negative_var + 2]\n" +
			"ulongInt = @[positive_var - 1]\n" +
			"d = @[double_var * 2]\n" +
			"strings = { beep @str_var }\n" +
			"ints = { 5 @[negative_var + 2] }\n" +
			"longs = { 5 @[negative_var + 2] }\n" +
			"ulongs = { 5 @[positive_var - 1] }\n" +
			"doubles = { 5 @[double_var * 2] }\n" +
			"assignments = { beep = @str_var }"
		);
		var instance = new TypeClass(reader);
		Assert.Equal("peep", instance.str);
		Assert.Equal(-1, instance.integer);
		Assert.Equal(-1, instance.longInt);
		Assert.Equal((ulong)42, instance.ulongInt);
		Assert.Equal(0.7d, instance.d, 8);
		Assert.Collection(instance.strings,
			item => Assert.Equal("beep", item),
			item => Assert.Equal("peep", item));
		Assert.Collection(instance.ints,
			item => Assert.Equal(5, item),
			item => Assert.Equal(-1, item));
		Assert.Collection(instance.longs,
			item => Assert.Equal(5, item),
			item => Assert.Equal(-1, item));
		Assert.Collection(instance.ulongs,
			item => Assert.Equal((ulong)5, item),
			item => Assert.Equal((ulong)42, item));
		Assert.Collection(instance.doubles,
			item => Assert.Equal(5, item),
			item => Assert.Equal(0.7, item, 8));
		Assert.Collection(instance.assignments,
			item => {
				var (key, value) = item;
				Assert.Equal("beep", key);
				Assert.Equal("peep", value);
			}
		);
	}
}