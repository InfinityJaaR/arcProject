using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Gestor que controla los dos modos de operación de la aplicación:
/// - MARKER_TRACKING: Detección y tracking de patrones AR
/// - NAVIGATION: Navegación con brújula hacia un destino
/// </summary>
public class AppModeManager : MonoBehaviour
{
    public static AppModeManager Instance { get; private set; }
    
    public enum AppMode
    {
        MARKER_TRACKING,  // Modo de tracking de imágenes AR
        NAVIGATION        // Modo de navegación con brújula
    }
    
    [Header("Referencias")]
    [Tooltip("Componente que gestiona el tracking de imágenes AR")]
    public ARTrackedImageManager arTrackedImageManager;
    
    [Tooltip("Componente que spawna objetos sobre marcadores")]
    public MultiImageSpawner multiImageSpawner;
    
    [Tooltip("Controlador del sistema de navegación")]
    public NavigationArrowController navigationController;
    
    [Tooltip("UI Manager de navegación")]
    public NavigationUIManager navigationUIManager;
    
    [Header("Estado")]
    [SerializeField]
    private AppMode currentMode = AppMode.MARKER_TRACKING;
    
    public AppMode CurrentMode => currentMode;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // Validar referencias
        if (arTrackedImageManager == null)
            arTrackedImageManager = FindAnyObjectByType<ARTrackedImageManager>();
        
        if (multiImageSpawner == null)
            multiImageSpawner = FindAnyObjectByType<MultiImageSpawner>();
        
        Debug.Log("[AppModeManager] Inicializado en modo: " + currentMode);
    }
    
    void Start()
    {
        // Asegurar que empezamos en modo MARKER_TRACKING
        SetMode(AppMode.MARKER_TRACKING);
    }
    
    /// <summary>
    /// Cambia el modo de operación de la aplicación
    /// </summary>
    public void SetMode(AppMode newMode)
    {
        if (currentMode == newMode)
        {
            Debug.Log($"[AppModeManager] Ya estamos en modo {newMode}");
            return;
        }
        
        Debug.Log($"[AppModeManager] ?? Cambiando modo: {currentMode} ? {newMode}");
        
        currentMode = newMode;
        
        switch (newMode)
        {
            case AppMode.MARKER_TRACKING:
                EnableMarkerTracking();
                DisableNavigation();
                break;
                
            case AppMode.NAVIGATION:
                DisableMarkerTracking();
                EnableNavigation();
                break;
        }
    }
    
    /// <summary>
    /// Activa el modo de navegación con destino específico
    /// </summary>
    public void StartNavigation(BuildingData destination)
    {
        if (destination == null)
        {
            Debug.LogWarning("[AppModeManager] ?? No se puede iniciar navegación sin destino");
            return;
        }
        
        Debug.Log($"[AppModeManager] ?? Iniciando navegación hacia: {destination.name}");
        
        SetMode(AppMode.NAVIGATION);
        
        if (navigationController != null)
        {
            navigationController.SetDestination(destination);
        }
    }
    
    /// <summary>
    /// Cancela la navegación y vuelve al modo de tracking
    /// </summary>
    public void CancelNavigation()
    {
        Debug.Log("[AppModeManager] ? Cancelando navegación");
        SetMode(AppMode.MARKER_TRACKING);
    }
    
    /// <summary>
    /// Activa el sistema de tracking de marcadores AR
    /// </summary>
    private void EnableMarkerTracking()
    {
        Debug.Log("[AppModeManager] ? Activando Marker Tracking");
        
        if (arTrackedImageManager != null)
        {
            arTrackedImageManager.enabled = true;
        }
        
        if (multiImageSpawner != null)
        {
            multiImageSpawner.SetTrackingEnabled(true);
        }
    }
    
    /// <summary>
    /// Desactiva el sistema de tracking de marcadores AR
    /// </summary>
    private void DisableMarkerTracking()
    {
        Debug.Log("[AppModeManager] ?? Desactivando Marker Tracking");
        
        if (multiImageSpawner != null)
        {
            multiImageSpawner.SetTrackingEnabled(false);
        }
        
        // No desactivamos completamente el ARTrackedImageManager para mantener la sesión AR
        // Solo ocultamos los objetos spawneados
    }
    
    /// <summary>
    /// Activa el sistema de navegación
    /// </summary>
    private void EnableNavigation()
    {
        Debug.Log("[AppModeManager] ? Activando Navigation");
        
        if (navigationController != null)
        {
            navigationController.enabled = true;
        }
        
        if (navigationUIManager != null)
        {
            navigationUIManager.ShowNavigationUI();
        }
    }
    
    /// <summary>
    /// Desactiva el sistema de navegación
    /// </summary>
    private void DisableNavigation()
    {
        Debug.Log("[AppModeManager] ?? Desactivando Navigation");
        
        if (navigationController != null)
        {
            navigationController.StopNavigation();
            navigationController.enabled = false;
        }
        
        if (navigationUIManager != null)
        {
            navigationUIManager.HideNavigationUI();
        }
    }
    
    /// <summary>
    /// Alterna entre los dos modos
    /// </summary>
    public void ToggleMode()
    {
        if (currentMode == AppMode.MARKER_TRACKING)
            SetMode(AppMode.NAVIGATION);
        else
            SetMode(AppMode.MARKER_TRACKING);
    }
}
