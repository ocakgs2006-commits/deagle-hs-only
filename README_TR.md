# Deagle HS Only - Kurulum Rehberi

Bu plugin: **Deagle** ile yapılan vuruşlarda sadece **headshot** öldürür,
body/kol/bacak vuruşlarında verilen hasarı anında geri verir (yani etkisiz
hale gelir). **AWP dahil diğer tüm silahlar hiç etkilenmez.**

---

## 1) Derleme (Compile) - Bilgisayarında yapılacak

Bu bir kaynak kod dosyasıdır, sunucuya direkt atılamaz. Önce .dll haline
getirmen (derlemen) gerekiyor. Bunun için bir kere .NET 8 SDK kurman
yeterli, sonrasında tek komutla derlenir.

1. https://dotnet.microsoft.com/download/dotnet/8.0 adresinden
   **.NET 8 SDK**'yı indirip kur (ücretsiz).
2. Bu klasörü (DeagleHsOnly) bilgisayarına indir.
3. Klasörün içinde bir terminal/CMD aç ve şu komutu çalıştır:

   ```
   dotnet build -c Release
   ```

4. Derleme bitince şu yolda `DeagleHsOnly.dll` dosyasını bulacaksın:

   ```
   bin/Release/net8.0/DeagleHsOnly.dll
   ```

   NOT: Eğer "CounterStrikeSharp.API" paketi bulunamadı hatası alırsan,
   `DeagleHsOnly.csproj` içindeki `Version` numarasını nuget.org üzerinde
   "CounterStrikeSharp.API" araması yapıp en güncel sürümle değiştir.

---

## 2) Sunucuya Yükleme

1. Panelindeki dosya yöneticisinden şu klasöre git:
   ```
   game/csgo/addons/counterstrikesharp/plugins/
   ```
2. İçine `DeagleHsOnly` adında yeni bir klasör aç.
3. Az önce derlediğin `DeagleHsOnly.dll` dosyasını bu klasörün içine yükle.
   Sonuç şöyle görünmeli:
   ```
   plugins/DeagleHsOnly/DeagleHsOnly.dll
   ```
4. Sunucuyu yeniden başlat, ya da konsoldan:
   ```
   css_plugins load "plugins/DeagleHsOnly/DeagleHsOnly.dll"
   ```

---

## 3) Test Etme

- Bir bot veya arkadaşınla Deagle alıp vücuduna ateş et: ölmemeli / can
  kaybı olmamalı.
- Kafasına ateş et: normal şekilde ölmeli.
- AWP ile hem gövdeye hem kafaya ateş et: hiçbir şey değişmemiş olmalı.
- Konsolda `[DeagleHsOnly] Plugin yuklendi.` yazısını görürsen plugin aktif
  demektir.

---

## Notlar / Sınırlamalar

- Bu, oyun içi "player_hurt" event'ini dinleyerek çalışır; hasar
  uygulandıktan hemen sonra canı geri verir. Çok nadir durumlarda (örneğin
  oyuncu zaten çok düşük candayken tek Deagle body shot ile tam o anda
  ölüyorsa) can geri verme işlemi ölümden önce yetişemeyebilir. Pratikte
  bu neredeyse hiç yaşanmaz çünkü Deagle'ın body shot hasarı tek başına
  100 canı bitirmeye genelde yetmez.
- Kod test edilmeden sana iletildi; sunucunda birebir doğrulaman gerekir.
  Bir sorun yaşarsan (örn. hitgroup numarası tutmuyor gibi görünüyorsa)
  bana ekran görüntüsü/log ile bildir, birlikte düzeltiriz.
