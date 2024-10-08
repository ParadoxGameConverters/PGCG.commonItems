﻿using commonItems.Collections;
using System.Collections.Generic;

namespace commonItems.Localization;

public class LocBlock : IIdentifiable<string>, IEnumerable<KeyValuePair<string, string?>> {
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
		localizations = new Dictionary<string, string?>(otherBlock.localizations);
	}

	public void CopyFrom(LocBlock otherBlock) {
		foreach (var (language, loc) in otherBlock.localizations) {
			localizations[language] = loc;
		}
	}
	
	public bool HasLocForLanguage(string language) {
		return localizations.GetValueOrDefault(language) is not null;
	}

	public string? this[string language] {
		get {
			var toReturn = localizations.GetValueOrDefault(language, null);
			if (toReturn is not null) {
				return toReturn;
			}
			
			// As fallback, try to use base language loc.
			if (language != baseLanguage) {
				toReturn = localizations.GetValueOrDefault(baseLanguage, null);
			}
			return toReturn;
		}
		set => localizations[language] = value;
	}

	/// <summary>
	/// Helps remove boilerplate by applying modifyingMethod to every language in the struct
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
			localizations[language] = modifyingFunction(localizations[language], otherBlock[language], language);
		}
		if (!localizations.ContainsKey(baseLanguage)) {
			localizations[baseLanguage] = modifyingFunction(null, otherBlock[baseLanguage], baseLanguage);
		}
	}
	public void ModifyForEveryLanguage(LocDelegate modifyingFunction) {
		foreach (var language in localizations.Keys) {
			localizations[language] = modifyingFunction(localizations[language], language);
		}
		if (!localizations.ContainsKey(baseLanguage)) {
			localizations[baseLanguage] = modifyingFunction(null, baseLanguage);
		}
	}

	public string GetYmlLocLineForLanguage(string language) {
		return $" {Id}: \"{this[language]}\"";
	}

	public IEnumerator<KeyValuePair<string, string?>> GetEnumerator() {
		return localizations.GetEnumerator();
	}

	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}