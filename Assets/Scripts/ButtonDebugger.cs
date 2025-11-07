using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script de diagnóstico para verificar que el botón de navegación funcione
/// Adjunta esto temporalmente al botón OpenLocationPanelButton para debug
/// </summary>
public class ButtonDebugger : MonoBehaviour
{
    private Button button;
    private Canvas parentCanvas;
    
    void Start()
    {
        button = GetComponent<Button>();
        
        if (button == null)
        {
            Debug.LogError("[ButtonDebugger] ? No hay componente Button en este GameObject!");
            return;
        }
        
        Debug.Log($"[ButtonDebugger] ? Button encontrado en '{gameObject.name}'");
        Debug.Log($"[ButtonDebugger] Interactable: {button.interactable}");
        Debug.Log($"[ButtonDebugger] Listeners actuales: {button.onClick.GetPersistentEventCount()}");
        
        // Verificar Canvas
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            Debug.Log($"[ButtonDebugger] ?? Canvas: {parentCanvas.gameObject.name}");
            Debug.Log($"[ButtonDebugger] ?? Canvas RenderMode: {parentCanvas.renderMode}");
            Debug.Log($"[ButtonDebugger] ?? Canvas SortingOrder: {parentCanvas.sortingOrder}");
            Debug.Log($"[ButtonDebugger] ?? Canvas OverrideSorting: {parentCanvas.overrideSorting}");
            
            // Forzar Canvas a estar encima de todo
            parentCanvas.sortingOrder = 1000;
            Debug.Log($"[ButtonDebugger] ?? Canvas SortingOrder cambiado a: 1000");
        }
        
        // Verificar RectTransform y posición
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Debug.Log($"[ButtonDebugger] ?? Position: {rectTransform.position}");
            Debug.Log($"[ButtonDebugger] ?? AnchoredPosition: {rectTransform.anchoredPosition}");
            Debug.Log($"[ButtonDebugger] ?? SizeDelta: {rectTransform.sizeDelta}");
        }
        
        // Verificar GraphicRaycaster
        GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
        if (raycaster != null)
        {
            Debug.Log($"[ButtonDebugger] ? GraphicRaycaster encontrado - Enabled: {raycaster.enabled}");
        }
        else
        {
            Debug.LogError("[ButtonDebugger] ? NO se encontró GraphicRaycaster!");
        }
        
        // Verificar EventSystem
        UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.EventSystems.EventSystem.current;
        if (eventSystem != null)
        {
            Debug.Log($"[ButtonDebugger] ? EventSystem encontrado: {eventSystem.gameObject.name}");
        }
        else
        {
            Debug.LogError("[ButtonDebugger] ? NO se encontró EventSystem!");
        }
        
        // Añadir listener de prueba
        button.onClick.AddListener(OnButtonClicked);
        Debug.Log("[ButtonDebugger] ? Listener de test añadido");
        
        // NUEVO: Buscar NavigationUIManager
        Debug.Log("[ButtonDebugger] ?? Buscando NavigationUIManager en la escena...");
        NavigationUIManager[] allManagers = FindObjectsOfType<NavigationUIManager>();
        Debug.Log($"[ButtonDebugger] ?? NavigationUIManagers encontrados: {allManagers.Length}");
        
        for (int i = 0; i < allManagers.Length; i++)
        {
            Debug.Log($"[ButtonDebugger]    [{i}] {allManagers[i].gameObject.name} - Active: {allManagers[i].gameObject.activeInHierarchy} - Enabled: {allManagers[i].enabled}");
        }
    }
    
    void Update()
    {
        // Detectar toques en pantalla
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log($"[ButtonDebugger] ?? TOQUE DETECTADO en posición: {touch.position}");
                
                // Verificar si el toque está sobre el botón
                RectTransform rectTransform = GetComponent<RectTransform>();
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform, 
                    touch.position, 
                    parentCanvas != null ? parentCanvas.worldCamera : null, 
                    out localPoint))
                {
                    bool isInside = rectTransform.rect.Contains(localPoint);
                    Debug.Log($"[ButtonDebugger] ?? Toque en rectángulo del botón: {(isInside ? "SÍ ?" : "NO ?")}");
                    Debug.Log($"[ButtonDebugger] ?? LocalPoint: {localPoint}, Rect: {rectTransform.rect}");
                }
            }
        }
    }
    
    private void OnButtonClicked()
    {
        Debug.Log("[ButtonDebugger] ?? ============================================");
        Debug.Log("[ButtonDebugger] ?? ¡BOTÓN PRESIONADO! El botón SÍ está funcionando.");
        Debug.Log("[ButtonDebugger] ?? ============================================");
        Debug.Log($"[ButtonDebugger] Hora: {System.DateTime.Now:HH:mm:ss}");
        
        // Verificar NavigationUIManager
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        if (uiManager == null)
        {
            Debug.LogError("[ButtonDebugger] ? NavigationUIManager NO ENCONTRADO en la escena!");
            Debug.LogError("[ButtonDebugger] ?? SOLUCIÓN: Verifica que NavigationCanvas esté en la escena");
        }
        else
        {
            Debug.Log("[ButtonDebugger] ? NavigationUIManager encontrado");
            Debug.Log($"[ButtonDebugger] ?? GameObject: {uiManager.gameObject.name}");
            Debug.Log($"[ButtonDebugger] ?? Active: {uiManager.gameObject.activeInHierarchy}");
            Debug.Log($"[ButtonDebugger] ?? Enabled: {uiManager.enabled}");
            
            // Verificar referencias
            if (uiManager.locationSelectionPanel == null)
                Debug.LogError("[ButtonDebugger] ? locationSelectionPanel es NULL");
            else
                Debug.Log($"[ButtonDebugger] ? locationSelectionPanel: {uiManager.locationSelectionPanel.name}");
                
            if (uiManager.locationButtonPrefab == null)
                Debug.LogError("[ButtonDebugger] ? locationButtonPrefab es NULL");
            else
                Debug.Log($"[ButtonDebugger] ? locationButtonPrefab: {uiManager.locationButtonPrefab.name}");
                
            if (uiManager.openLocationPanelButton == null)
                Debug.LogError("[ButtonDebugger] ? openLocationPanelButton es NULL");
            else
                Debug.Log($"[ButtonDebugger] ? openLocationPanelButton: {uiManager.openLocationPanelButton.gameObject.name}");
        }
        
        // Verificar FirebaseManager
        if (FirebaseManager.Instance == null)
        {
            Debug.LogWarning("[ButtonDebugger] ?? FirebaseManager.Instance es NULL");
        }
        else
        {
            Debug.Log("[ButtonDebugger] ? FirebaseManager encontrado");
        }
        
        // Intentar forzar la acción manualmente
        if (uiManager != null && uiManager.locationSelectionPanel != null)
        {
            Debug.Log("[ButtonDebugger] ?? Intentando abrir panel manualmente...");
            uiManager.locationSelectionPanel.SetActive(true);
            Debug.Log($"[ButtonDebugger] ?? Panel ahora está: {(uiManager.locationSelectionPanel.activeSelf ? "ACTIVO ?" : "INACTIVO ?")}");
        }
    }
}
