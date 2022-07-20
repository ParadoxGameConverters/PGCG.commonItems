using System;
using System.IO;
using System.Text;

namespace commonItems; 

public class GameVersion {
	public int? FirstPart { get; }
	public int? SecondPart { get; }
	public int? ThirdPart { get; }
	public int? FourthPart { get; }

	public GameVersion() { }

	public GameVersion(int? theFirstPart, int? theSecondPart, int? theThirdPart, int? theFourthPart) {
		FirstPart = theFirstPart;
		SecondPart = theSecondPart;
		ThirdPart = theThirdPart;
		FourthPart = theFourthPart;
	}

	public GameVersion(string version) {
		version = version.Trim();
		if (string.IsNullOrEmpty(version)) {
			return;
		}
		var parts = version.Split('.');

		if (parts.Length > 0) {
			FirstPart = int.Parse(parts[0]);
		} else {
			return;
		}
		if (parts.Length > 1) {
			SecondPart = int.Parse(parts[1]);
		} else {
			return;
		}
		if (parts.Length > 2) {
			ThirdPart = int.Parse(parts[2]);
		} else {
			return;
		}
		if (parts.Length > 3) {
			FourthPart = int.Parse(parts[3]);
		}
	}

	public GameVersion(BufferedReader gameVersionReader) {
		int? firstPart = null, secondPart = null, thirdPart = null, fourthPart = null;
		
		var parser = new Parser();
		parser.RegisterKeyword("first", reader => firstPart = reader.GetInt());
		parser.RegisterKeyword("second", reader => secondPart = reader.GetInt());
		parser.RegisterKeyword("third", reader => thirdPart = reader.GetInt());
		parser.RegisterKeyword("forth", reader => fourthPart = reader.GetInt());
		parser.IgnoreAndLogUnregisteredItems();
		parser.ParseStream(gameVersionReader);
		
		FirstPart = firstPart;
		SecondPart = secondPart;
		ThirdPart = thirdPart;
		FourthPart = fourthPart;
	}

	public override bool Equals(object? obj) {
		if (obj is not GameVersion rhs) {
			return false;
		}
		var testL = 0;
		var testR = 0;
		if (FirstPart is not null) {
			testL = FirstPart.Value;
		}

		if (rhs.FirstPart is not null) {
			testR = rhs.FirstPart.Value;
		}

		if (testL != testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (SecondPart is not null) {
			testL = SecondPart.Value;
		}

		if (rhs.SecondPart is not null) {
			testR = rhs.SecondPart.Value;
		}

		if (testL != testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (ThirdPart is not null) {
			testL = ThirdPart.Value;
		}

		if (rhs.ThirdPart is not null) {
			testR = rhs.ThirdPart.Value;
		}

		if (testL != testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (FourthPart is not null) {
			testL = FourthPart.Value;
		}

		if (rhs.FourthPart is not null) {
			testR = rhs.FourthPart.Value;
		}

		return testL == testR;
	}

	public override int GetHashCode() {
		return HashCode.Combine(FirstPart, SecondPart, ThirdPart, FourthPart);
	}

	public static bool operator >=(GameVersion lhs, GameVersion rhs) {
		return lhs > rhs || lhs.Equals(rhs);
	}
	public static bool operator >(GameVersion lhs, GameVersion rhs) {
		int testL = 0;
		int testR = 0;
		if (lhs.FirstPart != null) {
			testL = lhs.FirstPart.Value;
		}
		if (rhs.FirstPart != null) {
			testR = rhs.FirstPart.Value;
		}

		if (testL > testR) {
			return true;
		}
		if (testL < testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.SecondPart != null) {
			testL = lhs.SecondPart.Value;
		}

		if (rhs.SecondPart != null) {
			testR = rhs.SecondPart.Value;
		}

		if (testL > testR) {
			return true;
		}

		if (testL < testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.ThirdPart != null) {
			testL = lhs.ThirdPart.Value;
		}

		if (rhs.ThirdPart != null) {
			testR = rhs.ThirdPart.Value;
		}

		if (testL > testR) {
			return true;
		}

		if (testL < testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.FourthPart != null) {
			testL = lhs.FourthPart.Value;
		}

		if (rhs.FourthPart != null) {
			testR = rhs.FourthPart.Value;
		}

		if (testL > testR) {
			return true;
		}

		return false;
	}

	public static bool operator <(GameVersion lhs, GameVersion rhs) {
		var testL = 0;
		var testR = 0;
		if (lhs.FirstPart != null) {
			testL = lhs.FirstPart.Value;
		}

		if (rhs.FirstPart != null) {
			testR = rhs.FirstPart.Value;
		}

		if (testL < testR) {
			return true;
		}

		if (testL > testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.SecondPart != null) {
			testL = lhs.SecondPart.Value;
		}

		if (rhs.SecondPart != null) {
			testR = rhs.SecondPart.Value;
		}

		if (testL < testR) {
			return true;
		}

		if (testL > testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.ThirdPart != null) {
			testL = lhs.ThirdPart.Value;
		}

		if (rhs.ThirdPart != null) {
			testR = rhs.ThirdPart.Value;
		}

		if (testL < testR) {
			return true;
		}

		if (testL > testR) {
			return false;
		}

		testL = 0;
		testR = 0;
		if (lhs.FourthPart != null) {
			testL = lhs.FourthPart.Value;
		}

		if (rhs.FourthPart != null) {
			testR = rhs.FourthPart.Value;
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
		if (FirstPart is not null) {
			sb.Append(FirstPart.Value);
			sb.Append('.');
		} else {
			sb.Append("0.");
		}
		if (SecondPart is not null) {
			sb.Append(SecondPart.Value);
			sb.Append('.');
		} else {
			sb.Append("0.");
		}
		if (ThirdPart is not null) {
			sb.Append(ThirdPart.Value);
			sb.Append('.');
		} else {
			sb.Append("0.");
		}
		if (FourthPart is not null) {
			sb.Append(FourthPart.Value);
		} else {
			sb.Append('0');
		}
		return sb.ToString();
	}

	public string ToShortString() {
		var sb = new StringBuilder();
		if (FourthPart is not null) {
			sb.Append('.');
			sb.Append(FourthPart.Value);
		}
		if (ThirdPart is not null) {
			sb.Insert(0, ThirdPart.Value);
			sb.Insert(0, '.');
		}
		if (SecondPart is not null) {
			sb.Insert(0, SecondPart.Value);
			sb.Insert(0, '.');
		}
		if (FirstPart is not null) {
			sb.Insert(0, FirstPart.Value);
		}
		return sb.ToString();
	}

	public string ToWildCard() {
		var sb = new StringBuilder();
		if (FourthPart != null) {
			sb.Append('.');
			sb.Append(FourthPart.Value);
		} else if (ThirdPart != null) {
			sb.Append(".*");
		}

		if (ThirdPart != null) {
			sb.Insert(0, ThirdPart.Value);
			sb.Insert(0, '.');
		} else if (SecondPart != null) {
			sb.Clear();
			sb.Append(".*");
		}

		if (SecondPart != null) {
			sb.Insert(0, SecondPart.Value);
			sb.Insert(0, '.');
		} else if (FirstPart != null) {
			sb.Clear();
			sb.Append(".*");
		}

		if (FirstPart != null) {
			sb.Insert(0, FirstPart.Value);
		} else {
			sb.Clear();
			sb.Append('*');
		}

		return sb.ToString();
	}

	// Largerish is intended for fuzzy comparisons like "converter works with up to 1.9",
	// so everything incoming on rhs from 0.0.0.0 to 1.9.x.y will match, (where x and y are >= 0),
	// thus overshooting the internal "1.9.0.0" setup. This works if ".0.0" are actually undefined.
	public bool IsLargerishThan(GameVersion rhs) {
		var testDigit = 0;
		if (rhs.FirstPart is not null) {
			testDigit = rhs.FirstPart.Value;
		}

		if (FirstPart is not null) {
			if (testDigit > FirstPart.Value) {
				return false;
			}
			if (testDigit < FirstPart.Value) {
				return true;
			}
		}

		testDigit = 0;
		if (rhs.SecondPart is not null) {
			testDigit = rhs.SecondPart.Value;
		}

		if (SecondPart is not null) {
			if (testDigit > SecondPart.Value) {
				return false;
			}
			if (testDigit < SecondPart.Value) {
				return true;
			}
		}

		testDigit = 0;
		if (rhs.ThirdPart is not null) {
			testDigit = rhs.ThirdPart.Value;
		}

		if (ThirdPart is not null) {
			if (testDigit > ThirdPart.Value) {
				return false;
			}
			if (testDigit < ThirdPart.Value) {
				return true;
			}
		}

		testDigit = 0;
		if (rhs.FourthPart is not null) {
			testDigit = rhs.FourthPart.Value;
		}

		if (FourthPart is not null && testDigit > FourthPart.Value) {
			return false;
		}

		return true;
	}

	public static GameVersion? ExtractVersionFromLauncher(string filePath) {
		// use this for modern PDX games, point filePath to launcher-settings.json to get installation version.

		if (!File.Exists(filePath)) {
			Logger.Warn("Failure extracting version: " + filePath + " does not exist.");
			return null;
		}

		var result = ExtractVersionByStringFromLauncher("rawVersion", filePath);
		if (result is null) {
			// Imperator: Rome?
			result = ExtractVersionByStringFromLauncher("version", filePath);
		}
		if (result is null) {
			Logger.Warn("Failure extracting version: " + filePath + " does not contain installation version!");
			return null;
		}
		return result;
	}

	private static GameVersion? ExtractVersionByStringFromLauncher(string versionString, string filePath) {
		try {
			using StreamReader sr = File.OpenText(filePath);
			while (!sr.EndOfStream) {
				string? line = sr.ReadLine();
				if (line is null || !line.Contains(versionString, StringComparison.InvariantCulture)) {
					continue;
				}
				var pos = line.IndexOf(':');
				if (pos == -1) {
					sr.Close();
					return null;
				}

				line = line[(pos + 1)..];
				pos = line.IndexOf('\"');
				if (pos == -1) {
					sr.Close();
					return null;
				}

				line = line.Substring(pos + 1);
				pos = line.IndexOf('\"');
				if (pos == -1) {
					sr.Close();
					return null;
				}
				line = line.Substring(0, pos);

				try {
					return new GameVersion(line);
				} catch (Exception) {
					sr.Close();
					return null;
				}
			}
		} catch (Exception) {
			return null;
		}

		return null;
	}
}