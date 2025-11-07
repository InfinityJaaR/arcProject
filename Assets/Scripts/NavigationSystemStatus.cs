using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Panel de información en pantalla que muestra el estado completo del sistema de navegación
/// Útil para debugging en dispositivos Android donde no se puede ver la Console
/// </summary>
public class NavigationSystemStatus : MonoBehaviour
{
    [Header("?? Configuración")]
    [Tooltip("Tecla para mostrar/ocultar el panel")]
    public KeyCode toggleKey = KeyCode.I;
    
    [Tooltip("Mostrar al iniciar")]
    public bool showOnStart = true;
    
    [Tooltip("Actualizar cada X segundos")]
    public float updateInterval = 0.5f;
    
    [Header("?? Apariencia")]
    public int fontSize = 16;
    public Color backgroundColor = new Color(0, 0, 0, 0.8f);
    public Color textColor = Color.white;
    
    private bool isVisible = true;
    private float lastUpdateTime;
    private StringBuilder statusText = new StringBuilder();
    
    // Referencias
    private NavigationUIManager uiManager;
    private AppModeManager appModeManager;
    private EventSystem eventSystem;
    private Canvas[] allCanvases;
    private Button openButton;
    
    void Start()
    {
        isVisible = showOnStart;
        FindReferences();
    }
    
    void Update()
    {
        // Toggle visibility
        if (Input.GetKeyDown(toggleKey))
        {
            isVisible = !isVisible;
        }
        
        // Update status
        if (Time.time - lastUpdateTime > updateInterval)
        {
            lastUpdateTime = Time.time;
            UpdateStatus();
        }
    }
    
    void OnGUI()
    {
        if (!isVisible) return;
        
        // Background box
        GUI.backgroundColor = backgroundColor;
        
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.fontSize = fontSize;
        boxStyle.normal.textColor = textColor;
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.wordWrap = true;
        
        // Calculate size
        float width = Screen.width * 0.95f;
        float height = Screen.height * 0.8f;
        float x = Screen.width * 0.025f;
        float y = Screen.height * 0.1f;
        
        GUI.Box(new Rect(x, y, width, height), statusText.ToString(), boxStyle);
        
        // Toggle button
        if (GUI.Button(new Rect(10, 10, 100, 40), isVisible ? "Ocultar [I]" : "Mostrar [I]"))
        {
            isVisible = !isVisible;
        }
    }
    
    private void FindReferences()
    {
        uiManager = FindObjectOfType<NavigationUIManager>();
        appModeManager = FindObjectOfType<AppModeManager>();
        eventSystem = FindObjectOfType<EventSystem>();
        allCanvases = FindObjectsOfType<Canvas>();
        
        if (uiManager != null && uiManager.openLocationPanelButton != null)
        {
            openButton = uiManager.openLocationPanelButton;
        }
        else
        {
            // Buscar el botón por nombre
            GameObject buttonObj = GameObject.Find("OpenLocationPanelButton");
            if (buttonObj != null)
            {
                openButton = buttonObj.GetComponent<Button>();
            }
        }
    }
    
    private void UpdateStatus()
    {
        statusText.Clear();
        
        statusText.AppendLine("?? ESTADO DEL SISTEMA DE NAVEGACIÓN");
        statusText.AppendLine("???????????????????????????????????????");
        statusText.AppendLine();
        
        // EventSystem
        statusText.AppendLine("?? EVENTSYSTEM:");
        if (eventSystem == null)
        {
            statusText.AppendLine("? NO ENCONTRADO");
        }
        else
        {
            statusText.AppendLine($"? Encontrado: {eventSystem.gameObject.name}");
            statusText.AppendLine($"   Active: {eventSystem.gameObject.activeInHierarchy}");
            statusText.AppendLine($"   Enabled: {eventSystem.enabled}");
        }
        statusText.AppendLine();
        
        // NavigationUIManager
        statusText.AppendLine("?? NAVIGATION UI MANAGER:");
        if (uiManager == null)
        {
            statusText.AppendLine("? NO ENCONTRADO");
        }
        else
        {
            statusText.AppendLine($"? Encontrado: {uiManager.gameObject.name}");
            statusText.AppendLine($"   Active: {uiManager.gameObject.activeInHierarchy}");
            statusText.AppendLine($"   Enabled: {uiManager.enabled}");
            statusText.AppendLine($"   Location Panel: {(uiManager.locationSelectionPanel != null ? "?" : "?")}");
            if (uiManager.locationSelectionPanel != null)
            {
                statusText.AppendLine($"      Active: {uiManager.locationSelectionPanel.activeSelf}");
            }
            statusText.AppendLine($"   Nav Panel: {(uiManager.navigationActivePanel != null ? "?" : "?")}");
            statusText.AppendLine($"   Button: {(uiManager.openLocationPanelButton != null ? "?" : "?")}");
            statusText.AppendLine($"   Prefab: {(uiManager.locationButtonPrefab != null ? "?" : "?")}");
        }
        statusText.AppendLine();
        
        // AppModeManager
        statusText.AppendLine("?? APP MODE MANAGER:");
        if (appModeManager == null)
        {
            statusText.AppendLine("? NO ENCONTRADO");
        }
        else
        {
            statusText.AppendLine($"? Encontrado: {appModeManager.gameObject.name}");
            statusText.AppendLine($"   Modo actual: {appModeManager.CurrentMode}");
        }
        statusText.AppendLine();
        
        // Button
        statusText.AppendLine("?? BOTÓN NAVEGAR:");
        if (openButton == null)
        {
            statusText.AppendLine("? NO ENCONTRADO");
        }
        else
        {
            statusText.AppendLine($"? Encontrado: {openButton.gameObject.name}");
            statusText.AppendLine($"   Active: {openButton.gameObject.activeInHierarchy}");
            statusText.AppendLine($"   Interactable: {openButton.interactable}");
            statusText.AppendLine($"   Listeners: {openButton.onClick.GetPersistentEventCount()}");
            
            Image image = openButton.GetComponent<Image>();
            if (image != null)
            {
                statusText.AppendLine($"   Image.raycastTarget: {image.raycastTarget}");
            }
            
            RectTransform rect = openButton.GetComponent<RectTransform>();
            if (rect != null)
            {
                statusText.AppendLine($"   Position: {rect.position}");
                statusText.AppendLine($"   Size: {rect.sizeDelta}");
            }
            
            // Verificar scripts adicionales
            ForceButtonWork forceWork = openButton.GetComponent<ForceButtonWork>();
            statusText.AppendLine($"   ForceButtonWork: {(forceWork != null ? "?" : "?")}");
            
            ManualButtonClicker manualClick = openButton.GetComponent<ManualButtonClicker>();
            statusText.AppendLine($"   ManualButtonClicker: {(manualClick != null ? "?" : "?")}");
            
            ButtonDebugger debugger = openButton.GetComponent<ButtonDebugger>();
            statusText.AppendLine($"   ButtonDebugger: {(debugger != null ? "?" : "?")}");
        }
        statusText.AppendLine();
        
        // Canvas
        statusText.AppendLine("?? CANVAS EN LA ESCENA:");
        if (allCanvases == null || allCanvases.Length == 0)
        {
            statusText.AppendLine("? NO SE ENCONTRARON CANVAS");
        }
        else
        {
            statusText.AppendLine($"Total: {allCanvases.Length}");
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas == null) continue;
                
                statusText.AppendLine($"   • {canvas.gameObject.name}");
                statusText.AppendLine($"      Active: {canvas.gameObject.activeInHierarchy}");
                statusText.AppendLine($"      Render Mode: {canvas.renderMode}");
                statusText.AppendLine($"      Sorting Order: {canvas.sortingOrder}");
                
                GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
                statusText.AppendLine($"      GraphicRaycaster: {(raycaster != null ? "?" : "?")}");
                if (raycaster != null)
                {
                    statusText.AppendLine($"         Enabled: {raycaster.enabled}");
                }
            }
        }
        statusText.AppendLine();
        
        // Otros managers
        statusText.AppendLine("?? OTROS MANAGERS:");
        
        LocationManager locationMgr = FindObjectOfType<LocationManager>();
        statusText.AppendLine($"LocationManager: {(locationMgr != null ? "?" : "?")}");
        
        NavigationArrowController navArrow = FindObjectOfType<NavigationArrowController>();
        statusText.AppendLine($"NavigationArrowController: {(navArrow != null ? "?" : "?")}");
        
        FirebaseManager firebase = FindObjectOfType<FirebaseManager>();
        statusText.AppendLine($"FirebaseManager: {(firebase != null ? "?" : "?")}");
        
        statusText.AppendLine();
        statusText.AppendLine("???????????????????????????????????????");
        statusText.AppendLine($"Presiona [{toggleKey}] para ocultar/mostrar");
        statusText.AppendLine($"FPS: {(1f / Time.deltaTime):F0}");
    }
}
