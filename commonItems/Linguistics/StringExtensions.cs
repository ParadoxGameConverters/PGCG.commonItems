using Open.Collections;
using Open.Text;
using System;
using System.Linq;

namespace commonItems.Linguistics;

public static class StringExtensions {
	private static readonly OrderedDictionary<string, string> AdjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>

		// 4+ Letters		
		{"Verahram Qal'eh", "Verahrami"},
		{"* Islands", "*"}, // Cook Islands
		{"* Republic", "*"}, // Dominican Republic
		{"* Union", "*"}, // Soviet Union
		{"*emirates", "*emirati"}, // United Arab Emirates
		{"*[v]tzerland", "*[v]ss"}, // Switzerland
		{"*zealand", "*zealander"}, // New Zealand
		{"*reland", "*rish"}, // Ireland
		{"*ailand", "*ai"}, // Thailand
		{"*tland", "*ttish"}, // Scotland
		{"*pland", "*ppish"}, // Lapland
		{"*inland", "*innish"}, // Finland
		{"*nland", "*nlander"}, // Greenland
		{"*[c]land", "*[c]lish"}, // England
		{"*eland", "*elandic"}, // Iceland
		{"*[v]land", "*[v]lish"}, // Poland
		{"*ance", "*ench"}, // France
		{"*anon", "*anese"}, // Lebanon
		{"*berg", "*berger"}, // Nuremberg
		{"*burg", "*burger"}, // Hamburg
		{"*china", "*chinese"}, // Indochina
		{"*eece", "*eek"}, // Greece
		{"*egal", "*egalese"}, // Senegal
		{"*enmark", "*anish"}, // Denmark
		{"*hana", "*hanaian"}, // Ghana
		{"*irus", "*irote"}, // Epirus
		{"*pakistan", "*pakistani"}, // Pakistan
		{"*istan", "*"}, // Tajikistan
		{"*lles", "*llois"}, // Seychelles
		{"*pain", "*panish"}, // Spain
		{"*prus", "*priote"}, // Cyprus
		{"*stan", "*"}, // Kazakhstan
		{"*stein", "*steiner"}, // Liechtenstein
		{"*tain", "*tish"}, // Great Britain
		{"*tica", "*tic"}, // Antarctica
		{"*udan", "*udanese"}, // Sudan
		{"*uiana", "*uianese"}, // Guiana
		{"*urma", "*urmese"}, // Burma
		{"*venia", "*vene"}, // Slovenia
		{"*yotte", "*horan"}, // Mayotte
		{"*cese", "*cesan"}, // Diocese
		{"*qahn", "*qahni"},

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
		{"*eru", "*eruvian"}, // 	Peru
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
		{"*nes", "*ne"}, // 		Philippines
		{"*oon", "*oonian"}, // 		Cameroon
		{"*pan", "*panese"}, // 		Japan
		{"*que", "*can"}, // 		Martinique
		{"*ros", "*ran"}, // 		Comoros
		{"*sey", "*sey"}, // 		Jersey
		{"*[c]ey", "*[c]ish"}, // 	Turkey
		{"*tan", "*tanese"}, // 		Bhutan
		{"*nik", "*nikian"},
		{"*[v]ch", "*[v]chite"}, // Schech
		{"*yon", "*yonian"}, // Sikyon (rule made up)
		{"*yōn", "*yōnian"}, // Sikyōn (rule made up)

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
		{"*[v]z", "*[v]zite"}, // Ziz (rule made up)
		{"*yk", "*yk"}, // Karamyk (rule made up)
		{"*[v]k", "*[v]kian"},
		{"*th", "*thian"},  // Vilath (rule made up)
		{"*aí", "*aian"}, // Kolōnaí (Colonae in Latin) -> Kolonian
		{"*ai", "*aian"}, // Kolōnai

		// 1 Letter
		{"*a", "*an"}, // 		Libya
		{"*d", "*der"}, // 		Dortmund
		{"*e", "*ian"}, // 		Ukraine
		{"*g", "*gish"}, // 		Luxembourg
		{"*i", "*ian"}, // 		Burundi
		{"*l", "*lian"}, // 		Brazil
		{"*o", "*olese"}, // 		Congo
		{"*q", "*qi"}, // 		Iraq
		{"*r", "*rian"}, // 		Ecuador
		{"*s", "*ian"}, // 		Athens
		{"*t", "*tian"}, // 		Egypt
		{"*u", "*uan"}, // 		 Vanuatu
		{"*x", "*xian"}, // 		Essex
		{"*y", "*ian"} // 		Hungary
	};

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

			return adjectiveEnding
				.Replace("*", commonPart)
				.Replace("[c]", consonant)
				.Replace("[v]", vowel)
				.ToTitleCase();
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