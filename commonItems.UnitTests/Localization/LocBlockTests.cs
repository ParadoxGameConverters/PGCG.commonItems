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
			["english"] = "$NUM$$ORDER$ Revolt",
			["french"] = "$NUM$$ORDER$ révolte",
			["german"] = "$NUM$$ORDER$ Revolte",
			["russian"] = "$NUM$$ORDER$ бунт",
			["simp_chinese"] = "$NUM$$ORDER$ 反叛",
			["spanish"] = "$NUM$$ORDER$ revuelta"
		};
		
		var number = 2;
		
		nameLocBlock.ModifyForEveryLanguage((loc, _) => loc?.Replace("$NUM$", number.ToString()));
		nameLocBlock.ModifyForEveryLanguage((loc, languageName) => loc?.Replace("$ORDER$", number.ToOrdinalSuffix(languageName)));
		Assert.Equal("2nd Revolt", nameLocBlock["english"]);
		Assert.Equal("2e révolte", nameLocBlock["french"]);
		Assert.Equal("2nd Revolte", nameLocBlock["german"]); // not correct but who cares about German
		Assert.Equal("2nd бунт", nameLocBlock["russian"]); // also not correct (uses English suffix)
		Assert.Equal("2. 反叛", nameLocBlock["simp_chinese"]);
		Assert.Equal("2º revuelta", nameLocBlock["spanish"]);
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
	public void LocForMissingLanguageDefaultsToLocKey() {
		var locBlock = new LocBlock("key1", "english");
		Assert.Equal("key1", locBlock["italian"]);
	}
}