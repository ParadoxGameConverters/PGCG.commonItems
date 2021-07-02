using System;
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
    }
}
