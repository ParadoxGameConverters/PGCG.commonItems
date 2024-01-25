using Fernandezja.ColorHashSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace commonItems.Colors; 

public class ColorFactory {
	public Dictionary<string, Color> NamedColors { get; } = new();

	private Color GetRgbColor(BufferedReader reader) {
		var rgb = reader.GetDoubles();
		if (rgb.Count != 3) {
			throw new FormatException($"Color has wrong number of components for RGB: " +
			                          $"{string.Join(',', rgb)}");
		}
		return new Color((int)rgb[0], (int)rgb[1], (int)rgb[2]);
	}
	private Color GetHexColor(BufferedReader reader) {
		var hex = reader.GetStrings()[0];
		if (hex.Length != 6) {
			throw new FormatException("Color has wrong number of digits");
		}
		var r = int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		var g = int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		var b = int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		return new Color(r, g, b);
	}
	private static Color GetHsvColor(BufferedReader reader) {
		var hsv = reader.GetDoubles();
		var elementsCount = hsv.Count;
		return elementsCount switch {
			3 => new Color(hsv[0], hsv[1], hsv[2]),
			4 => new Color(hsv[0], hsv[1], hsv[2], hsv[3]),
			_ => throw new FormatException("Color has wrong number of components for HSV: " +
			                               $"{string.Join(',', hsv)}")
		};
	}
	private static Color GetHsv360Color(BufferedReader reader) {
		var hsv = reader.GetDoubles();
		if (hsv.Count != 3) {
			throw new FormatException("Color has wrong number of components for HSV 360: " +
			                          $"{string.Join(',', hsv)}");
		}

		return new Color(hsv[0] / 360, hsv[1] / 100, hsv[2] / 100);
	}
	private static Color GetUnprefixedColor(BufferedReader reader) {
		var questionableList = reader.GetStringOfItem().ToString();
		if (questionableList.Contains('.')) {
			// This is a double list.
			var doubleStreamReader = new BufferedReader(questionableList);
			var rgb = doubleStreamReader.GetDoubles();
			switch (rgb.Count) {
				case 3: {
					// This is not HSV, this is RGB doubles. Multiply by 255 and round to get normal RGB.
					var r = (int)Math.Round(rgb[0] * 255);
					var g = (int)Math.Round(rgb[1] * 255);
					var b = (int)Math.Round(rgb[2] * 255);
					return new Color(r, g, b);
				}
				case 4: {
					// This is a RGBA double situation. We shouldn't touch alpha.
					var r = (int)Math.Round(rgb[0] * 255);
					var g = (int)Math.Round(rgb[1] * 255);
					var b = (int)Math.Round(rgb[2] * 255);
					var a = rgb[3];
					return new Color(r, g, b, a);
				}
				default:
					throw new FormatException("Color has wrong number of components for unprefixed color: " +
					                          $"{string.Join(',', rgb)}");
			}
		} else {
			// integer list
			var integerStreamReader = new BufferedReader(questionableList);
			var rgb = integerStreamReader.GetInts();
			return rgb.Count switch {
				3 => new Color(rgb[0], rgb[1], rgb[2]),
				4 => new Color(rgb[0], rgb[1], rgb[2], (float)rgb[3]),
				_ => throw new FormatException("Color has wrong number of components for unprefixed color: " +
				                               $"{string.Join(',', rgb)}")
			};
		}
	}
	public Color GetColor(BufferedReader reader) {
		// Remove equals if necessary.
		var token = Parser.GetNextTokenWithoutMatching(reader);
		if (token is not null && token == "=") {
			token = Parser.GetNextTokenWithoutMatching(reader);
		}
		
		if (token is null) {
			throw new FormatException("Cannot get color without token");
		}
		token = token.RemQuotes();
		switch (token) {
			case "rgb": {
				return GetRgbColor(reader);
			}
			case "hex": {
				return GetHexColor(reader);
			}
			case "hsv" or "HSV": {
				return GetHsvColor(reader);
			}
			case "hsv360": {
				return GetHsv360Color(reader);
			}
			default: {
				if (CommonRegexes.Catchall.IsMatch(token)) {
					return GetColorByName(token);
				}

				foreach (var ch in token.ToCharArray().Reverse()) {
					reader.PushBack(ch);
				}

				return GetUnprefixedColor(reader);
			}
		}
	}

	private static System.Drawing.Color? GetSystemDrawingColorByName(string colorName) {
		try {
			var color = System.Drawing.Color.FromName(colorName);
			if (!color.IsKnownColor) {
				return null;
			}
			if (color == System.Drawing.Color.Empty) {
				return null;
			}

			return color;
		} catch {
			return null;
		}
	}
	
	public Color GetColorByName(string colorName) {
		if (NamedColors.TryGetValue(colorName, out var value)) {
			return value;
		}
		
		Logger.Warn($"{colorName} was not a cached color");
		
		// Try to find a color that matches the name.
		var systemDrawingColor = GetSystemDrawingColorByName(colorName);
		if (systemDrawingColor is not null) {
			var color = new Color(systemDrawingColor.Value);
			NamedColors[colorName] = color;
			return color;
		}
		
		// Split the color name into words and try to find a color that matches each word.
		foreach (string word in colorName.Split('_')) {
			string wordToUse = word;
			if (wordToUse == "grey") {
				wordToUse = "gray";
			}
			
			var colorFromWord = GetSystemDrawingColorByName(wordToUse);
			if (colorFromWord is null) {
				continue;
			}

			var color = new Color(colorFromWord.Value);
			NamedColors[colorName] = color;
			return color;
		}
		
		// If all else fails, instead of always returning the same color, use a color hash.
		// This will at least give us a different color for each name.
		Color fallbackColor = new(new ColorHash().Rgb(colorName));
		Logger.Warn($"No matching fallback color found for {colorName}, " +
		            $"using color hash {fallbackColor.OutputRgb()}.");
		return fallbackColor;
	}

	public void AddNamedColor(string name, Color color) {
		NamedColors[name] = color;
	}
	public void AddNamedColor(string name, BufferedReader reader) {
		NamedColors[name] = GetColor(reader);
	}

	public void AddNamedColorDict(IDictionary<string, Color> colorMap) {
		foreach (var (key, value) in colorMap) {
			NamedColors[key] = value;
		}
	}
	public void Clear() {
		NamedColors.Clear();
	}
}