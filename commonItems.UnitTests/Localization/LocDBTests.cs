using commonItems.Localization;
using commonItems.Mods;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace commonItems.UnitTests.Localization; 

public class LocDBTests {
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
	private void LocalizationWithNonStandardIndentIsCorrectlyRead() {
		var noIndentReader = new BufferedReader(
			"l_english:\n" +
			"key1:0 \"value 1\" # comment\n" +
			"key2:0 \"value \"subquoted\" 2\"\n"
		);
		var locDB = new LocDB("english");
		locDB.ScrapeStream(noIndentReader);
		Assert.Equal("value 1", locDB.GetLocBlockForKey("key1")!["english"]);
		Assert.Equal("value \"subquoted\" 2", locDB.GetLocBlockForKey("key2")!["english"]);
		
		var bigIndentReader = new BufferedReader(
			"l_english:\n" +
			"    key1:0 \"value 1\" # comment\n" +
			"    key2:0 \"value \"subquoted\" 2\"\n"
		);
		locDB = new LocDB("english");
		locDB.ScrapeStream(bigIndentReader);
		Assert.Equal("value 1", locDB.GetLocBlockForKey("key1")!["english"]);
		Assert.Equal("value \"subquoted\" 2", locDB.GetLocBlockForKey("key2")!["english"]);
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
	public void LocDBReturnsNullForMissingKey() {
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
	public void LocalizationCanBeAddedForKey() {
		var locDB = new LocDB("english", "french", "german", "russian", "simp_chinese", "spanish");
		Assert.Null(locDB.GetLocBlockForKey("key1"));
		var newLocBlock = locDB.AddLocBlock("key1");
		newLocBlock["english"] = "Roman";
		newLocBlock["french"] = "Romain";

		var locBlock = locDB.GetLocBlockForKey("key1");
		Assert.NotNull(locBlock);
		Assert.Equal("Roman", locBlock["english"]);
		Assert.Equal("Romain", locBlock["french"]);
	}

	[Fact]
	public void UnsupportedLanguageDoesNotCauseWarningsAboutUnspecifiedLanguage() {
		var output = new StringWriter();
		Console.SetOut(output);

		var locDB = new LocDB("english", "french");
		var reader = new BufferedReader(
			"l_korean:\n" +
			" key1:1 \"value 1\" # comment\n"
		);
		locDB.ScrapeStream(reader);
		
		Assert.DoesNotContain("without language specified!", output.ToString());
		Assert.Null(locDB.GetLocBlockForKey("key1"));
	}

	[Fact]
	public void VanillaAndModLocIsScraped() {
		var output = new StringWriter();
		Console.SetOut(output);

		var locDB = new LocDB("english", "french", "spanish");
		var mods = new List<Mod> { new Mod("themod", "TestFiles/mod/themod") };
		var modFS = new ModFilesystem("TestFiles/CK3/game", mods);
		locDB.ScrapeLocalizations(modFS);

		Assert.Equal("value1 modded", locDB.GetLocBlockForKey("KEY1")!["english"]);
		Assert.Equal("valeur1", locDB.GetLocBlockForKey("KEY1")!["french"]);
		Assert.Equal("value2", locDB.GetLocBlockForKey("KEY2")!["english"]);
		Assert.Equal("valeur2", locDB.GetLocBlockForKey("KEY2")!["french"]);
		Assert.Equal("mod loc", locDB.GetLocBlockForKey("MOD_KEY1")!["english"]);

		Assert.Contains("Scraping loc line [ NO_LANGUAGE_KEY1:0 \"loc w/o language\"] without language specified!", output.ToString());
		Assert.Equal("valor", locDB.GetLocBlockForKey("KEY1")!["spanish"]);
	}

	[Fact]
	public void NonYmlFilesAreNotScrapedForLocalization() {
		var locDB = new LocDB("english", "french", "spanish");
		var modFS = new ModFilesystem("TestFiles/CK3/game", new List<Mod>());
		locDB.ScrapeLocalizations(modFS);
		
		Assert.Null(locDB.GetLocBlockForKey("NON_LOC_KEY"));
	}
	
	[Fact]
	public void LocalizationIsCorrectlyReadFromUTF8Files() {
		var locDB = new LocDB("english", "french");
		locDB.ScrapeFile("TestFiles/Localization/test_l_french_without_bom.yml");
		
		Assert.Equal("Test loc 1", locDB.GetLocBlockForKey("test_key_1")!["french"]);
		Assert.Equal("Test loc 2", locDB.GetLocBlockForKey("test_key_2")!["french"]);
	}
	[Fact]
	public void LocalizationIsCorrectlyReadFromUTF8BOMFiles() {
		var locDB = new LocDB("english", "french");
		locDB.ScrapeFile("TestFiles/Localization/test_l_french_with_bom.yml");
		
		Assert.Equal("Test loc 1", locDB.GetLocBlockForKey("test_key_1")!["french"]);
		Assert.Equal("Test loc 2", locDB.GetLocBlockForKey("test_key_2")!["french"]);
	}

	[Fact]
	public void AddLocForKeyAndLanguageCanCreateOrUpdateLocBlock() {
		var locDB = new LocDB("english", "french");

		const string testLocKey = "testLocKey";
		Assert.Null(locDB.GetLocBlockForKey(testLocKey));

		locDB.AddLocForKeyAndLanguage(testLocKey, "french", loc: "Romain");
		Assert.Null(locDB.GetLocBlockForKey(testLocKey)!["english"]);
		Assert.Equal("Romain", locDB.GetLocBlockForKey(testLocKey)!["french"]);

		locDB.AddLocForKeyAndLanguage(testLocKey, "english", loc: "Roman");
		Assert.Equal("Roman", locDB.GetLocBlockForKey(testLocKey)!["english"]);
		Assert.Equal("Romain", locDB.GetLocBlockForKey(testLocKey)!["french"]);
	}
}