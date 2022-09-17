
using Open.Collections;

namespace commonItems.Linguistics;

public static partial class StringExtensions {
	private static readonly OrderedDictionary<string, string> AdjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>
		
// 6+ characters
{"*aix-en-Provence", "*aixois"}, // Aix-en-Provence
{"*aguascalientes", "*hidrocálido"}, // Aguascalientes
{"Buenos Aires", "Porteño"}, // Buenos Aires
{"Bolgatanga", "Guruŋa"}, // Bolgatanga
{"Eswatini", "Swati"}, // Eswatini
{"Ad Pontem", "Pontan"},
{"Verahram Qal'eh", "Verahrami"},
{"Orkney Islands", "Orcadian"},
{"* Islands", "*"}, // Cook Islands
{"* Republic", "*"}, // Dominican Republic
{"* Union", "*"}, // Soviet Union
{"*emirates", "*emirati"}, // United Arab Emirates
{"*hmedabad", "*mdavadi"}, // Ahmedabad
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
{"*angers", "*anjou"}, // Angers
{"*ckland", "*ckland"}, // Auckland
{"*ford Roxo", "*forroxense"}, // Belford Roxo
{"* Horizonte", "*-Horizontino"}, // Belo Horizonte
{"*éziers", "*itterois"}, // Béziers
{"*caramanga", "*mangués"}, // Bucaramanga
{"*gayan de Oro", "*gayanon"}, // Cagayan de Oro
{"*campinas", "*campineiro"}, // Campinas
{"* Grande", "*-grandense"}, // Campo Grande
{"*tenham", "*tonian"}, // Cheltenham
{"*veland", "*velander"}, // Cleveland
{"*órdoba", "*ordobense"}, // Córdoba
{"*Épalinges", "*Palinsard"}, // Épalinges
{"Foz do Iguaçu", "Iguaçuense"}, // Foz do Iguaçu
{"*lasgow", "*laswegian"}, // Glasgow
{"*dalming", "*dhelmian"}, // Godalming
{"* Coast", "* Coast"}, // Gold Coast
{"Guangzhou", "Cantonese"}, // Guangzhou
{"*Iloilo", "*Ilonggo"}, // Iloilo
{"Indianapolis", "Hoosier"}, // Indianapolis
{"*qaluit", "*qalummiut"}, // Iqaluit
{"Iriga", "Irigueño"}, // Iriga
{"* de Fora", "*-forano"}, // Juiz de Fora
{"* Borough", "*ite"}, // Juneau Borough

// 5 characters
{"* City", "*ite"}, // Carson City
{"* Kong", "*kongese"}, // Hong Kong
{"* Town", "*tonian"}, // Cape Town
{"*allia", "*aulish"}, // Gallia
{"*anton", "*antonese"}, // Canton
{"*atium", "*atin"}, // Latium
{"*blois", "*blesois"}, // Blois
{"*burgh", "*burgensian"}, // Edinburgh
{"*cádiz", "*gaditano"}, // Cádiz
{"*china", "*chinese"}, // Indochina
{"*deaux", "*delais"}, // Bordeaux
{"*eland", "*elandic"}, // Iceland
{"*iacán", "*ichi"}, // Culiacán
{"*idale", "*idilian"}, // Armidale
{"*istan", "*"}, // Tajikistan
{"*geles", "*gelino"}, // Angeles
{"*grade", "*gradian"}, // Belgrade
{"*khand", "*khandi"}, // Jharkhand
{"*lades", "*ladian"}, // Cyclades
{"*mania", "*manic"}, // Germania
{"*nland", "*nlander"}, // Greenland
{"*noble", "*noblois"}, // Grenoble
{"*nzona", "*nzonese"}, // Bellinzona
{"*pelos", "*pelitan"}, // Skopelos
{"*pland", "*ppish"}, // Lapland
{"*polis", "*politan"}, // Annapolis
{"*quipa", "*quipeño"}, // Arequipa
{"*remen", "*remer"}, // Bremen
{"*rouge", "*rougean"}, // Baton Rouge
{"*rrhae", "*rrean"}, // Serrhae
{"*ryana", "*ryanvi"}, // Haryana
{"*shmir", "*shmiri"}, // Kashmir
{"*shtra", "*shtrian"}, // Maharashtra
{"*stein", "*steiner"}, // Liechtenstein
{"*şehir", "*shehirian"}, // Eskişehir
{"*tings", "*tingite"}, // Hastings
{"*thiye", "*thiyennese"}, // Fethiye
{"*tland", "*ttish"}, // Scotland
{"*troit", "*troiter"}, // Detroit
{"*ttium", "*ttian"}, // Bruttium
{"*uiana", "*uianese"}, // Guiana
{"*venia", "*vene"}, // Slovenia
{"*ville", "*villian"}, // Asheville
{"*wayne", "*wayner"}, // Fort Wayne
{"*xeter", "*xonian"}, // Exeter
{"*yotte", "*horan"}, // Mayotte
{"*zoram", "*zo"}, // Mizoram
{"*[c]ilia", "*[c]iliense"}, // Brasilia
{"*[c]land", "*[c]lish"}, // England
{"*[v]land", "*[v]lish"}, // Poland

// 4 characters
{"*aïda", "*idonian"}, // Saïda
{"*agde", "*agathois"}, // Agde
{"*ales", "*elsh"}, // Wales
{"*aman", "*amanian"}, // Dalaman
{"*ammu", "*ammu"}, // Jammu
{"*ance", "*ench"}, // France
{"*anon", "*anese"}, // Lebanon
{"*aras", "*arentine"}, // Taras
{"*aros", "*arian"}, // Paros
{"*bany", "*banian"}, // Albany
{"*bath", "*bathonian"}, // Bath
{"*berg", "*berger"}, // Nuremberg
{"*bira", "*birano"}, // Itabira
{"*blin", "*blin"}, // Dublin
{"*borg", "*borgenser"}, // Aalborg
{"*burg", "*burger"}, // Hamburg
{"*burn", "*burnian"}, // Blackburn
{"Cali", "Caleño"}, // Cali
{"Cebu", "Cebuano"}, // Cebu
{"*cese", "*cesan"}, // Diocese
{"*ción", "*ceno"}, // Asunción
{"*dale", "*dalian"}, // Fort Lauderdale
{"*dana", "*danite"}, // Adana
{"*deen", "*donian"}, // Aberdeen
{"*djan", "*djanais"}, // Abidjan
{"*dong", "*dong"}, // Andong
{"*dung", "*dungite"}, // Bandung
{"*echk", "*echk"},
{"*eece", "*eek"}, // Greece
{"*eese", "*eese"}, // 	Cheese
{"*egal", "*egalese"}, // Senegal
{"*enne", "*enneite"}, // Cheyenne
{"*elos", "*elian"}, // Delos
{"*etus", "*esian"},
{"*ford", "*fordian"}, // Bedford
{"*furt", "*furter"}, // Frankfurt
{"*gamo", "*gamasque"}, // Bergamo
{"*gana", "*ganite"}, // Telangana
{"*gena", "*genero"}, // Cartagena
{"*gill", "*gill"}, // Invercargill
{"*gina", "*ginetan"}, // Aegina
{"*gnac", "*gnac"},
{"*gong", "*gonian"}, // Chittagong
{"*gotá", "*gotano"}, // Bogotá
{"*graz", "*grazer"}, // Graz
{"*hana", "*hanaian"}, // Ghana
{"*hria", "*hriasian"}, // Thria
{"*iana", "*ian"}, // Bactriana
{"*ifax", "*igonian"}, // Halifax
{"*irns", "*irnsite"}, // Cairns
{"*irus", "*irote"}, // Epirus
{"*jing", "*jingese"}, // Beijing
{"*kara", "*karite"}, // Ankara
{"*king", "*kinese"}, // Peking
{"*lais", "*lais"}, // Calais
{"*lius", "*liasian"}, // Phlius
{"*lles", "*llois"}, // Seychelles
{"*long", "*long"}, // Geelong
{"*magh", "*machian"}, // Armagh
{"*mara", "*maran"}, // Asmara
{"*mmon", "*mmonite"}, // Ammon
{"*mnus", "*mnian"}, // Epidamnus
{"*nati", "*natian"}, // Cincinnati
{"*neia", "*nean"}, // Mantineia
{"*nium", "*nite"}, // Samnium
{"*ngal", "*ngali"}, // West Bengal
{"*nsck", "*nsck"},
{"*ntum", "*ntine"}, // Tarentum
{"*ntus", "*ntic"}, // Pontus
{"*odes", "*odian"}, // Rhodes
{"*orus", "*oran"}, // Bosporus
{"*pain", "*panish"}, // Spain
{"*pira", "*pirense"}, // Spain
{"*pool", "*pudlian"}, // Hartlepool
{"*prus", "*priote"}, // Cyprus
{"*qaba", "*qabawi"}, // Aqaba
{"*qahn", "*qahni"},
{"*quil", "*quilean"}, // Guayaquil
{"*rala", "*ralite"}, // Kerala
{"*rage", "*rageite"}, // Anchorage
{"*rara", "*rarese"}, // Ferrara
{"*rque", "*rquean"}, // Albuquerque
{"*rrae", "*rrean"}, // Serrae
{"*sden", "*sdener"}, // Dresden
{"*stan", "*"}, // Kazakhstan
{"*stin", "*stonian"}, // Austin
{"*tain", "*tish"}, // Great Britain
{"*than", "*thani"}, // Rajasthan
{"*thon", "*thonian"}, // Marathon
{"*tiba", "*tibano"}, // Curitiba
{"*tica", "*tic"}, // Antarctica
{"*tino", "*tinesi"}, // Filettino
{"*tium", "*tine"}, // Byzantium
{"*udan", "*udanese"}, // Sudan
{"*urma", "*urmese"}, // Burma
{"*urus", "*urian"}, // Epidaurus
{"*rgos", "*rgive"}, // Argos
{"*yros", "*yrian"}, // Nisyros
{"*vana", "*vanan"}, // Havana
{"*[v]cis", "*[v]cian"}, // Phocis
{"*[c]cis", "*[c]cidian"}, // Chalcis
{"*[c]ver", "*[c]ver"}, // Denver
{"*[v]ver", "*[v]verian"}, // Hanover

// 3 characters
{"* Lat", "*latese"}, // Da Lat
{"*ain", "*aini"}, // Bahrain
{"*ame", "*amese"}, // Suriname
{"*ara", "*arian"}, // Megara
{"*aro", "*arense"}, // Amparo
{"*ati", "*ati"}, // Kiribati
{"*bad", "*badi"}, // Hyderabad
{"*bah", "*ban"},
{"*bai", "*baite"}, // Dubai
{"*ban", "*ban"}, // Durban
{"*bes", "*ban"}, // Thebes
{"*bin", "*binite"}, // Harbin
{"*bul", "*bulite"}, // Istanbul
{"*bus", "*busite"}, // Columbus
{"*car", "*can"}, // Madagascar
{"*cau", "*canese"}, // Macau
{"*cow", "*covian"}, // Cracow
{"*cus", "*cene"}, // Damascus
{"*dai", "*dan"}, // Oiniadai
{"*dam", "*dammer"}, // Amsterdam
{"*dee", "*donian"}, // Dundee
{"*den", "*dish"}, // Sweden
{"*des", "*dan"}, // Oiniades
{"*dge", "*dgian"}, // Cambridge
{"*din", "*din"}, // Dunedin
{"*dın", "*dinian"}, // Aydın
{"*dod", "*dodi"}, // Ashdod
{"*dos", "*dian"}, // Barbados
{"*don", "*donian"}, // Sidon
{"*egu", "*egu"}, // Daegu
{"*eia", "*ian"}, // Eleia
{"*ene", "*enian"}, // Cyrene
{"*eru", "*eruvian"}, // Peru
{"*ett", "*ettite"}, // Everett
{"*gal", "*guese"}, // Portugal
{"*gac", "*gac"},
{"*gen", "*gener"}, // Copenhagen
{"*gna", "*gnese"}, // Bologna
{"*goa", "*goan"}, // Bossangoa
{"*gue", "*guer"}, // Hague
{"*ham", "*hamian"}, // Birmingham
{"*iam", "*iamese"}, // Siam
{"*ini", "*inian"}, // Leontini
{"*ion", "*ian"}, // Rhegion
{"*ios", "*iot"}, // Chios
{"*iro", "*irene"}, // Cairo
{"*ise", "*isean"}, // Boise
{"*ite", "*itian"}, // Melite
{"*jab", "*jabi"}, // Punjab
{"*jan", "*jani"}, // Azerbaijan
{"*kia", "*k"}, // Slovakia
{"*kim", "*kimese"}, // Sikkim
{"*kır", "*kirian"}, // Diyarbakır
{"*kun", "*kunite"},
{"*lan", "*lanese"}, // Milan
{"*las", "*lasite"}, // Dallas
{"*ley", "*leian"}, // Finchley
{"*lhi", "*lhiite"}, // Delhi
{"*lod", "*lodnon"}, // Bacolod
{"*lon", "*lonian"}, // Ashkelon
{"*los", "*losian"}, // Pylos
{"*lta", "*ltese"}, // Malta
{"*man", "*mani"}, // Oman
{"*mas", "*mian"}, // Bahamas
{"*men", "*meni"}, // Yemen
{"*mir", "*mirian"}, // Izmir
{"*mis", "*minian"}, // Salamis
{"*mit", "*mitite"}, // Salamis
{"*mon", "*monian"}, // Lakedaemon
{"*mor", "*morese"}, // Timor
{"*nae", "*naean"}, // Mycenae
{"*nai", "*naite"}, // Chennai
{"*nce", "*ntine"}, // Florence
{"*nik", "*nikian"},
{"*nes", "*ne"}, // Philippines
{"*num", "*ne"}, // Sabinum
{"*oon", "*oonian"}, // Cameroon
{"*pan", "*panese"}, // Japan
{"*poh", "*pohian"}, // Ipoh
{"*pur", "*puri"}, // Manipur
{"*que", "*can"}, // Martinique
{"*qah", "*qian"},
{"*rat", "*rati"}, // Gujarat
{"*rii", "*rian"}, // Thurii
{"*ros", "*ran"}, // Comoros
{"*rra", "*rran"}, // Canberra
{"*rry", "*rry"}, // Derry
{"*rsa", "*rsanese"}, // Bursa
{"*rum", "*rumian"}, // Bodrum
{"*rut", "*ruti"}, // Beirut
{"*sco", "*sco"}, // Cusco
{"*sel", "*sler"}, // Basel
{"*sey", "*sey"}, // Jersey
{"*sir", "*sirian"}, // Balıkesir
{"*sle", "*slian"}, // Carlisle
{"*sso", "*ssese"}, // Chiasso
{"*sus", "*sian"}, // Ephesus
{"*tan", "*tanese"}, // Bhutan
{"*tar", "*tarian"}, // Gibraltar
{"*ton", "*tonian"}, // Croton
{"*tep", "*tepian"}, // Gaziantep
{"*ter", "*trian"}, // Chester
{"*tte", "*ttean"}, // Charlotte
{"*tus", "*tian"}, // Carystus
{"*ura", "*uran"}, // Tripura
{"*use", "*usan"}, // Syracuse
{"*vao", "*vaoeño"}, // Davao
{"*way", "*way"}, // Galway
{"*win", "*winian"}, // Darwin
{"*won", "*wonian"}, // Changwon
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

// 2 characters
{"*ab", "*abite"}, // Achtab (rule made up)
{"*ad", "*adian"}, // Chad
{"*ae", "*ian"}, // Colossae
{"*ag", "*agian"}, // Castlecrag
{"*ah", "*ah"},
{"*aí", "*aian"}, // Kolōnaí (Colonae in Latin) -> Kolonaian (rule made up)
{"*ai", "*aian"}, // Kolōnai
{"*al", "*ali"}, // Nepal
{"*an", "*anian"}, // Jordan
{"*ao", "*aoan"}, // Curaçao
{"*ar", "*ari"}, // Qatar
{"*as", "*an"}, // Honduras
{"*at", "*atian"}, // Eilat
{"*au", "*auan"}, // Palau
{"*co", "*can"}, // Morocco
{"*de", "*dean"}, // Cape Verde
{"*eh", "*ehi"},
{"*el", "*eli"}, // Israel
{"*em", "*emite"}, // Zarem (rule made up)
{"*en", "*enese"}, // Jan Mayen
{"*én", "*enés"}, // Jaén
{"*eo", "*ean"}, // Bengeo
{"*er", "*erite"}, // Casper
{"*es", "*ian"}, // Maldives
{"*ge", "*ginian"}, // Carthage
{"*gh", "*ghi"},
{"*gk", "*gkan"},
{"*go", "*goan"}, // Chicago
{"*in", "*inese"}, // Benin
{"*im", "*imite"},
{"*is", "*ian"}, // Locris
{"*it", "*iti"}, // Kuwait
{"*ía", "*ian"}, // Almería
{"*ju", "*ju"}, // Gwangju
{"*fi", "*fitan"}, // Amalfi
{"*ff", "*ffian"}, // Cardiff
{"*le", "*lean"}, // Chile
{"*ka", "*kai"}, // Dhaka
{"*kh", "*khi"},
{"*ku", "*kuvian"}, // Baku
{"*ld", "*ldian"}, // Chesterfield
{"*ll", "*llese"}, // Marshall
{"*lo", "*lonian"}, // Buffalo
{"*lu", "*lan"}, // Honolulu
{"*me", "*man"}, // Rome
{"*mi", "*mite"}, // Gumi
{"*my", "*mois"}, // Saint Barthélemy
{"*na", "*nian"}, // Argentina
{"*nj", "*nji"},
{"*no", "*nan"}, // Fresno
{"*ny", "*n"}, // Germany
{"*oa", "*oese"}, // Genoa
{"*oe", "*oese"}, // Faroe
{"*oh", "*ohan"},
{"*on", "*onese"}, // Gabon
{"*oy", "*ojan"}, // Troy
{"*os", "*ian"}, // Thasos
{"*ów", "*ovian"}, // Kraków
{"*po", "*pine"}, // Aleppo
{"*ra", "*rean"}, // Corcyra
{"*re", "*rean"}, // Singapore
{"*rh", "*rhi"}, // Chandigarh
{"*rk", "*rk"}, // Cork
{"*ro", "*rin"}, // Montenegro
{"*rn", "*rnese"}, // Bern
{"*sa", "*san"}, // Larissa
{"*sh", "*shi"}, // Bangladesh
{"*ta", "*tan"}, // Egesta
{"*te", "*tan"}, // Crete
{"*th", "*thian"},  // Vilath (rule made up)
{"*to", "*tan"}, // Benevento
{"*ue", "*uan"}, // Niue
{"*ul", "*ulish"}, // Gaul
{"*vo", "*var"}, // Kosovo
{"*we", "*wean"}, // Zimbabwe
{"*yk", "*yk"}, // Karamyk (rule made up)
{"*ze", "*zean"}, // Belize
{"*[v]h", "*[v]hite"},
{"*[v]k", "*[v]kian"},
{"*[c]s", "*[c]ian"}, // Athens
{"*[v]y", "*[v]yan"}, // Paraguay
{"*[v]z", "*[v]zite"}, // Ziz (rule made up)

// 1 character
{"*a", "*an"}, // Libya
{"*d", "*der"}, // Dortmund
{"*e", "*ian"}, // Ukraine
{"*g", "*gish"}, // Luxembourg
{"*i", "*ian"}, // Burundi
{"*k", "*kian"},
{"*l", "*lian"}, // Brazil
{"*o", "*olese"}, // Congo
{"*q", "*qi"}, // Iraq
{"*r", "*rian"}, // Ecuador
{"*t", "*tian"}, // Egypt
{"*u", "*uan"}, // Vanuatu
{"*x", "*xian"}, // Essex
{"*y", "*ian"} // Hungary
	};
}