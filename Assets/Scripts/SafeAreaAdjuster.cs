using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Herramienta para ajustar el panel considerando la cámara/notch de dispositivos Android
/// </summary>
public class SafeAreaAdjuster : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Ajustar TODOS los Paneles para Notch", false, 52)]
    static void AdjustAllPanelsForNotch()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AJUSTANDO TODOS LOS PANELES PARA NOTCH/CÁMARA");
        Debug.Log("???????????????????????????????????????????????");
        
        // Buscar NavigationUIManager
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null)
        {
            Debug.LogError("? NavigationUIManager no encontrado");
            return;
        }
        
        int panelsAdjusted = 0;
        
        // 1. Ajustar Panel de Selección
        if (uiManager.locationSelectionPanel != null)
        {
            Debug.Log("\n1?? Ajustando Panel de Selección...");
            AdjustSelectionPanel(uiManager.locationSelectionPanel);
            panelsAdjusted++;
        }
        else
        {
            Debug.LogWarning("?? locationSelectionPanel es NULL");
        }
        
        // 2. Ajustar Panel de Navegación Activa
        if (uiManager.navigationActivePanel != null)
        {
            Debug.Log("\n2?? Ajustando Panel de Navegación Activa...");
            AdjustNavigationPanel(uiManager.navigationActivePanel);
            panelsAdjusted++;
        }
        else
        {
            Debug.LogWarning("?? navigationActivePanel es NULL");
        }
        
        Debug.Log("\n???????????????????????????????????????????????");
        Debug.Log($"? {panelsAdjusted} PANELES AJUSTADOS");
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? Cambios aplicados:");
        Debug.Log("   - Panel de selección con padding superior");
        Debug.Log("   - Panel de navegación movido hacia abajo");
        Debug.Log("\n?? Los paneles ahora deberían estar visibles debajo del notch");
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Ajustar Solo Panel de Selección", false, 53)]
    static void AdjustSelectionPanelOnly()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AJUSTANDO PANEL DE SELECCIÓN");
        Debug.Log("???????????????????????????????????????????????");
        
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null || uiManager.locationSelectionPanel == null)
        {
            Debug.LogError("? No se encontró el panel");
            return;
        }
        
        AdjustSelectionPanel(uiManager.locationSelectionPanel);
        
        Debug.Log("? Panel de selección ajustado");
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Ajustar Solo Panel de Navegación Activa", false, 54)]
    static void AdjustNavigationPanelOnly()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AJUSTANDO PANEL DE NAVEGACIÓN ACTIVA");
        Debug.Log("???????????????????????????????????????????????");
        
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null || uiManager.navigationActivePanel == null)
        {
            Debug.LogError("? No se encontró el panel");
            return;
        }
        
        AdjustNavigationPanel(uiManager.navigationActivePanel);
        
        Debug.Log("? Panel de navegación activa ajustado");
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Añadir Safe Area a Ambos Paneles", false, 55)]
    static void AddSafeAreaToAllPanels()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AÑADIENDO SAFE AREA A TODOS LOS PANELES");
        Debug.Log("???????????????????????????????????????????????");
        
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null)
        {
            Debug.LogError("? NavigationUIManager no encontrado");
            return;
        }
        
        int componentsAdded = 0;
        
        // Panel de Selección
        if (uiManager.locationSelectionPanel != null)
        {
            if (AddSafeAreaComponent(uiManager.locationSelectionPanel, 60f))
            {
                componentsAdded++;
                Debug.Log("? SafeAreaPanel añadido a Panel de Selección");
            }
        }
        
        // Panel de Navegación Activa
        if (uiManager.navigationActivePanel != null)
        {
            if (AddSafeAreaComponent(uiManager.navigationActivePanel, 40f))
            {
                componentsAdded++;
                Debug.Log("? SafeAreaPanel añadido a Panel de Navegación");
            }
        }
        
        Debug.Log($"\n? {componentsAdded} componentes SafeAreaPanel añadidos");
        Debug.Log("?? Los paneles se ajustarán automáticamente en runtime");
    }
    
    // Función para ajustar el panel de selección
    static void AdjustSelectionPanel(GameObject panel)
    {
        // Buscar el texto del título
        Transform titleTransform = FindChildByName(panel.transform, "TitleText");
        
        if (titleTransform != null)
        {
            RectTransform titleRect = titleTransform.GetComponent<RectTransform>();
            if (titleRect != null)
            {
                // Mover el título más abajo
                titleRect.offsetMax = new Vector2(titleRect.offsetMax.x, -80);
                Debug.Log("? Título del panel de selección movido");
            }
        }
        
        // Buscar el ScrollView y moverlo hacia abajo
        Transform scrollViewTransform = FindChildByName(panel.transform, "LocationScrollView");
        
        if (scrollViewTransform != null)
        {
            RectTransform scrollRect = scrollViewTransform.GetComponent<RectTransform>();
            if (scrollRect != null)
            {
                scrollRect.offsetMax = new Vector2(scrollRect.offsetMax.x, -130);
                Debug.Log("? ScrollView movido más abajo");
            }
        }
        
        // Añadir padding superior al panel completo
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.offsetMax = new Vector2(panelRect.offsetMax.x, -40);
            Debug.Log("? Padding superior añadido al panel de selección");
        }
        
        EditorUtility.SetDirty(panel);
    }
    
    // Función para ajustar el panel de navegación activa
    static void AdjustNavigationPanel(GameObject panel)
    {
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        if (panelRect == null)
        {
            Debug.LogError("? Panel no tiene RectTransform");
            return;
        }
        
        // El panel está anclado en Top/Stretch
        // Cambiamos el Top offset para moverlo hacia abajo
        
        // Antes: offsetMax.y = 0 (pegado al top)
        // Ahora: offsetMax.y = -50 (50px hacia abajo)
        
        panelRect.offsetMax = new Vector2(panelRect.offsetMax.x, -50);
        
        Debug.Log($"? Panel de navegación movido 50px hacia abajo");
        Debug.Log($"   Antes: Top = 0, Ahora: Top = -50");
        
        // Opcional: Reducir la altura si es necesario
        // panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, 150);
        
        EditorUtility.SetDirty(panel);
    }
    
    // Función para añadir SafeAreaPanel component
    static bool AddSafeAreaComponent(GameObject panel, float topPadding)
    {
        SafeAreaPanel existing = panel.GetComponent<SafeAreaPanel>();
        if (existing != null)
        {
            Debug.LogWarning($"?? {panel.name} ya tiene SafeAreaPanel");
            return false;
        }
        
        SafeAreaPanel safeArea = panel.AddComponent<SafeAreaPanel>();
        safeArea.topPadding = topPadding;
        safeArea.bottomPadding = 0f;
        safeArea.updateEveryFrame = false;
        
        EditorUtility.SetDirty(panel);
        return true;
    }
    
    // Función auxiliar para buscar hijos por nombre
    static Transform FindChildByName(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;
        
        foreach (Transform child in parent)
        {
            Transform result = FindChildByName(child, name);
            if (result != null)
                return result;
        }
        
        return null;
    }
    
    #endif
}

/// <summary>
/// Componente que ajusta automáticamente el panel según el Safe Area del dispositivo
/// Se ajusta en runtime para evitar el notch/cámara
/// </summary>
public class SafeAreaPanel : MonoBehaviour
{
    [Header("?? Configuración")]
    [Tooltip("Padding adicional en la parte superior (para notch)")]
    public float topPadding = 40f;
    
    [Tooltip("Padding adicional en la parte inferior")]
    public float bottomPadding = 0f;
    
    [Tooltip("Actualizar cada frame (desactiva si causa lag)")]
    public bool updateEveryFrame = false;
    
    private RectTransform rectTransform;
    private Rect lastSafeArea;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Start()
    {
        ApplySafeArea();
    }
    
    void Update()
    {
        if (updateEveryFrame)
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea != lastSafeArea)
            {
                ApplySafeArea();
            }
        }
    }
    
    void ApplySafeArea()
    {
        if (rectTransform == null)
            return;
        
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        
        // Calcular el porcentaje del safe area
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        
        // Aplicar anchors
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        
        // Aplicar padding adicional
        rectTransform.offsetMin = new Vector2(0, bottomPadding);
        rectTransform.offsetMax = new Vector2(0, -topPadding);
        
        Debug.Log($"[SafeAreaPanel] Safe Area aplicado a {gameObject.name}");
        Debug.Log($"[SafeAreaPanel] Safe Area: {safeArea}");
        Debug.Log($"[SafeAreaPanel] Top padding: {topPadding}px");
    }
    
    // Método público para forzar actualización
    public void ForceUpdate()
    {
        ApplySafeArea();
    }
}
