using commonItems.Localization;
using System.Collections.Generic;
using Xunit;

namespace commonItems.UnitTests.Localization {
	public class LocalizationTests {
		[Fact]
		public void LocalizationCanBeLoadedAndMatched() {
			var reader1 = new BufferedReader(
				"l_english:\n" +
				" key1:0 \"value 1\" # comment\n" +
				" key2:0 \"value \"subquoted\" 2\"\n"
			);
			var reader2 = new BufferedReader(
				"l_french:\n" +
				" key1:0 \"valuee 1\"\n" +
				" key2:0 \"valuee \"subquoted\" 2\"\n"
			);
			var reader3 = new BufferedReader(
				"l_english:\n" +
				" key1:0 \"replaced value 1\"\n"
			);

			var locDB = new LocDB("english", "french");
			locDB.ScrapeStream(reader1);
			locDB.ScrapeStream(reader2);
			locDB.ScrapeStream(reader3);

			Assert.Equal("replaced value 1", locDB.GetLocBlockForKey("key1")!["english"]);
			Assert.Equal("value \"subquoted\" 2", locDB.GetLocBlockForKey("key2")!["english"]);
			Assert.Equal("valuee 1", locDB.GetLocBlockForKey("key1")!["french"]);
			Assert.Equal("valuee \"subquoted\" 2", locDB.GetLocBlockForKey("key2")!["french"]);
		}

		[Fact]
		public void UnquotedLocIsIgnored() {
			var reader = new BufferedReader(
				"l_english:\n" +
				" key1:0 unquotedValue"
			);
			var locDB = new LocDB("english");
			locDB.ScrapeStream(reader);
			Assert.Null(locDB.GetLocBlockForKey("key1"));
		}

		[Fact]
		public void LocUnseparatedFromKeyIsIgnored() {
			var reader = new BufferedReader(
				"l_english:\n" +
				" key1 \"loc\""
			);
			var locDB = new LocDB("english");
			locDB.ScrapeStream(reader);
			Assert.Null(locDB.GetLocBlockForKey("key1"));
		}

		[Fact]
		public void CommentLinesAreIgnored() {
			var reader = new BufferedReader(
				"l_english:\n" +
				"#key1: \"loc\""
			);
			var locDB = new LocDB("english");
			locDB.ScrapeStream(reader);
			Assert.Null(locDB.GetLocBlockForKey("key1"));
		}

		[Fact]
		public void LocDBReturnNullForMissingKey() {
			var locDB = new LocDB("english");
			Assert.Null(locDB.GetLocBlockForKey("key1"));
		}

		[Fact]
		public void LocDBReturnsEnglishForMissingLanguage() {
			var locDB = new LocDB("english", "french");
			var reader = new BufferedReader(
				"l_english:\n" +
				" key1:1 \"value 1\" # comment\n"
			);
			locDB.ScrapeStream(reader);

			Assert.Equal("value 1", locDB.GetLocBlockForKey("key1")!["french"]);
		}

		[Fact]
		public void LocCanBeModifiedByMethodForEveryLanguage() {
			var nameLocBlock = new LocBlock("english", "french", "german", "russian", "simp_chinese", "spanish") {
				["english"] = "$ADJ$ Revolt",
				["french"] = "$ADJ$ révolte",
				["german"] = "$ADJ$ Revolte",
				["russian"] = "$ADJ$ бунт",
				["simp_chinese"] = "$ADJ$ 反叛",
				["spanish"] = "$ADJ$ revuelta"
			};

			var adjLocBlock = new LocBlock("english", "french", "german", "russian", "simp_chinese", "spanish") {
				["english"] = "Roman",
				["french"] = "Romain",
				["german"] = "römisch",
				["russian"] = "Роман",
				["simp_chinese"] = "罗马",
				["spanish"] = "Romana"
			};

			nameLocBlock.ModifyForEveryLanguage(adjLocBlock, (baseLoc, modifyingLoc) =>
				baseLoc.Replace("$ADJ$", modifyingLoc)
			);
			Assert.Equal("Roman Revolt", nameLocBlock["english"]);
			Assert.Equal("Romain révolte", nameLocBlock["french"]);
			Assert.Equal("römisch Revolte", nameLocBlock["german"]);
			Assert.Equal("Роман бунт", nameLocBlock["russian"]);
			Assert.Equal("罗马 反叛", nameLocBlock["simp_chinese"]);
			Assert.Equal("Romana revuelta", nameLocBlock["spanish"]);
		}

		[Fact]
		public void LocalizationCanBeAddedForKey() {
			var locDB = new LocDB("english", "french", "german", "russian", "simp_chinese", "spanish");
			Assert.Null(locDB.GetLocBlockForKey("key1"));
			var newLocBlock = locDB.AddLocBlock("key1");
			newLocBlock["english"] = "Roman";
			newLocBlock["french"] = "Romain";

			var locBlock = locDB.GetLocBlockForKey("key1");
			Assert.Equal("Roman", locBlock["english"]);
			Assert.Equal("Romain", locBlock["french"]);
			Assert.Equal("Roman", locBlock["german"]);
			Assert.Equal("Roman", locBlock["russian"]);
			Assert.Equal("Roman", locBlock["simp_chinese"]);
			Assert.Equal("Roman", locBlock["spanish"]);
		}

		[Fact]
		public void LocBlockCanBeCopyConstructed() {
			var origLocBlock = new LocBlock("english", "french", "german", "russian", "simp_chinese", "spanish");
			origLocBlock["english"] = "a";
			origLocBlock["french"] = "b";
			origLocBlock["german"] = "c";
			origLocBlock["russian"] = "d";
			origLocBlock["simp_chinese"] = "e";
			origLocBlock["spanish"] = "f";

			var copyLocBlock = new LocBlock(origLocBlock);
			Assert.Equal("a", copyLocBlock["english"]);
			Assert.Equal("b", copyLocBlock["french"]);
			Assert.Equal("c", copyLocBlock["german"]);
			Assert.Equal("d", copyLocBlock["russian"]);
			Assert.Equal("e", copyLocBlock["simp_chinese"]);
			Assert.Equal("f", copyLocBlock["spanish"]);
		}

		[Fact]
		public void VanillaAndMocLocIsScraped() {
			var locDB = new LocDB("english", "french");
			var mods = new List<Mod> { new Mod("themod", "TestFiles/mod/themod") };
			locDB.ScrapeLocalizations("TestFiles/CK3", mods);

			Assert.Equal("value1 modded", locDB.GetLocBlockForKey("KEY1")!["english"]);
			Assert.Equal("value2", locDB.GetLocBlockForKey("KEY2")!["english"]);
			Assert.Equal("mod loc", locDB.GetLocBlockForKey("MOD_KEY1")!["english"]);
		}
	}
}
