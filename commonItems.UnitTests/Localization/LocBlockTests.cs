using commonItems.Localization;
using System;
using Xunit;

namespace commonItems.UnitTests.Localization; 

public class LocBlockTests {
	[Fact]
	public void LocBlockCanBeModifiedWithOtherLocBlockForEveryLanguage() {
		var nameLocBlock = new LocBlock("key1", "english") {
			["english"] = "$ADJ$ Revolt",
			["french"] = "$ADJ$ révolte",
			["german"] = "$ADJ$ Revolte",
			["russian"] = "$ADJ$ бунт",
			["simp_chinese"] = "$ADJ$ 反叛",
			["spanish"] = "$ADJ$ revuelta"
		};

		var adjLocBlock = new LocBlock("key2", "english") {
			["english"] = "Roman",
			["french"] = "Romain",
			["german"] = "römisch",
			["russian"] = "Роман",
			["simp_chinese"] = "罗马",
			["spanish"] = "Romana"
		};

		nameLocBlock.ModifyForEveryLanguage(adjLocBlock, (baseLoc, modifyingLoc, _) =>
			baseLoc?.Replace("$ADJ$", modifyingLoc)
		);
		Assert.Equal("Roman Revolt", nameLocBlock["english"]);
		Assert.Equal("Romain révolte", nameLocBlock["french"]);
		Assert.Equal("römisch Revolte", nameLocBlock["german"]);
		Assert.Equal("Роман бунт", nameLocBlock["russian"]);
		Assert.Equal("罗马 反叛", nameLocBlock["simp_chinese"]);
		Assert.Equal("Romana revuelta", nameLocBlock["spanish"]);
	}

	[Fact]
	public void LocBlockCanBeModifiedWithoutOtherLocBlockForEveryLanguage() {
		var nameLocBlock = new LocBlock("key1", "english") {
			["english"] = "$NUM$ Revolt",
			["french"] = "$NUM$ révolte",
			["german"] = "$NUM$ Revolte",
			["russian"] = "$NUM$ бунт",
			["simp_chinese"] = "$NUM$ 反叛",
			["spanish"] = "$NUM$ revuelta"
		};
		
		const int number = 2;
		nameLocBlock.ModifyForEveryLanguage((loc, _) => loc?.Replace("$NUM$", number.ToString()));
		Assert.Equal("2 Revolt", nameLocBlock["english"]);
		Assert.Equal("2 révolte", nameLocBlock["french"]);
		Assert.Equal("2 Revolte", nameLocBlock["german"]);
		Assert.Equal("2 бунт", nameLocBlock["russian"]);
		Assert.Equal("2 反叛", nameLocBlock["simp_chinese"]);
		Assert.Equal("2 revuelta", nameLocBlock["spanish"]);
	}

	[Fact]
	public void BaseLanguageIsAlwaysIncludedInModifyForEveryLanguageWithOtherBlock() {
		var locBlock = new LocBlock("locBlock", "english") {
			// Base language (English) loc not defined.
			["french"] = "frLocValue"
		};

		var otherLocBlock = new LocBlock("locBlock", "english") {
			["english"] = "enLocValue-2",
			["french"] = "frLocValue-2"
		};

		locBlock.ModifyForEveryLanguage(otherLocBlock, (loc, otherLoc, language) => {
			var baseLocStr = loc ?? "null";
			return $"{baseLocStr} updated with {otherLoc}";
		});
		Assert.Equal("null updated with enLocValue-2", locBlock["english"]);
		Assert.Equal("frLocValue updated with frLocValue-2", locBlock["french"]);
	}

	[Fact]
	public void BaseLanguageIsAlwaysIncludedInModifyForEveryLanguageWithoutOtherBlock() {
		var locBlock = new LocBlock("locBlock", "english") {
			// Base language (English) loc not defined.
			["french"] = "frLocValue"
		};

		locBlock.ModifyForEveryLanguage((loc, language) => {
			if (string.IsNullOrEmpty(loc)) {
				return $"new loc for language {language}";
			}
			return $"{loc} updated";
		});
		Assert.Equal("new loc for language english", locBlock["english"]);
		Assert.Equal("frLocValue updated", locBlock["french"]);
	}

	[Fact]
	public void LocBlockCanBeCopyConstructed() {
		var origLocBlock = new LocBlock("key1", "english") {
			["english"] = "a",
			["french"] = "b",
			["german"] = "c",
			["russian"] = "d",
			["simp_chinese"] = "e",
			["spanish"] = "f"
		};

		var copyLocBlock = new LocBlock("key2", origLocBlock);
		Assert.Equal("a", copyLocBlock["english"]);
		Assert.Equal("b", copyLocBlock["french"]);
		Assert.Equal("c", copyLocBlock["german"]);
		Assert.Equal("d", copyLocBlock["russian"]);
		Assert.Equal("e", copyLocBlock["simp_chinese"]);
		Assert.Equal("f", copyLocBlock["spanish"]);
	}

	[Fact]
	public void LocBlockCanCopyFromOtherBlock() {
		var origLocBlock = new LocBlock("key1", "english") {
			["english"] = "a",
			["french"] = "b",
			["german"] = "c",
			["russian"] = "d",
			["simp_chinese"] = "e",
			["spanish"] = "f"
		};

		var copyingLocBlock = new LocBlock("key2", "english");
		copyingLocBlock.CopyFrom(origLocBlock);

		Assert.Equal("a", copyingLocBlock["english"]);
		Assert.Equal("b", copyingLocBlock["french"]);
		Assert.Equal("c", copyingLocBlock["german"]);
		Assert.Equal("d", copyingLocBlock["russian"]);
		Assert.Equal("e", copyingLocBlock["simp_chinese"]);
		Assert.Equal("f", copyingLocBlock["spanish"]);
	}

	[Fact]
	public void LocForNonBaseLanguageDefaultsToBaseLanguageLoc() {
		var locBlock = new LocBlock("key1", "english") {
			["english"] = "Key 1 loc"
		};
		Assert.Equal("Key 1 loc", locBlock["italian"]);
	}

	[Fact]
	public void LocForNonBaseLanguageDefaultsNullIfBaseLanguageHasNoLoc() {
		var locBlock = new LocBlock("key1", "english");
		Assert.Null(locBlock["italian"]);
	}
}