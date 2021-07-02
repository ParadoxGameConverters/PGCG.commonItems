using System;
using System.IO;
using System.Text;

namespace commonItems {
    public class BufferedReader : StreamReader {
        // http://web.archive.org/web/20210702221522/https://stackoverflow.com/questions/7049401/c-sharp-roll-back-streamreader-1-character/7050430#7050430
        public BufferedReader(Stream stream) : base(stream) { }

        public BufferedReader(string input) : base(new MemoryStream(Encoding.UTF8.GetBytes(input))) {}

        private int lastChar = -1;
        public override int Read() {
            int ch;

            if (lastChar >= 0) {
                ch = lastChar;
                lastChar = -1;
            } else {
                ch = base.Read();  // could be -1 
            }
            return ch;
        }

        public void PushBack(char ch)  // char, don't allow Pushback(-1)
        {
            if (lastChar >= 0)
                throw new InvalidOperationException("PushBack of more than 1 char");

            lastChar = ch;
        }
    }
}