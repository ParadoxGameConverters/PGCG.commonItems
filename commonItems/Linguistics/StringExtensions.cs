using Open.Text;
using System;
using System.Linq;

namespace commonItems.Linguistics;

public static partial class StringExtensions {
	public static string TrimNonAlphanumericEnding(this string str) {
		return new string(str.Reverse().SkipWhile(c => !char.IsLetterOrDigit(c)).Reverse().ToArray());
	}

	public static string GetAdjective(this string str) {
		const string consonantPlaceholder = "[c]";
		const string vowelPlaceholder = "[v]";
		foreach (var (ending, adjectiveEnding) in AdjectiveRules) {
			var evaluatedStr = str;
			var evaluatedEnding = ending;

			string consonant = string.Empty;
			string vowel = string.Empty;

			var asteriskOrClosingBracketPos = ending.LastIndexOfAny(new[] {'*', ']'});
			string literalEnding = ending[(asteriskOrClosingBracketPos + 1)..];
			if (!str.EndsWith(literalEnding, StringComparison.OrdinalIgnoreCase)) {
				continue;
			}

			evaluatedEnding = evaluatedEnding[..evaluatedEnding.LastIndexOf(literalEnding, StringComparison.OrdinalIgnoreCase)];
			evaluatedStr = evaluatedStr[..evaluatedStr.LastIndexOf(literalEnding, StringComparison.OrdinalIgnoreCase)];

			if (evaluatedEnding.EndsWith(consonantPlaceholder)) {
				char previousChar = evaluatedStr[^1];
				if (previousChar.IsVowel()) {
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
			if (!adjectiveEnding.StartsWith("*-")) {
				adjective = adjective.ToTitleCase();
			}
			return adjective;
		}

		var foldedStr = str.FoldToASCII();
		if (foldedStr != str) {
			return foldedStr.GetAdjective();
		}

		var trimmedStr = str.TrimNonAlphanumericEnding();
		if (trimmedStr != str) {
			return trimmedStr.GetAdjective();
		}

		// fallback
		Logger.Warn($"No matching adjective rule found for \"{str}\"!");
		return $"{str}ite";
	}
}