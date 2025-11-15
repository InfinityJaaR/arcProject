using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

/// <summary>
/// Herramienta de diagnóstico para verificar disponibilidad del magnetómetro
/// en dispositivos Android
/// </summary>
public class MagnetometerDiagnostic : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Mostrar panel de diagnóstico en pantalla")]
    public bool showOnScreenDebug = true;
    
    [Tooltip("Posición del panel de debug")]
    public Vector2 debugPanelPosition = new Vector2(10, 10);
    
    [Tooltip("Tamaño del panel de debug")]
    public Vector2 debugPanelSize = new Vector2(500, 400);
    
    private bool magnetometerAvailable = false;
    private bool compassInitialized = false;
    private string diagnosticInfo = "";
    
    void Start()
    {
        RunDiagnostic();
    }
    
    /// <summary>
    /// Ejecuta diagnóstico completo del magnetómetro
    /// </summary>
    public void RunDiagnostic()
    {
        Debug.Log("????????????????????????????????????");
        Debug.Log("[MagnetometerDiagnostic] ?? INICIANDO DIAGNÓSTICO");
        Debug.Log("????????????????????????????????????");
        
        diagnosticInfo = "";
        
        // 1. Verificar plataforma
        diagnosticInfo += "?? PLATAFORMA\n";
        diagnosticInfo += $"  Sistema: {SystemInfo.operatingSystem}\n";
        diagnosticInfo += $"  Dispositivo: {SystemInfo.deviceModel}\n";
        diagnosticInfo += $"  Unity: {Application.unityVersion}\n\n";
        
        Debug.Log($"[MagnetometerDiagnostic] Plataforma: {SystemInfo.operatingSystem}");
        Debug.Log($"[MagnetometerDiagnostic] Dispositivo: {SystemInfo.deviceModel}");
        
        // 2. Verificar magnetómetro (Android)
        diagnosticInfo += "?? MAGNETÓMETRO\n";
        
        #if UNITY_ANDROID && !UNITY_EDITOR
        magnetometerAvailable = CheckMagnetometerAvailable();
        diagnosticInfo += $"  Disponible: {(magnetometerAvailable ? "? SÍ" : "? NO")}\n";
        
        if (!magnetometerAvailable)
        {
            diagnosticInfo += "  ?? Este dispositivo NO tiene sensor de brújula\n";
            diagnosticInfo += "  ?? Usa GPS bearing como alternativa\n";
        }
        #else
        diagnosticInfo += "  ?? Modo Editor - no se puede verificar\n";
        #endif
        
        diagnosticInfo += "\n";
        
        // 3. Verificar Input.compass
        diagnosticInfo += "?? INPUT.COMPASS\n";
        diagnosticInfo += $"  Enabled: {Input.compass.enabled}\n";
        
        if (!Input.compass.enabled)
        {
            Input.compass.enabled = true;
            diagnosticInfo += "  ?? Habilitando brújula...\n";
        }
        
        diagnosticInfo += $"  Timestamp: {Input.compass.timestamp}\n";
        
        if (Input.compass.timestamp < 0)
        {
            diagnosticInfo += "  ?? Sin datos del sensor\n";
            compassInitialized = false;
        }
        else
        {
            diagnosticInfo += "  ? Sensor proporcionando datos\n";
            compassInitialized = true;
        }
        
        diagnosticInfo += $"  TrueHeading: {Input.compass.trueHeading:F2}°\n";
        diagnosticInfo += $"  MagneticHeading: {Input.compass.magneticHeading:F2}°\n";
        diagnosticInfo += $"  Accuracy: {Input.compass.headingAccuracy:F2}°\n";
        diagnosticInfo += $"  RawVector: {Input.compass.rawVector}\n\n";
        
        Debug.Log($"[MagnetometerDiagnostic] Compass enabled: {Input.compass.enabled}");
        Debug.Log($"[MagnetometerDiagnostic] Timestamp: {Input.compass.timestamp}");
        Debug.Log($"[MagnetometerDiagnostic] TrueHeading: {Input.compass.trueHeading}°");
        
        // 4. Verificar GPS
        diagnosticInfo += "?? GPS\n";
        diagnosticInfo += $"  Enabled by User: {Input.location.isEnabledByUser}\n";
        diagnosticInfo += $"  Status: {Input.location.status}\n";
        
        if (Input.location.status == LocationServiceStatus.Running)
        {
            diagnosticInfo += "  ? GPS funcionando\n";
            diagnosticInfo += $"  Lat: {Input.location.lastData.latitude:F6}\n";
            diagnosticInfo += $"  Lon: {Input.location.lastData.longitude:F6}\n";
            diagnosticInfo += $"  Accuracy: {Input.location.lastData.horizontalAccuracy:F1}m\n";
        }
        else
        {
            diagnosticInfo += "  ?? GPS no está funcionando\n";
        }
        
        diagnosticInfo += "\n";
        
        // 5. Verificar otros sensores
        diagnosticInfo += "?? OTROS SENSORES\n";
        diagnosticInfo += $"  Gyroscope: {(SystemInfo.supportsGyroscope ? "?" : "?")}\n";
        diagnosticInfo += $"  Accelerometer: {(SystemInfo.supportsAccelerometer ? "?" : "?")}\n";
        diagnosticInfo += $"  Vibration: {(SystemInfo.supportsVibration ? "?" : "?")}\n\n";
        
        // 6. Verificar permisos (Android)
        #if UNITY_ANDROID && !UNITY_EDITOR
        diagnosticInfo += "?? PERMISOS\n";
        
        bool fineLocation = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        bool coarseLocation = Permission.HasUserAuthorizedPermission(Permission.CoarseLocation);
        
        diagnosticInfo += $"  Fine Location: {(fineLocation ? "?" : "?")}\n";
        diagnosticInfo += $"  Coarse Location: {(coarseLocation ? "?" : "?")}\n\n";
        
        Debug.Log($"[MagnetometerDiagnostic] Fine Location: {fineLocation}");
        Debug.Log($"[MagnetometerDiagnostic] Coarse Location: {coarseLocation}");
        #endif
        
        // 7. Recomendaciones
        diagnosticInfo += "?? RECOMENDACIONES\n";
        
        if (!compassInitialized)
        {
            diagnosticInfo += "  ?? Brújula no inicializada:\n";
            diagnosticInfo += "    1. Aleja el teléfono de metal\n";
            diagnosticInfo += "    2. Mueve en figura de 8\n";
            diagnosticInfo += "    3. Sal al exterior si es posible\n";
        }
        else
        {
            diagnosticInfo += "  ? Brújula funcionando correctamente\n";
        }
        
        if (!magnetometerAvailable)
        {
            diagnosticInfo += "  ?? Sin magnetómetro:\n";
            diagnosticInfo += "    - Usa GPS bearing\n";
            diagnosticInfo += "    - El usuario debe moverse\n";
        }
        
        Debug.Log(diagnosticInfo);
        Debug.Log("????????????????????????????????????");
    }
    
    /// <summary>
    /// Verifica si el dispositivo Android tiene magnetómetro
    /// </summary>
    private bool CheckMagnetometerAvailable()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject sensorManager = activity.Call<AndroidJavaObject>("getSystemService", "sensor");
            
            // TYPE_MAGNETIC_FIELD = 2
            AndroidJavaObject magnetometer = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", 2);
            
            bool available = magnetometer != null;
            
            if (available)
            {
                string sensorName = magnetometer.Call<string>("getName");
                string vendor = magnetometer.Call<string>("getVendor");
                float maxRange = magnetometer.Call<float>("getMaximumRange");
                
                Debug.Log($"[MagnetometerDiagnostic] ? Magnetómetro encontrado:");
                Debug.Log($"  Nombre: {sensorName}");
                Debug.Log($"  Fabricante: {vendor}");
                Debug.Log($"  Rango máximo: {maxRange} µT");
            }
            else
            {
                Debug.LogWarning("[MagnetometerDiagnostic] ? NO se encontró magnetómetro en el dispositivo");
            }
            
            return available;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[MagnetometerDiagnostic] Error verificando magnetómetro: {e.Message}");
            return false;
        }
        #else
        return true; // Asumir disponible en otras plataformas
        #endif
    }
    
    /// <summary>
    /// Dibuja panel de debug en pantalla
    /// </summary>
    void OnGUI()
    {
        if (!showOnScreenDebug) return;
        
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 16;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.yellow;
        
        GUILayout.BeginArea(new Rect(debugPanelPosition.x, debugPanelPosition.y, 
                                     debugPanelSize.x, debugPanelSize.y));
        
        GUILayout.Label("?? DIAGNÓSTICO MAGNETÓMETRO", titleStyle);
        
        GUILayout.BeginVertical(style);
        GUILayout.Label(diagnosticInfo);
        GUILayout.EndVertical();
        
        if (GUILayout.Button("?? Actualizar Diagnóstico", GUILayout.Height(40)))
        {
            RunDiagnostic();
        }
        
        if (!compassInitialized && GUILayout.Button("?? Reintentar Brújula", GUILayout.Height(40)))
        {
            Input.compass.enabled = false;
            Invoke(nameof(ReenableCompass), 0.5f);
        }
        
        GUILayout.EndArea();
    }
    
    private void ReenableCompass()
    {
        Input.compass.enabled = true;
        Invoke(nameof(RunDiagnostic), 1f);
    }
    
    void Update()
    {
        // Actualizar diagnóstico periódicamente
        if (Time.frameCount % 300 == 0) // Cada 5 segundos
        {
            RunDiagnostic();
        }
    }
}
