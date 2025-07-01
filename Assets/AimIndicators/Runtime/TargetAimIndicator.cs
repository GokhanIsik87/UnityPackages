using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Circular target aim indicator for area targeting
    /// Alan hedefleme için dairesel hedef nişan göstergesi
    /// </summary>
    public class TargetAimIndicator : AimIndicatorBase
    {
        [Header("Target Settings / Hedef Ayarları")]
        [SerializeField] private float innerRadius = 1f;
        [SerializeField] private float outerRadius = 3f;
        [SerializeField] [Range(3, 100)] private int circleResolution = 36;
        [SerializeField] private bool showCrosshair = true;
        [SerializeField] private bool showRings = true;
        [SerializeField] [Range(1, 5)] private int ringCount = 3;
        
        [Header("Visual Effects / Görsel Efektler")]
        [SerializeField] private bool rotateTarget = false;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private bool pulseEffect = false;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmplitude = 0.2f;
        
        private Mesh targetMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private LineRenderer[] ringRenderers;
        private LineRenderer crosshairRenderer;
        
        private float currentRotation = 0f;
        private float currentPulse = 1f;
        
        protected override void InitializeIndicator()
        {
            base.InitializeIndicator();
            
            // Setup mesh components
            meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
                
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();
            
            // Setup crosshair
            if (showCrosshair)
            {
                GameObject crosshairObj = new GameObject("Crosshair");
                crosshairObj.transform.SetParent(transform);
                crosshairObj.transform.localPosition = Vector3.zero;
                crosshairRenderer = crosshairObj.AddComponent<LineRenderer>();
                SetupCrosshairRenderer();
            }
            
            // Setup rings
            if (showRings)
            {
                CreateRingRenderers();
            }
            
            CreateTargetMesh();
        }
        
        protected override void SetupMaterial()
        {
            if (meshRenderer != null)
            {
                indicatorMaterial = new Material(Shader.Find("Sprites/Default"));
                meshRenderer.material = indicatorMaterial;
                
                // Setup material for transparency
                indicatorMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                indicatorMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                indicatorMaterial.SetInt("_ZWrite", 0);
                indicatorMaterial.DisableKeyword("_ALPHATEST_ON");
                indicatorMaterial.EnableKeyword("_ALPHABLEND_ON");
                indicatorMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                indicatorMaterial.renderQueue = 3000;
                
                UpdateMaterialColor();
            }
        }
        
        private void SetupCrosshairRenderer()
        {
            if (crosshairRenderer != null)
            {
                crosshairRenderer.material = new Material(Shader.Find("Sprites/Default"));
                crosshairRenderer.color = indicatorColor;
                crosshairRenderer.width = 0.05f;
                crosshairRenderer.useWorldSpace = false;
                crosshairRenderer.positionCount = 4;
                crosshairRenderer.sortingOrder = 2;
            }
        }
        
        private void CreateRingRenderers()
        {
            ringRenderers = new LineRenderer[ringCount];
            
            for (int i = 0; i < ringCount; i++)
            {
                GameObject ringObj = new GameObject($"Ring_{i}");
                ringObj.transform.SetParent(transform);
                ringObj.transform.localPosition = Vector3.zero;
                
                LineRenderer ring = ringObj.AddComponent<LineRenderer>();
                ring.material = new Material(Shader.Find("Sprites/Default"));
                ring.color = indicatorColor;
                ring.width = 0.03f;
                ring.useWorldSpace = false;
                ring.positionCount = circleResolution + 1;
                ring.sortingOrder = 1;
                
                ringRenderers[i] = ring;
            }
        }
        
        private void CreateTargetMesh()
        {
            targetMesh = new Mesh();
            targetMesh.name = "TargetIndicatorMesh";
            
            UpdateTargetMesh();
            
            if (meshFilter != null)
                meshFilter.mesh = targetMesh;
        }
        
        private void UpdateTargetMesh()
        {
            if (targetMesh == null) return;
            
            // Create circle mesh with hole in center
            int vertexCount = (circleResolution + 1) * 2; // Inner and outer circles
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uv = new Vector2[vertexCount];
            int[] triangles = new int[circleResolution * 6]; // Two triangles per segment
            
            float angleStep = 2f * Mathf.PI / circleResolution;
            
            // Create vertices for inner and outer circles
            for (int i = 0; i <= circleResolution; i++)
            {
                float angle = i * angleStep;
                float cos = Mathf.Cos(angle);
                float sin = Mathf.Sin(angle);
                
                // Inner circle vertices
                vertices[i] = new Vector3(cos * innerRadius, 0, sin * innerRadius);
                uv[i] = new Vector2((cos + 1) * 0.3f + 0.35f, (sin + 1) * 0.3f + 0.35f);
                
                // Outer circle vertices
                vertices[i + circleResolution + 1] = new Vector3(cos * outerRadius, 0, sin * outerRadius);
                uv[i + circleResolution + 1] = new Vector2((cos + 1) * 0.5f, (sin + 1) * 0.5f);
            }
            
            // Create triangles
            int triIndex = 0;
            for (int i = 0; i < circleResolution; i++)
            {
                int innerCurrent = i;
                int innerNext = i + 1;
                int outerCurrent = i + circleResolution + 1;
                int outerNext = i + circleResolution + 2;
                
                // First triangle
                triangles[triIndex] = innerCurrent;
                triangles[triIndex + 1] = outerCurrent;
                triangles[triIndex + 2] = innerNext;
                
                // Second triangle
                triangles[triIndex + 3] = innerNext;
                triangles[triIndex + 4] = outerCurrent;
                triangles[triIndex + 5] = outerNext;
                
                triIndex += 6;
            }
            
            targetMesh.Clear();
            targetMesh.vertices = vertices;
            targetMesh.triangles = triangles;
            targetMesh.uv = uv;
            targetMesh.RecalculateNormals();
            
            // Update rings and crosshair
            UpdateRings();
            UpdateCrosshair();
        }
        
        private void UpdateRings()
        {
            if (ringRenderers == null || !showRings) return;
            
            for (int ringIndex = 0; ringIndex < ringRenderers.Length; ringIndex++)
            {
                if (ringRenderers[ringIndex] == null) continue;
                
                float ringRadius = Mathf.Lerp(innerRadius, outerRadius, (float)(ringIndex + 1) / (ringCount + 1));
                Vector3[] positions = new Vector3[circleResolution + 1];
                
                float angleStep = 2f * Mathf.PI / circleResolution;
                for (int i = 0; i <= circleResolution; i++)
                {
                    float angle = i * angleStep;
                    positions[i] = new Vector3(
                        Mathf.Cos(angle) * ringRadius,
                        0,
                        Mathf.Sin(angle) * ringRadius
                    );
                }
                
                ringRenderers[ringIndex].SetPositions(positions);
                ringRenderers[ringIndex].color = Color.Lerp(indicatorColor, fillColor, 0.5f);
            }
        }
        
        private void UpdateCrosshair()
        {
            if (crosshairRenderer == null || !showCrosshair) return;
            
            float crosshairSize = innerRadius * 0.8f;
            Vector3[] positions = new Vector3[4];
            
            // Horizontal line
            positions[0] = new Vector3(-crosshairSize, 0, 0);
            positions[1] = new Vector3(crosshairSize, 0, 0);
            
            // Vertical line
            positions[2] = new Vector3(0, 0, -crosshairSize);
            positions[3] = new Vector3(0, 0, crosshairSize);
            
            crosshairRenderer.SetPositions(positions);
            crosshairRenderer.color = indicatorColor;
        }
        
        protected override void UpdateIndicator()
        {
            // Position target at aim point
            if (targetTransform != null)
            {
                Vector3 direction = (targetTransform.position - transform.position).normalized;
                Vector3 targetPosition = transform.position + direction * indicatorRange;
                transform.position = targetPosition;
                aimDirection = direction;
            }
            else if (aimDirection != Vector3.zero)
            {
                Vector3 targetPosition = transform.position + aimDirection * indicatorRange;
                transform.position = targetPosition;
            }
            
            // Apply rotation effect
            if (rotateTarget)
            {
                currentRotation += rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            }
            
            // Apply pulse effect
            if (pulseEffect)
            {
                currentPulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
                float scaledInnerRadius = innerRadius * currentPulse;
                float scaledOuterRadius = outerRadius * currentPulse;
                
                // Update scale
                transform.localScale = Vector3.one * currentPulse;
            }
            
            UpdateMaterialColor();
        }
        
        private void UpdateMaterialColor()
        {
            if (indicatorMaterial != null && enableFill)
            {
                Color currentColor = Color.Lerp(fillColor, indicatorColor, fillAmount);
                currentColor.a = fillColor.a * fillAmount;
                indicatorMaterial.color = currentColor;
            }
            
            // Update ring colors
            if (ringRenderers != null)
            {
                foreach (var ring in ringRenderers)
                {
                    if (ring != null)
                    {
                        ring.color = Color.Lerp(indicatorColor, fillColor, 0.3f);
                    }
                }
            }
            
            // Update crosshair color
            if (crosshairRenderer != null)
            {
                crosshairRenderer.color = indicatorColor;
            }
        }
        
        public override void SetRange(float range)
        {
            base.SetRange(range);
            // Range affects the distance from origin, not the target size
        }
        
        /// <summary>
        /// Set target inner radius
        /// Hedef iç yarıçapını ayarla
        /// </summary>
        public void SetInnerRadius(float radius)
        {
            innerRadius = Mathf.Max(0.1f, radius);
            if (innerRadius >= outerRadius)
                outerRadius = innerRadius + 0.5f;
            UpdateTargetMesh();
        }
        
        /// <summary>
        /// Set target outer radius
        /// Hedef dış yarıçapını ayarla
        /// </summary>
        public void SetOuterRadius(float radius)
        {
            outerRadius = Mathf.Max(innerRadius + 0.1f, radius);
            UpdateTargetMesh();
        }
        
        /// <summary>
        /// Toggle crosshair visibility
        /// Artı işareti görünürlüğünü aç/kapat
        /// </summary>
        public void SetCrosshairVisible(bool visible)
        {
            showCrosshair = visible;
            if (crosshairRenderer != null)
            {
                crosshairRenderer.enabled = visible;
            }
        }
        
        /// <summary>
        /// Toggle rings visibility
        /// Halka görünürlüğünü aç/kapat
        /// </summary>
        public void SetRingsVisible(bool visible)
        {
            showRings = visible;
            if (ringRenderers != null)
            {
                foreach (var ring in ringRenderers)
                {
                    if (ring != null)
                    {
                        ring.enabled = visible;
                    }
                }
            }
        }
        
        /// <summary>
        /// Set rotation effect
        /// Döndürme efektini ayarla
        /// </summary>
        public void SetRotationEffect(bool enable, float speed = 30f)
        {
            rotateTarget = enable;
            rotationSpeed = speed;
        }
        
        /// <summary>
        /// Set pulse effect
        /// Nabız efektini ayarla
        /// </summary>
        public void SetPulseEffect(bool enable, float speed = 2f, float amplitude = 0.2f)
        {
            pulseEffect = enable;
            pulseSpeed = speed;
            pulseAmplitude = amplitude;
        }
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            
            if (meshRenderer != null)
                meshRenderer.enabled = active;
                
            if (ringRenderers != null)
            {
                foreach (var ring in ringRenderers)
                {
                    if (ring != null)
                        ring.enabled = active && showRings;
                }
            }
            
            if (crosshairRenderer != null)
                crosshairRenderer.enabled = active && showCrosshair;
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw target circles in editor
            Gizmos.color = indicatorColor;
            
            Vector3 center = transform.position;
            
            // Draw inner circle
            for (int i = 0; i < circleResolution; i++)
            {
                float angle1 = (float)i / circleResolution * 2f * Mathf.PI;
                float angle2 = (float)(i + 1) / circleResolution * 2f * Mathf.PI;
                
                Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * innerRadius, 0, Mathf.Sin(angle1) * innerRadius);
                Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * innerRadius, 0, Mathf.Sin(angle2) * innerRadius);
                
                Gizmos.DrawLine(point1, point2);
            }
            
            // Draw outer circle
            for (int i = 0; i < circleResolution; i++)
            {
                float angle1 = (float)i / circleResolution * 2f * Mathf.PI;
                float angle2 = (float)(i + 1) / circleResolution * 2f * Mathf.PI;
                
                Vector3 point1 = center + new Vector3(Mathf.Cos(angle1) * outerRadius, 0, Mathf.Sin(angle1) * outerRadius);
                Vector3 point2 = center + new Vector3(Mathf.Cos(angle2) * outerRadius, 0, Mathf.Sin(angle2) * outerRadius);
                
                Gizmos.DrawLine(point1, point2);
            }
            
            // Draw crosshair
            if (showCrosshair)
            {
                float crossSize = innerRadius * 0.8f;
                Gizmos.DrawLine(center + Vector3.left * crossSize, center + Vector3.right * crossSize);
                Gizmos.DrawLine(center + Vector3.back * crossSize, center + Vector3.forward * crossSize);
            }
        }
        
        private void OnDestroy()
        {
            if (targetMesh != null)
            {
                DestroyImmediate(targetMesh);
            }
        }
    }
}