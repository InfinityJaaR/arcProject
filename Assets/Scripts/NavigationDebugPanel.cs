using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Herramienta de debug para el sistema de navegación AR
/// Muestra información en pantalla sobre el estado del GPS, brújula y navegación
/// </summary>
public class NavigationDebugPanel : MonoBehaviour
{
    [Header("Referencias de Texto")]
    public TextMeshProUGUI gpsStatusText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI bearingText;
    public TextMeshProUGUI destinationText;
    public TextMeshProUGUI modeText;
    
    [Header("Configuración")]
    public bool showDebugInfo = true;
    public KeyCode toggleKey = KeyCode.D;
    
    private GameObject debugPanel;
    
    void Start()
    {
        // Crear panel de debug si no existe
        if (gpsStatusText == null)
        {
            CreateDebugPanel();
        }
        
        if (debugPanel != null)
        {
            debugPanel.SetActive(showDebugInfo);
        }
    }
    
    void Update()
    {
        // Toggle con tecla D
        if (Input.GetKeyDown(toggleKey))
        {
            showDebugInfo = !showDebugInfo;
            if (debugPanel != null)
                debugPanel.SetActive(showDebugInfo);
        }
        
        if (!showDebugInfo) return;
        
        UpdateDebugInfo();
    }
    
    private void UpdateDebugInfo()
    {
        // Estado del GPS
        if (gpsStatusText != null && LocationManager.Instance != null)
        {
            string gpsStatus = LocationManager.Instance.IsGPSReady ? "? GPS Activo" : "? GPS Inactivo";
            string compassStatus = LocationManager.Instance.IsCompassReady ? "? Brújula Activa" : "? Brújula Inactiva";
            gpsStatusText.text = $"{gpsStatus}\n{compassStatus}";
        }
        
        // Ubicación actual
        if (locationText != null && LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
        {
            locationText.text = $"?? Ubicación:\n" +
                              $"Lat: {LocationManager.Instance.CurrentLatitude:F6}\n" +
                              $"Lon: {LocationManager.Instance.CurrentLongitude:F6}\n" +
                              $"Precisión: ±{LocationManager.Instance.CurrentAccuracy:F1}m";
        }
        
        // Bearing actual
        if (bearingText != null && LocationManager.Instance != null && LocationManager.Instance.IsCompassReady)
        {
            float bearing = LocationManager.Instance.CurrentBearing;
            string cardinal = GeoUtils.BearingToCardinal(bearing);
            bearingText.text = $"?? Orientación:\n{cardinal} ({bearing:F0}°)";
        }
        
        // Información de destino
        if (destinationText != null)
        {
            NavigationArrowController activeNav = FindAnyObjectByType<NavigationArrowController>();
            if (activeNav != null && activeNav.enabled)
            {
                destinationText.text = "?? Navegación Activa";
            }
            else
            {
                destinationText.text = "?? Sin navegación";
            }
        }
        
        // Modo actual
        if (modeText != null && AppModeManager.Instance != null)
        {
            string mode = AppModeManager.Instance.CurrentMode == AppModeManager.AppMode.MARKER_TRACKING 
                ? "?? TRACKING" 
                : "?? NAVIGATION";
            modeText.text = $"Modo: {mode}";
        }
    }
    
    private void CreateDebugPanel()
    {
        // Buscar canvas existente
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("[NavigationDebugPanel] No se encontró Canvas. No se puede crear panel de debug.");
            return;
        }
        
        // Crear panel de debug
        debugPanel = new GameObject("DebugPanel");
        debugPanel.transform.SetParent(canvas.transform, false);
        
        RectTransform panelRect = debugPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);
        panelRect.pivot = new Vector2(0, 1);
        panelRect.anchoredPosition = new Vector2(10, -10);
        panelRect.sizeDelta = new Vector2(300, 400);
        
        // Añadir fondo semi-transparente
        Image bg = debugPanel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);
        
        // Crear texto de estado
        CreateDebugText("GPSStatus", new Vector2(0, -10), ref gpsStatusText);
        CreateDebugText("Location", new Vector2(0, -80), ref locationText);
        CreateDebugText("Bearing", new Vector2(0, -180), ref bearingText);
        CreateDebugText("Destination", new Vector2(0, -260), ref destinationText);
        CreateDebugText("Mode", new Vector2(0, -320), ref modeText);
        
        Debug.Log("[NavigationDebugPanel] ? Panel de debug creado. Presiona 'D' para mostrar/ocultar");
    }
    
    private void CreateDebugText(string name, Vector2 position, ref TextMeshProUGUI textRef)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(debugPanel.transform, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(-20, 60);
        
        textRef = textObj.AddComponent<TextMeshProUGUI>();
        textRef.fontSize = 14;
        textRef.color = Color.white;
        textRef.alignment = TextAlignmentOptions.TopLeft;
        textRef.text = name;
    }
}
