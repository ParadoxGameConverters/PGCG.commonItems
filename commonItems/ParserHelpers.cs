using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
					var lexeme = Parser.GetNextLexeme(sr);
					if (lexeme == "{") {
						++braceDepth;
					} else if (lexeme == "}") {
						--braceDepth;
						if (braceDepth == 0) {
							return;
						}
					}
				}
			}
		}

		public static void IgnoreAndLogItem(BufferedReader sr, string keyword) {
			IgnoreItem(sr);
			Logger.Debug($"Ignoring keyword: {keyword}");
		}

		public static string GetString(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new SingleString(reader, variables).String;
		}
		public static int GetInt(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new SingleInt(reader, variables).Int;
		}
		public static long GetLong(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new SingleLong(reader, variables).Long;
		}
		public static ulong GetULong(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new SingleULong(reader, variables).ULong;
		}
		public static double GetDouble(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new SingleDouble(reader, variables).Double;
		}
		public static List<string> GetStrings(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new StringList(reader, variables).Strings;
		}
		public static List<int> GetInts(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new IntList(reader, variables).Ints;
		}
		public static List<long> GetLongs(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new LongList(reader, variables).Longs;
		}
		public static List<ulong> GetULongs(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new ULongList(reader, variables).ULongs;
		}
		public static List<double> GetDoubles(BufferedReader reader, Dictionary<string, string>? variables = null) {
			return new DoubleList(reader, variables).Doubles;
		}

		public static Dictionary<string, string> GetAssignments(BufferedReader reader, Dictionary<string, string>? variables = null) {
			var assignments = new Dictionary<string, string>();
			var parser = new Parser(variables);
			parser.RegisterRegex(CommonRegexes.Catchall, (reader, assignmentName) => {
				parser.GetNextTokenWithoutMatching(reader); // remove equals
				var assignmentValue = parser.GetNextTokenWithoutMatching(reader);
				if (assignmentValue is null) {
					throw new FormatException($"Cannot assign null to {assignmentName}!");
				}
				assignments[assignmentName] = assignmentValue;
			});
			parser.ParseStream(reader);
			return assignments;
		}
	}
	public class SingleString {
		public SingleString(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			// remove equals
			parser.GetNextTokenWithoutMatching(sr);

			var token = parser.GetNextTokenWithoutMatching(sr);
			if (token is null) {
				Logger.Error("SingleString: next token not found!");
			} else {
				String = StringUtils.RemQuotes(token);
			}
		}
		public string String { get; } = "";
	}

	public class SingleInt {
		public SingleInt(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var intStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr, variables));
			if (!int.TryParse(intStr, out int theInt)) {
				Logger.Warn($"Could not convert string {intStr} to int!");
				return;
			}
			Int = theInt;
		}
		public int Int { get; }
	}

	public class SingleLong {
		public SingleLong(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var longStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr, variables));
			if (!long.TryParse(longStr, out long theLong)) {
				Logger.Warn($"Could not convert string {longStr} to long!");
				return;
			}
			Long = theLong;
		}
		public long Long { get; }
	}

	public class SingleULong {
		public SingleULong(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var ulongStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr, variables));
			if (!ulong.TryParse(ulongStr, out ulong theULong)) {
				Logger.Warn($"Could not convert string {ulongStr} to ulong!");
				return;
			}
			ULong = theULong;
		}
		public ulong ULong { get; }
	}

	public class SingleDouble {
		public SingleDouble(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var doubleStr = StringUtils.RemQuotes(ParserHelpers.GetString(sr, variables));
			if (!double.TryParse(doubleStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double theDouble)) {
				Logger.Warn($"Could not convert string {doubleStr} to double!");
				return;
			}
			Double = theDouble;
		}
		public double Double { get; }
	}

	public class StringList {
		public StringList(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			parser.RegisterKeyword("\"\"", _ => { });
			parser.RegisterRegex(CommonRegexes.String, (_, theString) =>
				Strings.Add(theString)
			);
			parser.RegisterRegex(CommonRegexes.Variable, (_, varStr) =>
				Strings.Add(parser.ResolveVariable(varStr))
			);
			parser.RegisterRegex(CommonRegexes.QuotedString, (_, theString) =>
				Strings.Add(StringUtils.RemQuotes(theString))
			);
			parser.ParseStream(sr);
		}
		public List<string> Strings { get; } = new();
	}

	public class IntList {
		public IntList(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			parser.RegisterRegex(CommonRegexes.Integer, (_, intString) => Ints.Add(int.Parse(intString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, intString) => {
				// remove quotes
				intString = intString[1..^1];
				Ints.Add(int.Parse(intString));
			});
			parser.ParseStream(sr);
		}
		public List<int> Ints { get; } = new();
	}

	public class LongList {
		public LongList(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			parser.RegisterRegex(CommonRegexes.Integer, (_, longString) => Longs.Add(long.Parse(longString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, longString) => {
				// remove quotes
				longString = longString[1..^1];
				Longs.Add(long.Parse(longString));
			});
			parser.ParseStream(sr);
		}
		public List<long> Longs { get; } = new();
	}

	public class ULongList {
		public ULongList(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			parser.RegisterRegex(CommonRegexes.Integer, (_, ulongString) => ULongs.Add(ulong.Parse(ulongString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, ulongString) => {
				// remove quotes
				ulongString = ulongString[1..^1];
				ULongs.Add(ulong.Parse(ulongString));
			});
			parser.ParseStream(sr);
		}
		public List<ulong> ULongs { get; } = new();
	}

	public class DoubleList {
		public DoubleList(BufferedReader sr, Dictionary<string, string>? variables = null) {
			var parser = new Parser(variables);
			parser.RegisterRegex(CommonRegexes.Float, (_, floatString) =>
				Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture))
			);
			parser.RegisterRegex(CommonRegexes.QuotedFloat, (_, floatString) => {
				// remove quotes
				floatString = floatString[1..^1];
				Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
			});
			parser.ParseStream(sr);
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
