using NCalc;
using System;
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
		private abstract class RegisteredKeywordOrRegex : IEquatable<RegisteredKeywordOrRegex> {
			public abstract bool Equals(RegisteredKeywordOrRegex? other);
			public abstract bool Matches(string token);
			public abstract override int GetHashCode();

			public override bool Equals(object? obj) {
				return Equals(obj as RegisteredKeywordOrRegex);
			}
		}
		private class RegisteredKeyword : RegisteredKeywordOrRegex {
			private readonly string keyword;
			public RegisteredKeyword(string keyword) {
				this.keyword = keyword;
			}
			public override bool Equals(RegisteredKeywordOrRegex? other) {
				return other is RegisteredKeyword rk && rk.keyword.Equals(keyword);
			}
			public override int GetHashCode() {
				return keyword.GetHashCode();
			}
			public override bool Matches(string token) { return keyword == token; }
		}
		private class RegisteredRegex : RegisteredKeywordOrRegex {
			private readonly Regex regex;
			public RegisteredRegex(string regexString) { regex = new Regex(regexString); }
			public RegisteredRegex(Regex regex) { this.regex = regex; }
			public override bool Equals(RegisteredKeywordOrRegex? other) {
				return other is RegisteredRegex rr && rr.regex.ToString().Equals(regex.ToString());
			}
			public override int GetHashCode() {
				return regex.ToString().GetHashCode();
			}
			public override bool Matches(string token) {
				var match = regex.Match(token);
				return match.Success && match.Length == token.Length;
			}
		}

		public Parser() {
			registeredRules[new RegisteredRegex(CommonRegexes.Variable)] = new TwoArgDelegate((reader, varStr) => {
				var value = reader.GetString(Variables);
				if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue)) {
					Variables.Add(varStr[1..], intValue);
					return;
				}
				if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue)) {
					Variables.Add(varStr[1..], doubleValue);
					return;
				}
				Variables.Add(varStr[1..], value);
			});
		}

		public Parser(Dictionary<string, object>? variables) : this() {
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
			registeredRules[new RegisteredKeyword(keyword)] = new TwoArgDelegate(del);
		}
		public void RegisterKeyword(string keyword, SimpleDel del) {
			registeredRules[new RegisteredKeyword(keyword)] = new OneArgDelegate(del);
		}
		public void RegisterRegex(string keyword, Del del) {
			registeredRules[new RegisteredRegex(keyword)] = new TwoArgDelegate(del);
		}
		public void RegisterRegex(string keyword, SimpleDel del) {
			registeredRules[new RegisteredRegex(keyword)] = new OneArgDelegate(del);
		}
		public void RegisterRegex(Regex regex, Del del) {
			registeredRules[new RegisteredRegex(regex)] = new TwoArgDelegate(del);
		}
		public void RegisterRegex(Regex regex, SimpleDel del) {
			registeredRules[new RegisteredRegex(regex)] = new OneArgDelegate(del);
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

			return false;
		}

		// Returned value indicates whether the lexeme-building loop should be broken
		private static bool HandleCharOutsideQuotes(BufferedReader reader, StringBuilder sb, ref char previousChar, ref bool inQuotes, ref bool inLiteralQuote, ref bool inInterpolatedExpression, char inputChar) {
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
			} else if (!inLiteralQuote && !inInterpolatedExpression && char.IsWhiteSpace(inputChar)) {
				if (sb.Length != 0) {
					return true; // break loop
				}
			} else if (previousChar == '@' && inputChar == '[') { // beginning of interpolated expression
				inInterpolatedExpression = true;
				sb.Append(inputChar);
			} else if (inInterpolatedExpression && inputChar == ']') { // end of interpolated expression
				inInterpolatedExpression = false;
				sb.Append(inputChar);
				return true; // break loop
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

		public static string GetNextLexeme(BufferedReader reader) {
			var sb = new StringBuilder();

			var inQuotes = false;
			var inLiteralQuote = false;
			var inInterpolatedExpression = false;
			var previousChar = '\0';

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
					if (previousChar == '\r') {
						// We're in the middle of a Windows line ending, already handled by condition for '\r'.
					} else if (inQuotes) {
						// Fix Paradox' mistake and don't break proper names in half.
						sb.Append(' ');
					} else if (sb.Length != 0) {
						break;
					}
				} else if (inputChar == '(' && inLiteralQuote && sb.Length == 1) {
					continue;
				} else if (inputChar == '\"' && inLiteralQuote && previousChar == ')') {
					--sb.Length;
					sb.Append(inputChar);
					break;
				} else if (inQuotes) {
					if (inputChar == '\"' && previousChar != '\\') {
						sb.Append(inputChar);
						break;
					} else {
						sb.Append(inputChar);
					}
				} else { // not in quotes
					if (HandleCharOutsideQuotes(reader, sb, ref previousChar, ref inQuotes, ref inLiteralQuote, ref inInterpolatedExpression, inputChar)) {
						break;
					}
				}

				previousChar = inputChar;
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

		// WithoutMatching refers to not matching against registered rules.
		// Here we are only matching against variable and interpolated expression regexes
		// to resolve them before returning.
		public string? GetNextTokenWithoutMatching(BufferedReader reader) {
			if (reader.EndOfStream) {
				return null;
			}
			var lexeme = GetNextLexeme(reader);
			if (CommonRegexes.Variable.IsMatch(lexeme)) {
				return GetValueString(ResolveVariable(lexeme));
			}
			if (CommonRegexes.InterpolatedExpression.IsMatch(lexeme)) {
				return GetValueString(EvaluateExpression(lexeme));
			}
			return lexeme;

			static string? GetValueString(object obj) {
				if (obj is double d) {
					return d.ToString(CultureInfo.InvariantCulture);
				}
				return obj.ToString();
			}
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

			return string.IsNullOrEmpty(token) ? null : token;
		}

		public void ParseStream(BufferedReader reader) {
			var braceDepth = 0;
			var value = false; // tracker to indicate whether we reached the value part of key=value pair
			var tokensSoFar = new StringBuilder();

			while (true) {
				var token = GetNextToken(reader);
				if (token is not null) {
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

		public Dictionary<string, object> Variables { get; } = new();
	}
}
