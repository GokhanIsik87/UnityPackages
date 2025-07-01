using UnityEngine;

namespace AimIndicators
{
    /// <summary>
    /// Hedef (Daire) şeklinde nişan göstergesi
    /// Target (Circle) shaped aim indicator
    /// </summary>
    [System.Serializable]
    public class TargetAimIndicator : AimIndicatorBase
    {
        [Header("Hedef Ayarları / Target Settings")]
        [SerializeField] protected float targetRadius = 1f;
        [SerializeField] protected int circleResolution = 32;
        [SerializeField] protected bool showCrosshair = true;
        [SerializeField] protected float crosshairSize = 0.8f;
        [SerializeField] protected float ringThickness = 0.1f;
        
        [Header("Çoklu Halka / Multiple Rings")]
        [SerializeField] protected bool useMultipleRings = false;
        [SerializeField] protected int ringCount = 3;
        [SerializeField] protected float ringSpacing = 0.3f;
        
        [Header("Sektör Ayarları / Sector Settings")]
        [SerializeField] protected bool useSectors = false;
        [SerializeField] protected int sectorCount = 8;
        [SerializeField] protected bool alternateSectorColors = true;

        protected LineRenderer outerRing;
        protected LineRenderer crosshairHorizontal;
        protected LineRenderer crosshairVertical;
        protected LineRenderer[] additionalRings;
        protected LineRenderer[] sectorLines;

        /// <summary>
        /// Hedef yarıçapı
        /// Target radius
        /// </summary>
        public float TargetRadius
        {
            get => targetRadius;
            set
            {
                targetRadius = Mathf.Max(0.1f, value);
                UpdateTargetGeometry();
            }
        }

        /// <summary>
        /// Halka sayısı
        /// Number of rings
        /// </summary>
        public int RingCount
        {
            get => ringCount;
            set
            {
                ringCount = Mathf.Max(1, value);
                if (useMultipleRings)
                {
                    SetupMultipleRings();
                }
            }
        }

        protected override void InitializeComponent()
        {
            base.InitializeComponent();
            SetupTargetRenderers();
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
        /// Hedef renderer'larını ayarla
        /// Setup target renderers
        /// </summary>
        protected virtual void SetupTargetRenderers()
        {
            // Ana halka
            SetupMainRing();
            
            // Artı işareti
            if (showCrosshair)
            {
                SetupCrosshair();
            }

            // Çoklu halkalar
            if (useMultipleRings)
            {
                SetupMultipleRings();
            }

            // Sektörler
            if (useSectors)
            {
                SetupSectors();
            }
        }

        /// <summary>
        /// Ana halkayı ayarla
        /// Setup main ring
        /// </summary>
        protected virtual void SetupMainRing()
        {
            GameObject ringObj = new GameObject("OuterRing");
            ringObj.transform.SetParent(transform);
            ringObj.transform.localPosition = Vector3.zero;
            
            outerRing = ringObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(outerRing);
        }

        /// <summary>
        /// Artı işaretini ayarla
        /// Setup crosshair
        /// </summary>
        protected virtual void SetupCrosshair()
        {
            // Yatay çizgi
            GameObject horizontalObj = new GameObject("CrosshairHorizontal");
            horizontalObj.transform.SetParent(transform);
            horizontalObj.transform.localPosition = Vector3.zero;
            
            crosshairHorizontal = horizontalObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(crosshairHorizontal);

            // Dikey çizgi
            GameObject verticalObj = new GameObject("CrosshairVertical");
            verticalObj.transform.SetParent(transform);
            verticalObj.transform.localPosition = Vector3.zero;
            
            crosshairVertical = verticalObj.AddComponent<LineRenderer>();
            ConfigureLineRenderer(crosshairVertical);
        }

        /// <summary>
        /// Çoklu halkaları ayarla
        /// Setup multiple rings
        /// </summary>
        protected virtual void SetupMultipleRings()
        {
            // Önceki halkaları temizle
            if (additionalRings != null)
            {
                foreach (var ring in additionalRings)
                {
                    if (ring != null && ring.gameObject != null)
                    {
                        DestroyImmediate(ring.gameObject);
                    }
                }
            }

            if (ringCount <= 1) return;

            additionalRings = new LineRenderer[ringCount - 1];
            
            for (int i = 0; i < additionalRings.Length; i++)
            {
                GameObject ringObj = new GameObject($"Ring_{i + 1}");
                ringObj.transform.SetParent(transform);
                ringObj.transform.localPosition = Vector3.zero;
                
                LineRenderer ring = ringObj.AddComponent<LineRenderer>();
                ConfigureLineRenderer(ring);
                
                additionalRings[i] = ring;
            }
        }

        /// <summary>
        /// Sektörleri ayarla
        /// Setup sectors
        /// </summary>
        protected virtual void SetupSectors()
        {
            // Önceki sektörleri temizle
            if (sectorLines != null)
            {
                foreach (var sector in sectorLines)
                {
                    if (sector != null && sector.gameObject != null)
                    {
                        DestroyImmediate(sector.gameObject);
                    }
                }
            }

            sectorLines = new LineRenderer[sectorCount];
            
            for (int i = 0; i < sectorCount; i++)
            {
                GameObject sectorObj = new GameObject($"Sector_{i}");
                sectorObj.transform.SetParent(transform);
                sectorObj.transform.localPosition = Vector3.zero;
                
                LineRenderer sector = sectorObj.AddComponent<LineRenderer>();
                ConfigureLineRenderer(sector);
                
                sectorLines[i] = sector;
            }
        }

        /// <summary>
        /// LineRenderer'ı yapılandır
        /// Configure LineRenderer
        /// </summary>
        /// <param name="lr">Yapılandırılacak LineRenderer</param>
        protected virtual void ConfigureLineRenderer(LineRenderer lr)
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = ringThickness;
            lr.endWidth = ringThickness;
            lr.useWorldSpace = false;
            lr.sortingOrder = 1;
            lr.loop = false;
        }

        protected override void CreateIndicatorSprite()
        {
            UpdateTargetGeometry();
        }

        /// <summary>
        /// Hedef geometrisini güncelle
        /// Update target geometry
        /// </summary>
        protected virtual void UpdateTargetGeometry()
        {
            UpdateMainRing();
            UpdateCrosshair();
            UpdateAdditionalRings();
            UpdateSectors();
        }

        /// <summary>
        /// Ana halkayı güncelle
        /// Update main ring
        /// </summary>
        protected virtual void UpdateMainRing()
        {
            if (outerRing == null) return;

            Vector3[] positions = new Vector3[circleResolution + 1];
            
            for (int i = 0; i <= circleResolution; i++)
            {
                float angle = (float)i / circleResolution * 2f * Mathf.PI;
                float x = Mathf.Cos(angle) * targetRadius;
                float y = Mathf.Sin(angle) * targetRadius;
                positions[i] = new Vector3(x, y, 0);
            }

            outerRing.positionCount = positions.Length;
            outerRing.SetPositions(positions);
        }

        /// <summary>
        /// Artı işaretini güncelle
        /// Update crosshair
        /// </summary>
        protected virtual void UpdateCrosshair()
        {
            if (!showCrosshair) return;

            float crosshairLength = targetRadius * crosshairSize;
            
            // Yatay çizgi
            if (crosshairHorizontal != null)
            {
                crosshairHorizontal.positionCount = 2;
                crosshairHorizontal.SetPosition(0, new Vector3(-crosshairLength, 0, 0));
                crosshairHorizontal.SetPosition(1, new Vector3(crosshairLength, 0, 0));
            }

            // Dikey çizgi
            if (crosshairVertical != null)
            {
                crosshairVertical.positionCount = 2;
                crosshairVertical.SetPosition(0, new Vector3(0, -crosshairLength, 0));
                crosshairVertical.SetPosition(1, new Vector3(0, crosshairLength, 0));
            }
        }

        /// <summary>
        /// Ek halkaları güncelle
        /// Update additional rings
        /// </summary>
        protected virtual void UpdateAdditionalRings()
        {
            if (!useMultipleRings || additionalRings == null) return;

            for (int ringIndex = 0; ringIndex < additionalRings.Length; ringIndex++)
            {
                if (additionalRings[ringIndex] == null) continue;

                float ringRadius = targetRadius - (ringIndex + 1) * ringSpacing;
                if (ringRadius <= 0) continue;

                Vector3[] positions = new Vector3[circleResolution + 1];
                
                for (int i = 0; i <= circleResolution; i++)
                {
                    float angle = (float)i / circleResolution * 2f * Mathf.PI;
                    float x = Mathf.Cos(angle) * ringRadius;
                    float y = Mathf.Sin(angle) * ringRadius;
                    positions[i] = new Vector3(x, y, 0);
                }

                additionalRings[ringIndex].positionCount = positions.Length;
                additionalRings[ringIndex].SetPositions(positions);
            }
        }

        /// <summary>
        /// Sektörleri güncelle
        /// Update sectors
        /// </summary>
        protected virtual void UpdateSectors()
        {
            if (!useSectors || sectorLines == null) return;

            for (int i = 0; i < sectorCount; i++)
            {
                if (sectorLines[i] == null) continue;

                float angle = (float)i / sectorCount * 2f * Mathf.PI;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                Vector3 endPoint = direction * targetRadius;

                sectorLines[i].positionCount = 2;
                sectorLines[i].SetPosition(0, Vector3.zero);
                sectorLines[i].SetPosition(1, endPoint);
            }
        }

        protected override void UpdateVisuals()
        {
            Color baseColor = indicatorColor;
            
            if (outerRing != null)
                outerRing.color = baseColor;

            if (crosshairHorizontal != null)
                crosshairHorizontal.color = baseColor;

            if (crosshairVertical != null)
                crosshairVertical.color = baseColor;

            if (additionalRings != null)
            {
                foreach (var ring in additionalRings)
                {
                    if (ring != null)
                        ring.color = baseColor;
                }
            }

            if (sectorLines != null)
            {
                for (int i = 0; i < sectorLines.Length; i++)
                {
                    if (sectorLines[i] != null)
                    {
                        Color sectorColor = baseColor;
                        if (alternateSectorColors && i % 2 == 1)
                        {
                            sectorColor = Color.Lerp(baseColor, Color.white, 0.3f);
                        }
                        sectorLines[i].color = sectorColor;
                    }
                }
            }
        }

        protected override void UpdateVisibility()
        {
            if (outerRing != null)
                outerRing.enabled = isVisible;

            if (crosshairHorizontal != null)
                crosshairHorizontal.enabled = isVisible && showCrosshair;

            if (crosshairVertical != null)
                crosshairVertical.enabled = isVisible && showCrosshair;

            if (additionalRings != null)
            {
                foreach (var ring in additionalRings)
                {
                    if (ring != null)
                        ring.enabled = isVisible && useMultipleRings;
                }
            }

            if (sectorLines != null)
            {
                foreach (var sector in sectorLines)
                {
                    if (sector != null)
                        sector.enabled = isVisible && useSectors;
                }
            }
        }

        protected override void UpdateFillEffect()
        {
            if (!enableFillEffect) return;

            Color fillColor = indicatorColor;
            fillColor.a *= currentFillAmount;

            if (outerRing != null)
                outerRing.color = fillColor;

            if (crosshairHorizontal != null)
                crosshairHorizontal.color = fillColor;

            if (crosshairVertical != null)
                crosshairVertical.color = fillColor;

            if (additionalRings != null)
            {
                foreach (var ring in additionalRings)
                {
                    if (ring != null)
                        ring.color = fillColor;
                }
            }

            if (sectorLines != null)
            {
                for (int i = 0; i < sectorLines.Length; i++)
                {
                    if (sectorLines[i] != null)
                    {
                        Color sectorFillColor = fillColor;
                        if (alternateSectorColors && i % 2 == 1)
                        {
                            sectorFillColor = Color.Lerp(fillColor, Color.white, 0.3f);
                        }
                        sectorLines[i].color = sectorFillColor;
                    }
                }
            }
        }

        /// <summary>
        /// Hedef yarıçapını ayarla
        /// Set target radius
        /// </summary>
        /// <param name="radius">Yarıçap / Radius</param>
        public void SetTargetRadius(float radius)
        {
            TargetRadius = radius;
        }

        /// <summary>
        /// Artı işaretini açık/kapalı yap
        /// Toggle crosshair visibility
        /// </summary>
        /// <param name="show">Göster/Gizle</param>
        public void ShowCrosshair(bool show)
        {
            showCrosshair = show;
            UpdateVisibility();
        }

        /// <summary>
        /// Sektör modunu açık/kapalı yap
        /// Toggle sector mode
        /// </summary>
        /// <param name="enable">Etkinleştir/Devre dışı bırak</param>
        public void EnableSectors(bool enable)
        {
            useSectors = enable;
            if (enable && sectorLines == null)
            {
                SetupSectors();
                UpdateSectors();
            }
            UpdateVisibility();
        }

        /// <summary>
        /// Çoklu halka modunu açık/kapalı yap
        /// Toggle multiple rings mode
        /// </summary>
        /// <param name="enable">Etkinleştir/Devre dışı bırak</param>
        public void EnableMultipleRings(bool enable)
        {
            useMultipleRings = enable;
            if (enable && additionalRings == null)
            {
                SetupMultipleRings();
                UpdateAdditionalRings();
            }
            UpdateVisibility();
        }

        protected virtual void OnDestroy()
        {
            // Child GameObjects otomatik olarak yok olacak
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected()
        {
            // Editor'da hedef alanını göster
            Gizmos.color = Color.green;
            Gizmos.DrawWireCircle(transform.position, targetRadius);
            
            if (useMultipleRings)
            {
                Gizmos.color = Color.yellow;
                for (int i = 1; i < ringCount; i++)
                {
                    float ringRadius = targetRadius - i * ringSpacing;
                    if (ringRadius > 0)
                    {
                        Gizmos.DrawWireCircle(transform.position, ringRadius);
                    }
                }
            }
        }
#endif
    }
}