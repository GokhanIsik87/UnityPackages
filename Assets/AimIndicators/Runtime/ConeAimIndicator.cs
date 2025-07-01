using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Koni şeklinde nişan göstergesi
    /// Cone-shaped aim indicator
    /// </summary>
    [System.Serializable]
    public class ConeAimIndicator : AimIndicatorBase
    {
        [Header("Koni Ayarları / Cone Settings")]
        [SerializeField] protected float coneAngle = 30f;
        [SerializeField] protected float coneLength = 5f;
        [SerializeField] protected int coneResolution = 16;
        [SerializeField] protected bool showOutline = true;
        [SerializeField] protected float outlineWidth = 0.1f;

        protected Mesh coneMesh;
        protected MeshFilter meshFilter;
        protected MeshRenderer meshRenderer;

        /// <summary>
        /// Koni açısı (derece cinsinden)
        /// Cone angle in degrees
        /// </summary>
        public float ConeAngle
        {
            get => coneAngle;
            set
            {
                coneAngle = Mathf.Clamp(value, 1f, 180f);
                UpdateConeMesh();
            }
        }

        /// <summary>
        /// Koni uzunluğu
        /// Cone length
        /// </summary>
        public float ConeLength
        {
            get => coneLength;
            set
            {
                coneLength = Mathf.Max(0.1f, value);
                UpdateConeMesh();
            }
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            
            // Mesh bileşenlerini ayarla
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            }
        }

        protected override void SetupRenderer()
        {
            // MeshRenderer kullanacağımız için SpriteRenderer'ı devre dışı bırak
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }

            if (meshRenderer.material == null)
            {
                meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }

            CreateIndicatorSprite();
        }

        protected override void CreateIndicatorSprite()
        {
            CreateConeMesh();
        }

        /// <summary>
        /// Koni mesh'ini oluştur
        /// Create cone mesh
        /// </summary>
        protected virtual void CreateConeMesh()
        {
            if (coneMesh != null)
            {
                DestroyImmediate(coneMesh);
            }

            coneMesh = new Mesh();
            coneMesh.name = "ConeAimIndicator";

            // Koni geometrisini oluştur
            Vector3[] vertices = new Vector3[coneResolution + 2];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] triangles = new int[coneResolution * 3];

            // Merkez nokta (koni başlangıcı)
            vertices[0] = Vector3.zero;
            uvs[0] = new Vector2(0.5f, 0f);

            // Koni ucu
            vertices[1] = new Vector3(coneLength, 0, 0);
            uvs[1] = new Vector2(1f, 0.5f);

            // Koni tabanındaki noktalar
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
            for (int i = 0; i < coneResolution; i++)
            {
                float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / (coneResolution - 1));
                float x = coneLength;
                float y = Mathf.Sin(angle) * coneLength;
                
                vertices[i + 2] = new Vector3(x, y, 0);
                uvs[i + 2] = new Vector2(1f, (float)i / (coneResolution - 1));
            }

            // Üçgenleri oluştur
            for (int i = 0; i < coneResolution - 1; i++)
            {
                int triangleIndex = i * 3;
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = i + 2;
                triangles[triangleIndex + 2] = i + 3;
            }

            coneMesh.vertices = vertices;
            coneMesh.uv = uvs;
            coneMesh.triangles = triangles;
            coneMesh.RecalculateNormals();
            coneMesh.RecalculateBounds();

            if (meshFilter != null)
            {
                meshFilter.mesh = coneMesh;
            }
        }

        /// <summary>
        /// Koni mesh'ini güncelle
        /// Update cone mesh
        /// </summary>
        protected virtual void UpdateConeMesh()
        {
            CreateConeMesh();
        }

        protected override void UpdateVisuals()
        {
            if (meshRenderer != null)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", indicatorColor);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        protected override void UpdateVisibility()
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = isVisible;
            }
        }

        protected override void UpdateFillEffect()
        {
            if (meshRenderer != null && enableFillEffect)
            {
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat("_FillAmount", currentFillAmount);
                
                // Fill efekti için alpha değerini ayarla
                Color fillColor = indicatorColor;
                fillColor.a *= currentFillAmount;
                propertyBlock.SetColor("_Color", fillColor);
                
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        /// <summary>
        /// Koninin yayılım açısını ayarla
        /// Set the cone spread angle
        /// </summary>
        /// <param name="angle">Açı (derece) / Angle in degrees</param>
        public void SetConeAngle(float angle)
        {
            ConeAngle = angle;
        }

        /// <summary>
        /// Koninin uzunluğunu ayarla
        /// Set the cone length
        /// </summary>
        /// <param name="length">Uzunluk / Length</param>
        public void SetConeLength(float length)
        {
            ConeLength = length;
        }

        /// <summary>
        /// Koniyi hedef yöne çevir
        /// Rotate cone towards target direction
        /// </summary>
        /// <param name="direction">Hedef yön / Target direction</param>
        public void SetConeDirection(Vector3 direction)
        {
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        protected override void UpdatePulseAnimation()
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulseValue = 1f + Mathf.Sin(pulseTimer) * pulseIntensity;
            
            if (meshRenderer != null)
            {
                Color pulsedColor = indicatorColor;
                pulsedColor.a *= pulseValue;
                
                if (enableFillEffect)
                {
                    pulsedColor.a *= currentFillAmount;
                }
                
                meshRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor("_Color", pulsedColor);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        protected virtual void OnDestroy()
        {
            if (coneMesh != null)
            {
                DestroyImmediate(coneMesh);
            }
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            // Editor'da koni sınırlarını göster
            Gizmos.color = Color.yellow;
            Vector3 forward = transform.right;
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
            
            Vector3 topBound = transform.position + Quaternion.AngleAxis(-coneAngle * 0.5f, Vector3.forward) * forward * coneLength;
            Vector3 bottomBound = transform.position + Quaternion.AngleAxis(coneAngle * 0.5f, Vector3.forward) * forward * coneLength;
            
            Gizmos.DrawLine(transform.position, topBound);
            Gizmos.DrawLine(transform.position, bottomBound);
        }
#endif
    }
}