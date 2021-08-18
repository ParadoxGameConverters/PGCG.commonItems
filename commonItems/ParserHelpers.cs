﻿using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace commonItems {
    public static class ParserHelpers {
        public static void IgnoreItem(BufferedReader sr) {
            var next = Parser.GetNextLexeme(sr);
            if (next == "=") {
                next = Parser.GetNextLexeme(sr);
            }
            if (next is "rgb" or "hsv") // Needed for ignoring color. Example: "color = rgb { 2 4 8 }"
            {
                if ((char)sr.Peek() == '{') {
                    next = Parser.GetNextLexeme(sr);
                } else { // don't go further in cases like "type = rgb"
                    return;
                }
            }
            if (next == "{") {
                var braceDepth = 1;
                while (true) {
                    if (sr.EndOfStream) {
                        return;
                    }
                    var token = Parser.GetNextLexeme(sr);
                    switch (token) {
                        case "{":
                            ++braceDepth;
                            break;
                        case "}": {
                            --braceDepth;
                            if (braceDepth == 0) {
                                return;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
            IgnoreItem(sr);
            Logger.Log(LogLevel.Debug, "Ignoring keyword: " + keyword);
        }
    }
    public class SingleString : Parser {
        public SingleString(BufferedReader sr) {
            // remove equals
            GetNextTokenWithoutMatching(sr);

            var token = GetNextTokenWithoutMatching(sr);
            if (token is null) {
                Logger.Log(LogLevel.Error, "SingleString: next token not found!");
            } else {
                String = StringUtils.RemQuotes(token);
            }
        }
        public string String { get; } = "";
    }

    public class SingleInt {
        public SingleInt(BufferedReader sr) {
            var intString = StringUtils.RemQuotes(new SingleString(sr).String);
            if (!int.TryParse(intString, out int theInt)) {
                Logger.Log(LogLevel.Warning, "Could not convert string " + intString + " to int!");
                return;
            }
            Int = theInt;
        }
        public int Int { get; }
    }

    public class SingleLong {
        public SingleLong(BufferedReader sr) {
            var longString = StringUtils.RemQuotes(new SingleString(sr).String);
            if (!long.TryParse(longString, out long theLong)) {
                Logger.Log(LogLevel.Warning, "Could not convert string " + longString + " to long!");
                return;
            }
            Long = theLong;
        }
        public long Long { get; }
    }

    public class SingleULong {
        public SingleULong(BufferedReader sr) {
            var ulongString = StringUtils.RemQuotes(new SingleString(sr).String);
            if (!ulong.TryParse(ulongString, out ulong theULong)) {
                Logger.Log(LogLevel.Warning, "Could not convert string " + ulongString + " to ulong!");
                return;
            }
            ULong = theULong;
        }
        public ulong ULong { get; }
    }

    public class SingleDouble {
        public SingleDouble(BufferedReader sr) {
            var doubleString = StringUtils.RemQuotes(new SingleString(sr).String);
            if (!double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture, out double theDouble)) {
                Logger.Log(LogLevel.Warning, "Could not convert string " + doubleString + " to double!");
                return;
            }
            Double = theDouble;
        }
        public double Double { get; }
    }

    public class StringList : Parser {
        public StringList(BufferedReader sr) {
            RegisterKeyword("\"\"", sr => { });
            RegisterRegex(CommonRegexes.String, (sr, theString) => {
                Strings.Add(theString);
            });
            RegisterRegex(CommonRegexes.QuotedString, (sr, theString) => {
                Strings.Add(StringUtils.RemQuotes(theString));
            });
            ParseStream(sr);
        }
        public List<string> Strings { get; } = new();
    }

    public class IntList : Parser {
        public IntList(BufferedReader sr) {
            RegisterRegex(CommonRegexes.Integer, (sr, intString) => {
                Ints.Add(int.Parse(intString));
            });
            RegisterRegex(CommonRegexes.QuotedInteger, (sr, intString) => {
                // remove quotes
                intString = intString[1..^1];
                Ints.Add(int.Parse(intString));
            });
            ParseStream(sr);
        }
        public List<int> Ints { get; } = new();
    }

    public class LongList : Parser {
        public LongList(BufferedReader sr) {
            RegisterRegex(CommonRegexes.Integer, (sr, longString) => {
                Longs.Add(long.Parse(longString));
            });
            RegisterRegex(CommonRegexes.QuotedInteger, (sr, longString) => {
                // remove quotes
                longString = longString[1..^1];
                Longs.Add(long.Parse(longString));
            });
            ParseStream(sr);
        }
        public List<long> Longs { get; } = new();
    }

    public class ULongList : Parser {
        public ULongList(BufferedReader sr) {
            RegisterRegex(CommonRegexes.Integer, (sr, ulongString) => {
                ULongs.Add(ulong.Parse(ulongString));
            });
            RegisterRegex(CommonRegexes.QuotedInteger, (sr, ulongString) => {
                // remove quotes
                ulongString = ulongString[1..^1];
                ULongs.Add(ulong.Parse(ulongString));
            });
            ParseStream(sr);
        }
        public List<ulong> ULongs { get; } = new();
    }

    public class DoubleList : Parser {
        public DoubleList(BufferedReader sr) {
            RegisterRegex(CommonRegexes.Float, (sr, floatString) => {
                Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
            });
            RegisterRegex(CommonRegexes.QuotedFloat, (sr, floatString) => {
                // remove quotes
                floatString = floatString[1..^1];
                Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
            });
            ParseStream(sr);
        }
        public List<double> Doubles { get; } = new();
    }

    public class StringOfItem : Parser {
        public StringOfItem(BufferedReader reader) {
            var next = GetNextLexeme(reader);
            var sb = new StringBuilder();
            if (next == "=") {
                sb.Append(next);
                sb.Append(' ');
                next = GetNextLexeme(reader);
            }
            sb.Append(next);

            if (next == "{") {
                var braceDepth = 1;
                while (true) {
                    if (reader.EndOfStream) {
                        String = sb.ToString();
                        return;
                    }

                    char inputChar = (char)reader.Read();
                    sb.Append(inputChar);

                    if (inputChar == '{') {
                        ++braceDepth;
                    } else if (inputChar == '}') {
                        --braceDepth;
                        if (braceDepth == 0) {
                            String = sb.ToString();
                            return;
                        }
                    }
                }
            }
            String = sb.ToString();
        }
        public string String { get; }
    }
}
