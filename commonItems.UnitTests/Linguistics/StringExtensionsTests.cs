﻿using commonItems.Linguistics;
using Xunit;

namespace commonItems.UnitTests.Linguistics;

public class StringExtensionsTests {
	[Theory]
	[InlineData("Verahram Qal'eh", "Verahrami")]
	[InlineData("Ad Pontem", "Pontan")]
	[InlineData("Cook Islands", "Cook")]
	[InlineData("Dominican Republic", "Dominican")]
	[InlineData("Soviet Union", "Soviet")]
	[InlineData("Scotland", "Scottish")]
	[InlineData("Lapland", "Lappish")]
	[InlineData("Finland", "Finnish")]
	[InlineData("England", "English")]
	[InlineData("Poland", "Polish")]
	[InlineData("Iceland", "Icelandic")]
	[InlineData("Switzerland", "Swiss")]
	[InlineData("Thailand", "Thai")]
	[InlineData("France", "French")]
	[InlineData("Lebanon", "Lebanese")]
	[InlineData("Nuremberg", "Nuremberger")]
	[InlineData("Hamburg", "Hamburger")]
	[InlineData("Indochina", "Indochinese")]
	[InlineData("Greece", "Greek")]
	[InlineData("Senegal", "Senegalese")]
	[InlineData("United Arab Emirates", "United Arab Emirati")]
	[InlineData("Denmark", "Danish")]
	[InlineData("Ghana", "Ghanaian")]
	[InlineData("Tajikistan", "Tajik")]
	[InlineData("Seychelles", "Seychellois")]
	[InlineData("Spain", "Spanish")]
	[InlineData("Pakistan", "Pakistani")]
	[InlineData("Ireland", "Irish")]
	[InlineData("Kazakhstan", "Kazakh")]
	[InlineData("Liechtenstein", "Liechtensteiner")]
	[InlineData("Great Britain", "Great British")]
	[InlineData("Antarctica", "Antarctic")]
	[InlineData("Sudan", "Sudanese")]
	[InlineData("Guiana", "Guianese")]
	[InlineData("Burma", "Burmese")]
	[InlineData("Slovenia", "Slovene")]
	[InlineData("Mayotte", "Mahoran")]
	[InlineData("New Zealand", "New Zealander")]
	[InlineData("Diocese", "Diocesan")]
	[InlineData("Vietnam", "Vietnamese")]
	[InlineData("Belarus", "Belarusian")]
	[InlineData("Guam", "Guamanian")]
	[InlineData("Laos", "Lao")]
	[InlineData("Mauritius", "Mauritian")]
	[InlineData("Bahrain", "Bahraini")]
	[InlineData("Wales", "Welsh")]
	[InlineData("Suriname", "Surinamese")]
	[InlineData("Kiribati", "Kiribati")]
	[InlineData("Madagascar", "Madagascan")]
	[InlineData("Macau", "Macanese")]
	[InlineData("Sweden", "Swedish")]
	[InlineData("Barbados", "Barbadian")]
	[InlineData("Peru", "Peruvian")]
	[InlineData("Cheese", "Cheese")] // 🤔
	[InlineData("Portugal", "Portuguese")]
	[InlineData("Eswatini", "Eswati")]
	[InlineData("Azerbaijan", "Azerbaijani")]
	[InlineData("Slovakia", "Slovak")]
	[InlineData("Milan", "Milanese")]
	[InlineData("Oman", "Omani")]
	[InlineData("Bahamas", "Bahamian")]
	[InlineData("Yemen", "Yemeni")]
	[InlineData("Timor", "Timorese")]
	[InlineData("Florence", "Florentine")]
	[InlineData("Philippines", "Philippine")]
	[InlineData("Cameroon", "Cameroonian")]
	[InlineData("Japan", "Japanese")]
	[InlineData("Martinique", "Martinican")]
	[InlineData("Comoros", "Comoran")]
	[InlineData("Jersey", "Jersey")]
	[InlineData("Turkey", "Turkish")]
	[InlineData("Bhutan", "Bhutanese")]
	[InlineData("Hong Kong", "Hong Konger")]
	[InlineData("Paraguay", "Paraguayan")]
	[InlineData("Chad", "Chadian")]
	[InlineData("Nepal", "Nepali")]
	[InlineData("Jordan", "Jordanian")]
	[InlineData("Curaçao", "Curaçaoan")]
	[InlineData("Qatar", "Qatari")]
	[InlineData("Honduras", "Honduran")]
	[InlineData("Palau", "Palauan")]
	[InlineData("Morocco", "Moroccan")]
	[InlineData("Cape Verde", "Cape Verdean")]
	[InlineData("Israel", "Israeli")]
	[InlineData("Jan Mayen", "Jan Mayenese")]
	[InlineData("Maldives", "Maldivian")]
	[InlineData("Benin", "Beninese")]
	[InlineData("Kuwait", "Kuwaiti")]
	[InlineData("Chile", "Chilean")]
	[InlineData("Marshall", "Marshallese")]
	[InlineData("Saint Barthélemy", "Saint Barthélemois")]
	[InlineData("Argentina", "Argentinian")]
	[InlineData("Germany", "German")]
	[InlineData("Faroe", "Faroese")]
	[InlineData("Gabon", "Gabonese")]
	[InlineData("Singapore", "Singaporean")]
	[InlineData("Montenegro", "Montenegrin")]
	[InlineData("Bangladesh", "Bangladeshi")]
	[InlineData("Malta", "Maltese")]
	[InlineData("Niue", "Niuan")]
	[InlineData("Belgium", "Belgian")]
	[InlineData("Kosovo", "Kosovar")]
	[InlineData("Zimbabwe", "Zimbabwean")]
	[InlineData("Belize", "Belizean")]
	[InlineData("Libya", "Libyan")]
	[InlineData("Dortmund", "Dortmunder")]
	[InlineData("Ukraine", "Ukrainian")]
	[InlineData("Luxembourg", "Luxembourgish")]
	[InlineData("Burundi", "Burundian")]
	[InlineData("Brazil", "Brazilian")]
	[InlineData("Congo", "Congolese")]
	[InlineData("Iraq", "Iraqi")]
	[InlineData("Ecuador", "Ecuadorian")]
	[InlineData("Egypt", "Egyptian")]
	[InlineData("Vanuatu", "Vanuatuan")]
	[InlineData("Essex", "Essexian")]
	[InlineData("Hungary", "Hungarian")]
	[InlineData("Myanik", "Myanikian")]
	[InlineData("Mizdaqahn", "Mizdaqahni")]
	[InlineData("Schech", "Schechite")]
	[InlineData("Ziz", "Zizite")]
	[InlineData("Karamyk", "Karamyk")]
	[InlineData("Sikyōn", "Sikyōnian")]
	[InlineData("Sikyon", "Sikyonian")]
	[InlineData("Vilath", "Vilathian")]
	[InlineData("Kolōnaí", "Kolōnaian")]
	[InlineData("Kolōnai", "Kolōnaian")]
	[InlineData("Zarem", "Zaremite")]
	[InlineData("Achtab", "Achtabite")]
	[InlineData("Arn", "Arnite")]
	[InlineData("Mandigac", "Mandigac")]
	[InlineData("Aghaechk", "Aghaechk")]
	[InlineData("Bielsk", "Bielskian")]
	[InlineData("Baqbaqah", "Baqbaqian")]
	[InlineData("Abaskun", "Abaskunite")]
	[InlineData("Makarabah", "Makaraban")]
	[InlineData("Kurnsck", "Kurnsck")]
	[InlineData("Sahim", "Sahimite")]
	[InlineData("Shethigk", "Shethigkan")]
	[InlineData("Oghlu Qal'eh", "Oghlu")]
	[InlineData("Siam", "Siamese")]
	[InlineData("Rabigh", "Rabighi")]
	[InlineData("Saliah", "Saliah")]
	[InlineData("Khureh", "Khurehi")]
	[InlineData("Kariglukh", "Kariglukhi")]
	[InlineData("Bhawaniganj", "Bhawaniganji")]
	[InlineData("Damoh", "Damohan")]
	
	// from https://en.wikipedia.org/wiki/List_of_adjectival_and_demonymic_forms_of_place_names#Regions_in_Greco-Roman_antiquity
	[InlineData("Acarnania", "Acarnanian")]
	[InlineData("Achaea", "Achaean")]
	[InlineData("Aegina", "Aeginetan")]
	[InlineData("Aethaea", "Aethaean")]
	[InlineData("Aetolia", "Aetolian")]
	[InlineData("Andalusia", "Andalusian")]
	[InlineData("Apulia", "Apulian")]
	[InlineData("Aquitania", "Aquitanian")]
	[InlineData("Arcadia", "Arcadian")]
	[InlineData("Argos", "Argive")]
	[InlineData("Arretium", "Arretine")]
	[InlineData("Athens", "Athenian")]
	[InlineData("Bactria", "Bactrian")]
	[InlineData("Bactriana", "Bactrian")]
	[InlineData("Bavaria", "Bavarian")]
	[InlineData("Boeotia", "Boeotian")]
	[InlineData("Boiohaemum", "Boius")]
	[InlineData("Bosporus", "Bosporan")]
	[InlineData("Bosphorus", "Bosphoran")]
	[InlineData("Bruttium", "Bruttian")]
	[InlineData("Byzantium", "Byzantine")]
	[InlineData("Calabria", "Calabrian")]
	[InlineData("Campania", "Campanian")]
	[InlineData("Cantabria", "Cantabrian")]
	[InlineData("Caria", "Carian")]
	[InlineData("Carthage", "Carthaginian")]
	[InlineData("Carystus", "Carystian")]
	[InlineData("Catalonia", "Catalonian")]
	[InlineData("Cephalonia", "Cephalonian")]
	[InlineData("Chalcis", "Chalcidian")]
	[InlineData("Chios", "Chiot")]
	[InlineData("Colchis", "Colchian")]
	[InlineData("Colossae", "Colossian")]
	[InlineData("Consentia", "Consentian")]
	[InlineData("Corcyra", "Corcyrean")]
	[InlineData("Corsica", "Corsican")]
	[InlineData("Crete", "Cretan")]
	[InlineData("Croton", "Crotonian")]
	[InlineData("Cyclades", "Cycladian")]
	[InlineData("Cyprus", "Cypriote")]
	[InlineData("Cyrenaica", "Cyrenaican")]
	[InlineData("Cyrene", "Cyrenian")]
	[InlineData("Dacia", "Dacian")]
	[InlineData("Dalmatia", "Dalmatian")]
	[InlineData("Delos", "Delian")]
	[InlineData("Dodecanese", "Dodecanesian")]
	[InlineData("Edonia", "Edonian")]
	[InlineData("Egesta", "Egestan")]
	[InlineData("Eleusina", "Eleusinian")]
	[InlineData("Eleusis", "Eleusian")]
	[InlineData("Elis", "Elian")]
	[InlineData("Eleia", "Elian")]
	[InlineData("Ephesus", "Ephesian")]
	[InlineData("Epidamnus", "Epidamnian")]
	[InlineData("Epidamnos", "Epidamnian")]
	[InlineData("Epidaurus", "Epidaurian")]
	[InlineData("Epirus", "Epirote")]
	[InlineData("Eretria", "Eretrian")]
	[InlineData("Etruria", "Etrurian")]
	[InlineData("Euboea", "Euboean")]
	[InlineData("Galatia", "Galatian")]
	[InlineData("Gallaecia", "Gallaecus")]
	[InlineData("Gallia", "Gaulish")]
	[InlineData("Gaul", "Gaulish")]
	[InlineData("Germania", "Germanic")]
	[InlineData("Iberia", "Iberian")]
	[InlineData("Illyria", "Illyrian")]
	[InlineData("Ionia", "Ionian")]
	[InlineData("Kalymnos", "Kalymnian")]
	[InlineData("Kaulonia", "Kaulonian")]
	[InlineData("Knossos", "Knossian")]
	[InlineData("Lakedaimon", "Lakedaimonian")]
	[InlineData("Lakedaimonia", "Lakedaimonian")]
	[InlineData("Lakedaemon", "Lakedaemonian")]
	[InlineData("Lakedaemonia", "Lakedaemonian")]
	[InlineData("Larissa", "Larissan")]
	[InlineData("Latium", "Latin")]
	[InlineData("Leontini", "Leontinian")]
	[InlineData("Leontium", "Leontinian")]
	[InlineData("Lesbos", "Lesbian")]
	[InlineData("Locris", "Locrian")]
	[InlineData("Lucania", "Lucanian")]
	[InlineData("Lydia", "Lydian")]
	[InlineData("Macedonia", "Macedonian")]
	[InlineData("Maeonia", "Maeonian")]
	[InlineData("Mantineia", "Mantinean")]
	[InlineData("Marathon", "Marathonian")]
	[InlineData("Media", "Median")]
	[InlineData("Megara", "Megarian")]
	[InlineData("Melite", "Melitian")]
	[InlineData("Melos", "Melian")]
	[InlineData("Mesopotamia", "Mesopotamian")]
	[InlineData("Messenia", "Messenian")]
	[InlineData("Miletus", "Milesian")]
	[InlineData("Milesian", "Methymnian")]
	[InlineData("Methymna", "Methymnian")]
	[InlineData("Moravia", "Moravian")]
	[InlineData("Mycenae", "Mycenaean")]
	[InlineData("Mytilene", "Mytilenean")]
	[InlineData("Naupactus", "Naupactian")]
	[InlineData("Nafpaktos", "Naupactian")]
	[InlineData("Naxos", "Naxian")]
	[InlineData("Nisyros", "Nisyrian")]
	[InlineData("Oea", "Oean")]
	[InlineData("Olympia", "Olympian")]
	[InlineData("Oiniades", "Oiniadan")]
	[InlineData("Oiniadai", "Oiniadan")]
	[InlineData("Orkney Islands", "Orcadian")]
	[InlineData("Paeonia", "Paeonian")]
	[InlineData("Paionia", "Paeonian")]
	[InlineData("Pamphylia", "Pamphylian")]
	[InlineData("Paros", "Parian")]
	[InlineData("Patmos", "Patmian")]
	[InlineData("Peloponnese", "Peloponnesian")]
	[InlineData("Pergamum", "Pergamian")]
	[InlineData("Persia", "Persian")]
	[InlineData("Philippi", "Philippian")]
	[InlineData("Phlius", "Phliasian")]
	[InlineData("Phocis", "Phocian")]
	[InlineData("Phoenicia", "Phoenician")]
	[InlineData("Phrygia", "Phrygian")]
	[InlineData("Pisidia", "Pisidian")]
	[InlineData("Pontus", "Pontic")]
	[InlineData("Pylos", "Pylosian")]
	[InlineData("Rhegion", "Rhegian")]
	[InlineData("Rhodes", "Rhodian")]
	[InlineData("Rhithymna", "Rhithymnian")]
	[InlineData("Rhithymnia", "Rhithymnian")]
	[InlineData("Rithymna", "Rhithymnian")]
	[InlineData("Rome", "Roman")]
	[InlineData("Sabinum", "Sabine")]
	[InlineData("Salamis", "Salamis")]
	[InlineData("Samnium", "Samnium")]
	[InlineData("Samos", "Samian")]
	[InlineData("Sardinia", "Sardinian")]
	[InlineData("Sardis", "Sardian")]
	[InlineData("Sarmatia", "Sarmatian")]
	[InlineData("Scythia", "Scythian")]
	[InlineData("Serrae", "Serrean")]
	[InlineData("Serrhae", "Serrean")]
	[InlineData("Serica", "Sere")]
	[InlineData("Sicily", "Sicilian")]
	[InlineData("Sicyon", "Sicyonian")]
	[InlineData("Sidon", "Sidonian")]
	[InlineData("Saïda", "Sidonian")]
	[InlineData("Silesia", "Silesian")]
	[InlineData("Skopelos", "Skopelitan")]
	[InlineData("Sparta", "Spartan")]
	[InlineData("Suebia", "Suebian")]
	[InlineData("Symi", "Symian")]
	[InlineData("Syracuse", "Syracusan")]
	[InlineData("Taras", "Tarentine")]
	[InlineData("Tarentum", "Tarentine")]
	[InlineData("Tegea", "Tegean")]
	[InlineData("Tenedos", "Tenedian")]
	[InlineData("Tenedos", "Tenedian")]
	[InlineData("Thasos", "Thasian")]
	[InlineData("Thebes", "Theban")]
	[InlineData("Thespis", "Thespian")]
	[InlineData("Thessaly", "Thessalian")]
	[InlineData("Thrace", "Thracian")]
	[InlineData("Thria", "Thriasian")]
	[InlineData("Thuria", "Thuriat")]
	[InlineData("Thurii", "Thurian")]
	[InlineData("Thynia", "Thynian")]
	[InlineData("Trichonos", "Trichonian")]
	[InlineData("Troezen", "Troezenian")]
	[InlineData("Troy", "Trojan")]
	[InlineData("Umbria", "Umbrian")]
	[InlineData("Xanthi", "Xanthian")]
	[InlineData("Zakynthos", "Zakynthian")]

	public void GetAdjectiveGeneratesCorrectishAdjective(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}

	[Theory]
	[InlineData("Vilath#?!", "Vilathian")]
	[InlineData("Egypt.", "Egyptian")]
	public void GetAdjectiveHandlesStringsWithNonAlphanumericEndings(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}
	
	[Theory]
	[InlineData("Rome", "Rome")]
	[InlineData("Rome123", "Rome123")]
	[InlineData("Rome###", "Rome")]
	[InlineData("Rome?!", "Rome")]
	[InlineData("Rome¡", "Rome")]
	public void TrimNonAlphanumericEndingReturnsCorrectValue(string str, string expectedValue) {
		Assert.Equal(expectedValue, str.TrimNonAlphanumericEnding());
	}
}