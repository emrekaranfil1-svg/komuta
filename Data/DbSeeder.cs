using Komuta.Api.Models;

namespace Komuta.Api.Data;

public static class DbSeeder
{
    public static void Seed(KomutaDbContext db)
    {
        if (db.Users.Any())
            return;

        db.Users.Add(new UserAccount { Username = "admin", Password = "admin", Role = "admin" });

        var teams = new List<EmergencyTeam>
        {
            new() { Code = "amir", Name = "Acil Durum Amiri", SortOrder = 1, MinInside = 1, IsCritical = true, Accent = "#0057B8" },
            new() { Code = "kriz", Name = "Kriz Yönetimi Ekibi", SortOrder = 2, MinInside = 2, IsCritical = true, Accent = "#6B2D91" },
            new() { Code = "kurtarma", Name = "Kurtarma Ekibi", SortOrder = 3, MinInside = 3, IsCritical = true, Accent = "#1B9A5A" },
            new() { Code = "sondurme", Name = "Söndürme Ekibi", SortOrder = 4, MinInside = 3, IsCritical = true, Accent = "#E30613" },
            new() { Code = "koruma", Name = "Koruma Ekibi", SortOrder = 5, MinInside = 2, IsCritical = false, Accent = "#0057B8" },
            new() { Code = "ilkyardim", Name = "İlkyardım Ekibi", SortOrder = 6, MinInside = 2, IsCritical = true, Accent = "#E30613" },
            new() { Code = "teknik", Name = "Teknik Ekip", SortOrder = 7, MinInside = 2, IsCritical = false, Accent = "#6B2D91" }
        };
        db.Teams.AddRange(teams);
        db.SaveChanges();

        var people = new List<Person>();
        void Add(string card, string name, string title, bool inside, int seed)
        {
            var now = DateTime.Now.AddMinutes(-Random.Shared.Next(8, 420));
            people.Add(new Person
            {
                CardId = card,
                FullName = name,
                Title = title,
                Kind = "employee",
                IsInside = inside,
                IsVip = false,
                AvatarSeed = seed,
                LastEntryAt = inside ? now : now.AddHours(-6),
                LastExitAt = inside ? null : now
            });
        }

        Add("BC-0001", "ALİ YENİYIL", "Setter", true, 8);
        Add("BC-0002", "ERDOĞAN KESKİN", "Setter", true, 15);
        Add("BC-0003", "GÖKHAN TURAN", "Setter", false, 22);
        Add("BC-0004", "MUSTAFA DEMİRBİLEKLİLER", "Setter", true, 29);
        Add("BC-0005", "ŞAHSADE KESGİN", "Sevk Hazırlama Denetim Elemanı", true, 36);
        Add("BC-0006", "SAMET AYKAN", "Setter", false, 43);
        Add("BC-0007", "SERKAN YILDIZ", "Üretim Elemanı - Operatör Setter", true, 50);
        Add("BC-0008", "AYDIN TÜRKMENOĞLU", "Güvenlik Elemanı", true, 57);
        Add("BC-0009", "ALTAY SOYKAN", "Kalite Proses Kontrol Elemanı", false, 64);
        Add("BC-0010", "SEDAT CAN", "Üretim Elemanı", true, 71);
        Add("BC-0011", "UMUT DEMİRBAŞ", "Operatör Setter", true, 78);
        Add("BC-0012", "SELAMET SALKILIÇ", "Kalite Proses Vardiya Sorumlusu", false, 85);
        Add("BC-0013", "YAHYA ACAR", "Kalite Proses Kontrol Elemanı", true, 92);
        Add("BC-0014", "MAHMUT OLTAN MUSLU", "Setter", true, 2);
        Add("BC-0015", "YUNUS ABA", "Setter", false, 9);
        Add("BC-0016", "UFUK DEMİRBAŞ", "Üretim Elemanı-Operatör Setter", true, 16);
        Add("BC-0017", "AHMET YUSUF DEMİR", "Üretim Elemanı", true, 23);
        Add("BC-0018", "FIRAT ACUR", "Üretim Mühendisi", false, 30);
        Add("BC-0019", "HAKAN YILMAZ", "Setter", true, 37);
        Add("BC-0020", "HAKAN GÜNGÖR", "İmalat Uzmanı", true, 44);
        Add("BC-0021", "SERDAL AKÇAY", "Mekanik Bakım Elemanı", false, 51);
        Add("BC-0022", "ERTAN ÜNER", "İmalat Uzmanı", true, 58);
        Add("BC-0023", "ORHAN KAPAR", "Kalıphane - Üretim Elemanı", true, 65);
        Add("BC-0024", "VAROL ŞEREMET", "Kalıphane - Üretim Elemanı", false, 72);
        Add("BC-0025", "CENGİZHAN KAYA", "Enj.Special Uzmanı", true, 79);
        Add("BC-0026", "AHMET GÖKGÖZ", "Üretim Elemanı", true, 86);
        Add("BC-0027", "İLYAS CAN ÖZGEN", "Vardiya Koordinatörü", false, 93);
        Add("BC-0028", "SONER YİĞE", "Sevkiyat Elemanı", true, 3);
        Add("BC-0029", "TUNCER SELÇUK", "Üretim Elemanı", true, 10);
        Add("BC-0030", "ABDULKADİR DAMAT", "Setter", false, 17);
        Add("BC-0031", "FATİH ALTINDAĞ", "Üretim Elemanı", true, 24);
        Add("BC-0032", "İBRAHİM BİLGE", "Üretim Elemanı - Operatör Setter", true, 31);
        Add("BC-0033", "YAHYA ÇAKIR", "Üretim Elemanı", false, 38);
        Add("BC-0034", "ENGİN ŞAHİN", "Üretim Elemanı", true, 45);
        Add("BC-0035", "AVNİ YETKİN", "Setter", true, 52);
        Add("BC-0036", "İLKER ATASOY", "Üretim Elemanı - Operatör Setter", false, 59);
        Add("BC-0037", "CİHAN DEMİRBAŞ", "İdari İşler Şefi", true, 66);
        Add("BC-0038", "İBRAHİM SÖĞÜT", "Vardiya Koordinatörü", true, 73);
        Add("BC-0039", "SERCAN KARAKURT", "EHS ve Kalite Güvence Lideri", false, 80);
        Add("BC-0040", "AHMET BADAR", "Vardiya Koordinatörü", true, 87);
        Add("BC-0041", "YILMAZ FINDIK", "Güvenlik Elemanı", true, 94);
        Add("BC-0042", "TARIK AK", "Güvenlik Elemanı", false, 4);
        Add("BC-0043", "MAHMUT ÇAYAN", "Setter", true, 11);
        Add("BC-0044", "EMSAL AYDIN", "Setter", true, 18);
        Add("BC-0045", "TURGAY KARABULUT", "Setter", false, 25);
        Add("BC-0046", "MEHMET AYGIN", "Sevkiyat Elemanı", true, 32);
        Add("BC-0047", "AZİZ CAN AKTAŞ", "Sevkiyat Elemanı", true, 39);
        Add("BC-0048", "ERSİN CABADAK", "Üretim Elemanı", false, 46);
        Add("BC-0049", "MÜSLÜM KOLU", "Üretim Elemanı", true, 53);
        Add("BC-0050", "ERHAN YILMAZ", "Güvenlik Elemanı", true, 60);
        Add("BC-0051", "NİHAT AYDIN", "Baskı-Montaj Şefi", false, 67);
        Add("BC-0052", "AHMET ÇAĞLAR", "Sevkiyat Elemanı", true, 74);
        Add("BC-0053", "DENİZ KAVGACI", "Kalıphane - Bakım Elemanı", true, 81);
        Add("BC-0054", "ERCAN ÇETİN", "Enjeksiyon Special Üretim Şefi", false, 88);
        Add("BC-0055", "FARUK KARAGÖZ", "Setter", true, 95);
        Add("BC-0056", "GÜRER KUTLUĞ", "Ar-Ge ve SSTS Direktörü", true, 5);
        Add("BC-0057", "HAKAN DUMAN", "Girdi Kontrol Elemanı", false, 12);
        Add("BC-0058", "KEREM GÖRECİ", "Satınalma Lideri", true, 19);
        Add("BC-0059", "ÖZDEMİR KAYA", "Süreç Geliştirme Lideri", true, 26);
        Add("BC-0060", "ŞENOL DEMİR", "Kalıphane - Bakım Elemanı", false, 33);
        Add("BC-0061", "TAMER TÜRK", "Enj Su& Meşrubat Üretim Şefi", true, 40);
        Add("BC-0062", "UFUK TURANOĞLU", "Kalite Laboratuvar Uzmanı", true, 47);
        Add("BC-0063", "MUSTAFA YUCA", "Setter", false, 54);
        Add("BC-0064", "SERKAN AYDIN", "Setter", true, 61);
        Add("BC-0065", "EMRE KARANFİL", "Üretim Mühendisi", true, 68);
        Add("BC-0066", "TAYFUN GÜZELOĞLU", "Setter", false, 75);
        Add("BC-0067", "KALBİYE ÖZPINARLI", "Muhasebe ve Finans Şefi", true, 82);
        Add("BC-0068", "ABDULLAH TUNA", "Sevkiyat Elemanı", true, 89);
        Add("BC-0069", "İPEK AL", "Kalite Kontrol Lideri", false, 96);
        Add("BC-0070", "EMİNE EVKAYA", "Kalite Güvence Uzmanı/Auditor", true, 6);
        Add("BC-0071", "EMEL İPEK", "Temizlik Elemanı", true, 13);
        Add("BC-0072", "ERAY KAYIK", "Vardiya Koordinatörü", false, 20);
        Add("BC-0073", "MUHAMMED MUSTAFA KAYADELEN", "Vardiya Kalite Sorumlusu", true, 27);
        Add("BC-0074", "ÖZGÜR KARAKUŞ", "Vardiya Kalite Sorumlusu", true, 34);
        Add("BC-0075", "SUAT ŞAHİN", "Vardiya Kalite Sorumlusu", false, 41);
        Add("BC-0076", "ERCAN AKPINARLI", "Mekanik Bakım Elemanı", true, 48);
        Add("BC-0077", "MUSTAFA HALUK KADİROĞULLARI", "Bakım Lideri", true, 55);
        Add("BC-0078", "MEKAN AKKUL", "Elektrik Bakım Elemanı", false, 62);
        Add("BC-0079", "M.KÜRŞAT YALÇIN", "Elektrik Bakım Elemanı", true, 69);
        Add("BC-0080", "DURDU BAL", "Elektrik Bakım Elemanı", true, 76);
        Add("BC-0081", "GÖKHAN AKTAŞ", "Genel Müdür", false, 83);
        Add("BC-0082", "GÖZDE BAYRAMOĞLU", "Planlama ve Lojistik Müdürü", true, 90);
        Add("BC-0083", "MEHMET ARIK", "Cluster IT Dir&Group Head of SAP", true, 97);
        Add("BC-0084", "NİL SEDA KARADUMAN", "Satın Alma ve İthalat Direktörü", false, 7);
        Add("BC-0085", "MEHMET TEMUR", "Üretim ve Bakım Direktörü", true, 14);

        db.People.AddRange(people);
        db.SaveChanges();

        var map = people.ToDictionary(x => x.CardId, x => x.Id);
        int Id(string card) => map[card];

        void Link(string code, params string[] cards)
        {
            var teamId = teams.First(t => t.Code == code).Id;
            foreach (var card in cards)
                db.Memberships.Add(new TeamMembership { TeamId = teamId, PersonId = Id(card) });
        }

        Link("amir", "BC-0009", "BC-0012", "BC-0027", "BC-0038", "BC-0040", "BC-0072", "BC-0073", "BC-0074", "BC-0075");
        Link("kriz", "BC-0039", "BC-0081", "BC-0082", "BC-0083", "BC-0084", "BC-0085");
        Link("kurtarma", "BC-0018", "BC-0019", "BC-0020", "BC-0021", "BC-0022", "BC-0023", "BC-0024", "BC-0025", "BC-0026", "BC-0027", "BC-0028", "BC-0029", "BC-0030", "BC-0031", "BC-0032", "BC-0033", "BC-0034", "BC-0035", "BC-0036");
        Link("sondurme", "BC-0001", "BC-0002", "BC-0003", "BC-0004", "BC-0005", "BC-0006", "BC-0007", "BC-0008", "BC-0009", "BC-0010", "BC-0011", "BC-0012", "BC-0013", "BC-0014", "BC-0015", "BC-0016", "BC-0017");
        Link("koruma", "BC-0037", "BC-0038", "BC-0039", "BC-0040", "BC-0041", "BC-0042", "BC-0043", "BC-0044", "BC-0045", "BC-0046", "BC-0047", "BC-0048", "BC-0049");
        Link("ilkyardim", "BC-0038", "BC-0050", "BC-0051", "BC-0052", "BC-0053", "BC-0054", "BC-0055", "BC-0056", "BC-0057", "BC-0058", "BC-0059", "BC-0060", "BC-0061", "BC-0062", "BC-0063", "BC-0064", "BC-0065", "BC-0066", "BC-0067", "BC-0068", "BC-0069", "BC-0070", "BC-0071");
        Link("teknik", "BC-0076", "BC-0077", "BC-0078", "BC-0079", "BC-0080");

        var lastInside = people.Where(p => p.IsInside && p.LastEntryAt.HasValue)
            .OrderByDescending(p => p.LastEntryAt)
            .First();

        db.SystemState.Add(new SystemState
        {
            AccidentFreeDays = 312,
            EmergencyActive = false,
            Mode = "normal",
            LastTurnstileAt = lastInside.LastEntryAt,
            LastTurnstileName = lastInside.FullName,
            LastTurnstileEntered = true
        });

        db.SaveChanges();
    }
}
