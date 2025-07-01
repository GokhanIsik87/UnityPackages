using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Parabolic trajectory aim indicator for projectile targeting
    /// Mermi hedefleme için parabolik yörünge nişan göstergesi
    /// </summary>
    public class ParabolicAimIndicator : AimIndicatorBase
    {
        [Header("Parabolic Settings / Parabolik Ayarlar")]
        [SerializeField] private float projectileSpeed = 10f;
        [SerializeField] private float gravity = 9.81f;
        [SerializeField] private float maxTrajectoryTime = 5f;
        [SerializeField] [Range(10, 100)] private int trajectoryResolution = 50;
        [SerializeField] private bool showLandingPoint = true;
        [SerializeField] private bool showApexPoint = true;
        [SerializeField] private float landingIndicatorSize = 1f;
        
        [Header("Visual Settings / Görsel Ayarlar")]
        [SerializeField] private bool showTrajectory = true;
        [SerializeField] private bool showVelocityVector = false;
        [SerializeField] private float trajectoryWidth = 0.08f;
        [SerializeField] private Gradient trajectoryGradient;
        
        private LineRenderer trajectoryRenderer;
        private LineRenderer velocityRenderer;
        private Transform landingIndicator;
        private Transform apexIndicator;
        private Vector3[] trajectoryPoints;
        
        private Vector3 launchVelocity;
        private Vector3 landingPoint;
        private Vector3 apexPoint;
        private float flightTime;
        
        protected override void InitializeIndicator()
        {
            base.InitializeIndicator();
            
            // Setup trajectory renderer
            trajectoryRenderer = GetComponent<LineRenderer>();
            if (trajectoryRenderer == null)
                trajectoryRenderer = gameObject.AddComponent<LineRenderer>();
            
            SetupTrajectoryRenderer();
            
            // Setup velocity vector renderer
            if (showVelocityVector)
            {
                GameObject velocityObj = new GameObject("VelocityVector");
                velocityObj.transform.SetParent(transform);
                velocityRenderer = velocityObj.AddComponent<LineRenderer>();
                SetupVelocityRenderer();
            }
            
            // Setup landing indicator
            if (showLandingPoint)
            {
                CreateLandingIndicator();
            }
            
            // Setup apex indicator
            if (showApexPoint)
            {
                CreateApexIndicator();
            }
            
            // Initialize gradient
            if (trajectoryGradient == null)
            {
                trajectoryGradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[2];
                colorKeys[0] = new GradientColorKey(indicatorColor, 0f);
                colorKeys[1] = new GradientColorKey(Color.yellow, 1f);
                
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
                alphaKeys[0] = new GradientAlphaKey(1f, 0f);
                alphaKeys[1] = new GradientAlphaKey(0.5f, 1f);
                
                trajectoryGradient.SetKeys(colorKeys, alphaKeys);
            }
            
            trajectoryPoints = new Vector3[trajectoryResolution];
        }
        
        protected override void SetupMaterial()
        {
            if (trajectoryRenderer != null)
            {
                indicatorMaterial = new Material(Shader.Find("Sprites/Default"));
                trajectoryRenderer.material = indicatorMaterial;
            }
        }
        
        private void SetupTrajectoryRenderer()
        {
            if (trajectoryRenderer != null)
            {
                trajectoryRenderer.width = trajectoryWidth;
                trajectoryRenderer.useWorldSpace = true;
                trajectoryRenderer.positionCount = trajectoryResolution;
                trajectoryRenderer.material = new Material(Shader.Find("Sprites/Default"));
                trajectoryRenderer.sortingOrder = 1;
                
                // Enable gradient coloring
                trajectoryRenderer.colorGradient = trajectoryGradient;
            }
        }
        
        private void SetupVelocityRenderer()
        {
            if (velocityRenderer != null)
            {
                velocityRenderer.width = trajectoryWidth * 1.5f;
                velocityRenderer.useWorldSpace = true;
                velocityRenderer.positionCount = 2;
                velocityRenderer.material = new Material(Shader.Find("Sprites/Default"));
                velocityRenderer.color = Color.cyan;
                velocityRenderer.sortingOrder = 2;
            }
        }
        
        private void CreateLandingIndicator()
        {
            GameObject landingObj = new GameObject("LandingIndicator");
            landingObj.transform.SetParent(transform);
            landingIndicator = landingObj.transform;
            
            // Create a simple circle mesh for landing indicator
            MeshRenderer landingRenderer = landingObj.AddComponent<MeshRenderer>();
            MeshFilter landingFilter = landingObj.AddComponent<MeshFilter>();
            
            landingRenderer.material = new Material(Shader.Find("Sprites/Default"));
            landingRenderer.material.color = new Color(indicatorColor.r, indicatorColor.g, indicatorColor.b, 0.7f);
            
            // Create circle mesh
            Mesh circleMesh = CreateCircleMesh(landingIndicatorSize, 16);
            landingFilter.mesh = circleMesh;
        }
        
        private void CreateApexIndicator()
        {
            GameObject apexObj = new GameObject("ApexIndicator");
            apexObj.transform.SetParent(transform);
            apexIndicator = apexObj.transform;
            
            // Create a small sphere for apex indicator
            MeshRenderer apexRenderer = apexObj.AddComponent<MeshRenderer>();
            MeshFilter apexFilter = apexObj.AddComponent<MeshFilter>();
            
            apexRenderer.material = new Material(Shader.Find("Sprites/Default"));
            apexRenderer.material.color = Color.green;
            
            // Create small sphere mesh
            GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            apexFilter.mesh = tempSphere.GetComponent<MeshFilter>().mesh;
            DestroyImmediate(tempSphere);
            
            apexObj.transform.localScale = Vector3.one * 0.2f;
        }
        
        private Mesh CreateCircleMesh(float radius, int segments)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[segments + 1];
            int[] triangles = new int[segments * 3];
            Vector2[] uv = new Vector2[vertices.Length];
            
            // Center vertex
            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0.5f);
            
            // Circle vertices
            float angleStep = 2f * Mathf.PI / segments;
            for (int i = 0; i < segments; i++)
            {
                float angle = i * angleStep;
                vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                uv[i + 1] = new Vector2((Mathf.Cos(angle) + 1) * 0.5f, (Mathf.Sin(angle) + 1) * 0.5f);
            }
            
            // Triangles
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % segments + 1;
            }
            
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
            
            return mesh;
        }
        
        protected override void UpdateIndicator()
        {
            CalculateTrajectory();
            UpdateTrajectoryVisualization();
            UpdateIndicators();
        }
        
        private void CalculateTrajectory()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos;
            
            if (targetTransform != null)
            {
                targetPos = targetTransform.position;
                aimDirection = (targetPos - startPos).normalized;
            }
            else if (aimDirection != Vector3.zero)
            {
                targetPos = startPos + aimDirection * indicatorRange;
            }
            else
            {
                targetPos = startPos + transform.forward * indicatorRange;
                aimDirection = transform.forward;
            }
            
            // Calculate launch angle and velocity for parabolic trajectory
            Vector3 displacement = targetPos - startPos;
            float horizontalDistance = new Vector3(displacement.x, 0, displacement.z).magnitude;
            float verticalDistance = displacement.y;
            
            // Calculate optimal launch angle (45 degrees adjusted for height difference)
            float discriminant = Mathf.Pow(projectileSpeed, 4) - gravity * (gravity * horizontalDistance * horizontalDistance + 2 * verticalDistance * projectileSpeed * projectileSpeed);
            
            if (discriminant >= 0)
            {
                float angle = Mathf.Atan((projectileSpeed * projectileSpeed - Mathf.Sqrt(discriminant)) / (gravity * horizontalDistance));
                
                Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;
                launchVelocity = horizontalDirection * projectileSpeed * Mathf.Cos(angle) + Vector3.up * projectileSpeed * Mathf.Sin(angle);
                
                // Calculate flight time
                flightTime = horizontalDistance / (projectileSpeed * Mathf.Cos(angle));
                flightTime = Mathf.Min(flightTime, maxTrajectoryTime);
                
                // Calculate trajectory points
                for (int i = 0; i < trajectoryResolution; i++)
                {
                    float t = (float)i / (trajectoryResolution - 1) * flightTime * fillAmount;
                    trajectoryPoints[i] = CalculatePositionAtTime(startPos, launchVelocity, t);
                }
                
                // Calculate landing point
                landingPoint = CalculatePositionAtTime(startPos, launchVelocity, flightTime);
                
                // Calculate apex point
                float apexTime = launchVelocity.y / gravity;
                if (apexTime > 0 && apexTime < flightTime)
                {
                    apexPoint = CalculatePositionAtTime(startPos, launchVelocity, apexTime);
                }
                else
                {
                    apexPoint = startPos;
                }
            }
            else
            {
                // Target is unreachable with current speed, draw straight line
                for (int i = 0; i < trajectoryResolution; i++)
                {
                    float t = (float)i / (trajectoryResolution - 1);
                    trajectoryPoints[i] = Vector3.Lerp(startPos, targetPos, t * fillAmount);
                }
                landingPoint = targetPos;
                apexPoint = Vector3.Lerp(startPos, targetPos, 0.5f);
            }
        }
        
        private Vector3 CalculatePositionAtTime(Vector3 startPos, Vector3 velocity, float time)
        {
            return startPos + velocity * time + 0.5f * Vector3.down * gravity * time * time;
        }
        
        private void UpdateTrajectoryVisualization()
        {
            if (trajectoryRenderer != null && showTrajectory)
            {
                trajectoryRenderer.positionCount = trajectoryResolution;
                trajectoryRenderer.SetPositions(trajectoryPoints);
                trajectoryRenderer.width = trajectoryWidth;
                
                // Update color gradient
                UpdateTrajectoryColors();
            }
            
            // Update velocity vector
            if (velocityRenderer != null && showVelocityVector)
            {
                Vector3 startPos = transform.position;
                Vector3 velocityEnd = startPos + launchVelocity.normalized * 2f;
                
                velocityRenderer.SetPosition(0, startPos);
                velocityRenderer.SetPosition(1, velocityEnd);
            }
        }
        
        private void UpdateTrajectoryColors()
        {
            if (trajectoryRenderer == null) return;
            
            // Update gradient colors based on current indicator color
            GradientColorKey[] colorKeys = new GradientColorKey[3];
            colorKeys[0] = new GradientColorKey(indicatorColor, 0f);
            colorKeys[1] = new GradientColorKey(Color.Lerp(indicatorColor, fillColor, 0.5f), 0.5f);
            colorKeys[2] = new GradientColorKey(fillColor, 1f);
            
            GradientAlphaKey[] alphaKeys = trajectoryGradient.alphaKeys;
            
            trajectoryGradient.SetKeys(colorKeys, alphaKeys);
            trajectoryRenderer.colorGradient = trajectoryGradient;
        }
        
        private void UpdateIndicators()
        {
            // Update landing indicator
            if (landingIndicator != null && showLandingPoint)
            {
                landingIndicator.position = landingPoint;
                landingIndicator.gameObject.SetActive(isActive);
            }
            
            // Update apex indicator
            if (apexIndicator != null && showApexPoint)
            {
                apexIndicator.position = apexPoint;
                apexIndicator.gameObject.SetActive(isActive);
            }
        }
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);
            
            if (trajectoryRenderer != null)
                trajectoryRenderer.enabled = active && showTrajectory;
                
            if (velocityRenderer != null)
                velocityRenderer.enabled = active && showVelocityVector;
                
            if (landingIndicator != null)
                landingIndicator.gameObject.SetActive(active && showLandingPoint);
                
            if (apexIndicator != null)
                apexIndicator.gameObject.SetActive(active && showApexPoint);
        }
        
        /// <summary>
        /// Set projectile speed for trajectory calculation
        /// Yörünge hesaplaması için mermi hızını ayarla
        /// </summary>
        public void SetProjectileSpeed(float speed)
        {
            projectileSpeed = Mathf.Max(0.1f, speed);
        }
        
        /// <summary>
        /// Set gravity value for trajectory calculation
        /// Yörünge hesaplaması için yerçekimi değerini ayarla
        /// </summary>
        public void SetGravity(float gravityValue)
        {
            gravity = Mathf.Max(0.1f, gravityValue);
        }
        
        /// <summary>
        /// Set maximum trajectory time
        /// Maksimum yörünge süresini ayarla
        /// </summary>
        public void SetMaxTrajectoryTime(float maxTime)
        {
            maxTrajectoryTime = Mathf.Max(0.1f, maxTime);
        }
        
        /// <summary>
        /// Toggle trajectory visibility
        /// Yörünge görünürlüğünü aç/kapat
        /// </summary>
        public void SetTrajectoryVisible(bool visible)
        {
            showTrajectory = visible;
            if (trajectoryRenderer != null)
            {
                trajectoryRenderer.enabled = visible && isActive;
            }
        }
        
        /// <summary>
        /// Toggle velocity vector visibility
        /// Hız vektörü görünürlüğünü aç/kapat
        /// </summary>
        public void SetVelocityVectorVisible(bool visible)
        {
            showVelocityVector = visible;
            if (velocityRenderer != null)
            {
                velocityRenderer.enabled = visible && isActive;
            }
        }
        
        /// <summary>
        /// Toggle landing point indicator
        /// İniş noktası göstergesini aç/kapat
        /// </summary>
        public void SetLandingPointVisible(bool visible)
        {
            showLandingPoint = visible;
            if (landingIndicator != null)
            {
                landingIndicator.gameObject.SetActive(visible && isActive);
            }
        }
        
        /// <summary>
        /// Toggle apex point indicator
        /// Tepe noktası göstergesini aç/kapat
        /// </summary>
        public void SetApexPointVisible(bool visible)
        {
            showApexPoint = visible;
            if (apexIndicator != null)
            {
                apexIndicator.gameObject.SetActive(visible && isActive);
            }
        }
        
        /// <summary>
        /// Set trajectory width
        /// Yörünge kalınlığını ayarla
        /// </summary>
        public void SetTrajectoryWidth(float width)
        {
            trajectoryWidth = Mathf.Max(0.01f, width);
            if (trajectoryRenderer != null)
            {
                trajectoryRenderer.width = trajectoryWidth;
            }
        }
        
        /// <summary>
        /// Get the calculated landing point
        /// Hesaplanan iniş noktasını al
        /// </summary>
        public Vector3 GetLandingPoint()
        {
            return landingPoint;
        }
        
        /// <summary>
        /// Get the calculated flight time
        /// Hesaplanan uçuş süresini al
        /// </summary>
        public float GetFlightTime()
        {
            return flightTime;
        }
        
        /// <summary>
        /// Get the launch velocity
        /// Fırlatma hızını al
        /// </summary>
        public Vector3 GetLaunchVelocity()
        {
            return launchVelocity;
        }
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            if (trajectoryPoints != null && trajectoryPoints.Length > 1)
            {
                // Draw trajectory in editor
                Gizmos.color = indicatorColor;
                for (int i = 0; i < trajectoryPoints.Length - 1; i++)
                {
                    Gizmos.DrawLine(trajectoryPoints[i], trajectoryPoints[i + 1]);
                }
                
                // Draw landing point
                if (showLandingPoint)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(landingPoint, landingIndicatorSize);
                }
                
                // Draw apex point
                if (showApexPoint)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(apexPoint, 0.3f);
                }
                
                // Draw velocity vector
                if (showVelocityVector)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position, transform.position + launchVelocity.normalized * 2f);
                }
            }
        }
    }
}