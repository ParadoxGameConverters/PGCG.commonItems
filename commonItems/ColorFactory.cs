using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace commonItems {
    class ColorFactory {
        private static bool DoesRegexFullyMatch(Regex re, string str) {
            var match = re.Match(str);
            return match.Success && match.Length == str.Length;
        }
        public Dictionary<string, Color> NamedColors { get; } = new();
        public Color GetColor(BufferedReader reader) {
            Parser.GetNextTokenWithoutMatching(reader); // equals sign

            var token = Parser.GetNextTokenWithoutMatching(reader);
            if (token != null) {
                token = StringUtils.RemQuotes(token);
            }
            if (token == "rgb") {
                var rgb = new IntList(reader).Ints;
                if (rgb.Count != 3) {
                    throw new FormatException("Color has wrong number of components");
                }
                return new Color(rgb.ToArray());
            } else if (token == "hex") {
                var hex = new SingleString(reader).String;
                if (hex.Length != 6) {
                    throw new FormatException("Color has wrong number of digits");
                }
                var r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return new Color(new [] { r, g, b });
            } else if (token == "hsv") {
                var hsv = new DoubleList(reader).Doubles;
                if (hsv.Count != 3) {
                    throw new FormatException("Color has wrong number of components");
                }
                return new Color(new double[] { hsv[0], hsv[1], hsv[2] });
            } else if (token == "hsv360") {
                var hsv = new DoubleList(reader).Doubles;
                if (hsv.Count != 3) {
                    throw new FormatException("Color has wrong number of components");
                }
                return new Color(new double[] { hsv[0] / 360, hsv[1] / 100, hsv[2] / 100 });
            } else if (DoesRegexFullyMatch(new Regex(CommonRegexes.Catchall), token)) {
                if (NamedColors.TryGetValue(token, out var value)) {
                    return value;
                } else {
                    throw new ArgumentException(token + " was not a cached color");
                }
            } else {
                var actualToken = token;
                foreach(var ch in actualToken.ToCharArray().Reverse()) {
                    reader.PushBack(ch);
                }
                var questionableList = new StringOfItem(reader).String;
                if (questionableList.IndexOf('.') != -1) {
                    // This is a double list.
                    var doubleStreamReader = new BufferedReader(questionableList);
                    var hsv = new DoubleList(doubleStreamReader).Doubles;
                    if (hsv.Count != 3) {
                        throw new FormatException("Color has wrong number of components");
                    }
                    return new Color(new [] { (int)Math.Round(hsv[0] * 255), (int)Math.Round(hsv[1] * 255), (int)Math.Round(hsv[2] * 255) });
                } else {
                    // integer list
                    var integerStreamReader = new BufferedReader(questionableList);
                    var rgb = new IntList(integerStreamReader).Ints;
                    if (rgb.Count != 3) {
                        throw new FormatException("Color has wrong number of components");
                    }
                    return new Color(rgb.ToArray());
                }
            }
        }

        public Color GetColor(string colorName) {
            if (NamedColors.TryGetValue(colorName, out var value)) {
                return value;
            } else {
                throw new ArgumentException(colorName + " was not a cached color");
            }
        }

        public void AddNamedColor(string name, Color color) {
            NamedColors[name] = color;
        }
        public void AddNamedColor(string name, BufferedReader reader) {
            NamedColors[name] = GetColor(reader);
        }

        public void AddNamedColorDict(Dictionary<string, Color> colorMap) {
            foreach (var (key, value) in colorMap) {
                NamedColors[key] = value;
            }
        }
        public void Clear() {
            NamedColors.Clear();
        }
    }
}
