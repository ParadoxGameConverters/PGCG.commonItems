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
			int ch;
			if (characterStack.TryPop(out int character)) {
				ch = character;
			} else {
				ch = streamReader.Read();  // could be -1 
			}
			return ch;
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

		public string GetString(Dictionary<string, object>? variables = null) {
			return new SingleString(this, variables).String;
		}
		public int GetInt(Dictionary<string, object>? variables = null) {
			return new SingleInt(this, variables).Int;
		}
		public long GetLong(Dictionary<string, object>? variables = null) {
			return new SingleLong(this, variables).Long;
		}
		public ulong GetULong(Dictionary<string, object>? variables = null) {
			return new SingleULong(this, variables).ULong;
		}
		public double GetDouble(Dictionary<string, object>? variables = null) {
			return new SingleDouble(this, variables).Double;
		}
		public List<string> GetStrings(Dictionary<string, object>? variables = null) {
			return new StringList(this, variables).Strings;
		}
		public List<int> GetInts(Dictionary<string, object>? variables = null) {
			return new IntList(this, variables).Ints;
		}
		public List<long> GetLongs(Dictionary<string, object>? variables = null) {
			return new LongList(this, variables).Longs;
		}
		public List<ulong> GetULongs(Dictionary<string, object>? variables = null) {
			return new ULongList(this, variables).ULongs;
		}
		public List<double> GetDoubles(Dictionary<string, object>? variables = null) {
			return new DoubleList(this, variables).Doubles;
		}
		public StringOfItem GetStringOfItem() {
			return new StringOfItem(this);
		}
	}
}