using System.IO;
using Xunit;

namespace commonItems.UnitTests {
    [Collection("Sequential")]
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class ConverterVersionTests {
        private const string versionFilePath = "UnitTests/TestFiles/version.txt";

        [Fact] public void ItemsDefaultToEmpty() {
            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion("emptyVersion.txt");

            Assert.True(string.IsNullOrEmpty(converterVersion.Name));
            Assert.True(string.IsNullOrEmpty(converterVersion.Version));
            Assert.True(string.IsNullOrEmpty(converterVersion.Source));
            Assert.True(string.IsNullOrEmpty(converterVersion.Target));
            Assert.Equal(new GameVersion(), converterVersion.MinSource);
            Assert.Equal(new GameVersion(), converterVersion.MaxSource);
            Assert.Equal(new GameVersion(), converterVersion.MinTarget);
            Assert.Equal(new GameVersion(), converterVersion.MaxTarget);
        }
        [Fact]
        public void ItemsCanBeImported() {
            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion(versionFilePath);

            Assert.Equal("Adams-prerelease", converterVersion.Name);
            Assert.Equal("0.1A", converterVersion.Version);
            Assert.Equal("EU4", converterVersion.Source);
            Assert.Equal("Vic3", converterVersion.Target);
            Assert.Equal(new GameVersion("1.31"), converterVersion.MinSource);
            Assert.Equal(new GameVersion("1.31.7"), converterVersion.MaxSource);
            Assert.Equal(new GameVersion("1.0"), converterVersion.MinTarget);
            Assert.Equal(new GameVersion("1.1"), converterVersion.MaxTarget);
        }
        [Fact] public void DescriptionCanBeConstructed() {
            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion(versionFilePath);
            Assert.Equal("Compatible with EU4 [v1.31-v1.31.7] and Vic3 [v1.0-v1.1]", converterVersion.GetDescription());
        }
        [Fact]
        public void DescriptionDoesNotDuplicateIdenticalVersions() {
            var input = new BufferedReader("source = \"EU4\"\n"
                + "target = \"Vic3\"\n"
                + "minSource = \"1.31\"\n"
                + "maxSource = \"1.31\"\n"
                + "minTarget = \"1.0\"\n"
                + "maxTarget = \"1.0\"\n");
            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion(input);
            Assert.Equal("Compatible with EU4 [v1.31] and Vic3 [v1.0]", converterVersion.GetDescription());
        }
        [Fact] public void ConverterVersionCanBeOutput() {
            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion(versionFilePath);

            var actualOutput = new StringReader(converterVersion.ToString());
            var expectedOutput = new StringReader("\n\n"
                + "************ -= The Paradox Game Converters Group =- *****************\n"
                + "* Converter version 0.1A \"Adams-prerelease\"\n"
                + "* Compatible with EU4 [v1.31-v1.31.7] and Vic3 [v1.0-v1.1]\n"
                + "* Built on [some timestamp]\n"
                + "************************** + EU4 To Vic3 + ***************************\n");

            for (var counter = 0; counter < 5; ++counter) { // first 5 lines
                Assert.Equal(expectedOutput.ReadLine(), actualOutput.ReadLine());
            }
            var expected = expectedOutput.ReadLine();
            var actual = actualOutput.ReadLine();
            Assert.NotEqual(expected, actual); // Can't match timestamps between build and tests.

            expected = expectedOutput.ReadLine();
            actual = actualOutput.ReadLine();
            Assert.Equal(expected, actual); // footer line
        }
        [Fact]
        public void ConverterVersionOutputSkipsIncompleteVersionOrName() {
            var reader = new BufferedReader("source = \"EU4\"\n"
                + "target = \"Vic3\"\n"
                + "minSource = \"1.31\"\n"
                + "maxSource = \"1.31\"\n"
                + "minTarget = \"1.0\"\n"
                + "maxTarget = \"1.0\"\n");

            var converterVersion = new ConverterVersion();
            converterVersion.LoadVersion(reader);

            var actualOutput = new StringReader(converterVersion.ToString());
            var expectedOutput = new StringReader("\n\n"
                + "************ -= The Paradox Game Converters Group =- *****************\n"
                + "* Compatible with EU4 [v1.31] and Vic3 [v1.0]\n"
                + "* Built on [some timestamp]\n"
                + "************************** + EU4 To Vic3 + ***************************\n");

            for (var counter = 0; counter < 4; ++counter) { // first 4 lines
                Assert.Equal(expectedOutput.ReadLine(), actualOutput.ReadLine());
            }
            var expected = expectedOutput.ReadLine();
            var actual = actualOutput.ReadLine();
            Assert.NotEqual(expected, actual); // Can't match timestamps between build and tests.

            expected = expectedOutput.ReadLine();
            actual = actualOutput.ReadLine();
            Assert.Equal(expected, actual); // footer line
        }
    }
}
