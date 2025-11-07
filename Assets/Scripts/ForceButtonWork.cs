using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script de emergencia que se añade directamente al botón problemático
/// Fuerza el funcionamiento del botón de múltiples maneras
/// </summary>
[RequireComponent(typeof(Button))]
public class ForceButtonWork : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    private Button button;
    private NavigationUIManager uiManager;
    private bool buttonPressed = false;
    
    [Header("?? Configuración")]
    [Tooltip("Usar detección manual de toques como respaldo")]
    public bool useManualTouchDetection = false; // DESACTIVADO por defecto
    
    [Tooltip("Usar interfaces IPointer como respaldo")]
    public bool usePointerInterfaces = true;
    
    [Tooltip("Forzar interactable cada frame")]
    public bool forceInteractable = true;
    
    [Tooltip("Mostrar logs detallados")]
    public bool debugLogs = true;
    
    void Awake()
    {
        button = GetComponent<Button>();
        
        if (button == null)
        {
            LogError("? No se encontró componente Button!");
            return;
        }
        
        Log("? ForceButtonWork inicializado");
    }
    
    void Start()
    {
        // Buscar NavigationUIManager
        uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null)
        {
            LogError("? NavigationUIManager no encontrado en la escena!");
        }
        else
        {
            Log($"? NavigationUIManager encontrado: {uiManager.gameObject.name}");
        }
        
        // Añadir listener al botón
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
            Log("? Listener añadido al botón");
        }
        
        // Verificar configuración
        VerifyConfiguration();
    }
    
    void Update()
    {
        if (button == null) return;
        
        // Forzar que el botón sea interactable
        if (forceInteractable && !button.interactable)
        {
            button.interactable = true;
            Log("?? Botón forzado a ser interactable");
        }
        
        // Detección manual de toques (respaldo)
        if (useManualTouchDetection)
        {
            DetectManualTouch();
        }
    }
    
    #region Button Click Handlers
    
    /// <summary>
    /// Handler principal del botón (método tradicional)
    /// </summary>
    private void OnButtonClick()
    {
        if (buttonPressed)
        {
            Log("?? Click ya procesado en este frame, ignorando...");
            return;
        }
        
        buttonPressed = true;
        
        Log("?? ============================================");
        Log("?? BOTÓN PRESIONADO (OnButtonClick)");
        Log("?? ============================================");
        
        ExecuteButtonAction();
        
        // Reset flag después de un frame
        Invoke(nameof(ResetButtonPressed), 0.1f);
    }
    
    /// <summary>
    /// Handler de IPointerClickHandler (método alternativo)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!usePointerInterfaces) return;
        
        if (buttonPressed)
        {
            Log("?? Click ya procesado en este frame, ignorando...");
            return;
        }
        
        buttonPressed = true;
        
        Log("?? ============================================");
        Log("?? BOTÓN PRESIONADO (OnPointerClick)");
        Log("?? ============================================");
        
        ExecuteButtonAction();
        
        Invoke(nameof(ResetButtonPressed), 0.1f);
    }
    
    /// <summary>
    /// Handler de IPointerDownHandler (método alternativo 2)
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!usePointerInterfaces) return;
        
        Log("?? Pointer Down detectado");
    }
    
    /// <summary>
    /// Detección manual de toques (último recurso)
    /// </summary>
    private void DetectManualTouch()
    {
        // Solo si está habilitado y el Input Manager está disponible
        #if !ENABLE_LEGACY_INPUT_MANAGER
        return; // No disponible con New Input System puro
        #else
        
        bool touchDetected = false;
        Vector2 touchPosition = Vector2.zero;
        
        // Detectar toques en Android/iOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchDetected = true;
                touchPosition = touch.position;
            }
        }
        
        // Detectar clicks en Editor
        #if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            touchDetected = true;
            touchPosition = Input.mousePosition;
        }
        #endif
        
        if (touchDetected && IsTouchInsideButton(touchPosition))
        {
            if (buttonPressed)
            {
                return; // Ya se procesó en este frame
            }
            
            buttonPressed = true;
            
            Log("?? ============================================");
            Log("?? BOTÓN PRESIONADO (Detección Manual)");
            Log("?? ============================================");
            
            ExecuteButtonAction();
            
            Invoke(nameof(ResetButtonPressed), 0.1f);
        }
        #endif
    }
    
    /// <summary>
    /// Verifica si un toque está dentro del área del botón
    /// </summary>
    private bool IsTouchInsideButton(Vector2 screenPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null) return false;
        
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Camera cam = null;
        
        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cam = parentCanvas.worldCamera;
        }
        
        Vector2 localPoint;
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
                Log($"?? Toque detectado DENTRO del botón! Posición: {screenPosition}");
            }
            return contains;
        }
        
        return false;
    }
    
    #endregion
    
    #region Action Execution
    
    /// <summary>
    /// Ejecuta la acción principal del botón
    /// </summary>
    private void ExecuteButtonAction()
    {
        if (uiManager == null)
        {
            LogError("? No se puede ejecutar acción: NavigationUIManager es NULL");
            
            // Intentar encontrarlo de nuevo
            uiManager = FindObjectOfType<NavigationUIManager>();
            
            if (uiManager == null)
            {
                LogError("? NavigationUIManager sigue siendo NULL después de buscar");
                return;
            }
            
            Log("? NavigationUIManager encontrado en segundo intento");
        }
        
        if (uiManager.locationSelectionPanel == null)
        {
            LogError("? locationSelectionPanel es NULL");
            return;
        }
        
        // ABRIR EL PANEL
        Log($"?? Estado actual del panel: {uiManager.locationSelectionPanel.activeSelf}");
        
        uiManager.locationSelectionPanel.SetActive(true);
        
        Log($"?? Nuevo estado del panel: {uiManager.locationSelectionPanel.activeSelf}");
        Log("? Panel de lugares abierto exitosamente");
        
        // Verificar si necesita cargar lugares
        if (uiManager.locationListContent != null)
        {
            int childCount = uiManager.locationListContent.childCount;
            Log($"?? Lugares en la lista: {childCount}");
            
            if (childCount == 0)
            {
                Log("?? Lista vacía, intentando cargar lugares...");
                uiManager.LoadAvailableLocations();
            }
        }
    }
    
    #endregion
    
    #region Verification
    
    /// <summary>
    /// Verifica la configuración del botón al iniciar
    /// </summary>
    private void VerifyConfiguration()
    {
        Log("\n?? Verificando configuración del botón...");
        
        // Verificar Button
        if (button == null)
        {
            LogError("? Componente Button es NULL");
            return;
        }
        
        Log($"? Button encontrado: {button.gameObject.name}");
        Log($"   - Interactable: {button.interactable}");
        Log($"   - Navigation: {button.navigation.mode}");
        
        // Verificar Image (raycast target)
        Image image = button.GetComponent<Image>();
        if (image != null)
        {
            Log($"   - Image.raycastTarget: {image.raycastTarget}");
            if (!image.raycastTarget)
            {
                LogError("?? Image.raycastTarget está en false - esto puede bloquear clicks");
                image.raycastTarget = true;
                Log("?? raycastTarget activado");
            }
        }
        
        // Verificar Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            LogError("? Botón no está dentro de un Canvas");
        }
        else
        {
            Log($"? Canvas encontrado: {canvas.gameObject.name}");
            Log($"   - Render Mode: {canvas.renderMode}");
            Log($"   - Sorting Order: {canvas.sortingOrder}");
            
            // Verificar GraphicRaycaster
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                LogError("? Canvas no tiene GraphicRaycaster");
            }
            else
            {
                Log($"   - GraphicRaycaster.enabled: {raycaster.enabled}");
            }
        }
        
        // Verificar EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            LogError("? No hay EventSystem en la escena");
        }
        else
        {
            Log($"? EventSystem encontrado: {eventSystem.gameObject.name}");
            Log($"   - Enabled: {eventSystem.enabled}");
        }
        
        // Verificar RectTransform
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Log($"?? RectTransform:");
            Log($"   - Position: {rectTransform.position}");
            Log($"   - Size: {rectTransform.sizeDelta}");
            Log($"   - Anchors: Min({rectTransform.anchorMin}) Max({rectTransform.anchorMax})");
        }
        
        Log("?? Verificación completada\n");
    }
    
    #endregion
    
    #region Utilities
    
    private void ResetButtonPressed()
    {
        buttonPressed = false;
    }
    
    private void Log(string message)
    {
        if (debugLogs)
        {
            Debug.Log($"[ForceButtonWork] {message}");
        }
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[ForceButtonWork] {message}");
    }
    
    #endregion
    
    #region On-Screen Debug
    
    void OnGUI()
    {
        if (!debugLogs) return;
        
        // Mostrar estado en pantalla
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 20;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        
        string status = "?? ForceButtonWork\n";
        status += $"Button: {(button != null ? "?" : "?")}\n";
        status += $"UIManager: {(uiManager != null ? "?" : "?")}\n";
        status += $"Interactable: {(button != null ? button.interactable : false)}\n";
        
        GUI.Box(new Rect(10, Screen.height - 150, 300, 140), status, style);
    }
    
    #endregion
}
