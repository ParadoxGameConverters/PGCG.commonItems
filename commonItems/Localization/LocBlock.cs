using System.Collections.Generic;
using System.Linq;

namespace commonItems.Localization {
	public class LocBlock {
		private readonly string baseLanguage;
		private readonly Dictionary<string, string> locs;

		public LocBlock(string baseLanguage, params string[] otherLanguages) {
			locs[baseLanguage] = string.Empty;
			this.baseLanguage = baseLanguage;

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
		private void FillEmptyLocWithBaseLanguageLoc(string language) {
			if (string.IsNullOrEmpty(language)) {
				locs[language] = locs[baseLanguage];
			}
		}
		public void FillEmptyLocsWithBaseLanguageLoc() {
			foreach (string language in locs.Keys.Where(language => language != baseLanguage)) {
				FillEmptyLocWithBaseLanguageLoc(language);
			}
		}
	}
}
