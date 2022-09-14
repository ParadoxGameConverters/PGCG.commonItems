using Open.Collections;
using Open.Text;
using System;
using System.Linq;

namespace commonItems.Linguistics;

public static class StringExtensions {
	private static readonly OrderedDictionary<string, string> AdjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>
		
		// 6+ letters
		{"Ad Pontem", "Pontan"},
		{"Verahram Qal'eh", "Verahrami"},
		{"Orkney Islands", "Orcadian"},
		{"* Islands", "*"}, // Cook Islands
		{"* Republic", "*"}, // Dominican Republic
		{"* Union", "*"}, // Soviet Union
		{"*emirates", "*emirati"}, // United Arab Emirates
		{"* Qal'eh", "*"},
		{"*[v]tzerland", "*[v]ss"}, // Switzerland
		{"*zealand", "*zealander"}, // New Zealand
		{"*pakistan", "*pakistani"}, // Pakistan
		{"*enmark", "*anish"}, // Denmark
		{"*inland", "*innish"}, // Finland
		{"*reland", "*rish"}, // Ireland
		{"*ailand", "*ai"}, // Thailand
		
		// 5+ letters
		{"*atium", "*atin"}, // Latium
		{"*china", "*chinese"}, // Indochina
		{"*eland", "*elandic"}, // Iceland
		{"*istan", "*"}, // Tajikistan
		{"*lades", "*ladian"}, // Cyclades
		{"*nland", "*nlander"}, // Greenland
		{"*pland", "*ppish"}, // Lapland
		{"*tland", "*ttish"}, // Scotland
		{"*ttium", "*ttian"}, // Bruttium
		{"*stein", "*steiner"}, // Liechtenstein
		{"*uiana", "*uianese"}, // Guiana
		{"*venia", "*vene"}, // Slovenia
		{"*yotte", "*horan"}, // Mayotte
		{"*[c]land", "*[c]lish"}, // England
		{"*[v]land", "*[v]lish"}, // Poland
		
		// 4 Letters
		{"*ance", "*ench"}, // France
		{"*anon", "*anese"}, // Lebanon
		{"*berg", "*berger"}, // Nuremberg
		{"*burg", "*burger"}, // Hamburg
		{"*echk", "*echk"},
		{"*eece", "*eek"}, // Greece
		{"*eese", "*eese"}, // 	Cheese
		{"*egal", "*egalese"}, // Senegal
		{"*etus", "*esian"},
		{"*hana", "*hanaian"}, // Ghana
		{"*iana", "*ian"}, // Bactriana
		{"*irus", "*irote"}, // Epirus
		{"*lles", "*llois"}, // Seychelles
		{"*nsck", "*nsck"},
		{"*ntus", "*ntic"}, // Pontus
		{"*orus", "*oran"}, // Bosporus
		{"*pain", "*panish"}, // Spain
		{"*prus", "*priote"}, // Cyprus
		{"*stan", "*"}, // Kazakhstan
		{"*tain", "*tish"}, // Great Britain
		{"*tica", "*tic"}, // Antarctica
		{"*tium", "*tine"}, // Byzantium
		{"*udan", "*udanese"}, // Sudan
		{"*urma", "*urmese"}, // Burma
		{"*cese", "*cesan"}, // Diocese
		{"*qahn", "*qahni"},
		{"*gnac", "*gnac"},
		{"*rgos", "*rgive"}, // Argos
		{"*[v]cis", "*[v]cian"}, // Phocis
		{"*[c]cis", "*[c]cidian"}, // Chalcis

		// 3 Letters
		{"*ain", "*aini"}, // Bahrain
		{"*ales", "*elsh"}, // Wales
		{"*ame", "*amese"}, // Suriname
		{"*ara", "*arian"}, // Megara
		{"*ati", "*ati"}, // Kiribati
		{"*bah", "*ban"},
		{"*car", "*can"}, // Madagascar
		{"*cau", "*canese"}, // Macau
		{"*dai", "*dan"}, // Oiniadai
		{"*den", "*dish"}, // Sweden
		{"*des", "*dan"}, // Oiniades
		{"*dos", "*dian"}, // Barbados
		{"*don", "*donian"}, // Sidon
		{"*eia", "*ian"}, // Eleia
		{"*eru", "*eruvian"}, // Peru
		{"*gal", "*guese"}, // Portugal
		{"*gac", "*gac"},
		{"*iam", "*iamese"}, // Siam
		{"*ini", "*i"}, // Eswatini
		{"*ios", "*iot"}, // Chios
		{"*ite", "*itian"}, // Melite
		{"*jan", "*jani"}, // Azerbaijan
		{"*kia", "*k"}, // Slovakia
		{"*kun", "*kunite"},
		{"*lan", "*lanese"}, // Milan
		{"*lta", "*ltese"}, // Malta
		{"*man", "*mani"}, // Oman
		{"*mas", "*mian"}, // Bahamas
		{"*men", "*meni"}, // Yemen
		{"*mis", "*minian"}, // Salamis
		{"*mor", "*morese"}, // Timor
		{"*nae", "*naean"}, // Mycenae
		{"*nce", "*ntine"}, // Florence
		{"*nik", "*nikian"},
		{"*nes", "*ne"}, // Philippines
		{"*oon", "*oonian"}, // Cameroon
		{"*pan", "*panese"}, // Japan
		{"*que", "*can"}, // Martinique
		{"*qah", "*qian"},
		{"*ros", "*ran"}, // Comoros
		{"*sey", "*sey"}, // Jersey
		{"*sus", "*sian"}, // Ephesus
		{"*tan", "*tanese"}, // Bhutan
		{"*ton", "*tonian"}, // Croton
		{"*tus", "*tian"}, // Carystus
		{"*use", "*usan"}, // Syracuse
		{"*yon", "*yonian"}, // Sikyon (rule made up)
		{"*yōn", "*yōnian"}, // Sikyōn (rule made up)
		{"*[v]ng", "*[v]nger"}, // 		Hong Kong
		{"*[v]os", "*[v]o"}, // 	Laos
		{"*[v]us", "*[v]an"}, // 	Mauritius
		{"*[c]ey", "*[c]ish"}, // 	Turkey
		{"*[v]am", "*[v]amanian"}, // 	Guam
		{"*[c]am", "*[c]amese"}, // 	Vietnam
		{"*[c]us", "*[c]usian"}, // 	Belarus
		{"*[v]ch", "*[v]chite"}, // Schech

		// 2 Letters
		{"*ab", "*abite"}, // Achtab (rule made up)
		{"*ad", "*adian"}, // 		Chad
		{"*ae", "*ian"}, // Colossae
		{"*ah", "*ah"},
		{"*aí", "*aian"}, // Kolōnaí (Colonae in Latin) -> Kolonaian (rule made up)
		{"*ai", "*aian"}, // Kolōnai
		{"*al", "*ali"}, // 		Nepal
		{"*an", "*anian"}, // 		Jordan
		{"*ao", "*aoan"}, // 		Curaçao
		{"*ar", "*ari"}, // 		Qatar
		{"*as", "*an"}, // 		Honduras
		{"*au", "*auan"}, // 		Palau
		{"*co", "*can"}, // 		Morocco
		{"*de", "*dean"}, // 		Cape Verde
		{"*eh", "*ehi"},
		{"*el", "*eli"}, // 		Israel
		{"*em", "*emite"}, // Zarem (rule made up)
		{"*en", "*enese"}, // 		Jan Mayen
		{"*es", "*ian"}, // 		Maldives
		{"*ge", "*ginian"}, // Carthage
		{"*gh", "*ghi"},
		{"*gk", "*gkan"},
		{"*in", "*inese"}, // 		Benin
		{"*im", "*imite"},
		{"*is", "*ian"}, // Locris
		{"*it", "*iti"}, // 		Kuwait
		{"*le", "*lean"}, // 		Chile
		{"*kh", "*khi"},
		{"*ll", "*llese"}, // Marshall
		{"*me", "*man"}, // Rome
		{"*my", "*mois"}, // Saint Barthélemy
		{"*na", "*nian"}, // Argentina
		{"*nj", "*nji"},
		{"*ny", "*n"}, // 		Germany
		{"*oe", "*oese"}, // 		Faroe
		{"*oh", "*ohan"},
		{"*on", "*onese"}, // 		Gabon
		{"*oy", "*ojan"}, // Troy
		{"*os", "*ian"}, // Thasos
		{"*ra", "*rean"}, // Corcyra
		{"*re", "*rean"}, // Singapore
		{"*ro", "*rin"}, // 		Montenegro
		{"*rn", "*rnite"}, // made up: Arn -> Arnite
		{"*sh", "*shi"}, // 		Bangladesh
		{"*ta", "*tan"}, // Egesta
		{"*te", "*tan"}, // Crete
		{"*th", "*thian"},  // Vilath (rule made up)
		{"*ue", "*uan"}, // Niue
		{"*ul", "*ulish"}, // Gaul
		{"*um", "*an"}, // 		Belgium
		{"*vo", "*var"}, // 		Kosovo
		{"*we", "*wean"}, // 		Zimbabwe
		{"*yk", "*yk"}, // Karamyk (rule made up)
		{"*ze", "*zean"}, // 		Belize
		{"*[v]h", "*[v]hite"},
		{"*[v]k", "*[v]kian"},
		{"*[c]s", "*[c]ian"}, // 		Athens
		{"*[v]y", "*[v]yan"}, // 		Paraguay
		{"*[v]z", "*[v]zite"}, // Ziz (rule made up)

		// 1 Letter
		{"*a", "*an"}, // 		Libya
		{"*d", "*der"}, // 		Dortmund
		{"*e", "*ian"}, // 		Ukraine
		{"*g", "*gish"}, // 		Luxembourg
		{"*i", "*ian"}, // 		Burundi
		{"*k", "*kian"},
		{"*l", "*lian"}, // 		Brazil
		{"*o", "*olese"}, // 		Congo
		{"*q", "*qi"}, // 		Iraq
		{"*r", "*rian"}, // 		Ecuador
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