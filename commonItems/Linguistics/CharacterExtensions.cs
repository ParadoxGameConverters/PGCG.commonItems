using System;

using System.Collections.Generic;

namespace commonItems.Linguistics; 

public static class CharacterExtensions {
	private const string Vowels = "AÀÁÂÃÄÅĀĂĄǺȀȂẠẢẤẦẨẪẬẮẰẲẴẶḀÆǼEȄȆḔḖḘḚḜẸẺẼẾỀỂỄỆĒĔĖĘĚÈÉÊËIȈȊḬḮỈỊĨĪĬĮİÌÍÎÏĲOŒØǾȌȎṌṎṐṒỌỎỐỒỔỖỘỚỜỞỠỢŌÒÓŎŐÔÕÖUŨŪŬŮŰŲÙÚÛÜȔȖṲṴṶṸṺỤỦỨỪỬỮỰYẙỲỴỶỸŶŸÝ";
	private static readonly HashSet<char> UppercaseVowels = [.. Vowels];
	
	public static bool IsVowel(this char character) {
		return UppercaseVowels.Contains(char.ToUpperInvariant(character));
	}

	public static bool IsConsonant(this char character) {
		if (!char.IsLetter(character)) {
			return false;
		}
		
		return !character.IsVowel();
	}
}