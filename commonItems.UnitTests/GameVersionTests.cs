using Xunit;

namespace commonItems.UnitTests {
	public class GameVersionTests {
		private readonly string testFilesPath = "TestFiles/";
		[Fact]
		public void GameVersionDefaultsToZeroZeroZeroZero() {
			var version = new GameVersion();
			Assert.Equal("0.0.0.0", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetDirectly() {
			var version = new GameVersion(1, 2, 3, 4);
			Assert.Equal("1.2.3.4", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByFourPartString() {
			var version = new GameVersion("1.2.3.4");
			Assert.Equal("1.2.3.4", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByThreePartString() {
			var version = new GameVersion("1.2.3");
			Assert.Equal("1.2.3.0", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByTwoPartString() {
			var version = new GameVersion("1.2");
			Assert.Equal("1.2.0.0", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByOnePartString() {
			var version = new GameVersion("1");
			Assert.Equal("1.0.0.0", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByEmptyString() {
			var version = new GameVersion("");
			Assert.Equal("0.0.0.0", version.ToString());
		}
		[Fact]
		public void GameVersionCanBeSetByStream() {
			const string input = "= {\n"
			+ "\tfirst = 1\n"
			+ "\tsecond = 2\n"
			+ "\tthird = 3\n"
			+ "\tforth = 4\n" // paradox's misspelling
			+ "}";
			var reader = new BufferedReader(input);
			var version = new GameVersion(reader);
			Assert.Equal("1.2.3.4", version.ToString());
		}
		[Fact]
		public void EqualityCanBeTrue() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.Equal(version, version2);
		}
		[Fact]
		public void InequalityCanBeTrueFromFirstPart() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(2, 2, 3, 4);
			Assert.NotEqual(version, version2);
		}
		[Fact]
		public void InequalityCanBeTrueFromSecondPart() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 3, 3, 4);
			Assert.NotEqual(version, version2);
		}
		[Fact]
		public void InequalityCanBeTrueFromThirdPart() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 2, 4, 4);
			Assert.NotEqual(version, version2);
		}
		[Fact]
		public void InequalityCanBeTrueFromFourthPart() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 2, 3, 5);
			Assert.NotEqual(version, version2);
		}
		[Fact]
		public void GreaterThanCanBeSetFromFourthPart() {
			var version = new GameVersion(1, 2, 3, 5);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version > version2);
		}
		[Fact]
		public void GreaterThanCanBeSetFromThirdPart() {
			var version = new GameVersion(1, 2, 4, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version > version2);
		}
		[Fact]
		public void GreaterThanCanBeSetFromSecondPart() {
			var version = new GameVersion(1, 3, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version > version2);
		}
		[Fact]
		public void GreaterThanCanBeSetFromFirstPart() {
			var version = new GameVersion(2, 1, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version > version2);
		}
		[Fact]
		public void GreaterThanOrEqualsFromGreaterThan() {
			var version = new GameVersion(2, 1, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version >= version2);
		}
		[Fact]
		public void GreaterThanOrEqualsFromEquals() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version >= version2);
		}
		[Fact]
		public void LessThanCanBeSetFromFourthPart() {
			var version = new GameVersion(1, 2, 3, 5);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 < version);
		}
		[Fact]
		public void LessThanCanBeSetFromThirdPart() {
			var version = new GameVersion(1, 2, 4, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 < version);
		}
		[Fact]
		public void LessThanCanBeSetFromSecondPart() {
			var version = new GameVersion(1, 3, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 < version);
		}
		[Fact]
		public void LessThanCanBeSetFromFirstPart() {
			var version = new GameVersion(2, 1, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 < version);
		}
		[Fact]
		public void LessThanOrEqualsFromGreaterThan() {
			var version = new GameVersion(2, 1, 2, 3);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 <= version);
		}
		[Fact]
		public void LessThanOrEqualsFromEquals() {
			var version = new GameVersion(1, 2, 3, 4);
			var version2 = new GameVersion(1, 2, 3, 4);
			Assert.True(version2 <= version);
		}
		[Fact]
		public void GameVersionEqualMissingFourthPartIsSameAsZero() {
			var version = new GameVersion("1.3.3.0");
			var requiredVersion = new GameVersion("1.3.3");
			Assert.Equal(version, requiredVersion);
		}
		[Fact]
		public void GameVersionNotEqualMissingFourthPartIsNotSameAsThirdPart() {
			var version = new GameVersion("1.3.3.3");
			var requiredVersion = new GameVersion("1.3.3");
			Assert.NotEqual(version, requiredVersion);
		}
		[Fact]
		public void GameVersionFullNameReturned() {
			var version1 = new GameVersion("1.3.0.3");
			var version2 = new GameVersion("1.3.0");
			var version3 = new GameVersion("1.3");
			var version4 = new GameVersion("1");
			var version5 = new GameVersion("");

			Assert.Equal("1.3.0.3", version1.ToString());
			Assert.Equal("1.3.0.0", version2.ToString());
			Assert.Equal("1.3.0.0", version3.ToString());
			Assert.Equal("1.0.0.0", version4.ToString());
			Assert.Equal("0.0.0.0", version5.ToString());
		}
		[Fact]
		public void GameVersionShortNameReturned() {
			var version1 = new GameVersion("1.3.0.3");
			var version2 = new GameVersion("1.3.0");
			var version3 = new GameVersion("1.3");
			var version4 = new GameVersion("1");
			var version5 = new GameVersion("");

			Assert.Equal("1.3.0.3", version1.ToShortString());
			Assert.Equal("1.3.0", version2.ToShortString());
			Assert.Equal("1.3", version3.ToShortString());
			Assert.Equal("1", version4.ToShortString());
			Assert.Equal("", version5.ToShortString());
		}
		[Fact]
		public void GameVersionWildCardReturned() {
			var version1 = new GameVersion("1.3.0.3");
			var version2 = new GameVersion("1.3.0");
			var version3 = new GameVersion("1.3");
			var version4 = new GameVersion("1");
			var version5 = new GameVersion("");

			Assert.Equal("1.3.0.3", version1.ToWildCard());
			Assert.Equal("1.3.0.*", version2.ToWildCard());
			Assert.Equal("1.3.*", version3.ToWildCard());
			Assert.Equal("1.*", version4.ToWildCard());
			Assert.Equal("*", version5.ToWildCard());
		}

		[Fact]
		public void LargerishFalseForLarger() {
			var requiredVersion = new GameVersion("2.1.1.1");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.1.1.2")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.1.2.0")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.2.0.0")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("3.0.0.0")));

			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.1.2")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.2")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("3")));
		}

		[Fact]
		public void LargerishTrueForSmaller() {
			var requiredVersion = new GameVersion("2.1.1.1");
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.0.0.0")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.0.0")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.1.0")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("1.0.0.0")));

			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.0")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("1")));

			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("0.0.0.0")));
		}
		[Fact]
		public void LargerishTrueForOvershootingSmallerish() {
			// This is the main meat.

			var requiredVersion = new GameVersion("2.1");
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.99.0")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.99.99")));
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("2.1.1.1")));
		}

		[Fact]
		public void LargerishFalseForLargerish() {
			var requiredVersion = new GameVersion("2.1");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("2.2")));
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("3.0")));
		}

		[Fact]
		public void LargerishZeroTrueForZero() {
			var requiredVersion = new GameVersion("0.0.0.0");
			Assert.True(requiredVersion.IsLargerishThan(new GameVersion("0.0.0.0")));
		}

		[Fact]
		public void LargerishForActualIntendedZeroWithSubversions() {
			var requiredVersion = new GameVersion("1.0.9");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("1.1")));

			requiredVersion = new GameVersion("1.0.0.9");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("1.0.1")));
		}

		[Fact]
		public void LargerishForActualIntendedZeroWithoutSubversions() {
			var requiredVersion = new GameVersion("1.0");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("1.1")));

			requiredVersion = new GameVersion("1.0.0");
			Assert.False(requiredVersion.IsLargerishThan(new GameVersion("1.9.1")));
		}

		[Fact]
		public void ExtractVersionFromLauncherExtractsGameVersion() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "launcher-settings.json");
			Assert.Equal(new GameVersion("1.31.5"), version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsNullForMissingFile() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "launcher-settings.json2");
			Assert.Null(version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsNullForMissingRawVersion() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "ChangeLog.txt");
			Assert.Null(version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsNullForBrokenRawVersion() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "broken-settings.json");
			Assert.Null(version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsNullForNonsenseRawVersion() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "broken-settings2.json");
			Assert.Null(version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsVersionForChangedRawVersion() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "changed-settings.json");
			Assert.Equal(new GameVersion("1.31.5"), version);
		}

		[Fact]
		public void ExtractVersionFromLauncherReturnsVersionForRome() {
			var version = GameVersion.ExtractVersionFromLauncher(testFilesPath + "rome-settings.json");
			Assert.Equal(new GameVersion("2.0.3"), version);
		}
	}
}
