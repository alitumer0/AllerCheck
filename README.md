ALLERCHECK - ALERJEN TAKİP SİSTEMİ
============================

PROJE HAKKINDA
-------------
AllerCheck, kullanıcıların gıda alerjilerini yönetmelerine ve güvenli gıda tüketmelerine yardımcı olan bir web uygulamasıdır. Kullanıcılar, ürünlerin içeriklerini inceleyebilir, alerjik oldukları içerikleri kara listeye ekleyebilir ve favori ürünlerini listeleyebilir.

TEMEL ÖZELLİKLER
---------------

1. Kullanıcı İşlemleri
   - Kayıt olma ve giriş yapma
   - Profil bilgilerini güncelleme
   - Şifre güvenliği (SHA256 ile hashleme)
   - Oturum yönetimi (Cookie tabanlı kimlik doğrulama)

2. Ürün Yönetimi
   - Ürün ekleme ve düzenleme
   - Kategori bazlı listeleme
   - Üretici bilgileri
   - İçerik analizi
   - Ürün arama

3. İçerik Sistemi
   - Risk seviyelerine göre sınıflandırma:
     * Yüksek Risk
     * Orta Risk
     * Düşük Risk
   - İçerik bilgileri ve detayları
   - İçerik-ürün ilişkilendirme

4. Kişisel Listeler
   - Favori ürün listeleri oluşturma
   - Kara liste yönetimi
   - Liste özelleştirme

TEKNİK ALTYAPI
-------------

1. Kullanılan Teknolojiler
   - ASP.NET Core MVC (.NET 7)
   - Entity Framework Core
   - SQL Server
   - AutoMapper
   - Bootstrap 5
   - jQuery

2. Proje Katmanları
   - AllerCheck.UI: MVC web uygulaması
   - AllerCheck_Core: Entity modelleri
   - AllerCheck_Data: Veritabanı işlemleri
   - AllerCheck_Services: İş mantığı
   - AllerCheck_DTO: Veri transfer nesneleri
   - AllerCheck_Mapping: AutoMapper profilleri

3. Veritabanı Tabloları
   - Icerik (İçerikler)
     * IcerikId
     * IcerikAdi
     * IcerikBilgi
     * RiskDurumId

   - Urun (Ürünler)
     * UrunId
     * UrunAdi
     * KategoriId
     * UreticiId
     * UyeId
     * UrunKayitTarihi

   - KaraListe (Kara Liste)
     * KaraListeId
     * UyeId
     * IcerikId

   - FavoriListesi (Favori Listesi)
     * FavoriListesiId
     * ListeAdi
     * UyeId

   - UrunIcerik (Ürün-İçerik İlişkisi)
     * UrunIcerikId
     * UrunId
     * IcerikId

GÜVENLİK ÖZELLİKLERİ
-------------------
1. Kimlik Doğrulama ve Yetkilendirme
   - Cookie tabanlı kimlik doğrulama
   - [Authorize] attribute kullanımı
   - Rol bazlı yetkilendirme

2. Güvenlik Önlemleri
   - HTTPS zorunluluğu
   - XSS koruması
   - CSRF token kullanımı
   - Şifre hashleme (SHA256)
   - SQL injection koruması (EF Core)

3. Oturum Yönetimi
   - 2 saatlik oturum süresi
   - Güvenli cookie ayarları
   - HttpOnly cookie kullanımı

VERİTABANI BAĞLANTISI
--------------------
Connection string örneği (appsettings.json):
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=YOUR_SERVER;Database=AllerCheck;Trusted_Connection=True;MultipleActiveResultSets=true"
    }
}

KURULUM ADIMLARI
--------------
1. Veritabanı kurulumu:
   - SQL Server'da yeni bir veritabanı oluşturun
   - Connection string'i güncelleyin

2. Paket kurulumu:
   dotnet restore

3. Veritabanı migration:
   dotnet ef database update

4. Uygulamayı çalıştırma:
   dotnet run

PROJE YAPISI VE DOSYALAR
-----------------------
1. Controllers
   - HomeController: Ana sayfa ve ürün işlemleri
   - AccountController: Kullanıcı profil işlemleri
   - AuthController: Kimlik doğrulama işlemleri

2. Views
   - Home: Ana sayfa ve ürün görünümleri
   - Account: Profil ve liste görünümleri
   - Auth: Giriş ve kayıt görünümleri

3. Services
   - UserService: Kullanıcı işlemleri
   - ProductService: Ürün işlemleri
   - ContentService: İçerik işlemleri
   - AuthenticationService: Kimlik doğrulama

4. Repositories
   - UserRepository: Kullanıcı veritabanı işlemleri
   - ProductRepository: Ürün veritabanı işlemleri
   - ContentRepository: İçerik veritabanı işlemleri

NOTLAR
-----
- Proje .NET 8 ile geliştirilmiştir
- Entity Framework Code First yaklaşımı kullanılmıştır
- Repository Pattern ve Unit of Work desenleri uygulanmıştır
- AutoMapper ile entity-DTO dönüşümleri yapılmaktadır
- Bootstrap 5 ile responsive tasarım uygulanmıştır 
