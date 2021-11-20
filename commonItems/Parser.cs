﻿using NCalc;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace commonItems {
	public delegate void Del(BufferedReader sr, string keyword);
	public delegate void SimpleDel(BufferedReader sr);

	internal abstract class AbstractDelegate {
		public abstract void Execute(BufferedReader sr, string token);
	}

	internal class TwoArgDelegate : AbstractDelegate {
		private readonly Del del;
		public TwoArgDelegate(Del del) { this.del = del; }
		public override void Execute(BufferedReader sr, string token) {
			del(sr, token);
		}
	}

	internal class OneArgDelegate : AbstractDelegate {
		private readonly SimpleDel del;
		public OneArgDelegate(SimpleDel del) { this.del = del; }
		public override void Execute(BufferedReader sr, string token) {
			del(sr);
		}
	}

	public class Parser {
		private abstract class RegisteredKeywordOrRegex {
			public abstract bool Matches(string token);
		}
		private class RegisteredKeyword : RegisteredKeywordOrRegex {
			private readonly string keyword;
			public RegisteredKeyword(string keyword) {
				this.keyword = keyword;
			}
			public override bool Matches(string token) { return keyword == token; }
		}
		private class RegisteredRegex : RegisteredKeywordOrRegex {
			private readonly Regex regex;
			public RegisteredRegex(string regexString) { regex = new Regex(regexString); }
			public RegisteredRegex(Regex regex) { this.regex = regex; }
			public override bool Matches(string token) {
				var match = regex.Match(token);
				return match.Success && match.Length == token.Length;
			}
		}

		public Parser() {
			builtinRules.Add(
				new RegisteredRegex(CommonRegexes.Variable), new TwoArgDelegate((reader, varStr) => {
						var value = ParserHelpers.GetString(reader, Variables);
						if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue)) {
							Variables.Add(varStr[1..], intValue);
							return;
						}
						if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue)) {
							Variables.Add(varStr[1..], doubleValue);
							return;
						}
						Variables.Add(varStr[1..], value);
					})
			);
		}

		public Parser(Dictionary<string, object>? variables = null) : this() {
			if (variables is not null) {
				Variables = variables;
			}
		}

		public static void AbsorbBOM(BufferedReader reader) {
			var firstChar = reader.Peek();
			if (firstChar == '\xEF') {
				reader.Skip(3); // skip 3 bytes
			}
		}

		public void RegisterKeyword(string keyword, Del del) {
			registeredRules.Add(new RegisteredKeyword(keyword), new TwoArgDelegate(del));
		}
		public void RegisterKeyword(string keyword, SimpleDel del) {
			registeredRules.Add(new RegisteredKeyword(keyword), new OneArgDelegate(del));
		}
		public void RegisterRegex(string keyword, Del del) {
			registeredRules.Add(new RegisteredRegex(keyword), new TwoArgDelegate(del));
		}
		public void RegisterRegex(string keyword, SimpleDel del) {
			registeredRules.Add(new RegisteredRegex(keyword), new OneArgDelegate(del));
		}
		public void RegisterRegex(Regex regex, Del del) {
			registeredRules.Add(new RegisteredRegex(regex), new TwoArgDelegate(del));
		}
		public void RegisterRegex(Regex regex, SimpleDel del) {
			registeredRules.Add(new RegisteredRegex(regex), new OneArgDelegate(del));
		}

		public void ClearRegisteredRules() {
			registeredRules.Clear();
		}

		private bool TryToMatch(string token, string strippedToken, bool isTokenQuoted, BufferedReader reader) {
			foreach (var (rule, fun) in registeredRules) {
				if (!rule.Matches(token)) {
					continue;
				}

				fun.Execute(reader, token);
				return true;
			}
			if (isTokenQuoted) {
				foreach (var (rule, fun) in registeredRules) {
					if (!rule.Matches(strippedToken)) {
						continue;
					}

					fun.Execute(reader, token);
					return true;
				}
			}

			foreach (var (rule, fun) in builtinRules) {
				if (!rule.Matches(token)) {
					continue;
				}

				fun.Execute(reader, token);
				return true;
			}

			return false;
		}

		public static string GetNextLexeme(BufferedReader reader) {
			var sb = new StringBuilder();

			var inQuotes = false;
			var inLiteralQuote = false;
			var inInterpolatedExpression = false; // TODO: ADD CONDITIONS BELOW
			var previousCharacter = '\0';

			while (!reader.EndOfStream) {
				var inputChar = (char)reader.Read();

				if (inputChar == '\r') {
					if (inQuotes) {
						// Fix Paradox' mistake and don't break proper names in half.
						sb.Append(' ');
					} else if (sb.Length != 0) {
						break;
					}
				} else if (inputChar == '\n') {
					if (previousCharacter == '\r') {
						// We're in the middle of a Windows line ending, already handled by condition for '\r'.
					} else if (inQuotes) {
						// Fix Paradox' mistake and don't break proper names in half.
						sb.Append(' ');
					} else if (sb.Length != 0) {
						break;
					}
				} else if (inputChar == '(' && inLiteralQuote && sb.Length == 1) {
					continue;
				} else if (inputChar == '\"' && inLiteralQuote && previousCharacter == ')') {
					--sb.Length;
					sb.Append(inputChar);
					break;
				} else if (inQuotes) {
					if (inputChar == '\"' && previousCharacter != '\\') {
						sb.Append(inputChar);
						break;
					} else {
						sb.Append(inputChar);
					}
				} else { // not in quotes
					if (HandleCharOutsideQuotes(reader, sb, ref inQuotes, ref inLiteralQuote, inputChar)) {
						break;
					}
				}

				previousCharacter = inputChar;
			}
			return sb.ToString();
		}

		public object ResolveVariable(string lexeme) {
			return Variables[lexeme[1..]];
		}

		public object EvaluateExpression(string lexeme) {
			var expression = new Expression(lexeme[2..^1]);
			foreach (var (name, value) in Variables) {
				expression.Parameters[name] = value;
			}

			return expression.Evaluate();
		}

		private static bool HandleCharOutsideQuotes(BufferedReader reader, StringBuilder sb, ref bool inQuotes, ref bool inLiteralQuote, char inputChar) {
			if (inputChar == '#') {
				reader.ReadLine();
				if (sb.Length != 0) {
					return true; // break loop
				}
			} else if (inputChar == '\"' && sb.Length == 0) {
				inQuotes = true;
				sb.Append(inputChar);
			} else if (inputChar == '\"' && sb.Length == 1 && sb.ToString().Last() == 'R') {
				inLiteralQuote = true;
				--sb.Length;
				sb.Append(inputChar);
			} else if (!inLiteralQuote && char.IsWhiteSpace(inputChar)) {
				if (sb.Length != 0) {
					return true; // break loop
				}
			} else if (!inLiteralQuote && inputChar == '{') {
				if (sb.Length == 0) {
					sb.Append(inputChar);
				} else {
					reader.PushBack('{');
				}
				return true; // break loop
			} else if (!inLiteralQuote && inputChar == '}') {
				if (sb.Length == 0) {
					sb.Append(inputChar);
				} else {
					reader.PushBack('}');
				}
				return true; // break loop
			} else if (!inLiteralQuote && inputChar == '=') {
				if (sb.Length == 0) {
					sb.Append(inputChar);
				} else {
					reader.PushBack('=');
				}
				return true; // break loop
			} else {
				sb.Append(inputChar);
			}

			return false;
		}

		public string? GetNextTokenWithoutMatching(BufferedReader reader) {
			if (reader.EndOfStream) {
				return null;
			}

			var lexeme = GetNextLexeme(reader);
			if (CommonRegexes.Variable.IsMatch(lexeme)) {
				return ResolveVariable(lexeme).ToString();
			}
			if (CommonRegexes.InterpolatedExpression.IsMatch(lexeme)) {
				return EvaluateExpression(lexeme).ToString();
			}
			return lexeme;
		}

		private string? GetNextToken(BufferedReader reader) {
			string? token = null;

			var gotToken = false;
			while (!gotToken) {
				if (reader.EndOfStream) {
					return null;
				}

				token = GetNextLexeme(reader);

				var strippedToken = StringUtils.RemQuotes(token);
				var isTokenQuoted = strippedToken.Length < token.Length;

				var matched = TryToMatch(token, strippedToken, isTokenQuoted, reader);

				if (!matched) {
					gotToken = true;
				}
			}
			return token;
		}

		public void ParseStream(BufferedReader reader) {
			var braceDepth = 0;
			var value = false; // tracker to indicate whether we reached the value part of key=value pair
			var tokensSoFar = new StringBuilder();

			while (true) {
				var token = GetNextToken(reader);
				if (token != null) {
					tokensSoFar.Append(token);
					if (token == "=") {
						// swapping to value part.
						if (!value) {
							value = true;
							continue;
						}
						// leaving else to be noticeable.
						else {
							// value is positive, meaning we were at value, and now we're hitting an equal. This is bad. We need to
							// manually fast-forward to brace-lvl 0 and die.
							FastForwardTo0Depth(reader, ref braceDepth, tokensSoFar);
							Logger.Warn($"Broken token syntax at {tokensSoFar}");
							return;
						}
					} else if (token == "{") {
						++braceDepth;
					} else if (token == "}") {
						--braceDepth;
						if (braceDepth == 0) {
							break;
						}
					} else {
						Logger.Warn($"Unknown token while parsing stream: {token}");
					}
				} else {
					break;
				}
			}
		}

		private static void FastForwardTo0Depth(BufferedReader reader, ref int braceDepth, StringBuilder tokensSoFar) {
			while (braceDepth != 0) {
				var inputChar = (char)reader.Read();
				switch (inputChar) {
					case '{':
						++braceDepth;
						break;
					case '}':
						--braceDepth;
						break;
					default:
						if (!char.IsWhiteSpace(inputChar)) {
							tokensSoFar.Append(inputChar);
						}
						break;
				}
			}
		}

		public void ParseFile(string filename) {
			if (!File.Exists(filename)) {
				Logger.Error($"Could not open {filename} for parsing");
				return;
			}
			var reader = new BufferedReader(File.OpenText(filename));
			AbsorbBOM(reader);
			ParseStream(reader);
		}

		private readonly Dictionary<RegisteredKeywordOrRegex, AbstractDelegate> registeredRules = new();

		private readonly Dictionary<RegisteredKeywordOrRegex, AbstractDelegate> builtinRules = new();

		public Dictionary<string, object> Variables { get; } = new();
	}
}
