using System;
using System.IO;
using System.Linq;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class ParserTests {
        [Fact]
        public void AbsorbBOMAbsorbsBOM() {
            Stream input = Parser.GenerateStreamFromString("\xEF\xBB\xBFMore text");
            var stream = new BufferedReader(input);
            Parser.AbsorbBOM(stream);
            Assert.Equal("More text", stream.ReadToEnd());
        }

        [Fact]
        public void AbsorbBOMDoesNotAbsorbNonBOM() {
            Stream input = Parser.GenerateStreamFromString("More text");
            var stream = new BufferedReader(input);
            Parser.AbsorbBOM(stream);
            Assert.Equal("More text", stream.ReadToEnd());
        }

        private class Test : Parser {
            public string key;
            public string value;
            public Test(BufferedReader BufferedReader) {
                RegisterKeyword("key", (BufferedReader sr, string k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(BufferedReader);
            }
        };

        [Fact]
        public void KeywordsAreMatched() {
            Stream input = Parser.GenerateStreamFromString("key = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.Equal("key", test.key);
            Assert.Equal("value", test.value);
        }

        [Fact]
        public void QuotedKeywordsAreMatched() {
            Stream input = Parser.GenerateStreamFromString("\"key\" = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        private class Test2 : Parser {
            public string key;
            public string value;
            public Test2(BufferedReader BufferedReader) {
                RegisterKeyword("\"key\"", (BufferedReader sr, string k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(BufferedReader);
            }
        };

        [Fact]
        public void QuotedKeywordsAreQuotedlyMatched() {
            Stream input = Parser.GenerateStreamFromString("\"key\" = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test2(BufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        [Fact]
        public void QuotedValuesAreParsed() {
            Stream input = Parser.GenerateStreamFromString(@"key = ""value quote""");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.Equal("key", test.key);
            Assert.Equal("value quote", test.value);
        }

        [Fact]
        public void QuotedValuesWithEscapedQuotesAreParsed() {
            Stream input = Parser.GenerateStreamFromString(@"key = ""value \""quote\"" string""");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.Equal("key", test.key);
            Assert.Equal(@"value \""quote\"" string", test.value);
        }

        [Fact]
        public void StringLiteralsAreParsed() {
            Stream input = Parser.GenerateStreamFromString(@"key = R""(value ""quote"" string)""");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.Equal("key", test.key);
            Assert.Equal(@"value ""quote"" string", test.value);
        }

        [Fact]
        public void WrongKeywordsAreIgnored() {
            Stream input = Parser.GenerateStreamFromString(@"wrongkey = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test(BufferedReader);
            Assert.True(string.IsNullOrEmpty(test.key));
            Assert.True(string.IsNullOrEmpty(test.value));
        }

        private class Test3 : Parser {
            public string key;
            public string value;
            public Test3(BufferedReader BufferedReader) {
                RegisterRegex("[key]+", (BufferedReader sr, string k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(BufferedReader);
            }
        };

        [Fact]
        public void QuotedRegexesAreMatched() {
            Stream input = Parser.GenerateStreamFromString("\"key\" = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test3(BufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        private class Test4 : Parser {
            public string key;
            public string value;
            public Test4(BufferedReader BufferedReader) {
                RegisterRegex("[k\"ey]+", (BufferedReader sr, string k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(BufferedReader);
            }
        };

        [Fact]
        public void QuotedRegexesAreQuotedlyMatched() {
            Stream input = Parser.GenerateStreamFromString("\"key\" = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test4(BufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        private class Test5 : Parser {
            public string key;
            public string value;
            public Test5(BufferedReader BufferedReader) {
                RegisterRegex(CommonRegexes.Catchall, (BufferedReader sr, string k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(BufferedReader);
            }
        };

        [Fact]
        public void CatchAllCatchesQuotedKeys() {
            Stream input = Parser.GenerateStreamFromString("\"key\" = value");
            var BufferedReader = new BufferedReader(input);
            var test = new Test5(BufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        [Fact]
        public void CatchAllCatchesQuotedKeysWithWhitespaceInside() {
            Stream input = Parser.GenerateStreamFromString("\"this\tis a\nkey\n\" = value");
            var bufferedReader = new BufferedReader(input);
            var test = new Test5(bufferedReader);
            Assert.Equal("\"this\tis a key \"", test.key);
            Assert.Equal("value", test.value);
        }

        [Fact]
        public void CatchAllCatchesQuotedKeysWithFigurativeCrapInside() {
            Stream input = Parser.GenerateStreamFromString("\"this = is a silly { key\t} \" = value");
            var bufferedReader = new BufferedReader(input);
            var test = new Test5(bufferedReader);
            Assert.Equal("\"this = is a silly { key\t} \"", test.key);
            Assert.Equal("value", test.value);
        }
    }
}
