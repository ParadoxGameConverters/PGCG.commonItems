
using Open.Collections;

namespace commonItems.Linguistics;

public static partial class StringExtensions {
	private static readonly OrderedDictionary<string, string> AdjectiveRules = new() {
		// <ENDING, ADJ. SUFFIX>
		
// 6+ characters
{"Myos Hormos", "Myos"},
{"Baetica Cordubensis", "Cordubensian"}, // https://1library.co/article/cultura-y-pensamiento-estudios-medievales-hisp%C3%A1nicos.zlg664or
{"Baetica Gaditanus", "Gaditanian"}, // https://en.wiktionary.org/wiki/Gaditanian
{"Baetica Hispalensis", "Hispalensian"}, // https://quod.lib.umich.edu/e/eebo/A67489.0001.001/1:12.11?rgn=div2;view=fulltext
{"Coele Syria", "Coelesyrian"},
{"Alpes Carniae", "Alpine Carnian"},
{"Alpes Raetiae", "Alpine Raetian"},
{"Alpes Cottiae", "Cottian"},
{"Alpes Graiae", "Grajan"},
{"Maritime Alps", "Maritimois"},
{"*Tarim Basin", "*Tarim"}, // Tarim Basin
{"*Solihull", "*Silhillian"}, // Sendai
{"*Sendai", "*Sendaikko"}, // Sendai
{"*Sapporo", "*Sapporokko"}, // Sapporo
{"*São Paulo", "*Paulistano"}, // São Paulo
{"*São Luís", "*Ludovicense"}, // São Luís
{"*Santiago", "*Santiaguinos"}, // Santiago
{"*Santa Cruz de la Sierra", "*Cruceño"}, // Santa Cruz de la Sierra
{"*Şanlıurfa", "*Urfanian"}, // Şanlıurfa
{"*San Jose", "*San Josean"}, // San Jose
{"*Lake City", "*Lake"}, // Salt Lake City
{"*Quebec City", "*Québécois"}, // Quebec City
{"*r City", "*ri"}, // Rangpur City
{"*Aix-en-Provence", "*Aixois"}, // Aix-en-Provence
{"*Aguascalientes", "*Hidrocálido"}, // Aguascalientes
{"Buenos Aires", "Porteño"}, // Buenos Aires
{"Bolgatanga", "Guruŋa"}, // Bolgatanga
{"Eswatini", "Swati"}, // Eswatini
{"Ad Pontem", "Pontan"},
{"Verahram Qal'eh", "Verahrami"},
{"*Pacific Islands", "*Pacific Islands"}, // Pacific Islands
{"*Orkney Islands", "*Orcadian"}, // Orkney Islands
{"* Islands", "*"}, // Cook Islands
{"* Republic", "*"}, // Dominican Republic
{"* Union", "*"}, // Soviet Union
{"*Emirates", "*Emirati"}, // United Arab Emirates
{"*hmedabad", "*mdavadi"}, // Ahmedabad
{"* Qal'eh", "*"},
{"*[v]tzerland", "*[v]ss"}, // Switzerland
{"*Zealand", "*Zealander"}, // New Zealand
{"*Pakistan", "*Pakistani"}, // Pakistan
{"*enmark", "*anish"}, // Denmark
{"*inland", "*innish"}, // Finland
{"*reland", "*rish"}, // Ireland
{"*ailand", "*ai"}, // Thailand
{"*ontium", "*ontinian"}, // Leontium
{"*Serica", "*Sere"}, // Serica
{"*iohaemum", "*ius"}, // Boiohaemum
{"*fpaktos", "*upactian"}, // Nafpaktos
{"*Rithymna", "*Rhithymnian"}, // Rithymna
{"*Thuria", "*Thuriat"}, // Thuria
{"*aionia", "*aeonian"}, // Paionia
{"*dhra Pradesh", "*dhrulu"}, // Andhra Pradesh
{"*al Pradesh", "*ali"}, // Arunachal Pradesh
{"*kshadweep", "*ccadivian"}, // Lakshadweep
{"*galand", "*galandese"}, // Nagaland
{"* Nadu", "*"}, // Tamil Nadu
{"*Angers", "*Anjou"}, // Angers
{"*ckland", "*ckland"}, // Auckland
{"*ford Roxo", "*forroxense"}, // Belford Roxo
{"* Horizonte", "*-Horizontino"}, // Belo Horizonte
{"*éziers", "*itterois"}, // Béziers
{"*caramanga", "*mangués"}, // Bucaramanga
{"*gayan de Oro", "*gayanon"}, // Cagayan de Oro
{"*Campinas", "*Campineiro"}, // Campinas
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
{"*Khulna", "*Khulna"}, // Khulna
{"* Mirim", "*miriano"}, // Mogi Mirim
{"*e City", "*e Citian"}, // Traverse City
{"*k City", "*ker"}, // New York City
{"*u City", "*uite"}, // Juneau City
{"*[c] City", "*[c]ite"}, // Carson City
{"*[v] City", "*ite"}, // Oklahoma City
{"*tle upon Tyne", "*tlian"}, // Newcastle upon Tyne
{"*ton North", "*tonian"}, // Palmerston North
{"*Phnom Penh", "*Phnom Penh"}, // Phnom Penh
{"*a Arenas", "*arenian"}, // Punta Arenas
{"*otland", "*ottish"}, // Scotland
{"* Helier", "*-Helian"}, // Saint Helier
{"* Carlos", "*carlense"}, // São Carlos
{"*sbourg", "*sbourgeois"}, // Strasbourg
{"*hassee", "*hasseean"}, // Tallahassee
{"*Rivières", "*rivieran"}, // Trois-Rivières
{"* Lakes", "* Laker"}, // Twin Lakes
{"*Valladolid", "*Vallisoletano"}, // Valladolid
{"*Valparaíso", "*Porteño"}, // Valparaíso
{"*Warsaw", "*Varsovian"}, // Warsaw
{"*Xalapa", "*Xalapeño"}, // Xalapa
{"*gorica", "*gorician"}, // Podgorica

// 5 characters
{"* Kong", "*kongese"}, // Hong Kong
{"* Paul", "* Paulite"}, // Saint Paul
{"* Town", "*tonian"}, // Cape Town
{"*allia", "*aulish"}, // Gallia
{"*angal", "*angalite"}, // Warangal
{"*anton", "*antonese"}, // Canton
{"*atium", "*atin"}, // Latium
{"*ândia", "*andense"}, // Uberlândia
{"*Blois", "*Blesois"}, // Blois
{"*burgh", "*burgher"}, // Edinburgh
{"*Cádiz", "*Gaditano"}, // Cádiz
{"*china", "*chinese"}, // Indochina
{"*deaux", "*delais"}, // Bordeaux
{"*dence", "*dentian"}, // Providence
{"*Derry", "*Derry"}, // Derry
{"*eland", "*elandic"}, // Iceland
{"*enice", "*enetian"}, // Venice
{"*iacán", "*ichi"}, // Culiacán
{"*idale", "*idilian"}, // Armidale
{"*istan", "*"}, // Tajikistan
{"*geles", "*gelino"}, // Angeles
{"*grade", "*gradian"}, // Belgrade
{"*khand", "*khandi"}, // Jharkhand
{"*kland", "*klander"}, // Oakland
{"*knife", "*knifer"}, // Yellowknife
{"*lades", "*ladian"}, // Cyclades
{"*Leeds", "*Leodensian"}, // Leeds
{"*Louis", "*Louisan"}, // St. Louis
{"*mania", "*manic"}, // Germania
{"*njing", "*nkinese"}, // Nanjing
{"*nland", "*nlander"}, // Greenland
{"*noble", "*noblois"}, // Grenoble
{"*nzona", "*nzonese"}, // Bellinzona
{"*pelos", "*pelitan"}, // Skopelos
{"*pland", "*ppish"}, // Lapland
{"*polis", "*politan"}, // Annapolis
{"*quipa", "*quipeño"}, // Arequipa
{"*remen", "*remer"}, // Bremen
{"*rland", "*rlandian"}, // Sunderland
{"*Rouge", "*Rougean"}, // Baton Rouge
{"*rrhae", "*rrean"}, // Serrhae
{"*ryana", "*ryanvi"}, // Haryana
{"*Seoul", "*Seoulite"}, // Seoul
{"*shawe", "*shavian"}, // Wythenshawe
{"*shmir", "*shmiri"}, // Kashmir
{"*shtra", "*shtrian"}, // Maharashtra
{"*sland", "*slander"}, // Staten Island
{"*sogon", "*soganon"}, // Sorsogon
{"*stein", "*steiner"}, // Liechtenstein
{"*şehir", "*shehirian"}, // Eskişehir
{"*Tacna", "*Tacneño"}, // Tacna
{"*tings", "*tingite"}, // Hastings
{"*thiye", "*thiyennese"}, // Fethiye
{"*tland", "*tlander"}, // Portland
{"*troit", "*troiter"}, // Detroit
{"*Tsang", "*Tsangpa"}, // Tsang
{"*ttium", "*ttian"}, // Bruttium
{"*uiana", "*uianese"}, // Guiana
{"*ourne", "*urnian"}, // Melbourne
{"*venia", "*vene"}, // Slovenia
{"*ville", "*villian"}, // Asheville
{"*Wayne", "*Wayner"}, // Fort Wayne
{"*xeter", "*xonian"}, // Exeter
{"*yotte", "*horan"}, // Mayotte
{"*zoram", "*zo"}, // Mizoram
{"*[c]ilia", "*[c]iliense"}, // Brasilia
{"*[c]land", "*[c]lish"}, // England
{"*[v]land", "*[v]lish"}, // Poland

// 4 characters
{"* Bay", "*bite"}, // Thunder Bay
{"* Jaw", "* Jaw"}, // Moose Jaw
{"* Lat", "*latese"}, // Da Lat
{"*aïda", "*idonian"}, // Saïda
{"*Agde", "*Agathois"}, // Agde
{"*ales", "*elsh"}, // Wales
{"*alta", "*altese"}, // Malta
{"*aman", "*amanian"}, // Dalaman
{"*ammu", "*ammu"}, // Jammu
{"*ance", "*ench"}, // France
{"*ango", "*angoan"}, // Durango
{"*anon", "*anese"}, // Lebanon
{"*anya", "*anyan"}, // Netanya
{"*aras", "*arentine"}, // Taras
{"*aros", "*arian"}, // Paros
{"*atar", "*atari"}, // Qatar
{"*atte", "*atian"}, // Ville Platte
{"*baco", "*baqueno"}, // Tabaco
{"*bany", "*banian"}, // Albany
{"*bean", "*bean"}, // Caribbean
{"*Bath", "*Bathonian"}, // Bath
{"*berg", "*berger"}, // Nuremberg
{"*bira", "*birano"}, // Itabira
{"*blin", "*blin"}, // Dublin
{"*borg", "*borgenser"}, // Aalborg
{"*bouk", "*bouki"}, // Tabouk
{"*burg", "*burger"}, // Hamburg
{"*burn", "*burnian"}, // Blackburn
{"Cali", "Caleño"}, // Cali
{"*cara", "*carese"}, // Pescara
{"Cebu", "Cebuano"}, // Cebu
{"*cese", "*cesan"}, // Diocese
{"*ción", "*ceno"}, // Asunción
{"*ckay", "*ckayite"}, // Mackay
{"*dale", "*dalian"}, // Fort Lauderdale
{"*dana", "*danite"}, // Adana
{"*deen", "*donian"}, // Aberdeen
{"*Dhaka", "*Dhakai"}, // Dhaka
{"*dies", "*dian"}, // West Indies
{"*dina", "*dinan"}, // Medina
{"*djan", "*djanais"}, // Abidjan
{"*dney", "*dneysider"}, // Sydney
{"*dong", "*dong"}, // Andong
{"*dung", "*dungite"}, // Bandung
{"*East", "*Eastern"}, // Middle East
{"*echk", "*echk"},
{"*eece", "*eek"}, // Greece
{"*eens", "*eensite"}, // Queens
{"*eese", "*eese"}, // 	Cheese
{"*egal", "*egalese"}, // Senegal
{"*eigh", "*eighite"}, // Raleigh
{"*eiro", "*eiran"}, // Rio de Janeiro
{"*enna", "*ennese"}, // Siena
{"*enne", "*enneite"}, // Cheyenne
{"*elos", "*elian"}, // Delos
{"*etus", "*esian"}, // Miletus
{"*ford", "*fordian"}, // Bedford
{"*furt", "*furter"}, // Frankfurt
{"*gamo", "*gamasque"}, // Bergamo
{"*gano", "*ganese"}, // Lugano
{"*gana", "*ganite"}, // Telangana
{"*gena", "*genero"}, // Cartagena
{"*gill", "*gill"}, // Invercargill
{"*gina", "*ginan"}, // Regina
{"*gnac", "*gnac"},
{"*gong", "*gonian"}, // Chittagong
{"*gotá", "*gotano"}, // Bogotá
{"*Graz", "*Grazer"}, // Graz
{"*hana", "*hanaian"}, // Ghana
{"*hang", "*hang"}, // Pohang
{"*hria", "*hriasian"}, // Thria
{"*iana", "*ian"}, // Bactriana
{"*ifax", "*igonian"}, // Halifax
{"*irns", "*irnsite"}, // Cairns
{"*irus", "*irote"}, // Epirus
{"*jing", "*jingese"}, // Beijing
{"*kara", "*karite"}, // Ankara
{"*king", "*kinese"}, // Peking
{"*lais", "*lais"}, // Calais
{"*Lake", "*Laker"}, // Paddock Lake
{"*lius", "*liasian"}, // Phlius
{"*lles", "*llois"}, // Seychelles
{"*long", "*long"}, // Geelong
{"*lson", "*lson"}, // Nelson
{"*luga", "*luzhanin"}, // Kaluga
{"*magh", "*machian"}, // Armagh
{"*mara", "*maran"}, // Asmara
{"*maru", "*maruvian"}, // Oamaru
{"*mbai", "*mbaikar"}, // Mumbai
{"*mers", "*mersite"}, // Somers
{"*mmon", "*mmonite"}, // Ammon
{"*mnus", "*mnian"}, // Epidamnus
{"*mont", "*montarian"}, // Polmont
{"*myra", "*myrene"}, // Palmyra
{"*nada", "*nadan"}, // Granada
{"*naji", "*njimite"}, // Panaji
{"*naka", "*nakade"}, // Larnaka
{"*nang", "*nangite"}, // Penang
{"*nati", "*natian"}, // Cincinnati
{"*ncie", "*nsonian"}, // Muncie
{"*neia", "*nean"}, // Mantineia
{"*nich", "*nchner"}, // Munich
{"*nisa", "*nisanian"}, // Manisa
{"*nium", "*nite"}, // Samnium
{"*ngal", "*ngali"}, // West Bengal
{"*nsck", "*nsck"},
{"*ntum", "*ntine"}, // Tarentum
{"*ntus", "*ntic"}, // Pontus
{"*oder", "*odran"}, // Shkoder
{"*odes", "*odian"}, // Rhodes
{"*olle", "*ollian"}, // Zwolle
{"*onto", "*ontonian"}, // Toronto
{"*orto", "*ortuense"}, // Porto
{"*orus", "*oran"}, // Bosporus
{"*pain", "*panish"}, // Spain
{"*phou", "*fitis"}, // Morphou
{"*pira", "*pirense"}, // Spain
{"*poli", "*politan"}, // Tripoli
{"*pool", "*pudlian"}, // Hartlepool
{"*prus", "*priote"}, // Cyprus
{"*qaba", "*qabawi"}, // Aqaba
{"*qahn", "*qahni"},
{"*quil", "*quilean"}, // Guayaquil
{"*rala", "*ralite"}, // Kerala
{"*rage", "*rageite"}, // Anchorage
{"*rara", "*rarese"}, // Ferrara
{"*rdin", "*rdinian"}, // Mardin
{"*real", "*realer"}, // Montreal
{"*rick", "*rick"}, // Limerick
{"*rona", "*ronese"}, // Verona
{"*rque", "*rquean"}, // Albuquerque
{"*rrae", "*rrean"}, // Serrae
{"*sbon", "*sboner"}, // Lisbon
{"*sden", "*sdener"}, // Dresden
{"*Side", "*Sidetan"}, // Side
{"*sovo", "*sovar"}, // Kosovo
{"*stan", "*"}, // Kazakhstan
{"*stin", "*stonian"}, // Austin
{"*tain", "*tish"}, // Great Britain
{"*taka", "*takan"}, // Karnataka
{"*than", "*thani"}, // Rajasthan
{"*thon", "*thonian"}, // Marathon
{"*tiba", "*tibano"}, // Curitiba
{"*tica", "*tic"}, // Antarctica
{"*tino", "*tinesi"}, // Filettino
{"*tium", "*tine"}, // Byzantium
{"*ttan", "*ttanite"}, // Manhattan
{"*udan", "*udanese"}, // Sudan
{"*uğla", "*ughlanian"}, // Muğla
{"*urma", "*urmese"}, // Burma
{"*urus", "*urian"}, // Epidaurus
{"*usco", "*usco"}, // Cusco
{"*reto", "*retano"}, // Ribeirão Preto
{"*rgos", "*rgive"}, // Argos
{"*rich", "*richer"}, // Zurich
{"*rnai", "*rnaisian"}, // Tournai
{"*yros", "*yrian"}, // Nisyros
{"*vana", "*vanan"}, // Havana
{"*vara", "*varese"}, // Novara
{"*Waco", "*Wacoite"}, // Waco
{"*[v]cis", "*[v]cian"}, // Phocis
{"*[c]cis", "*[c]cidian"}, // Chalcis
{"*[c]nai", "*[c]naite"}, // Chennai
{"*[v]ris", "*[v]risian"}, // Marmaris
{"*[c]ver", "*[c]ver"}, // Denver
{"*[v]ver", "*[v]verian"}, // Hanover

// 3 characters
{"*ada", "*adian"}, // Ponferrada
{"*ain", "*aini"}, // Bahrain
{"*all", "*allite"}, // Marshall
{"*ame", "*amese"}, // Suriname
{"*ara", "*arian"}, // Megara
{"*aro", "*arense"}, // Amparo
{"*ati", "*ati"}, // Kiribati
{"*aus", "*auense"}, // Manaus
{"*awa", "*awian"}, // Oshawa
{"*bad", "*badi"}, // Hyderabad
{"*bah", "*ban"},
{"*bai", "*baite"}, // Dubai
{"*ban", "*ban"}, // Durban
{"*bes", "*ban"}, // Thebes
{"*bin", "*binite"}, // Harbin
{"*bon", "*bonese"}, // Gabon
{"*bul", "*bulite"}, // Istanbul
{"*bus", "*busite"}, // Columbus
{"*cah", "*cahan"}, // Paducah
{"*car", "*can"}, // Madagascar
{"*cau", "*canese"}, // Macau
{"*chi", "*chiite"}, // Karachi
{"*cio", "*cian"}, // Laugaricio
{"*cow", "*covian"}, // Cracow
{"*cus", "*cene"}, // Damascus
{"*dağ", "*danian"}, // Tekirdağ
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
{"*eer", "*eerian"}, // Red Deer
{"*ego", "*egan"}, // San Diego
{"*egu", "*egu"}, // Daegu
{"*eia", "*ian"}, // Eleia
{"*ene", "*enian"}, // Cyrene
{"*eno", "*enoite"}, // Reno
{"*eru", "*eruvian"}, // Peru
{"*ett", "*ettite"}, // Everett
{"*evo", "*evan"}, // Sarajevo
{"*gal", "*guese"}, // Portugal
{"*gac", "*gac"},
{"*gen", "*gener"}, // Copenhagen
{"*gna", "*gnese"}, // Bologna
{"*Goa", "*Goan"}, // Goa
{"*goa", "*goan"}, // Bossangoa
{"*gos", "*gosian"}, // Lagos
{"*gra", "*gran"}, // Serra Negra
{"*gue", "*guer"}, // Hague
{"*ham", "*hamian"}, // Birmingham
{"*har", "*hari"}, // Bihar
{"*iam", "*iamese"}, // Siam
{"*ica", "*ican"}, // Corsica
{"*ice", "*icean"}, // Nice
{"*ier", "*ieran"}, // Napier
{"*iev", "*ievan"}, // Kiev
{"*ing", "*ingite"}, // Kuching
{"*ini", "*inian"}, // Leontini
{"*inz", "*inzer"}, // Linz
{"*ion", "*ian"}, // Rhegion
{"*ios", "*iot"}, // Chios
{"*iri", "*iri"}, // Mahendragiri
{"*iro", "*irene"}, // Cairo
{"*ise", "*isean"}, // Boise
{"*ite", "*itian"}, // Melite
{"*jab", "*jabi"}, // Punjab
{"*jan", "*jani"}, // Azerbaijan
{"*kee", "*keean"}, // Milwaukee
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
{"*loo", "*luvian"}, // Waterloo
{"*los", "*losian"}, // Pylos
{"*lta", "*ltan"}, // Western Delta
{"*mah", "*mi"}, // Tihamah
{"*man", "*mani"}, // Oman
{"*mas", "*mian"}, // Bahamas
{"*men", "*meni"}, // Yemen
{"*mir", "*mirian"}, // Izmir
{"*mis", "*minian"}, // Salamis
{"*mit", "*mitite"}, // Izmit
{"*mno", "*mnian"}, // Rethymno
{"*mon", "*monian"}, // Lakedaemon
{"*mor", "*morese"}, // Timor
{"*mot", "*motter"}, // Wilmot
{"*nae", "*naean"}, // Mycenae
{"*nah", "*nian"}, // Savannah
{"*nce", "*ntine"}, // Florence
{"*nik", "*nikian"},
{"*nio", "*nian"}, // San Antonio
{"*nes", "*ne"}, // Philippines
{"*ngh", "*nghi"}, // Mymensingh
{"*ngo", "*ngolese"}, // Congo
{"*nto", "*ntine"}, // Sorrento
{"*num", "*ne"}, // Sabinum
{"*nya", "*nyanite"}, // Konya
{"*oke", "*oker"}, // Roanoke
{"*oon", "*oonian"}, // Cameroon
{"*osu", "*osuite"}, // Yeosu
{"*pan", "*panese"}, // Japan
{"*peg", "*pegger"}, // Winnipeg
{"*pes", "*pine"}, // Alpes
{"*pje", "*pjan"}, // Skopje
{"*poh", "*pohian"}, // Ipoh
{"*pur", "*puri"}, // Manipur
{"*que", "*can"}, // Martinique
{"*qah", "*qian"},
{"*ral", "*ralite"}, // Liberal
{"*rat", "*rati"}, // Gujarat
{"*res", "*rean"}, // Serres
{"*rid", "*rilenian"}, // Madrid
{"*rii", "*rian"}, // Thurii
{"*rma", "*rmesan"}, // Parma
{"*rmo", "*rmitan"}, // Palermo
{"*rno", "*rnese"}, // Locarno
{"*ros", "*ran"}, // Comoros
{"*rra", "*rran"}, // Canberra
{"*rry", "*rrian"}, // Pondicherry
{"*rsa", "*rsanese"}, // Bursa
{"*rum", "*rumian"}, // Bodrum
{"*rut", "*ruti"}, // Beirut
{"*sco", "*scan"}, // San Francisco
{"*sel", "*sler"}, // Basel
{"*sey", "*sey"}, // Jersey
{"*sin", "*sinite"}, // Mersin
{"*sio", "*siense"}, // Mendrisio
{"*sir", "*sirian"}, // Balıkesir
{"*sle", "*slian"}, // Carlisle
{"*slo", "*slovian"}, // Oslo
{"*son", "*sonian"}, // Tucson
{"*sso", "*ssese"}, // Chiasso
{"*sus", "*sian"}, // Ephesus
{"*tan", "*tanese"}, // Bhutan
{"*tar", "*tarian"}, // Gibraltar
{"*ton", "*tonian"}, // Croton
{"*tep", "*tepian"}, // Gaziantep
{"*ter", "*trian"}, // Chester
{"*tle", "*tlian"}, // Newcastle
{"*tra", "*tran"}, // Suvarna Gotra
{"*tte", "*ttean"}, // Charlotte
{"*tus", "*tian"}, // Carystus
{"*umi", "*umite"}, // Gumi
{"*ura", "*uran"}, // Tripura
{"*use", "*usan"}, // Syracuse
{"*val", "*valois"}, // Laval
{"*vao", "*vaoeño"}, // Davao
{"*viv", "*vivian"}, // Lviv
{"*vor", "*vorite"}, // Trevor
{"*way", "*way"}, // Galway
{"*win", "*winian"}, // Darwin
{"*won", "*wonian"}, // Changwon
{"*yiv", "*yivan"}, // Kyiv
{"*yon", "*yonian"}, // Sikyon (rule made up)
{"*yōn", "*yōnian"}, // Sikyōn (rule made up)
{"*zag", "*zagite"}, // Vizag
{"*zen", "*zenian"}, // Troezen
{"*zig", "*ziger"}, // Leipzig
{"*zon", "*zonian"}, // Trabzon
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
{"*as", "*an"}, // Honduras
{"*at", "*atian"}, // Eilat
{"*au", "*auan"}, // Palau
{"*ca", "*can"}, // Africa
{"*ce", "*cian"}, // Thrace
{"*co", "*can"}, // Morocco
{"*de", "*dean"}, // Cape Verde
{"*du", "*dunite"}, // Ordu
{"*eh", "*ehi"},
{"*el", "*eli"}, // Israel
{"*em", "*emite"}, // Zarem (rule made up)
{"*en", "*enese"}, // Jan Mayen
{"*én", "*enés"}, // Jaén
{"*eo", "*ean"}, // Bengeo
{"*er", "*erite"}, // Casper
{"*es", "*ian"}, // Maldives
{"*ge", "*ginian"}, // Carthage
{"*gh", "*ghian"}, // Middlesbroughian
{"*gk", "*gkan"},
{"*go", "*goan"}, // Chicago
{"*in", "*inese"}, // Benin
{"*im", "*imite"},
{"*is", "*ian"}, // Locris
{"*it", "*iti"}, // Kuwait
{"*ix", "*ician"}, // Phoenix
{"*ía", "*ian"}, // Almería
{"*ík", "*ikian"}, // Reykjavík
{"*jh", "*jhi"}, // Wajh
{"*ju", "*ju"}, // Gwangju
{"*fi", "*fitan"}, // Amalfi
{"*ff", "*ffian"}, // Cardiff
{"*le", "*lean"}, // Chile
{"*ka", "*kan"}, // Osaka
{"*kh", "*khi"},
{"*ku", "*kuvian"}, // Baku
{"*ld", "*ldian"}, // Chesterfield
{"*ll", "*llese"}, // Marshall
{"*lo", "*lonian"}, // Buffalo
{"*lu", "*lan"}, // Honolulu
{"*me", "*man"}, // Rome
{"*mi", "*mian"}, // Symi
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
{"*pa", "*pan"}, // Tampa
{"*pe", "*pean"}, // Europe
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
{"*vo", "*voan"}, // Provo
{"*we", "*wean"}, // Zimbabwe
{"*ye", "*yer"}, // Rye
{"*yk", "*yk"}, // Karamyk (rule made up)
{"*ze", "*zean"}, // Belize
{"*[v]h", "*[v]hite"},
{"*[v]k", "*[v]kian"},
{"*[c]s", "*[c]ian"}, // Athens
{"*[v]y", "*[v]yan"}, // Paraguay
{"*[v]z", "*[v]zite"}, // Ziz (rule made up)

// 1 character
{"*a", "*an"}, // Libya
{"*b", "*bian"}, // Zagreb
{"*d", "*der"}, // Dortmund
{"*e", "*ian"}, // Ukraine
{"*g", "*gish"}, // Luxembourg
{"*i", "*ian"}, // Burundi
{"*k", "*kian"},
{"*l", "*lian"}, // Brazil
{"*q", "*qi"}, // Iraq
{"*r", "*rian"}, // Ecuador
{"*t", "*tian"}, // Egypt
{"*u", "*uan"}, // Vanuatu
{"*v", "*vite"}, // Yonnav
{"*x", "*xian"}, // Essex
{"*y", "*ian"} // Hungary
	};
	// Same as adjective rules, but are matched multiple times.
	private static readonly OrderedDictionary<string, string> AdjectiveRewriteRules = new() {
		{"Arabia Petrea", "Petrea"}, // https://en.wikipedia.org/wiki/Arabia_Petraea
{"Arabia Petraea", "Petrea"}, // https://en.wikipedia.org/wiki/Arabia_Petraea
{"Alpes Maritimae", "Maritime Alps"},
{"Media Atropatene", "Atropatene"},
{"Media Choromithrene", "Choromithrene"},
{"Caucasian Iberia", "Kartvelia"},
{"Ptolemais Epithera", "Epitheros"}, // http://www.perseus.tufts.edu/hopper/text?doc=Perseus%3Atext%3A1999.04.0006%3Aentry%3Dptolemais-theron
{"Bithynia et Paphlagonia", "Bithyno-Paphlagonia"}, // https://dergipark.org.tr/tr/download/article-file/1715241
{"Vallis Arni", "Arno"}, // means "valley of Arno"
{"Caucasian Albania", "Albania"}, // https://en.wikipedia.org/wiki/Caucasian_Albania

{"* Ultima", "Furthest *"}, // Scythia Ultima
{"* Insula", "Insular *"}, // Scandia Insula
{"* Ulterioris", "Ulterior *"}, // Phrygia Ulterioris
{"* Pontica", "Pontic *"}, // Cappadocia Pontica
{"* Taurica", "Tauric *"}, // Cappadocia Taurica
{"* Transmontem", "Transmontano-*"}, // Scythia Transmontem
{"* Asiatica", "Asio-*"}, // Sarmatia Asiatica
{"* Europea", "Euro-*"}, // Sarmatia Europea
{"* Hyrcania", "Hyrcano-*"}, // Sarmatia Hyrcania
{"* Parorea", "Paroreo-*"}, // Phrygia Parorea
{"* Trachea", "Tracheo-*"}, // Cilicia Trachea
{"* Felix", "Felicio-*"}, // Arabia Felix
{"* Felix Centralis", "Central Felicio-*"},  // Arabia Felix Centralis
{"* Felix Occidentalis", "West Felicio-*"},  // Arabia Felix Occidentalis
{"* Felix Orientalis", "East Felicio-*"},  // Arabia Felix Orientalis
{"* Minoris", "Lesser *"}, // Armenia Minoris
{"* Minores", "Lesser *"}, // Maeotia Minores
{"* Maioris", "Greater *"}, // Armenia Maioris
{"* Superioris", "Upper *"}, // Caria Superioris
{"* Inferioris", "Lower *"}, // Caria Inferioris
{"* Borealis", "North *"}, // Sardinia Borealis
{"* Septentrionalis", "North *"}, // Celtiberia Septentrionalis
{"* Meridionalis", "South *"}, // Celtiberia Meridionalis
{"* Australis", "South *"}, // Ivernia Australis
{"* Orientalis", "East *"}, // Alania Orientalis
{"* Centralis", "Central *"}, // Celtiberia Centralis
{"* Magna", "Greater *"}, // Media Magna
{"* Ripensis", "Riparian *"}, // Dacia Ripensis
{"* Prima", "Proto-*"}, // Armenia Prima
{"* Secunda", "Deutero-*"}, // Armenia Secunda
{"* Tertia", "Trito-*"}, // Armenia Tertia
{"* Pedias", "Smooth *"}, // Cilicia Pedias
{"* Ad Sinus", "Sinuo-*"}, // Arabia Ad Sinus
{"* Maritimae", "Thalasso-*"}, // Arabia Maritimae
{"* Relicta", "Relicto-*"}, // Arabia Relicta
{"* Salutaris", "Hygio-*"},  // Phrygia Salutaris
{"* Greca", "Graeco-*"},  // Illyria Greca
{"* Graeca", "Graeco-*"},

{"* pros to Latmo", "*"} // Alexandreia pros to Latmo
	};
}