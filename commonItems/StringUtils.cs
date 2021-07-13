using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    class StringUtils {
        public static string RemQuotes(string str) {
            var length = str.Length;
            if (length < 2) {
                return str;
            }
            if (!str.StartsWith('"') || !str.EndsWith('"')) {
                return str;
            }
            return str.Substring(1, length - 2);
        }

        public static string AddQuotes(string str)
        {
            if (str.Length > 2 && str.StartsWith('"') && str.EndsWith('"'))
            {
                return str;
            }

            if (!str.StartsWith('"') && !str.EndsWith('"'))
            {
                return "\"" + str + "\"";
            }
            return str;
        }
    }
}
