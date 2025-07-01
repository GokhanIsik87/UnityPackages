using UnityEngine;
using AimIndicators;

namespace AimIndicators.Samples
{
    /// <summary>
    /// Nişan göstergelerinin kullanım örneği
    /// Example usage of aim indicators
    /// </summary>
    public class AimIndicatorExample : MonoBehaviour
    {
        [Header("Gösterge Referansları / Indicator References")]
        [SerializeField] private ConeAimIndicator coneIndicator;
        [SerializeField] private LineAimIndicator lineIndicator;
        [SerializeField] private TargetAimIndicator targetIndicator;
        [SerializeField] private ParabolicAimIndicator parabolicIndicator;

        [Header("Kontrol Ayarları / Control Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private bool followMouse = true;
        [SerializeField] private Camera mainCamera;

        [Header("Test Ayarları / Test Settings")]
        [SerializeField] private KeyCode switchIndicatorKey = KeyCode.Space;
        [SerializeField] private KeyCode toggleFillKey = KeyCode.F;
        [SerializeField] private KeyCode toggleVisibilityKey = KeyCode.V;

        private AimIndicatorBase[] allIndicators;
        private int currentIndicatorIndex = 0;

        void Start()
        {
            InitializeIndicators();
            SetupIndicators();
        }

        void Update()
        {
            HandleInput();
            UpdateTargeting();
        }

        /// <summary>
        /// Göstergeleri başlat
        /// Initialize indicators
        /// </summary>
        private void InitializeIndicators()
        {
            // Otomatik referans bulma
            if (coneIndicator == null)
                coneIndicator = FindObjectOfType<ConeAimIndicator>();
            if (lineIndicator == null)
                lineIndicator = FindObjectOfType<LineAimIndicator>();
            if (targetIndicator == null)
                targetIndicator = FindObjectOfType<TargetAimIndicator>();
            if (parabolicIndicator == null)
                parabolicIndicator = FindObjectOfType<ParabolicAimIndicator>();

            if (mainCamera == null)
                mainCamera = Camera.main;

            // Tüm göstergeleri listeye ekle
            allIndicators = new AimIndicatorBase[]
            {
                coneIndicator,
                lineIndicator,
                targetIndicator,
                parabolicIndicator
            };

            // Sadece ilk göstergeyi aktif et
            for (int i = 0; i < allIndicators.Length; i++)
            {
                if (allIndicators[i] != null)
                {
                    allIndicators[i].IsVisible = (i == currentIndicatorIndex);
                }
            }
        }

        /// <summary>
        /// Göstergeleri ayarla
        /// Setup indicators
        /// </summary>
        private void SetupIndicators()
        {
            // Koni göstergesi ayarları
            if (coneIndicator != null)
            {
                coneIndicator.ConeAngle = 30f;
                coneIndicator.ConeLength = 5f;
                coneIndicator.IndicatorColor = Color.red;
                coneIndicator.enableFillEffect = true;
                coneIndicator.fillSpeed = 2f;
            }

            // Çizgi göstergesi ayarları
            if (lineIndicator != null)
            {
                lineIndicator.LineLength = 8f;
                lineIndicator.LineWidth = 0.15f;
                lineIndicator.IndicatorColor = Color.blue;
                lineIndicator.CurrentLineType = LineAimIndicator.LineType.Solid;
                lineIndicator.showArrowHead = true;
            }

            // Hedef göstergesi ayarları
            if (targetIndicator != null)
            {
                targetIndicator.TargetRadius = 2f;
                targetIndicator.IndicatorColor = Color.green;
                targetIndicator.ShowCrosshair(true);
                targetIndicator.crosshairSize = 0.7f;
                targetIndicator.EnableMultipleRings(true);
                targetIndicator.RingCount = 3;
            }

            // Parabolik gösterge ayarları
            if (parabolicIndicator != null)
            {
                parabolicIndicator.LaunchAngle = 45f;
                parabolicIndicator.LaunchVelocity = 12f;
                parabolicIndicator.IndicatorColor = Color.yellow;
                parabolicIndicator.showLandingPoint = true;
                parabolicIndicator.showApexPoint = true;
            }
        }

        /// <summary>
        /// Kullanıcı girişlerini işle
        /// Handle user input
        /// </summary>
        private void HandleInput()
        {
            // Gösterge değiştir
            if (Input.GetKeyDown(switchIndicatorKey))
            {
                SwitchIndicator();
            }

            // Fill efektini aç/kapat
            if (Input.GetKeyDown(toggleFillKey))
            {
                ToggleFillEffect();
            }

            // Görünürlüğü aç/kapat
            if (Input.GetKeyDown(toggleVisibilityKey))
            {
                ToggleVisibility();
            }

            // Mouse ile fill kontrolü
            if (Input.GetMouseButton(0))
            {
                AnimateFillUp();
            }
            else if (Input.GetMouseButton(1))
            {
                AnimateFillDown();
            }
        }

        /// <summary>
        /// Hedeflemeyi güncelle
        /// Update targeting
        /// </summary>
        private void UpdateTargeting()
        {
            Vector3 targetPosition = GetTargetPosition();
            
            if (targetPosition != Vector3.zero)
            {
                // Aktif göstergeyi hedef pozisyonuna yönlendir
                AimIndicatorBase activeIndicator = GetActiveIndicator();
                if (activeIndicator != null)
                {
                    activeIndicator.PointTowards(targetPosition);
                    
                    // Özel gösterge türü işlemleri
                    if (activeIndicator is LineAimIndicator lineInd)
                    {
                        lineInd.SetLineTarget(targetPosition);
                    }
                    else if (activeIndicator is ParabolicAimIndicator parabolicInd)
                    {
                        parabolicInd.AimAtTarget(targetPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Hedef pozisyonunu al
        /// Get target position
        /// </summary>
        /// <returns>Hedef pozisyon</returns>
        private Vector3 GetTargetPosition()
        {
            if (followMouse && mainCamera != null)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10f; // Kamera mesafesi
                return mainCamera.ScreenToWorldPoint(mousePos);
            }
            else if (target != null)
            {
                return target.position;
            }
            
            return Vector3.zero;
        }

        /// <summary>
        /// Gösterge değiştir
        /// Switch indicator
        /// </summary>
        private void SwitchIndicator()
        {
            // Mevcut göstergeyi gizle
            if (allIndicators[currentIndicatorIndex] != null)
            {
                allIndicators[currentIndicatorIndex].IsVisible = false;
            }

            // Sonraki göstergeye geç
            currentIndicatorIndex = (currentIndicatorIndex + 1) % allIndicators.Length;

            // Yeni göstergeyi göster
            if (allIndicators[currentIndicatorIndex] != null)
            {
                allIndicators[currentIndicatorIndex].IsVisible = true;
            }

            Debug.Log($"Switched to indicator: {GetActiveIndicator()?.GetType().Name}");
        }

        /// <summary>
        /// Fill efektini aç/kapat
        /// Toggle fill effect
        /// </summary>
        private void ToggleFillEffect()
        {
            AimIndicatorBase activeIndicator = GetActiveIndicator();
            if (activeIndicator != null)
            {
                activeIndicator.enableFillEffect = !activeIndicator.enableFillEffect;
                Debug.Log($"Fill effect: {activeIndicator.enableFillEffect}");
            }
        }

        /// <summary>
        /// Görünürlüğü aç/kapat
        /// Toggle visibility
        /// </summary>
        private void ToggleVisibility()
        {
            AimIndicatorBase activeIndicator = GetActiveIndicator();
            if (activeIndicator != null)
            {
                activeIndicator.IsVisible = !activeIndicator.IsVisible;
                Debug.Log($"Visibility: {activeIndicator.IsVisible}");
            }
        }

        /// <summary>
        /// Fill'i artır
        /// Animate fill up
        /// </summary>
        private void AnimateFillUp()
        {
            AimIndicatorBase activeIndicator = GetActiveIndicator();
            if (activeIndicator != null)
            {
                activeIndicator.AnimateFill(1f);
            }
        }

        /// <summary>
        /// Fill'i azalt
        /// Animate fill down
        /// </summary>
        private void AnimateFillDown()
        {
            AimIndicatorBase activeIndicator = GetActiveIndicator();
            if (activeIndicator != null)
            {
                activeIndicator.AnimateFill(0f);
            }
        }

        /// <summary>
        /// Aktif göstergeyi al
        /// Get active indicator
        /// </summary>
        /// <returns>Aktif gösterge</returns>
        private AimIndicatorBase GetActiveIndicator()
        {
            if (currentIndicatorIndex >= 0 && currentIndicatorIndex < allIndicators.Length)
            {
                return allIndicators[currentIndicatorIndex];
            }
            return null;
        }

        void OnGUI()
        {
            // Kullanım talimatları
            GUI.Label(new Rect(10, 10, 300, 200), 
                $"Aim Indicators Demo\n\n" +
                $"Current: {GetActiveIndicator()?.GetType().Name}\n\n" +
                $"Controls:\n" +
                $"{switchIndicatorKey} - Switch Indicator\n" +
                $"{toggleFillKey} - Toggle Fill Effect\n" +
                $"{toggleVisibilityKey} - Toggle Visibility\n" +
                $"Left Click - Fill Up\n" +
                $"Right Click - Fill Down\n" +
                $"Mouse - Aim Target");
        }
    }
}