using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace commonItems {
    public delegate void Del(BufferedReader sr, string keyword);
    public delegate void SimpleDel(BufferedReader sr);

    internal abstract class AbstractDelegate {
        public abstract void Execute(BufferedReader sr, string token);
    }

    internal class TwoArgDelegate : AbstractDelegate {
        private readonly Del del;
        public TwoArgDelegate(Del del) { this.del = del; }
        public override void Execute(BufferedReader sr, string token) {
            del(sr, token);
        }
    }

    internal class OneArgDelegate : AbstractDelegate {
        private readonly SimpleDel del;
        public OneArgDelegate(SimpleDel del) { this.del = del; }
        public override void Execute(BufferedReader sr, string token) {
            del(sr);
        }
    }

    public class Parser {
        private abstract class RegisteredKeywordOrRegex {
            public abstract bool Matches(string token);
        }
        private class RegisteredKeyword : RegisteredKeywordOrRegex {
            private readonly string keyword;
            public RegisteredKeyword(string keyword) {
                this.keyword = keyword;
            }
            public override bool Matches(string token) { return keyword == token; }
        }
        private class RegisteredRegex : RegisteredKeywordOrRegex {
            private readonly Regex regex;
            public RegisteredRegex(string regexString) { regex = new Regex(regexString); }
            public override bool Matches(string token) {
                var match = regex.Match(token);
                return match.Success && match.Length == token.Length;
            }
        }

        public static void AbsorbBOM(BufferedReader reader) {
            var firstChar = reader.Peek();
            if (firstChar == '\xEF') {
                reader.Skip(3); // skip 3 bytes
            }
        }

        public void RegisterKeyword(string keyword, Del del) {
            registeredRules.Add(new RegisteredKeyword(keyword), new TwoArgDelegate(del));
        }
        public void RegisterKeyword(string keyword, SimpleDel del) {
            registeredRules.Add(new RegisteredKeyword(keyword), new OneArgDelegate(del));
        }
        public void RegisterRegex(string keyword, Del del) {
            registeredRules.Add(new RegisteredRegex(keyword), new TwoArgDelegate(del));
        }
        public void RegisterRegex(string keyword, SimpleDel del) {
            registeredRules.Add(new RegisteredRegex(keyword), new OneArgDelegate(del));
        }

        public void ClearRegisteredRules() {
            registeredRules.Clear();
        }

        private bool TryToMatch(string token, string strippedToken, bool isTokenQuoted, BufferedReader reader) {
            foreach (var (rule, fun) in registeredRules) {
                if (rule.Matches(token)) {
                    fun.Execute(reader, token);
                    return true;
                }
            }
            if (isTokenQuoted) {
                foreach (var (rule, fun) in registeredRules) {
                    if (rule.Matches(strippedToken)) {
                        fun.Execute(reader, token);
                        return true;
                    }
                }
            }
            return false;
        }

        public static string GetNextLexeme(BufferedReader reader) {
            var sb = new StringBuilder();

            var inQuotes = false;
            var inLiteralQuote = false;
            var previousCharacter = '\0';

            while (true) {
                if (reader.EndOfStream) {
                    break;
                }

                var inputChar = (char)reader.Read();

                if (!inQuotes && inputChar == '#') {
                    reader.ReadLine();
                    if (sb.Length != 0) {
                        break;
                    }
                } else if (inputChar == '\n') {
                    if (!inQuotes) {
                        if (sb.Length != 0) {
                            break;
                        }
                    } else { // fix Paradox' mistake and don't break proper names in half
                        sb.Append(' ');
                    }
                } else if (inputChar == '\"' && !inQuotes && sb.Length == 0) {
                    inQuotes = true;
                    sb.Append(inputChar);
                } else if (inputChar == '\"' && !inQuotes && sb.Length == 1 && sb.ToString().Last() == 'R') {
                    inLiteralQuote = true;
                    --sb.Length;
                    sb.Append(inputChar);
                } else if (inputChar == '(' && inLiteralQuote && sb.Length == 1) {
                    continue;
                } else if (inputChar == '\"' && inLiteralQuote && previousCharacter == ')') {
                    --sb.Length;
                    sb.Append(inputChar);
                    break;
                } else if (inputChar == '\"' && inQuotes && previousCharacter != '\\') {
                    sb.Append(inputChar);
                    break;
                } else if (!inQuotes && !inLiteralQuote && char.IsWhiteSpace(inputChar)) {
                    if (sb.Length != 0) {
                        break;
                    }
                } else if (!inQuotes && !inLiteralQuote && inputChar == '{') {
                    if (sb.Length == 0) {
                        sb.Append(inputChar);
                    } else {
                        reader.PushBack('{');
                    }
                    break;
                } else if (!inQuotes && !inLiteralQuote && inputChar == '}') {
                    if (sb.Length == 0) {
                        sb.Append(inputChar);
                    } else {
                        reader.PushBack('}');
                    }
                    break;
                } else if (!inQuotes && !inLiteralQuote && inputChar == '=') {
                    if (sb.Length == 0) {
                        sb.Append(inputChar);
                    } else {
                        reader.PushBack('=');
                    }
                    break;
                } else {
                    sb.Append(inputChar);
                }

                previousCharacter = inputChar;
            }
            return sb.ToString();
        }

        public static string? GetNextTokenWithoutMatching(BufferedReader reader) {
            return reader.EndOfStream ? null : GetNextLexeme(reader);
        }

        public string? GetNextToken(BufferedReader reader) {
            var sb = new StringBuilder();

            var gotToken = false;
            while (!gotToken) {
                if (reader.EndOfStream) {
                    return null;
                }

                sb.Clear();
                sb.Append(GetNextLexeme(reader));

                var strippedToken = StringUtils.RemQuotes(sb.ToString());
                var isTokenQuoted = (strippedToken.Length < sb.ToString().Length);

                var matched = TryToMatch(sb.ToString(), strippedToken, isTokenQuoted, reader);

                if (!matched) {
                    gotToken = true;
                }
            }
            return sb.Length == 0 ? null : sb.ToString();
        }

        public void ParseStream(BufferedReader reader) {
            var braceDepth = 0;
            var value = false; // tracker to indicate whether we reached the value part of key=value pair
            var tokensSoFar = new StringBuilder();

            while (true) {
                var token = GetNextToken(reader);
                if (token != null) {
                    tokensSoFar.Append(token);
                    if (token == "=") {
                        // swapping to value part.
                        if (!value) {
                            value = true;
                            continue;
                        }
                        // leaving else to be noticeable.
                        else {
                            // value is positive, meaning we were at value, and now we're hitting an equal. This is bad. We need to
                            // manually fast-forward to brace-lvl 0 and die.
                            FastForwardTo0Depth(ref reader, ref braceDepth, ref tokensSoFar);
                            Logger.Warn("Broken token syntax at " + tokensSoFar.ToString());
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
                        Logger.Warn("Unknown token while parsing stream: " + token);
                    }
                } else {
                    break;
                }
            }
        }

        public static void FastForwardTo0Depth(ref BufferedReader reader, ref int braceDepth, ref StringBuilder tokensSoFar) {
            while (braceDepth != 0) {
                var inputChar = (char)reader.Read();
                switch (inputChar) {
                    case '{':
                        ++braceDepth;
                        break;
                    case '}':
                        --braceDepth;
                        break;
                    default: {
                            if (!char.IsWhiteSpace(inputChar)) {
                                tokensSoFar.Append(inputChar);
                            }

                            break;
                        }
                }
            }
        }

        public void ParseFile(string filename) {
            if (!File.Exists(filename)) {
                Logger.Error("Could not open " + filename + " for parsing");
                return;
            }
            var reader = new BufferedReader(File.OpenText(filename));
            AbsorbBOM(reader);
            ParseStream(reader);
        }

        private readonly Dictionary<RegisteredKeywordOrRegex, AbstractDelegate> registeredRules = new();
    }
}
