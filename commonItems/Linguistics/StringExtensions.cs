using Open.Collections;
using System;
using System.Linq;

namespace commonItems.Linguistics;

public static partial class StringExtensions {
	public static string TrimNonAlphanumericEnding(this string str) {
		return new string(str.Reverse().SkipWhile(c => !char.IsLetterOrDigit(c)).Reverse().ToArray());
	}

	// Function for trimming non-letter characters from the end.
	public static string TrimNonLetterEnding(this string str) {
		return new string(str.Reverse().SkipWhile(c => !char.IsLetter(c)).Reverse().ToArray());
	}

	private static string ApplyAdjectiveRules(this string str, OrderedDictionary<string, string> rules, bool multipleIterations = false) {
		const string consonantPlaceholder = "[c]";
		const string vowelPlaceholder = "[v]";
		foreach (var (ending, adjectiveEnding) in rules) {
			var evaluatedStr = str;
			var evaluatedEnding = ending;

			string consonant = string.Empty;
			string vowel = string.Empty;

			var asteriskOrClosingBracketPos = ending.LastIndexOfAny(new[] {'*', ']'});
			string literalEnding = ending[(asteriskOrClosingBracketPos + 1)..];
			if (!str.EndsWith(literalEnding, StringComparison.Ordinal)) {
				continue;
			}

			evaluatedEnding = evaluatedEnding[..evaluatedEnding.LastIndexOf(literalEnding, StringComparison.Ordinal)];
			evaluatedStr = evaluatedStr[..evaluatedStr.LastIndexOf(literalEnding, StringComparison.Ordinal)];

			if (evaluatedEnding.EndsWith(consonantPlaceholder)) {
				char previousChar = evaluatedStr[^1];
				if (!previousChar.IsConsonant()) {
					continue;
				}

				consonant = previousChar.ToString();
				evaluatedStr = evaluatedStr[..^1];
			} else if (evaluatedEnding.EndsWith(vowelPlaceholder)) {
				char previousChar = evaluatedStr[^1];
				if (!previousChar.IsVowel()) {
					continue;
				}

				vowel = previousChar.ToString();
				evaluatedStr = evaluatedStr[..^1];
			}

			string commonPart = evaluatedStr;

			var adjective = adjectiveEnding
				.Replace("*", commonPart)
				.Replace("[c]", consonant)
				.Replace("[v]", vowel);
			
			if (multipleIterations && adjective != str) {
				return adjective.ApplyAdjectiveRules(rules, multipleIterations);
			}
			return adjective;
		}
		
		if (multipleIterations) {
			return str;
		}

		var foldedStr = str.FoldToASCII();
		if (foldedStr != str) {
			return foldedStr.ApplyAdjectiveRules(rules, multipleIterations);
		}

		var trimmedStr = str.TrimNonLetterEnding();
		if (trimmedStr != str) {
			return trimmedStr.ApplyAdjectiveRules(rules, multipleIterations);
		}
			
		// fallback
		Logger.Warn($"No matching adjective rule found for \"{str}\"!");
		return $"{str}ite";
	}

	public static string GetAdjective(this string str) => str
		.ApplyAdjectiveRules(AdjectiveRewriteRules, multipleIterations: true)
		.ApplyAdjectiveRules(AdjectiveRules, multipleIterations: false);
}