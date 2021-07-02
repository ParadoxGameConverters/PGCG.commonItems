using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class ParserHelperTests {
        [Fact]
        public void IgnoreItemIgnoresSimpleText() {
            Stream stream = Parser.GenerateStreamFromString("ignore_me More text");
            var input = new BufferedReader(stream);
            ParserHelpers.IgnoreItem(input);
            Assert.Equal("More text", input.ReadToEnd());
        }

        [Fact]
        public void IgnoreItemIgnoresAssignedText() {
            Stream stream = Parser.GenerateStreamFromString("= ignore_me More text");
            var input = new BufferedReader(stream);
            ParserHelpers.IgnoreItem(input);
            Assert.Equal("More text", input.ReadToEnd());
        }

        [Fact]
        public void IgnoreItemIgnoresBracedItem() {
            Stream stream = Parser.GenerateStreamFromString("= { { ignore_me } } More text");
            var input = new BufferedReader(stream);
            ParserHelpers.IgnoreItem(input);
            Assert.Equal(" More text", input.ReadToEnd());
        }

        [Fact]
        public void IgnoreItemIgnoresAssignedBracedItem() {
            Stream stream = Parser.GenerateStreamFromString("= { { ignore_me } } More text");
            var input = new BufferedReader(stream);
            ParserHelpers.IgnoreItem(input);
            Assert.Equal(" More text", input.ReadToEnd());
        }


        private class Test1 : Parser {
            public string? value1;
            public string? value2;
            public string? value3;
            public Test1(BufferedReader BufferedReader) {
                RegisterKeyword("key1", sr => {
                    value1 = new SingleString(sr).String;
                });
                RegisterKeyword("key2", sr => {
                    value2 = new SingleString(sr).String;
                });
                RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
                ParseStream(BufferedReader);
            }
        }

        [Fact]
        public void IgnoreAndLogItemLogsIgnoredKeyword()
        {
            var output = new StringWriter();
            Console.SetOut(output);
            
            var input = new BufferedReader("key1=val1 key2=val2 key3=mess");
            var test = new Test1(input);
            Assert.Equal("val1", test.value1);
            Assert.Equal("val2", test.value2);
            Assert.Equal("    [DEBUG]         Ignoring keyword: key3", output.ToString().TrimEnd());
        }
    }
}
