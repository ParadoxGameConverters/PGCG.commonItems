using commonItems.Serialization;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace commonItems.Colors; 

/// <summary>
///  Represents a Paradox - defined color.
///  Can be directly created in either the RGB or HSV color spaces.
///  If alpha is present, we're assuming it's RGBA or HSVA.
///
/// <code>
///  Can be imported in:
///  * Unspecified with ints (becomes RGB) - "= { 64 128 128 }"
///  * Unspecified with three floats (becomes RGB) - "= { 0.5 0.9 0.1 }"
///  * Unspecified with four floats (becomes RGBA) - "= { 0.5 0.9 0.1 0.1 }"
///  * RGB - "= rgb { 64 128 128 }"
///  * Hex - "= hex { 408080 }"
///  * HSV - "= hsv { 0.5 0.5 0.5 }"
///  * HSVA - "= hsv { 0.5 0.5 0.5 0.1 }"
///  * HSV360 - "= hsv360 { 180 50 50 }"
///  * Name (requires caching definitions for the named colors in advance) - "= dark_moderate_cyan"
/// </code>
/// <code>
///  Can be output in:
///  * unspecified (RGB) - "= { 64 128 128 }"
///  * RGB - "= rgb { 64 128 128 }"
///  * RGBA - we don't export RGBA. yet.
///  * hex - "= hex { 408080 }"
///  * HSV - "= hsv { 0.5 0.5 0.5 }"
///  * HSVA - "= hsv { 0.5 0.5 0.5 0.1 }"
///  * HSV360 - "= hsv360 { 180 50 50 }"
///  </code>
///  
///  The individual components can be accessed in both RGB and HSV color spaces,
///  equality and inequality can be checked, the color cache can be reviewed and modified,
///  and colors can have a random fluctuation be applied automatically.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Color : IPDXSerializable, IEquatable<Color> {
	public Color() { }
	public Color(int r, int g, int b) {
		if (r is < 0 or > 255) {
			Logger.Warn($"RGB color component R out of bounds: {r}. Clamping to 0-255.");
			r = Math.Clamp(r, 0, 255);
		}
		if (g is < 0 or > 255) {
			Logger.Warn($"RGB color component G out of bounds: {g}. Clamping to 0-255.");
			g = Math.Clamp(g, 0, 255);
		}
		if (b is < 0 or > 255) {
			Logger.Warn($"RGB color component B out of bounds: {b}. Clamping to 0-255.");
			b = Math.Clamp(b, 0, 255);
		}
		
		R = (byte)r;
		G = (byte)g;
		B = (byte)b;
		
		var (h, s, v) = DeriveHsvFromRgb();
		H = h;
		S = s;
		V = v;
	}

	public Color(int r, int g, int b, float alpha) : this(r, g, b) {
		a = (byte)Math.Round(alpha * 255);
	}
	public Color(double h, double s, double v) {
		H = (float)h;
		S = (float)s;
		V = (float)v;
		
		var (r, g, b) = DeriveRgbFromHsv();
		R = r;
		G = g;
		B = b;
	}

	public Color(double h, double s, double v, float alpha) : this(h, s, v) {
		a = (byte)Math.Round(alpha * 255);
	}
	
	public Color(System.Drawing.Color color) {
		R = color.R;
		G = color.G;
		B = color.B;
		a = color.A;
		
		var (h, s, v) = DeriveHsvFromRgb();
		H = h;
		S = s;
		V = v;
	}

	public override bool Equals(object? obj) {
		return obj is Color color && R == color.R && G == color.G && B == color.B && a == color.a;
	}

	public override int GetHashCode() {
		return HashCode.Combine(R, G, B, a);
	}

	public byte R { get; } = 0;
	public byte G { get; } = 0;
	public byte B { get; } = 0;
	

	private readonly byte a = 255;
	public float A => (float)a / 255;

	public float H { get; } = 0;
	public float S { get; } = 0;
	public float V { get; } = 0;

	public string Serialize(string indent, bool withBraces) {
		return Output();
	}

	public string Output() {
		var sb = new StringBuilder("{ ");
		sb.Append(R);
		sb.Append(' ');
		sb.Append(G);
		sb.Append(' ');
		sb.Append(B);
		sb.Append(" }");
		return sb.ToString();
	}

	public string OutputRgb() {
		var sb = new StringBuilder("rgb { ");
		sb.Append(R);
		sb.Append(' ');
		sb.Append(G);
		sb.Append(' ');
		sb.Append(B);
		sb.Append(" }");
		return sb.ToString();
	}

	public string OutputHex() {
		var sb = new StringBuilder("hex { ");
		sb.Append($"{R:X2}{G:X2}{B:X2}");
		sb.Append(" }");
		return sb.ToString();
	}

	public string OutputHsv() {
		var cultureInfo = CultureInfo.InvariantCulture;
		var sb = new StringBuilder("hsv { ");
		sb.Append(H.ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
		sb.Append(' ');
		sb.Append(S.ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
		sb.Append(' ');
		sb.Append(V.ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
		if (a != 255) {
			sb.Append(' ');
			sb.Append(A.ToString("0.00", cultureInfo).TrimEnd('0').TrimEnd('.'));
		}
		sb.Append(" }");
		return sb.ToString();
	}

	public string OutputHsv360() {
		var sb = new StringBuilder("hsv360 { ");
		sb.Append(Math.Round(H * 360));
		sb.Append(' ');
		sb.Append(Math.Round(S * 100));
		sb.Append(' ');
		sb.Append(Math.Round(V * 100));
		sb.Append(" }");
		return sb.ToString();
	}

	private Tuple<float, float, float> DeriveHsvFromRgb() {
		var r = (double)R / 255;
		var g = (double)G / 255;
		var b = (double)B / 255;
		var xMax = new[] { r, g, b }.Max();
		var xMin = new[] { r, g, b }.Min();
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
		
		return new(
			(float)h,
			xMax == 0.0f ? 0f : (float)(chroma / xMax),
			(float)xMax
		);
	}

	private Tuple<byte, byte, byte> DeriveRgbFromHsv() {
		var h = (double)H;
		var s = (double)S;
		var v = (double)V;

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
		
		return new(
			(byte)Math.Clamp((int)r, 0, 255),
			(byte)Math.Clamp((int)g, 0, 255),
			(byte)Math.Clamp((int)b, 0, 255)
		);
	}

	public override string ToString() => OutputRgb();
	public static bool operator ==(Color left, Color right) {
		return left.Equals(right);
	}

	public static bool operator !=(Color left, Color right) {
		return !(left == right);
	}

	public bool Equals(Color other) {
		return R == other.R && G == other.G && B == other.B && a == other.a;
	}
}