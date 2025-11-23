# IPTV Player

Jednostavan desktop IPTV player za Windows, napisan u C# koristeći Windows Forms i LibVLCSharp.

## Značajke

*   **Učitavanje M3U playlista**: Podržava lokalne i udaljene M3U datoteke.
*   **Grupisanje kanala**: Kanali se automatski grupišu po kategorijama navedenim u `group-title` atributu.
*   **Reprodukcija**: Osnovne kontrole za reprodukciju, pauziranje i zaustavljanje kanala.
*   **Pretraga**: Brza pretraga kanala po imenu.
*   **Kontrola zvuka i prikaz preko cijelog ekrana**: Jednostavne kontrole za jačinu zvuka i prikaz preko cijelog ekrana.
*   **Upravljanje favoritima**: Dodajte i uklonite kanale iz liste favorita.
*   **Skrivanje neželjenih kanala**: Mogućnost skrivanja pojedinačnih kanala ili cijelih grupa.
*   **EPG (Electronic Program Guide)**: Dohvaćanje i prikaz EPG informacija za TV kanale.
*   **Prilagodljivost**:
    *   Prilagodljiva tema (standardna i visoki kontrast).
    *   Prilagodljiva veličina fonta za bolju čitljivost.

## Postavljanje

1.  **Klonirajte repozitorij**:
    ```bash
    git clone https://github.com/your-username/IPTV-Related.git
    ```
2.  **Otvorite rješenje**: Otvorite `IPTV-Related.sln` u Visual Studiju.
3.  **Vratite NuGet pakete**: Visual Studio bi trebao automatski vratiti sve potrebne NuGet pakete. Ako se to ne dogodi, možete to učiniti ručno putem "NuGet Package Manager" konzole:
    ```powershell
    Update-Package -Reinstall
    ```
4.  **Preuzmite LibVLC**: Ovaj projekt koristi `LibVLCSharp`. Potrebno je preuzeti odgovarajuće LibVLC nativne binarne datoteke za svoju arhitekturu (x86 ili x64) i kopirati ih u `IPTV-Related/bin/Debug` ili `IPTV-Related/bin/Release` direktorij. Možete ih pronaći na [službenoj stranici VideoLAN-a](https://www.videolan.org/vlc/index.html).
5.  **Pokrenite aplikaciju**: Pritisnite F5 ili kliknite na "Start" gumb u Visual Studiju.

## Korištenje

1.  **Učitavanje playliste**: Prilikom prvog pokretanja, aplikacija će od vas tražiti da učitate M3U datoteku. Možete odabrati lokalnu datoteku ili unijeti URL.
2.  **Odabir kanala**: S lijeve strane nalazi se popis grupa kanala. Odaberite grupu, a zatim kanal s glavnog popisa.
3.  **Reprodukcija**: Dvaput kliknite na kanal za početak reprodukcije.
4.  **Postavke**: U postavkama možete promijeniti temu, veličinu fonta i druge opcije.

## Primjer M3U datoteke

Aplikacija parsira M3U datoteke koje slijede standardni format. Evo primjera kako bi vaša M3U datoteka trebala izgledati:

```m3u
#EXTM3U
#EXTINF:-1 tvg-id="7873" tvg-name="HRT 1" tvg-logo="http://IpAddress/logo/Hrvatska/HRT1.png" group-title="Hrvatska" ,HRT 1
http://domain.ext:port/username/rRgG66dDc/4455
#EXTINF:-1 tvg-id="103" tvg-name="HRT 2" tvg-logo="http://IpAddress/logo/Hrvatska/HRT2.png" group-title="Hrvatska" ,HRT 2
http://domain.ext:port/username/rRgG66dDc/6953
#EXTINF:-1 tvg-logo="" group-title="Domaci filmovi" ,Priča o fabrici (1949)
http://IpAddress:port/movie/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="http://image.somedb.ext/t/p/w154/2zhIugPpwfrDSPJjzP4jjAS.jpg" group-title="Domaci filmovi" ,Trojanski konj (1982) (1982)
http://IpAddress:port/movie/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="http://image.somedb.ext/t/p/w154/u0EfH2rLOvEKVvWGgzGRQn3.jpg" group-title="Strani filmovi" ,Before the Flood (2016) (2016)
http://IpAddress:port/movie/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="http://image.somedb.ext/t/p/w154/7YBAO3t3ZMlEz219HbxYhNT.jpg" group-title="Filmovi 2024" ,Out Come the Wolves (2024)
http://IpAddress:port/movie/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="https://image.somedb.ext/t/p/w600_and_h900_bestv2/2yCFut7mAlbQSX3033mHgeQN.jpg" group-title="Domace serije" ,Preziveti Beograd : Season # 1 : Episode # 2
http://IpAddress:port/series/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="https://image.somedb.ext/t/p/w600_and_h900_bestv2/2yCFut7mAlbQSX3033mHgeQN.jpg" group-title="Domace serije" ,Preziveti Beograd : Season # 1 : Episode # 3
http://IpAddress:port/series/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="http://domain.ext:port/series_covers/cover-684.jpg" group-title="Domace serije" ,Konak kod Hilmije (2019) : Season # 1 : Episode # 1
http://IpAddress:port/series/username/rRgG66dDc/file.mp4
#EXTINF:-1 tvg-logo="http://domain.ext:port/series_covers/cover-686.jpg" group-title="Strane serije" ,Evil 2019 : Season # 1 : Episode # 1
http://IpAddress:port/series/username/rRgG66dDc/file.mp4
#EXTINF:-1 group-title="EXYU",Avaz radio
http://domain.ext:port/radio/username/rRgG66dDc/1
```
