using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Line-based aim indicator for precise targeting
    /// Hassas hedefleme için çizgi tabanlı nişan göstergesi
    /// </summary>
    public class LineAimIndicator : AimIndicatorBase
    {
        [Header("Line Settings / Çizgi Ayarları")]
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private bool showArrowHead = true;
        [SerializeField] private float arrowHeadSize = 0.5f;
        [SerializeField] private LineType lineType = LineType.Solid;
        [SerializeField] private float dashLength = 0.5f;
        [SerializeField] private float gapLength = 0.3f;
        
        public enum LineType
        {
            Solid,      // Düz çizgi
            Dashed,     // Kesikli çizgi
            Dotted      // Noktalı çizgi
        }
        
        private LineRenderer lineRenderer;
        private LineRenderer arrowHeadRenderer;
        private Mesh lineMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        
        protected override void InitializeIndicator()
        {
            base.InitializeIndicator();
            
            // Setup LineRenderer for main line
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            
            // Setup arrow head
            if (showArrowHead)
            {
                GameObject arrowObj = new GameObject("ArrowHead");
                arrowObj.transform.SetParent(transform);
                arrowHeadRenderer = arrowObj.AddComponent<LineRenderer>();
                SetupArrowHeadRenderer();
            }
            
            SetupLineRenderer();
        }
        
        protected override void SetupMaterial()
        {
            if (lineRenderer != null)
            {
                indicatorMaterial = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.material = indicatorMaterial;
                lineRenderer.color = indicatorColor;
            }
        }
        
        private void SetupLineRenderer()
        {
            if (lineRenderer != null)
            {
                lineRenderer.width = lineWidth;
                lineRenderer.useWorldSpace = true;
                lineRenderer.positionCount = 2;
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                lineRenderer.color = indicatorColor;
                
                // Configure for world space
                lineRenderer.sortingOrder = 1;
            }
        }
        
        private void SetupArrowHeadRenderer()
        {
            if (arrowHeadRenderer != null)
            {
                arrowHeadRenderer.width = lineWidth * 1.5f;
                arrowHeadRenderer.useWorldSpace = true;
                arrowHeadRenderer.positionCount = 4;
                arrowHeadRenderer.material = new Material(Shader.Find("Sprites/Default"));
                arrowHeadRenderer.color = indicatorColor;
                arrowHeadRenderer.sortingOrder = 2;
            }
        }
        
        protected override void UpdateIndicator()
        {
            if (lineRenderer == null) return;
            
            Vector3 startPos = transform.position;
            Vector3 endPos;
            
            if (targetTransform != null)
            {
                Vector3 direction = (targetTransform.position - startPos).normalized;
                endPos = startPos + direction * indicatorRange;
                aimDirection = direction;
            }
            else if (aimDirection != Vector3.zero)
            {
                endPos = startPos + aimDirection * indicatorRange;
            }
            else
            {
                endPos = startPos + transform.forward * indicatorRange;
            }
            
            // Apply fill amount to line length
            Vector3 actualEndPos = Vector3.Lerp(startPos, endPos, fillAmount);
            
            UpdateLinePositions(startPos, actualEndPos);
            UpdateArrowHead(actualEndPos);
            UpdateLineAppearance();
        }
        
        private void UpdateLinePositions(Vector3 start, Vector3 end)
        {
            if (lineType == LineType.Solid)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, start);
                lineRenderer.SetPosition(1, end);
            }
            else
            {
                CreateDashedLine(start, end);
            }
        }
        
        private void CreateDashedLine(Vector3 start, Vector3 end)
        {
            Vector3 direction = (end - start).normalized;
            float totalDistance = Vector3.Distance(start, end);
            
            float segmentLength = lineType == LineType.Dashed ? dashLength : dashLength * 0.3f;
            float gapSize = lineType == LineType.Dashed ? gapLength : gapLength * 0.5f;
            
            int segmentCount = Mathf.FloorToInt(totalDistance / (segmentLength + gapSize));
            lineRenderer.positionCount = segmentCount * 2;
            
            int posIndex = 0;
            for (int i = 0; i < segmentCount; i++)
            {
                float segmentStart = i * (segmentLength + gapSize);
                float segmentEnd = segmentStart + segmentLength;
                
                if (segmentEnd > totalDistance)
                    segmentEnd = totalDistance;
                
                Vector3 segStartPos = start + direction * segmentStart;
                Vector3 segEndPos = start + direction * segmentEnd;
                
                if (posIndex < lineRenderer.positionCount - 1)
                {
                    lineRenderer.SetPosition(posIndex, segStartPos);
                    lineRenderer.SetPosition(posIndex + 1, segEndPos);
                    posIndex += 2;
                }
            }
        }
        
        private void UpdateArrowHead(Vector3 endPos)
        {
            if (arrowHeadRenderer == null || !showArrowHead) return;
            
            Vector3 direction = aimDirection;
            if (direction == Vector3.zero)
                direction = transform.forward;
            
            // Create arrow head shape
            Vector3 arrowTip = endPos;
            Vector3 arrowBase = arrowTip - direction * arrowHeadSize;
            
            // Calculate perpendicular vectors for arrow wings
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            if (right == Vector3.zero)
                right = Vector3.Cross(direction, Vector3.forward).normalized;
            
            Vector3 leftWing = arrowBase + right * (arrowHeadSize * 0.5f);
            Vector3 rightWing = arrowBase - right * (arrowHeadSize * 0.5f);
            
            // Set arrow head positions
            arrowHeadRenderer.positionCount = 4;
            arrowHeadRenderer.SetPosition(0, leftWing);
            arrowHeadRenderer.SetPosition(1, arrowTip);
            arrowHeadRenderer.SetPosition(2, rightWing);
            arrowHeadRenderer.SetPosition(3, arrowBase);
        }
        
        private void UpdateLineAppearance()
        {
            if (lineRenderer != null)
            {
                Color currentColor = Color.Lerp(fillColor, indicatorColor, fillAmount);
                if (enableFill)
                {
                    currentColor.a = fillColor.a;
                }
                lineRenderer.color = currentColor;
                lineRenderer.width = lineWidth;
            }
            
            if (arrowHeadRenderer != null)
            {
                arrowHeadRenderer.color = lineRenderer.color;
                arrowHeadRenderer.width = lineWidth * 1.5f;
            }
        }
        
        public override void SetColor(Color color)
        {
            base.SetColor(color);
            UpdateLineAppearance();
        }
        
        /// <summary>
        /// Set line width
        /// Çizgi kalınlığını ayarla
        /// </summary>
        public void SetLineWidth(float width)
        {
            lineWidth = Mathf.Max(0.01f, width);
            UpdateLineAppearance();
        }
        
        /// <summary>
        /// Set line type (Solid, Dashed, Dotted)
        /// Çizgi tipini ayarla (Düz, Kesikli, Noktalı)
        /// </summary>
        public void SetLineType(LineType type)
        {
            lineType = type;
        }
        
        /// <summary>
        /// Toggle arrow head visibility
        /// Ok başı görünürlüğünü aç/kapat
        /// </summary>
        public void SetArrowHeadVisible(bool visible)
        {
            showArrowHead = visible;
            if (arrowHeadRenderer != null)
            {
                arrowHeadRenderer.enabled = visible;
            }
        }
        
        /// <summary>
        /// Set arrow head size
        /// Ok başı boyutunu ayarla
        /// </summary>
        public void SetArrowHeadSize(float size)
        {
            arrowHeadSize = Mathf.Max(0.1f, size);
        }
        
        /// <summary>
        /// Set dash parameters for dashed/dotted lines
        /// Kesikli/noktalı çizgiler için çizgi parametrelerini ayarla
        /// </summary>
        public void SetDashParameters(float dash, float gap)
        {
            dashLength = Mathf.Max(0.1f, dash);
            gapLength = Mathf.Max(0.1f, gap);
        }
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            
            if (lineRenderer != null)
                lineRenderer.enabled = active;
            
            if (arrowHeadRenderer != null)
                arrowHeadRenderer.enabled = active && showArrowHead;
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw line direction in editor
            Gizmos.color = indicatorColor;
            Vector3 direction = aimDirection != Vector3.zero ? aimDirection : transform.forward;
            Vector3 endPoint = transform.position + direction * indicatorRange;
            
            Gizmos.DrawLine(transform.position, endPoint);
            
            // Draw arrow head in gizmos
            if (showArrowHead)
            {
                Vector3 arrowBase = endPoint - direction * arrowHeadSize;
                Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
                if (right == Vector3.zero)
                    right = Vector3.Cross(direction, Vector3.forward).normalized;
                
                Vector3 leftWing = arrowBase + right * (arrowHeadSize * 0.5f);
                Vector3 rightWing = arrowBase - right * (arrowHeadSize * 0.5f);
                
                Gizmos.DrawLine(leftWing, endPoint);
                Gizmos.DrawLine(rightWing, endPoint);
            }
        }
    }
}