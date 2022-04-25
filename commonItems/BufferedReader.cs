﻿using NCalc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace commonItems; 

/// <summary>
/// This is a wrapper for StreamReader with a limited set of methods
/// that allows for returning multiple characters earlier in a stream.
/// Idea for the buffer implementation was initially borrowed from:
/// http://web.archive.org/web/20210702221522/https://stackoverflow.com/questions/7049401/c-sharp-roll-back-streamreader-1-character/7050430#7050430
/// but later updated
/// </summary>
public class BufferedReader {
	private readonly StreamReader streamReader;

	public BufferedReader() : this(string.Empty) { }
	public BufferedReader(StreamReader reader) { streamReader = reader; }
	public BufferedReader(Stream stream) { streamReader = new StreamReader(stream); }

	public BufferedReader(string input) {
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
		streamReader = new StreamReader(stream);
	}

	private readonly Stack<int> characterStack = new();

	public int Read() {
		if (characterStack.TryPop(out int character)) {
			return character;
		} else {
			return streamReader.Read();  // could be -1 
		}
	}

	public string Read(uint numberOfChars) {
		var sb = new StringBuilder();
		for (uint i = 0; i < numberOfChars; ++i) {
			var ch = Read();
			if (ch == -1) {
				break;
			}
			sb.Append((char)ch);
		}

		return sb.ToString();
	}

	public string? ReadLine() {
		return streamReader.ReadLine();
	}

	public string ReadToEnd() {
		var sb = new StringBuilder();
		while (characterStack.TryPop(out int character)) {
			sb.Append((char)character);
		}
		sb.Append(streamReader.ReadToEnd());
		return sb.ToString();
	}

	public int Peek() {
		return characterStack.TryPeek(out int character) ? character : streamReader.Peek();
	}

	public bool EndOfStream {
		get {
			if (characterStack.Count == 0) {
				return streamReader.EndOfStream;
			}
			return false;
		}
	}

	public void Skip(uint numberOfBytes) { // not present in StreamReader
		for (uint i = 0; i < numberOfBytes; ++i) {
			Read();
		}
	}

	public void PushBack(char ch) {
		characterStack.Push(ch);
	}

	public string GetString() {
		// remove equals
		Parser.GetNextTokenWithoutMatching(this);

		var token = Parser.GetNextTokenWithoutMatching(this);
		if (token is not null) {
			return StringUtils.RemQuotes(token);
		}

		Logger.Error("SingleString: next token not found!");
		return string.Empty;
	}
	public int GetInt() {
		var intStr = StringUtils.RemQuotes(GetString());
		if (int.TryParse(intStr, out int theInt)) {
			return theInt;
		}

		Logger.Warn($"Could not convert string {intStr} to int!");
		return 0;
	}
	public long GetLong() {
		var longStr = StringUtils.RemQuotes(GetString());
		if (long.TryParse(longStr, out long theLong)) {
			return theLong;
		}

		Logger.Warn($"Could not convert string {longStr} to long!");
		return 0;
	}
	public ulong GetULong() {
		var ulongStr = StringUtils.RemQuotes(GetString());
		if (ulong.TryParse(ulongStr, out ulong theULong)) {
			return theULong;
		}

		Logger.Warn($"Could not convert string {ulongStr} to ulong!");
		return 0;
	}
	public double GetDouble() {
		var doubleStr = StringUtils.RemQuotes(GetString());
		if (double.TryParse(doubleStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double theDouble)) {
			return theDouble;
		}

		Logger.Warn($"Could not convert string {doubleStr} to double!");
		return 0;
	}
	public List<string> GetStrings() {
		var strings = new List<string>();
		var parser = new Parser();
		parser.RegisterKeyword("\"\"", _ => { });
		parser.RegisterRegex(CommonRegexes.String, (_, theString) =>
			strings.Add(theString)
		);
		parser.RegisterRegex(CommonRegexes.QuotedString, (_, theString) =>
			strings.Add(StringUtils.RemQuotes(theString))
		);
		if (Variables.Count > 0) {
			parser.RegisterRegex(CommonRegexes.Variable, (reader, varStr) => {
				var value = reader.ResolveVariable(varStr).ToString();
				if (value is null) {
					Logger.Warn($"StringList: variable {varStr} resolved to null value!");
				} else {
					strings.Add(value);
				}
			});
		}

		parser.ParseStream(this);
		return strings;
	}
	public List<int> GetInts() {
		var ints = new List<int>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.Integer, (_, intString) => ints.Add(int.Parse(intString)));
		parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, intString) => {
			// remove quotes
			intString = intString[1..^1];
			ints.Add(int.Parse(intString));
		});
		if (Variables.Count > 0) {
			parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
				var value = (int)reader.EvaluateExpression(expr);
				ints.Add(value);
			});
		}

		parser.ParseStream(this);
		return ints;
	}
	public List<long> GetLongs() {
		var longs = new List<long>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.Integer, (_, longString) => longs.Add(long.Parse(longString)));
		parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, longString) => {
			// remove quotes
			longString = longString[1..^1];
			longs.Add(long.Parse(longString));
		});
		if (Variables.Count > 0) {
			parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
				var value = (long)(int)reader.EvaluateExpression(expr);
				longs.Add(value);
			});
		}

		parser.ParseStream(this);
		return longs;
	}
	public List<ulong> GetULongs() {
		var ulongs = new List<ulong>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.Integer, (_, ulongString) => ulongs.Add(ulong.Parse(ulongString)));
		parser.RegisterRegex(CommonRegexes.QuotedInteger, (_, ulongString) => {
			// remove quotes
			ulongString = ulongString[1..^1];
			ulongs.Add(ulong.Parse(ulongString));
		});
		if (Variables.Count > 0) {
			parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
				var value = (ulong)(int)reader.EvaluateExpression(expr);
				ulongs.Add(value);
			});
		}

		parser.ParseStream(this);
		return ulongs;
	}
	public List<double> GetDoubles() {
		var doubles = new List<double>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.Float, (_, floatString) =>
			doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture))
		);
		parser.RegisterRegex(CommonRegexes.QuotedFloat, (_, floatString) => {
			// remove quotes
			floatString = floatString[1..^1];
			doubles.Add(double.Parse(floatString, NumberStyles.Any, CultureInfo.InvariantCulture));
		});
		if (Variables.Count > 0) {
			parser.RegisterRegex(CommonRegexes.InterpolatedExpression, (reader, expr) => {
				var value = (double)reader.EvaluateExpression(expr);
				doubles.Add(value);
			});
		}

		parser.ParseStream(this);
		return doubles;
	}
	public StringOfItem GetStringOfItem() {
		return new StringOfItem(this);
	}
	public PDXBool GetPDXBool() {
		return new PDXBool(this);
	}

	public Dictionary<string, string> GetAssignments() {
		var assignments = new Dictionary<string, string>();
		var parser = new Parser();
		parser.RegisterRegex(CommonRegexes.Catchall, (reader, assignmentName) => {
			Parser.GetNextTokenWithoutMatching(reader); // remove equals
			var assignmentValue = Parser.GetNextTokenWithoutMatching(reader);
			if (assignmentValue is null) {
				throw new FormatException($"Cannot assign null to {assignmentName}!");
			}
			assignments[assignmentName] = assignmentValue;
		});
		parser.ParseStream(this);
		return assignments;
	}

	public Dictionary<string, object> Variables { get; } = new();

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

	public void CopyVariables(BufferedReader otherReader) {
		var variablesToCopy = new Dictionary<string, object>(otherReader.Variables);
		foreach (var (key, value) in variablesToCopy) {
			Variables[key] = value;
		}
	}
}