using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel de debugging para la brújula
/// Muestra valores en tiempo real y permite recalibrar
/// </summary>
public class CompassDebugPanel : MonoBehaviour
{
    [Header("UI Referencias")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI trueHeadingText;
    public TextMeshProUGUI magneticHeadingText;
    public TextMeshProUGUI timestampText;
    public TextMeshProUGUI rawQuaternionText;
    public Button recalibrateButton;
    
    [Header("Configuración")]
    public bool showOnStart = true;
    public KeyCode toggleKey = KeyCode.C; // Presiona 'C' para mostrar/ocultar
    
    private bool isVisible = true;
    private CanvasGroup canvasGroup;
    
    void Start()
    {
        // Configurar canvas group
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Configurar botón de recalibración
        if (recalibrateButton != null)
        {
            recalibrateButton.onClick.AddListener(OnRecalibrateClicked);
        }
        
        // Mostrar u ocultar según configuración
        SetVisible(showOnStart);
    }
    
    void Update()
    {
        // Toggle con tecla
        if (Input.GetKeyDown(toggleKey))
        {
            SetVisible(!isVisible);
        }
        
        if (!isVisible) return;
        
        // Actualizar textos
        UpdateCompassInfo();
    }
    
    void UpdateCompassInfo()
    {
        #if !UNITY_EDITOR
        
        if (!Input.compass.enabled)
        {
            if (statusText != null)
                statusText.text = "? BRÚJULA DESACTIVADA";
            return;
        }
        
        // Status
        if (statusText != null)
        {
            statusText.text = "? BRÚJULA ACTIVA";
        }
        
        // True Heading
        if (trueHeadingText != null)
        {
            float trueHeading = Input.compass.trueHeading;
            string cardinal = GetCardinalDirection(trueHeading);
            
            if (float.IsNaN(trueHeading) || trueHeading < 0)
            {
                trueHeadingText.text = $"True Heading: ? INVÁLIDO ({trueHeading})";
                trueHeadingText.color = Color.red;
            }
            else
            {
                trueHeadingText.text = $"True Heading: {trueHeading:F1}° ({cardinal})";
                trueHeadingText.color = Color.green;
            }
        }
        
        // Magnetic Heading
        if (magneticHeadingText != null)
        {
            float magneticHeading = Input.compass.magneticHeading;
            string cardinal = GetCardinalDirection(magneticHeading);
            
            if (float.IsNaN(magneticHeading) || magneticHeading < 0)
            {
                magneticHeadingText.text = $"Magnetic Heading: ? INVÁLIDO ({magneticHeading})";
                magneticHeadingText.color = Color.red;
            }
            else
            {
                magneticHeadingText.text = $"Magnetic Heading: {magneticHeading:F1}° ({cardinal})";
                magneticHeadingText.color = Color.yellow;
            }
        }
        
        // Timestamp
        if (timestampText != null)
        {
            double timestamp = Input.compass.timestamp;
            double currentTime = Time.realtimeSinceStartup;
            double age = currentTime - timestamp;
            
            timestampText.text = $"Timestamp: {timestamp:F2}\nEdad: {age:F2}s";
            
            if (age > 2.0)
            {
                timestampText.color = Color.red;
            }
            else
            {
                timestampText.color = Color.white;
            }
        }
        
        // Raw Quaternion
        if (rawQuaternionText != null)
        {
            Vector3 rawVector = Input.compass.rawVector;
            rawQuaternionText.text = $"Raw Vector:\nX: {rawVector.x:F2}\nY: {rawVector.y:F2}\nZ: {rawVector.z:F2}";
        }
        
        #else
        
        // En Editor
        if (statusText != null)
        {
            statusText.text = "?? MODO EDITOR (Simulado)";
        }
        
        if (trueHeadingText != null)
        {
            if (LocationManager.Instance != null)
            {
                float bearing = LocationManager.Instance.CurrentBearing;
                string cardinal = GetCardinalDirection(bearing);
                trueHeadingText.text = $"Bearing Simulado: {bearing:F1}° ({cardinal})";
                trueHeadingText.color = Color.cyan;
            }
        }
        
        #endif
    }
    
    void OnRecalibrateClicked()
    {
        Debug.Log("[CompassDebugPanel] ?? Recalibrando brújula...");
        
        if (LocationManager.Instance != null)
        {
            LocationManager.Instance.RecalibrateCompass();
        }
        
        #if PLATFORM_ANDROID
        // Mostrar mensaje al usuario
        ShowCalibrationMessage();
        #endif
    }
    
    void ShowCalibrationMessage()
    {
        Debug.Log("[CompassDebugPanel] ?? Mueve el dispositivo en figura de 8");
        
        // Aquí podrías mostrar un Toast o diálogo en Android
        // Por ahora solo log
    }
    
    void SetVisible(bool visible)
    {
        isVisible = visible;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
    
    string GetCardinalDirection(float bearing)
    {
        if (float.IsNaN(bearing) || bearing < 0)
            return "???";
        
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "N" };
        int index = Mathf.RoundToInt(bearing / 45f);
        return directions[index];
    }
}
