using System.Collections.Generic;
using System.Linq;

namespace commonItems.Localization {
	public class LocBlock {
		private readonly string baseLanguage;
		private readonly string[] otherLanguages;
		private readonly Dictionary<string, string> locs;

		public LocBlock(string baseLanguage, params string[] otherLanguages) {
			this.baseLanguage = baseLanguage;
			this.otherLanguages = otherLanguages;
			locs = new() { [this.baseLanguage] = string.Empty };

			foreach (var language in otherLanguages) {
				locs[language] = string.Empty;
			}
		}

		public string this[string language] {
			get => locs[language];
			set => locs[language] = value;
		}

		public LocBlock(LocBlock otherBlock) {
			baseLanguage = otherBlock.baseLanguage;
			otherLanguages = otherBlock.otherLanguages;
			locs = new(otherBlock.locs);
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
			foreach (var language in locs.Keys) {
				locs[language] = modifyingFunction(locs[language], otherBlock[language]);
			}
		}
		private void FillMissingLocWithBaseLanguageLoc(string language) {
			if (string.IsNullOrEmpty(locs[language])) {
				locs[language] = locs[baseLanguage];
			}
		}
		public void FillMissingLocsWithBaseLanguageLoc() {
			foreach (string language in otherLanguages) {
				FillMissingLocWithBaseLanguageLoc(language);
			}
		}

		public bool HasMissingSecondaryLanguageLoc() {
			bool hasBaseLanguageLoc = !string.IsNullOrEmpty(locs[baseLanguage]);
			bool hasMissingSecondaryLanguageLoc = otherLanguages.Any(language => string.IsNullOrEmpty(locs[language]));

			return hasBaseLanguageLoc && hasMissingSecondaryLanguageLoc;
		}
	}
}
