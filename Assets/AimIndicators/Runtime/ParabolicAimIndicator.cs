using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Parabolik yörünge nişan göstergesi
    /// Parabolic trajectory aim indicator
    /// </summary>
    [System.Serializable]
    public class ParabolicAimIndicator : AimIndicatorBase
    {
        [Header("Parabolik Ayarlar / Parabolic Settings")]
        [SerializeField] protected float launchAngle = 45f;
        [SerializeField] protected float launchVelocity = 10f;
        [SerializeField] protected float gravity = -9.81f;
        [SerializeField] protected float maxTime = 5f;
        [SerializeField] protected int trajectoryResolution = 50;
        
        [Header("Görsel Ayarlar / Visual Settings")]
        [SerializeField] protected bool showLandingPoint = true;
        [SerializeField] protected bool showApexPoint = true;
        [SerializeField] protected float pointSize = 0.2f;
        [SerializeField] protected bool showVelocityLines = false;
        [SerializeField] protected int velocityLineCount = 5;
        
        [Header("Fizik Ayarları / Physics Settings")]
        [SerializeField] protected bool useRealPhysics = true;
        [SerializeField] protected float airResistance = 0f;
        [SerializeField] protected float mass = 1f;
        [SerializeField] protected Vector3 windForce = Vector3.zero;

        protected LineRenderer trajectoryLine;
        protected LineRenderer[] velocityLines;
        protected Transform landingPoint;
        protected Transform apexPoint;
        protected Vector3[] trajectoryPoints;
        protected float actualLandingTime;

        /// <summary>
        /// Fırlatma açısı (derece)
        /// Launch angle in degrees
        /// </summary>
        public float LaunchAngle
        {
            get => launchAngle;
            set
            {
                launchAngle = Mathf.Clamp(value, 0f, 90f);
                CalculateTrajectory();
            }
        }

        /// <summary>
        /// Fırlatma hızı
        /// Launch velocity
        /// </summary>
        public float LaunchVelocity
        {
            get => launchVelocity;
            set
            {
                launchVelocity = Mathf.Max(0.1f, value);
                CalculateTrajectory();
            }
        }

        /// <summary>
        /// Yerçekimi kuvveti
        /// Gravity force
        /// </summary>
        public float Gravity
        {
            get => gravity;
            set
            {
                gravity = value;
                CalculateTrajectory();
            }
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            SetupTrajectoryRenderer();
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
        /// Yörünge renderer'ını ayarla
        /// Setup trajectory renderer
        /// </summary>
        protected virtual void SetupTrajectoryRenderer()
        {
            // Ana yörünge çizgisi
            GameObject trajectoryObj = new GameObject("TrajectoryLine");
            trajectoryObj.transform.SetParent(transform);
            trajectoryObj.transform.localPosition = Vector3.zero;
            
            trajectoryLine = trajectoryObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(trajectoryLine);

            // İniş noktası
            if (showLandingPoint)
            {
                SetupLandingPoint();
            }

            // Tepe noktası
            if (showApexPoint)
            {
                SetupApexPoint();
            }

            // Hız çizgileri
            if (showVelocityLines)
            {
                SetupVelocityLines();
            }
        }

        /// <summary>
        /// İniş noktasını ayarla
        /// Setup landing point
        /// </summary>
        protected virtual void SetupLandingPoint()
        {
            GameObject landingObj = new GameObject("LandingPoint");
            landingObj.transform.SetParent(transform);
            landingPoint = landingObj.transform;

            // Görsel gösterim için küçük bir daire
            LineRenderer landingRing = landingObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(landingRing);
            
            // Daire oluştur
            CreateCircle(landingRing, pointSize, 16);
        }

        /// <summary>
        /// Tepe noktasını ayarla
        /// Setup apex point
        /// </summary>
        protected virtual void SetupApexPoint()
        {
            GameObject apexObj = new GameObject("ApexPoint");
            apexObj.transform.SetParent(transform);
            apexPoint = apexObj.transform;

            // Görsel gösterim için küçük bir daire
            LineRenderer apexRing = apexObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(apexRing);
            
            // Daire oluştur
            CreateCircle(apexRing, pointSize * 0.7f, 12);
        }

        /// <summary>
        /// Hız çizgilerini ayarla
        /// Setup velocity lines
        /// </summary>
        protected virtual void SetupVelocityLines()
        {
            velocityLines = new LineRenderer[velocityLineCount];
            
            for (int i = 0; i < velocityLineCount; i++)
            {
                GameObject velObj = new GameObject($"VelocityLine_{i}");
                velObj.transform.SetParent(transform);
                velObj.transform.localPosition = Vector3.zero;
                
                LineRenderer velLine = velObj.AddComponent<LineRenderer>();
                ConfigureLineRenderer(velLine);
                velLine.startWidth = 0.05f;
                velLine.endWidth = 0.05f;
                
                velocityLines[i] = velLine;
            }
        }

        /// <summary>
        /// Daire oluştur
        /// Create circle
        /// </summary>
        /// <param name="lr">LineRenderer</param>
        /// <param name="radius">Yarıçap</param>
        /// <param name="resolution">Çözünürlük</param>
        protected virtual void CreateCircle(LineRenderer lr, float radius, int resolution)
        {
            Vector3[] positions = new Vector3[resolution + 1];
            
            for (int i = 0; i <= resolution; i++)
            {
                float angle = (float)i / resolution * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                positions[i] = new Vector3(x, y, 0);
            }

            lr.positionCount = positions.Length;
            lr.SetPositions(positions);
        }

        /// <summary>
        /// LineRenderer'ı yapılandır
        /// Configure LineRenderer
        /// </summary>
        /// <param name="lr">Yapılandırılacak LineRenderer</param>
        protected virtual void ConfigureLineRenderer(LineRenderer lr)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.useWorldSpace = true;
            lr.sortingOrder = 1;
        }

        protected override void CreateIndicatorSprite()
        {
            CalculateTrajectory();
        }

        /// <summary>
        /// Yörüngeyi hesapla
        /// Calculate trajectory
        /// </summary>
        protected virtual void CalculateTrajectory()
        {
            if (trajectoryLine == null) return;

            trajectoryPoints = new Vector3[trajectoryResolution];
            
            Vector3 startPosition = transform.position;
            float angleRad = launchAngle * Mathf.Deg2Rad;
            Vector3 initialVelocity = new Vector3(
                launchVelocity * Mathf.Cos(angleRad),
                launchVelocity * Mathf.Sin(angleRad),
                0
            );

            float timeStep = maxTime / trajectoryResolution;
            float highestY = startPosition.y;
            int apexIndex = 0;
            int landingIndex = trajectoryResolution - 1;

            for (int i = 0; i < trajectoryResolution; i++)
            {
                float t = i * timeStep;
                Vector3 position = CalculatePositionAtTime(startPosition, initialVelocity, t);
                
                trajectoryPoints[i] = position;

                // Tepe noktası
                if (position.y > highestY)
                {
                    highestY = position.y;
                    apexIndex = i;
                }

                // İniş noktası (yerin altına geçtiği nokta)
                if (position.y <= startPosition.y && i > 0)
                {
                    landingIndex = i;
                    actualLandingTime = t;
                    break;
                }
            }

            // Yörünge çizgisini güncelle
            UpdateTrajectoryLine(landingIndex);
            
            // Özel noktaları güncelle
            UpdateSpecialPoints(apexIndex, landingIndex);
            
            // Hız çizgilerini güncelle
            if (showVelocityLines)
            {
                UpdateVelocityLines(landingIndex);
            }
        }

        /// <summary>
        /// Belirli bir zamandaki pozisyonu hesapla
        /// Calculate position at specific time
        /// </summary>
        /// <param name="startPos">Başlangıç pozisyonu</param>
        /// <param name="initialVel">Başlangıç hızı</param>
        /// <param name="time">Zaman</param>
        /// <returns>Hesaplanan pozisyon</returns>
        protected virtual Vector3 CalculatePositionAtTime(Vector3 startPos, Vector3 initialVel, float time)
        {
            Vector3 position = startPos;
            
            if (useRealPhysics)
            {
                // Gerçekçi fizik (hava direnci ve rüzgar dahil)
                position.x = startPos.x + initialVel.x * time + (windForce.x / mass) * time * time * 0.5f;
                position.y = startPos.y + initialVel.y * time + (gravity + windForce.y / mass) * time * time * 0.5f;
                
                // Hava direnci etkisi
                if (airResistance > 0)
                {
                    float resistanceFactor = Mathf.Exp(-airResistance * time);
                    position.x *= resistanceFactor;
                    position.y = Mathf.Lerp(position.y, startPos.y + gravity * time * time * 0.5f, 1f - resistanceFactor);
                }
            }
            else
            {
                // Basit parabolik hareket
                position.x = startPos.x + initialVel.x * time;
                position.y = startPos.y + initialVel.y * time + gravity * time * time * 0.5f;
            }

            return position;
        }

        /// <summary>
        /// Yörünge çizgisini güncelle
        /// Update trajectory line
        /// </summary>
        /// <param name="pointCount">Nokta sayısı</param>
        protected virtual void UpdateTrajectoryLine(int pointCount)
        {
            if (trajectoryLine == null) return;

            pointCount = Mathf.Min(pointCount + 1, trajectoryPoints.Length);
            trajectoryLine.positionCount = pointCount;
            
            for (int i = 0; i < pointCount; i++)
            {
                trajectoryLine.SetPosition(i, trajectoryPoints[i]);
            }
        }

        /// <summary>
        /// Özel noktaları güncelle
        /// Update special points
        /// </summary>
        /// <param name="apexIndex">Tepe noktası indeksi</param>
        /// <param name="landingIndex">İniş noktası indeksi</param>
        protected virtual void UpdateSpecialPoints(int apexIndex, int landingIndex)
        {
            if (showApexPoint && apexPoint != null && trajectoryPoints.Length > apexIndex)
            {
                apexPoint.position = trajectoryPoints[apexIndex];
            }

            if (showLandingPoint && landingPoint != null && trajectoryPoints.Length > landingIndex)
            {
                landingPoint.position = trajectoryPoints[landingIndex];
            }
        }

        /// <summary>
        /// Hız çizgilerini güncelle
        /// Update velocity lines
        /// </summary>
        /// <param name="maxIndex">Maksimum indeks</param>
        protected virtual void UpdateVelocityLines(int maxIndex)
        {
            if (velocityLines == null) return;

            int step = Mathf.Max(1, maxIndex / velocityLineCount);
            
            for (int i = 0; i < velocityLines.Length && i * step < maxIndex; i++)
            {
                if (velocityLines[i] == null) continue;

                int pointIndex = i * step;
                Vector3 position = trajectoryPoints[pointIndex];
                
                // Hız vektörünü hesapla
                float t = pointIndex * (maxTime / trajectoryResolution);
                Vector3 velocity = CalculateVelocityAtTime(t);
                Vector3 endPoint = position + velocity.normalized * 0.5f;

                velocityLines[i].positionCount = 2;
                velocityLines[i].SetPosition(0, position);
                velocityLines[i].SetPosition(1, endPoint);
            }
        }

        /// <summary>
        /// Belirli bir zamandaki hızı hesapla
        /// Calculate velocity at specific time
        /// </summary>
        /// <param name="time">Zaman</param>
        /// <returns>Hız vektörü</returns>
        protected virtual Vector3 CalculateVelocityAtTime(float time)
        {
            float angleRad = launchAngle * Mathf.Deg2Rad;
            Vector3 velocity = new Vector3(
                launchVelocity * Mathf.Cos(angleRad),
                launchVelocity * Mathf.Sin(angleRad) + gravity * time,
                0
            );

            if (useRealPhysics)
            {
                velocity += windForce / mass * time;
                
                if (airResistance > 0)
                {
                    velocity *= Mathf.Exp(-airResistance * time);
                }
            }

            return velocity;
        }

        protected override void UpdateVisuals()
        {
            if (trajectoryLine != null)
                trajectoryLine.color = indicatorColor;

            if (landingPoint != null)
            {
                LineRenderer lr = landingPoint.GetComponent<LineRenderer>();
                if (lr != null) lr.color = Color.red;
            }

            if (apexPoint != null)
            {
                LineRenderer lr = apexPoint.GetComponent<LineRenderer>();
                if (lr != null) lr.color = Color.yellow;
            }

            if (velocityLines != null)
            {
                foreach (var velLine in velocityLines)
                {
                    if (velLine != null)
                        velLine.color = Color.cyan;
                }
            }
        }

        protected override void UpdateVisibility()
        {
            if (trajectoryLine != null)
                trajectoryLine.enabled = isVisible;

            if (landingPoint != null)
            {
                LineRenderer lr = landingPoint.GetComponent<LineRenderer>();
                if (lr != null) lr.enabled = isVisible && showLandingPoint;
            }

            if (apexPoint != null)
            {
                LineRenderer lr = apexPoint.GetComponent<LineRenderer>();
                if (lr != null) lr.enabled = isVisible && showApexPoint;
            }

            if (velocityLines != null)
            {
                foreach (var velLine in velocityLines)
                {
                    if (velLine != null)
                        velLine.enabled = isVisible && showVelocityLines;
                }
            }
        }

        protected override void UpdateFillEffect()
        {
            if (!enableFillEffect) return;

            Color fillColor = indicatorColor;
            fillColor.a *= currentFillAmount;

            if (trajectoryLine != null)
            {
                trajectoryLine.color = fillColor;
                
                // Fill efekti ile yörünge uzunluğunu ayarla
                int visiblePoints = Mathf.RoundToInt(trajectoryLine.positionCount * currentFillAmount);
                if (visiblePoints < trajectoryLine.positionCount)
                {
                    trajectoryLine.positionCount = Mathf.Max(2, visiblePoints);
                }
            }
        }

        /// <summary>
        /// Fırlatma parametrelerini ayarla
        /// Set launch parameters
        /// </summary>
        /// <param name="angle">Fırlatma açısı</param>
        /// <param name="velocity">Fırlatma hızı</param>
        public void SetLaunchParameters(float angle, float velocity)
        {
            launchAngle = Mathf.Clamp(angle, 0f, 90f);
            launchVelocity = Mathf.Max(0.1f, velocity);
            CalculateTrajectory();
        }

        /// <summary>
        /// Hedef noktayı vur
        /// Aim at target point
        /// </summary>
        /// <param name="targetPoint">Hedef nokta</param>
        public void AimAtTarget(Vector3 targetPoint)
        {
            Vector3 direction = targetPoint - transform.position;
            float distance = direction.magnitude;
            
            // Optimal açıyı hesapla
            float optimalAngle = CalculateOptimalAngle(distance);
            
            // Yönü ayarla
            float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
            
            LaunchAngle = optimalAngle;
        }

        /// <summary>
        /// Optimal fırlatma açısını hesapla
        /// Calculate optimal launch angle
        /// </summary>
        /// <param name="distance">Hedef mesafe</param>
        /// <returns>Optimal açı</returns>
        protected virtual float CalculateOptimalAngle(float distance)
        {
            // Maksimum menzil için optimal açı hesapla
            float optimalAngle = 45f;
            
            // Yerçekimi ve hız göz önünde bulundur
            float g = Mathf.Abs(gravity);
            float v = launchVelocity;
            
            if (g > 0 && v > 0)
            {
                float maxRange = v * v / g;
                
                if (distance <= maxRange)
                {
                    optimalAngle = 0.5f * Mathf.Asin(distance * g / (v * v)) * Mathf.Rad2Deg;
                }
            }
            
            return Mathf.Clamp(optimalAngle, 0f, 90f);
        }

        /// <summary>
        /// İniş noktasını al
        /// Get landing point
        /// </summary>
        /// <returns>İniş noktası pozisyonu</returns>
        public Vector3 GetLandingPoint()
        {
            return landingPoint != null ? landingPoint.position : Vector3.zero;
        }

        /// <summary>
        /// İniş zamanını al
        /// Get landing time
        /// </summary>
        /// <returns>İniş zamanı</returns>
        public float GetLandingTime()
        {
            return actualLandingTime;
        }

        protected virtual void OnDestroy()
        {
            // Child GameObjects otomatik olarak yok olacak
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            // Editor'da yörüngeyi göster
            if (trajectoryPoints != null)
            {
                Gizmos.color = Color.magenta;
                for (int i = 0; i < trajectoryPoints.Length - 1; i++)
                {
                    Gizmos.DrawLine(trajectoryPoints[i], trajectoryPoints[i + 1]);
                }
            }
        }
#endif
    }
}