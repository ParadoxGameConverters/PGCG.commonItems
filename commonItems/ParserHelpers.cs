using commonItems.Serialization;
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
	}

	public class StringList {
		public StringList(BufferedReader sr) {
			var parser = new Parser();
			parser.RegisterKeyword("\"\"", _ => { });
			parser.RegisterRegex(CommonRegexes.String, (_, theString) =>
				Strings.Add(theString)
			);
			parser.RegisterRegex(CommonRegexes.QuotedString, (_, theString) =>
				Strings.Add(StringUtils.RemQuotes(theString))
			);
			if (sr.Variables.Count > 0) {
				parser.RegisterRegex(CommonRegexes.Variable, (reader, varStr) => {
					var value = reader.ResolveVariable(varStr).ToString();
					if (value is null) {
						Logger.Warn($"StringList: variable {varStr} resolved to null value!");
					} else {
						Strings.Add(value);
					}
				});
			}

			parser.ParseStream(sr);
		}
		public List<string> Strings { get; } = new();
	}

	public class IntList {
		public IntList(BufferedReader sr) {
			var parser = new Parser();
			parser.RegisterRegex(CommonRegexes.Integer, (_, intString) => Ints.Add(int.Parse(intString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, intString) => {
				// remove quotes
				intString = intString[1..^1];
				Ints.Add(int.Parse(intString));
			});
			if (sr.Variables.Count > 0) {
				parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
					var value = (int)reader.EvaluateExpression(expr);
					Ints.Add(value);
				});
			}

			parser.ParseStream(sr);
		}
		public List<int> Ints { get; } = new();
	}

	public class LongList {
		public LongList(BufferedReader sr) {
			var parser = new Parser();
			parser.RegisterRegex(CommonRegexes.Integer, (_, longString) => Longs.Add(long.Parse(longString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, longString) => {
				// remove quotes
				longString = longString[1..^1];
				Longs.Add(long.Parse(longString));
			});
			if (sr.Variables.Count > 0) {
				parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
					var value = (long)(int)reader.EvaluateExpression(expr);
					Longs.Add(value);
				});
			}

			parser.ParseStream(sr);
		}
		public List<long> Longs { get; } = new();
	}

	public class ULongList {
		public ULongList(BufferedReader sr) {
			var parser = new Parser();
			parser.RegisterRegex(CommonRegexes.Integer, (_, ulongString) => ULongs.Add(ulong.Parse(ulongString)));
			parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, ulongString) => {
				// remove quotes
				ulongString = ulongString[1..^1];
				ULongs.Add(ulong.Parse(ulongString));
			});
			if (sr.Variables.Count > 0) {
				parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
					var value = (ulong)(int)reader.EvaluateExpression(expr);
					ULongs.Add(value);
				});
			}

			parser.ParseStream(sr);
		}
		public List<ulong> ULongs { get; } = new();
	}

	public class DoubleList {
		public DoubleList(BufferedReader sr) {
			var parser = new Parser();
			parser.RegisterRegex(CommonRegexes.Float, (_, floatString) =>
				Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture))
			);
			parser.RegisterRegex(CommonRegexes.QuotedFloat, (_, floatString) => {
				// remove quotes
				floatString = floatString[1..^1];
				Doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
			});
			if (sr.Variables.Count > 0) {
				parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
					var value = (double)reader.EvaluateExpression(expr);
					Doubles.Add(value);
				});
			}

			parser.ParseStream(sr);
		}
		public List<double> Doubles { get; } = new();
	}

	public class StringOfItem : IPDXSerializable {
		public StringOfItem(BufferedReader reader) {
			var next = Parser.GetNextLexeme(reader);
			var sb = new StringBuilder();
			if (next == "=") {
				next = Parser.GetNextLexeme(reader);
			}
			sb.Append(next);

			if (next == "{") {
				var braceDepth = 1;
				while (!reader.EndOfStream) {
					char inputChar = (char)reader.Read();
					sb.Append(inputChar);

					if (inputChar == '{') {
						++braceDepth;
					} else if (inputChar == '}') {
						--braceDepth;
						if (braceDepth == 0) {
							str = sb.ToString();
							return;
						}
					}
				}
			}
			str = sb.ToString();
		}

		public string Serialize(string indent, bool withBraces) => ToString();
		public override string ToString() => str;

		private readonly string str;
	}

	public class BlobList : Parser {
		public List<string> Blobs { get; } = new();
		public BlobList(BufferedReader reader) {
			var next = GetNextLexeme(reader);
			if (next == "=") {
				next = GetNextLexeme(reader);
			}
			if (next != "{") {
				return;
			}

			var braceDepth = 0;
			var sb = new StringBuilder();
			while (!reader.EndOfStream) {
				char inputChar = (char)reader.Read();
				if (inputChar == '{') {
					if (braceDepth > 0) {
						sb.Append(inputChar);
					}
					++braceDepth;
				} else if (inputChar == '}') {
					--braceDepth;
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
