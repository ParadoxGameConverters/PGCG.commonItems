using commonItems.Serialization;
using System;
using System.Linq;
using System.Text;

namespace commonItems {
	public class Color : IPDXSerializable {
		public Color() { }
		public Color(int[] rgbComponents) {
			RgbComponents = rgbComponents;
			DeriveHsvFromRgb();
		}
		public Color(double[] hsvComponents) {
			HsvComponents = hsvComponents;
			DeriveRgbFromHsv();
		}

		public override bool Equals(object? obj) {
			return obj is Color color && RgbComponents.SequenceEqual(color.RgbComponents);
		}

		public override int GetHashCode() {
			return (R * 1000000) + (G * 1000) + B;
		}

		public int R => RgbComponents[0];
		public int G => RgbComponents[1];
		public int B => RgbComponents[2];

		public double H => HsvComponents[0];
		public double S => HsvComponents[1];
		public double V => HsvComponents[2];

		public string Serialize() {
			return Output();
		}

		public string Output() {
			var sb = new StringBuilder("{ ");
			sb.Append(RgbComponents[0]);
			sb.Append(' ');
			sb.Append(RgbComponents[1]);
			sb.Append(' ');
			sb.Append(RgbComponents[2]);
			sb.Append(" }");
			return sb.ToString();
		}

		public string OutputRgb() {
			var sb = new StringBuilder("rgb { ");
			sb.Append(RgbComponents[0]);
			sb.Append(' ');
			sb.Append(RgbComponents[1]);
			sb.Append(' ');
			sb.Append(RgbComponents[2]);
			sb.Append(" }");
			return sb.ToString();
		}

		public string OutputHex() {
			var sb = new StringBuilder("hex { ");
			sb.Append($"{RgbComponents[0]:X2}{RgbComponents[1]:X2}{RgbComponents[2]:X2}");
			sb.Append(" }");
			return sb.ToString();
		}

		public string OutputHsv() {
			var cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
			var sb = new StringBuilder("hsv { ");
			sb.Append(HsvComponents[0].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
			sb.Append(' ');
			sb.Append(HsvComponents[1].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
			sb.Append(' ');
			sb.Append(HsvComponents[2].ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
			sb.Append(" }");
			return sb.ToString();
		}

		public string OutputHsv360() {
			var sb = new StringBuilder("hsv360 { ");
			sb.Append(Math.Round(HsvComponents[0] * 360));
			sb.Append(' ');
			sb.Append(Math.Round(HsvComponents[1] * 100));
			sb.Append(' ');
			sb.Append(Math.Round(HsvComponents[2] * 100));
			sb.Append(" }");
			return sb.ToString();
		}

		private void DeriveHsvFromRgb() {
			var r = (double)RgbComponents[0] / 255;
			var g = (double)RgbComponents[1] / 255;
			var b = (double)RgbComponents[2] / 255;
			var xMax = new[] { r, g, b }.Max();
			var xMin = new[] { r, g, b, }.Min();
			var chroma = xMax - xMin;

			double h = 0;
			if (chroma == 0) {
				h = 0;
			} else if (xMax == r) {
				h = (g - b) / chroma;
			} else if (xMax == g) {
				h = (b - r) / chroma;
				h += 2;
			} else if (xMax == b) {
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

		private void DeriveRgbFromHsv() {
			var h = HsvComponents[0];
			var s = HsvComponents[1];
			var v = HsvComponents[2];

			double r, g, b;
			if (s == 0.0f) { // achromatic (grey)
				r = g = b = v;
			} else {
				if (h >= 1.0f) {
					h = 0.0f;
				}
				int sector = (int)Math.Floor(h * 6.0f);
				double fraction = (h * 6.0f) - sector;
				double p = v * (1 - s);
				double q = v * (1 - (s * fraction));
				double t = v * (1 - (s * (1 - fraction)));
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
						throw new ArgumentOutOfRangeException(nameof(h), "Hue greater than 1.0");
				}
			}

			r *= 255;
			g *= 255;
			b *= 255;

			RgbComponents = new[] { (int)r, (int)g, (int)b };
		}

		public int[] RgbComponents { get; private set; } = {
			0, 0, 0
		};

		public double[] HsvComponents { get; } = {
			0, 0, 0
		};
	}
}
