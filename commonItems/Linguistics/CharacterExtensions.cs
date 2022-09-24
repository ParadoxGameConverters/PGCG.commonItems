using System;

namespace commonItems.Linguistics; 

public static class CharacterExtensions {
	private const string Vowels = "AÀÁÂÃÄÅĀĂĄǺȀȂẠẢẤẦẨẪẬẮẰẲẴẶḀÆǼEȄȆḔḖḘḚḜẸẺẼẾỀỂỄỆĒĔĖĘĚÈÉÊËIȈȊḬḮỈỊĨĪĬĮİÌÍÎÏĲOŒØǾȌȎṌṎṐṒỌỎỐỒỔỖỘỚỜỞỠỢŌÒÓŎŐÔÕÖUŨŪŬŮŰŲÙÚÛÜȔȖṲṴṶṸṺỤỦỨỪỬỮỰYẙỲỴỶỸŶŸÝ";
	
	public static bool IsVowel(this char character) {
		bool isVowel = Vowels.Contains(character, StringComparison.InvariantCultureIgnoreCase);
		return isVowel;
	}

	public static bool IsConsonant(this char character) {
		if (!char.IsLetter(character)) {
			return false;
		}
		
		return !character.IsVowel();
	}
}