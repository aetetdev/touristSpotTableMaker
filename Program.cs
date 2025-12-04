using Dapper;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Web;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Tourist places fetching started...\n");

        string connectionString = @"Server=.\DEVSQLSERVER;Database=TourAppDB;Trusted_Connection=True;";

        var cities = new List<string>
        {
            "Adana","Adıyaman","Afyonkarahisar","Ağrı","Amasya","Ankara","Antalya",
            "Artvin","Aydın","Balıkesir","Bilecik","Bingöl","Bitlis","Bolu","Burdur",
            "Bursa","Çanakkale","Çankırı","Çorum","Denizli","Diyarbakır","Edirne",
            "Elazığ","Erzincan","Erzurum","Eskişehir","Gaziantep","Giresun","Gümüşhane",
            "Hakkari","Hatay","Isparta","Mersin","İstanbul","İzmir","Kars","Kastamonu",
            "Kayseri","Kırklareli","Kırşehir","Kocaeli","Konya","Kütahya","Malatya",
            "Manisa","Kahramanmaraş","Mardin","Muğla","Muş","Nevşehir","Niğde",
            "Ordu","Rize","Sakarya","Samsun","Siirt","Sinop","Sivas","Tekirdağ",
            "Tokat","Trabzon","Tunceli","Şanlıurfa","Uşak","Van","Yozgat","Zonguldak",
            "Aksaray","Bayburt","Karaman","Kırıkkale","Batman","Şırnak","Bartın",
            "Ardahan","Iğdır","Yalova","Karabük","Kilis","Osmaniye","Düzce"
        };

        var metroDistricts = new Dictionary<string, List<string>>
        {
            ["İstanbul"] = new List<string> { "Adalar", "Bakırköy", "Beşiktaş", "Beykoz", "Beyoğlu", "Çatalca", "Esenler", "Esenyurt", "Eyüpsultan", "Fatih", "Gaziosmanpaşa", "Güngören", "Kadıköy", "Kağıthane", "Kartal", "Küçükçekmece", "Maltepe", "Pendik", "Sancaktepe", "Sarıyer", "Şile", "Şişli", "Sultanbeyli", "Sultangazi", "Tuzla", "Ümraniye", "Üsküdar", "Zeytinburnu" },
            ["Ankara"] = new List<string> { "Altındağ", "Ayaş", "Bala", "Çamlıdere", "Çankaya", "Elmadağ", "Etimesgut", "Evren", "Gölbaşı", "Haymana", "Kalecik", "Kazan", "Keçiören", "Kızılcahamam", "Mamak", "Nallıhan", "Polatlı", "Pursaklar", "Sincan", "Şereflikoçhisar", "Yenimahalle" },
            ["İzmir"] = new List<string> { "Aliağa", "Balçova", "Bayındır", "Bayraklı", "Bergama", "Beydağ", "Bornova", "Buca", "Çeşme", "Çiğli", "Dikili", "Foça", "Gaziemir", "Güzelbahçe", "Karabağlar", "Karşıyaka", "Kemalpaşa", "Kınık", "Konak", "Menderes", "Menemen", "Narlıdere", "Ödemiş", "Seferihisar", "Selçuk", "Tire", "Torbalı", "Urla" },
            ["Antalya"] = new List<string> { "Alanya", "Akseki", "Aksu", "Demre", "Döşemealtı", "Elmalı", "Finike", "Gazipaşa", "Gündoğmuş", "İbradı", "Kaş", "Kale", "Kemer", "Konyaaltı", "Korkuteli", "Kumluca", "Manavgat", "Muratpaşa", "Serik" },
            ["Bursa"] = new List<string> { "Nilüfer", "Osmangazi", "Yıldırım", "Gemlik", "İnegöl", "İznik", "Karacabey", "Kestel", "Orhaneli", "Orhangazi", "Mustafakemalpaşa", "Mudanya", "Yenişehir", "Büyükorhan" },
            ["Adana"] = new List<string> { "Aladağ", "Ceyhan", "Çukurova", "Feke", "İmamoğlu", "Karaisalı", "Karataş", "Kozan", "Pozantı", "Saimbeyli", "Seyhan", "Tufanbeyli", "Yumurtalık", "Yüreğir" },
            ["Konya"] = new List<string> { "Ahırlı", "Akören", "Akşehir", "Altınekin", "Beyşehir", "Bozkır", "Cihanbeyli", "Çeltik", "Çumra", "Derbent", "Derebucak", "Doğanhisar", "Emirgazi", "Ereğli", "Güneysınır", "Hadim", "Halkapınar", "Haşhaşlı", "Hüyük", "Ilgın", "Kadınhanı", "Karapınar", "Karatay", "Kulu", "Meram", "Sarayönü", "Selçuklu", "Seydişehir", "Taşkent", "Tuzlukçu", "Yalıhüyük", "Yunak" },
            ["Gaziantep"] = new List<string> { "Araban", "İslahiye", "Nizip", "Oğuzeli", "Şahinbey", "Karkamış", "Nurdağı", "Yavuzeli" },
            ["Mersin"] = new List<string> { "Akdeniz", "Anamur", "Aydıncık", "Bozyazı", "Çamlıyayla", "Erdemli", "Gülnar", "Mezitli", "Mut", "Tarsus", "Toroslar", "Silifke" },
            ["Kayseri"] = new List<string> { "Kocasinan", "Melikgazi", "Talas", "Hacılar", "Bünyan", "Develi", "Felahiye", "İncesu", "Pınarbaşı", "Sarıoğlan", "Sarız", "Tomarza", "Yahyalı", "Yeşilhisar" },
            ["Samsun"] = new List<string> { "Atakum", "Canik", "İlkadım", "Ondokuzmayıs", "Tekkeköy", "Alaçam", "Asarcık", "Bafra", "Çarşamba", "Havza", "Kavak", "Ladik", "Salıpazarı", "Vezirköprü", "Yakakent" },
            ["Trabzon"] = new List<string> { "Akçaabat", "Araklı", "Arsin", "Beşikdüzü", "Çarşıbaşı", "Çaykara", "Dernekpazarı", "Düzköy", "Hayrat", "Köprübaşı", "Maçka", "Of", "Ortahisar", "Sürmene", "Şalpazarı", "Tonya", "Vakfıkebir", "Yomra" },
            ["Balıkesir"] = new List<string> { "Altıeylül", "Karesi", "Ayvalık", "Balya", "Bandırma", "Bigadiç", "Burhaniye", "Dursunbey", "Edremit", "Erdek", "Gömeç", "Gönen", "Havran", "İvrindi", "Kepsut", "Manyas", "Marmara", "Savaştepe", "Sındırgı", "Susurluk" },
            ["Eskişehir"] = new List<string> { "Odunpazarı", "Tepebaşı", "Alpu", "Beylikova", "Çifteler", "Günyüzü", "Han", "Mihalıççık", "Mihalgazi", "Sarıcakaya", "Seyitgazi", "Sivrihisar" },
            ["Denizli"] = new List<string> { "Acıpayam", "Babadağ", "Baklan", "Bekilli", "Beyağaç", "Bozkurt", "Buldan", "Çal", "Çameli", "Çardak", "Çivril", "Güney", "Honaz", "Kale", "Merkezefendi", "Pamukkale", "Sarayköy", "Tavas" },
            ["Malatya"] = new List<string> { "Battalgazi", "Darende", "Doğanşehir", "Hekimhan", "Kale", "Kuluncak", "Pütürge", "Yazıhan", "Yeşilyurt", "Akçadağ", "Arguvan", "Doğanyol", "Doğanşehir", "Gürpınar", "Kovancılar", "Arapgir" },
            ["Sivas"] = new List<string> { "Merkez", "Altınyayla", "Divriği", "Doğanşar", "Gemerek", "Gölova", "Hafik", "İmranlı", "Kangal", "Koyulhisar", "Suşehri", "Şarkışla", "Ulaş", "Yıldızeli", "Zara" },
            ["Ordu"] = new List<string> { "Altınordu", "Akkuş", "Akköy", "Aybastı", "Fatsa", "Gölköy", "Gülyalı", "Gürgentepe", "İkizce", "Kabadüz", "Kabataş", "Korgan", "Kumru", "Mesudiye", "Perşembe", "Ulubey", "Ünye" },
            ["Hatay"] = new List<string> { "Antakya", "Arsuz", "Defne", "Dörtyol", "Hassa", "İskenderun", "Kırıkhan", "Payas", "Reyhanlı", "Samandağ", "Yayladağı", "Erzin", "Kumlu", "Altınözü" },
            ["Aydın"] = new List<string> { "Efeler", "Bozdoğan", "Çine", "Germencik", "İncirliova", "Karpuzlu", "Köşk", "Kuşadası", "Nazilli", "Söke", "Sultanhisar", "Didim", "Kuyucak", "Karacasu", "Koçarlı" },
            ["Tekirdağ"] = new List<string> { "Çerkezköy", "Çorlu", "Ergene", "Hayrabolu", "Malkara", "Marmaraereğlisi", "Muratlı", "Saray", "Süleymanpaşa", "Şarköy" },
            ["Kocaeli"] = new List<string> { "İzmit", "Gebze", "Gölcük", "Kandıra", "Karamürsel", "Kartepe", "Derince", "Başiskele", "Çayırova", "Darıca", "Dilovası", "Körfez" },
            ["Sakarya"] = new List<string> { "Adapazarı", "Akyazı", "Arifiye", "Erenler", "Ferizli", "Karapürçek", "Karasu", "Kaynarca", "Kocaali", "Pamukova", "Sapanca", "Serdivan", "Söğütlü", "Taraklı" },
            ["Bolu"] = new List<string> { "Merkez", "Dörtdivan", "Gerede", "Göynük", "Kıbrıscık", "Mengen", "Mudurnu", "Seben", "Yeniçağa" },
            ["Aksaray"] = new List<string> { "Merkez", "Ağaçören", "Eskil", "Gülağaç", "Güzelyurt", "Ortaköy", "Sarıyahşi" },
            ["Batman"] = new List<string> { "Merkez", "Beşiri", "Gercüş", "Hasankeyf", "Kozluk", "Sason" },
            ["Elazığ"] = new List<string> { "Merkez", "Ağın", "Alacakaya", "Arıcak", "Baskil", "Karakoçan", "Keban", "Kovancılar", "Maden", "Palu", "Sivrice" },
            ["Diyarbakır"] = new List<string> { "Bağlar", "Bismil", "Çermik", "Çınar", "Dicle", "Ergani", "Hani", "Hazro", "Kayapınar", "Kocaköy", "Kulp", "Lice", "Silvan", "Sur", "Yenişehir" }
        };

        int totalInserted = 0;

        foreach (var city in cities)
        {
            List<string> areas = metroDistricts.ContainsKey(city)
                ? metroDistricts[city]
                : new List<string> { city };

            int cityInserted = 0;

            foreach (var area in areas)
            {
                int inserted = await FetchAndInsertPlacesWithRetry(city, area, connectionString);
                cityInserted += inserted;

                Console.WriteLine($"[{city} / {area}] -> {inserted} place(s) added.");
            }

            totalInserted += cityInserted;
            Console.WriteLine($"City {city} finished: {cityInserted} places inserted.\n");
        }

        Console.WriteLine($"\nAll done! Total places inserted: {totalInserted}");

        await photoFetcher();
    }

    static async Task photoFetcher()
    {
        Console.WriteLine("Photo fetcher started...\n");

        string connectionString = @"Server=.\DEVSQLSERVER;Database=TourAppDB;Trusted_Connection=True;";

        using var db = new SqlConnection(connectionString);

        // Photo değişkeni boş olan kayıtları al
        var places = await db.QueryAsync<dynamic>(
            "SELECT Id, Coordinates, Name FROM TouristPlaces WHERE Photo IS NULL OR Photo = ''"
        );

        int updatedCount = 0;

        foreach (var place in places)
        {
            string name = place.Name;
            string coords = place.Coordinates;
            if (string.IsNullOrWhiteSpace(coords)) continue;

            var p = coords.Split(',');

            double lat = double.Parse($"{p[0]}.{p[1]}", System.Globalization.CultureInfo.InvariantCulture);
            double lon = double.Parse($"{p[2]}.{p[3]}", System.Globalization.CultureInfo.InvariantCulture);

            try
            {
                string wikidataId = await GetWikidataId(lat, lon);
                if (wikidataId == null)
                {
                    Console.WriteLine($"[Skip] No wikidata for {name}, {coords}");
                    continue;
                }

                string photoUrl = await GetWikidataPhoto(wikidataId);
                if (photoUrl == null)
                {
                    Console.WriteLine($"[Skip] No photo for {name},{wikidataId}");
                    continue;
                }

                await db.ExecuteAsync(
                    "UPDATE TouristPlaces SET Photo = @Photo WHERE Id = @Id",
                    new { Photo = photoUrl, Id = place.Id }
                );

                updatedCount++;

                Console.WriteLine($"[OK] Photo saved for Name: {name}, ID {place.Id}: {photoUrl}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] for {name}, {coords} -> {ex.Message}");
            }

            await Task.Delay(1500); // API ban yememek için
        }

        Console.WriteLine($"\nCompleted. {updatedCount} records updated.");
    }

    static async Task<string> GetWikidataId(double lat, double lon)
    {
        string url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&extratags=1";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        string json = await client.GetStringAsync(url);
        var obj = JObject.Parse(json);

        string wikidata = obj["extratags"]?["wikidata"]?.ToString();
        return wikidata;
    }

    static async Task<string> GetWikidataPhoto(string wikidataId)
    {
        string url = $"https://www.wikidata.org/wiki/Special:EntityData/{wikidataId}.json";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        string json = await client.GetStringAsync(url);
        var root = JObject.Parse(json);

        // P18: image
        var img = root["entities"]?[wikidataId]?["claims"]?["P18"]?[0]?["mainsnak"]?["datavalue"]?["value"]?.ToString();
        if (img == null) return null;

        string encoded = HttpUtility.UrlEncode(img);

        return $"https://commons.wikimedia.org/wiki/Special:FilePath/{encoded}";
    }

    static async Task<int> FetchAndInsertPlacesWithRetry(string city, string area, string connStr, int retries = 10)
    {
        int delay = 10000;
        string[] mirrors =
        {
            "https://overpass-api.de/api/interpreter",
            "https://lz4.overpass-api.de/api/interpreter",
            "https://overpass.kumi.systems/api/interpreter"
        };

        for (int attempt = 1; attempt <= retries; attempt++)
        {
            foreach (var mirror in mirrors)
            {
                try
                {
                    return await FetchAndInsertPlaces(city, area, connStr, mirror);
                }
                catch (Exception ex) when (attempt < retries)
                {
                    Console.WriteLine($"Retry {attempt} for {city}/{area} on {mirror}. Waiting {delay / 1000}s. Reason: {ex.Message}");
                    Thread.Sleep(delay);
                    delay *= 2;
                }
            }
        }

        Console.WriteLine($"FAILED: {city}/{area} after {retries} retries.");
        return 0;
    }

    static async Task<int> FetchAndInsertPlaces(string city, string area, string connStr, string baseUrl)
    {
        string query = $@"[out:json][timeout:180];
            area[""name""=""{area}""][""boundary""=""administrative""]->.a;
            (
                node[""tourism""](area.a);
                way[""tourism""](area.a);
                relation[""tourism""](area.a);
            );
            out center;";

        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        string url = $"{baseUrl}?data={Uri.EscapeDataString(query)}";
        string response = await client.GetStringAsync(url);
        var list = JObject.Parse(response)["elements"] as JArray;

        if (list == null) return 0;

        using var conn = new SqlConnection(connStr);
        int count = 0;

        foreach (var item in list)
        {
            string name = item["tags"]?["name"]?.ToString();
            if (string.IsNullOrWhiteSpace(name)) continue;

            string lat = item["lat"]?.ToString() ?? item["center"]?["lat"]?.ToString();
            string lon = item["lon"]?.ToString() ?? item["center"]?["lon"]?.ToString();
            if (lat == null || lon == null) continue;

            bool isMetro = area != city;

            // Büyükşehir
            string district = isMetro ? area : city;
            string subdistrict = item["tags"]?["addr:suburb"]?.ToString()
                ?? item["tags"]?["addr:neighbourhood"]?.ToString()
                ?? item["tags"]?["addr:village"]?.ToString()
                ?? "Merkez";

            string coordinates = $"{lat},{lon}";
            string category = item["tags"]?["tourism"]?.ToString()
                ?? item["tags"]?["amenity"]?.ToString()
                ?? item["tags"]?["shop"]?.ToString()
                ?? "unknown";

            await conn.ExecuteAsync(
                @"INSERT INTO TouristPlaces
                (Country, City, District, Subdistrict, Coordinates, Name, Category)
                VALUES (@Country, @City, @District, @Subdistrict, @Coordinates, @Name, @Category)",
                new
                {
                    Country = "Turkey",
                    City = city,
                    District = district,
                    Subdistrict = subdistrict,
                    Coordinates = coordinates,
                    Name = name,
                    Category = category
                });

            count++;
        }

        return count;
    }
}
