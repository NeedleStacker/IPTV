# IPTV Player - Detaljne Upute

## Uvod

Ovaj dokument pruža detaljan pregled IPTV Player aplikacije, njezine arhitekture, funkcionalnosti korisničkog sučelja i unutarnjih mehanizama. Namijenjen je korisnicima koji žele razumjeti kako aplikacija radi i kako je koristiti na najbolji način.

## Arhitektura aplikacije

Aplikacija je razvijena u C# koristeći Windows Forms i temelji se na jednostavnoj, ali efikasnoj arhitekturi koja razdvaja odgovornosti u nekoliko ključnih komponenti:

*   **Korisničko sučelje (UI)**:
    *   Glavni prozor (`frmMain.cs`) služi kao centralno mjesto za interakciju s korisnikom. Sadrži sve vizualne kontrole, kao što su popisi kanala, video player i gumbi za upravljanje.
    *   Dizajniran je tako da bude intuitivan, s jasnim rasporedom elemenata.

*   **Servisni sloj (`Services`)**:
    *   Ovaj sloj sadrži logiku koja nije izravno vezana za korisničko sučelje. Odgovoran je za pozadinske operacije i upravljanje podacima.
    *   **`M3uService.cs`**: Upravlja učitavanjem i parsiranjem M3U datoteka. Koristi vanjsku biblioteku `m3uParser` za pretvaranje tekstualnog sadržaja M3U datoteke u strukturirane objekte.
    *   **`EpgService.cs`**: Odgovoran za dohvaćanje i obradu EPG (Electronic Program Guide) podataka. Spaja se na vanjski servis (`mojtv.net`) kako bi preuzeo TV raspored za dostupne kanale.
    *   **`SettingsService.cs`**: Upravlja postavkama aplikacije. Čita i sprema korisničke postavke (npr. putanja do M3U datoteke, veličina fonta, tema) u `settings.json` datoteku koja se nalazi u `%AppData%/IPTV-Player` direktoriju.
    *   **`LoggerService.cs`**: Jednostavan servis za zapisivanje grešaka u datoteku, što olakšava dijagnostiku problema.

*   **Modeli podataka (`Classes`)**:
    *   Ovaj direktorij sadrži klase koje definiraju strukturu podataka koje aplikacija koristi, kao što su `AppSettings.cs` (za postavke) i `MojTvChannelsModel.cs` (za EPG kanale).

*   **Video reprodukcija**:
    *   Za reprodukciju video sadržaja koristi se `Vlc.DotNet`, što je C# omotač oko moćne `libvlc` biblioteke. To omogućuje podršku za širok raspon video formata i streaming protokola.

Ova arhitektura omogućuje lakše održavanje i buduće nadogradnje, jer je logika jasno odvojena po odgovornostima.

## Vodič za korisničko sučelje

Glavni prozor aplikacije podijeljen je u nekoliko cjelina:

**1. Učitavanje M3U datoteke**
*   **Putanja do datoteke (`txtFile`)**: Tekstualno polje koje prikazuje putanju do učitane M3U datoteke.
*   **`...` (btnSelectFile)**: Otvara dijalog za odabir lokalne M3U datoteke s vašeg računala.
*   **`Učitaj` (btnLoad)**: Pokreće učitavanje i obradu odabrane M3U datoteke.
*   **`Automatski učitaj` (chkAutoLoad)**: Ako je označeno, aplikacija će automatski učitati posljednje korištenu M3U datoteku prilikom svakog pokretanja.

**2. Prikaz kanala i kategorija**
*   **Lista kategorija (`lstKategorije`)**: Prikazuje sve kategorije (`group-title`) pronađene u M3U datoteci. Klikom na kategoriju filtriraju se kanali.
*   **Filter (`txtFilter`)**: Omogućuje brzo pretraživanje kanala po imenu unutar odabrane kategorije.
*   **Tablica kanala (`dgvEpg`)**: Prikazuje popis kanala. Sadrži stupce "Kanal" (ime kanala) i "Program" (trenutna emisija prema EPG-u). Dvostrukim klikom na redak pokreće se reprodukcija kanala.

**3. Kontrole reprodukcije**
*   **`Play` (btnPlay)**: Pokreće reprodukciju kanala odabranog u tablici.
*   **`Stop` (btnStop)**: Zaustavlja reprodukciju.
*   **`Pauza` (btnPause)**: Pauzira ili nastavlja reprodukciju.
*   **`Prethodni` (btnPrev)**: Prelazi na prethodni kanal na popisu i pokreće reprodukciju.
*   **`Sljedeći` (btnNext)**: Prelazi na sljedeći kanal na popisu i pokreće reprodukciju.

**4. Kontrole zvuka i prikaza**
*   **`+` (btnVolumeUp)**: Povećava jačinu zvuka.
*   **`-` (btnVolumeDown)**: Smanjuje jačinu zvuka.
*   **`Puni ekran` (btnFullScreen)**: Prebacuje video player u prikaz preko cijelog ekrana. Izlazak iz ovog načina rada moguć je pritiskom na tipku `ESC` ili klikom na gumb za zatvaranje koji se pojavi.

**5. Postavke i dodatne opcije**
*   **`Favoriti` (btnToggleFavorites)**: Prikazuje samo kanale koji su dodani u favorite.
*   **`Tema` (btnToggleTheme)**: Mijenja izgled aplikacije između standardne i teme s visokim kontrastom.
*   **`A+` (btnPovecaj)**: Povećava veličinu fonta u tablici kanala.
*   **`A-` (btnSmanji)**: Smanjuje veličinu fonta u tablici kanala.
*   **`Raspored` (btnProgram)**: Prikazuje cijeli TV raspored za odabrani kanal u novom prozoru.

**6. Kontekstni izbornik (desni klik na kanal)**
*   **`Dodaj u favorite`**: Dodaje odabrani kanal na listu favorita.
*   **`Makni sa popisa`**: Trajno skriva odabrani kanal s popisa (kanal se dodaje u `KanaliKojiSeNePrikazujuNaListi.txt`).
*   **`Kopiraj URL`**: Kopira URL (link) odabranog kanala u međuspremnik.

## Unutarnji mehanizmi

*   **Učitavanje M3U datoteke**:
    1.  Kada korisnik klikne `Učitaj`, `M3uService` čita sadržaj datoteke.
    2.  Biblioteka `m3uParser` analizira tekst i pretvara ga u listu `Media` objekata. Svaki objekt sadrži informacije o jednom kanalu (ime, URL, atributi).
    3.  Aplikacija zatim izdvaja jedinstvene `group-title` vrijednosti kako bi popunila listu kategorija.

*   **Dohvaćanje EPG podataka**:
    1.  Nakon što su kanali učitani, `EpgService` preuzima kontrolu.
    2.  Za svaki TV kanal, servis pronalazi odgovarajući ID kanala koristeći lokalnu datoteku `mojtv ID kanala.txt`.
    3.  Zatim šalje HTTP zahtjev na `mojtv.net` API (`.../service.ashx`) s ID-om kanala i današnjim datumom kako bi dobio XML datoteku s TV rasporedom.
    4.  XML podaci se parsiraju i prikazuju u tablici kanala pored imena kanala.
    5.  Ovaj proces se odvija u pozadinskoj niti (`BackgroundWorker`) kako se korisničko sučelje ne bi zamrznulo.

*   **Spremanje postavki i podataka**:
    *   Sve korisničke postavke (putanja do datoteke, tema, itd.) spremaju se u `settings.json` putem `SettingsService`.
    *   Liste favorita i skrivenih kanala spremaju se u jednostavne tekstualne datoteke (`favorites.txt`, `KanaliKojiSeNePrikazujuNaListi.txt`) unutar `%AppData%/IPTV-Player` direktorija. Ovo osigurava da podaci ostaju sačuvani između pokretanja aplikacije.

## Objašnjenje M3U formata

M3U je jednostavan tekstualni format za playliste. Aplikacija koristi proširenu verziju koja sadrži dodatne metapodatke za svaki kanal.

**Analiza primjera:**

```m3u
#EXTM3U
```
*   **`#EXTM3U`**: Ovo je zaglavlje i mora biti prva linija u datoteci. Označava da se radi o proširenoj M3U datoteci.

---

```m3u
#EXTINF:-1 tvg-id="7873" tvg-name="HRT 1" tvg-logo="http://..." group-title="Hrvatska" ,HRT 1
http://domain.ext:port/username/rRgG66dDc/4455
```
*   **`#EXTINF:-1`**: Ova linija sadrži metapodatke za medij koji slijedi.
    *   **`-1`**: Trajanje medija u sekundama. Za live streamove, vrijednost je obično `-1`.
    *   **`tvg-id="7873"`**: Jedinstveni ID kanala koji se koristi za dohvaćanje EPG podataka. Aplikacija ga koristi za povezivanje kanala s TV rasporedom.
    *   **`tvg-name="HRT 1"`**: Ime kanala prema TV vodiču.
    *   **`tvg-logo="http://..."`**: URL do logotipa kanala.
    *   **`group-title="Hrvatska"`**: Ključni atribut koji aplikacija koristi za grupiranje kanala. Svi kanali s istim `group-title` bit će prikazani u istoj kategoriji.
    *   **`,HRT 1`**: Ime kanala koje će biti prikazano korisniku. Nalazi se nakon zareza.
*   **`http://...`**: Ovo je stvarni URL (link) do video streama. Aplikacija ovaj link prosljeđuje VLC playeru za reprodukciju.

---

```m3u
#EXTINF:-1 tvg-logo="..." group-title="Domaci filmovi" ,Priča o fabrici (1949)
http://IpAddress:port/movie/username/rRgG66dDc/file.mp4
```
*   Ovaj primjer prikazuje unos za film. Format je isti, ali `group-title` se koristi za sortiranje filmova u kategorije (npr. "Domaci filmovi", "Strani filmovi").

---

```m3u
#EXTINF:-1 tvg-logo="..." group-title="Domace serije" ,Preziveti Beograd : Season # 1 : Episode # 2
http://IpAddress:port/series/username/rRgG66dDc/file.mp4
```
*   Slično kao i filmovi, serije se također organiziraju pomoću `group-title`. Ime obično sadrži naziv serije, sezonu i broj epizode.

---

```m3u
#EXTINF:-1 group-title="EXYU",Avaz radio
http://domain.ext:port/radio/username/rRgG66dDc/1
```
*   Ovaj unos prikazuje radio stanicu. Nedostaju neki `tvg` atributi jer EPG nije relevantan za radio, ali `group-title` i dalje služi za kategorizaciju.
