using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Text.RegularExpressions;

namespace commonItems {
    class Color
    {
        public Color() {}
        public Color(int[] rgbComponents) {
            RgbComponents = rgbComponents;
            DeriveHsvFromRgb();
        }
        public Color(double[] hsvComponents) {
            HsvComponents = hsvComponents;
            DeriveRgbFromHsv();
        }

        public static bool operator ==(Color lhs, Color rhs) {
            return lhs.RgbComponents == rhs.RgbComponents;
        }
        public static bool operator !=(Color lhs, Color rhs) {
            return !(lhs == rhs);
        }

        public int R => RgbComponents[0];
        public int G => RgbComponents[1];
        public int B => RgbComponents[2];

        public double H => HsvComponents[0];
        public double S => HsvComponents[1];
        public double V => HsvComponents[2];

        public string Output() {
            var sb = new StringBuilder("= { ");
            sb.Append(RgbComponents[0]);
            sb.Append(' ');
            sb.Append(RgbComponents[1]);
            sb.Append(' ');
            sb.Append(RgbComponents[2]);
            sb.Append(" }");
            return sb.ToString();
        }

        public string OutputRgb()
        {
            var sb = new StringBuilder("= rgb { ");
            sb.Append(RgbComponents[0]);
            sb.Append(' ');
            sb.Append(RgbComponents[1]);
            sb.Append(' ');
            sb.Append(RgbComponents[2]);
            sb.Append(" }");
            return sb.ToString();
        }

        public string OutputHex()
        {
            var sb = new StringBuilder("= hex { ");
            sb.Append($"{RgbComponents[0]:X2}{RgbComponents[1]:X2}{RgbComponents[2]:X2}");
            sb.Append(" }");
            return sb.ToString();
        }

        public string OutputHsv() {
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            var sb = new StringBuilder("= hsv { ");
            sb.Append(HsvComponents[0].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(' ');
            sb.Append(HsvComponents[1].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(' ');
            sb.Append(HsvComponents[2].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(" }");
            return sb.ToString();
        }

        public string OutputHsv360() {
            var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
            var sb = new StringBuilder("= hsv360 { ");
            sb.Append((HsvComponents[0] * 360).ToString("0.000", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(' ');
            sb.Append((HsvComponents[1] * 100).ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(' ');
            sb.Append((HsvComponents[2] * 100).ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
            sb.Append(" }");
            return sb.ToString();
        }

        void DeriveHsvFromRgb()
        {
            var r = (double) RgbComponents[0] / 255;
            var g = (double)RgbComponents[1] / 255;
            var b = (double)RgbComponents[2] / 255;
            var xMax = new[] {r, g, b}.Max();
            var xMin = new[] {r, g, b,}.Min();
            var chroma = xMax - xMin;

            double h = 0;
            if (chroma == 0) {
                h = 0;
            }
            else if (xMax == r) {
                h = (g - b) / chroma;
            }
            else if (xMax == g) {
                h = (b - r) / chroma;
                h += 2;
            }
            else if (xMax == b) {
                h = (r - g) / chroma;
                h += 4;
            }
            h /= 6.0f;
            if (h < 0) {
                h += 1.0f;
            }
            HsvComponents[0] = h;

            if (xMax == 0.0f) {
                HsvComponents[1] = 0.0f;
            } else {
                HsvComponents[1] = chroma / xMax;
            }
            HsvComponents[2] = xMax;
        }

        void DeriveRgbFromHsv() {
            var h = HsvComponents[0];
            var s = HsvComponents[1];
            var v = HsvComponents[2];

            double r, g, b;
            if (s == 0.0f) // achromatic (grey)
    {
                r = g = b = v;
            } else {
                if (h >= 1.0f)
                    h = 0.0f;
                int sector = (int)Math.Floor(h * 6.0f);
                double fraction = h * 6.0f - sector;
                double p = v * (1 - s);
                double q = v * (1 - s * fraction);
                double t = v * (1 - s * (1 - fraction));
                switch (sector) {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;
                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;
                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;
                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;
                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = p;
                        b = q;
                        break;
                    default:
                        throw new Exception("Hue greater than 1.0");
                }
            }

            r *= 255;
            g *= 255;
            b *= 255;

            RgbComponents = new int[]{ (int)r, (int)g, (int)b };
        }

        public int[] RgbComponents { get; private set; } = {
            0, 0, 0
        };

        public double[] HsvComponents { get; private set; } = {
            0, 0, 0
        };
    }

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
                return new Color(new int[] { r, g, b });
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
                if (NamedColors.TryGetValue(token, out Color value)) {
                    return value;
                } else {
                    throw new Exception(token + " was not a cached color");
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
                    return new Color(new int[] { (int)Math.Round(hsv[0] * 255), (int)Math.Round(hsv[1] * 255), (int)Math.Round(hsv[2] * 255) });
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
            if (NamedColors.TryGetValue(colorName, out Color value)) {
                return value;
            } else {
                throw new Exception(colorName + " was not a cached color");
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
