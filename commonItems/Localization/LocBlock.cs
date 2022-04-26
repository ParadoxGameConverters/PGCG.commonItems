using System.Collections.Generic;
using System.Linq;

namespace commonItems.Localization;

public class LocBlock {
	private readonly string baseLanguage;
	private readonly Dictionary<string, string?> localizations;

	public LocBlock(string baseLanguage) {
		this.baseLanguage = baseLanguage;
		localizations = new Dictionary<string, string?>();
	}

	public LocBlock(LocBlock otherBlock) {
		baseLanguage = otherBlock.baseLanguage;
		localizations = new(otherBlock.localizations);
	}

	public void CopyFrom(LocBlock otherBlock) {
		foreach (var (language, loc) in otherBlock.localizations) {
			localizations[language] = loc;
		}
	}

	public string? this[string language] {
		get {
			var toReturn = localizations.GetValueOrDefault(language, null);
			if (toReturn is null && language != baseLanguage) {
				toReturn = localizations.GetValueOrDefault(baseLanguage, null);
			}

			return toReturn;
		}
		set => localizations[language] = value;
	}

	/// <summary>
	/// <see cref="ModifyForEveryLanguage"/> helps remove boilerplate by applying modifyingMethod to every language in the struct
	/// For example:
	/// nameLocBlock["english"] = nameLocBlock["english"].Replace("$ADJ$", baseAdjLocBlock["english"]);
	/// nameLocBlock["french"] = nameLocBlock["french"].Replace("$ADJ$", baseAdjLocBlock["french"]);
	/// nameLocBlock["german"] = nameLocBlock["german"].Replace("$ADJ$", baseAdjLocBlock["german"]);
	/// nameLocBlock["russian"] = nameLocBlock["russian"].Replace("$ADJ$", baseAdjLocBlock["russian"]);
	/// nameLocBlock["simp_chinese"] = nameLocBlock["simp_chinese"].Replace("$ADJ$", baseAdjLocBlock["simp_chinese"]);
	/// nameLocBlock["spanish"] = nameLocBlock["spanish"].Replace("$ADJ$", baseAdjLocBlock["spanish"]);
	///
	/// Can be replaced by:
	/// nameLocBlock.ModifyForEveryLanguage(baseAdjLocBlock, (string baseLoc, string modifyingLoc) => {
	/// 	return baseLoc.Replace("$ADJ$", modifyingLoc);
	/// });
	/// </summary>
	/// <param name="otherBlock"></param>
	/// <param name="modifyingFunction"></param>
	public void ModifyForEveryLanguage(LocBlock otherBlock, LocDelegate modifyingFunction) {
		foreach (var language in localizations.Keys) {
			localizations[language] = modifyingFunction(localizations[language], otherBlock[language]);
		}
	}
}