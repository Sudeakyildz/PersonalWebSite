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
| PersonalWebSite.Web | MVC web uygulaması |
| PersonalWebSite.API | REST API |

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

## Varsayılan Admin Girişi

- **E-posta:** sudeaky5@gmail.com  
- **Şifre:** Sude1234!

*(İlk çalıştırmada Admin rolü ve bu kullanıcı otomatik oluşturulur.)*

## Özellikler

- Kullanıcı kayıt ve giriş
- Soru ekleme, düzenleme, silme (sadece sahibi veya Admin)
- Sorulara cevap yazma
- Soru/cevap listesi ve detay sayfaları
- REST API: `/Question/api`, `/Question/api/{id}`, `/Question/api/{id}/answers`
- İstek loglama middleware’i

## Bağlantı Dizesi

Varsayılan olarak SQL Server LocalDB kullanılır. Bağlantıyı değiştirmek için `PersonalWebSite.Web/appsettings.json` içindeki `ConnectionStrings:DefaultConnection` değerini düzenleyin.

## Lisans

Bu proje eğitim/kişisel kullanım amaçlıdır.
