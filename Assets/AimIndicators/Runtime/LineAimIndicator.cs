using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Çizgi şeklinde nişan göstergesi
    /// Line-shaped aim indicator
    /// </summary>
    [System.Serializable]
    public class LineAimIndicator : AimIndicatorBase
    {
        [Header("Çizgi Ayarları / Line Settings")]
        [SerializeField] protected float lineLength = 5f;
        [SerializeField] protected float lineWidth = 0.2f;
        [SerializeField] protected LineType lineType = LineType.Solid;
        [SerializeField] protected float dashLength = 0.5f;
        [SerializeField] protected float dashSpacing = 0.2f;
        [SerializeField] protected bool showArrowHead = true;
        [SerializeField] protected float arrowHeadSize = 0.3f;

        [Header("Çoklu Çizgi / Multiple Lines")]
        [SerializeField] protected bool useMultipleLines = false;
        [SerializeField] protected int lineCount = 3;
        [SerializeField] protected float lineSpacing = 0.3f;

        protected LineRenderer lineRenderer;
        protected LineRenderer[] additionalLines;

        public enum LineType
        {
            Solid,      // Düz çizgi
            Dashed,     // Kesikli çizgi
            Dotted      // Noktalı çizgi
        }

        /// <summary>
        /// Çizgi uzunluğu
        /// Line length
        /// </summary>
        public float LineLength
        {
            get => lineLength;
            set
            {
                lineLength = Mathf.Max(0.1f, value);
                UpdateLineGeometry();
            }
        }

        /// <summary>
        /// Çizgi kalınlığı
        /// Line width
        /// </summary>
        public float LineWidth
        {
            get => lineWidth;
            set
            {
                lineWidth = Mathf.Max(0.01f, value);
                UpdateLineWidth();
            }
        }

        /// <summary>
        /// Çizgi tipi
        /// Line type
        /// </summary>
        public LineType CurrentLineType
        {
            get => lineType;
            set
            {
                lineType = value;
                UpdateLinePattern();
            }
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            SetupLineRenderer();
        }

        protected override void SetupRenderer()
        {
            // SpriteRenderer'ı devre dışı bırak, LineRenderer kullanacağız
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }

            CreateIndicatorSprite();
        }

        /// <summary>
        /// LineRenderer'ı ayarla
        /// Setup LineRenderer
        /// </summary>
        protected virtual void SetupLineRenderer()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            // LineRenderer ayarları
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sortingOrder = 1;

            if (useMultipleLines && lineCount > 1)
            {
                SetupAdditionalLines();
            }
        }

        /// <summary>
        /// Ek çizgileri ayarla
        /// Setup additional lines
        /// </summary>
        protected virtual void SetupAdditionalLines()
        {
            if (additionalLines != null)
            {
                for (int i = 0; i < additionalLines.Length; i++)
                {
                    if (additionalLines[i] != null)
                    {
                        DestroyImmediate(additionalLines[i].gameObject);
                    }
                }
            }

            additionalLines = new LineRenderer[lineCount - 1];
            
            for (int i = 0; i < additionalLines.Length; i++)
            {
                GameObject lineObj = new GameObject($"AdditionalLine_{i + 1}");
                lineObj.transform.SetParent(transform);
                lineObj.transform.localPosition = Vector3.zero;
                
                LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startWidth = lineWidth;
                lr.endWidth = lineWidth;
                lr.useWorldSpace = true;
                lr.sortingOrder = 1;
                
                additionalLines[i] = lr;
            }
        }

        protected override void CreateIndicatorSprite()
        {
            UpdateLineGeometry();
            UpdateLinePattern();
        }

        /// <summary>
        /// Çizgi geometrisini güncelle
        /// Update line geometry
        /// </summary>
        protected virtual void UpdateLineGeometry()
        {
            if (lineRenderer == null) return;

            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + transform.right * lineLength;

            // Ana çizgi
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, endPos);

            // Ok başı
            if (showArrowHead)
            {
                CreateArrowHead(endPos);
            }

            // Ek çizgiler
            if (useMultipleLines && additionalLines != null)
            {
                UpdateAdditionalLines();
            }
        }

        /// <summary>
        /// Ok başını oluştur
        /// Create arrow head
        /// </summary>
        /// <param name="tipPosition">Ok başının pozisyonu / Arrow tip position</param>
        protected virtual void CreateArrowHead(Vector3 tipPosition)
        {
            // Ok başı için yeni çizgiler ekle
            lineRenderer.positionCount = 6;
            
            Vector3 direction = transform.right;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;
            
            Vector3 arrowBase = tipPosition - direction * arrowHeadSize;
            Vector3 arrowTop = arrowBase + perpendicular * arrowHeadSize * 0.5f;
            Vector3 arrowBottom = arrowBase - perpendicular * arrowHeadSize * 0.5f;
            
            // Ana çizgi
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, arrowBase);
            
            // Ok başı
            lineRenderer.SetPosition(2, arrowTop);
            lineRenderer.SetPosition(3, tipPosition);
            lineRenderer.SetPosition(4, arrowBottom);
            lineRenderer.SetPosition(5, arrowBase);
        }

        /// <summary>
        /// Ek çizgileri güncelle
        /// Update additional lines
        /// </summary>
        protected virtual void UpdateAdditionalLines()
        {
            Vector3 perpendicular = Vector3.Cross(transform.right, Vector3.forward).normalized;
            
            for (int i = 0; i < additionalLines.Length; i++)
            {
                if (additionalLines[i] == null) continue;
                
                float offsetDistance = (i + 1) * lineSpacing;
                Vector3 offset = perpendicular * offsetDistance;
                
                // Alternatif olarak çizgileri farklı taraflara yerleştir
                if (i % 2 == 1)
                {
                    offset = -offset;
                }
                
                Vector3 startPos = transform.position + offset;
                Vector3 endPos = startPos + transform.right * lineLength;
                
                additionalLines[i].positionCount = 2;
                additionalLines[i].SetPosition(0, startPos);
                additionalLines[i].SetPosition(1, endPos);
            }
        }

        /// <summary>
        /// Çizgi desenini güncelle
        /// Update line pattern
        /// </summary>
        protected virtual void UpdateLinePattern()
        {
            if (lineRenderer == null) return;

            switch (lineType)
            {
                case LineType.Solid:
                    // Düz çizgi için özel işlem gerekmez
                    break;
                    
                case LineType.Dashed:
                    CreateDashedLine();
                    break;
                    
                case LineType.Dotted:
                    CreateDottedLine();
                    break;
            }
        }

        /// <summary>
        /// Kesikli çizgi oluştur
        /// Create dashed line
        /// </summary>
        protected virtual void CreateDashedLine()
        {
            float totalLength = lineLength;
            float segmentLength = dashLength + dashSpacing;
            int segmentCount = Mathf.FloorToInt(totalLength / segmentLength);
            
            lineRenderer.positionCount = segmentCount * 2;
            
            for (int i = 0; i < segmentCount; i++)
            {
                float startDistance = i * segmentLength;
                float endDistance = startDistance + dashLength;
                
                Vector3 startPos = transform.position + transform.right * startDistance;
                Vector3 endPos = transform.position + transform.right * Mathf.Min(endDistance, totalLength);
                
                lineRenderer.SetPosition(i * 2, startPos);
                lineRenderer.SetPosition(i * 2 + 1, endPos);
            }
        }

        /// <summary>
        /// Noktalı çizgi oluştur
        /// Create dotted line
        /// </summary>
        protected virtual void CreateDottedLine()
        {
            float dotSpacing = dashSpacing;
            int dotCount = Mathf.FloorToInt(lineLength / dotSpacing);
            
            lineRenderer.positionCount = dotCount * 2;
            
            for (int i = 0; i < dotCount; i++)
            {
                float distance = i * dotSpacing;
                Vector3 dotPos = transform.position + transform.right * distance;
                
                // Nokta için çok kısa çizgi segmenti
                lineRenderer.SetPosition(i * 2, dotPos);
                lineRenderer.SetPosition(i * 2 + 1, dotPos + transform.right * 0.05f);
            }
        }

        /// <summary>
        /// Çizgi kalınlığını güncelle
        /// Update line width
        /// </summary>
        protected virtual void UpdateLineWidth()
        {
            if (lineRenderer != null)
            {
                lineRenderer.startWidth = lineWidth;
                lineRenderer.endWidth = lineWidth;
            }

            if (additionalLines != null)
            {
                foreach (var line in additionalLines)
                {
                    if (line != null)
                    {
                        line.startWidth = lineWidth;
                        line.endWidth = lineWidth;
                    }
                }
            }
        }

        protected override void UpdateVisuals()
        {
            if (lineRenderer != null)
            {
                lineRenderer.color = indicatorColor;
            }

            if (additionalLines != null)
            {
                foreach (var line in additionalLines)
                {
                    if (line != null)
                    {
                        line.color = indicatorColor;
                    }
                }
            }
        }

        protected override void UpdateVisibility()
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = isVisible;
            }

            if (additionalLines != null)
            {
                foreach (var line in additionalLines)
                {
                    if (line != null)
                    {
                        line.enabled = isVisible;
                    }
                }
            }
        }

        protected override void UpdateFillEffect()
        {
            if (lineRenderer != null && enableFillEffect)
            {
                Color fillColor = indicatorColor;
                fillColor.a *= currentFillAmount;
                lineRenderer.color = fillColor;

                if (additionalLines != null)
                {
                    foreach (var line in additionalLines)
                    {
                        if (line != null)
                        {
                            line.color = fillColor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Çizgi uzunluğunu ayarla
        /// Set line length
        /// </summary>
        /// <param name="length">Uzunluk / Length</param>
        public void SetLineLength(float length)
        {
            LineLength = length;
        }

        /// <summary>
        /// Çizgi hedef noktasını ayarla
        /// Set line target point
        /// </summary>
        /// <param name="targetPoint">Hedef nokta / Target point</param>
        public void SetLineTarget(Vector3 targetPoint)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, targetPoint);
            
            LineLength = distance;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            transform.right = direction;
        }

        protected virtual void OnDestroy()
        {
            if (additionalLines != null)
            {
                foreach (var line in additionalLines)
                {
                    if (line != null && line.gameObject != null)
                    {
                        DestroyImmediate(line.gameObject);
                    }
                }
            }
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            // Editor'da çizgi yönünü göster
            Gizmos.color = Color.cyan;
            Vector3 endPos = transform.position + transform.right * lineLength;
            Gizmos.DrawLine(transform.position, endPos);
            
            // Çizgi başlangıcını işaretle
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }
#endif
    }
}