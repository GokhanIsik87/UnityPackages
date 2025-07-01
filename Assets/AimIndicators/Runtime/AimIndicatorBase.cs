using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Nişan göstergelerinin temel sınıfı
    /// Base class for all aim indicators
    /// </summary>
    public abstract class AimIndicatorBase : MonoBehaviour
    {
        [Header("Genel Ayarlar / General Settings")]
        [SerializeField] protected Color indicatorColor = Color.red;
        [SerializeField] protected float indicatorSize = 1f;
        [SerializeField] protected bool isVisible = true;
        
        [Header("Fill Efekti / Fill Effect")]
        [SerializeField] protected bool enableFillEffect = true;
        [SerializeField] protected float fillAmount = 1f;
        [SerializeField] protected float fillSpeed = 1f;
        
        [Header("Animasyon / Animation")]
        [SerializeField] protected bool enablePulse = false;
        [SerializeField] protected float pulseSpeed = 2f;
        [SerializeField] protected float pulseIntensity = 0.3f;

        protected SpriteRenderer spriteRenderer;
        protected MaterialPropertyBlock propertyBlock;
        protected float currentFillAmount;
        protected float pulseTimer;

        /// <summary>
        /// Nişan göstergesinin rengi
        /// Color of the aim indicator
        /// </summary>
        public Color IndicatorColor
        {
            get => indicatorColor;
            set
            {
                indicatorColor = value;
                UpdateVisuals();
            }
        }

        /// <summary>
        /// Nişan göstergesinin boyutu
        /// Size of the aim indicator
        /// </summary>
        public float IndicatorSize
        {
            get => indicatorSize;
            set
            {
                indicatorSize = Mathf.Max(0.1f, value);
                UpdateSize();
            }
        }

        /// <summary>
        /// Nişan göstergesinin görünürlüğü
        /// Visibility of the aim indicator
        /// </summary>
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                UpdateVisibility();
            }
        }

        /// <summary>
        /// Fill efektinin miktarı (0-1 arası)
        /// Fill effect amount (0-1 range)
        /// </summary>
        public float FillAmount
        {
            get => fillAmount;
            set
            {
                fillAmount = Mathf.Clamp01(value);
                UpdateFillEffect();
            }
        }

        protected virtual void Awake()
        {
            InitializeComponent();
        }

        protected virtual void Start()
        {
            SetupRenderer();
            UpdateVisuals();
        }

        protected virtual void Update()
        {
            if (enableFillEffect)
            {
                UpdateFillAnimation();
            }

            if (enablePulse)
            {
                UpdatePulseAnimation();
            }
        }

        /// <summary>
        /// Bileşeni başlat
        /// Initialize the component
        /// </summary>
        protected virtual void InitializeComponent()
        {
            propertyBlock = new MaterialPropertyBlock();
            currentFillAmount = fillAmount;
        }

        /// <summary>
        /// Renderer'ı ayarla
        /// Setup the renderer
        /// </summary>
        protected virtual void SetupRenderer()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            // Unity'nin default sprite shader'ını kullan
            // Use Unity's default sprite shader
            if (spriteRenderer.material == null || spriteRenderer.material.name.Contains("Default"))
            {
                spriteRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            CreateIndicatorSprite();
        }

        /// <summary>
        /// Nişan göstergesi sprite'ını oluştur
        /// Create the aim indicator sprite
        /// </summary>
        protected abstract void CreateIndicatorSprite();

        /// <summary>
        /// Görselleri güncelle
        /// Update visuals
        /// </summary>
        protected virtual void UpdateVisuals()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", indicatorColor);
                spriteRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Boyutu güncelle
        /// Update size
        /// </summary>
        protected virtual void UpdateSize()
        {
            if (spriteRenderer != null)
            {
                transform.localScale = Vector3.one * indicatorSize;
            }
        }

        /// <summary>
        /// Görünürlüğü güncelle
        /// Update visibility
        /// </summary>
        protected virtual void UpdateVisibility()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = isVisible;
            }
        }

        /// <summary>
        /// Fill efektini güncelle
        /// Update fill effect
        /// </summary>
        protected virtual void UpdateFillEffect()
        {
            if (spriteRenderer != null && enableFillEffect)
            {
                spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_FillAmount", currentFillAmount);
                spriteRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Fill animasyonunu güncelle
        /// Update fill animation
        /// </summary>
        protected virtual void UpdateFillAnimation()
        {
            if (enableFillEffect)
            {
                currentFillAmount = Mathf.MoveTowards(currentFillAmount, fillAmount, fillSpeed * Time.deltaTime);
                UpdateFillEffect();
            }
        }

        /// <summary>
        /// Nabız animasyonunu güncelle
        /// Update pulse animation
        /// </summary>
        protected virtual void UpdatePulseAnimation()
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulseValue = 1f + Mathf.Sin(pulseTimer) * pulseIntensity;
            
            if (spriteRenderer != null)
            {
                Color pulsedColor = indicatorColor;
                pulsedColor.a *= pulseValue;
                
                spriteRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", pulsedColor);
                spriteRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Nişan göstergesini hedef pozisyona yönlendir
        /// Point the aim indicator towards target position
        /// </summary>
        /// <param name="targetPosition">Hedef pozisyon / Target position</param>
        public virtual void PointTowards(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// Fill animasyonunu başlat
        /// Start fill animation
        /// </summary>
        /// <param name="targetFill">Hedef fill miktarı / Target fill amount</param>
        public virtual void AnimateFill(float targetFill)
        {
            fillAmount = Mathf.Clamp01(targetFill);
        }

        /// <summary>
        /// Nişan göstergesini sıfırla
        /// Reset the aim indicator
        /// </summary>
        public virtual void ResetIndicator()
        {
            currentFillAmount = 0f;
            fillAmount = 0f;
            pulseTimer = 0f;
            UpdateVisuals();
        }
    }
}