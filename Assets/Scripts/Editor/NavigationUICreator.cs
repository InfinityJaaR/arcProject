using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script que crea automáticamente toda la UI de navegación con un solo click
/// Ejecuta desde: Hierarchy > Click derecho > Navigation > Create Complete UI
/// </summary>
public class NavigationUICreator : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("GameObject/Navigation/Create Complete Navigation UI", false, 10)]
    public static void CreateCompleteNavigationUI()
    {
        Debug.Log("[NavigationUICreator] ?? Creando UI completa de navegación...");
        
        // 1. Crear Canvas principal
        GameObject canvasObj = CreateCanvas();
        
        // 2. Crear Panel de Selección de Lugares
        GameObject locationPanel = CreateLocationSelectionPanel(canvasObj.transform);
        
        // 3. Crear Panel de Navegación Activa
        GameObject navPanel = CreateNavigationActivePanel(canvasObj.transform);
        
        // 4. Crear Botón Principal
        GameObject mainButton = CreateMainButton(canvasObj.transform);
        
        // 5. Añadir y configurar NavigationUIManager
        NavigationUIManager uiManager = canvasObj.GetComponent<NavigationUIManager>();
        if (uiManager == null)
        {
            uiManager = canvasObj.AddComponent<NavigationUIManager>();
        }
        ConfigureUIManager(uiManager, locationPanel, navPanel, mainButton);
        
        // 6. Ocultar paneles por defecto
        locationPanel.SetActive(false);
        navPanel.SetActive(false);
        
        // 7. Seleccionar el canvas creado
        Selection.activeGameObject = canvasObj;
        
        Debug.Log("[NavigationUICreator] ? UI de navegación creada exitosamente!");
        Debug.Log("[NavigationUICreator] ?? Los paneles están ocultos por defecto");
        
        EditorUtility.DisplayDialog(
            "? UI Creada Exitosamente",
            "La UI de navegación ha sido creada correctamente.\n\n" +
            "?? SIGUIENTE PASO:\n\n" +
            "1. Selecciona 'NavigationCanvas' en Hierarchy\n\n" +
            "2. En el Inspector, busca 'Navigation UI Manager'\n\n" +
            "3. Arrastra el prefab 'LocationButton.prefab' desde:\n" +
            "   Assets/Prefabs/LocationButton.prefab\n" +
            "   hacia el campo 'Location Button Prefab'\n\n" +
            "4. ¡Listo para usar!",
            "Entendido"
        );
    }
    
    private static GameObject CreateCanvas()
    {
        // Verificar si ya existe un Canvas
        Canvas existingCanvas = GameObject.FindObjectOfType<Canvas>();
        if (existingCanvas != null && existingCanvas.gameObject.name == "NavigationCanvas")
        {
            Debug.LogWarning("[NavigationUICreator] Ya existe un NavigationCanvas. Se usará ese.");
            return existingCanvas.gameObject;
        }
        
        // Crear Canvas
        GameObject canvasObj = new GameObject("NavigationCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Asegurar que esté encima de todo
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Crear EventSystem si no existe
        if (GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("[NavigationUICreator] ? EventSystem creado");
        }
        
        Debug.Log("[NavigationUICreator] ? Canvas creado");
        return canvasObj;
    }
    
    private static GameObject CreateLocationSelectionPanel(Transform parent)
    {
        // Panel principal
        GameObject panel = new GameObject("LocationSelectionPanel");
        panel.transform.SetParent(parent, false);
        panel.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rectPanel = panel.AddComponent<RectTransform>();
        rectPanel.anchorMin = Vector2.zero;
        rectPanel.anchorMax = Vector2.one;
        rectPanel.sizeDelta = Vector2.zero;
        rectPanel.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.85f);
        
        // Título
        CreateTitle(panel.transform);
        
        // Loading Text
        CreateLoadingText(panel.transform);
        
        // ScrollView con Content
        CreateScrollView(panel.transform);
        
        Debug.Log("[NavigationUICreator] ? LocationSelectionPanel creado");
        return panel;
    }
    
    private static void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(parent, false);
        titleObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -30);
        titleRect.sizeDelta = new Vector2(900, 70);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "?? Selecciona un Destino";
        titleText.fontSize = 32;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
    }
    
    private static void CreateLoadingText(Transform parent)
    {
        GameObject loadingObj = new GameObject("LoadingText");
        loadingObj.transform.SetParent(parent, false);
        loadingObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform loadingRect = loadingObj.AddComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.5f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.5f);
        loadingRect.pivot = new Vector2(0.5f, 0.5f);
        loadingRect.anchoredPosition = Vector2.zero;
        loadingRect.sizeDelta = new Vector2(600, 100);
        
        TextMeshProUGUI loadingText = loadingObj.AddComponent<TextMeshProUGUI>();
        loadingText.text = "? Cargando lugares...";
        loadingText.fontSize = 24;
        loadingText.alignment = TextAlignmentOptions.Center;
        loadingText.color = new Color(1f, 0.9f, 0.3f); // Amarillo
        
        loadingObj.SetActive(false);
    }
    
    private static void CreateScrollView(Transform parent)
    {
        GameObject scrollViewObj = new GameObject("LocationScrollView");
        scrollViewObj.transform.SetParent(parent, false);
        scrollViewObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform scrollRect = scrollViewObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.pivot = new Vector2(0.5f, 0.5f);
        scrollRect.offsetMin = new Vector2(30, 120); // Left, Bottom
        scrollRect.offsetMax = new Vector2(-30, -120); // Right, Top
        
        Image scrollBg = scrollViewObj.AddComponent<Image>();
        scrollBg.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
        
        ScrollRect scroll = scrollViewObj.AddComponent<ScrollRect>();
        
        // Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollViewObj.transform, false);
        viewport.layer = LayerMask.NameToLayer("UI");
        
        RectTransform viewportRect = viewport.AddComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;
        
        Image viewportImg = viewport.AddComponent<Image>();
        viewportImg.color = new Color(1, 1, 1, 0.01f);
        
        Mask mask = viewport.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        
        // Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        content.layer = LayerMask.NameToLayer("UI");
        
        RectTransform contentRect = content.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0, 800);
        
        VerticalLayoutGroup layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 15;
        layout.padding = new RectOffset(15, 15, 15, 15);
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        
        ContentSizeFitter fitter = content.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        // Configurar ScrollRect
        scroll.content = contentRect;
        scroll.viewport = viewportRect;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Elastic;
        scroll.scrollSensitivity = 30f;
    }
    
    private static GameObject CreateNavigationActivePanel(Transform parent)
    {
        GameObject panel = new GameObject("NavigationActivePanel");
        panel.transform.SetParent(parent, false);
        panel.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rectPanel = panel.AddComponent<RectTransform>();
        rectPanel.anchorMin = new Vector2(0, 1);
        rectPanel.anchorMax = new Vector2(1, 1);
        rectPanel.pivot = new Vector2(0.5f, 1);
        rectPanel.anchoredPosition = Vector2.zero;
        rectPanel.sizeDelta = new Vector2(0, 220);
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0, 0.35f, 0.6f, 0.95f);
        
        // Crear todos los textos
        CreateDestinationNameText(panel.transform);
        CreateDistanceText(panel.transform);
        CreateDirectionText(panel.transform);
        CreateCancelButton(panel.transform);
        
        Debug.Log("[NavigationUICreator] ? NavigationActivePanel creado");
        return panel;
    }
    
    private static void CreateDestinationNameText(Transform parent)
    {
        GameObject obj = new GameObject("DestinationNameText");
        obj.transform.SetParent(parent, false);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector2(0, -15);
        rect.sizeDelta = new Vector2(-40, 50);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = "Destino";
        text.fontSize = 28;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }
    
    private static void CreateDistanceText(Transform parent)
    {
        GameObject obj = new GameObject("DistanceText");
        obj.transform.SetParent(parent, false);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0, 5);
        rect.sizeDelta = new Vector2(900, 70);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = "--- m";
        text.fontSize = 42;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.3f, 1f, 0.3f); // Verde brillante
    }
    
    private static void CreateDirectionText(Transform parent)
    {
        GameObject obj = new GameObject("DirectionText");
        obj.transform.SetParent(parent, false);
        obj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0);
        rect.anchorMax = new Vector2(0.5f, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = new Vector2(0, 70);
        rect.sizeDelta = new Vector2(900, 40);
        
        TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
        text.text = "N (0°)";
        text.fontSize = 22;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.9f, 0.9f, 0.9f);
    }
    
    private static void CreateCancelButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("CancelNavigationButton");
        buttonObj.transform.SetParent(parent, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.pivot = new Vector2(0.5f, 0);
        buttonRect.anchoredPosition = new Vector2(0, 15);
        buttonRect.sizeDelta = new Vector2(320, 55);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.9f, 0.25f, 0.25f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "? Cancelar Navegación";
        buttonText.fontSize = 20;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
    }
    
    private static GameObject CreateMainButton(Transform parent)
    {
        GameObject buttonObj = new GameObject("OpenLocationPanelButton");
        buttonObj.transform.SetParent(parent, false);
        buttonObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 0);
        buttonRect.anchorMax = new Vector2(1, 0);
        buttonRect.pivot = new Vector2(1, 0);
        buttonRect.anchoredPosition = new Vector2(-25, 25);
        buttonRect.sizeDelta = new Vector2(160, 160);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.65f, 1f, 0.95f);
        
        Button button = buttonObj.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        
        // Texto del botón
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        textObj.layer = LayerMask.NameToLayer("UI");
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "??\nNavegar";
        buttonText.fontSize = 26;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        Debug.Log("[NavigationUICreator] ? Botón principal creado");
        return buttonObj;
    }
    
    private static void ConfigureUIManager(NavigationUIManager manager, GameObject locationPanel, GameObject navPanel, GameObject mainButton)
    {
        manager.locationSelectionPanel = locationPanel;
        manager.navigationActivePanel = navPanel;
        manager.openLocationPanelButton = mainButton.GetComponent<Button>();
        
        // Encontrar componentes en LocationPanel
        manager.loadingText = locationPanel.transform.Find("LoadingText").GetComponent<TextMeshProUGUI>();
        manager.locationListContent = locationPanel.transform.Find("LocationScrollView/Viewport/Content");
        
        // Encontrar componentes en NavigationActivePanel
        manager.destinationNameText = navPanel.transform.Find("DestinationNameText").GetComponent<TextMeshProUGUI>();
        manager.distanceText = navPanel.transform.Find("DistanceText").GetComponent<TextMeshProUGUI>();
        manager.directionText = navPanel.transform.Find("DirectionText").GetComponent<TextMeshProUGUI>();
        manager.cancelNavigationButton = navPanel.transform.Find("CancelNavigationButton").GetComponent<Button>();
        
        manager.loadLocationsOnStart = true;
        
        // Marcar como sucio para guardar cambios
        EditorUtility.SetDirty(manager);
        
        Debug.Log("[NavigationUICreator] ? NavigationUIManager configurado completamente");
    }
    
    #endif
}
