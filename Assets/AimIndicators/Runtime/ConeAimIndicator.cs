using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Cone-shaped aim indicator for wide area targeting
    /// Geniş alan hedefleme için koni şeklinde nişan göstergesi
    /// </summary>
    public class ConeAimIndicator : AimIndicatorBase
    {
        [Header("Cone Settings / Koni Ayarları")]
        [SerializeField] [Range(5f, 180f)] private float coneAngle = 45f;
        [SerializeField] [Range(3, 50)] private int coneResolution = 20;
        [SerializeField] private bool showConeOutline = true;
        
        private Mesh coneMesh;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private LineRenderer outlineRenderer;
        
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
            
            // Setup outline renderer
            if (showConeOutline)
            {
                GameObject outlineObj = new GameObject("ConeOutline");
                outlineObj.transform.SetParent(transform);
                outlineObj.transform.localPosition = Vector3.zero;
                outlineRenderer = outlineObj.AddComponent<LineRenderer>();
                SetupOutlineRenderer();
            }
            
            CreateConeMesh();
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
        
        private void SetupOutlineRenderer()
        {
            if (outlineRenderer != null)
            {
                outlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                outlineRenderer.color = indicatorColor;
                outlineRenderer.width = 0.05f;
                outlineRenderer.useWorldSpace = false;
                outlineRenderer.positionCount = coneResolution + 2;
            }
        }
        
        private void CreateConeMesh()
        {
            coneMesh = new Mesh();
            coneMesh.name = "ConeIndicatorMesh";
            
            UpdateConeMesh();
            
            if (meshFilter != null)
                meshFilter.mesh = coneMesh;
        }
        
        private void UpdateConeMesh()
        {
            if (coneMesh == null) return;
            
            Vector3[] vertices = new Vector3[coneResolution + 2];
            int[] triangles = new int[coneResolution * 3];
            Vector2[] uv = new Vector2[vertices.Length];
            
            // Center vertex
            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0.5f);
            
            // Create cone vertices
            float angleStep = coneAngle * Mathf.Deg2Rad / coneResolution;
            float startAngle = -coneAngle * 0.5f * Mathf.Deg2Rad;
            
            for (int i = 0; i <= coneResolution; i++)
            {
                float angle = startAngle + angleStep * i;
                float x = Mathf.Sin(angle) * indicatorRange;
                float z = Mathf.Cos(angle) * indicatorRange;
                
                vertices[i + 1] = new Vector3(x, 0, z);
                
                // UV mapping for fill effect
                float u = (Mathf.Sin(angle) + 1) * 0.5f;
                float v = (Mathf.Cos(angle) + 1) * 0.5f;
                uv[i + 1] = new Vector2(u, v);
            }
            
            // Create triangles
            for (int i = 0; i < coneResolution; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            
            coneMesh.Clear();
            coneMesh.vertices = vertices;
            coneMesh.triangles = triangles;
            coneMesh.uv = uv;
            coneMesh.RecalculateNormals();
            
            // Update outline
            UpdateOutline();
        }
        
        private void UpdateOutline()
        {
            if (outlineRenderer == null || !showConeOutline) return;
            
            Vector3[] positions = new Vector3[coneResolution + 2];
            positions[0] = Vector3.zero;
            
            float angleStep = coneAngle * Mathf.Deg2Rad / coneResolution;
            float startAngle = -coneAngle * 0.5f * Mathf.Deg2Rad;
            
            for (int i = 0; i <= coneResolution; i++)
            {
                float angle = startAngle + angleStep * i;
                float x = Mathf.Sin(angle) * indicatorRange;
                float z = Mathf.Cos(angle) * indicatorRange;
                positions[i + 1] = new Vector3(x, 0, z);
            }
            
            outlineRenderer.positionCount = positions.Length;
            outlineRenderer.SetPositions(positions);
        }
        
        protected override void UpdateIndicator()
        {
            if (targetTransform != null)
            {
                Vector3 direction = (targetTransform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
                aimDirection = direction;
            }
            else if (aimDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(aimDirection);
            }
            
            UpdateMaterialColor();
        }
        
        private void UpdateMaterialColor()
        {
            if (indicatorMaterial != null && enableFill)
            {
                Color currentColor = Color.Lerp(fillColor, indicatorColor, 1f - fillAmount);
                currentColor.a = fillColor.a * fillAmount;
                indicatorMaterial.color = currentColor;
            }
            
            if (outlineRenderer != null)
            {
                outlineRenderer.color = indicatorColor;
            }
        }
        
        public override void SetRange(float range)
        {
            base.SetRange(range);
            UpdateConeMesh();
        }
        
        /// <summary>
        /// Set the cone angle in degrees
        /// Koni açısını derece cinsinden ayarla
        /// </summary>
        public void SetConeAngle(float angle)
        {
            coneAngle = Mathf.Clamp(angle, 5f, 180f);
            UpdateConeMesh();
        }
        
        /// <summary>
        /// Toggle cone outline visibility
        /// Koni çerçevesi görünürlüğünü aç/kapat
        /// </summary>
        public void SetOutlineVisible(bool visible)
        {
            showConeOutline = visible;
            if (outlineRenderer != null)
            {
                outlineRenderer.enabled = visible;
            }
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            // Draw cone wireframe in editor
            Gizmos.color = indicatorColor;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            
            float halfAngle = coneAngle * 0.5f * Mathf.Deg2Rad;
            Vector3 leftEdge = Quaternion.AngleAxis(-coneAngle * 0.5f, transform.up) * forward * indicatorRange;
            Vector3 rightEdge = Quaternion.AngleAxis(coneAngle * 0.5f, transform.up) * forward * indicatorRange;
            
            Gizmos.DrawLine(transform.position, transform.position + leftEdge);
            Gizmos.DrawLine(transform.position, transform.position + rightEdge);
        }
        
        private void OnDestroy()
        {
            if (coneMesh != null)
            {
                DestroyImmediate(coneMesh);
            }
        }
    }
}