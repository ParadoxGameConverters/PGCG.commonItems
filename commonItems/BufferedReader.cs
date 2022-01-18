using NCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace commonItems {
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
				if (ch == -1)
					break;
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

		public void Skip(uint numberOfBytes) // not present in StreamReader
		{
			for (uint i = 0; i < numberOfBytes; ++i) {
				Read();
			}
		}

		public void PushBack(char ch) {
			characterStack.Push(ch);
		}

		public string GetString() {
			return new SingleString(this).String;
		}
		public int GetInt() {
			return new SingleInt(this).Int;
		}
		public long GetLong() {
			return new SingleLong(this).Long;
		}
		public ulong GetULong() {
			return new SingleULong(this).ULong;
		}
		public double GetDouble() {
			return new SingleDouble(this).Double;
		}
		public List<string> GetStrings() {
			return new StringList(this).Strings;
		}
		public List<int> GetInts() {
			return new IntList(this).Ints;
		}
		public List<long> GetLongs() {
			return new LongList(this).Longs;
		}
		public List<ulong> GetULongs() {
			return new ULongList(this).ULongs;
		}
		public List<double> GetDoubles() {
			return new DoubleList(this).Doubles;
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
	}
}