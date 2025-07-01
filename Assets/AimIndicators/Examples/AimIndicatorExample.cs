using UnityEngine;

namespace AimIndicators.Examples
{
    /// <summary>
    /// Basic example showing how to use different aim indicators
    /// Farklı nişan göstergelerinin nasıl kullanılacağını gösteren temel örnek
    /// </summary>
    public class AimIndicatorExample : MonoBehaviour
    {
        [Header("Indicator Components")]
        [SerializeField] private ConeAimIndicator coneIndicator;
        [SerializeField] private LineAimIndicator lineIndicator;
        [SerializeField] private TargetAimIndicator targetIndicator;
        [SerializeField] private ParabolicAimIndicator parabolicIndicator;
        
        [Header("Example Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private bool useMouseControl = true;
        [SerializeField] private Camera playerCamera;
        
        [Header("Indicator Selection")]
        [SerializeField] private IndicatorType currentIndicatorType = IndicatorType.Line;
        
        public enum IndicatorType
        {
            Cone,
            Line,
            Target,
            Parabolic
        }
        
        private void Start()
        {
            // Initialize camera if not assigned
            if (playerCamera == null)
                playerCamera = Camera.main;
            
            // Setup initial indicator
            SetActiveIndicator(currentIndicatorType);
            
            // Configure indicators with default settings
            ConfigureIndicators();
        }
        
        private void Update()
        {
            HandleInput();
            UpdateIndicators();
        }
        
        private void HandleInput()
        {
            // Switch between indicators with number keys
            if (Input.GetKeyDown(KeyCode.Alpha1))
                SetActiveIndicator(IndicatorType.Cone);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                SetActiveIndicator(IndicatorType.Line);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                SetActiveIndicator(IndicatorType.Target);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                SetActiveIndicator(IndicatorType.Parabolic);
            
            // Toggle mouse control
            if (Input.GetKeyDown(KeyCode.M))
                useMouseControl = !useMouseControl;
        }
        
        private void UpdateIndicators()
        {
            if (useMouseControl && playerCamera != null)
            {
                // Use mouse position for aiming
                UpdateWithMouseControl();
            }
            else if (target != null)
            {
                // Use assigned target
                UpdateWithTarget();
            }
        }
        
        private void UpdateWithMouseControl()
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 aimDirection = (hit.point - transform.position).normalized;
                
                switch (currentIndicatorType)
                {
                    case IndicatorType.Cone:
                        if (coneIndicator != null)
                            coneIndicator.SetAimDirection(aimDirection);
                        break;
                    case IndicatorType.Line:
                        if (lineIndicator != null)
                            lineIndicator.SetAimDirection(aimDirection);
                        break;
                    case IndicatorType.Target:
                        if (targetIndicator != null)
                            targetIndicator.SetAimDirection(aimDirection);
                        break;
                    case IndicatorType.Parabolic:
                        if (parabolicIndicator != null)
                            parabolicIndicator.SetAimDirection(aimDirection);
                        break;
                }
            }
        }
        
        private void UpdateWithTarget()
        {
            switch (currentIndicatorType)
            {
                case IndicatorType.Cone:
                    if (coneIndicator != null)
                        coneIndicator.SetTarget(target);
                    break;
                case IndicatorType.Line:
                    if (lineIndicator != null)
                        lineIndicator.SetTarget(target);
                    break;
                case IndicatorType.Target:
                    if (targetIndicator != null)
                        targetIndicator.SetTarget(target);
                    break;
                case IndicatorType.Parabolic:
                    if (parabolicIndicator != null)
                        parabolicIndicator.SetTarget(target);
                    break;
            }
        }
        
        private void SetActiveIndicator(IndicatorType type)
        {
            currentIndicatorType = type;
            
            // Deactivate all indicators
            if (coneIndicator != null) coneIndicator.SetActive(false);
            if (lineIndicator != null) lineIndicator.SetActive(false);
            if (targetIndicator != null) targetIndicator.SetActive(false);
            if (parabolicIndicator != null) parabolicIndicator.SetActive(false);
            
            // Activate selected indicator
            switch (type)
            {
                case IndicatorType.Cone:
                    if (coneIndicator != null) coneIndicator.SetActive(true);
                    break;
                case IndicatorType.Line:
                    if (lineIndicator != null) lineIndicator.SetActive(true);
                    break;
                case IndicatorType.Target:
                    if (targetIndicator != null) targetIndicator.SetActive(true);
                    break;
                case IndicatorType.Parabolic:
                    if (parabolicIndicator != null) parabolicIndicator.SetActive(true);
                    break;
            }
            
            Debug.Log($"Switched to {type} indicator");
        }
        
        private void ConfigureIndicators()
        {
            // Configure Cone Indicator
            if (coneIndicator != null)
            {
                coneIndicator.SetColor(Color.red);
                coneIndicator.SetRange(10f);
                coneIndicator.SetConeAngle(45f);
                coneIndicator.fillColor = new Color(1f, 0f, 0f, 0.3f);
            }
            
            // Configure Line Indicator
            if (lineIndicator != null)
            {
                lineIndicator.SetColor(Color.blue);
                lineIndicator.SetRange(15f);
                lineIndicator.SetLineWidth(0.1f);
                lineIndicator.SetArrowHeadVisible(true);
                lineIndicator.fillColor = new Color(0f, 0f, 1f, 0.5f);
            }
            
            // Configure Target Indicator
            if (targetIndicator != null)
            {
                targetIndicator.SetColor(Color.green);
                targetIndicator.SetInnerRadius(1f);
                targetIndicator.SetOuterRadius(3f);
                targetIndicator.SetCrosshairVisible(true);
                targetIndicator.fillColor = new Color(0f, 1f, 0f, 0.2f);
            }
            
            // Configure Parabolic Indicator
            if (parabolicIndicator != null)
            {
                parabolicIndicator.SetColor(Color.yellow);
                parabolicIndicator.SetProjectileSpeed(12f);
                parabolicIndicator.SetGravity(9.81f);
                parabolicIndicator.SetLandingPointVisible(true);
                parabolicIndicator.SetTrajectoryVisible(true);
                parabolicIndicator.fillColor = new Color(1f, 1f, 0f, 0.4f);
            }
        }
        
        // Method to create indicators programmatically
        [ContextMenu("Create All Indicators")]
        public void CreateAllIndicators()
        {
            // Create Cone Indicator
            if (coneIndicator == null)
            {
                GameObject coneObj = new GameObject("ConeIndicator");
                coneObj.transform.SetParent(transform);
                coneObj.transform.localPosition = Vector3.zero;
                coneIndicator = coneObj.AddComponent<ConeAimIndicator>();
            }
            
            // Create Line Indicator
            if (lineIndicator == null)
            {
                GameObject lineObj = new GameObject("LineIndicator");
                lineObj.transform.SetParent(transform);
                lineObj.transform.localPosition = Vector3.zero;
                lineIndicator = lineObj.AddComponent<LineAimIndicator>();
            }
            
            // Create Target Indicator
            if (targetIndicator == null)
            {
                GameObject targetObj = new GameObject("TargetIndicator");
                targetObj.transform.SetParent(transform);
                targetObj.transform.localPosition = Vector3.zero;
                targetIndicator = targetObj.AddComponent<TargetAimIndicator>();
            }
            
            // Create Parabolic Indicator
            if (parabolicIndicator == null)
            {
                GameObject parabolicObj = new GameObject("ParabolicIndicator");
                parabolicObj.transform.SetParent(transform);
                parabolicObj.transform.localPosition = Vector3.zero;
                parabolicIndicator = parabolicObj.AddComponent<ParabolicAimIndicator>();
            }
            
            ConfigureIndicators();
            SetActiveIndicator(currentIndicatorType);
            
            Debug.Log("All aim indicators created successfully!");
        }
        
        private void OnGUI()
        {
            // Simple UI for demonstration
            GUI.Box(new Rect(10, 10, 200, 120), "Aim Indicator Controls");
            GUI.Label(new Rect(20, 35, 180, 20), "1-4: Switch Indicators");
            GUI.Label(new Rect(20, 55, 180, 20), "M: Toggle Mouse Control");
            GUI.Label(new Rect(20, 75, 180, 20), $"Current: {currentIndicatorType}");
            GUI.Label(new Rect(20, 95, 180, 20), $"Mouse Control: {useMouseControl}");
        }
    }
}