# Deniz CRM

.NET 10 ve Clean Architecture yaklaşımıyla geliştirilen AI destekli CRM başlangıç projesi.

## Katmanlar

- `Domain`: Temel iş varlıkları
- `Application`: Kullanım senaryoları ve arayüzler
- `Infrastructure`: Kimlik doğrulama, JWT ve veri erişimi
- `WebApi`: HTTP API ve giriş arayüzü

## Çalıştırma

```powershell
dotnet run --project src/SaasAiCrm.WebApi
```

Arayüz: `http://localhost:5153`

Demo kullanıcı:

- E-posta: `admin@saasaicrm.com`
- Parola: `Admin123!`

## Güvenlik

`appsettings.json` içindeki JWT anahtarı yalnızca geliştirme içindir. Canlı ortamda
`Jwt__Key` ortam değişkeni veya güvenli bir secret provider kullanılmalıdır.
