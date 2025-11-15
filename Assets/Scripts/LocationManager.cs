using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Gestor del GPS y brújula del dispositivo
/// Proporciona la ubicación actual y orientación del usuario
/// </summary>
public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; private set; }
    
    [Header("Configuración")]
    [Tooltip("Precisión deseada en metros (menor = más batería)")]
    public float desiredAccuracyInMeters = 10f;
    
    [Tooltip("Distancia mínima en metros para actualizar ubicación")]
    public float updateDistanceInMeters = 5f;
    
    [Tooltip("Tiempo máximo de espera para inicializar GPS (segundos)")]
    public int maxWaitTime = 20;
    
    [Header("Configuración de Brújula")]
    [Tooltip("Tiempo de espera para inicializar brújula (segundos)")]
    public float compassInitTimeout = 10f;
    
    [Tooltip("Usar GPS bearing como fallback si brújula no funciona")]
    public bool useGPSBearingFallback = true;
    
    [Tooltip("Velocidad mínima (m/s) para usar GPS bearing")]
    public float minSpeedForGPSBearing = 0.5f;
    
    [Header("Simulación (Solo Editor)")]
    [Tooltip("Simular ubicación en Unity Editor para testing")]
    public bool simulateInEditor = true;
    
    [Tooltip("Latitud simulada (ej: Universidad)")]
    public double simulatedLatitude = 13.7181033;
    
    [Tooltip("Longitud simulada")]
    public double simulatedLongitude = -89.2040915;
    
    [Tooltip("Bearing simulado (0-360, donde 0 = Norte)")]
    public float simulatedBearing = 0f;
    
    [Header("Estado")]
    [SerializeField] private bool isGPSEnabled = false;
    [SerializeField] private bool isCompassEnabled = false;
    [SerializeField] private bool compassHasValidData = false;
    [SerializeField] private LocationServiceStatus locationStatus;
    
    // Ubicación actual del usuario
    private double currentLatitude;
    private double currentLongitude;
    private float currentAccuracy;
    private float currentBearing; // Bearing del dispositivo (orientación)
    private float currentSpeed; // Velocidad de movimiento (m/s)
    private float lastGPSBearing = 0f; // Último bearing obtenido del GPS
    
    // Eventos
    public event Action<double, double> OnLocationUpdated;
    public event Action<float> OnBearingUpdated;
    public event Action<string> OnLocationError;
    
    // Propiedades públicas
    public bool IsGPSReady => isGPSEnabled && locationStatus == LocationServiceStatus.Running;
    public bool IsCompassReady => compassHasValidData;
    public double CurrentLatitude => currentLatitude;
    public double CurrentLongitude => currentLongitude;
    public float CurrentAccuracy => currentAccuracy;
    public float CurrentBearing => currentBearing;
    public float CurrentSpeed => currentSpeed;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start()
    {
        StartCoroutine(InitializeLocation());
    }
    
    /// <summary>
    /// Inicializa los servicios de ubicación y brújula
    /// </summary>
    private IEnumerator InitializeLocation()
    {
        Debug.Log("[LocationManager] ?? Inicializando servicios de ubicación...");
        
        #if UNITY_EDITOR
        if (simulateInEditor)
        {
            Debug.Log("[LocationManager] ?? Modo simulación activado (Editor)");
            currentLatitude = simulatedLatitude;
            currentLongitude = simulatedLongitude;
            currentBearing = simulatedBearing;
            currentAccuracy = 5f;
            currentSpeed = 0f;
            isGPSEnabled = true;
            isCompassEnabled = true;
            compassHasValidData = true;
            locationStatus = LocationServiceStatus.Running;
            
            // Inicializar brújula simulada
            Input.compass.enabled = true;
            
            Debug.Log($"[LocationManager] ? Simulación iniciada: {currentLatitude:F6}, {currentLongitude:F6}");
            yield break;
        }
        #endif
        
        // EN ANDROID: Verificar permisos primero
        Debug.Log("[LocationManager] ?? Ejecutando en dispositivo Android...");
        
        // Verificar si el usuario tiene GPS en el dispositivo
        if (!Input.location.isEnabledByUser)
        {
            string error = "GPS no está habilitado por el usuario. Actívalo en Configuración.";
            Debug.LogError($"[LocationManager] ? {error}");
            OnLocationError?.Invoke(error);
            yield break;
        }
        
        Debug.Log("[LocationManager] ? GPS está habilitado por el usuario");
        
        // Iniciar servicio de ubicación
        Debug.Log($"[LocationManager] ?? Iniciando servicio GPS (precisión: {desiredAccuracyInMeters}m, distancia: {updateDistanceInMeters}m)...");
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        
        // Esperar hasta que el servicio esté inicializado
        int waitCounter = 0;
        while (Input.location.status == LocationServiceStatus.Initializing && waitCounter < maxWaitTime)
        {
            yield return new WaitForSeconds(1);
            waitCounter++;
            Debug.Log($"[LocationManager] ? Inicializando GPS... {waitCounter}/{maxWaitTime} (status: {Input.location.status})");
        }
        
        // Verificar timeout
        if (waitCounter >= maxWaitTime)
        {
            string error = "Timeout al inicializar GPS. Verifica tu conexión.";
            Debug.LogError($"[LocationManager] ?? {error}");
            Debug.LogError($"[LocationManager] ?? Status final: {Input.location.status}");
            OnLocationError?.Invoke(error);
            
            // INTENTAR USAR DE TODOS MODOS
            Debug.LogWarning("[LocationManager] ?? Intentando usar GPS de todos modos...");
            isGPSEnabled = true; // Forzar enabled
            locationStatus = Input.location.status;
            yield break;
        }
        
        // Verificar si falló la inicialización
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            string error = "No se pudo inicializar el GPS. Verifica los permisos.";
            Debug.LogError($"[LocationManager] ? {error}");
            OnLocationError?.Invoke(error);
            yield break;
        }
        
        // GPS inicializado correctamente
        isGPSEnabled = true;
        locationStatus = Input.location.status;
        
        // Obtener ubicación inicial
        UpdateLocation();
        
        Debug.Log($"[LocationManager] ? GPS inicializado correctamente");
        Debug.Log($"[LocationManager] ?? Ubicación inicial: {currentLatitude:F6}, {currentLongitude:F6}");
        Debug.Log($"[LocationManager] ?? Precisión: {currentAccuracy:F1}m");
        
        // Inicializar brújula CON VERIFICACIÓN
        yield return StartCoroutine(InitializeCompass());
    }
    
    /// <summary>
    /// Inicializa la brújula con verificación de disponibilidad
    /// </summary>
    private IEnumerator InitializeCompass()
    {
        Debug.Log("[LocationManager] ?? Inicializando brújula...");
        
        // Habilitar brújula
        Input.compass.enabled = true;
        isCompassEnabled = true;
        
        // Esperar a que la brújula proporcione datos válidos
        float elapsed = 0f;
        
        while (elapsed < compassInitTimeout)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
            
            // Verificar si tenemos datos válidos
            if (Input.compass.timestamp > 0)
            {
                compassHasValidData = true;
                Debug.Log($"[LocationManager] ? Brújula inicializada correctamente");
                Debug.Log($"[LocationManager] ?? TrueHeading: {Input.compass.trueHeading:F1}°");
                Debug.Log($"[LocationManager] ?? MagneticHeading: {Input.compass.magneticHeading:F1}°");
                Debug.Log($"[LocationManager] ?? Accuracy: {Input.compass.headingAccuracy:F1}°");
                
                currentBearing = GetValidBearing();
                yield break;
            }
            
            Debug.Log($"[LocationManager] ? Esperando brújula... {elapsed:F1}s (timestamp: {Input.compass.timestamp})");
        }
        
        // Timeout - brújula no proporcionó datos válidos
        Debug.LogWarning($"[LocationManager] ?? Brújula no proporcionó datos después de {compassInitTimeout}s");
        Debug.LogWarning("[LocationManager] ?? Estado de la brújula:");
        Debug.LogWarning($"   - Enabled: {Input.compass.enabled}");
        Debug.LogWarning($"   - Timestamp: {Input.compass.timestamp}");
        Debug.LogWarning($"   - TrueHeading: {Input.compass.trueHeading}°");
        Debug.LogWarning($"   - MagneticHeading: {Input.compass.magneticHeading}°");
        Debug.LogWarning($"   - RawVector: {Input.compass.rawVector}");
        
        compassHasValidData = false;
        
        if (useGPSBearingFallback)
        {
            Debug.Log("[LocationManager] ?? Usando GPS bearing como alternativa");
            Debug.Log("[LocationManager] ?? IMPORTANTE: Necesitas MOVERTE para que funcione");
            Debug.Log("[LocationManager] ?? Calibra la brújula moviendo el teléfono en forma de 8");
        }
    }
    
    /// <summary>
    /// Obtiene un bearing válido de la brújula o GPS
    /// </summary>
    private float GetValidBearing()
    {
        // 1. Intentar usar brújula si tiene datos válidos
        if (compassHasValidData && Input.compass.timestamp > 0)
        {
            float trueHeading = Input.compass.trueHeading;
            
            // Si trueHeading es válido, usarlo
            if (!float.IsNaN(trueHeading) && trueHeading >= 0)
            {
                return trueHeading;
            }
            
            // Fallback a magneticHeading
            float magneticHeading = Input.compass.magneticHeading;
            if (!float.IsNaN(magneticHeading) && magneticHeading >= 0)
            {
                Debug.LogWarning("[LocationManager] ?? trueHeading inválido, usando magneticHeading");
                return magneticHeading;
            }
        }
        
        // 2. Fallback a GPS bearing si está en movimiento
        // NOTA: LocationInfo no siempre tiene 'course' disponible en Unity
        // Por ahora, confiar en la brújula o último valor conocido
        if (useGPSBearingFallback && currentSpeed >= minSpeedForGPSBearing)
        {
            if (Time.frameCount % 120 == 0)
            {
                Debug.LogWarning($"[LocationManager] ?? GPS bearing no disponible en esta versión de Unity (velocidad: {currentSpeed:F1} m/s)");
            }
        }
        
        // 3. Usar último bearing conocido
        if (Time.frameCount % 300 == 0)
        {
            Debug.LogWarning("[LocationManager] ?? Sin bearing válido - usando último conocido");
        }
        
        return currentBearing; // Mantener el último valor válido
    }
    
    void Update()
    {
        if (!IsGPSReady) return;
        
        #if UNITY_EDITOR
        if (simulateInEditor)
        {
            // En modo simulación, permitir cambiar el bearing con las teclas
            if (Input.GetKey(KeyCode.LeftArrow))
                simulatedBearing = (simulatedBearing - 90f * Time.deltaTime + 360f) % 360f;
            if (Input.GetKey(KeyCode.RightArrow))
                simulatedBearing = (simulatedBearing + 90f * Time.deltaTime) % 360f;
            
            currentBearing = simulatedBearing;
            OnBearingUpdated?.Invoke(currentBearing);
            return;
        }
        #endif
        
        // Actualizar ubicación GPS
        UpdateLocation();
        
        // Actualizar bearing
        float newBearing = GetValidBearing();
        
        if (newBearing != currentBearing)
        {
            currentBearing = newBearing;
            OnBearingUpdated?.Invoke(currentBearing);
        }
        
        // DEBUG: Mostrar estado periódicamente
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"[LocationManager] ?? Estado Brújula:");
            Debug.Log($"   - Compass Valid: {compassHasValidData}");
            Debug.Log($"   - Timestamp: {Input.compass.timestamp}");
            Debug.Log($"   - TrueHeading: {Input.compass.trueHeading:F1}°");
            Debug.Log($"   - MagneticHeading: {Input.compass.magneticHeading:F1}°");
            Debug.Log($"   - Current Bearing: {currentBearing:F1}°");
            Debug.Log($"   - Speed: {currentSpeed:F2} m/s");
            
            // Verificar si cambió el estado de timestamp
            if (Input.compass.timestamp > 0 && !compassHasValidData)
            {
                Debug.Log("[LocationManager] ? ¡Brújula ahora proporciona datos!");
                compassHasValidData = true;
            }
        }
    }
    
    /// <summary>
    /// Actualiza la ubicación actual desde el GPS
    /// </summary>
    private void UpdateLocation()
    {
        #if !UNITY_EDITOR
        if (Input.location.status != LocationServiceStatus.Running)
        {
            locationStatus = Input.location.status;
            return;
        }
        
        LocationInfo locationInfo = Input.location.lastData;
        
        // Verificar si la ubicación cambió significativamente
        bool hasChanged = currentLatitude != locationInfo.latitude || 
                         currentLongitude != locationInfo.longitude;
        
        if (hasChanged)
        {
            currentLatitude = locationInfo.latitude;
            currentLongitude = locationInfo.longitude;
            currentAccuracy = locationInfo.horizontalAccuracy;
            
            OnLocationUpdated?.Invoke(currentLatitude, currentLongitude);
            
            // Log solo cuando cambia significativamente
            if (currentAccuracy < 20f)
            {
                Debug.Log($"[LocationManager] ?? Ubicación actualizada: {currentLatitude:F6}, {currentLongitude:F6} (±{currentAccuracy:F1}m)");
            }
        }
        
        // NOTA: LocationInfo.speed no está disponible en todas las versiones de Unity
        // Calculamos velocidad manualmente si es necesario, o dejamos en 0
        // currentSpeed = 0f; // Por defecto, sin movimiento
        #endif
    }
    
    /// <summary>
    /// Calcula la dirección (bearing) hacia un destino específico
    /// </summary>
    /// <param name="destinationLat">Latitud del destino</param>
    /// <param name="destinationLon">Longitud del destino</param>
    /// <returns>Bearing en grados [0, 360) donde 0 = Norte</returns>
    public float GetBearingToDestination(double destinationLat, double destinationLon)
    {
        return GeoUtils.CalculateBearing(currentLatitude, currentLongitude, destinationLat, destinationLon);
    }
    
    /// <summary>
    /// Calcula la distancia en metros hacia un destino específico
    /// </summary>
    public float GetDistanceToDestination(double destinationLat, double destinationLon)
    {
        return GeoUtils.CalculateDistance(currentLatitude, currentLongitude, destinationLat, destinationLon);
    }
    
    /// <summary>
    /// Calcula el ángulo relativo entre la orientación actual del dispositivo y el destino
    /// Este es el ángulo que necesita la flecha para apuntar correctamente
    /// </summary>
    /// <param name="destinationLat">Latitud del destino</param>
    /// <param name="destinationLon">Longitud del destino</param>
    /// <returns>Ángulo en grados [-180, 180] donde 0 = adelante, 90 = derecha, -90 = izquierda</returns>
    public float GetRelativeAngleToDestination(double destinationLat, double destinationLon)
    {
        float bearingToDestination = GetBearingToDestination(destinationLat, destinationLon);
        float relativeAngle = bearingToDestination - currentBearing;
        return GeoUtils.NormalizeAngle(relativeAngle);
    }
    
    void OnDestroy()
    {
        #if !UNITY_EDITOR
        if (Input.location.isEnabledByUser)
        {
            Input.location.Stop();
        }
        
        if (Input.compass.enabled)
        {
            Input.compass.enabled = false;
        }
        #endif
        
        Debug.Log("[LocationManager] ?? Servicios de ubicación detenidos");
    }
    
    /// <summary>
    /// Reinicializa la ubicación después de obtener permisos
    /// Llamado por PermissionsManager
    /// </summary>
    public IEnumerator ReinitializeLocation()
    {
        Debug.Log("[LocationManager] ?? Reinicializando GPS después de obtener permisos...");
        
        // Detener servicio actual si está corriendo
        #if !UNITY_EDITOR
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
            yield return new WaitForSeconds(0.5f);
        }
        
        // Detener y reiniciar brújula
        if (Input.compass.enabled)
        {
            Input.compass.enabled = false;
            yield return new WaitForSeconds(0.5f);
        }
        #endif
        
        // Reiniciar
        yield return StartCoroutine(InitializeLocation());
    }
    
    /// <summary>
    /// Fuerza la recalibración de la brújula
    /// </summary>
    public void RecalibrateCompass()
    {
        Debug.Log("[LocationManager] ?? Recalibrando brújula...");
        
        #if !UNITY_EDITOR
        // Reiniciar brújula
        if (Input.compass.enabled)
        {
            Input.compass.enabled = false;
        }
        
        StartCoroutine(RecalibrateCompassCoroutine());
        #else
        Debug.Log("[LocationManager] ?? En Editor - simulando recalibración");
        #endif
    }
    
    private IEnumerator RecalibrateCompassCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        
        Input.compass.enabled = true;
        isCompassEnabled = true;
        compassHasValidData = false;
        
        Debug.Log("[LocationManager] ? Brújula reiniciada");
        Debug.Log("[LocationManager] ?? INSTRUCCIONES:");
        Debug.Log("[LocationManager]    1. Aleja el teléfono de objetos metálicos");
        Debug.Log("[LocationManager]    2. Mueve el dispositivo en forma de FIGURA 8");
        Debug.Log("[LocationManager]    3. Repite varias veces");
        
        // Reintentar inicialización
        yield return StartCoroutine(InitializeCompass());
    }
}
