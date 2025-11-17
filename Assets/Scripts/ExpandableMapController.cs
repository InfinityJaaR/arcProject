using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador del mapa expandible que permite ver el minimapa en pantalla completa
/// Gestiona las transiciones entre vista compacta y expandida
/// </summary>
public class ExpandableMapController : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Panel del minimapa compacto (esquina)")]
    public GameObject compactMapPanel;
    
    [Tooltip("Panel del mapa expandido (pantalla completa)")]
    public GameObject expandedMapPanel;
    
    [Tooltip("Contenedor del mapa en vista compacta")]
    public RectTransform compactMapContainer;
    
    [Tooltip("Contenedor del mapa en vista expandida")]
    public RectTransform expandedMapContainer;
    
    [Tooltip("Botón para expandir el mapa")]
    public Button expandButton;
    
    [Tooltip("Botón para cerrar el mapa expandido")]
    public Button closeButton;
    
    [Tooltip("Botón para centrar en tu ubicación")]
    public Button centerButton;
    
    [Tooltip("Slider de zoom")]
    public Slider zoomSlider;
    
    [Tooltip("Texto que muestra el nivel de zoom")]
    public TextMeshProUGUI zoomText;
    
    [Header("Configuración de Animación")]
    [Tooltip("Duración de la animación de transición")]
    public float transitionDuration = 0.3f;
    
    [Tooltip("Tipo de easing para la animación")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Configuración de Zoom")]
    [Tooltip("Nivel de zoom mínimo")]
    public float minZoom = 0.5f;
    
    [Tooltip("Nivel de zoom máximo")]
    public float maxZoom = 3.0f;
    
    [Tooltip("Zoom inicial al expandir")]
    public float initialExpandedZoom = 1.5f;
    
    [Header("Configuración de Pan")]
    [Tooltip("Permitir arrastrar el mapa en vista expandida")]
    public bool allowPanning = true;
    
    [Tooltip("Velocidad de pan/arrastre")]
    public float panSpeed = 1.0f;
    
    [Tooltip("Límites de pan (máxima distancia del centro)")]
    public float panLimitDistance = 500f;
    
    // Estado
    private bool isExpanded = false;
    private float currentZoom = 1.0f;
    private Vector2 currentPanOffset = Vector2.zero;
    private bool isPanning = false;
    private Vector2 lastPanPosition;
    
    // Referencias
    private MiniMapController miniMapController;
    private CanvasGroup compactCanvasGroup;
    private CanvasGroup expandedCanvasGroup;
    
    void Start()
    {
        Debug.Log("[ExpandableMapController] ??? Inicializando mapa expandible...");
        
        // Obtener referencias
        miniMapController = FindObjectOfType<MiniMapController>();
        if (miniMapController == null)
        {
            Debug.LogError("[ExpandableMapController] ? No se encontró MiniMapController!");
            enabled = false;
            return;
        }
        
        // Configurar Canvas Groups para animaciones
        SetupCanvasGroups();
        
        // Configurar listeners de botones
        SetupButtons();
        
        // Configurar zoom slider
        SetupZoomSlider();
        
        // Iniciar en vista compacta
        ShowCompactView(instant: true);
        
        Debug.Log("[ExpandableMapController] ? Mapa expandible inicializado");
    }
    
    void Update()
    {
        // Manejar pan/arrastre en vista expandida
        if (isExpanded && allowPanning)
        {
            HandlePanning();
        }
        
        // Actualizar texto de zoom
        UpdateZoomText();
    }
    
    /// <summary>
    /// Configura los Canvas Groups para animaciones
    /// </summary>
    private void SetupCanvasGroups()
    {
        if (compactMapPanel != null)
        {
            compactCanvasGroup = compactMapPanel.GetComponent<CanvasGroup>();
            if (compactCanvasGroup == null)
            {
                compactCanvasGroup = compactMapPanel.AddComponent<CanvasGroup>();
            }
        }
        
        if (expandedMapPanel != null)
        {
            expandedCanvasGroup = expandedMapPanel.GetComponent<CanvasGroup>();
            if (expandedCanvasGroup == null)
            {
                expandedCanvasGroup = expandedMapPanel.AddComponent<CanvasGroup>();
            }
        }
    }
    
    /// <summary>
    /// Configura los listeners de los botones
    /// </summary>
    private void SetupButtons()
    {
        if (expandButton != null)
        {
            expandButton.onClick.AddListener(ExpandMap);
            Debug.Log("[ExpandableMapController] ? Botón expandir configurado");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? expandButton no asignado");
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CollapseMap);
            Debug.Log("[ExpandableMapController] ? Botón cerrar configurado");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? closeButton no asignado");
        }
        
        if (centerButton != null)
        {
            centerButton.onClick.AddListener(CenterOnUserLocation);
            Debug.Log("[ExpandableMapController] ? Botón centrar configurado");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? centerButton no asignado");
        }
    }
    
    /// <summary>
    /// Configura el slider de zoom
    /// </summary>
    private void SetupZoomSlider()
    {
        if (zoomSlider != null)
        {
            zoomSlider.minValue = minZoom;
            zoomSlider.maxValue = maxZoom;
            zoomSlider.value = currentZoom;
            zoomSlider.onValueChanged.AddListener(OnZoomChanged);
            Debug.Log("[ExpandableMapController] ? Slider de zoom configurado");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? zoomSlider no asignado");
        }
    }
    
    /// <summary>
    /// Expande el mapa a pantalla completa
    /// </summary>
    public void ExpandMap()
    {
        if (isExpanded) return;
        
        Debug.Log("[ExpandableMapController] ?? Expandiendo mapa...");
        
        isExpanded = true;
        
        // Mover el contenido del mapa al contenedor expandido
        if (miniMapController != null && miniMapController.mapContainer != null)
        {
            Debug.Log($"[ExpandableMapController] ?? Moviendo mapContainer de {miniMapController.mapContainer.parent.name} a {expandedMapContainer.name}");
            
            // Guardar referencia al mapContainer
            RectTransform mapContainer = miniMapController.mapContainer;
            
            // Mover al contenedor expandido
            mapContainer.SetParent(expandedMapContainer, false);
            
            // Resetear posición y rotación
            mapContainer.anchoredPosition = Vector2.zero;
            mapContainer.localRotation = Quaternion.identity;
            
            // Ajustar escala para la vista expandida
            currentZoom = initialExpandedZoom;
            currentPanOffset = Vector2.zero;
            
            // Aplicar zoom inicial
            ApplyZoomAndPan();
            
            // Actualizar slider
            if (zoomSlider != null)
            {
                zoomSlider.value = currentZoom;
            }
            
            // Asegurar que esté visible
            mapContainer.gameObject.SetActive(true);
            
            Debug.Log("[ExpandableMapController] ? mapContainer movido al contenedor expandido");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? miniMapController o mapContainer es null al expandir");
        }
        
        // Animar transición
        StartCoroutine(AnimateTransition(false, true));
        
        Debug.Log("[ExpandableMapController] ? Mapa expandido");
    }
    
    /// <summary>
    /// Colapsa el mapa a vista compacta
    /// </summary>
    public void CollapseMap()
    {
        if (!isExpanded) return;
        
        Debug.Log("[ExpandableMapController] ?? Colapsando mapa...");
        
        isExpanded = false;
        
        // Resetear zoom y pan ANTES de mover
        currentZoom = 1.0f;
        currentPanOffset = Vector2.zero;
        
        // Mover el contenido del mapa al contenedor compacto
        if (miniMapController != null && miniMapController.mapContainer != null)
        {
            Debug.Log($"[ExpandableMapController] ?? Moviendo mapContainer de {miniMapController.mapContainer.parent.name} a {compactMapContainer.name}");
            
            // Guardar referencia al mapContainer
            RectTransform mapContainer = miniMapController.mapContainer;
            
            // Mover al contenedor compacto
            mapContainer.SetParent(compactMapContainer, false);
            
            // Resetear transformación a valores por defecto
            mapContainer.anchoredPosition = Vector2.zero;
            mapContainer.localScale = Vector3.one;
            mapContainer.localRotation = Quaternion.identity;
            
            // Asegurar que esté visible
            mapContainer.gameObject.SetActive(true);
            
            Debug.Log("[ExpandableMapController] ? mapContainer movido y reseteado al contenedor compacto");
        }
        else
        {
            Debug.LogWarning("[ExpandableMapController] ?? miniMapController o mapContainer es null al colapsar");
        }
        
        // Resetear slider de zoom
        if (zoomSlider != null)
        {
            zoomSlider.value = 1.0f;
        }
        
        // Animar transición
        StartCoroutine(AnimateTransition(true, false));
        
        Debug.Log("[ExpandableMapController] ? Mapa colapsado");
    }
    
    /// <summary>
    /// Anima la transición entre vistas
    /// </summary>
    private IEnumerator AnimateTransition(bool showCompact, bool showExpanded)
    {
        float elapsedTime = 0f;
        
        // Estados iniciales
        float compactStart = showCompact ? 0f : 1f;
        float compactEnd = showCompact ? 1f : 0f;
        float expandedStart = showExpanded ? 0f : 1f;
        float expandedEnd = showExpanded ? 1f : 0f;
        
        // Activar paneles antes de animar
        if (showCompact && compactMapPanel != null)
            compactMapPanel.SetActive(true);
        if (showExpanded && expandedMapPanel != null)
            expandedMapPanel.SetActive(true);
        
        // Animar
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);
            
            if (compactCanvasGroup != null)
            {
                compactCanvasGroup.alpha = Mathf.Lerp(compactStart, compactEnd, t);
            }
            
            if (expandedCanvasGroup != null)
            {
                expandedCanvasGroup.alpha = Mathf.Lerp(expandedStart, expandedEnd, t);
            }
            
            yield return null;
        }
        
        // Estados finales
        if (compactCanvasGroup != null)
            compactCanvasGroup.alpha = compactEnd;
        if (expandedCanvasGroup != null)
            expandedCanvasGroup.alpha = expandedEnd;
        
        // Desactivar paneles que no se usan
        if (!showCompact && compactMapPanel != null)
            compactMapPanel.SetActive(false);
        if (!showExpanded && expandedMapPanel != null)
            expandedMapPanel.SetActive(false);
    }
    
    /// <summary>
    /// Muestra la vista compacta
    /// </summary>
    private void ShowCompactView(bool instant = false)
    {
        if (instant)
        {
            if (compactMapPanel != null)
            {
                compactMapPanel.SetActive(true);
                if (compactCanvasGroup != null)
                    compactCanvasGroup.alpha = 1f;
            }
            
            if (expandedMapPanel != null)
            {
                expandedMapPanel.SetActive(false);
                if (expandedCanvasGroup != null)
                    expandedCanvasGroup.alpha = 0f;
            }
        }
        else
        {
            StartCoroutine(AnimateTransition(true, false));
        }
    }
    
    /// <summary>
    /// Callback cuando cambia el zoom
    /// </summary>
    private void OnZoomChanged(float newZoom)
    {
        currentZoom = newZoom;
        ApplyZoomAndPan();
    }
    
    /// <summary>
    /// Aplica el zoom y pan actual al contenedor del mapa
    /// </summary>
    private void ApplyZoomAndPan()
    {
        if (miniMapController == null || miniMapController.mapContainer == null)
            return;
        
        // Aplicar escala
        miniMapController.mapContainer.localScale = Vector3.one * currentZoom;
        
        // Aplicar offset de pan
        miniMapController.mapContainer.anchoredPosition = currentPanOffset;
    }
    
    /// <summary>
    /// Maneja el pan/arrastre del mapa
    /// </summary>
    private void HandlePanning()
    {
        // Detectar inicio de pan
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                isPanning = true;
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector2 delta = touch.position - lastPanPosition;
                currentPanOffset += delta * panSpeed;
                
                // Limitar pan
                currentPanOffset = Vector2.ClampMagnitude(currentPanOffset, panLimitDistance);
                
                ApplyZoomAndPan();
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }
        }
        
        // Soporte para editor (mouse)
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isPanning = true;
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0) && isPanning)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastPanPosition;
            currentPanOffset += delta * panSpeed;
            
            // Limitar pan
            currentPanOffset = Vector2.ClampMagnitude(currentPanOffset, panLimitDistance);
            
            ApplyZoomAndPan();
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isPanning = false;
        }
        #endif
    }
    
    /// <summary>
    /// Centra el mapa en la ubicación del usuario
    /// </summary>
    public void CenterOnUserLocation()
    {
        Debug.Log("[ExpandableMapController] ?? Centrando en ubicación del usuario...");
        
        // Resetear pan para centrar
        currentPanOffset = Vector2.zero;
        ApplyZoomAndPan();
        
        // Animar el reseteo
        StartCoroutine(AnimateCentering());
    }
    
    /// <summary>
    /// Anima el centrado del mapa
    /// </summary>
    private IEnumerator AnimateCentering()
    {
        Vector2 startOffset = currentPanOffset;
        float elapsedTime = 0f;
        float duration = 0.3f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = transitionCurve.Evaluate(elapsedTime / duration);
            
            currentPanOffset = Vector2.Lerp(startOffset, Vector2.zero, t);
            ApplyZoomAndPan();
            
            yield return null;
        }
        
        currentPanOffset = Vector2.zero;
        ApplyZoomAndPan();
    }
    
    /// <summary>
    /// Actualiza el texto del nivel de zoom
    /// </summary>
    private void UpdateZoomText()
    {
        if (zoomText != null && isExpanded)
        {
            zoomText.text = $"Zoom: {currentZoom:F1}x";
        }
    }
    
    /// <summary>
    /// Alterna entre vista expandida y compacta
    /// </summary>
    public void ToggleExpanded()
    {
        if (isExpanded)
        {
            CollapseMap();
        }
        else
        {
            ExpandMap();
        }
    }
}
