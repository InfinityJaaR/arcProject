using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestor de la interfaz de usuario para el sistema de navegación
/// Muestra lista de lugares disponibles y controles de navegación
/// </summary>
public class NavigationUIManager : MonoBehaviour
{
    [Header("Paneles UI")]
    [Tooltip("Panel principal con la lista de lugares")]
    public GameObject locationSelectionPanel;
    
    [Tooltip("Panel mostrado durante la navegación")]
    public GameObject navigationActivePanel;
    
    [Header("Lista de Lugares")]
    [Tooltip("ScrollView content donde se agregarán los botones de lugares")]
    public Transform locationListContent;
    
    [Tooltip("Prefab del botón para cada lugar")]
    public GameObject locationButtonPrefab;
    
    [Tooltip("Texto de loading mientras carga la lista")]
    public TextMeshProUGUI loadingText;
    
    [Header("Panel de Navegación Activa")]
    [Tooltip("Texto que muestra el nombre del destino actual")]
    public TextMeshProUGUI destinationNameText;
    
    [Tooltip("Texto que muestra la distancia al destino")]
    public TextMeshProUGUI distanceText;
    
    [Tooltip("Texto que muestra la dirección cardinal")]
    public TextMeshProUGUI directionText;
    
    [Tooltip("Botón para cancelar la navegación")]
    public Button cancelNavigationButton;
    
    [Header("Botones Principales")]
    [Tooltip("Botón para abrir el panel de selección de lugares")]
    public Button openLocationPanelButton;
    
    [Header("Configuración")]
    [Tooltip("Cargar lista de lugares automáticamente al iniciar")]
    public bool loadLocationsOnStart = true;
    
    private List<BuildingData> availableBuildings = new List<BuildingData>();
    private BuildingData selectedDestination;
    private bool isInitialized = false;
    
    void Awake()
    {
        Debug.Log("[NavigationUIManager] ?? Awake llamado");
    }
    
    void Start()
    {
        Debug.Log("[NavigationUIManager] ?? Start llamado - Iniciando configuración...");
        StartCoroutine(InitializeWithDelay());
    }
    
    private IEnumerator InitializeWithDelay()
    {
        // Esperar un frame para asegurar que todo esté inicializado
        yield return null;
        
        Initialize();
    }
    
    private void Initialize()
    {
        if (isInitialized)
        {
            Debug.Log("[NavigationUIManager] ?? Ya está inicializado, saltando...");
            return;
        }
        
        Debug.Log("[NavigationUIManager] ?? Inicializando componentes...");
        
        // Verificar y buscar el botón principal si no está asignado
        if (openLocationPanelButton == null)
        {
            Debug.LogWarning("[NavigationUIManager] ?? openLocationPanelButton no asignado, buscando...");
            
            // Buscar en el canvas actual
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.gameObject.name == "OpenLocationPanelButton")
                {
                    openLocationPanelButton = btn;
                    Debug.Log("[NavigationUIManager] ? Botón encontrado automáticamente en hijos");
                    break;
                }
            }
            
            // Si aún no lo encuentra, buscar en toda la escena
            if (openLocationPanelButton == null)
            {
                GameObject buttonObj = GameObject.Find("OpenLocationPanelButton");
                if (buttonObj != null)
                {
                    openLocationPanelButton = buttonObj.GetComponent<Button>();
                    Debug.Log("[NavigationUIManager] ? Botón encontrado en escena");
                }
            }
        }
        
        // Configurar listeners de botones
        if (cancelNavigationButton != null)
        {
            // NO remover listeners anteriores
            // cancelNavigationButton.onClick.RemoveAllListeners(); ? COMENTADO
            cancelNavigationButton.onClick.AddListener(OnCancelNavigationClicked);
            Debug.Log("[NavigationUIManager] ? Listener de cancelar configurado");
        }
        else
        {
            Debug.LogWarning("[NavigationUIManager] ?? cancelNavigationButton no asignado");
        }
        
        if (openLocationPanelButton != null)
        {
            // NO remover listeners anteriores - solo añadir el nuestro
            // openLocationPanelButton.onClick.RemoveAllListeners(); ? COMENTADO
            
            // Añadir nuestro listener
            openLocationPanelButton.onClick.AddListener(OnOpenLocationPanelClicked);
            
            Debug.Log($"[NavigationUIManager] ? Listener del botón '{openLocationPanelButton.gameObject.name}' configurado");
            Debug.Log($"[NavigationUIManager] ?? Botón interactable: {openLocationPanelButton.interactable}");
            Debug.Log($"[NavigationUIManager] ?? Total de listeners en runtime ahora: {openLocationPanelButton.onClick.GetPersistentEventCount()}");
            
            // Verificar si el listener se añadió correctamente
            int listenerCount = 0;
            try
            {
                // Intentar contar los listeners de otra forma
                var buttonEvent = openLocationPanelButton.onClick;
                listenerCount = buttonEvent.GetPersistentEventCount();
            }
            catch
            {
                listenerCount = -1;
            }
            Debug.Log($"[NavigationUIManager] ?? Verificación de listeners: {listenerCount}");
        }
        else
        {
            Debug.LogError("[NavigationUIManager] ? openLocationPanelButton es NULL - El botón NO funcionará!");
            Debug.LogError("[NavigationUIManager] ?? SOLUCIÓN: Asigna manualmente el botón en el Inspector");
        }
        
        // Ocultar paneles al inicio
        HideAllPanels();
        
        // Verificar referencias críticas
        VerifyReferences();
        
        // Cargar lista de lugares
        if (loadLocationsOnStart)
        {
            LoadAvailableLocations();
        }
        
        isInitialized = true;
        Debug.Log("[NavigationUIManager] ? Inicializado completamente");
    }
    
    private void VerifyReferences()
    {
        int errorCount = 0;
        
        if (locationSelectionPanel == null)
        {
            Debug.LogError("[NavigationUIManager] ? locationSelectionPanel no asignado");
            errorCount++;
        }
        
        if (navigationActivePanel == null)
        {
            Debug.LogError("[NavigationUIManager] ? navigationActivePanel no asignado");
            errorCount++;
        }
            
        if (locationListContent == null)
        {
            Debug.LogError("[NavigationUIManager] ? locationListContent no asignado");
            errorCount++;
        }
            
        if (locationButtonPrefab == null)
        {
            Debug.LogError("[NavigationUIManager] ? locationButtonPrefab no asignado");
            errorCount++;
        }
        
        if (errorCount > 0)
        {
            Debug.LogError($"[NavigationUIManager] ?? TOTAL DE ERRORES: {errorCount}");
        }
        else
        {
            Debug.Log("[NavigationUIManager] ? Todas las referencias verificadas correctamente");
        }
    }
    
    /// <summary>
    /// Carga la lista de lugares disponibles desde Firebase
    /// </summary>
    public async void LoadAvailableLocations()
    {
        if (FirebaseManager.Instance == null)
        {
            Debug.LogError("[NavigationUIManager] ? FirebaseManager no encontrado");
            return;
        }
        
        // Mostrar loading
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = "? Cargando lugares...";
        }
        
        Debug.Log("[NavigationUIManager] ?? Cargando lugares desde Firebase...");
        
        // Obtener todos los edificios
        availableBuildings = await FirebaseManager.Instance.GetAllBuildingsAsync();
        
        // Ocultar loading
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
        }
        
        if (availableBuildings.Count == 0)
        {
            Debug.LogWarning("[NavigationUIManager] ?? No se encontraron lugares");
            return;
        }
        
        Debug.Log($"[NavigationUIManager] ? Se cargaron {availableBuildings.Count} lugares");
        
        // Poblar la lista UI
        PopulateLocationList();
    }
    
    /// <summary>
    /// Puebla la UI con botones para cada lugar disponible
    /// </summary>
    private void PopulateLocationList()
    {
        if (locationListContent == null || locationButtonPrefab == null)
        {
            Debug.LogError("[NavigationUIManager] ? Faltan referencias de UI");
            return;
        }
        
        // Limpiar lista anterior
        foreach (Transform child in locationListContent)
        {
            Destroy(child.gameObject);
        }
        
        // Crear botón para cada lugar
        foreach (BuildingData building in availableBuildings)
        {
            GameObject buttonObj = Instantiate(locationButtonPrefab, locationListContent);
            
            // Configurar texto del botón - MEJORADO
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                // Solo mostrar el nombre en grande, descripción más pequeña debajo
                string desc = building.description.Length > 60 
                    ? building.description.Substring(0, 60) + "..." 
                    : building.description;
                
                buttonText.text = $"<size=20><b>{building.name}</b></size>\n<size=14><color=#CCCCCC>{desc}</color></size>";
                
                // Asegurar alineación correcta
                buttonText.alignment = TextAlignmentOptions.Left;
                buttonText.margin = new Vector4(15, 10, 15, 10); // Márgenes internos
            }
            
            // Configurar click del botón
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                BuildingData buildingCopy = building; // Captura para el closure
                button.onClick.AddListener(() => OnLocationSelected(buildingCopy));
            }
        }
        
        Debug.Log($"[NavigationUIManager] ?? Lista de {availableBuildings.Count} lugares creada");
    }
    
    /// <summary>
    /// Callback cuando el usuario selecciona un lugar
    /// </summary>
    private void OnLocationSelected(BuildingData building)
    {
        selectedDestination = building;
        
        Debug.Log($"[NavigationUIManager] ?? Lugar seleccionado: {building.name}");
        
        // Ocultar panel de selección
        if (locationSelectionPanel != null)
        {
            locationSelectionPanel.SetActive(false);
        }
        
        // Iniciar navegación
        if (AppModeManager.Instance != null)
        {
            AppModeManager.Instance.StartNavigation(building);
        }
        else
        {
            Debug.LogError("[NavigationUIManager] ? AppModeManager.Instance es NULL");
        }
        
        // Mostrar panel de navegación activa
        ShowNavigationUI();
    }
    
    /// <summary>
    /// Muestra la UI de navegación activa
    /// </summary>
    public void ShowNavigationUI()
    {
        if (navigationActivePanel != null)
        {
            navigationActivePanel.SetActive(true);
        }
        
        if (selectedDestination != null && destinationNameText != null)
        {
            destinationNameText.text = selectedDestination.name;
        }
        
        Debug.Log("[NavigationUIManager] ? UI de navegación mostrada");
    }
    
    /// <summary>
    /// Oculta la UI de navegación activa
    /// </summary>
    public void HideNavigationUI()
    {
        if (navigationActivePanel != null)
        {
            navigationActivePanel.SetActive(false);
        }
        
        Debug.Log("[NavigationUIManager] ?? UI de navegación ocultada");
    }
    
    /// <summary>
    /// Oculta todos los paneles
    /// </summary>
    private void HideAllPanels()
    {
        if (locationSelectionPanel != null)
            locationSelectionPanel.SetActive(false);
        
        if (navigationActivePanel != null)
            navigationActivePanel.SetActive(false);
            
        Debug.Log("[NavigationUIManager] ?? Todos los paneles ocultados");
    }
    
    /// <summary>
    /// Callback cuando se hace click en "Abrir Panel de Lugares"
    /// </summary>
    private void OnOpenLocationPanelClicked()
    {
        Debug.Log("[NavigationUIManager] ?? ============================================");
        Debug.Log("[NavigationUIManager] ?? BOTÓN 'NAVEGAR' PRESIONADO!");
        Debug.Log("[NavigationUIManager] ?? ============================================");
        
        if (locationSelectionPanel == null)
        {
            Debug.LogError("[NavigationUIManager] ? locationSelectionPanel es NULL - No se puede abrir el panel");
            return;
        }
        
        bool wasActive = locationSelectionPanel.activeSelf;
        locationSelectionPanel.SetActive(true);
        
        string statusMessage = wasActive ? "ya estaba activo" : "ahora está activo";
        Debug.Log($"[NavigationUIManager] ? Panel de lugares {statusMessage}");
        Debug.Log($"[NavigationUIManager] ?? Panel name: {locationSelectionPanel.name}");
        Debug.Log($"[NavigationUIManager] ?? Panel active: {locationSelectionPanel.activeSelf}");
        
        // Recargar lista por si hubo cambios o si está vacía
        if (availableBuildings == null || availableBuildings.Count == 0)
        {
            Debug.Log("[NavigationUIManager] ?? Lista vacía, cargando lugares...");
            LoadAvailableLocations();
        }
        else
        {
            Debug.Log($"[NavigationUIManager] ?? Mostrando {availableBuildings.Count} lugares disponibles");
        }
    }
    
    /// <summary>
    /// Callback cuando se hace click en "Cancelar Navegación"
    /// </summary>
    private void OnCancelNavigationClicked()
    {
        Debug.Log("[NavigationUIManager] ? Navegación cancelada por el usuario");
        
        if (AppModeManager.Instance != null)
        {
            AppModeManager.Instance.CancelNavigation();
        }
        
        HideNavigationUI();
        selectedDestination = null;
    }
    
    /// <summary>
    /// Actualiza los textos de distancia y dirección en tiempo real
    /// Debe ser llamado desde NavigationArrowController
    /// </summary>
    public void UpdateNavigationInfo(float distance, string direction)
    {
        if (distanceText != null)
        {
            distanceText.text = GeoUtils.FormatDistance(distance);
        }
        
        if (directionText != null)
        {
            directionText.text = direction;
        }
    }
    
    void Update()
    {
        // Actualizar información de navegación en tiempo real
        if (navigationActivePanel != null && navigationActivePanel.activeSelf && selectedDestination != null)
        {
            if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
            {
                float distance = LocationManager.Instance.GetDistanceToDestination(
                    selectedDestination.latitude,
                    selectedDestination.longitude
                );
                
                float bearing = LocationManager.Instance.GetBearingToDestination(
                    selectedDestination.latitude,
                    selectedDestination.longitude
                );
                
                UpdateNavigationInfo(distance, GeoUtils.BearingToCardinal(bearing) + $" ({bearing:F0}°)");
            }
        }
    }
    
    // Método público para forzar reinicialización si es necesario
    public void ForceInitialize()
    {
        isInitialized = false;
        Initialize();
    }
}
