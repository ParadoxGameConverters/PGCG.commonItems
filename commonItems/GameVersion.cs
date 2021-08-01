using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace commonItems {
    public class GameVersion : Parser {
        private int? firstPart;
        private int? secondPart;
        private int? thirdPart;
        private int? fourthPart;

        public GameVersion(string version) {
            if (string.IsNullOrEmpty(version)) {
                return;
            }

            var dot = version.IndexOf('.');
            firstPart = int.Parse(version.Substring(0, dot));
            if (dot == -1) {
                return;
            }

            version = version.Substring(dot + 1);
            dot = version.IndexOf('.');
            secondPart = int.Parse(version.Substring(0, dot));
            if (dot == -1) {
                return;
            }

            version = version.Substring(dot + 1);
            dot = version.IndexOf('.');
            thirdPart = int.Parse(version.Substring(0, dot));
            if (dot == -1) {
                return;
            }

            version = version.Substring(dot + 1);
            dot = version.IndexOf('.');
            fourthPart = int.Parse(version.Substring(0, dot));
        }

        public GameVersion(BufferedReader reader) {
            RegisterKeys();
            ParseStream(reader);
            ClearRegisteredRules();
        }

        private void RegisterKeys() {
            RegisterKeyword("first", (reader) => {
                firstPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("second", (reader) => {
                secondPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("third", (reader) => {
                thirdPart = new SingleInt(reader).Int;
            });
            RegisterKeyword("forth", (reader) => {
                fourthPart = new SingleInt(reader).Int;
            });
            RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
        }

        public override bool Equals(object? obj) {
            if (obj is not GameVersion rhs) {
                return false;
            }
            var testL = 0;
            var testR = 0;
            if (firstPart != null) {
                testL = firstPart.Value;
            }

            if (rhs.firstPart != null) {
                testR = rhs.firstPart.Value;
            }

            if (testL != testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (secondPart != null) {
                testL = secondPart.Value;
            }

            if (rhs.secondPart != null) {
                testR = rhs.secondPart.Value;
            }

            if (testL != testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (thirdPart != null) {
                testL = thirdPart.Value;
            }

            if (rhs.thirdPart != null) {
                testR = rhs.thirdPart.Value;
            }

            if (testL != testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (fourthPart != null) {
                testL = fourthPart.Value;
            }

            if (rhs.fourthPart != null) {
                testR = rhs.fourthPart.Value;
            }

            if (testL != testR) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            return HashCode.Combine(firstPart, secondPart, thirdPart, fourthPart);
        }

        public static bool operator >=(GameVersion lhs, GameVersion rhs) {
            return lhs > rhs || lhs.Equals(rhs);
        }
        public static bool operator >(GameVersion lhs, GameVersion rhs) {
            int testL = 0;
            int testR = 0;
            if (lhs.firstPart != null) {
                testL = lhs.firstPart.Value;
            }
            if (rhs.firstPart != null) {
                testR = rhs.firstPart.Value;
            }

            if (testL > testR) {
                return true;
            }
            if (testL < testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.secondPart != null) {
                testL = lhs.secondPart.Value;
            }

            if (rhs.secondPart != null) {
                testR = rhs.secondPart.Value;
            }

            if (testL > testR) {
                return true;
            }

            if (testL < testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.thirdPart != null) {
                testL = lhs.thirdPart.Value;
            }

            if (rhs.thirdPart != null) {
                testR = rhs.thirdPart.Value;
            }

            if (testL > testR) {
                return true;
            }

            if (testL < testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.fourthPart != null) {
                testL = lhs.fourthPart.Value;
            }

            if (rhs.fourthPart != null) {
                testR = rhs.fourthPart.Value;
            }

            if (testL > testR) {
                return true;
            }

            return false;
        }

        public static bool operator <(GameVersion lhs, GameVersion rhs) {
            var testL = 0;
            var testR = 0;
            if (lhs.firstPart != null) {
                testL = lhs.firstPart.Value;
            }

            if (rhs.firstPart != null) {
                testR = rhs.firstPart.Value;
            }

            if (testL < testR) {
                return true;
            }

            if (testL > testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.secondPart != null) {
                testL = lhs.secondPart.Value;
            }

            if (rhs.secondPart != null) {
                testR = rhs.secondPart.Value;
            }

            if (testL < testR) {
                return true;
            }

            if (testL > testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.thirdPart != null) {
                testL = lhs.thirdPart.Value;
            }

            if (rhs.thirdPart != null) {
                testR = rhs.thirdPart.Value;
            }

            if (testL < testR) {
                return true;
            }

            if (testL > testR) {
                return false;
            }

            testL = 0;
            testR = 0;
            if (lhs.fourthPart != null) {
                testL = lhs.fourthPart.Value;
            }

            if (rhs.fourthPart != null) {
                testR = rhs.fourthPart.Value;
            }

            if (testL < testR) {
                return true;
            }

            return false;
        }

        public static bool operator <=(GameVersion lhs, GameVersion rhs) {
            return lhs < rhs || lhs.Equals(rhs);
        }

        public override string ToString() {
            var sb = new StringBuilder();
            if (firstPart != null) {
                sb.Append(firstPart.Value);
                sb.Append('.');
            } else {
                sb.Append("0.");
            }
            if (secondPart != null) {
                sb.Append(secondPart.Value);
                sb.Append('.');
            } else {
                sb.Append("0.");
            }
            if (thirdPart != null) {
                sb.Append(thirdPart.Value);
                sb.Append('.');
            } else {
                sb.Append("0.");
            }
            if (fourthPart != null) {
                sb.Append(fourthPart.Value);
                sb.Append('.');
            } else {
                sb.Append("0.");
            }
            return sb.ToString();
        }

        public string ToShortString() {
            var sb = new StringBuilder();
            if (fourthPart != null) {
                sb.Append('.');
                sb.Append(fourthPart.Value);
            }
            if (thirdPart != null) {
                sb.Insert(0, thirdPart.Value);
                sb.Insert(0, '.');
            }
            if (secondPart != null) {
                sb.Insert(0, secondPart.Value);
                sb.Insert(0, '.');
            }
            if (firstPart != null) {
                sb.Insert(0, firstPart.Value);
                sb.Insert(0, '.');
            }
            return sb.ToString();
        }

        public string ToWildCard() {
            var sb = new StringBuilder();
            if (fourthPart != null) {
                sb.Append('.');
                sb.Append(fourthPart.Value);
            } else if (thirdPart != null) {
                sb.Append(".*");
            }

            if (thirdPart != null) {
                sb.Insert(0, thirdPart.Value);
                sb.Insert(0, '.');
            } else if (secondPart != null) {
                sb.Clear();
                sb.Append(".*");
            }

            if (secondPart != null) {
                sb.Insert(0, secondPart.Value);
                sb.Insert(0, '.');
            } else if (firstPart != null) {
                sb.Clear();
                sb.Append(".*");
            }

            if (firstPart != null) {
                sb.Insert(0, firstPart.Value);
            } else {
                sb.Clear();
                sb.Append('*');
            }

            return sb.ToString();
        }
    }
}
