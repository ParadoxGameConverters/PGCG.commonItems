using Open.Collections;
using System;

namespace commonItems.Linguistics; 

public static class StringExtensions {
	private static OrderedDictionary<string, string> adjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>
		// 4+ Letters		
		{"* Islands", "*"}, // Cook Islands
		{"* Republic", "*"}, // Dominican Republic
		{"* Union", "*"}, // Soviet Union
		{"*tland", "*ttish"}, // Scotland
		{"*pland", "*ppish"}, // Lapland
		{"*nland", "*nnish"}, // Finland
		{"*[c]land", "*[c]ish"}, // England
		{"*[v]land", "*[v]landic"}, // Iceland
		{"*[v]tzerland", "*ss"}, // Switzerland
		{"*ailand", "*ai"}, // Thailand
		{"*ance", "*ench"}, // France
		{"*anon", "*anese"}, // Lebanon
		{"*berg", "*berger"}, // Nuremberg
		{"*burg", "*burger"}, // Hamburg
		{"*china", "*chinese"}, // Indochina
		{"*eece", "*eek"}, // Greece
		{"*egal", "*egalese"}, // Senegal
		{"*emirates", "*emirati"}, // United Arab Emirates
		{"*enmark", "*anish"}, // Denmark
		{"*hana", "*hanaian"}, // Ghana
		{"*irus", "*irote"}, // Epirus
		{"*istan", "*"}, // Tajikistan
		{"*lles", "*llois"}, // Seychelles
		{"*nland", "*nlander"}, // Greenland
		{"*pain", "*panish"}, // Spain
		{"*pakistan", "*pakistani"}, // Pakistan
		{"*prus", "*priote"}, // Cyprus
		{"*reland", "*rish"}, // Ireland
		{"*stan", "*"}, // Kazakhstan
		{"*stein", "*steiner"}, // Liechtenstein
		{"*tain", "*tish"}, // Great Britain
		{"*tica", "*tic"}, // Antarctica
		{"*udan", "*udanese"}, // Sudan
		{"*uiana", "*uianese"}, // Guiana
		{"*urma", "*urmese"}, // Burma
		{"*venia", "*vene"}, // Slovenia
		{"*yotte", "*horan"}, // Mayotte
		{"*zealand", "*zealander"}, // New Zealand
		{"*cese", "*cesan"}, // Diocese
		// 3 Letters
		{"*[c]am", "*[c]amese"}, // 	Vietnam
		{"*[c]us", "*[c]usian"}, // 	Belarus
		{"*[v]am", "*[v]amanian"}, // 	Guam
		{"*[v]os", "*[v]o"}, // 	Laos
		{"*[v]us", "*[v]an"}, // 	Mauritius
		{"*ain", "*aini"}, // 	Bahrain
		{"*ales", "*elsh"}, // 	Wales
		{"*ame", "*amese"}, // 	Suriname
		{"*ati", "*ati"}, // 	Kiribati
		{"*car", "*can"}, // 	Madagascar
		{"*cau", "*canese"}, // 	Macau
		{"*den", "*dish"}, // 	Sweden
		{"*dos", "*dian"}, // 	Barbados
		{"*eru", "*uvian"}, // 	Peru
		{"*ese", "*ese"}, // 	Cheese
		{"*gal", "*guese"}, // 		Portugal
		{"*ini", "*i"}, // 		Eswatini
		{"*jan", "*jani"}, // 		Azerbaijan
		{"*kia", "*k"}, // 		Slovakia
		{"*lan", "*lanese"}, // 		Milan
		{"*man", "*mani"}, // 		Oman
		{"*mas", "*mian"}, // 		Bahamas
		{"*men", "*meni"}, // 		Yemen
		{"*mor", "*morese"}, // 		Timor
		{"*nce", "*ntine"}, // 		Florence
		{"*nes", "*ne"}, // 		Phillipenes
		{"*oon", "*oonian"}, // 		Cameroon
		{"*pan", "*panese"}, // 		Japan
		{"*que", "*can"}, // 		Martinique
		{"*ros", "*ran"}, // 		Comoros
		{"*sey", "*sey"}, // 		Jersey
		{"*[c]ey", "*ish"}, // 	Turkey
		{"*tan", "*tanese"}, // 		Bhutan
		
		// 2 Letters
		{"*[v]ng", "*[v]nger"}, // 		Hong Kong
		{"*[v]y", "*[v]yan"}, // 		Paraguay
		{"*ad", "*adian"}, // 		Chad
		{"*al", "*ali"}, // 		Nepal
		{"*an", "*anian"}, // 		Jordan
		{"*ao", "*aoan"}, // 		Curaçao
		{"*ar", "*ari"}, // 		Qatar
		{"*as", "*an"}, // 		Honduras
		{"*au", "*auan"}, // 		Palau
		{"*co", "*can"}, // 		Morocco
		{"*de", "*dean"}, // 		Cape Verde
		{"*el", "*eli"}, // 		Israel
		{"*en", "*enese"}, // 		Jan Mayen
		{"*es", "*ian"}, // 		Maldives
		{"*in", "*inese"}, // 		Benin
		{"*it", "*iti"}, // 		Kuwait
		{"*le", "*lean"}, // 		Chile
		{"*ll", "*llese"}, // 		Marshall
		{"*my", "*mois"}, // 		Saint Barthélemy
		{"*na", "*nian"}, // 		Argentina
		{"*ny", "*n"}, // 		Germany
		{"*oe", "*oese"}, // 		Faroe
		{"*on", "*onese"}, // 		Gabon
		{"*re", "*rean"}, // 		Singapore
		{"*ro", "*rin"}, // 		Montenegro
		{"*sh", "*shi"}, // 		Bangladesh
		{"*ta", "*tese"}, // 		Malta
		{"*ue", "*uan"}, // 		Niue
		{"*um", "*an"}, // 		Belgium
		{"*vo", "*var"}, // 		Kosovo
		{"*we", "*wean"}, // 		Zimbabwe
		{"*ze", "*zean"}, // 		Belize

		// 1 Letter
		{"*a", "*an"}, // 		Libya
		{"*d", "*der"}, // 		Åland
		{"*e", "*ian"}, // 		Ukraine
		{"*g", "*gish"}, // 		Luxembourg
		{"*i", "*ian"}, // 		Burundi
		{"*l", "*lian"}, // 		Brazil
		{"*o", "*olese"}, // 		Congo
		{"*q", "*qi"}, // 		Iraq
		{"*r", "*rian"}, // 		Ecuador
		{"*s", "*ian"}, // 		Athens
		{"*t", "*tian"}, // 		Egypt
		{"*u", "*uan"}, // 		Papua
		{"*x", "*xian"}, // 		Essex
		{"*y", "*ian"} // 		Hungary
	};
	public static string GetAdjective(this string str) {
		const string consonantPlaceholder = "[c]";
		const string vowelPlaceholder = "[v]";
		foreach (var (ending, adjectiveEnding) in adjectiveRules) {
			var evaluatedStr = str;
			var evaluatedEnding = ending;

			string commonPart = string.Empty;
			string consonant = string.Empty;
			string vowel = string.Empty;
			
			var asteriskOrClosingBracketPos = ending.LastIndexOfAny(new[]{'*', ']'});
			if (asteriskOrClosingBracketPos != -1) {
				string literalEnding = ending[(asteriskOrClosingBracketPos + 1)..];
				if (!str.EndsWith(literalEnding)) {
					continue;
				}

				evaluatedEnding = evaluatedEnding[..evaluatedEnding.LastIndexOf(literalEnding, StringComparison.Ordinal)];
				evaluatedStr = evaluatedStr[..evaluatedStr.LastIndexOf(literalEnding, StringComparison.Ordinal)];
			}

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

			commonPart = evaluatedStr;

			return adjectiveEnding
				.Replace("*", commonPart)
				.Replace("[c]", consonant)
				.Replace("[v]", vowel);
		}
		// fallback
		Logger.Warn($"No matching adjective rule found for \"{str}\"!");
		return str + "ite";
	}
}