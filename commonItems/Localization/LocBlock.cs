using commonItems.Collections;
using System.Collections.Generic;

namespace commonItems.Localization;

public class LocBlock : IIdentifiable<string> {
	private readonly string baseLanguage;
	private readonly Dictionary<string, string?> localizations;
	
	public string Id { get; }
	
	public LocBlock(string locKey, string baseLanguage) {
		Id = locKey;
		this.baseLanguage = baseLanguage;
		localizations = new Dictionary<string, string?>();
	}

	public LocBlock(string locKey, LocBlock otherBlock) {
		Id = locKey;
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
			if (toReturn is not null) {
				return toReturn;
			}
			
			if (language != baseLanguage) {
				toReturn = localizations.GetValueOrDefault(baseLanguage, null);
			}
			return toReturn ?? Id;
		}
		set => localizations[language] = value;
	}

	/// <summary>
	/// <see cref="ModifyForEveryLanguage"/> helps remove boilerplate by applying modifyingMethod to every language in the struct
	/// For example:
	/// <code>
	/// nameLocBlock["english"] = nameLocBlock["english"].Replace("$ADJ$", baseAdjLocBlock["english"]);
	/// nameLocBlock["french"] = nameLocBlock["french"].Replace("$ADJ$", baseAdjLocBlock["french"]);
	/// nameLocBlock["german"] = nameLocBlock["german"].Replace("$ADJ$", baseAdjLocBlock["german"]);
	/// nameLocBlock["russian"] = nameLocBlock["russian"].Replace("$ADJ$", baseAdjLocBlock["russian"]);
	/// nameLocBlock["simp_chinese"] = nameLocBlock["simp_chinese"].Replace("$ADJ$", baseAdjLocBlock["simp_chinese"]);
	/// nameLocBlock["spanish"] = nameLocBlock["spanish"].Replace("$ADJ$", baseAdjLocBlock["spanish"]);
	/// </code>
	/// 
	/// Can be replaced by:
	/// <code>
	/// nameLocBlock.ModifyForEveryLanguage(baseAdjLocBlock, (string baseLoc, string modifyingLoc) => {
	/// 	return baseLoc.Replace("$ADJ$", modifyingLoc);
	/// });
	/// </code>
	/// </summary>
	/// <param name="otherBlock"></param>
	/// <param name="modifyingFunction"></param>
	public void ModifyForEveryLanguage(LocBlock otherBlock, TwoArgLocDelegate modifyingFunction) {
		foreach (var language in localizations.Keys) {
			localizations[language] = modifyingFunction(localizations[language], otherBlock[language]);
		}
	}
	public void ModifyForEveryLanguage(LocDelegate modifyingFunction) {
		foreach (var language in localizations.Keys) {
			localizations[language] = modifyingFunction(localizations[language]);
		}
	}
}