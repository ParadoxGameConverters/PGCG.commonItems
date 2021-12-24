using System.Collections.Generic;
using System.Linq;

namespace commonItems.Localization;

public class LocBlock {
	private readonly string baseLanguage;
	private readonly string[] otherLanguages;
	private readonly Dictionary<string, string> localizations;

	public LocBlock(string baseLanguage, params string[] otherLanguages) {
		this.baseLanguage = baseLanguage;
		this.otherLanguages = otherLanguages;
		localizations = new() { [this.baseLanguage] = string.Empty };

		foreach (var language in otherLanguages) {
			localizations[language] = string.Empty;
		}
	}

	public string this[string language] {
		get => localizations[language];
		set => localizations[language] = value;
	}

	public LocBlock(LocBlock otherBlock) {
		baseLanguage = otherBlock.baseLanguage;
		otherLanguages = otherBlock.otherLanguages;
		localizations = new(otherBlock.localizations);
	}

	// ModifyForEveryLanguage helps remove boilerplate by applying modifyingMethod to every language in the struct
	//
	// For example:
	// nameLocBlock["english"] = nameLocBlock["english"].Replace("$ADJ$", baseAdjLocBlock["english"]);
	// nameLocBlock["french"] = nameLocBlock["french"].Replace("$ADJ$", baseAdjLocBlock["french"]);
	// nameLocBlock["german"] = nameLocBlock["german"].Replace("$ADJ$", baseAdjLocBlock["german"]);
	// nameLocBlock["russian"] = nameLocBlock["russian"].Replace("$ADJ$", baseAdjLocBlock["russian"]);
	// nameLocBlock["simp_chinese"] = nameLocBlock["simp_chinese"].Replace("$ADJ$", baseAdjLocBlock["simp_chinese"]);
	// nameLocBlock["spanish"] = nameLocBlock["spanish"].Replace("$ADJ$", baseAdjLocBlock["spanish"]);
	//
	// Can be replaced by:
	// nameLocBlock.ModifyForEveryLanguage(baseAdjLocBlock, (string baseLoc, string modifyingLoc) => {
	//     return baseLoc.Replace("$ADJ$", modifyingLoc);
	// });
	public void ModifyForEveryLanguage(LocBlock otherBlock, LocDelegate modifyingFunction) {
		foreach (var language in localizations.Keys) {
			localizations[language] = modifyingFunction(localizations[language], otherBlock[language]);
		}
	}
	private void FillMissingLocWithBaseLanguageLoc(string language) {
		if (string.IsNullOrEmpty(localizations[language])) {
			localizations[language] = localizations[baseLanguage];
		}
	}
	public void FillMissingLocWithBaseLanguageLoc() {
		foreach (string language in otherLanguages) {
			FillMissingLocWithBaseLanguageLoc(language);
		}
	}

	public bool HasMissingSecondaryLanguageLoc() {
		bool hasBaseLanguageLoc = !string.IsNullOrEmpty(localizations[baseLanguage]);
		bool hasMissingSecondaryLanguageLoc = otherLanguages.Any(language => string.IsNullOrEmpty(localizations[language]));

		return hasBaseLanguageLoc && hasMissingSecondaryLanguageLoc;
	}
}