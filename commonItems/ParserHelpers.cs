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
            Log.WriteLine(LogLevel.Debug, "Ignoring keyword: " + keyword);
        }
    }
    public class SingleString : Parser {
        public SingleString(BufferedReader sr) {
            GetNextTokenWithoutMatching(sr); // remove equals
            var token = GetNextTokenWithoutMatching(sr);
            if (token == null) {
                Log.WriteLine(LogLevel.Error, "SingleString: next token not found!"); ;
            } else {
                String = RemQuotes(token);
            }
        }
        public string String { get; } = "";
    }

    public class SingleInt {
        public SingleInt(BufferedReader sr) {
            var intString = Parser.RemQuotes(new SingleString(sr).String);
            if (!int.TryParse(intString, out int theInt)) {
                Log.WriteLine(LogLevel.Warning, "Could not convert string " + intString + " to int!");
                return;
            }
            Int = theInt;
        }
        public int Int { get; }
    }

    public class SingleDouble {
        public SingleDouble(BufferedReader sr) {
            var doubleString = Parser.RemQuotes(new SingleString(sr).String);
            if (!double.TryParse(doubleString, NumberStyles.Any, CultureInfo.InvariantCulture, out double theDouble)) {
                Log.WriteLine(LogLevel.Warning, "Could not convert string " + doubleString + " to double!");
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
                Strings.Add(RemQuotes(theString) ?? string.Empty);
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
                intString = intString[1..^1]; // remove quotes
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
                floatString = floatString[1..^1]; // remove quotes
                Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
            });
            ParseStream(sr);
        }
        public List<double> Doubles { get; } = new();
    }
}
