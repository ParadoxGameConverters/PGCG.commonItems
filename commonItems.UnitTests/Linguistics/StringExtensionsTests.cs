﻿using commonItems.Linguistics;
using Csv;
using System.IO;
using System.Linq;
using Xunit;

namespace commonItems.UnitTests.Linguistics;

public class StringExtensionsTests {
	[Theory]
	// ReSharper disable StringLiteralTypo
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
	[InlineData("Eswatini", "Swati")]
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
	[InlineData("Gallaecia", "Gallaecian")]
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
	[InlineData("Mithymna", "Mithymnian")]
	[InlineData("Methymna", "Methymnian")]
	[InlineData("Moravia", "Moravian")]
	[InlineData("Mycenae", "Mycenaean")]
	[InlineData("Mytilene", "Mytilenian")]
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
	[InlineData("Salamis", "Salaminian")]
	[InlineData("Samnium", "Samnite")]
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
	
	// from https://en.wikipedia.org/wiki/List_of_adjectival_and_demonymic_forms_of_place_names#Indian_states_and_territories
	[InlineData("Andhra Pradesh", "Andhrulu")]
	[InlineData("Arunachal Pradesh", "Arunachali")]
	[InlineData("Assam", "Assamese")]
	[InlineData("Bihar", "Bihari")]
	[InlineData("Chandigarh", "Chandigarhi")]
	[InlineData("Chhattisgarh", "Chhattisgarhi")]
	[InlineData("Goa", "Goan")]
	[InlineData("Gujarat", "Gujarati")]
	[InlineData("Haryana", "Haryanvi")]
	[InlineData("Himachal Pradesh", "Himachali")]
	[InlineData("Jammu", "Jammu")]
	[InlineData("Kashmir", "Kashmiri")]
	[InlineData("Jharkhand", "Jharkhandi")]
	[InlineData("Karnataka", "Karnatakan")]
	[InlineData("Kerala", "Keralite")]
	[InlineData("Ladakh", "Ladakhi")]
	[InlineData("Lakshadweep", "Laccadivian")]
	[InlineData("Laccadives", "Laccadivian")]
	[InlineData("Madhya Pradesh", "Madhya Pradeshi")]
	[InlineData("Maharashtra", "Maharashtrian")]
	[InlineData("Manipur", "Manipuri")]
	[InlineData("Meghalaya", "Meghalayan")]
	[InlineData("Mizoram", "Mizo")]
	[InlineData("Nagaland", "Nagalandese")]
	[InlineData("Odisha", "Odishan")]
	[InlineData("Pondicherry", "Pondicherrian")]
	[InlineData("Punjab", "Punjabi")]
	[InlineData("Rajasthan", "Rajasthani")]
	[InlineData("Sikkim", "Sikkimese")]
	[InlineData("Tamil Nadu", "Tamil")]
	[InlineData("Telangana", "Telanganite")]
	[InlineData("Tripura", "Tripuran")]
	[InlineData("Uttar Pradesh", "Uttar Pradeshi")]
	[InlineData("Uttarakhand", "Uttarakhandi")]
	[InlineData("West Bengal", "West Bengali")]
	// ReSharper restore StringLiteralTypo
	public void GetAdjectiveGeneratesCorrectishAdjective(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}

	[Fact]
	public void CorrectAdjectivesAreGeneratedForCities() {
		var filePath = "TestFiles/adjectives/cities.csv";
		var csv = File.ReadAllText(filePath);
		foreach (var line in CsvReader.ReadFromText(csv)) {
			// Header is handled, each line will contain the actual row data.
			var name = line[0];
			var adj1 = line[1];
			var adj2 = line[2];
			var adj3 = line[3];
			var adj4 = line[4];
			var adj5 = line[5];

			var validAdjectives = new[] {adj1, adj2, adj3, adj4, adj5}
				.Where(adj=>!string.IsNullOrEmpty(adj));

			var generatedAdj = name.GetAdjective();
			Assert.Contains(generatedAdj, validAdjectives);
		}
	}

	[Theory]
	// ReSharper disable StringLiteralTypo
	[InlineData("Vilath#?!", "Vilathian")]
	[InlineData("Egypt.", "Egyptian")]
	// ReSharper restore StringLiteralTypo
	public void GetAdjectiveHandlesStringsWithNonAlphanumericEndings(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}
	
	[Theory]
	// ReSharper disable StringLiteralTypo
	[InlineData("Rome", "Rome")]
	[InlineData("Rome123", "Rome123")]
	[InlineData("Rome###", "Rome")]
	[InlineData("Rome?!", "Rome")]
	[InlineData("Rome¡", "Rome")]
	// ReSharper restore StringLiteralTypo
	public void TrimNonAlphanumericEndingReturnsCorrectValue(string str, string expectedValue) {
		Assert.Equal(expectedValue, str.TrimNonAlphanumericEnding());
	}
}