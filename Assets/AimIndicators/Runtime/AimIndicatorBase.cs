using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Base class for all aim indicator types
    /// Nişan göstergelerinin temel sınıfı
    /// </summary>
    public abstract class AimIndicatorBase : MonoBehaviour
    {
        [Header("Basic Settings / Temel Ayarlar")]
        [SerializeField] protected Color indicatorColor = Color.red;
        [SerializeField] protected float indicatorRange = 10f;
        [SerializeField] protected bool isActive = true;
        
        [Header("Fill Settings / Dolgu Ayarları")]
        [SerializeField] protected bool enableFill = true;
        [SerializeField] protected Color fillColor = new Color(1f, 0f, 0f, 0.3f);
        [SerializeField] [Range(0f, 1f)] protected float fillAmount = 1f;
        
        [Header("Animation Settings / Animasyon Ayarları")]
        [SerializeField] protected bool enableAnimation = false;
        [SerializeField] protected float animationSpeed = 1f;
        
        protected Transform targetTransform;
        protected Vector3 aimDirection;
        protected SpriteRenderer spriteRenderer;
        protected Material indicatorMaterial;
        
        protected virtual void Awake()
        {
            InitializeIndicator();
        }
        
        protected virtual void Start()
        {
            SetupMaterial();
        }
        
        protected virtual void Update()
        {
            if (isActive)
            {
                UpdateIndicator();
                if (enableAnimation)
                {
                    UpdateAnimation();
                }
            }
        }
        
        protected virtual void InitializeIndicator()
        {
            // Create sprite renderer if not exists
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }
        }
        
        protected virtual void SetupMaterial()
        {
            if (spriteRenderer != null)
            {
                // Use Sprites/Default shader for fill effects
                indicatorMaterial = new Material(Shader.Find("Sprites/Default"));
                spriteRenderer.material = indicatorMaterial;
                spriteRenderer.color = indicatorColor;
            }
        }
        
        protected abstract void UpdateIndicator();
        
        protected virtual void UpdateAnimation()
        {
            // Basic pulsing animation
            float pulse = Mathf.Sin(Time.time * animationSpeed) * 0.1f + 0.9f;
            if (spriteRenderer != null)
            {
                Color currentColor = indicatorColor;
                currentColor.a *= pulse;
                spriteRenderer.color = currentColor;
            }
        }
        
        /// <summary>
        /// Set the target to aim at
        /// Hedeflenen nesneyi ayarla
        /// </summary>
        public virtual void SetTarget(Transform target)
        {
            targetTransform = target;
        }
        
        /// <summary>
        /// Set the aim direction manually
        /// Nişan yönünü manuel olarak ayarla
        /// </summary>
        public virtual void SetAimDirection(Vector3 direction)
        {
            aimDirection = direction.normalized;
        }
        
        /// <summary>
        /// Toggle indicator visibility
        /// Gösterge görünürlüğünü aç/kapat
        /// </summary>
        public virtual void SetActive(bool active)
        {
            isActive = active;
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = active;
            }
        }
        
        /// <summary>
        /// Update indicator color
        /// Gösterge rengini güncelle
        /// </summary>
        public virtual void SetColor(Color color)
        {
            indicatorColor = color;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
        
        /// <summary>
        /// Update indicator range
        /// Gösterge menzilini güncelle
        /// </summary>
        public virtual void SetRange(float range)
        {
            indicatorRange = Mathf.Max(0.1f, range);
        }
        
        /// <summary>
        /// Update fill amount for visual feedback
        /// Görsel geri bildirim için dolgu miktarını güncelle
        /// </summary>
        public virtual void SetFillAmount(float amount)
        {
            fillAmount = Mathf.Clamp01(amount);
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw range in editor
            Gizmos.color = indicatorColor;
            Gizmos.DrawWireSphere(transform.position, indicatorRange);
        }
    }
}