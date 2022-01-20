using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
	[Collection("Sequential")]
	[CollectionDefinition("Sequential", DisableParallelization = true)]
	public class ParserHelperTests {
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
				RegisterKeyword("key1", sr => value1 = new SingleString(sr).String);
				RegisterKeyword("key2", sr => value2 = new SingleString(sr).String);
				RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
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
			Assert.Equal("[DEBUG] Ignoring keyword: key3", output.ToString().TrimEnd());
		}

		[Fact]
		public void IntListDefaultsToEmpty() {
			var input = new BufferedReader(string.Empty);
			var theIntegers = new IntList(input);
			Assert.Empty(theIntegers.Ints);
		}

		[Fact]
		public void IntListAddsInts() {
			var input = new BufferedReader("1 2 3");
			var theIntegers = new IntList(input);
			Assert.Equal(new List<int> { 1, 2, 3 }, theIntegers.Ints);
		}

		[Fact]
		public void IntListAddsNegativeInts() {
			var input = new BufferedReader("-1 -2 -3");
			var theIntegers = new IntList(input);
			Assert.Equal(new List<int> { -1, -2, -3 }, theIntegers.Ints);
		}

		[Fact]
		public void IntListAddsQuotedInts() {
			var input = new BufferedReader("\"1\" \"2\" \"3\"");
			var theIntegers = new IntList(input);
			Assert.Equal(new List<int> { 1, 2, 3 }, theIntegers.Ints);
		}

		[Fact]
		public void IntListAddsQuotedNegativeInts() {
			var input = new BufferedReader("\"-1\" \"-2\" \"-3\"");
			var theIntegers = new IntList(input);
			Assert.Equal(new List<int> { -1, -2, -3 }, theIntegers.Ints);
		}

		[Fact]
		public void IntListAddsIntsFromBracedBlock() {
			var input = new BufferedReader(" = {1 2 3} 4");
			var theIntegers = new IntList(input);
			Assert.Equal(new List<int> { 1, 2, 3 }, theIntegers.Ints);
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
			Assert.Equal("[WARN] Could not convert string foo to int!", output.ToString().TrimEnd());
			Assert.Equal(0, theInteger);
		}

		[Fact]
		public void DoubleListDefaultsToEmpty() {
			var input = new BufferedReader(string.Empty);
			var theDoubles = new DoubleList(input);
			Assert.Empty(theDoubles.Doubles);
		}

		[Fact]
		public void DoubleListAddsDoubles() {
			var input = new BufferedReader("1.25 2.5 3.75");
			var theDoubles = new DoubleList(input);
			Assert.Equal(new List<double> { 1.25, 2.5, 3.75 }, theDoubles.Doubles);
		}

		[Fact]
		public void DoubleListAddsNegativeDoubles() {
			var input = new BufferedReader("1.25 -2.5 -3.75");
			var theDoubles = new DoubleList(input);
			Assert.Equal(new List<double> { 1.25, -2.5, -3.75 }, theDoubles.Doubles);
		}

		[Fact]
		public void DoubleListAddsQuotedDoubles() {
			var input = new BufferedReader("\"1.25\" \"2.5\" \"3.75\"");
			var theDoubles = new DoubleList(input);
			Assert.Equal(new List<double> { 1.25, 2.5, 3.75 }, theDoubles.Doubles);
		}

		[Fact]
		public void DoubleListAddsQuotedNegativeDoubles() {
			var input = new BufferedReader("\"1.25\" \"-2.5\" \"-3.75\"");
			var theDoubles = new DoubleList(input);
			Assert.Equal(new List<double> { 1.25, -2.5, -3.75 }, theDoubles.Doubles);
		}

		[Fact]
		public void DoubleListAddsDoublesFromBracedBlock() {
			var input = new BufferedReader(" = {1.25 2.5 3.75} 4.5");
			var theDoubles = new DoubleList(input);
			Assert.Equal(new List<double> { 1.25, 2.5, 3.75 }, theDoubles.Doubles);
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
			Assert.Equal("[WARN] Could not convert string 345.345 foo to double!", output.ToString().TrimEnd());
			Assert.Equal(0, theDouble);
		}

		[Fact]
		public void StringListDefaultsToEmpty() {
			var input = new BufferedReader(string.Empty);
			var theStrings = new StringList(input);
			Assert.Empty(theStrings.Strings);
		}

		[Fact]
		public void StringListAddsStrings() {
			var input = new BufferedReader("foo bar baz");
			var theStrings = new StringList(input);
			Assert.Equal(new List<string> { "foo", "bar", "baz" }, theStrings.Strings);
		}

		[Fact]
		public void StringListAddsQuotedStrings() {
			var input = new BufferedReader("\"foo\" \"bar\" \"baz\"");
			var theStrings = new StringList(input);
			Assert.Equal(new List<string> { "foo", "bar", "baz" }, theStrings.Strings);
		}

		[Fact]
		public void StringListAddsStringsFromBracedBlock() {
			var input = new BufferedReader(" = { foo bar baz } qux");
			var theStrings = new StringList(input);
			Assert.Equal(new List<string> { "foo", "bar", "baz" }, theStrings.Strings);
		}

		[Fact]
		public void SingleStringGetsStringAfterEquals() {
			var input = new BufferedReader(" = foo");
			var theString = new SingleString(input);
			Assert.Equal("foo", theString.String);
		}

		[Fact]
		public void SingleStringGetsQuotedStringAfterEquals() {
			var input = new BufferedReader(" = \"foo\"");
			var theString = new SingleString(input);
			Assert.Equal("foo", theString.String);
		}

		[Fact]
		public void SingleStringLogsErrorOnTokenNotFound() {
			var output = new StringWriter();
			Console.SetOut(output);
			var reader = new BufferedReader(" =");
			_ = new SingleString(reader).String;
			Assert.Equal("[ERROR] SingleString: next token not found!", output.ToString().TrimEnd());
		}

		[Fact]
		public void AssignmentItemsWithinBracesToKeyValuePairs() {
			var reader = new BufferedReader(
				" = {\n" +
				"\tid = 180\n" +
				"\ttype = 46\n" +
				"}"
			);
			var assignments = reader.GetAssignments();

			var expectedAssignments = new Dictionary<string, string> { { "id", "180" }, { "type", "46" } };
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
					RegisterKeyword("test", reader => {
						var testStr = new SingleString(reader);
						test = testStr.String.Equals("yes");
					});
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
			public readonly Dictionary<string, bool> theMap = new();
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
		public void BlobListDefaultsToEmpty() {
			var reader = new BufferedReader(string.Empty);
			var theBlobs = new BlobList(reader);

			Assert.Empty(theBlobs.Blobs);
		}

		[Fact]
		public void BlobListAddsBlobs() {
			var reader = new BufferedReader("= { {foo} {bar} {baz} }");
			var theBlobs = new BlobList(reader);

			var expectedBlobs = new List<string> { "foo", "bar", "baz" };
			Assert.Equal(expectedBlobs, theBlobs.Blobs);
		}

		[Fact]
		public void BlobListAddsComplicatedBlobs() {
			var reader = new BufferedReader("= { {foo=bar bar=baz} {bar=baz baz=foo} {baz=foo foo=bar} }");
			var theBlobs = new BlobList(reader);

			var expectedBlobs = new List<string> { "foo=bar bar=baz", "bar=baz baz=foo", "baz=foo foo=bar" };
			Assert.Equal(expectedBlobs, theBlobs.Blobs);
		}

		[Fact]
		public void BlobListPreservesEverythingWithinBlobs() {
			var reader = new BufferedReader(
				"= { {foo\t=\nbar\n \n{bar\t=\tbaz\n\n}} {BROKEN\t\t\tbar\n=\nbaz\n \t\tbaz\t=\nfoo\t} {\t\nbaz\n\t=\t\n\tfoo\n " +
				"{} \n\tfoo\t=\tbar\t} }"
			);

			var theBlobs = new BlobList(reader);

			var expectedBlobs = new List<string> {
				"foo\t=\nbar\n \n{bar\t=\tbaz\n\n}",
				"BROKEN\t\t\tbar\n=\nbaz\n \t\tbaz\t=\nfoo\t",
				"\t\nbaz\n\t=\t\n\tfoo\n {} \n\tfoo\t=\tbar\t"
			};
			Assert.Equal(expectedBlobs, theBlobs.Blobs);
		}

		[Fact]
		public void BlobListIgnoresEverythingOutsideBlobs() {
			var reader = new BufferedReader(
				"= {\n\n\t\t{foo}\nkey=value\n\t {bar}\t\nsome=value\t\n{baz}\t\n  randomLooseText   }"
			);

			var theBlobs = new BlobList(reader);

			var expectedBlobs = new List<string> { "foo", "bar", "baz" };
			Assert.Equal(expectedBlobs, theBlobs.Blobs);
		}

		[Fact]
		public void BlobListIsEmptyOnTrivialWrongUsage() {
			var reader = new BufferedReader("= value\n");
			var theBlobs = new BlobList(reader);

			Assert.Empty(theBlobs.Blobs);
		}

		[Fact]
		public void BlobListIsEmptyOnSimpleWrongUsage() {
			var reader = new BufferedReader("= { key=value\n key2=value2 }");
			var theBlobs = new BlobList(reader);

			Assert.Empty(theBlobs.Blobs);
		}

		[Fact]
		public void BlobListIsNotAtFaultYouAreOnComplexWrongUsage() {
			var reader = new BufferedReader("= { key=value\n key2={ key3 = value2 }}");
			var theBlobs = new BlobList(reader);

			var expectedBlobs = new List<string> { " key3 = value2 " };
			Assert.Equal(expectedBlobs, theBlobs.Blobs);
		}

		[Fact]
		public void StringOfItemConvertsBracedObjectsToStrings() {
			const string input =
				@"= {\n
                \t{\n
                \t\tid = 180\n
                \t\ttype = 46\n
                \t}\n
                }";
			var reader = new BufferedReader(input);

			var theItem = reader.GetStringOfItem();
			Assert.Equal(input[2..], theItem.ToString());
		}
		[Fact]
		public void StringOfItemGetsStringAfterEquals() {
			var reader = new BufferedReader(" = foo");
			var theItem = reader.GetStringOfItem();
			Assert.Equal("foo", theItem.ToString());
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
		public void LongListDefaultsToEmpty() {
			var reader = new BufferedReader("");
			var longs = new LongList(reader).Longs;
			Assert.Empty(longs);
		}

		[Fact]
		public void ULongListDefaultsToEmpty() {
			var reader = new BufferedReader("");
			var ulongs = new ULongList(reader).ULongs;
			Assert.Empty(ulongs);
		}

		[Fact]
		public void LongListAddsLongs() {
			var reader = new BufferedReader("123456789012345 234567890123456 345678901234567");
			var theLongs = new LongList(reader).Longs;

			var expectedLlongs = new List<long> { 123456789012345, 234567890123456, 345678901234567 };
			Assert.Equal(expectedLlongs, theLongs);
		}

		[Fact]
		public void LongListAddsNegativeLongs() {
			var reader = new BufferedReader("-123456789012345 -234567890123456 -345678901234567");
			var theLongs = new LongList(reader).Longs;

			var expectedLongs = new List<long> { -123456789012345, -234567890123456, -345678901234567 };
			Assert.Equal(expectedLongs, theLongs);
		}

		[Fact]
		public void ULongListAddsLongs() {
			var reader = new BufferedReader("299792458000000000 299792458000000304 256792458000000304");

			var ulongs = new ULongList(reader).ULongs;

			var expectedULongs =
				 new List<ulong> { 299792458000000000, 299792458000000304, 256792458000000304 };
			Assert.Equal(expectedULongs, ulongs);
		}

		[Fact]
		public void LongListAddsQuotedLongs() {
			var reader = new BufferedReader(@"""123456789012345"" ""234567890123456"" ""345678901234567""");
			var theLongs = new LongList(reader).Longs;

			var expectedLongs = new List<long> { 123456789012345, 234567890123456, 345678901234567 };
			Assert.Equal(expectedLongs, theLongs);
		}

		[Fact]
		public void LongListAddsQuotedNegativeLongs() {
			var reader = new BufferedReader(@"""-123456789012345"" ""-234567890123456"" ""-345678901234567""");
			var theLongs = new LongList(reader).Longs;

			var expectedLongs = new List<long> { -123456789012345, -234567890123456, -345678901234567 };
			Assert.Equal(expectedLongs, theLongs);
		}

		[Fact]
		public void ULongListAddsQuotedLongs() {
			var reader = new BufferedReader(@"""299792458000000000"" ""299792458000000304"" ""256792458000000304""");

			var ulongs = new ULongList(reader).ULongs;

			var expectedULongs =
				 new List<ulong> { 299792458000000000, 299792458000000304, 256792458000000304 };
			Assert.Equal(expectedULongs, ulongs);
		}

		[Fact]
		public void LongListAddsLongsFromBracedBlock() {
			var reader = new BufferedReader(" = {123456789012345 234567890123456 345678901234567} 456789012345678");
			var theLongs = new LongList(reader).Longs;

			var expectedLongs = new List<long> { 123456789012345, 234567890123456, 345678901234567 };
			Assert.Equal(expectedLongs, theLongs);
		}

		[Fact]
		public void ULongListAddsULongsFromBracedBlock() {
			var reader = new BufferedReader(" = {299792458000000000 299792458000000304 256792458000000304} 256796558000000304");

			var ulongs = new ULongList(reader).ULongs;

			var expectedULongs =
				 new List<ulong> { 299792458000000000, 299792458000000304, 256792458000000304 };
			Assert.Equal(expectedULongs, ulongs);
		}

		private class TypeClass : Parser {
			public string? str;
			public int integer;
			public long longInt;
			public ulong ulongInt;
			public double d;
			public List<string> strings = new();
			public List<int> ints = new();
			public List<long> longs = new();
			public List<ulong> ulongs = new();
			public List<double> doubles = new();
			public Dictionary<string, string> assignments = new();

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
}
