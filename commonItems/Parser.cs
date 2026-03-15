using commonItems.Collections;
using commonItems.Mods;
using Open.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace commonItems;

public delegate void Del(BufferedReader sr, string keyword);
public delegate void SimpleDel(BufferedReader sr);

internal abstract class AbstractDelegate {
	public abstract void Execute(BufferedReader sr, string token);
}

internal sealed class TwoArgDelegate : AbstractDelegate {
	private readonly Del del;
	public TwoArgDelegate(Del del) { this.del = del; }
	public override void Execute(BufferedReader sr, string token) {
		del(sr, token);
	}
}

internal sealed class OneArgDelegate : AbstractDelegate {
	private readonly SimpleDel del;
	public OneArgDelegate(SimpleDel del) { this.del = del; }
	public override void Execute(BufferedReader sr, string token) {
		del(sr);
	}
}

public class Parser {
	public Parser() {
		RegisterRegex(CommonRegexes.Variable, (reader, varStr) => {
			var value = reader.GetString();
			var variableName = varStr[1..];
			if (int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out int intValue)) {
				reader.Variables[variableName] = intValue;
				return;
			}
			if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue)) {
				reader.Variables[variableName] = doubleValue;
				return;
			}
			reader.Variables[variableName] = value;
		});
	}

	public static void AbsorbBOM(BufferedReader reader) {
		var firstChar = reader.Peek();
		if (firstChar == '\xEF') {
			reader.Skip(3); // skip 3 bytes
		}
	}

	public void RegisterKeyword(string keyword, Del del) {
		keywordRules[keyword] = new TwoArgDelegate(del);
	}
	public void RegisterKeyword(string keyword, SimpleDel del) {
		keywordRules[keyword] = new OneArgDelegate(del);
	}
	public void RegisterRegex(string keyword, Del del) {
		AddOrReplaceRegexRule(new Regex(keyword), new TwoArgDelegate(del));
	}
	public void RegisterRegex(string keyword, SimpleDel del) {
		AddOrReplaceRegexRule(new Regex(keyword), new OneArgDelegate(del));
	}
	public void RegisterRegex(Regex regex, Del del) {
		AddOrReplaceRegexRule(regex, new TwoArgDelegate(del));
	}
	public void RegisterRegex(Regex regex, SimpleDel del) {
		AddOrReplaceRegexRule(regex, new OneArgDelegate(del));
	}

	private void AddOrReplaceRegexRule(Regex regex, AbstractDelegate del) {
		var regexStr = regex.ToString();
		for (int i = 0; i < regexRules.Count; i++) {
			if (regexRules[i].regex.ToString() == regexStr) {
				regexRules[i] = (regex, del);
				return;
			}
		}

		regexRules.Add((regex, del));
	}

	public void ClearRegisteredRules() {
		keywordRules.Clear();
		regexRules.Clear();
	}

	private bool TryToMatch(string token, BufferedReader reader) {
		// O(1) keyword lookup.
		if (keywordRules.TryGetValue(token, out var keywordFun)) {
			keywordFun.Execute(reader, token);
			return true;
		}
		// Linear scan over regex rules (typically 1-5 entries).
		foreach (var (regex, fun) in regexRules) {
			var match = regex.Match(token);
			if (match.Success && match.Length == token.Length) {
				fun.Execute(reader, token);
				return true;
			}
		}
		// Lazy RemQuotes: only strip and retry if token is quoted.
		if (token.Length >= 2 && token[0] == '"' && token[^1] == '"') {
			var strippedToken = token[1..^1];
			if (keywordRules.TryGetValue(strippedToken, out var strippedFun)) {
				strippedFun.Execute(reader, token);
				return true;
			}
			foreach (var (regex, fun) in regexRules) {
				var match = regex.Match(strippedToken);
				if (match.Success && match.Length == strippedToken.Length) {
					fun.Execute(reader, token);
					return true;
				}
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
		} else if (inputChar == '\"' && sb is ['R']) {
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
		} else if (!inLiteralQuote && inputChar == '?') {
			// We've likely encountered the beginning of an ExistEquals operator.
			if (sb.Length == 0) {
				sb.Append(inputChar);
			} else {
				reader.PushBack('?');
				return true; // break loop
			}
		} else if (!inLiteralQuote && inputChar == '=') {
			if (sb.Length == 0 || sb is ['?']) {
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

	[ThreadStatic]
	private static StringBuilder? t_lexemeBuilder;

	public static string GetNextLexeme(BufferedReader reader) {
		var sb = t_lexemeBuilder;
		if (sb is null) {
			sb = new StringBuilder(64);
			t_lexemeBuilder = sb;
		} else {
			sb.Clear();
			if (sb.Capacity > 1024) {
				sb.Capacity = 64;
			}
		}

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
				sb.Append(inputChar);
				if (inputChar == '\"' && previousChar != '\\') {
					break;
				}
			} else { // not in quotes
				if (HandleCharOutsideQuotes(reader, sb, ref previousChar, ref inQuotes, ref inLiteralQuote, ref inInterpolatedExpression, inputChar)) {
					break;
				}
			}

			previousChar = inputChar;
		}
		if (sb.Length == 1) { // Optimization for single-char tokens, which are very common (e.g. =, {, }, etc.)
			return sb[0].ToString();
		}
		return sb.ToString();
	}

	// "WithoutMatching" refers to not matching against registered rules.
	// Here we are only matching against variable and interpolated expression regexes
	// to resolve them before returning.
	public static string? GetNextTokenWithoutMatching(BufferedReader reader) {
		if (reader.EndOfStream) {
			return null;
		}
		var lexeme = GetNextLexeme(reader);
		if (CommonRegexes.Variable.IsMatch(lexeme)) {
			var variableValue = reader.ResolveVariable(lexeme);
			if (variableValue is null) {
				return null;
			}
			return GetValueString(variableValue);
		}
		if (CommonRegexes.InterpolatedExpression.IsMatch(lexeme)) {
			return GetValueString(reader.EvaluateExpression(lexeme));
		}
		return lexeme;

		static string? GetValueString(object obj) {
			if (obj is IFormattable formattable) {
				return formattable.ToString("0.######", CultureInfo.InvariantCulture);
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

			var matched = TryToMatch(token, reader);

			if (!matched) {
				gotToken = true;
			}
		}

		return string.IsNullOrEmpty(token) ? null : token;
	}

	/// <summary>
	///  Parses a stream in a buffered reader, does not absorb UTF8-BOM.
	/// </summary>
	public void ParseStream(BufferedReader reader) {
		var braceDepth = 0;
		var value = false; // tracker to indicate whether we reached the value part of key=value pair
		var tokensSoFar = new StringBuilder();

		while (true) {
			var token = GetNextToken(reader);
			if (token is not null) {
				tokensSoFar.Append(token);
				if (token is "=" or "?=") {
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

	/// <summary>
	///  Parses a file, absorbs UTF8-BOM if detected.
	/// </summary>
	/// <param name="filename"></param>
	public void ParseFile(string filename) {
		if (!File.Exists(filename)) {
			Logger.Error($"Could not open {filename} for parsing");
			return;
		}
		
		// Open file without locking it.
		using var streamReader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		var bufferedReader = new BufferedReader(streamReader);
		AbsorbBOM(bufferedReader);
		ParseStream(bufferedReader);
	}

	public void ParseFolder(string path, string extensions, bool recursive, bool logFilePaths = false) {
		var searchPattern = recursive ? "*" : "*.*";
		var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
		var files = Directory.GetFiles(path, searchPattern, searchOption).ToList();

		var validExtensions = extensions.Split(';');
		files.RemoveWhere(f => !validExtensions.Contains(CommonFunctions.GetExtension(f)));

		foreach (var file in files) {
			if (logFilePaths) {
				Logger.Debug($"Parsing file: {file}");
			}
			ParseFile(file);
		}
	}

	/// <summary>
	/// Parses a game folder in both vanilla game and mods directory.
	/// For example:
	///		relativePath may be "common/governments"
	///		extensions may be "txt;text" (a list separated by semicolon)
	/// </summary>
	public void ParseGameFolder(string relativePath, ModFilesystem modFS, string extensions, bool recursive, bool logFilePaths = false, bool parallel = false) {
		var extensionSet = extensions.Split(';');

		List<ModFSFileInfo> files;
		if (recursive) {
			files = modFS.GetAllFilesInFolderRecursive(relativePath);
		} else {
			files = modFS.GetAllFilesInFolder(relativePath);
		}
		files.RemoveWhere(f => !extensionSet.Contains(CommonFunctions.GetExtension(f.RelativePath)));

		files.Select(f => f.AbsolutePath).ForEach(ProcessFile, allowParallel: parallel);

		return;

		void ProcessFile(string filePath) {
			if (logFilePaths) {
				Logger.Debug($"Parsing file: {filePath}");
			}
			ParseFile(filePath);
		}
	}

	/// <summary>
	/// Parses a game file in either vanilla game or mods directory.
	/// For example:
	///		relativePath may be "map_data/areas.txt"
	/// </summary>
	public void ParseGameFile(string relativePath, ModFilesystem modFS) {
		var filePath = modFS.GetActualFileLocation(relativePath);
		
		if (File.Exists(filePath)) {
			ParseFile(filePath);
		}
	}

	private readonly Dictionary<string, AbstractDelegate> keywordRules = new();
	private readonly List<(Regex regex, AbstractDelegate fun)> regexRules = [];
}