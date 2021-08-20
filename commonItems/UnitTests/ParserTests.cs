using System;
using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class ParserTests {
        [Fact]
        public void AbsorbBOMAbsorbsBOM() {
            var stream = new BufferedReader("\xEF\xBB\xBFMore text");
            Parser.AbsorbBOM(stream);
            Assert.Equal("More text", stream.ReadToEnd());
        }

        [Fact]
        public void AbsorbBOMDoesNotAbsorbNonBOM() {
            var stream = new BufferedReader("More text");
            Parser.AbsorbBOM(stream);
            Assert.Equal("More text", stream.ReadToEnd());
        }

        private class Test : Parser {
            public string? key;
            public string? value;
            public Test(BufferedReader bufferedReader) {
                RegisterKeyword("key", (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(bufferedReader);
            }
        };

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

        private class Test2 : Parser {
            public string key = "";
            public string? value;
            public Test2(BufferedReader bufferedReader) {
                RegisterKeyword("\"key\"", (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(bufferedReader);
            }
        };

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

        private class Test3 : Parser {
            public string key = "";
            public string? value;
            public Test3(BufferedReader bufferedReader) {
                RegisterRegex("[key]+", (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(bufferedReader);
            }
        };

        [Fact]
        public void QuotedRegexesAreMatched() {
            var bufferedReader = new BufferedReader("\"key\" = value");
            var test = new Test3(bufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        private class Test4 : Parser {
            public string key = "";
            public string? value;
            public Test4(BufferedReader bufferedReader) {
                RegisterRegex("[k\"ey]+", (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(bufferedReader);
            }
        };

        [Fact]
        public void QuotedRegexesAreQuotedlyMatched() {
            var bufferedReader = new BufferedReader("\"key\" = value");
            var test = new Test4(bufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        private class Test5 : Parser {
            public string key = "";
            public string? value;
            public Test5(BufferedReader bufferedReader) {
                RegisterRegex(CommonRegexes.Catchall, (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                ParseStream(bufferedReader);
            }
        };

        [Fact]
        public void CatchAllCatchesQuotedKeys() {
            var bufferedReader = new BufferedReader("\"key\" = value");
            var test = new Test5(bufferedReader);
            Assert.Equal("\"key\"", test.key);
            Assert.Equal("value", test.value);
        }

        [Fact]
        public void CatchAllCatchesQuotedKeysWithWhitespaceInside() {
            var bufferedReader = new BufferedReader("\"this\tis a\nkey\n\" = value");
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

        private class Test6 : Parser {
            public uint keyCount = 0;
            public Test6(BufferedReader bufferedReader) {
                RegisterRegex(CommonRegexes.Catchall, sr => {
                    ++keyCount;
                    ParserHelpers.IgnoreItem(sr);
                });
                ParseStream(bufferedReader);
            }
        };

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
            Assert.Equal("[ERROR] Could not open missingFile.txt for parsing", output.ToString().TrimEnd());
        }

        private class FileTest : Parser {
            public string? value;
            public FileTest(string filename) {
                RegisterKeyword("key1", (sr) => {
                    value = new SingleString(sr).String;
                });
                ParseFile(filename);
            }
        }

        [Fact]
        public void ParserCanParseFiles() {
            const string filename = "UnitTests/TestFiles/keyValuePair.txt";
            var value = new FileTest(filename).value;
            Assert.Equal("value1", value);
        }

        [Fact]
        public void RegisteredRulesCanBeCleared() {
            const string filename = "UnitTests/TestFiles/keyValuePair.txt";
            var parser = new FileTest(filename);
            var value = parser.value;
            Assert.Equal("value1", value);

            parser.value = null;
            parser.ClearRegisteredRules();
            parser.ParseFile(filename);
            value = parser.value;
            Assert.Null(value);
        }

        private class Test7 : Parser {
            public string? key;
            public string? value;
            public string? broken;
            public Test7(BufferedReader bufferedReader) {
                RegisterKeyword("key", (sr, k) => {
                    key = k;
                    value = new SingleString(sr).String;
                });
                RegisterKeyword("broken", (_, k) => broken = k);
                ParseStream(bufferedReader);
            }
        };

        [Fact]
        public void FastForwardTo0DepthWorksWithOpeningBrackets() {
            var bufferedReader = new BufferedReader("= { key = value = { broken } }");
            var test = new Test7(bufferedReader);
            Assert.Equal("key", test.key);
            Assert.Equal("value", test.value);
            Assert.Null(test.broken);
        }
    }
}
