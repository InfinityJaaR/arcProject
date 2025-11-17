using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Herramienta de Editor para configurar el sistema de mapa expandible
/// Versión 1.0
/// </summary>
public class SetupExpandableMap : EditorWindow
{
    [MenuItem("AR Navigation/Setup Expandable Map System")]
    public static void ShowWindow()
    {
        SetupExpandableMap window = GetWindow<SetupExpandableMap>("Mapa Expandible");
        window.minSize = new Vector2(400, 600);
        window.Show();
    }
    
    private Vector2 scrollPosition;
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("??? CONFIGURACIÓN DE MAPA EXPANDIBLE", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Este sistema permite expandir el minimapa a pantalla completa con zoom y pan interactivos.",
            MessageType.Info
        );
        
        GUILayout.Space(20);
        
        // Botón de configuración automática
        if (GUILayout.Button("?? Configurar Mapa Expandible Automáticamente", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog(
                "Confirmar Configuración",
                "¿Crear el sistema de mapa expandible automáticamente?\n\n" +
                "Esto modificará el MiniMapCanvas existente.",
                "Sí, configurar",
                "Cancelar"))
            {
                SetupExpandableMapSystem();
            }
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "CARACTERÍSTICAS:\n" +
            "? Minimapa compacto en esquina\n" +
            "? Botón para expandir a pantalla completa\n" +
            "? Zoom interactivo (slider + pinch)\n" +
            "? Pan/arrastre con el dedo\n" +
            "? Botón para centrar en tu ubicación\n" +
            "? Animaciones suaves\n" +
            "? Interfaz táctil optimizada",
            MessageType.None
        );
        
        GUILayout.Space(20);
        
        // Botón de verificación
        if (GUILayout.Button("?? Verificar Configuración Actual", GUILayout.Height(30)))
        {
            VerifySetup();
        }
        
        GUILayout.Space(10);
        
        // Botón de documentación
        if (GUILayout.Button("?? Ver Documentación", GUILayout.Height(30)))
        {
            // Abrir archivo de documentación si existe
            string docPath = "Assets/GUIA_MAPA_EXPANDIBLE.md";
            if (System.IO.File.Exists(docPath))
            {
                System.Diagnostics.Process.Start(docPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Documentación", 
                    "La documentación se creará automáticamente al configurar el sistema.", 
                    "OK");
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private static void SetupExpandableMapSystem()
    {
        Debug.Log("[SetupExpandableMap] ?? Iniciando configuración del mapa expandible...");
        
        // 1. Buscar MiniMapCanvas existente
        GameObject miniMapCanvas = GameObject.Find("MiniMapCanvas");
        if (miniMapCanvas == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "No se encontró MiniMapCanvas.\n\n" +
                "Por favor, ejecuta primero:\n" +
                "AR Navigation ? Setup MiniMap System", 
                "OK");
            return;
        }
        
        // 2. Agregar ExpandableMapController al canvas
        ExpandableMapController expandableController = miniMapCanvas.GetComponent<ExpandableMapController>();
        if (expandableController == null)
        {
            expandableController = miniMapCanvas.AddComponent<ExpandableMapController>();
            Debug.Log("[SetupExpandableMap] ? ExpandableMapController agregado");
        }
        
        // 3. Encontrar o crear paneles
        GameObject compactPanel = FindOrCreateCompactPanel(miniMapCanvas);
        GameObject expandedPanel = CreateExpandedPanel(miniMapCanvas);
        
        // 4. Configurar referencias del controller
        MiniMapController miniMapController = miniMapCanvas.GetComponent<MiniMapController>();
        if (miniMapController != null)
        {
            // Mover el mapContainer al panel compacto
            Transform mapPanel = miniMapCanvas.transform.Find("MapPanel");
            if (mapPanel != null)
            {
                RectTransform mapContainer = mapPanel.Find("MapContainer") as RectTransform;
                if (mapContainer != null)
                {
                    expandableController.compactMapContainer = mapContainer;
                    Debug.Log("[SetupExpandableMap] ? compactMapContainer asignado");
                }
            }
        }
        
        // 5. Crear contenedor para mapa expandido
        GameObject expandedMapContainer = new GameObject("ExpandedMapContainer");
        RectTransform expandedRect = expandedMapContainer.AddComponent<RectTransform>();
        expandedRect.SetParent(expandedPanel.transform, false);
        
        // Configurar como panel centrado y grande
        expandedRect.anchorMin = new Vector2(0.1f, 0.1f);
        expandedRect.anchorMax = new Vector2(0.9f, 0.85f);
        expandedRect.offsetMin = Vector2.zero;
        expandedRect.offsetMax = Vector2.zero;
        
        expandableController.expandedMapContainer = expandedRect;
        Debug.Log("[SetupExpandableMap] ? expandedMapContainer creado");
        
        // 6. Crear controles del mapa expandido
        CreateExpandedControls(expandedPanel, expandableController);
        
        // 7. Crear botón de expandir en el minimapa
        CreateExpandButton(compactPanel, expandableController);
        
        // 8. Asignar paneles al controller
        expandableController.compactMapPanel = compactPanel;
        expandableController.expandedMapPanel = expandedPanel;
        
        // 9. Configurar valores iniciales
        expandableController.transitionDuration = 0.3f;
        expandableController.minZoom = 0.5f;
        expandableController.maxZoom = 3.0f;
        expandableController.initialExpandedZoom = 1.5f;
        expandableController.allowPanning = true;
        expandableController.panSpeed = 1.0f;
        expandableController.panLimitDistance = 500f;
        
        // 10. Guardar
        EditorUtility.SetDirty(miniMapCanvas);
        
        Debug.Log("[SetupExpandableMap] ? ¡Sistema de mapa expandible configurado!");
        
        EditorUtility.DisplayDialog(
            "¡Configuración Completa! ??",
            "El sistema de mapa expandible está listo.\n\n" +
            "CÓMO USAR:\n" +
            "• Botón ?? en el minimapa ? Expandir\n" +
            "• En vista expandida:\n" +
            "  - Desliza para mover el mapa\n" +
            "  - Usa el slider para zoom\n" +
            "  - Botón ?? para centrar\n" +
            "  - Botón ? para cerrar\n\n" +
            "¡Ejecuta la app y pruébalo!",
            "¡Excelente!"
        );
    }
    
    private static GameObject FindOrCreateCompactPanel(GameObject parent)
    {
        Transform existing = parent.transform.Find("CompactMapPanel");
        if (existing != null)
        {
            Debug.Log("[SetupExpandableMap] ? CompactMapPanel ya existe");
            return existing.gameObject;
        }
        
        // Encontrar MapPanel existente y renombrarlo
        Transform mapPanel = parent.transform.Find("MapPanel");
        if (mapPanel != null)
        {
            mapPanel.name = "CompactMapPanel";
            Debug.Log("[SetupExpandableMap] ? MapPanel renombrado a CompactMapPanel");
            return mapPanel.gameObject;
        }
        
        Debug.LogError("[SetupExpandableMap] ? No se encontró MapPanel");
        return null;
    }
    
    private static GameObject CreateExpandedPanel(GameObject parent)
    {
        // Buscar si ya existe
        Transform existing = parent.transform.Find("ExpandedMapPanel");
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }
        
        // Crear panel de fondo oscuro (pantalla completa)
        GameObject expandedPanel = new GameObject("ExpandedMapPanel");
        RectTransform panelRect = expandedPanel.AddComponent<RectTransform>();
        panelRect.SetParent(parent.transform, false);
        
        // Pantalla completa
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Fondo semi-transparente
        Image panelImage = expandedPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f); // Negro casi opaco
        
        // Canvas Group para animaciones
        expandedPanel.AddComponent<CanvasGroup>();
        
        expandedPanel.SetActive(false); // Inicialmente oculto
        
        Debug.Log("[SetupExpandableMap] ? ExpandedMapPanel creado");
        
        return expandedPanel;
    }
    
    private static void CreateExpandedControls(GameObject parent, ExpandableMapController controller)
    {
        // Header con título y botón cerrar
        CreateHeader(parent, controller);
        
        // Footer con controles (zoom, centrar)
        CreateFooter(parent, controller);
    }
    
    private static void CreateHeader(GameObject parent, ExpandableMapController controller)
    {
        GameObject header = new GameObject("Header");
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.SetParent(parent.transform, false);
        
        // Posición en la parte superior
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 80);
        headerRect.anchoredPosition = Vector2.zero;
        
        // Fondo del header
        Image headerBg = header.AddComponent<Image>();
        headerBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // Botón cerrar (X)
        GameObject closeBtn = new GameObject("CloseButton");
        RectTransform closeBtnRect = closeBtn.AddComponent<RectTransform>();
        closeBtnRect.SetParent(header.transform, false);
        closeBtnRect.anchorMin = new Vector2(0, 0.5f);
        closeBtnRect.anchorMax = new Vector2(0, 0.5f);
        closeBtnRect.pivot = new Vector2(0, 0.5f);
        closeBtnRect.sizeDelta = new Vector2(60, 60);
        closeBtnRect.anchoredPosition = new Vector2(10, 0);
        
        Image closeBtnImg = closeBtn.AddComponent<Image>();
        closeBtnImg.color = new Color(0.8f, 0.2f, 0.2f, 1f);
        
        Button closeBtnButton = closeBtn.AddComponent<Button>();
        controller.closeButton = closeBtnButton;
        
        // Texto del botón cerrar
        GameObject closeBtnText = new GameObject("Text");
        TextMeshProUGUI closeText = closeBtnText.AddComponent<TextMeshProUGUI>();
        closeText.text = "?";
        closeText.fontSize = 36;
        closeText.alignment = TextAlignmentOptions.Center;
        closeText.color = Color.white;
        
        RectTransform closeTextRect = closeBtnText.GetComponent<RectTransform>();
        closeTextRect.SetParent(closeBtn.transform, false);
        closeTextRect.anchorMin = Vector2.zero;
        closeTextRect.anchorMax = Vector2.one;
        closeTextRect.offsetMin = Vector2.zero;
        closeTextRect.offsetMax = Vector2.zero;
        
        // Título
        GameObject titleObj = new GameObject("Title");
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "??? MAPA DEL CAMPUS";
        title.fontSize = 28;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.SetParent(header.transform, false);
        titleRect.anchorMin = new Vector2(0.2f, 0);
        titleRect.anchorMax = new Vector2(0.8f, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        Debug.Log("[SetupExpandableMap] ? Header creado");
    }
    
    private static void CreateFooter(GameObject parent, ExpandableMapController controller)
    {
        GameObject footer = new GameObject("Footer");
        RectTransform footerRect = footer.AddComponent<RectTransform>();
        footerRect.SetParent(parent.transform, false);
        
        // Posición en la parte inferior
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0);
        footerRect.pivot = new Vector2(0.5f, 0);
        footerRect.sizeDelta = new Vector2(0, 100);
        footerRect.anchoredPosition = Vector2.zero;
        
        // Fondo del footer
        Image footerBg = footer.AddComponent<Image>();
        footerBg.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // Botón centrar
        GameObject centerBtn = CreateFooterButton(footer.transform, "?? Centrar", new Vector2(20, 0), new Vector2(0, 0.5f));
        controller.centerButton = centerBtn.GetComponent<Button>();
        
        // Slider de zoom
        CreateZoomSlider(footer.transform, controller);
        
        Debug.Log("[SetupExpandableMap] ? Footer creado");
    }
    
    private static GameObject CreateFooterButton(Transform parent, string text, Vector2 position, Vector2 anchor)
    {
        GameObject btn = new GameObject("Button_" + text.Replace(" ", ""));
        RectTransform btnRect = btn.AddComponent<RectTransform>();
        btnRect.SetParent(parent, false);
        btnRect.anchorMin = anchor;
        btnRect.anchorMax = anchor;
        btnRect.pivot = new Vector2(0, 0.5f);
        btnRect.sizeDelta = new Vector2(150, 60);
        btnRect.anchoredPosition = position;
        
        Image btnImg = btn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        
        btn.AddComponent<Button>();
        
        // Texto del botón
        GameObject btnText = new GameObject("Text");
        TextMeshProUGUI tmpText = btnText.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 20;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        
        RectTransform textRect = btnText.GetComponent<RectTransform>();
        textRect.SetParent(btn.transform, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return btn;
    }
    
    private static void CreateZoomSlider(Transform parent, ExpandableMapController controller)
    {
        GameObject sliderObj = new GameObject("ZoomSlider");
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.SetParent(parent, false);
        sliderRect.anchorMin = new Vector2(0.3f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.7f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.sizeDelta = new Vector2(0, 40);
        sliderRect.anchoredPosition = Vector2.zero;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        
        // Background
        GameObject bg = new GameObject("Background");
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.SetParent(sliderObj.transform, false);
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.SetParent(sliderObj.transform, false);
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.offsetMin = new Vector2(10, 0);
        fillAreaRect.offsetMax = new Vector2(-10, 0);
        
        GameObject fill = new GameObject("Fill");
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.SetParent(fillArea.transform, false);
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.2f, 0.6f, 1f, 1f);
        
        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.SetParent(sliderObj.transform, false);
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10, 0);
        handleAreaRect.offsetMax = new Vector2(-10, 0);
        
        GameObject handle = new GameObject("Handle");
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.SetParent(handleArea.transform, false);
        handleRect.sizeDelta = new Vector2(20, 40);
        
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        
        // Configurar slider
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        
        controller.zoomSlider = slider;
        
        // Texto de zoom
        GameObject zoomText = new GameObject("ZoomText");
        TextMeshProUGUI tmpText = zoomText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "Zoom: 1.0x";
        tmpText.fontSize = 18;
        tmpText.alignment = TextAlignmentOptions.Right;
        tmpText.color = Color.white;
        
        RectTransform zoomTextRect = zoomText.GetComponent<RectTransform>();
        zoomTextRect.SetParent(parent, false);
        zoomTextRect.anchorMin = new Vector2(0.75f, 0.5f);
        zoomTextRect.anchorMax = new Vector2(0.95f, 0.5f);
        zoomTextRect.sizeDelta = new Vector2(0, 30);
        zoomTextRect.anchoredPosition = Vector2.zero;
        
        controller.zoomText = tmpText;
        
        Debug.Log("[SetupExpandableMap] ? Zoom slider creado");
    }
    
    private static void CreateExpandButton(GameObject compactPanel, ExpandableMapController controller)
    {
        // Buscar si ya existe
        Transform existing = compactPanel.transform.Find("ExpandButton");
        if (existing != null)
        {
            DestroyImmediate(existing.gameObject);
        }
        
        GameObject expandBtn = new GameObject("ExpandButton");
        RectTransform btnRect = expandBtn.AddComponent<RectTransform>();
        btnRect.SetParent(compactPanel.transform, false);
        
        // Posición en la esquina inferior derecha del minimapa
        btnRect.anchorMin = new Vector2(1, 0);
        btnRect.anchorMax = new Vector2(1, 0);
        btnRect.pivot = new Vector2(1, 0);
        btnRect.sizeDelta = new Vector2(50, 50);
        btnRect.anchoredPosition = new Vector2(-5, 5);
        
        Image btnImg = expandBtn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 1f, 0.9f);
        
        Button button = expandBtn.AddComponent<Button>();
        controller.expandButton = button;
        
        // Texto del botón
        GameObject btnText = new GameObject("Text");
        TextMeshProUGUI tmpText = btnText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "??";
        tmpText.fontSize = 28;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        
        RectTransform textRect = btnText.GetComponent<RectTransform>();
        textRect.SetParent(expandBtn.transform, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Debug.Log("[SetupExpandableMap] ? Botón expandir creado");
    }
    
    private static void VerifySetup()
    {
        bool allGood = true;
        string report = "VERIFICACIÓN DEL SISTEMA:\n\n";
        
        // Verificar MiniMapCanvas
        GameObject miniMapCanvas = GameObject.Find("MiniMapCanvas");
        if (miniMapCanvas != null)
        {
            report += "? MiniMapCanvas encontrado\n";
            
            // Verificar ExpandableMapController
            ExpandableMapController controller = miniMapCanvas.GetComponent<ExpandableMapController>();
            if (controller != null)
            {
                report += "? ExpandableMapController encontrado\n";
                
                if (controller.compactMapPanel != null)
                    report += "? compactMapPanel asignado\n";
                else
                {
                    report += "? compactMapPanel NO asignado\n";
                    allGood = false;
                }
                
                if (controller.expandedMapPanel != null)
                    report += "? expandedMapPanel asignado\n";
                else
                {
                    report += "? expandedMapPanel NO asignado\n";
                    allGood = false;
                }
                
                if (controller.expandButton != null)
                    report += "? expandButton asignado\n";
                else
                {
                    report += "? expandButton NO asignado\n";
                    allGood = false;
                }
                
                if (controller.closeButton != null)
                    report += "? closeButton asignado\n";
                else
                {
                    report += "? closeButton NO asignado\n";
                    allGood = false;
                }
            }
            else
            {
                report += "? ExpandableMapController NO encontrado\n";
                report += "\nEjecuta: 'Configurar Mapa Expandible Automáticamente'\n";
                allGood = false;
            }
        }
        else
        {
            report += "? MiniMapCanvas NO encontrado\n";
            report += "\nPrimero ejecuta: AR Navigation ? Setup MiniMap System\n";
            allGood = false;
        }
        
        if (allGood)
        {
            report += "\n?? ¡TODO CONFIGURADO CORRECTAMENTE!";
            EditorUtility.DisplayDialog("Verificación Exitosa", report, "Genial!");
        }
        else
        {
            report += "\n?? Hay problemas en la configuración.";
            EditorUtility.DisplayDialog("Verificación Fallida", report, "Entendido");
        }
        
        Debug.Log("[SetupExpandableMap] " + report);
    }
}
