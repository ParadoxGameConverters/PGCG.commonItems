using System;
using System.IO;
using System.Linq;
using System.Text;

namespace commonItems; 

public readonly struct GameVersion : IEquatable<GameVersion> {
	public int? FirstPart { get; }
	public int? SecondPart { get; }
	public int? ThirdPart { get; }
	public int? FourthPart { get; }

	public GameVersion(int? theFirstPart, int? theSecondPart, int? theThirdPart, int? theFourthPart) {
		FirstPart = theFirstPart;
		SecondPart = theSecondPart;
		ThirdPart = theThirdPart;
		FourthPart = theFourthPart;
	}

	public GameVersion(string version) {
		var span = version.AsSpan().Trim();
		if (span.IsEmpty) {
			return;
		}

		var partIndex = 0;
		var segmentStart = 0;
		for (var i = 0; i <= span.Length; ++i) {
			if (i < span.Length && span[i] != '.') {
				continue;
			}

			var segment = span[segmentStart..i];
			int? parsedPart = segment is ['*'] ? null : int.Parse(segment);
			switch (partIndex) {
				case 0:
					FirstPart = parsedPart;
					break;
				case 1:
					SecondPart = parsedPart;
					break;
				case 2:
					ThirdPart = parsedPart;
					break;
				case 3:
					FourthPart = parsedPart;
					break;
			}

			partIndex++;
			if (partIndex > 3) {
				break;
			}

			segmentStart = i + 1;
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

	public bool Equals(GameVersion other) {
		return FirstPart.GetValueOrDefault() == other.FirstPart.GetValueOrDefault()
		       && SecondPart.GetValueOrDefault() == other.SecondPart.GetValueOrDefault()
		       && ThirdPart.GetValueOrDefault() == other.ThirdPart.GetValueOrDefault()
		       && FourthPart.GetValueOrDefault() == other.FourthPart.GetValueOrDefault();
	}

	public override bool Equals(object? obj) {
		if (obj is not GameVersion other) {
			return false;
		}
		return Equals(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(FirstPart, SecondPart, ThirdPart, FourthPart);
	}

	public static bool operator ==(GameVersion lhs, GameVersion rhs) => lhs.Equals(rhs);
	public static bool operator !=(GameVersion lhs, GameVersion rhs) => !lhs.Equals(rhs);

	public static bool operator >=(GameVersion lhs, GameVersion rhs) {
		return lhs > rhs || lhs.Equals(rhs);
	}
	public static bool operator >(GameVersion lhs, GameVersion rhs) {
		int testL = lhs.FirstPart.GetValueOrDefault();
		int testR = rhs.FirstPart.GetValueOrDefault();
		if (testL != testR) {
			return testL > testR;
		}

		testL = lhs.SecondPart.GetValueOrDefault();
		testR = rhs.SecondPart.GetValueOrDefault();
		if (testL != testR) {
			return testL > testR;
		}

		testL = lhs.ThirdPart.GetValueOrDefault();
		testR = rhs.ThirdPart.GetValueOrDefault();
		if (testL != testR) {
			return testL > testR;
		}

		testL = lhs.FourthPart.GetValueOrDefault();
		testR = rhs.FourthPart.GetValueOrDefault();
		return testL > testR;
	}

	public static bool operator <(GameVersion lhs, GameVersion rhs) => !(lhs > rhs) && !lhs.Equals(rhs);

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
		var sb = new StringBuilder(16);
		if (FirstPart is not null) {
			sb.Append(FirstPart.Value);
		}
		if (SecondPart is not null) {
			sb.Append('.');
			sb.Append(SecondPart.Value);
		}
		if (ThirdPart is not null) {
			sb.Append('.');
			sb.Append(ThirdPart.Value);
		}
		if (FourthPart is not null) {
			sb.Append('.');
			sb.Append(FourthPart.Value);
		}
		return sb.ToString();
	}

	public string ToWildCard() {
		if (FirstPart is null) {
			return "*";
		}

		var sb = new StringBuilder(16);
		sb.Append(FirstPart.Value);
		if (SecondPart is null) {
			sb.Append(".*");
			return sb.ToString();
		}

		sb.Append('.');
		sb.Append(SecondPart.Value);
		if (ThirdPart is null) {
			sb.Append(".*");
			return sb.ToString();
		}

		sb.Append('.');
		sb.Append(ThirdPart.Value);
		if (FourthPart is null) {
			sb.Append(".*");
			return sb.ToString();
		}

		sb.Append('.');
		sb.Append(FourthPart.Value);
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

	public static bool IsModCompatibleWithGame(GameVersion modSupportedVersion, GameVersion installedGameVersion) {
		// for cases like 1.2.3 vs 1.2.3
		if (modSupportedVersion.Equals(installedGameVersion)) {
			return true;
		}
		
		// for cases like 1.2 vs 1.1
		bool modLargerish = modSupportedVersion.IsLargerishThan(installedGameVersion);
		bool gameLargerish = installedGameVersion.IsLargerishThan(modSupportedVersion);
		if (modSupportedVersion > installedGameVersion && modLargerish  && !gameLargerish) {
			return false;
		}
		
		// for cases like 1.1 vs 1.2
		if (installedGameVersion > modSupportedVersion && gameLargerish && !modLargerish) {
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
				line = line[..pos];
				
				if (!string.IsNullOrEmpty(line) && line[0] == 'v') {
					line = line[1..];
				}

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