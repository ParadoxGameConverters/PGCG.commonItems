﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
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


        private class Test1 : Parser {
            public string? value1;
            public string? value2;
            public string? value3;
            public Test1(BufferedReader bufferedReader) {
                RegisterKeyword("key1", sr => {
                    value1 = new SingleString(sr).String;
                });
                RegisterKeyword("key2", sr => {
                    value2 = new SingleString(sr).String;
                });
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
            Assert.Equal("    [DEBUG]         Ignoring keyword: key3", output.ToString().TrimEnd());
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
            Assert.Equal(new List<int>{1,2,3}, theIntegers.Ints);
        }

        [Fact]
        public void IntListAddsNegativeInts()
        {
            var input = new BufferedReader("-1 -2 -3");
            var theIntegers = new IntList(input);
            Assert.Equal(new List<int> { -1, -2, -3 }, theIntegers.Ints);
        }

        [Fact] public void IntListAddsQuotedInts() {
            var input = new BufferedReader("\"1\" \"2\" \"3\"");
            var theIntegers = new IntList(input);
            Assert.Equal(new List<int> { 1, 2, 3 }, theIntegers.Ints);
        }

        [Fact] public void IntListAddsQuotedNegativeInts() {
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
        public void SingleIntGetsIntAfterEquals()
        {
            var input = new BufferedReader(" = 1");
            var theInteger = new SingleInt(input);
            Assert.Equal(1,theInteger.Int);
        }

        [Fact] public void SingleIntGetsNegativeIntAfterEquals() {
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
        public void SingleIntLogsInvalidInput()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var input = new BufferedReader("= foo");
            var theInteger = new SingleInt(input);
            Assert.Equal("  [WARNING] Could not convert string foo to int!", output.ToString().TrimEnd());
            Assert.Equal(0, theInteger.Int);
        }

        [Fact]
        public void DoubleListDefaultsToEmpty()
        {
            var input = new BufferedReader(string.Empty);
            var theDoubles = new DoubleList(input);
            Assert.Empty(theDoubles.Doubles);
        }

        [Fact]
        public void DoubleListAddsDoubles() {
            var input = new BufferedReader("1.25 2.5 3.75");
            var theDoubles = new DoubleList(input);
            Assert.Equal(new List<double> {1.25, 2.5, 3.75}, theDoubles.Doubles);
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
        public void SingleDoubleGetsDoubleAfterEquals()
        {
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
            Assert.Equal("  [WARNING] Could not convert string 345.345 foo to double!", output.ToString().TrimEnd());
            Assert.Equal(0, theDouble.Double);
        }

        [Fact]
        public void StringListDefaultsToEmpty()
        {
            var input = new BufferedReader(string.Empty);
            var theStrings = new StringList(input);
            Assert.Empty(theStrings.Strings);
        }

        [Fact]
        public void StringListAddsStrings()
        {
            var input = new BufferedReader("foo bar baz");
            var theStrings = new StringList(input);
            Assert.Equal(new List<string>{"foo","bar","baz"},theStrings.Strings);
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

        private class TestClass : Parser
        {
            public TestClass(BufferedReader reader)
            {
                RegisterKeyword("test", reader =>
                {
                    var testStr = new SingleString(reader);
                    test = testStr.String.Equals("yes");
                });
                ParseStream(reader);
            }
            public bool test = false;
        }

        private class WrapperClass : Parser
        {
            public WrapperClass(BufferedReader reader)
            {
                RegisterRegex("[a-z]", (reader, theKey) =>
                {
                    var newTest = new TestClass(reader);
                    theMap[theKey] = newTest.test;
                });
                ParseStream(reader);
            }
            public readonly Dictionary<string, bool> theMap = new();
        }

        [Fact]
        public void ParseStreamSkipsMissingKeyInBraces()
        {
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
        public void IgnoreItemIgnoresSimpleColorWithColorSpace()
        {
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
    }
}
