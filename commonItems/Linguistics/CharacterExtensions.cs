using System;

namespace commonItems.Linguistics; 

public static class CharacterExtensions {
	private const string Vowels = "aeiou";
	
	public static bool IsVowel(this char character) {
		bool isVowel = Vowels.Contains(character, StringComparison.InvariantCultureIgnoreCase);
		return isVowel;
	}
}