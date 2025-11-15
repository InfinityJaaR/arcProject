using UnityEngine;
using TMPro;

/// <summary>
/// Controla la flecha AR que apunta hacia el destino seleccionado
/// Usa GPS y brújula para calcular la dirección correcta
/// </summary>
public class NavigationArrowController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Prefab de la flecha que se instanciará")]
    public GameObject arrowPrefab;
    
    [Tooltip("Cámara AR (se detecta automáticamente si no se asigna)")]
    public Camera arCamera;
    
    [Tooltip("Texto para mostrar distancia (opcional)")]
    public TextMeshProUGUI distanceText;
    
    [Tooltip("Texto para mostrar dirección (opcional)")]
    public TextMeshProUGUI directionText;
    
    [Header("Configuración de Posicionamiento")]
    [Tooltip("Distancia de la flecha frente a la cámara (metros)")]
    public float arrowDistance = 2f;
    
    [Tooltip("Altura de la flecha respecto al nivel de los ojos (metros)")]
    public float arrowHeightOffset = -0.5f;
    
    [Tooltip("Escala de la flecha")]
    public float arrowScale = 0.3f;
    
    [Tooltip("Suavizar movimiento de posición (evita saltos)")]
    public bool smoothPositionMovement = true;
    
    [Tooltip("Velocidad de suavizado de posición")]
    [Range(1f, 20f)]
    public float positionSmoothSpeed = 10f;
    
    [Header("Corrección de Orientación")]
    [Tooltip("Marcar si el modelo de flecha apunta hacia atrás (invertido)")]
    public bool invertArrowModel = true; // TRUE por defecto según el reporte
    
    [Header("Configuración de Rotación")]
    [Tooltip("Suavizado de la rotación (menor = más suave, mayor = más responsivo)")]
    [Range(1f, 20f)]
    public float rotationSmoothSpeed = 8f;
    
    [Tooltip("Aplicar rotación vertical (inclinación) según distancia")]
    public bool enableVerticalTilt = true;
    
    [Header("Feedback Visual")]
    [Tooltip("Cambiar color según distancia")]
    public bool enableDistanceColorFeedback = true;
    
    [Tooltip("Color cuando está lejos (> 100m)")]
    public Color farColor = Color.red;
    
    [Tooltip("Color cuando está cerca (< 20m)")]
    public Color nearColor = Color.green;
    
    [Header("Animación")]
    [Tooltip("Hacer que la flecha pulse/oscile")]
    public bool enablePulseAnimation = true;
    
    [Tooltip("Velocidad de pulsación")]
    public float pulseSpeed = 2f;
    
    [Tooltip("Intensidad de pulsación (escala)")]
    public float pulseIntensity = 0.1f;
    
    // Estado interno
    private GameObject arrowInstance;
    private BuildingData currentDestination;
    private bool isNavigating = false;
    private Renderer arrowRenderer;
    private Vector3 baseScale;
    private float targetYRotation = 0f;
    
    void Awake()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
        
        if (arrowPrefab == null)
        {
            Debug.LogError("[NavigationArrowController] ? Arrow Prefab no está asignado!");
        }
    }
    
    void Start()
    {
        // Suscribirse a eventos del LocationManager
        if (LocationManager.Instance != null)
        {
            LocationManager.Instance.OnLocationUpdated += OnLocationUpdated;
            LocationManager.Instance.OnBearingUpdated += OnBearingUpdated;
        }
    }
    
    void Update()
    {
        if (!isNavigating || arrowInstance == null || currentDestination == null)
            return;
        
        // DEBUG: Mostrar estado cada 2 segundos
        if (Time.frameCount % 120 == 0 && LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
        {
            Debug.Log($"????????????????????????????????????");
            Debug.Log($"[NavigationArrowController] ?? ESTADO:");
            Debug.Log($"  GPS Ready: {LocationManager.Instance.IsGPSReady}");
            Debug.Log($"  Mi ubicación: {LocationManager.Instance.CurrentLatitude:F6}, {LocationManager.Instance.CurrentLongitude:F6}");
            Debug.Log($"  Destino: {currentDestination.name} ({currentDestination.latitude:F6}, {currentDestination.longitude:F6})");
            Debug.Log($"  Bearing dispositivo: {LocationManager.Instance.CurrentBearing:F1}°");
            float bearingToDest = LocationManager.Instance.GetBearingToDestination(currentDestination.latitude, currentDestination.longitude);
            Debug.Log($"  Bearing al destino: {bearingToDest:F1}°");
            Debug.Log($"  Ángulo relativo: {(bearingToDest - LocationManager.Instance.CurrentBearing):F1}°");
            Debug.Log($"  Distancia: {LocationManager.Instance.GetDistanceToDestination(currentDestination.latitude, currentDestination.longitude):F1}m");
            Debug.Log($"????????????????????????????????????");
        }
        
        UpdateArrowPosition();
        UpdateArrowRotation();
        UpdateDistanceDisplay();
        
        if (enablePulseAnimation)
        {
            UpdatePulseAnimation();
        }
    }
    
    /// <summary>
    /// Establece el destino y comienza la navegación
    /// </summary>
    public void SetDestination(BuildingData destination)
    {
        if (destination == null)
        {
            Debug.LogWarning("[NavigationArrowController] ?? Destino es null");
            return;
        }
        
        currentDestination = destination;
        
        Debug.Log($"[NavigationArrowController] ?? Destino establecido: {destination.name}");
        Debug.Log($"[NavigationArrowController] ?? Coordenadas: {destination.latitude:F6}, {destination.longitude:F6}");
        
        StartNavigation();
    }
    
    /// <summary>
    /// Inicia la navegación mostrando la flecha
    /// </summary>
    private void StartNavigation()
    {
        if (arrowPrefab == null)
        {
            Debug.LogError("[NavigationArrowController] ? No se puede iniciar navegación sin prefab de flecha");
            return;
        }
        
        // Destruir flecha anterior si existe
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
        }
        
        // Calcular posición inicial frente a la cámara
        Vector3 initialPosition = Vector3.zero;
        Quaternion initialRotation = Quaternion.identity;
        
        if (arCamera != null)
        {
            Vector3 forward = arCamera.transform.forward;
            forward.y = 0; // Mantener en plano horizontal
            forward.Normalize();
            
            initialPosition = arCamera.transform.position + 
                             forward * arrowDistance + 
                             Vector3.up * arrowHeightOffset;
            
            initialRotation = Quaternion.LookRotation(forward);
        }
        
        // Instanciar la flecha EN LA POSICIÓN CORRECTA
        arrowInstance = Instantiate(arrowPrefab, initialPosition, initialRotation);
        arrowInstance.name = "NavigationArrow";
        
        // IMPORTANTE: NO hacer la flecha hija de la cámara
        // Debe estar en el mundo para mantener orientación absoluta
        arrowInstance.transform.SetParent(null);
        
        // Configurar escala
        baseScale = Vector3.one * arrowScale;
        arrowInstance.transform.localScale = baseScale;
        
        // Obtener renderer para cambiar color
        arrowRenderer = arrowInstance.GetComponent<Renderer>();
        if (arrowRenderer == null)
        {
            arrowRenderer = arrowInstance.GetComponentInChildren<Renderer>();
        }
        
        isNavigating = true;
        
        Debug.Log("[NavigationArrowController] ? Flecha de navegación activa");
        Debug.Log($"[NavigationArrowController] ?? Posición inicial: {initialPosition}");
        
        // Verificar que el GPS esté listo
        if (LocationManager.Instance != null && !LocationManager.Instance.IsGPSReady)
        {
            Debug.LogWarning("[NavigationArrowController] ?? GPS no está listo. Esperando inicialización...");
        }
    }
    
    /// <summary>
    /// Detiene la navegación y oculta la flecha
    /// </summary>
    public void StopNavigation()
    {
        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
            arrowInstance = null;
        }
        
        isNavigating = false;
        currentDestination = null;
        
        Debug.Log("[NavigationArrowController] ?? Navegación detenida");
    }
    
    /// <summary>
    /// Actualiza la posición de la flecha frente a la cámara AR
    /// </summary>
    private void UpdateArrowPosition()
    {
        if (arCamera == null) return;
        
        // Calcular posición objetivo
        Vector3 forward = arCamera.transform.forward;
        forward.y = 0; // Mantener en plano horizontal
        forward.Normalize();
        
        Vector3 targetPosition = arCamera.transform.position + 
                                 forward * arrowDistance + 
                                 Vector3.up * arrowHeightOffset;
        
        // Aplicar posición con o sin suavizado
        if (smoothPositionMovement)
        {
            arrowInstance.transform.position = Vector3.Lerp(
                arrowInstance.transform.position,
                targetPosition,
                Time.deltaTime * positionSmoothSpeed
            );
        }
        else
        {
            arrowInstance.transform.position = targetPosition;
        }
    }
    
    /// <summary>
    /// Actualiza la rotación de la flecha para apuntar al destino
    /// USA BEARING RELATIVO para que la flecha apunte correctamente
    /// </summary>
    private void UpdateArrowRotation()
    {
        // Verificar LocationManager
        if (LocationManager.Instance == null)
        {
            Debug.LogError("[NavigationArrowController] ? LocationManager.Instance es NULL!");
            return;
        }
        
        // Verificar destino
        if (currentDestination == null)
        {
            Debug.LogWarning("[NavigationArrowController] ?? No hay destino establecido");
            return;
        }
        
        // Verificar GPS
        if (!LocationManager.Instance.IsGPSReady)
        {
            if (Time.frameCount % 300 == 0)
            {
                Debug.LogWarning("[NavigationArrowController] ?? GPS no está listo");
            }
            return;
        }
        
        // Obtener bearing ABSOLUTO hacia el destino (0-360° desde el Norte geográfico)
        float bearingToDestination = LocationManager.Instance.GetBearingToDestination(
            currentDestination.latitude,
            currentDestination.longitude
        );
        
        // Obtener bearing del dispositivo (hacia dónde apunta el teléfono)
        float deviceBearing = LocationManager.Instance.CurrentBearing;
        
        // CLAVE: Calcular ángulo RELATIVO
        // Esto hace que la flecha rote relativamente a tu orientación
        float relativeAngle = bearingToDestination - deviceBearing;
        
        // Normalizar el ángulo a rango [-180, 180]
        while (relativeAngle > 180f) relativeAngle -= 360f;
        while (relativeAngle < -180f) relativeAngle += 360f;
        
        // DEBUG: Mostrar valores cada segundo
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[NavigationArrowController] ???????????????????????????");
            Debug.Log($"[NavigationArrowController] ?? Destino: {currentDestination.name}");
            Debug.Log($"[NavigationArrowController] ?? Mi posición: {LocationManager.Instance.CurrentLatitude:F6}, {LocationManager.Instance.CurrentLongitude:F6}");
            Debug.Log($"[NavigationArrowController] ?? Destino: {currentDestination.latitude:F6}, {currentDestination.longitude:F6}");
            Debug.Log($"[NavigationArrowController] ?? Bearing al destino: {bearingToDestination:F1}° (desde Norte)");
            Debug.Log($"[NavigationArrowController] ?? Bearing del dispositivo: {deviceBearing:F1}° (hacia dónde miras)");
            Debug.Log($"[NavigationArrowController] ? Ángulo RELATIVO: {relativeAngle:F1}°");
            Debug.Log($"[NavigationArrowController] ?? Interpretación:");
            
            if (Mathf.Abs(relativeAngle) < 10f)
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta HACIA ADELANTE");
            else if (relativeAngle > 80f && relativeAngle < 100f)
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta a tu DERECHA");
            else if (relativeAngle < -80f && relativeAngle > -100f)
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta a tu IZQUIERDA");
            else if (Mathf.Abs(relativeAngle) > 170f)
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta HACIA ATRÁS");
            else if (relativeAngle > 0)
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta ADELANTE-DERECHA");
            else
                Debug.Log($"[NavigationArrowController]    ? Flecha apunta ADELANTE-IZQUIERDA");
                
            Debug.Log($"[NavigationArrowController] ???????????????????????????");
        }
        
        // ?? FIX PARA MODELO INVERTIDO:
        // Si el modelo 3D de la flecha apunta hacia -Z en vez de +Z,
        // necesitamos invertir la rotación agregando 180°
        
        // Establecer rotación objetivo
        targetYRotation = relativeAngle;
        
        // Aplicar corrección si el modelo está invertido
        if (invertArrowModel)
        {
            targetYRotation += 180f;
        }
        
        // Normalizar a rango [0, 360)
        targetYRotation = (targetYRotation + 360f) % 360f;
        
        // Crear la rotación objetivo
        Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);
        
        // Aplicar rotación suavizada
        arrowInstance.transform.rotation = Quaternion.Slerp(
            arrowInstance.transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothSpeed
        );
        
        // Aplicar inclinación vertical opcional
        if (enableVerticalTilt)
        {
            float distance = LocationManager.Instance.GetDistanceToDestination(
                currentDestination.latitude,
                currentDestination.longitude
            );
            
            // Inclinar hacia abajo si está cerca, hacia arriba si está lejos
            float tiltAngle = Mathf.Clamp(distance / 100f, -30f, 30f);
            Vector3 currentEuler = arrowInstance.transform.eulerAngles;
            currentEuler.x = tiltAngle;
            arrowInstance.transform.eulerAngles = currentEuler;
        }
    }
    
    /// <summary>
    /// Actualiza los textos de distancia y dirección
    /// </summary>
    private void UpdateDistanceDisplay()
    {
        if (LocationManager.Instance == null || !LocationManager.Instance.IsGPSReady)
        {
            if (distanceText != null)
                distanceText.text = "GPS no disponible";
            return;
        }
        
        float distance = LocationManager.Instance.GetDistanceToDestination(
            currentDestination.latitude,
            currentDestination.longitude
        );
        
        float bearing = LocationManager.Instance.GetBearingToDestination(
            currentDestination.latitude,
            currentDestination.longitude
        );
        
        // Actualizar texto de distancia
        if (distanceText != null)
        {
            distanceText.text = $"{currentDestination.name}\n{GeoUtils.FormatDistance(distance)}";
        }
        
        // Actualizar texto de dirección
        if (directionText != null)
        {
            string cardinal = GeoUtils.BearingToCardinal(bearing);
            directionText.text = $"{cardinal} ({bearing:F0}°)";
        }
        
        // Actualizar color según distancia
        if (enableDistanceColorFeedback && arrowRenderer != null)
        {
            float t = Mathf.InverseLerp(100f, 20f, distance);
            Color targetColor = Color.Lerp(farColor, nearColor, t);
            arrowRenderer.material.color = targetColor;
        }
    }
    
    /// <summary>
    /// Anima la flecha con un efecto de pulsación
    /// </summary>
    private void UpdatePulseAnimation()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
        arrowInstance.transform.localScale = baseScale * pulse;
    }
    
    /// <summary>
    /// Callback cuando la ubicación GPS se actualiza
    /// </summary>
    private void OnLocationUpdated(double lat, double lon)
    {
        if (!isNavigating) return;
        
        Debug.Log($"[NavigationArrowController] ?? Ubicación actualizada: {lat:F6}, {lon:F6}");
    }
    
    /// <summary>
    /// Callback cuando el bearing (orientación) se actualiza
    /// </summary>
    private void OnBearingUpdated(float bearing)
    {
        // La rotación se actualiza en Update()
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (LocationManager.Instance != null)
        {
            LocationManager.Instance.OnLocationUpdated -= OnLocationUpdated;
            LocationManager.Instance.OnBearingUpdated -= OnBearingUpdated;
        }
        
        StopNavigation();
    }
    
    /// <summary>
    /// Dibuja gizmos para debugging - muestra hacia dónde apunta la flecha
    /// </summary>
    void OnDrawGizmos()
    {
        if (!isNavigating || arrowInstance == null)
            return;
        
        // Línea verde mostrando la dirección de la flecha
        Vector3 arrowPos = arrowInstance.transform.position;
        Vector3 arrowForward = arrowInstance.transform.forward;
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(arrowPos, arrowPos + arrowForward * 5f);
        
        // Esfera amarilla en la posición de la flecha
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(arrowPos, 0.2f);
        
        // Si GPS está listo, mostrar información adicional
        if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady && currentDestination != null)
        {
            // Línea roja mostrando el bearing al destino (aproximado)
            float bearingToDest = LocationManager.Instance.GetBearingToDestination(
                currentDestination.latitude,
                currentDestination.longitude
            );
            
            float deviceBearing = LocationManager.Instance.CurrentBearing;
            float relativeAngle = bearingToDest - deviceBearing;
            
            Vector3 directionToDest = Quaternion.Euler(0, relativeAngle, 0) * Vector3.forward;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(arrowPos, arrowPos + directionToDest * 5f);
        }
    }
}
