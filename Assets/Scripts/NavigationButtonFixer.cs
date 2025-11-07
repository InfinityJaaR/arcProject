using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Script de diagnóstico y reparación automática para el botón de navegación
/// Este script detecta y soluciona todos los problemas comunes que impiden que el botón funcione
/// </summary>
public class NavigationButtonFixer : MonoBehaviour
{
    [Header("?? Auto-Reparación")]
    [Tooltip("Intenta reparar automáticamente los problemas encontrados")]
    public bool autoFix = true;
    
    [Header("?? Diagnóstico")]
    [Tooltip("Muestra logs detallados del diagnóstico")]
    public bool verboseLogging = true;
    
    [Header("Referencias a Verificar")]
    public Button openLocationPanelButton;
    public NavigationUIManager navigationUIManager;
    public Canvas navigationCanvas;
    
    private int errorCount = 0;
    private int fixCount = 0;
    
    void Start()
    {
        Log("?? ============================================");
        Log("?? INICIANDO DIAGNÓSTICO DEL SISTEMA DE NAVEGACIÓN");
        Log("?? ============================================");
        
        // Esperar un frame para que todo se inicialice
        Invoke(nameof(RunDiagnostic), 0.1f);
    }
    
    private void RunDiagnostic()
    {
        errorCount = 0;
        fixCount = 0;
        
        // 1. Verificar EventSystem
        CheckEventSystem();
        
        // 2. Buscar referencias automáticamente si no están asignadas
        FindReferences();
        
        // 3. Verificar Canvas
        CheckCanvas();
        
        // 4. Verificar NavigationUIManager
        CheckNavigationUIManager();
        
        // 5. Verificar Botón
        CheckButton();
        
        // 6. Verificar jerarquía UI
        CheckUIHierarchy();
        
        // Resultado final
        Log("?? ============================================");
        if (errorCount == 0)
        {
            Log("? ¡TODO ESTÁ CORRECTO! El botón debería funcionar.");
        }
        else
        {
            Log($"?? Se encontraron {errorCount} problemas.");
            if (fixCount > 0)
            {
                Log($"?? Se repararon automáticamente {fixCount} problemas.");
                Log("?? Verifica si el botón ahora funciona. Si no, revisa los errores que no se pudieron reparar.");
            }
            else
            {
                Log("? No se activó auto-reparación. Activa 'autoFix' para intentar solucionarlos.");
            }
        }
        Log("?? ============================================");
    }
    
    #region EventSystem
    private void CheckEventSystem()
    {
        Log("\n?? [1/6] Verificando EventSystem...");
        
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        
        if (eventSystem == null)
        {
            LogError("? No se encontró EventSystem en la escena");
            
            if (autoFix)
            {
                CreateEventSystem();
            }
            else
            {
                Log("?? Solución: GameObject ? UI ? Event System");
            }
        }
        else
        {
            Log($"? EventSystem encontrado: {eventSystem.gameObject.name}");
            
            // Verificar que esté activo
            if (!eventSystem.gameObject.activeInHierarchy)
            {
                LogError("? EventSystem está desactivado");
                if (autoFix)
                {
                    eventSystem.gameObject.SetActive(true);
                    Log("?? EventSystem activado");
                    fixCount++;
                }
            }
            
            // Verificar que esté habilitado
            if (!eventSystem.enabled)
            {
                LogError("? EventSystem.enabled = false");
                if (autoFix)
                {
                    eventSystem.enabled = true;
                    Log("?? EventSystem habilitado");
                    fixCount++;
                }
            }
            
            // ? NUEVO: Verificar InputModule
            CheckInputModule(eventSystem);
        }
    }
    
    private void CheckInputModule(EventSystem eventSystem)
    {
        Log("\n?? [1.5/6] Verificando Input Module...");
        
        // Verificar si tiene StandaloneInputModule (antiguo)
        StandaloneInputModule standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
        
        if (standaloneModule != null)
        {
            LogError("? PROBLEMA CRÍTICO: EventSystem usa StandaloneInputModule (sistema antiguo)");
            LogError("   Este módulo NO funciona con el New Input System");
            
            if (autoFix)
            {
                Log("?? Intentando reemplazar con InputSystemUIInputModule...");
                
                // Intentar obtener el tipo del nuevo módulo
                System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                
                if (inputSystemModuleType != null)
                {
                    // Eliminar el antiguo
                    DestroyImmediate(standaloneModule);
                    Log("?? StandaloneInputModule eliminado");
                    
                    // Añadir el nuevo
                    Component newModule = eventSystem.gameObject.AddComponent(inputSystemModuleType);
                    Log("?? InputSystemUIInputModule añadido");
                    
                    fixCount++;
                }
                else
                {
                    LogError("? No se pudo encontrar InputSystemUIInputModule");
                    LogError("?? SOLUCIÓN MANUAL:");
                    LogError("   1. Selecciona EventSystem en la jerarquía");
                    LogError("   2. En Inspector, busca 'Standalone Input Module'");
                    LogError("   3. Click en el botón: 'Replace with InputSystemUIInputModule'");
                    LogError("   O manualmente:");
                    LogError("   1. Remove Component: Standalone Input Module");
                    LogError("   2. Add Component: Input System UI Input Module");
                }
            }
            else
            {
                Log("?? SOLUCIÓN: Reemplaza StandaloneInputModule con InputSystemUIInputModule");
                Log("   1. Selecciona EventSystem");
                Log("   2. Click en 'Replace with InputSystemUIInputModule'");
            }
        }
        else
        {
            // Verificar si tiene el módulo correcto
            Component inputSystemModule = eventSystem.GetComponent("UnityEngine.InputSystem.UI.InputSystemUIInputModule");
            
            if (inputSystemModule != null)
            {
                Log("? InputSystemUIInputModule presente (correcto)");
            }
            else
            {
                LogError("?? No se encontró ningún Input Module");
                Log("?? Esto puede causar problemas con la detección de input");
                
                if (autoFix)
                {
                    Log("?? Intentando añadir InputSystemUIInputModule...");
                    System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                    
                    if (inputSystemModuleType != null)
                    {
                        eventSystem.gameObject.AddComponent(inputSystemModuleType);
                        Log("?? InputSystemUIInputModule añadido");
                        fixCount++;
                    }
                    else
                    {
                        // Fallback: añadir StandaloneInputModule como último recurso
                        eventSystem.gameObject.AddComponent<StandaloneInputModule>();
                        Log("?? StandaloneInputModule añadido como fallback");
                        fixCount++;
                    }
                }
            }
        }
    }
    
    private void CreateEventSystem()
    {
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<EventSystem>();
        
        // Intentar añadir el módulo correcto según el Input System
        System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        
        if (inputSystemModuleType != null)
        {
            eventSystemObj.AddComponent(inputSystemModuleType);
            Log("?? EventSystem creado con InputSystemUIInputModule");
        }
        else
        {
            eventSystemObj.AddComponent<StandaloneInputModule>();
            Log("?? EventSystem creado con StandaloneInputModule (fallback)");
        }
        
        fixCount++;
    }
    #endregion
    
    #region Referencias
    private void FindReferences()
    {
        Log("\n?? [2/6] Buscando referencias...");
        
        // Buscar NavigationUIManager
        if (navigationUIManager == null)
        {
            Log("?? NavigationUIManager no asignado, buscando...");
            navigationUIManager = FindObjectOfType<NavigationUIManager>();
            
            if (navigationUIManager != null)
            {
                Log($"? NavigationUIManager encontrado: {navigationUIManager.gameObject.name}");
            }
            else
            {
                LogError("? NavigationUIManager NO encontrado en la escena");
                Log("?? Solución: Asegúrate de tener NavigationCanvas en la escena con NavigationUIManager");
                return; // No podemos continuar sin esto
            }
        }
        else
        {
            Log($"? NavigationUIManager asignado: {navigationUIManager.gameObject.name}");
        }
        
        // Buscar Canvas
        if (navigationCanvas == null)
        {
            Log("?? Canvas no asignado, buscando...");
            navigationCanvas = navigationUIManager.GetComponentInParent<Canvas>();
            
            if (navigationCanvas == null)
            {
                navigationCanvas = FindObjectOfType<Canvas>();
            }
            
            if (navigationCanvas != null)
            {
                Log($"? Canvas encontrado: {navigationCanvas.gameObject.name}");
            }
            else
            {
                LogError("? Canvas NO encontrado");
            }
        }
        else
        {
            Log($"? Canvas asignado: {navigationCanvas.gameObject.name}");
        }
        
        // Buscar Botón
        if (openLocationPanelButton == null)
        {
            Log("?? Botón no asignado, buscando...");
            
            // Primero buscar en el UIManager
            if (navigationUIManager != null && navigationUIManager.openLocationPanelButton != null)
            {
                openLocationPanelButton = navigationUIManager.openLocationPanelButton;
                Log($"? Botón encontrado en UIManager: {openLocationPanelButton.gameObject.name}");
            }
            else
            {
                // Buscar por nombre
                GameObject buttonObj = GameObject.Find("OpenLocationPanelButton");
                if (buttonObj != null)
                {
                    openLocationPanelButton = buttonObj.GetComponent<Button>();
                    if (openLocationPanelButton != null)
                    {
                        Log($"? Botón encontrado por nombre: {openLocationPanelButton.gameObject.name}");
                    }
                }
                
                // Si aún no lo encontramos, buscar todos los botones
                if (openLocationPanelButton == null)
                {
                    Button[] allButtons = FindObjectsOfType<Button>(true);
                    foreach (Button btn in allButtons)
                    {
                        if (btn.gameObject.name.Contains("Location") || btn.gameObject.name.Contains("Navegar"))
                        {
                            openLocationPanelButton = btn;
                            Log($"? Botón encontrado: {openLocationPanelButton.gameObject.name}");
                            break;
                        }
                    }
                }
            }
            
            if (openLocationPanelButton == null)
            {
                LogError("? Botón NO encontrado");
                Log("?? Solución: Asigna manualmente el botón en el Inspector de este script");
            }
        }
        else
        {
            Log($"? Botón asignado: {openLocationPanelButton.gameObject.name}");
        }
    }
    #endregion
    
    #region Canvas
    private void CheckCanvas()
    {
        if (navigationCanvas == null)
        {
            LogError("? No se puede verificar Canvas (no encontrado)");
            return;
        }
        
        Log("\n?? [3/6] Verificando Canvas...");
        
        // Verificar que esté activo
        if (!navigationCanvas.gameObject.activeInHierarchy)
        {
            LogError("? Canvas está desactivado");
            if (autoFix)
            {
                navigationCanvas.gameObject.SetActive(true);
                Log("?? Canvas activado");
                fixCount++;
            }
        }
        
        // Verificar GraphicRaycaster
        GraphicRaycaster raycaster = navigationCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            LogError("? Canvas no tiene GraphicRaycaster");
            if (autoFix)
            {
                raycaster = navigationCanvas.gameObject.AddComponent<GraphicRaycaster>();
                Log("?? GraphicRaycaster añadido al Canvas");
                fixCount++;
            }
        }
        else
        {
            Log("? GraphicRaycaster presente");
            
            if (!raycaster.enabled)
            {
                LogError("? GraphicRaycaster está deshabilitado");
                if (autoFix)
                {
                    raycaster.enabled = true;
                    Log("?? GraphicRaycaster habilitado");
                    fixCount++;
                }
            }
        }
        
        // Verificar configuración del Canvas
        Log($"?? Canvas Render Mode: {navigationCanvas.renderMode}");
        Log($"?? Canvas Sorting Order: {navigationCanvas.sortingOrder}");
        
        if (navigationCanvas.sortingOrder < 0)
        {
            LogError($"?? Canvas Sorting Order es negativo ({navigationCanvas.sortingOrder})");
            if (autoFix)
            {
                navigationCanvas.sortingOrder = 100;
                Log("?? Canvas Sorting Order cambiado a 100");
                fixCount++;
            }
        }
    }
    #endregion
    
    #region NavigationUIManager
    private void CheckNavigationUIManager()
    {
        if (navigationUIManager == null)
        {
            LogError("? No se puede verificar NavigationUIManager (no encontrado)");
            return;
        }
        
        Log("\n?? [4/6] Verificando NavigationUIManager...");
        
        // Verificar que esté activo
        if (!navigationUIManager.gameObject.activeInHierarchy)
        {
            LogError("? NavigationUIManager está desactivado");
            if (autoFix)
            {
                navigationUIManager.gameObject.SetActive(true);
                Log("?? NavigationUIManager activado");
                fixCount++;
            }
        }
        
        if (!navigationUIManager.enabled)
        {
            LogError("? NavigationUIManager.enabled = false");
            if (autoFix)
            {
                navigationUIManager.enabled = true;
                Log("?? NavigationUIManager habilitado");
                fixCount++;
            }
        }
        
        // Verificar referencias críticas
        int nullReferences = 0;
        
        if (navigationUIManager.locationSelectionPanel == null)
        {
            LogError("? locationSelectionPanel es NULL");
            nullReferences++;
        }
        else
        {
            Log($"? locationSelectionPanel: {navigationUIManager.locationSelectionPanel.name}");
        }
        
        if (navigationUIManager.navigationActivePanel == null)
        {
            LogError("? navigationActivePanel es NULL");
            nullReferences++;
        }
        else
        {
            Log($"? navigationActivePanel: {navigationUIManager.navigationActivePanel.name}");
        }
        
        if (navigationUIManager.locationListContent == null)
        {
            LogError("? locationListContent es NULL");
            nullReferences++;
        }
        else
        {
            Log($"? locationListContent: {navigationUIManager.locationListContent.name}");
        }
        
        if (navigationUIManager.locationButtonPrefab == null)
        {
            LogError("? locationButtonPrefab es NULL");
            nullReferences++;
        }
        else
        {
            Log($"? locationButtonPrefab: {navigationUIManager.locationButtonPrefab.name}");
        }
        
        if (navigationUIManager.openLocationPanelButton == null)
        {
            LogError("? openLocationPanelButton es NULL en NavigationUIManager");
            nullReferences++;
            
            // Intentar asignar el botón que encontramos
            if (autoFix && openLocationPanelButton != null)
            {
                navigationUIManager.openLocationPanelButton = openLocationPanelButton;
                Log("?? openLocationPanelButton asignado en NavigationUIManager");
                fixCount++;
                nullReferences--;
            }
        }
        else
        {
            Log($"? openLocationPanelButton: {navigationUIManager.openLocationPanelButton.gameObject.name}");
        }
        
        if (nullReferences > 0)
        {
            Log($"?? Necesitas asignar {nullReferences} referencias en el Inspector de NavigationUIManager");
        }
    }
    #endregion
    
    #region Botón
    private void CheckButton()
    {
        if (openLocationPanelButton == null)
        {
            LogError("? No se puede verificar botón (no encontrado)");
            return;
        }
        
        Log("\n?? [5/6] Verificando Botón...");
        
        // Verificar que esté activo
        if (!openLocationPanelButton.gameObject.activeInHierarchy)
        {
            LogError("? Botón está desactivado");
            if (autoFix)
            {
                openLocationPanelButton.gameObject.SetActive(true);
                Log("?? Botón activado");
                fixCount++;
            }
        }
        
        // Verificar interactable
        if (!openLocationPanelButton.interactable)
        {
            LogError("? Botón NO es interactable");
            if (autoFix)
            {
                openLocationPanelButton.interactable = true;
                Log("?? Botón hecho interactable");
                fixCount++;
            }
        }
        else
        {
            Log("? Botón es interactable");
        }
        
        // Verificar raycast target
        Image buttonImage = openLocationPanelButton.GetComponent<Image>();
        if (buttonImage != null && !buttonImage.raycastTarget)
        {
            LogError("? La imagen del botón no tiene raycastTarget");
            if (autoFix)
            {
                buttonImage.raycastTarget = true;
                Log("?? raycastTarget activado en la imagen del botón");
                fixCount++;
            }
        }
        
        // Verificar listeners
        int listenerCount = openLocationPanelButton.onClick.GetPersistentEventCount();
        Log($"?? Listeners persistentes: {listenerCount}");
        
        // Contar listeners runtime (esto es aproximado)
        if (openLocationPanelButton.onClick != null)
        {
            Log("? onClick event está inicializado");
        }
        
        // Verificar RectTransform
        RectTransform rectTransform = openLocationPanelButton.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Log($"?? Posición: {rectTransform.position}");
            Log($"?? Tamaño: {rectTransform.sizeDelta}");
            
            // Verificar que el tamaño no sea 0
            if (rectTransform.sizeDelta.x <= 0 || rectTransform.sizeDelta.y <= 0)
            {
                LogError($"? El botón tiene tamaño inválido: {rectTransform.sizeDelta}");
                if (autoFix)
                {
                    rectTransform.sizeDelta = new Vector2(180, 180);
                    Log("?? Tamaño del botón ajustado a 180x180");
                    fixCount++;
                }
            }
        }
        
        // Verificar CanvasGroup (puede estar bloqueando)
        CanvasGroup[] canvasGroups = openLocationPanelButton.GetComponentsInParent<CanvasGroup>();
        foreach (CanvasGroup cg in canvasGroups)
        {
            if (!cg.interactable)
            {
                LogError($"? CanvasGroup '{cg.gameObject.name}' está bloqueando interacción");
                if (autoFix)
                {
                    cg.interactable = true;
                    Log($"?? CanvasGroup '{cg.gameObject.name}' hecho interactable");
                    fixCount++;
                }
            }
            
            if (cg.blocksRaycasts == false)
            {
                LogError($"? CanvasGroup '{cg.gameObject.name}' está bloqueando raycasts");
                if (autoFix)
                {
                    cg.blocksRaycasts = true;
                    Log($"?? CanvasGroup '{cg.gameObject.name}' ahora permite raycasts");
                    fixCount++;
                }
            }
        }
    }
    #endregion
    
    #region UI Hierarchy
    private void CheckUIHierarchy()
    {
        if (openLocationPanelButton == null)
        {
            return;
        }
        
        Log("\n?? [6/6] Verificando jerarquía UI...");
        
        // Verificar que el botón esté dentro del Canvas
        Canvas parentCanvas = openLocationPanelButton.GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            LogError("? El botón NO está dentro de un Canvas");
            Log("?? Solución: Mueve el botón dentro del Canvas en la jerarquía");
        }
        else
        {
            Log($"? Botón está dentro del Canvas: {parentCanvas.gameObject.name}");
        }
        
        // Verificar orden de sorting
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        Log($"?? Hay {allCanvases.Length} Canvas en la escena");
        
        foreach (Canvas canvas in allCanvases)
        {
            Log($"   - {canvas.gameObject.name}: Order {canvas.sortingOrder}, Active: {canvas.gameObject.activeInHierarchy}");
        }
    }
    #endregion
    
    #region Logging
    private void Log(string message)
    {
        if (verboseLogging)
        {
            Debug.Log($"[NavigationButtonFixer] {message}");
        }
    }
    
    private void LogError(string message)
    {
        errorCount++;
        Debug.LogError($"[NavigationButtonFixer] {message}");
    }
    #endregion
    
    #region Testing
    void Update()
    {
        // Atajo para re-ejecutar el diagnóstico
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Log("?? Re-ejecutando diagnóstico (F5)...");
            RunDiagnostic();
        }
        
        // Atajo para testear el botón manualmente
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Log("?? Testeando botón manualmente (F6)...");
            TestButtonManually();
        }
    }
    
    private void TestButtonManually()
    {
        if (openLocationPanelButton == null)
        {
            LogError("? No se puede testear: botón no encontrado");
            return;
        }
        
        if (navigationUIManager == null)
        {
            LogError("? No se puede testear: NavigationUIManager no encontrado");
            return;
        }
        
        Log("?? Invocando onClick del botón...");
        openLocationPanelButton.onClick.Invoke();
        
        Log($"?? locationSelectionPanel activo: {navigationUIManager.locationSelectionPanel != null && navigationUIManager.locationSelectionPanel.activeSelf}");
    }
    #endregion
}
