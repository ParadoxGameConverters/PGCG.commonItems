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
		public void SingleIntGetsIntAfterEquals() {
			var input = new BufferedReader(" = 1");
			var theInteger = new SingleInt(input);
			Assert.Equal(1, theInteger.Int);
		}

		[Fact]
		public void SingleIntGetsNegativeIntAfterEquals() {
			var input = new BufferedReader(" = -1");
			var theInteger = new SingleInt(input);
			Assert.Equal(-1, theInteger.Int);
		}

		[Fact]
		public void SingleIntGetsQuotedIntAfterEquals() {
			var input = new BufferedReader(" = \"-1\"");
			var theInteger = new SingleInt(input);
			Assert.Equal(-1, theInteger.Int);
		}

		[Fact]
		public void SingleIntLogsInvalidInput() {
			var output = new StringWriter();
			Console.SetOut(output);

			var input = new BufferedReader("= foo");
			var theInteger = new SingleInt(input);
			Assert.Equal("[WARN] Could not convert string foo to int!", output.ToString().TrimEnd());
			Assert.Equal(0, theInteger.Int);
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
		public void SingleDoubleGetsDoubleAfterEquals() {
			var input = new BufferedReader(" = 1.25");
			var theDouble = new SingleDouble(input);
			Assert.Equal(1.25, theDouble.Double);
		}

		[Fact]
		public void SingleDoubleGetsQuotedDoubleAfterEquals() {
			var input = new BufferedReader(" = \"1.25\"");
			var theDouble = new SingleDouble(input);
			Assert.Equal(1.25, theDouble.Double);
		}

		[Fact]
		public void SingleDoubleLogsNotMatchingInput() {
			var output = new StringWriter();
			Console.SetOut(output);

			var input = new BufferedReader("= \"345.345 foo\"");
			var theDouble = new SingleDouble(input);
			Assert.Equal("[WARN] Could not convert string 345.345 foo to double!", output.ToString().TrimEnd());
			Assert.Equal(0, theDouble.Double);
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
			var assignments = ParserHelpers.GetAssignments(reader);

			var expectedAssignments = new Dictionary<string, string> { { "id", "180" }, { "type", "46" } };
			Assert.Equal(expectedAssignments, assignments);
		}

		[Fact]
		public void ExceptionIsThrownOnNullValueAssignment() {
			var reader = new BufferedReader(
				"\tid = 180\n" +
				"\ttype ="
			);
			var e = Assert.Throws<FormatException>(() => ParserHelpers.GetAssignments(reader));
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

			var theItem = new StringOfItem(reader);
			Assert.Equal(input, theItem.String);
		}
		[Fact]
		public void StringOfItemGetsStringAfterEquals() {
			var reader = new BufferedReader(" = foo");
			var theItem = new StringOfItem(reader);
			Assert.Equal("= foo", theItem.String);
		}

		[Fact]
		public void SingleLongGetsLongAfterEquals() {
			var reader = new BufferedReader(" = 123456789012345");
			var theLong = new SingleLong(reader).Long;

			Assert.Equal(123456789012345, theLong);
		}

		[Fact]
		public void SingleLongGetsNegativeLongAfterEquals() {
			var reader = new BufferedReader(" = -123456789012345");
			var theLong = new SingleLong(reader).Long;

			Assert.Equal(-123456789012345, theLong);
		}

		[Fact]
		public void SingleULongGetsULongAfterEquals() {
			var reader = new BufferedReader(" = 299792458000000000");
			var theULong = new SingleULong(reader).ULong;

			Assert.Equal((ulong)299792458000000000, theULong);
		}

		[Fact]
		public void SingleLongGetsQuotedLongAfterEquals() {
			var reader = new BufferedReader(@"= ""123456789012345""");
			var theLong = new SingleLong(reader).Long;

			Assert.Equal(123456789012345, theLong);
		}

		[Fact]
		public void SingleLongGetsQuotedNegativeLongAfterEquals() {
			var reader = new BufferedReader(@"= ""-123456789012345""");
			var theLong = new SingleLong(reader).Long;

			Assert.Equal(-123456789012345, theLong);
		}

		[Fact]
		public void SingleULongGetsQuotedUlongAfterEquals() {
			var reader = new BufferedReader(@"= ""123456789012345""");
			var theULong = new SingleULong(reader).ULong;

			Assert.Equal((ulong)123456789012345, theULong);
		}

		[Fact]
		public void SingleLongLogsInvalidInput() {
			var reader = new BufferedReader("= foo");

			var output = new StringWriter();
			Console.SetOut(output);
			var theLong = new SingleLong(reader).Long;

			Assert.Contains("Could not convert string foo to long!", output.ToString());
			Assert.Equal(0, theLong);
		}

		[Fact]
		public void SingleULongLogsInvalidInput() {
			var reader = new BufferedReader("= foo");

			var output = new StringWriter();
			Console.SetOut(output);

			var theULong = new SingleULong(reader).ULong;

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
	}
}
