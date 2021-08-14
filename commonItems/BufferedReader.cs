using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace commonItems {
    /// <summary>
    /// This is a wrapper for StreamReader with a limited set of methods
    /// that allows for returning multiple characters earlier in a stream.
    /// Idea for the buffer implementation was initially borrowed from:
    /// http://web.archive.org/web/20210702221522/https://stackoverflow.com/questions/7049401/c-sharp-roll-back-streamreader-1-character/7050430#7050430
    /// but later updated
    /// </summary>
    public class BufferedReader {
        public StreamReader StreamReader { get; }

        public BufferedReader(StreamReader reader) { StreamReader = reader; }
        public BufferedReader(Stream stream) { StreamReader = new StreamReader(stream); }

        public BufferedReader(string input) {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            StreamReader = new StreamReader(stream);
        }

        private readonly Stack<int> characterStack = new();

        public int Read() {
            int ch;
            if (characterStack.TryPop(out int character)) {
                ch = character;
            } else {
                ch = StreamReader.Read();  // could be -1 
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
            return StreamReader.ReadLine();
        }

        public string ReadToEnd() {
            var sb = new StringBuilder();
            while (characterStack.TryPop(out int character)) {
                sb.Append((char)character);
            }
            sb.Append(StreamReader.ReadToEnd());
            return sb.ToString();
        }

        public int Peek() {
            return characterStack.TryPeek(out int character) ? character : StreamReader.Peek();
        }

        public bool EndOfStream {
            get {
                if (characterStack.Count == 0) {
                    return StreamReader.EndOfStream;
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
    }
}