using commonItems.Linguistics;
using Csv;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
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
	[InlineData("Mahendragiri", "Mahendragiri")]
	[InlineData("Tsang", "Tsangpa")]
	[InlineData("Alpes Cottiae", "Cottian")]
	[InlineData("Alpes Graiae", "Grajan")]
	[InlineData("Tarim Basin", "Tarim")]
	[InlineData("South Tarim Basin", "South Tarim")]
	[InlineData("North Tarim Basin", "North Tarim")]
	[InlineData("Media Atropatene", "Atropatenian")]
	[InlineData("Atropatene", "Atropatenian")]
	[InlineData("Maritime Alps", "Maritimois")]
	[InlineData("Coele Syria", "Coelesyrian")]
	[InlineData("Laugaricio", "Laugarician")]
	[InlineData("Yonnav", "Yonnavite")]
	[InlineData("Baetica Gaditanus", "Gaditanian")]
	[InlineData("Baetica Hispalensis", "Hispalensian")]
	[InlineData("Baetica Cordubensis", "Cordubensian")]
	[InlineData("Western Delta", "Western Deltan")]  // "* Deltaic" would also be fine
	[InlineData("Central Delta", "Central Deltan")]
	[InlineData("Eastern Delta", "Eastern Deltan")]
	[InlineData("Dakshina Kosala", "Dakshina Kosalan")]  // Southern Kosalan would also be fine
	[InlineData("Alpes Carniae", "Alpine Carnian")] // Carnian would also be fine
	[InlineData("Alpes Raetiae", "Alpine Raetian")]  // Raetian/Rhaetian would also be fine
	[InlineData("Ultima Thule", "Ultima Thulean")] // Furthest Thulean/Thulian would also be fine
	[InlineData("Myos Hormos", "Myos")]
	[InlineData("Arambys", "Arambysian")]
	[InlineData("Guercif", "Guercifi")]
	[InlineData("Mcif", "Mcifi")]
	[InlineData("Maqomo", "Maqomo")]
	[InlineData("Frej", "Frejite")]
	[InlineData("Trelew", "Trelewense")]
	[InlineData("Nanaimo", "Nanaimoite")]
	[InlineData("Envigado", "Envigadeño")]
	[InlineData("Chigorodó", "Chigorodoseño")]
	[InlineData("San Bernardo", "San Bernardino")]
	[InlineData("Encantado", "Encantense")]
	[InlineData("Lincoln", "Lincolnite")]
	[InlineData("Charlottetown", "Charlottetonian")]
	[InlineData("Bulawayo", "Bulawayan")]
	[InlineData("Rondebosch", "Rondeboscher")]
	[InlineData("Grabouw", "Grabouwite")]
	[InlineData("Klerksdorp", "Klerksdorpian")]
	[InlineData("Ejido", "Ejidense")]
	[InlineData("Maracaibo", "Maracaibero")]
	[InlineData("Hazorasp", "Hazoraspian")]
	[InlineData("Kristinehamn", "Kristinehamner")]
	
	// adjectives relying on rewrite rules
	[InlineData("Armenia Maioris", "Greater Armenian")]
	[InlineData("Armenia Minoris", "Lesser Armenian")]
	[InlineData("Caria Superioris", "Upper Carian")]
	[InlineData("Germania Superior", "Upper Germanic")]
	[InlineData("Caria Inferioris", "Lower Carian")]
	[InlineData("Germania Inferior", "Lower Germanic")]
	[InlineData("Celtiberia Meridionalis", "South Celtiberian")]
	[InlineData("Celtiberia Septentrionalis", "North Celtiberian")]
	[InlineData("Alania Orientalis", "East Alanian")]
	[InlineData("Sardinia Borealis", "North Sardinian")]
	[InlineData("Media Magna", "Greater Median")]
	[InlineData("Ivernia Australis", "South Ivernian")]
	[InlineData("Alpes Maritimae", "Maritimois")]
	[InlineData("Celtiberia Centralis", "Central Celtiberian")]
	[InlineData("Pannonia Prima", "Proto-Pannonian")]
	[InlineData("Pannonia Secunda", "Deutero-Pannonian")]
	[InlineData("Pannonia Tertia", "Trito-Pannonian")]
	[InlineData("Caucasian Iberia", "Kartvelian")]
	[InlineData("Dacia Ripensis", "Riparian Dacian")]
	[InlineData("Phrygia Ripensis", "Riparian Phrygian")]
	[InlineData("Cilicia Pedias", "Smooth Cilician")]
	[InlineData("Alexandreia pros to Latmo", "Alexandrian")]
	[InlineData("Arabia Felix", "Felicio-Arabian")]
	[InlineData("Arabia Felix Centralis", "Central Felicio-Arabian")]
	[InlineData("Arabia Felix Occidentalis", "West Felicio-Arabian")]
	[InlineData("Arabia Felix Orientalis", "East Felicio-Arabian")]
	[InlineData("Arabia Ad Sinus", "Sinuo-Arabian")]
	[InlineData("Arabia Maritimae", "Thalasso-Arabian")]
	[InlineData("Arabia Petrea", "Petrean")]
	[InlineData("Arabia Petraea", "Petrean")]
	[InlineData("Arabia Relicta", "Relicto-Arabian")]
	[InlineData("Ptolemais Epithera", "Epitheran")]
	[InlineData("Phrygia Salutaris", "Hygio-Phrygian")]
	[InlineData("Treveria Salutaris", "Hygio-Treverian")]
	[InlineData("Bithynia et Paphlagonia", "Bithyno-Paphlagonian")]
	[InlineData("Illyria Graeca", "Graeco-Illyrian")]
	[InlineData("Illyria Greca", "Graeco-Illyrian")]
	[InlineData("Sarmatia Europea", "Euro-Sarmatian")]
	[InlineData("Sarmatia Asiatica", "Asio-Sarmatian")]  // Asiatic Sarmatian would also be fine
	[InlineData("Media Choromithrene", "Choromithrenian")] // Choromithrene would also be fine
	[InlineData("Suvarna Gotra", "Suvarna Gotran")]
	[InlineData("Phrygia Parorea", "Paroreo-Phrygian")]
	[InlineData("Cilicia Trachea", "Tracheo-Cilician")]
	[InlineData("Vallis Arni", "Arnese")]  // any other adjective derived from Arno would work
	[InlineData("Sarmatia Hyrcania", "Hyrcano-Sarmatian")]
	[InlineData("Caucasian Albania", "Albanian")] // Caucaso-Albanian would also be fine
	[InlineData("Scythia Transmontem", "Transmontano-Scythian")]
	[InlineData("Cappadocia Pontica", "Pontic Cappadocian")] // Pontico-Cappadocian would also be fine
	[InlineData("Cappadocia Taurica", "Tauric Cappadocian")]  // Taurico-Cappadocian would also be fine
	[InlineData("Phrygia Ulterioris", "Ulterior Phrygian")] // Further Phrygian would also be fine
	[InlineData("Scandia Insula", "Insular Scandian")]
	[InlineData("Scythia Ultima", "Furthest Scythian")]
	[InlineData("Maeotia Minores", "Lesser Maeotian")]

	// from https://en.wikipedia.org/wiki/List_of_adjectival_and_demonymic_forms_of_place_names#Regions_in_Greco-Roman_antiquity
	[InlineData("Acarnania", "Acarnanian")]
	[InlineData("Achaea", "Achaean")]
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
	[InlineData("Zhanatalap", "Zhanatalapian")]
	// ReSharper restore StringLiteralTypo
	public void GetAdjectiveGeneratesCorrectishAdjective(string noun, string expectedAdjective) {
		Assert.Equal(expectedAdjective, noun.GetAdjective());
	}

	[Theory]
	[InlineData("TestFiles/adjectives/cities.csv")] // https://en.wikipedia.org/wiki/Adjectivals_and_demonyms_for_cities
	[InlineData("TestFiles/adjectives/continents.csv")] // https://en.wikipedia.org/wiki/List_of_adjectival_and_demonymic_forms_of_place_names#Continents
	public void CorrectAdjectivesAreGeneratedForNamesFromCsv(string csvFilePath) {
		var csv = File.ReadAllText(csvFilePath);
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

	[Fact]
	public void AdjectiveRuleExistsForEveryCountryAndMajorCityInTheWorld() {
		var csvUrl = "https://datahub.io/core/world-cities/r/world-cities.csv";

		using var httpClient = new HttpClient();
		var request = new HttpRequestMessage(HttpMethod.Get, csvUrl);
		var response = httpClient.Send(request);
		using var reader = new StreamReader(response.Content.ReadAsStream());
		var csv = reader.ReadToEnd();
		
		var cities = CsvReader.ReadFromText(csv)
			.Select(line => line[0])
			.Where(city => !string.IsNullOrEmpty(city))
			.Where(city => !city.StartsWith("Zürich (Kreis"))
			.Where(city => !city.StartsWith("Sector "))
			.Distinct()
			.ToList();
		
		var countries = CsvReader.ReadFromText(csv)
			.Select(line => line[1])
			.Where(country => !string.IsNullOrEmpty(country))
			.Distinct()
			.ToList();

		var output = new StringWriter();
		Console.SetOut(output);

		foreach (var city in cities) {
			_ = city.GetAdjective();
		}

		foreach (var country in countries) {
			_ = country.GetAdjective();
		}
		output.ToString().Should().NotContain("No matching adjective rule found");
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

	[Theory]
	// ReSharper disable StringLiteralTypo
	[InlineData("Rome", "Rome")]
	[InlineData("Rome123", "Rome")]
	[InlineData("Rome###", "Rome")]
	[InlineData("Rome?!", "Rome")]
	[InlineData("Rome¡", "Rome")]
	// ReSharper restore StringLiteralTypo
	public void TrimNonLetterEndingReturnsCorrectValueForNonAlphanumericEndings(string str, string expectedValue) {
		Assert.Equal(expectedValue, str.TrimNonLetterEnding());
	}
}