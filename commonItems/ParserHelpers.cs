using System.IO;
using System.Numerics;
using System.Collections.Generic;
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
            if (token == null) {
                Logger.Log(LogLevel.Error, "SingleString: next token not found!"); ;
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
            RegisterRegex(CommonRegexes.StringRegex, (sr, theString) => {
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
            if (next == "=") {
                String += next + ' ';
                next = GetNextLexeme(reader);
            }
            String += next;

            if (next == "{") {
                var braceDepth = 1;
                while (true) {
                    if (reader.EndOfStream) {
                        return;
                    }

                    char inputChar = (char)reader.Read();
                    String += inputChar;

                    if (inputChar == '{') {
                        ++braceDepth;
                    } else if (inputChar == '}') {
                        --braceDepth;
                        if (braceDepth == 0) {
                            return;
                        }
                    }
                }
            }
        }
        public string String { get; }
    }
}
