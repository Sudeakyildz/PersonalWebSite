# PersonalWebSite

ASP.NET Core MVC ile geliştirilmiş kişisel web sitesi projesi. Kullanıcı kaydı/girişi, soru-cevap içeriği ve rol tabanlı yetkilendirme (Admin/User) içerir.

## Teknolojiler

- **.NET 9** – ASP.NET Core MVC
- **Entity Framework Core** – SQL Server (LocalDB)
- **ASP.NET Core Identity** – Kimlik doğrulama ve roller
- **Razor Views** – Arayüz

## Proje Yapısı

| Proje | Açıklama |
|-------|----------|
| PersonalWebSite.Core | Modeller, arayüzler |
| PersonalWebSite.Data | DbContext, repository'ler |
| PersonalWebSite.Business | İş mantığı servisleri |
| PersonalWebSite.Web | MVC web uygulaması (JSON API uçları dahil) |

## Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (Visual Studio ile gelir) veya SQL Server

## Çalıştırma

1. Depoyu klonlayın:
   ```bash
   git clone https://github.com/Sudeakyildz/PersonalWebSite.git
   cd PersonalWebSite
   ```

2. Veritabanı (ilk kurulumda):
   - EF Core CLI yoksa: `dotnet tool install --global dotnet-ef`
   - Migration yoksa önce oluşturun:
     ```bash
     dotnet ef migrations add InitialCreate --project PersonalWebSite.Data --startup-project PersonalWebSite.Web
     ```
   - Veritabanını güncelleyin:
     ```bash
     dotnet ef database update --project PersonalWebSite.Data --startup-project PersonalWebSite.Web
     ```

3. Web uygulamasını çalıştırın:
   ```bash
   dotnet run --project PersonalWebSite.Web
   ```

4. Tarayıcıda açın: **http://localhost:5000**

## Admin Kullanıcısı

Admin bilgileri koda gömülü değildir; `AdminUser` ayarlarından okunur. Geliştirme için
`PersonalWebSite.Web/appsettings.Development.json` içinde tanımlıdır. Kendi bilgilerinizi
kullanmak için user-secrets ya da ortam değişkeni kullanın:

```bash
dotnet user-secrets set "AdminUser:Email" "siz@ornek.com" --project PersonalWebSite.Web
dotnet user-secrets set "AdminUser:Password" "GucluBirSifre1!" --project PersonalWebSite.Web
```

İlk çalıştırmada `Admin` ve `User` rolleri ile (ayar varsa) admin kullanıcısı otomatik oluşturulur.
**Gerçek ortamda bu şifreyi mutlaka değiştirin.**

## Özellikler

- Kullanıcı kayıt ve giriş (başarısız denemelerde hesap kilitleme)
- Soru ekleme, düzenleme, silme (sadece sahibi veya Admin)
- Sorulara yalnızca Admin cevap verebilir, düzenleyebilir, silebilir
- Soru arama ve sayfalama
- Herkese açık sayfalarda e-posta yerine ad-soyad gösterilir
- Özel 404 / hata sayfaları
- REST API (hassas alan içermez): `/Question/api`, `/Question/api/{id}`, `/Question/api/{id}/answers`
- İstek loglama middleware’i

## Bağlantı Dizesi

Varsayılan olarak SQL Server LocalDB kullanılır. Bağlantıyı değiştirmek için `PersonalWebSite.Web/appsettings.json` içindeki `ConnectionStrings:DefaultConnection` değerini düzenleyin.

## Lisans

Bu proje eğitim/kişisel kullanım amaçlıdır.
