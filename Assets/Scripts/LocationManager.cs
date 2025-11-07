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
    [SerializeField] private LocationServiceStatus locationStatus;
    
    // Ubicación actual del usuario
    private double currentLatitude;
    private double currentLongitude;
    private float currentAccuracy;
    private float currentBearing; // Bearing del dispositivo (orientación)
    
    // Eventos
    public event Action<double, double> OnLocationUpdated;
    public event Action<float> OnBearingUpdated;
    public event Action<string> OnLocationError;
    
    // Propiedades públicas
    public bool IsGPSReady => isGPSEnabled && locationStatus == LocationServiceStatus.Running;
    public bool IsCompassReady => isCompassEnabled;
    public double CurrentLatitude => currentLatitude;
    public double CurrentLongitude => currentLongitude;
    public float CurrentAccuracy => currentAccuracy;
    public float CurrentBearing => currentBearing;
    
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
            isGPSEnabled = true;
            isCompassEnabled = true;
            locationStatus = LocationServiceStatus.Running;
            yield break;
        }
        #endif
        
        // Verificar si el usuario tiene GPS en el dispositivo
        if (!Input.location.isEnabledByUser)
        {
            string error = "GPS no está habilitado por el usuario. Actívalo en Configuración.";
            Debug.LogError($"[LocationManager] ? {error}");
            OnLocationError?.Invoke(error);
            yield break;
        }
        
        // Iniciar servicio de ubicación
        Input.location.Start(desiredAccuracyInMeters, updateDistanceInMeters);
        
        // Esperar hasta que el servicio esté inicializado
        int waitCounter = 0;
        while (Input.location.status == LocationServiceStatus.Initializing && waitCounter < maxWaitTime)
        {
            yield return new WaitForSeconds(1);
            waitCounter++;
            Debug.Log($"[LocationManager] ? Inicializando GPS... {waitCounter}/{maxWaitTime}");
        }
        
        // Verificar timeout
        if (waitCounter >= maxWaitTime)
        {
            string error = "Timeout al inicializar GPS. Verifica tu conexión.";
            Debug.LogError($"[LocationManager] ?? {error}");
            OnLocationError?.Invoke(error);
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
        
        // Inicializar brújula
        Input.compass.enabled = true;
        isCompassEnabled = true;
        Debug.Log("[LocationManager] ?? Brújula inicializada");
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
        
        // Actualizar bearing de la brújula
        if (isCompassEnabled)
        {
            // Input.compass.trueHeading da el norte verdadero
            // Input.compass.magneticHeading da el norte magnético
            currentBearing = Input.compass.trueHeading;
            OnBearingUpdated?.Invoke(currentBearing);
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
}
