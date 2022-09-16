﻿
using Open.Collections;
using System;

namespace commonItems.Linguistics;

public static partial class StringExtensions {
	private static readonly OrderedDictionary<string, string> AdjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>
		
// 6+ letters
{"Eswatini", "Swati"}, // Eswatini
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
{"*ontium", "*ontinian"}, // Leontium
{"*serica", "*sere"}, // Serica
{"*iohaemum", "*ius"}, // Boiohaemum
{"*fpaktos", "*upactian"}, // Nafpaktos
{"*rithymna", "*rhithymnian"}, // Rithymna
{"*thuria", "*thuriat"}, // Thuria
{"*aionia", "*aeonian"}, // Paionia
{"*dhra Pradesh", "*dhrulu"}, // Andhra Pradesh
{"*al Pradesh", "*ali"}, // Arunachal Pradesh
{"*kshadweep", "*ccadivian"}, // Lakshadweep
{"*galand", "*galandese"}, // Nagaland
{"* Nadu", "*"}, // Tamil Nadu

// 5 letters
{"*allia", "*aulish"}, // Gallia
{"*atium", "*atin"}, // Latium
{"*china", "*chinese"}, // Indochina
{"*eland", "*elandic"}, // Iceland
{"*istan", "*"}, // Tajikistan
{"*khand", "*khandi"}, // Jharkhand
{"*lades", "*ladian"}, // Cyclades
{"*mania", "*manic"}, // Germania
{"*nland", "*nlander"}, // Greenland
{"*pelos", "*pelitan"}, // Skopelos
{"*pland", "*ppish"}, // Lapland
{"*rrhae", "*rrean"}, // Serrhae
{"*ryana", "*ryanvi"}, // Haryana
{"*shtra", "*shtrian"}, // Maharashtra
{"*stein", "*steiner"}, // Liechtenstein
{"*tland", "*ttish"}, // Scotland
{"*ttium", "*ttian"}, // Bruttium
{"*uiana", "*uianese"}, // Guiana
{"*venia", "*vene"}, // Slovenia
{"*yotte", "*horan"}, // Mayotte
{"*zoram", "*zo"}, // Mizoram
{"*[c]land", "*[c]lish"}, // England
{"*[v]land", "*[v]lish"}, // Poland

// 4 Letters
{"*aïda", "*idonian"}, // Saïda
{"*ammu", "*ammu"}, // Jammu
{"*ance", "*ench"}, // France
{"*anon", "*anese"}, // Lebanon
{"*aras", "*arentine"}, // Taras
{"*aros", "*arian"}, // Paros
{"*berg", "*berger"}, // Nuremberg
{"*borg", "*borgenser"}, // Aalborg
{"*burg", "*burger"}, // Hamburg
{"*cese", "*cesan"}, // Diocese
{"*deen", "*donian"}, // Aberdeen
{"*echk", "*echk"},
{"*eece", "*eek"}, // Greece
{"*eese", "*eese"}, // 	Cheese
{"*egal", "*egalese"}, // Senegal
{"*elos", "*elian"}, // Delos
{"*etus", "*esian"},
{"*gana", "*ganite"}, // Telangana
{"*gina", "*ginetan"}, // Aegina
{"*gnac", "*gnac"},
{"*hana", "*hanaian"}, // Ghana
{"*hria", "*hriasian"}, // Thria
{"*iana", "*ian"}, // Bactriana
{"*irus", "*irote"}, // Epirus
{"*lius", "*liasian"}, // Phlius
{"*lles", "*llois"}, // Seychelles
{"*mnus", "*mnian"}, // Epidamnus
{"*neia", "*nean"}, // Mantineia
{"*nium", "*nite"}, // Samnium
{"*ngal", "*ngali"}, // West Bengal
{"*nsck", "*nsck"},
{"*ntum", "*ntine"}, // Tarentum
{"*ntus", "*ntic"}, // Pontus
{"*odes", "*odian"}, // Rhodes
{"*orus", "*oran"}, // Bosporus
{"*pain", "*panish"}, // Spain
{"*prus", "*priote"}, // Cyprus
{"*rala", "*ralite"}, // Kerala
{"*rrae", "*rrean"}, // Serrae
{"*stan", "*"}, // Kazakhstan
{"*tain", "*tish"}, // Great Britain
{"*than", "*thani"}, // Rajasthan
{"*thon", "*thonian"}, // Marathon
{"*tica", "*tic"}, // Antarctica
{"*tium", "*tine"}, // Byzantium
{"*udan", "*udanese"}, // Sudan
{"*urma", "*urmese"}, // Burma
{"*urus", "*urian"}, // Epidaurus
{"*qahn", "*qahni"},
{"*rgos", "*rgive"}, // Argos
{"*yros", "*yrian"}, // Nisyros
{"*[v]cis", "*[v]cian"}, // Phocis
{"*[c]cis", "*[c]cidian"}, // Chalcis

// 3 Letters
{"*ain", "*aini"}, // Bahrain
{"*ales", "*elsh"}, // Wales
{"*ame", "*amese"}, // Suriname
{"*ara", "*arian"}, // Megara
{"*ati", "*ati"}, // Kiribati
{"*bah", "*ban"},
{"*bes", "*ban"}, // Thebes
{"*car", "*can"}, // Madagascar
{"*cau", "*canese"}, // Macau
{"*dai", "*dan"}, // Oiniadai
{"*den", "*dish"}, // Sweden
{"*des", "*dan"}, // Oiniades
{"*dos", "*dian"}, // Barbados
{"*don", "*donian"}, // Sidon
{"*eia", "*ian"}, // Eleia
{"*ene", "*enian"}, // Cyrene
{"*eru", "*eruvian"}, // Peru
{"*gal", "*guese"}, // Portugal
{"*gac", "*gac"},
{"*iam", "*iamese"}, // Siam
{"*ini", "*inian"}, // Leontini
{"*ion", "*ian"}, // Rhegion
{"*ios", "*iot"}, // Chios
{"*ite", "*itian"}, // Melite
{"*jab", "*jabi"}, // Punjab
{"*jan", "*jani"}, // Azerbaijan
{"*kia", "*k"}, // Slovakia
{"*kim", "*kimese"}, // Sikkim
{"*kun", "*kunite"},
{"*lan", "*lanese"}, // Milan
{"*los", "*losian"}, // Pylos
{"*lta", "*ltese"}, // Malta
{"*man", "*mani"}, // Oman
{"*mas", "*mian"}, // Bahamas
{"*men", "*meni"}, // Yemen
{"*mir", "*miri"}, // Kashmir
{"*mis", "*minian"}, // Salamis
{"*mon", "*monian"}, // Lakedaemon
{"*mor", "*morese"}, // Timor
{"*nae", "*naean"}, // Mycenae
{"*nce", "*ntine"}, // Florence
{"*nik", "*nikian"},
{"*nes", "*ne"}, // Philippines
{"*num", "*ne"}, // Sabinum
{"*oon", "*oonian"}, // Cameroon
{"*pan", "*panese"}, // Japan
{"*pur", "*puri"}, // Manipur
{"*que", "*can"}, // Martinique
{"*qah", "*qian"},
{"*rii", "*rian"}, // Thurii
{"*ros", "*ran"}, // Comoros
{"*sey", "*sey"}, // Jersey
{"*sus", "*sian"}, // Ephesus
{"*tan", "*tanese"}, // Bhutan
{"*ton", "*tonian"}, // Croton
{"*tus", "*tian"}, // Carystus
{"*ura", "*uran"}, // Tripura
{"*use", "*usan"}, // Syracuse
{"*yon", "*yonian"}, // Sikyon (rule made up)
{"*yōn", "*yōnian"}, // Sikyōn (rule made up)
{"*zen", "*zenian"}, // Troezen
{"*[v]ng", "*[v]nger"}, // Hong Kong
{"*[v]os", "*[v]o"}, // Laos
{"*[v]us", "*[v]an"}, // Mauritius
{"*[c]ey", "*[c]ish"}, // Turkey
{"*[v]am", "*[v]amanian"}, // Guam
{"*[c]am", "*[c]amese"}, // Vietnam
{"*[c]us", "*[c]usian"}, // Belarus
{"*[v]ch", "*[v]chite"}, // Schech
{"*[v]um", "*[v]an"}, // Belgium
{"*[c]um", "*[c]ian"}, // Pergamum

// 2 Letters
{"*ab", "*abite"}, // Achtab (rule made up)
{"*ad", "*adian"}, // Chad
{"*ae", "*ian"}, // Colossae
{"*ah", "*ah"},
{"*aí", "*aian"}, // Kolōnaí (Colonae in Latin) -> Kolonaian (rule made up)
{"*ai", "*aian"}, // Kolōnai
{"*al", "*ali"}, // Nepal
{"*an", "*anian"}, // Jordan
{"*ao", "*aoan"}, // Curaçao
{"*ar", "*ari"}, // Qatar
{"*as", "*an"}, // Honduras
{"*at", "*ati"}, // Gujarat
{"*au", "*auan"}, // Palau
{"*co", "*can"}, // Morocco
{"*de", "*dean"}, // Cape Verde
{"*eh", "*ehi"},
{"*el", "*eli"}, // Israel
{"*em", "*emite"}, // Zarem (rule made up)
{"*en", "*enese"}, // 		Jan Mayen
{"*es", "*ian"}, // 		Maldives
{"*ge", "*ginian"}, // Carthage
{"*gh", "*ghi"},
{"*gk", "*gkan"},
{"*in", "*inese"}, // Benin
{"*im", "*imite"},
{"*is", "*ian"}, // Locris
{"*it", "*iti"}, // Kuwait
{"*le", "*lean"}, // Chile
{"*kh", "*khi"},
{"*ll", "*llese"}, // Marshall
{"*me", "*man"}, // Rome
{"*my", "*mois"}, // Saint Barthélemy
{"*na", "*nian"}, // Argentina
{"*nj", "*nji"},
{"*ny", "*n"}, // Germany
{"*oe", "*oese"}, // Faroe
{"*oh", "*ohan"},
{"*on", "*onese"}, // Gabon
{"*oy", "*ojan"}, // Troy
{"*os", "*ian"}, // Thasos
{"*ra", "*rean"}, // Corcyra
{"*re", "*rean"}, // Singapore
{"*rh", "*rhi"}, // Chandigarh
{"*ro", "*rin"}, // 		Montenegro
{"*rn", "*rnite"}, // made up: Arn -> Arnite
{"*sh", "*shi"}, // 		Bangladesh
{"*ta", "*tan"}, // Egesta
{"*te", "*tan"}, // Crete
{"*th", "*thian"},  // Vilath (rule made up)
{"*ue", "*uan"}, // Niue
{"*ul", "*ulish"}, // Gaul
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
}