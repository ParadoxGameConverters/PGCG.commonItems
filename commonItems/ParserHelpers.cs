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
            Logger.Debug("Ignoring keyword: " + keyword);
        }

        public static string GetString(BufferedReader reader) {
            return new SingleString(reader).String;
        }
        public static int GetInt(BufferedReader reader) {
            return new SingleInt(reader).Int;
        }
        public static long GetLong(BufferedReader reader) {
            return new SingleLong(reader).Long;
        }
        public static ulong GetULong(BufferedReader reader) {
            return new SingleULong(reader).ULong;
        }
        public static double GetDouble(BufferedReader reader) {
            return new SingleDouble(reader).Double;
        }
        public static List<string> GetStrings(BufferedReader reader) {
            return new StringList(reader).Strings;
        }
        public static List<int> GetInts(BufferedReader reader) {
            return new IntList(reader).Ints;
        }
        public static List<long> GetLongs(BufferedReader reader) {
            return new LongList(reader).Longs;
        }
        public static List<ulong> GetULongs(BufferedReader reader) {
            return new ULongList(reader).ULongs;
        }
        public static List<double> GetDoubles(BufferedReader reader) {
            return new DoubleList(reader).Doubles;
        }
    }
    public class SingleString : Parser {
        public SingleString(BufferedReader sr) {
            // remove equals
            GetNextTokenWithoutMatching(sr);

            var token = GetNextTokenWithoutMatching(sr);
            if (token is null) {
                Logger.Error("SingleString: next token not found!");
            } else {
                String = StringUtils.RemQuotes(token);
            }
        }
        public string String { get; } = "";
    }

    public class SingleInt {
        public SingleInt(BufferedReader sr) {
            var intStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr));
            if (!int.TryParse(intStr, out int theInt)) {
                Logger.Warn($"Could not convert string {intStr} to int!");
                return;
            }
            Int = theInt;
        }
        public int Int { get; }
    }

    public class SingleLong {
        public SingleLong(BufferedReader sr) {
            var longStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr));
            if (!long.TryParse(longStr, out long theLong)) {
                Logger.Warn($"Could not convert string {longStr} to long!");
                return;
            }
            Long = theLong;
        }
        public long Long { get; }
    }

    public class SingleULong {
        public SingleULong(BufferedReader sr) {
            var ulongStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr));
            if (!ulong.TryParse(ulongStr, out ulong theULong)) {
                Logger.Warn($"Could not convert string {ulongStr} to ulong!");
                return;
            }
            ULong = theULong;
        }
        public ulong ULong { get; }
    }

    public class SingleDouble {
        public SingleDouble(BufferedReader sr) {
            var doubleStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr));
            if (!double.TryParse(doubleStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double theDouble)) {
                Logger.Warn($"Could not convert string {doubleStr} to double!");
                return;
            }
            Double = theDouble;
        }
        public double Double { get; }
    }

    public class StringList : Parser {
        public StringList(BufferedReader sr) {
            RegisterKeyword("\"\"", _ => { });
            RegisterRegex(CommonRegexes.String, (_, theString) =>
                Strings.Add(theString)
            );
            RegisterRegex(CommonRegexes.QuotedString, (_, theString) =>
                Strings.Add(StringUtils.RemQuotes(theString))
            );
            ParseStream(sr);
        }
        public List<string> Strings { get; } = new();
    }

    public class IntList : Parser {
        public IntList(BufferedReader sr) {
            RegisterRegex(CommonRegexes.Integer, (_, intString) => Ints.Add(int.Parse(intString)));
            RegisterRegex(CommonRegexes.QuotedInteger, (_, intString) => {
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
            RegisterRegex(CommonRegexes.Integer, (_, longString) => Longs.Add(long.Parse(longString)));
            RegisterRegex(CommonRegexes.QuotedInteger, (_, longString) => {
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
            RegisterRegex(CommonRegexes.Integer, (_, ulongString) => ULongs.Add(ulong.Parse(ulongString)));
            RegisterRegex(CommonRegexes.QuotedInteger, (_, ulongString) => {
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
            RegisterRegex(CommonRegexes.Float, (_, floatString) =>
                Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture))
            );
            RegisterRegex(CommonRegexes.QuotedFloat, (_, floatString) => {
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

    public class BlobList : Parser {
        public List<string> Blobs { get; } = new();
        public BlobList(BufferedReader reader) {
            var next = GetNextLexeme(reader);
            if (next == "=") {
                next = GetNextLexeme(reader);
            }
            while (next == "{") {
                var braceDepth = 0;
                var sb = new StringBuilder();
                while (true) {
                    if (reader.EndOfStream) {
                        return;
                    }
                    char inputChar = (char)reader.Read();
                    if (inputChar == '{') {
                        if (braceDepth > 0) {
                            sb.Append(inputChar);
                        }
                        braceDepth++;
                    } else if (inputChar == '}') {
                        braceDepth--;
                        if (braceDepth > 0) {
                            sb.Append(inputChar);
                        } else if (braceDepth == 0) {
                            Blobs.Add(sb.ToString());
                            sb.Clear();
                        } else if (braceDepth == -1) {
                            return;
                        }
                    } else if (braceDepth == 0) {
                        // Ignore this character. Only look for blobs.
                    } else {
                        sb.Append(inputChar);
                    }
                }
            }
        }
    }
}
