using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    class EncodingConversions {
        public static string ConvertUTF8ToASCII(string utf8String) {
            return Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(utf8String)));
        }
    }
}
