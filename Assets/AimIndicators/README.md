# Aim Indicators - Nişan Göstergeleri

Unity 6 ve üzeri sürümler ile uyumlu, dört farklı tip nişan göstergesi içeren kapsamlı bir Unity package'ı.

## Özellikler

### 🎯 Dört Farklı Nişan Türü
- **Cone (Koni)**: Geniş alan hedefleme için koni şeklinde gösterge
- **Line (Çizgi)**: Hassas hedefleme için çizgi tabanlı gösterge  
- **Target (Daire/Hedef)**: Alan hedefleme için dairesel hedef göstergesi
- **Parabolic (Parabolik)**: Mermi yörüngesi için parabolik gösterge

### ✨ Temel Özellikler
- Unity 6+ uyumluluğu
- World space çalışma desteği
- Inspector'dan tam özelleştirme
- Fill efektleri ve animasyonlar
- Sprites/Default shader kullanımı
- Kolay entegrasyon ve kullanım

## Kurulum

### Package Manager ile
1. Unity Package Manager'ı açın
2. "Add package from git URL" seçeneğini tıklayın
3. Şu URL'yi girin: `https://github.com/GokhanIsik87/UnityPackages.git`

### Manuel Kurulum
1. Bu repository'yi indirin
2. `Assets/AimIndicators` klasörünü projenize kopyalayın

## Kullanım

### Temel Kullanım

```csharp
using AimIndicators;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private ConeAimIndicator coneIndicator;
    [SerializeField] private Transform target;
    
    void Start()
    {
        // Hedef belirleme
        coneIndicator.SetTarget(target);
        
        // Renk ayarlama
        coneIndicator.SetColor(Color.red);
        
        // Menzil ayarlama
        coneIndicator.SetRange(10f);
    }
}
```

## Nişan Göstergeleri Detayları

### 1. Cone Aim Indicator (Koni Nişan Göstergesi)

Geniş alan etkisi olan silahlar için ideal.

#### Özellikler:
- Ayarlanabilir koni açısı (5-180 derece)
- Koni çözünürlüğü kontrolü
- Çerçeve görünürlük seçeneği
- Fill efektleri

#### Kullanım:
```csharp
ConeAimIndicator cone = gameObject.AddComponent<ConeAimIndicator>();
cone.SetConeAngle(45f);           // Koni açısını ayarla
cone.SetOutlineVisible(true);     // Çerçeveyi görünür yap
cone.SetRange(15f);               // Menzili ayarla
```

#### Inspector Ayarları:
- **Cone Angle**: Koni açısı (derece)
- **Cone Resolution**: Koni çözünürlüğü (3-50)
- **Show Cone Outline**: Çerçeve görünürlüğü

### 2. Line Aim Indicator (Çizgi Nişan Göstergesi)

Hassas hedefleme için çizgi tabanlı gösterge.

#### Özellikler:
- Ayarlanabilir çizgi kalınlığı
- Ok başı göstergesi
- Üç farklı çizgi tipi (Düz, Kesikli, Noktalı)
- Özelleştirilebilir kesik/nokta parametreleri

#### Kullanım:
```csharp
LineAimIndicator line = gameObject.AddComponent<LineAimIndicator>();
line.SetLineWidth(0.2f);                        // Çizgi kalınlığı
line.SetLineType(LineAimIndicator.LineType.Dashed); // Kesikli çizgi
line.SetArrowHeadVisible(true);                 // Ok başını göster
line.SetArrowHeadSize(0.8f);                    // Ok başı boyutu
```

#### Inspector Ayarları:
- **Line Width**: Çizgi kalınlığı
- **Show Arrow Head**: Ok başı görünürlüğü
- **Arrow Head Size**: Ok başı boyutu
- **Line Type**: Çizgi tipi (Solid/Dashed/Dotted)
- **Dash Length**: Kesik uzunluğu
- **Gap Length**: Boşluk uzunluğu

### 3. Target Aim Indicator (Hedef Nişan Göstergesi)

Dairesel alan hedefleme için ideal.

#### Özellikler:
- İç ve dış yarıçap kontrolü
- Ayarlanabilir halka sayısı
- Artı işareti (crosshair) göstergesi
- Döndürme ve nabız efektleri

#### Kullanım:
```csharp
TargetAimIndicator target = gameObject.AddComponent<TargetAimIndicator>();
target.SetInnerRadius(1f);               // İç yarıçap
target.SetOuterRadius(3f);               // Dış yarıçap
target.SetCrosshairVisible(true);        // Artı işaretini göster
target.SetRotationEffect(true, 45f);     // Döndürme efekti (45°/sn)
target.SetPulseEffect(true, 2f, 0.3f);   // Nabız efekti
```

#### Inspector Ayarları:
- **Inner Radius**: İç yarıçap
- **Outer Radius**: Dış yarıçap
- **Circle Resolution**: Daire çözünürlüğü
- **Show Crosshair**: Artı işareti görünürlüğü
- **Show Rings**: Halka görünürlüğü
- **Ring Count**: Halka sayısı
- **Rotate Target**: Döndürme efekti
- **Rotation Speed**: Döndürme hızı
- **Pulse Effect**: Nabız efekti

### 4. Parabolic Aim Indicator (Parabolik Nişan Göstergesi)

Mermi yörüngesi ve parabolik hareket için.

#### Özellikler:
- Gerçekçi fizik hesaplamaları
- Ayarlanabilir mermi hızı ve yerçekimi
- İniş noktası göstergesi
- Tepe noktası göstergesi
- Hız vektörü görselleştirmesi
- Gradient renk desteği

#### Kullanım:
```csharp
ParabolicAimIndicator parabolic = gameObject.AddComponent<ParabolicAimIndicator>();
parabolic.SetProjectileSpeed(15f);          // Mermi hızı
parabolic.SetGravity(9.81f);                // Yerçekimi
parabolic.SetTrajectoryVisible(true);       // Yörüngeyi göster
parabolic.SetLandingPointVisible(true);     // İniş noktasını göster
parabolic.SetVelocityVectorVisible(true);   // Hız vektörünü göster

// Yörünge bilgilerini al
Vector3 landingPoint = parabolic.GetLandingPoint();
float flightTime = parabolic.GetFlightTime();
Vector3 launchVelocity = parabolic.GetLaunchVelocity();
```

#### Inspector Ayarları:
- **Projectile Speed**: Mermi hızı
- **Gravity**: Yerçekimi değeri
- **Max Trajectory Time**: Maksimum yörünge süresi
- **Trajectory Resolution**: Yörünge çözünürlüğü
- **Show Landing Point**: İniş noktası göstergesi
- **Show Apex Point**: Tepe noktası göstergesi
- **Show Trajectory**: Yörünge görünürlüğü
- **Show Velocity Vector**: Hız vektörü görünürlüğü
- **Trajectory Width**: Yörünge çizgi kalınlığı

## Ortak Özellikler

Tüm nişan göstergeleri `AimIndicatorBase` sınıfından türetilmiştir ve şu ortak özelliklere sahiptir:

### Temel Ayarlar
```csharp
// Renk ayarları
indicator.SetColor(Color.blue);                    // Ana renk
indicator.fillColor = new Color(0, 0, 1, 0.3f);   // Dolgu rengi

// Menzil ve aktivasyon
indicator.SetRange(20f);           // Menzil ayarla
indicator.SetActive(true);         // Aktif/pasif
indicator.SetFillAmount(0.8f);     // Dolgu miktarı (0-1)

// Hedef belirleme
indicator.SetTarget(targetTransform);              // Hedef nesne
indicator.SetAimDirection(Vector3.forward);        // Manuel yön
```

### Animasyon Ayarları
```csharp
// Temel animasyon (nabız efekti)
indicator.enableAnimation = true;
indicator.animationSpeed = 2f;
```

## Örnek Senaryolar

### 1. FPS Silah Sistemi
```csharp
public class FPSWeapon : MonoBehaviour
{
    [SerializeField] private LineAimIndicator aimLine;
    [SerializeField] private Camera playerCamera;
    
    void Update()
    {
        // Mouse pozisyonuna göre nişan alma
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 aimDirection = (hit.point - transform.position).normalized;
            aimLine.SetAimDirection(aimDirection);
        }
        
        // Ateş etme
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    
    void Fire()
    {
        // Ateş efekti için fill amount animasyonu
        StartCoroutine(FireAnimation());
    }
    
    IEnumerator FireAnimation()
    {
        float duration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float fillAmount = 1f - (elapsed / duration);
            aimLine.SetFillAmount(fillAmount);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        aimLine.SetFillAmount(1f);
    }
}
```

### 2. Granaat Fırlatma Sistemi
```csharp
public class GrenadeLauncher : MonoBehaviour
{
    [SerializeField] private ParabolicAimIndicator trajectoryIndicator;
    [SerializeField] private float grenadeSpeed = 12f;
    
    void Start()
    {
        trajectoryIndicator.SetProjectileSpeed(grenadeSpeed);
        trajectoryIndicator.SetLandingPointVisible(true);
    }
    
    void Update()
    {
        // Mouse pozisyonuna göre hedef belirleme
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                trajectoryIndicator.SetTarget(hit.transform);
            }
        }
        
        // Granaat fırlatma
        if (Input.GetMouseButtonUp(0))
        {
            LaunchGrenade();
        }
    }
    
    void LaunchGrenade()
    {
        Vector3 launchVelocity = trajectoryIndicator.GetLaunchVelocity();
        Vector3 landingPoint = trajectoryIndicator.GetLandingPoint();
        
        // Granaat prefab'ını fırlat
        // ... granaat fırlatma kodu
    }
}
```

### 3. Çok Hedefli Sistem
```csharp
public class MultiTargetSystem : MonoBehaviour
{
    [SerializeField] private ConeAimIndicator coneIndicator;
    [SerializeField] private List<Transform> enemies;
    
    void Update()
    {
        // Koni içindeki düşmanları tespit et
        List<Transform> targetsInCone = GetTargetsInCone();
        
        // Koni rengini hedef sayısına göre ayarla
        if (targetsInCone.Count > 0)
        {
            float intensity = Mathf.Min(1f, targetsInCone.Count / 5f);
            coneIndicator.SetColor(Color.Lerp(Color.yellow, Color.red, intensity));
            coneIndicator.SetFillAmount(intensity);
        }
        else
        {
            coneIndicator.SetColor(Color.gray);
            coneIndicator.SetFillAmount(0.3f);
        }
    }
    
    List<Transform> GetTargetsInCone()
    {
        List<Transform> targets = new List<Transform>();
        
        foreach (Transform enemy in enemies)
        {
            Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToEnemy);
            float distance = Vector3.Distance(transform.position, enemy.position);
            
            if (angle <= coneIndicator.coneAngle * 0.5f && distance <= coneIndicator.indicatorRange)
            {
                targets.Add(enemy);
            }
        }
        
        return targets;
    }
}
```

## Performance İpuçları

1. **Mesh Sharing**: Aynı tipte birden fazla gösterge kullanırken mesh'leri paylaştırın
2. **LOD System**: Uzak göstergeler için düşük çözünürlük kullanın
3. **Object Pooling**: Dinamik göstergeler için object pooling uygulayın
4. **Update Optimization**: Gerekli olmadığında Update() çağrılarını devre dışı bırakın

```csharp
// Göstergeyi geçici olarak durdurma
indicator.enabled = false;  // Update() çağrılarını durdur
indicator.SetActive(false); // Görselliği gizle
```

## Sorun Giderme

### Yaygın Problemler

1. **Gösterge görünmüyor**
   - `SetActive(true)` çağrıldığından emin olun
   - Camera layer ayarlarını kontrol edin
   - Material ve shader ayarlarını kontrol edin

2. **Performance sorunları**
   - Trajectory resolution değerini düşürün
   - Gereksiz animasyonları kapatın
   - Update frequency'sini azaltın

3. **World space pozisyon sorunları**
   - Transform parent-child ilişkilerini kontrol edin
   - Local/World space koordinat dönüşümlerini doğrulayın

## Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'inizi push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında dağıtılmaktadır. Detaylar için `LICENSE` dosyasına bakın.

## İletişim

Gökhan Işık - gokhanisik87@gmail.com

Proje Link: [https://github.com/GokhanIsik87/UnityPackages](https://github.com/GokhanIsik87/UnityPackages)

## Teşekkürler

- Unity Technologies for Unity Engine
- Community feedback ve contributions