using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente alternativo para detectar clicks en el botón usando detección manual de toques
/// Compatible con New Input System y Old Input Manager
/// </summary>
public class ManualButtonClicker : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private NavigationUIManager uiManager;
    
    [Header("?? Configuración")]
    [Tooltip("Desactivar si causa errores con Input System")]
    public bool enableTouchDetection = false;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        uiManager = FindObjectOfType<NavigationUIManager>();
        
        Debug.Log("[ManualButtonClicker] ? Inicializado en: " + gameObject.name);
        Debug.Log($"[ManualButtonClicker] Canvas: {(parentCanvas != null ? parentCanvas.gameObject.name : "NULL")}");
        Debug.Log($"[ManualButtonClicker] UIManager: {(uiManager != null ? "Encontrado" : "NULL")}");
        
        if (!enableTouchDetection)
        {
            Debug.LogWarning("[ManualButtonClicker] ?? Touch detection desactivada. Este componente está en standby.");
            Debug.LogWarning("[ManualButtonClicker] ?? Si el botón no funciona, activa 'enableTouchDetection' en Inspector");
        }
    }
    
    void Update()
    {
        if (!enableTouchDetection) return;
        
        // Solo intentar usar Input si está disponible
        #if ENABLE_LEGACY_INPUT_MANAGER
        DetectInput();
        #else
        // Si no está disponible, este script queda desactivado
        // Usa ForceButtonWork o el Input System UI Input Module en su lugar
        #endif
    }
    
    #if ENABLE_LEGACY_INPUT_MANAGER
    private void DetectInput()
    {
        // Detectar toques en Android
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                if (IsTouchInsideButton(touch.position))
                {
                    OnButtonClicked();
                }
            }
        }
        
        // Detectar clicks en Editor (para testing)
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (IsTouchInsideButton(Input.mousePosition))
            {
                OnButtonClicked();
            }
        }
        #endif
    }
    #endif
    
    private bool IsTouchInsideButton(Vector2 screenPosition)
    {
        if (rectTransform == null) return false;
        
        Vector2 localPoint;
        Camera cam = parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay 
            ? parentCanvas.worldCamera 
            : null;
            
        bool isInRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            screenPosition,
            cam,
            out localPoint
        );
        
        if (isInRect)
        {
            bool contains = rectTransform.rect.Contains(localPoint);
            if (contains)
            {
                Debug.Log($"[ManualButtonClicker] ?? Toque detectado DENTRO del botón! Position: {screenPosition}");
            }
            return contains;
        }
        
        return false;
    }
    
    private void OnButtonClicked()
    {
        Debug.Log("[ManualButtonClicker] ?? ============================================");
        Debug.Log("[ManualButtonClicker] ?? BOTÓN PRESIONADO VÍA DETECCIÓN MANUAL!");
        Debug.Log("[ManualButtonClicker] ?? ============================================");
        
        if (uiManager == null)
        {
            Debug.LogError("[ManualButtonClicker] ? NavigationUIManager no encontrado!");
            uiManager = FindObjectOfType<NavigationUIManager>();
        }
        
        if (uiManager != null && uiManager.locationSelectionPanel != null)
        {
            Debug.Log("[ManualButtonClicker] ? Abriendo panel de lugares...");
            uiManager.locationSelectionPanel.SetActive(true);
            Debug.Log($"[ManualButtonClicker] ?? Panel activo: {uiManager.locationSelectionPanel.activeSelf}");
        }
        else
        {
            Debug.LogError("[ManualButtonClicker] ? No se pudo abrir el panel!");
        }
    }
}
