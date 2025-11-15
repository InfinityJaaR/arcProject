using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

/// <summary>
/// Gestor de permisos para Android
/// Pide permisos de ubicación en runtime (requerido para Android 6.0+)
/// </summary>
public class PermissionsManager : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Pedir permisos automáticamente al iniciar")]
    public bool requestOnStart = true;
    
    [Tooltip("Mostrar diálogo explicativo antes de pedir permisos")]
    public bool showRationale = false; // Cambiado a false para simplificar
    
    // Estado de permisos
    private bool locationPermissionGranted = false;
    private bool cameraPermissionGranted = false;
    
    // Callbacks
    public System.Action OnAllPermissionsGranted;
    public System.Action OnPermissionsDenied;
    
    void Start()
    {
        if (requestOnStart)
        {
            CheckAndRequestPermissions();
        }
    }
    
    /// <summary>
    /// Verifica y pide todos los permisos necesarios
    /// </summary>
    public void CheckAndRequestPermissions()
    {
        Debug.Log("[PermissionsManager] ?? Verificando permisos...");
        
        #if PLATFORM_ANDROID
        
        // Verificar permisos de ubicación
        bool hasLocationFine = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        bool hasLocationCoarse = Permission.HasUserAuthorizedPermission(Permission.CoarseLocation);
        
        // Verificar permiso de cámara
        bool hasCamera = Permission.HasUserAuthorizedPermission(Permission.Camera);
        
        Debug.Log($"[PermissionsManager] ?? Ubicación (Fine): {hasLocationFine}");
        Debug.Log($"[PermissionsManager] ?? Ubicación (Coarse): {hasLocationCoarse}");
        Debug.Log($"[PermissionsManager] ?? Cámara: {hasCamera}");
        
        // Si faltan permisos, pedirlos
        if (!hasLocationFine || !hasLocationCoarse || !hasCamera)
        {
            RequestPermissions();
        }
        else
        {
            Debug.Log("[PermissionsManager] ? Todos los permisos ya están concedidos");
            locationPermissionGranted = true;
            cameraPermissionGranted = true;
            OnAllPermissionsGranted?.Invoke();
        }
        
        #else
        // En Editor o iOS, asumir que los permisos están concedidos
        Debug.Log("[PermissionsManager] ?? No es Android, asumiendo permisos concedidos");
        locationPermissionGranted = true;
        cameraPermissionGranted = true;
        OnAllPermissionsGranted?.Invoke();
        #endif
    }
    
    /// <summary>
    /// Pide los permisos al usuario
    /// </summary>
    private void RequestPermissions()
    {
        #if PLATFORM_ANDROID
        
        Debug.Log("[PermissionsManager] ?? Pidiendo permisos al usuario...");
        
        // Array de permisos a pedir
        string[] permissions = new string[]
        {
            Permission.FineLocation,
            Permission.CoarseLocation,
            Permission.Camera
        };
        
        Debug.Log("[PermissionsManager] ?? Pidiendo permisos:");
        foreach (string perm in permissions)
        {
            Debug.Log($"   - {perm}");
        }
        
        // Pedir permisos usando la API de Unity
        Permission.RequestUserPermissions(permissions);
        
        // Verificar permisos después de un breve delay
        Invoke(nameof(CheckPermissionsResult), 1f);
        
        #endif
    }
    
    /// <summary>
    /// Verifica el resultado de los permisos después de pedirlos
    /// </summary>
    private void CheckPermissionsResult()
    {
        #if PLATFORM_ANDROID
        
        Debug.Log("[PermissionsManager] ?? Verificando resultado de permisos...");
        
        bool hasLocationFine = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        bool hasLocationCoarse = Permission.HasUserAuthorizedPermission(Permission.CoarseLocation);
        bool hasCamera = Permission.HasUserAuthorizedPermission(Permission.Camera);
        
        locationPermissionGranted = hasLocationFine || hasLocationCoarse;
        cameraPermissionGranted = hasCamera;
        
        Debug.Log($"[PermissionsManager] ?? Ubicación: {locationPermissionGranted}");
        Debug.Log($"[PermissionsManager] ?? Cámara: {cameraPermissionGranted}");
        
        if (locationPermissionGranted && cameraPermissionGranted)
        {
            Debug.Log("[PermissionsManager] ? Todos los permisos concedidos!");
            OnAllPermissionsGranted?.Invoke();
            
            // Reiniciar LocationManager para que intente inicializar GPS
            if (LocationManager.Instance != null)
            {
                Debug.Log("[PermissionsManager] ?? Reiniciando LocationManager...");
                StartCoroutine(LocationManager.Instance.ReinitializeLocation());
            }
        }
        else
        {
            Debug.LogWarning("[PermissionsManager] ? Faltan permisos necesarios");
            OnPermissionsDenied?.Invoke();
            
            // Reintentar después de 2 segundos por si el usuario no respondió aún
            Invoke(nameof(RetryPermissions), 2f);
        }
        
        #endif
    }
    
    /// <summary>
    /// Reintenta verificar permisos
    /// </summary>
    private void RetryPermissions()
    {
        #if PLATFORM_ANDROID
        
        // Verificar una vez más
        bool hasLocationFine = Permission.HasUserAuthorizedPermission(Permission.FineLocation);
        bool hasLocationCoarse = Permission.HasUserAuthorizedPermission(Permission.CoarseLocation);
        bool hasCamera = Permission.HasUserAuthorizedPermission(Permission.Camera);
        
        locationPermissionGranted = hasLocationFine || hasLocationCoarse;
        cameraPermissionGranted = hasCamera;
        
        if (locationPermissionGranted && cameraPermissionGranted)
        {
            Debug.Log("[PermissionsManager] ? Permisos concedidos (retry)");
            OnAllPermissionsGranted?.Invoke();
            
            if (LocationManager.Instance != null)
            {
                StartCoroutine(LocationManager.Instance.ReinitializeLocation());
            }
        }
        else
        {
            Debug.LogError("[PermissionsManager] ? Permisos denegados por el usuario");
            Debug.LogWarning("[PermissionsManager] ?? La app no funcionará correctamente sin permisos de ubicación");
        }
        
        #endif
    }
    
    // Propiedades públicas
    public bool HasLocationPermission => locationPermissionGranted;
    public bool HasCameraPermission => cameraPermissionGranted;
    public bool HasAllPermissions => locationPermissionGranted && cameraPermissionGranted;
}
