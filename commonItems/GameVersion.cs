using System;
using System.IO;
using System.Text;

namespace commonItems {
	public class GameVersion {
		private int? firstPart;
		private int? secondPart;
		private int? thirdPart;
		private int? fourthPart;

		public GameVersion() { }

		public GameVersion(int? theFirstPart,
			int? theSecondPart,
			int? theThirdPart,
			int? theFourthPart) {
			firstPart = theFirstPart;
			secondPart = theSecondPart;
			thirdPart = theThirdPart;
			fourthPart = theFourthPart;
		}

		public GameVersion(string version) {
			version = version.Trim();
			if (string.IsNullOrEmpty(version)) {
				return;
			}
			var parts = version.Split('.');

			if (parts.Length > 0) {
				firstPart = int.Parse(parts[0]);
			} else {
				return;
			}
			if (parts.Length > 1) {
				secondPart = int.Parse(parts[1]);
			} else {
				return;
			}
			if (parts.Length > 2) {
				thirdPart = int.Parse(parts[2]);
			} else {
				return;
			}
			if (parts.Length > 3) {
				fourthPart = int.Parse(parts[3]);
			}
		}

		public GameVersion(BufferedReader reader) {
			var parser = new Parser();
			parser.RegisterKeyword("first", reader => firstPart = reader.GetInt());
			parser.RegisterKeyword("second", reader => secondPart = reader.GetInt());
			parser.RegisterKeyword("third", reader => thirdPart = reader.GetInt());
			parser.RegisterKeyword("forth", reader => fourthPart = reader.GetInt());
			parser.RegisterRegex(CommonRegexes.Catchall, ParserHelpers.IgnoreAndLogItem);
			parser.ParseStream(reader);
			parser.ClearRegisteredRules();
		}

		public override bool Equals(object? obj) {
			if (obj is not GameVersion rhs) {
				return false;
			}
			var testL = 0;
			var testR = 0;
			if (firstPart is not null) {
				testL = firstPart.Value;
			}

			if (rhs.firstPart is not null) {
				testR = rhs.firstPart.Value;
			}

			if (testL != testR) {
				return false;
			}

			testL = 0;
			testR = 0;
			if (secondPart is not null) {
				testL = secondPart.Value;
			}

			if (rhs.secondPart is not null) {
				testR = rhs.secondPart.Value;
			}

			if (testL != testR) {
				return false;
			}

			testL = 0;
			testR = 0;
			if (thirdPart is not null) {
				testL = thirdPart.Value;
			}

			if (rhs.thirdPart is not null) {
				testR = rhs.thirdPart.Value;
			}

			if (testL != testR) {
				return false;
			}

			testL = 0;
			testR = 0;
			if (fourthPart is not null) {
				testL = fourthPart.Value;
			}

			if (rhs.fourthPart is not null) {
				testR = rhs.fourthPart.Value;
			}

			return testL == testR;
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
			if (firstPart is not null) {
				sb.Append(firstPart.Value);
				sb.Append('.');
			} else {
				sb.Append("0.");
			}
			if (secondPart is not null) {
				sb.Append(secondPart.Value);
				sb.Append('.');
			} else {
				sb.Append("0.");
			}
			if (thirdPart is not null) {
				sb.Append(thirdPart.Value);
				sb.Append('.');
			} else {
				sb.Append("0.");
			}
			if (fourthPart is not null) {
				sb.Append(fourthPart.Value);
			} else {
				sb.Append('0');
			}
			return sb.ToString();
		}

		public string ToShortString() {
			var sb = new StringBuilder();
			if (fourthPart is not null) {
				sb.Append('.');
				sb.Append(fourthPart.Value);
			}
			if (thirdPart is not null) {
				sb.Insert(0, thirdPart.Value);
				sb.Insert(0, '.');
			}
			if (secondPart is not null) {
				sb.Insert(0, secondPart.Value);
				sb.Insert(0, '.');
			}
			if (firstPart is not null) {
				sb.Insert(0, firstPart.Value);
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

		// Largerish is intended for fuzzy comparisons like "converter works with up to 1.9",
		// so everything incoming on rhs from 0.0.0.0 to 1.9.x.y will match, (where x and y are >= 0),
		// thus overshooting the internal "1.9.0.0" setup. This works if ".0.0" are actually undefined.
		public bool IsLargerishThan(GameVersion rhs) {
			var testDigit = 0;
			if (rhs.firstPart is not null) {
				testDigit = rhs.firstPart.Value;
			}

			if (firstPart is not null) {
				if (testDigit > firstPart.Value) {
					return false;
				}
				if (testDigit < firstPart.Value) {
					return true;
				}
			}

			testDigit = 0;
			if (rhs.secondPart is not null) {
				testDigit = rhs.secondPart.Value;
			}

			if (secondPart is not null) {
				if (testDigit > secondPart.Value) {
					return false;
				}
				if (testDigit < secondPart.Value) {
					return true;
				}
			}

			testDigit = 0;
			if (rhs.thirdPart is not null) {
				testDigit = rhs.thirdPart.Value;
			}

			if (thirdPart is not null) {
				if (testDigit > thirdPart.Value) {
					return false;
				}
				if (testDigit < thirdPart.Value) {
					return true;
				}
			}

			testDigit = 0;
			if (rhs.fourthPart is not null) {
				testDigit = rhs.fourthPart.Value;
			}

			if (fourthPart is not null && testDigit > fourthPart.Value) {
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
}
