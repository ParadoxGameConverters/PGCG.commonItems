using System;
using System.IO;
using System.Text;

namespace commonItems {
    /// <summary>
    /// This is a wrapper for StreamReader with a limited set of methods
    /// that allows for returning one character earlier in a stream.
    /// Idea for the buffer implementation was borrowed from:
    /// http://web.archive.org/web/20210702221522/https://stackoverflow.com/questions/7049401/c-sharp-roll-back-streamreader-1-character/7050430#7050430
    /// </summary>
    public class BufferedReader {
        private readonly StreamReader streamReader;

        public BufferedReader(StreamReader reader) { streamReader = reader; }
        public BufferedReader(Stream stream) { streamReader = new StreamReader(stream); }

        public BufferedReader(string input) {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));
            streamReader = new StreamReader(stream);
        }

        private int lastChar = -1;
        public int Read() {
            int ch;

            if (lastChar >= 0) {
                ch = lastChar;
                lastChar = -1;
            } else {
                ch = streamReader.Read();  // could be -1 
            }
            return ch;
        }

        public string Read(uint numberOfChars)
        {
            var sb = new StringBuilder();
            for (uint i = 0; i < numberOfChars; ++i)
            {
                var ch = Read();
                if (ch == -1)
                    break;
                sb.Append((char)ch);
            }

            return sb.ToString();
        }

        public string? ReadLine()
        {
            return streamReader.ReadLine();
        }

        public string ReadToEnd() {
            return (char)Read() + streamReader.ReadToEnd();
        }

        public int Peek() {
            return lastChar >= 0 ? lastChar : streamReader.Peek();
        }

        public bool EndOfStream => streamReader.EndOfStream;

        public void Skip(uint numberOfBytes) // not present in StreamReader
        {
            for (uint i = 0; i < numberOfBytes; ++i) {
                Read();
            }
        }

        public void PushBack(char ch)  // char, don't allow Pushback(-1)
        {
            if (lastChar >= 0)
                throw new InvalidOperationException("PushBack of more than 1 char");

            lastChar = ch;
        }
    }
}