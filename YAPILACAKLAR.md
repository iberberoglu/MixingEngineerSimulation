# Mixing Engineer Simulation — Bulgular ve Yapılacaklar

> Bu dosya 28 Ağustos 2026'da yapılan kod incelemesinin çıktısıdır.
> Her madde bağımsız ele alınabilir. Satır numaraları inceleme
> anındaki hâle aittir; düzeltmeye başlamadan önce doğrula.

## Projenin özeti

2D pixel-art müzik prodüktörü simülasyonu. Stüdyo odasında dolaşıp
bilgisayardan mix görevi alıyor, 4 kanallı bir mixer'da şarkıyı
mixliyor, teslim edip para ve XP kazanıyorsun. Parayla mağazadan
ekipman alıp işini kolaylaştırıyorsun.

Öne çıkan tarafı: **mixleme gerçekten çalışıyor.** Fader'lar sahte
değil, FMOD event'lerinin `Gain` parametresini sürüyor; seviye metreleri
gerçek FMOD DSP metering'inden okunuyor.

| Konu | Değer |
|---|---|
| Unity | **2022.3.50f1** (tam bu sürüm gerekli) |
| Render | URP 14.0.11, 2D |
| Ses | **FMOD 2.02.25** — 6 bank |
| Giriş | Input System 1.11.2 (WASD + E) |
| Kamera | Cinemachine 2.10.3 |
| Kod | 27 script, ~2.100 satır C# |
| Sahneler | MainMenu → MainScene |
| Repo | `github.com/iberberoglu/MixingEngineerSimulation` |

**Oyun döngüsü:**

```
Bilgisayar (E) → Görev seç → Accept
   → MixerControl.setSong() → FMOD'dan 4 stem
   → Fader'ları ayarla → setParameterByName("Gain", lerp(-80,+10))
   → Complete Task → puanlama → XP + Para → Mağaza
   → Yatak (E) → 8 saat uyu → yeni gün → yeni görev
```

**Puanlama** (`MixTasksManager.CalculateRewardFactor`):

```
her kanal:  mesafe = |fader − ideal|
            mesafe > tolerans    →  GÖREV ANINDA BAŞARISIZ
            mesafe < tolerans/4  →  faktör = 1.0
            aksi halde           →  faktör = 1 − (mesafe/tolerans)

çarpan = (kritik kanalların ort. × 0.7) + (diğerlerinin ort. × 0.3)
XP ve para bu çarpanla ölçekleniyor
```

---

## Git durumu (28 Ağu 2026'da düzeltildi ve push edildi)

Yapılanlar:

- **Oyunun asıl sahneleri (`MainScene.unity`, `MainMenu.unity`) hiç
  commit edilmemişti** — depo klonlansa oynanabilir oyun çıkmıyordu.
  Artık takipte.
- Commit edilmemiş ~1 aylık iş (19 Ara 2024 → 25 Oca 2025) commit
  edildi: `MixTasksManager` 264 satır, `StoreManager` 58 satır, ve
  6 yeni script (`TaskTimeDisplay`, `EscMenu`, `ButtonClickSound`,
  `FontManager`, `ItemInfoPopUp`, `MainMenu/MainMenu`)
- FMOD bank'leri (2,7 MB) takibe alındı — gerekçe aşağıda
- Görseller, prefab'lar, ScriptableObject'ler, URP ayarları commit edildi
- `.DS_Store` ve `.plastic/` takipten çıkarıldı, `.idea/` gitignore'a
  eklendi
- Proje `Producer Simulation` → `Mixing Engineer Simulation` olarak
  yeniden adlandırıldı (aşağıya bak)
- `Test` dalı `main`'e birleştirildi ve `main` push edildi. GitHub'daki
  depo artık oynanabilir oyunun tamamını içeriyor.

### Proje adı değişikliği (28 Ağu 2026)

`Producer Simulation` → `Mixing Engineer Simulation`. Değişen yerler:

| Yer | Not |
|---|---|
| `origin` remote | `github.com/iberberoglu/MixingEngineerSimulation` |
| `ProjectSettings.asset` → `productName` | build çıktısının adı |
| `ProjectSettings.asset` → `projectName` | Unity Cloud alanı (`cloudEnabled: 0`, kullanılmıyor) |
| `Assets/Input System/…Input System.inputactions` | dosya adı + içindeki `name`; GUID `ff4aab23…` korundu, sahne referansları sağlam |
| `.vscode/settings.json` | `dotnet.defaultSolution` |
| Disk klasörü | `~/Documents/Mixing Engineer Simulation` |

Eski `Producer Simulation.sln` silindi; Unity açılışta yeni adla
yeniden üretir (`.sln`/`.csproj` zaten git'te değil).

**Bilerek değiştirilmeyenler:**

- `companyName: Berberoglu Games`
- `cloudProjectId` / `organizationId` — Unity bağlantısı bozulmasın diye
- Ana menü başlığı `"Mix Mühendisi Simülasyonu"` — menünün geri kalanı
  da Türkçe ("Başla"), tutarlı kalsın diye
- FMOD `sourceProjectPath: ../Producer-Simulation-FMOD/…` — yedekten
  gelecek klasörün adı bu; ikisi birlikte değişmeli (3 numaraya bak)
- `Builds/` içindeki eski `.app` dosyalarının adları

⚠ `applicationIdentifier` boş ve `overrideDefaultApplicationIdentifier: 0`
olduğu için bundle id `productName`'den türüyor — yani o da değişti.
Kayıt sistemi henüz olmadığı için kaybolan veri yok, ama **2 numaradaki
kayıt sistemini eklemeden önce** bu id'yi sabitle; sonradan değişirse
oyuncuların kayıtları kaybolur.
### FMOD bank'leri neden takibe alındı

FMOD'un standart `.gitignore`'u `.bank` dosyalarını yok sayar. O kural
**FMOD Studio kaynak projesi ayrıca versiyonlandığında** doğrudur.
Bu depoda kaynak proje yok (aşağıya bak), dolayısıyla derlenmiş
bank'ler oyunun sesinin tek kopyasıydı. `.gitignore`'a açık bir
istisna eklendi. Kaynak proje depoya girerse istisna kaldırılabilir.

---

## README (28 Ağu 2026'da eklendi)

Depoda iki README var, ikisi de CV'ye/portfolyoya bakan bir okuyucu
için yazıldı:

- [`README.md`](README.md) — İngilizce, ana dosya
- [`README.tr.md`](README.tr.md) — Türkçe

Kaynakları: kaynak kodun tamamı, `Serbest Proje Çalışması 3/` içindeki
okul raporu ve sunumu. Sunum klasörü `.gitignore`'da — ders teslimi,
depoya girmemeli.

Ekran görüntüleri `docs/screenshots/` altında (raporun `OYUN GÖRSELLER`
klasöründen alınıp 1440 px'e küçültüldü). Ana menü ekran görüntüsü
bilerek kullanılmadı: eski "Producer Simulation" başlığını gösteriyor.

### ⚠ Çalışma kuralı

**README'yi etkileyen bir değişiklik yaparsak README'yi de aynı anda
güncelleyeceğiz** — iki dilde birden. README'ye dokunmayı gerektiren
tipik işler:

| Yapılan iş | README'de güncellenecek |
|---|---|
| 1 (zaman hatası) düzeltilirse | "Bilinen eksikler" listesinden çıkar |
| 2 (kayıt sistemi) eklenirse | "Bilinen eksikler" + sistemler tablosu |
| 4 (yeni görev/şarkı) eklenirse | Görev tablosu, "içerik az" maddesi |
| 5 (rastgele görev aralığı) bağlanırsa | Oyun döngüsü diyagramı |
| Yeni ekran/mekanik | Ekran görüntüleri, sistemler tablosu |
| Yeni build alınırsa | "Çalıştırmak" bölümü |
| Puanlama formülü değişirse | Puanlama bölümü (iki dilde de formül var) |

Bilinen eksikler bölümü bilerek dürüst yazıldı; maddeler düzeldikçe
oradan silinmesi gerekiyor, yoksa README projeyi olduğundan kötü
gösterir.


## 🔴 Kritik

### 1. Görev süresi hiçbir zaman dolmuyor

**Yer:** `Assets/Scripts/GameTimeManager.cs:47-51` +
`Assets/Scripts/MixTasksManager.cs:259`

`GameTimeManager` saati her gün başında sıfırlıyor:

```csharp
currentTimeInMinutes += Time.deltaTime * timeScale;
if (currentTimeInMinutes >= 1440) { currentTimeInMinutes = 0; dayCount++; }
```

`MixTasksManager` ise bitiş zamanını **mutlak** hesaplıyor:

```csharp
taskDueTimeInMinutes = currentTimeInMinutes + (selectedTask.taskDuration * 24 * 60);
// Mix Task 1 (2 gün): 360 + 2880 = 3240
```

`currentTimeInMinutes` asla 1440'ı geçmediği için
`currentTimeInMinutes >= taskDueTimeInMinutes` koşulu **hiç sağlanmaz**.
`OnTaskDurationEnd()` ölü kod.

**Etkisi:** oyunun tek zaman baskısı mekaniği çalışmıyor. Görevleri
sonsuza kadar bekletebilirsin. `TaskTimeDisplay`'deki geri sayım da
her gün başında sıçrıyor.

**Düzeltme** — zamanı mutlak tut, günü ondan türet:

```csharp
private void UpdateGameTime()
{
    currentTimeInMinutes += Time.deltaTime * timeScale;
    dayCount = (int)(currentTimeInMinutes / 1440) + 1;   // sıfırlama YOK
}
```

`UpdateUI` zaten `% 24` kullanıyor, değişmesi gerekmiyor. `AddHours`
da sadeleşir:

```csharp
public void AddHours(int hours) => currentTimeInMinutes += hours * 60;
```

Değiştirdikten sonra `Awake`'teki `currentTimeInMinutes = 360f`
(06:00) hâlâ doğru başlangıç.

---

### 2. Hiç kayıt sistemi yok

**Yer:** proje geneli

`PlayerPrefs`, `JsonUtility`, dosya serileştirme — hiçbiri yok
(27 scriptin hiçbirinde eşleşme yok). Oyunu kapattığın an level, para,
XP ve satın alınanlar sıfırlanıyor.

İlerleme oyunu için en büyük yapısal eksik. `PlayerStats` ve
`ItemCosts` zaten singleton/ScriptableObject olduğu için ekleme yeri
belli:

```csharp
// PlayerStats.cs
public void Save()
{
    PlayerPrefs.SetInt("level", level);
    PlayerPrefs.SetInt("experience", experience);
    PlayerPrefs.SetFloat("money", money);
    PlayerPrefs.Save();
}
```

Ayrıca kaydedilmesi gerekenler: `dayCount`, `currentTimeInMinutes`,
her `MixTasks.isCompleted`, her `Item.isPurchased`,
`itemCosts.isPlayerHasSpeaker`, aktif görev ve kalan süresi.

**Dikkat:** ScriptableObject'lerdeki `isCompleted` / `isPurchased`
Editor'de diske yazılır ama build'de yazılmaz. Bu yüzden zaten
`Start()` içinde sıfırlanıyorlar (`MixTasksManager.cs:52`,
`StoreManager.cs:34`). Kayıt sistemi eklerken bu sıfırlamaların
yüklemeden **önce** çalıştığından emin ol, yoksa kaydı ezer.

---

## 🟠 Ciddi

### 3. FMOD kaynak projesi yedekte yok

**Yer:** `Assets/Plugins/FMOD/Resources/FMODStudioSettings.asset`

```yaml
sourceProjectPath: ../Producer-Simulation-FMOD/Producer-Simulation-FMOD.fspro
sourceBankPath:    ../Producer-Simulation-FMOD/Build
```

Bu klasör Unity projesinin **kardeşi** ve yedeğe girmedi. Derlenmiş
`.bank`'ler var (oyun çalışır) ama `.fspro` olmadan **hiçbir sese
dokunamazsın**: yeni şarkı ekleyemez, stem değiştiremez, mix event'i
düzenleyemezsin.

**Yapılacak:** harici diski tak,
`2025 Ağustos Eski Bilgisayar son/UNITY/UnityProjects/` altında
`Producer-Simulation-FMOD` klasörünü ara ve
`~/Documents/` altına, Unity projesinin **kardeşi olacak şekilde**
kopyala (yol ayarı göreli, o yüzden konum önemli):

```
~/Documents/Mixing Engineer Simulation/
~/Documents/Producer-Simulation-FMOD/     ← buraya
```

Bulunamazsa bank'lerden geri dönüş yok — sesi sıfırdan kurman gerekir.

---

### 4. İçerik sadece 2 görev, üstelik tekrar denenemiyor

**Yer:** `Assets/Scriptable Objects/` + `MixTasksManager.cs`

Mevcut içerik:

| Görev | Süre | Ödül | Tolerans |
|---|---|---|---|
| "Vokal çok az geliyor!" | 2 gün | 100 XP / 100$ | 10 |
| "Davul ve Klavye çok yüksek!" | 4 gün | ? / 200$ | 9 |

İki şarkı var (Track 1, Track 2), her biri 4 kanal.

`GetRandomTask()` tamamlananları havuzdan çıkarıyor, yani **3. günde
oyun içerik olarak bitiyor**: "Şu an yeni görev yok, git biraz uyu!"
mesajı sonsuza kadar kalıyor.

Daha kötüsü, görev **başarısız olsa da** tamamlandı sayılıyor:

- `MixTasksManager.cs:410` — tolerans dışı kalınca `isCompleted = true`
- `MixTasksManager.cs:437` — süresi dolunca `isCompleted = true`

Yani batırdığın görevi bir daha deneyemiyorsun. Bu muhtemelen istenen
davranış değil.

**Düzeltme yönü:** başarısız görevleri havuza geri koy (belki azalan
ödülle), ve görev sayısını artır. Şarkı başına 3-4 görev bile döngüyü
epey uzatır.

---

## 🟡 Orta

### 5. Rastgele görev aralığı mekaniği hiç bağlanmamış

**Yer:** `Assets/Scripts/MixTasksManager.cs:15-16, 40, 167`

```csharp
[SerializeField] private int randomTaskDayMin = 1;   // hiç kullanılmıyor
[SerializeField] private int randomTaskDayMax = 3;   // hiç kullanılmıyor
private int nextTaskDay;                             // hiç okunmuyor
private int CalculateNextTaskDay() { ... }           // hiç çağrılmıyor
```

`UpdateTasks()` her gün koşulsuz bir görev ekliyor. Inspector'daki iki
alan hiçbir şey yapmıyor — yarım kalmış bir tasarım.

**Karar ver:** ya mekaniği bağla (görevler rastgele aralıklarla gelsin),
ya da ölü kodu sil. İkisi de kabul edilebilir; belirsiz bırakmak değil.

---

### 6. `StopCoroutine` hiçbir şeyi durdurmuyor

**Yer:** `Assets/Scripts/PlayerCameraMovement.cs:24, 30`

```csharp
StartCoroutine(MoveCamera(cameraSpeed));
...
StopCoroutine(MoveCamera(cameraSpeed));   // YENİ enumerator üretip onu durduruyor
```

Çalışan coroutine durmuyor.

**Düzeltme:**

```csharp
private Coroutine moveRoutine;

// başlatırken
if (moveRoutine != null) StopCoroutine(moveRoutine);
moveRoutine = StartCoroutine(MoveCamera(cameraSpeed));

// durdururken
if (moveRoutine != null) { StopCoroutine(moveRoutine); moveRoutine = null; }
```

**Aynı dosyada ikinci sorun:** `MoveCamera` sadece **yukarı** hareket
ediyor (`newY = pos.y + step`, hedefe ulaşınca kırpılıyor). `offset.y`
negatif olursa `while` koşulu asla sağlanmaz → sonsuz coroutine.
Yönü hesaba kat:

```csharp
float dir = Mathf.Sign(cameraTargetPositon - mainCamera.transform.position.y);
float newY = mainCamera.transform.position.y + step * dir;
```

---

### 7. Girdi sistemi karışık

**Yer:** `Assets/Scripts/BedInteraction.cs:16`

Proje yeni Input System kullanıyor (`PlayerInteraction.OnInteract()`),
ama yatak eski sistemi çağırıyor:

```csharp
if (isPlayerNearby && !isSleeping && Input.GetKeyDown(KeyCode.E))
```

`ProjectSettings` içinde `activeInputHandler: 2` ("Both") olduğu için
**şu an çalışıyor**. Ama bu ayarı "Input System Package (New)" yaparsan
yatak mekaniği sessizce ölür.

Ayrıca yatak `OnCollisionEnter2D`, piyano `OnTriggerEnter2D` kullanıyor
— aynı işlev için iki farklı yaklaşım.

**Düzeltme yönü:** yatağı da `PlayerInteraction` / `InteractableObject`
akışına taşı, tek bir etkileşim yolu kalsın.

---

### 8. `isPlayerHasSpeaker` her satın almada true oluyor

**Yer:** `Assets/Scripts/StoreManager.cs:83`

```csharp
itemCosts.isPlayerHasSpeaker = true;   // hoparlör kontrolünün DIŞINDA
```

Şu an mağazada sadece 2 hoparlör olduğu için görünmüyor. Üçüncü bir
ürün (mikrofon, kulaklık…) eklediğin an, onu alan oyuncu hoparlör
almış sayılacak ve mix ipuçları açılacak.

**Düzeltme:** satırı hoparlör bloğunun içine al, veya
`itemCosts.isPlayerHasSpeaker = itemCosts.items.Exists(x => x.itemName.StartsWith("Speaker") && x.isPurchased);`

---

### 9. Slider aralığı tutarsız

**Yer:** `Assets/Scripts/MixerControl.cs:46-49` ve `107-110`

```csharp
// Start():
slider1.value = 0.5f;     // ← yanlış

// setSongEmpty():
slider1.value = 50f;      // ← doğru
```

Sistem 0–100 aralığında çalışıyor: `SetVolume` `value / 100f` yapıyor,
`Song 1.channelVolumeStart` = 60. `Start()`'taki `0.5f` yanlış.
`setSong` hemen üzerine yazdığı için görünür etkisi yok, ama ölü ve
yanıltıcı kod.

---

### 10. Float eşitlik karşılaştırması

**Yer:** `Assets/Scripts/MixTasksManager.cs:445`

```csharp
if (rewardMultiplier == 1)   // "kusursuz" mesajını seçmek için
```

Float'ta tam eşitlik güvenilmez — kayan nokta hatası yüzünden kusursuz
bir mix "yarı kusursuz" mesajı alabilir.

```csharp
if (rewardMultiplier >= 0.999f)
```

---

## 🔵 Küçük

| # | Yer | Sorun |
|---|---|---|
| 11 | `MeteringDisplay.GetSpriteForDb` | `levelSprites.Length == 1` ise sıfıra bölme |
| 12 | `StoreManager.cs:33` | `ForEach` içinde `IndexOf` → O(n²), tekrar eden referansta yanlış indeks. Düz `for` yeterdi |
| 13 | `MixTasksManager.Update()` | Her karede `UpdateTaskAvailabilityMessage()` çağırıp string atıyor → gereksiz GC baskısı. Sadece durum değişince güncelle |
| 14 | `Assets/0.unity` | Kökte "0" adında başıboş sahne — sil veya adlandır |
| 15 | `ProjectSettings/EditorBuildSettings.asset` | Silinmiş `SampleScene.unity` hâlâ listede (disabled, zararsız) |
| 16 | `MixTasksManager.cs:414` | Tolerans dışında erken `return` → `PlayerStatsUI` güncellemeleri atlanıyor |
| 17 | `MixTasksManager.Start()` | `GameTimeManager.Instance` null olabilir (script execution order'a bağlı) |

---

## Build'ler

`Builds/` klasöründe 10 derlenmiş sürüm var (git'te değil, `.gitignore`'da):

- `Producer Simulation 1.0` … `1.8` (Ara 2024 – Oca 2025)
- **`Mix Mühendisi Simulation 1.9.app`** — en yeni, 25 Oca 2025,
  universal (x86_64 + arm64). Çift tıklayıp oynayabilirsin.

İsim değişikliği 1.9'da build tarafında olmuş ama proje ayarlarına
yansımamıştı; 28 Ağu 2026'da `productName` da
`Mixing Engineer Simulation` yapıldı. Bundan sonraki build'ler bu adla
çıkacak — eski `.app`'lerin adları geçmiş kayıt olarak duruyor.

---

## Önerilen sıra

1. ~~**Git**: `Test` dalını push et~~ ✅ 28 Ağu 2026 — `main`'e
   birleştirilip push edildi, proje adı da değiştirildi
2. **3 numara**: FMOD kaynak projesini diskten kurtar (bu kaybolursa
   geri dönüşü yok, en acili)
3. **1 numara**: zaman hatası — oyunun ana mekaniği ölü durumda
4. **4 numara**: içerik (görev sayısı + başarısız görev tekrarı)
5. **2 numara**: kayıt sistemi
6. **6, 8, 9, 10**: küçük ve bağımsız, aralara sıkıştırılabilir
7. Kalanlar
