# Aim Indicators (Nişan Göstergeleri)

Unity 6+ ile uyumlu, dört farklı tip nişan göstergesi içeren paket. Her gösterge türü özelleştirilebilir parametreler, fill efektleri ve dünya koordinat sisteminde çalışma desteği sunar.

## Özellikler

- **Unity 6+ Uyumlu**: En son Unity sürümleri ile tam uyumluluk
- **4 Farklı Gösterge Tipi**: Cone, Line, Target ve Parabolic
- **Fill Efektleri**: Tüm göstergelerde animasyonlu fill efektleri
- **Inspector Entegrasyonu**: Kolay kullanım için özelleştirilebilir parametreler
- **Dünya Koordinat Sistemi**: WorldSpace desteği
- **Namespace Desteği**: Temiz kod organizasyonu
- **Performans Optimizasyonu**: Sprites/Default shader kullanımı

## Gösterge Tipleri

### 1. ConeAimIndicator (Koni Nişan Göstergesi)
Koni şeklinde yayılan nişan göstergesi. Shotgun, alevatar gibi yayılan silahlar için idealdir.

**Özellikler:**
- Ayarlanabilir koni açısı (1-180 derece)
- Koni uzunluğu kontrolü
- Çözünürlük ayarı
- Kenar çizgisi desteği

### 2. LineAimIndicator (Çizgi Nişan Göstergesi)
Düz çizgi şeklinde nişan göstergesi. Tüfek, sniper gibi hassas silahlar için idealdir.

**Özellikler:**
- Ayarlanabilir çizgi uzunluğu ve kalınlığı
- Üç farklı çizgi tipi: Solid, Dashed, Dotted
- Ok başı gösterimi
- Çoklu çizgi desteği

### 3. TargetAimIndicator (Hedef Nişan Göstergesi)
Daire şeklinde hedef göstergesi. Artı işareti ve sektör desteği ile.

**Özellikler:**
- Ayarlanabilir yarıçap
- Artı işareti (crosshair) gösterimi
- Çoklu halka desteği
- Sektör modları

### 4. ParabolicAimIndicator (Parabolik Nişan Göstergesi)
Parabolik yörünge göstergisi. Bomba, top, ok gibi eğimli atış yapan silahlar için idealdir.

**Özellikler:**
- Gerçekçi fizik hesaplamaları
- Fırlatma açısı ve hız kontrolü
- İniş ve tepe noktası gösterimi
- Hava direnci ve rüzgar etkisi

## Kurulum

### Unity Package Manager ile:
1. Unity Package Manager'ı açın (Window > Package Manager)
2. "+" butonuna tıklayın ve "Add package from git URL" seçin
3. URL'yi girin: `https://github.com/GokhanIsik87/UnityPackages.git`

### Manuel Kurulum:
1. Bu repoyu indirin
2. `Assets/AimIndicators` klasörünü projenizin Assets klasörüne kopyalayın

## Kullanım

### Temel Kullanım

```csharp
using AimIndicators;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private ConeAimIndicator coneIndicator;
    [SerializeField] private LineAimIndicator lineIndicator;
    [SerializeField] private TargetAimIndicator targetIndicator;
    [SerializeField] private ParabolicAimIndicator parabolicIndicator;

    void Start()
    {
        // Koni göstergesini ayarla
        coneIndicator.ConeAngle = 30f;
        coneIndicator.ConeLength = 5f;
        coneIndicator.IndicatorColor = Color.red;

        // Çizgi göstergesini ayarla
        lineIndicator.LineLength = 10f;
        lineIndicator.LineWidth = 0.2f;
        lineIndicator.CurrentLineType = LineAimIndicator.LineType.Solid;

        // Hedef göstergesini ayarla
        targetIndicator.TargetRadius = 2f;
        targetIndicator.ShowCrosshair(true);
        targetIndicator.EnableMultipleRings(true);

        // Parabolik göstergeyi ayarla
        parabolicIndicator.LaunchAngle = 45f;
        parabolicIndicator.LaunchVelocity = 15f;
        parabolicIndicator.Gravity = -9.81f;
    }

    void Update()
    {
        // Hedef takibi örneği
        Vector3 targetPosition = GetTargetPosition();
        
        coneIndicator.PointTowards(targetPosition);
        lineIndicator.SetLineTarget(targetPosition);
        parabolicIndicator.AimAtTarget(targetPosition);
    }

    Vector3 GetTargetPosition()
    {
        // Hedef pozisyonunu döndür (örnek)
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
```

### Fill Efektleri

```csharp
// Fill animasyonu başlat
indicator.AnimateFill(1f); // %100'e kadar doldur

// Fill miktarını direkt ayarla
indicator.FillAmount = 0.5f; // %50 dolu

// Fill efektini devre dışı bırak
indicator.enableFillEffect = false;
```

### Renk ve Görünüm

```csharp
// Renk değiştir
indicator.IndicatorColor = Color.blue;

// Boyut ayarla
indicator.IndicatorSize = 1.5f;

// Görünürlük kontrolü
indicator.IsVisible = false;

// Nabız efekti
indicator.enablePulse = true;
indicator.pulseSpeed = 2f;
indicator.pulseIntensity = 0.3f;
```

### Koni Göstergesi Özel Kullanım

```csharp
ConeAimIndicator cone = GetComponent<ConeAimIndicator>();

// Koni parametrelerini ayarla
cone.SetConeAngle(45f);
cone.SetConeLength(8f);

// Koniyi yönlendir
Vector3 targetDirection = (target.position - transform.position).normalized;
cone.SetConeDirection(targetDirection);
```

### Çizgi Göstergesi Özel Kullanım

```csharp
LineAimIndicator line = GetComponent<LineAimIndicator>();

// Çizgi tipini değiştir
line.CurrentLineType = LineAimIndicator.LineType.Dashed;

// Kesikli çizgi ayarları
line.dashLength = 0.5f;
line.dashSpacing = 0.2f;

// Ok başını göster
line.showArrowHead = true;
line.arrowHeadSize = 0.3f;

// Çoklu çizgi modu
line.useMultipleLines = true;
line.lineCount = 3;
line.lineSpacing = 0.3f;
```

### Hedef Göstergesi Özel Kullanım

```csharp
TargetAimIndicator target = GetComponent<TargetAimIndicator>();

// Hedef yarıçapını ayarla
target.SetTargetRadius(3f);

// Sektör modunu etkinleştir
target.EnableSectors(true);
target.sectorCount = 8;
target.alternateSectorColors = true;

// Çoklu halka modu
target.EnableMultipleRings(true);
target.RingCount = 4;
target.ringSpacing = 0.5f;
```

### Parabolik Gösterge Özel Kullanım

```csharp
ParabolicAimIndicator parabolic = GetComponent<ParabolicAimIndicator>();

// Fırlatma parametrelerini ayarla
parabolic.SetLaunchParameters(60f, 20f); // 60 derece, 20 hız

// Gerçekçi fizik kullan
parabolic.useRealPhysics = true;
parabolic.airResistance = 0.1f;
parabolic.windForce = new Vector3(2f, 0f, 0f);

// İniş noktasını al
Vector3 landingPoint = parabolic.GetLandingPoint();
float landingTime = parabolic.GetLandingTime();

// Hedef noktayı vur
parabolic.AimAtTarget(enemyPosition);
```

## Inspector Parametreleri

### Genel Ayarlar (Tüm Göstergeler)
- **Indicator Color**: Gösterge rengi
- **Indicator Size**: Gösterge boyutu
- **Is Visible**: Görünürlük durumu
- **Enable Fill Effect**: Fill efekti aktif/pasif
- **Fill Amount**: Fill miktarı (0-1)
- **Fill Speed**: Fill animasyon hızı
- **Enable Pulse**: Nabız efekti aktif/pasif
- **Pulse Speed**: Nabız hızı
- **Pulse Intensity**: Nabız şiddeti

### Koni Göstergesi Özel
- **Cone Angle**: Koni açısı (derece)
- **Cone Length**: Koni uzunluğu
- **Cone Resolution**: Koni çözünürlüğü
- **Show Outline**: Kenar çizgisi göster
- **Outline Width**: Kenar çizgisi kalınlığı

### Çizgi Göstergesi Özel
- **Line Length**: Çizgi uzunluğu
- **Line Width**: Çizgi kalınlığı
- **Line Type**: Çizgi tipi (Solid/Dashed/Dotted)
- **Dash Length**: Kesik uzunluğu
- **Dash Spacing**: Kesik aralığı
- **Show Arrow Head**: Ok başı göster
- **Arrow Head Size**: Ok başı boyutu

### Hedef Göstergesi Özel
- **Target Radius**: Hedef yarıçapı
- **Circle Resolution**: Daire çözünürlüğü
- **Show Crosshair**: Artı işareti göster
- **Crosshair Size**: Artı işareti boyutu
- **Ring Thickness**: Halka kalınlığı
- **Use Multiple Rings**: Çoklu halka kullan
- **Ring Count**: Halka sayısı
- **Ring Spacing**: Halka aralığı

### Parabolik Gösterge Özel
- **Launch Angle**: Fırlatma açısı (derece)
- **Launch Velocity**: Fırlatma hızı
- **Gravity**: Yerçekimi kuvveti
- **Max Time**: Maksimum süre
- **Trajectory Resolution**: Yörünge çözünürlüğü
- **Show Landing Point**: İniş noktası göster
- **Show Apex Point**: Tepe noktası göster
- **Use Real Physics**: Gerçekçi fizik kullan

## Performans İpuçları

1. **Çözünürlük Ayarları**: Yüksek çözünürlük daha güzel görüntü verir ancak performansı etkiler
2. **Fill Efektleri**: Gerekli olmadığında fill efektlerini devre dışı bırakın
3. **Çoklu Göstergeler**: Aynı anda çok fazla gösterge kullanmaktan kaçının
4. **Update Optimizasyonu**: Sürekli güncelleme yerine gerektiğinde güncelleyin

## Sistem Gereksinimleri

- Unity 6000.0.0b13 veya üzeri
- .NET Standard 2.1 desteği
- Platform: Windows, Mac, Linux, Android, iOS

## Lisans

Bu proje MIT lisansı altında dağıtılmaktadır. Detaylar için LICENSE dosyasına bakınız.

## Katkıda Bulunma

1. Bu repoyu fork edin
2. Yeni bir özellik dalı oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik ekle'`)
4. Dalınıza push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

## Destek

Sorularınız veya sorunlarınız için GitHub Issues kullanabilirsiniz.

## Değişiklik Geçmişi

### v1.0.0 (İlk Sürüm)
- ConeAimIndicator eklendi
- LineAimIndicator eklendi  
- TargetAimIndicator eklendi
- ParabolicAimIndicator eklendi
- Fill efektleri desteği
- Unity 6+ uyumluluğu
- Inspector entegrasyonu
- Türkçe dokümantasyon