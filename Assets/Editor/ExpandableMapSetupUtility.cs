using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Utilidad para configurar el mapa expandible manualmente
/// Ejecutar desde: Assets ? Create ? AR Navigation ? Setup Mapa Expandible
/// </summary>
public class ExpandableMapSetupUtility
{
    [MenuItem("Assets/Create/AR Navigation/Setup Mapa Expandible")]
    public static void SetupFromAssetsMenu()
    {
        if (EditorUtility.DisplayDialog(
            "Setup Mapa Expandible",
            "¿Configurar el sistema de mapa expandible automáticamente?\n\n" +
            "Esto modificará el MiniMapCanvas existente.",
            "Sí, configurar",
            "Cancelar"))
        {
            SetupExpandableMapSystem();
        }
    }
    
    public static void SetupExpandableMapSystem()
    {
        Debug.Log("[ExpandableMapSetup] ?? Iniciando configuración del mapa expandible...");
        
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
            Debug.Log("[ExpandableMapSetup] ? ExpandableMapController agregado");
        }
        else
        {
            Debug.Log("[ExpandableMapSetup] ?? ExpandableMapController ya existe, reconfigurando...");
        }
        
        // 3. Encontrar o crear paneles
        GameObject compactPanel = FindOrCreateCompactPanel(miniMapCanvas);
        if (compactPanel == null)
        {
            Debug.LogError("[ExpandableMapSetup] ? No se pudo crear CompactMapPanel");
            return;
        }
        
        GameObject expandedPanel = CreateExpandedPanel(miniMapCanvas);
        
        // 4. Configurar referencias del controller
        MiniMapController miniMapController = miniMapCanvas.GetComponent<MiniMapController>();
        if (miniMapController != null)
        {
            // Buscar el mapContainer directamente del MiniMapController
            RectTransform mapContainer = miniMapController.mapContainer;
            if (mapContainer != null)
            {
                // El compactMapContainer debe ser el PADRE del mapContainer
                // No el mapContainer mismo
                expandableController.compactMapContainer = mapContainer.parent as RectTransform;
                Debug.Log($"[ExpandableMapSetup] ? compactMapContainer asignado: {expandableController.compactMapContainer.name}");
                Debug.Log($"[ExpandableMapSetup] ?? mapContainer actual: {mapContainer.name} (hijo de {mapContainer.parent.name})");
            }
            else
            {
                // Buscar manualmente el contenedor en el panel compacto
                Transform mapContainerTransform = compactPanel.transform.Find("MapContainer");
                if (mapContainerTransform == null)
                {
                    // Si no hay MapContainer hijo, usar el panel mismo como contenedor
                    expandableController.compactMapContainer = compactPanel.GetComponent<RectTransform>();
                    Debug.LogWarning($"[ExpandableMapSetup] ?? No se encontró MapContainer hijo, usando {compactPanel.name} como contenedor");
                }
                else
                {
                    // Usar el padre del MapContainer
                    expandableController.compactMapContainer = mapContainerTransform.parent as RectTransform;
                    Debug.Log($"[ExpandableMapSetup] ? compactMapContainer asignado desde búsqueda: {expandableController.compactMapContainer.name}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[ExpandableMapSetup] ?? No se encontró MiniMapController");
        }
        
        // 5. Crear contenedor para mapa expandido
        Transform existingExpanded = expandedPanel.transform.Find("ExpandedMapContainer");
        GameObject expandedMapContainer;
        
        if (existingExpanded != null)
        {
            expandedMapContainer = existingExpanded.gameObject;
            Debug.Log("[ExpandableMapSetup] ?? ExpandedMapContainer ya existe");
        }
        else
        {
            expandedMapContainer = new GameObject("ExpandedMapContainer");
            RectTransform expandedRect = expandedMapContainer.AddComponent<RectTransform>();
            expandedRect.SetParent(expandedPanel.transform, false);
            
            // Configurar para dejar espacio al panel lateral derecho
            // Header arriba: 80px, Panel derecho: 200px
            expandedRect.anchorMin = new Vector2(0.05f, 0.05f);
            expandedRect.anchorMax = new Vector2(0.85f, 0.9f); // Dejar 15% a la derecha para controles
            expandedRect.offsetMin = Vector2.zero;
            expandedRect.offsetMax = Vector2.zero;
            
            Debug.Log("[ExpandableMapSetup] ? ExpandedMapContainer creado con espacio para panel lateral");
        }
        
        expandableController.expandedMapContainer = expandedMapContainer.GetComponent<RectTransform>();
        
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
        
        Debug.Log("[ExpandableMapSetup] ? ¡Sistema de mapa expandible configurado!");
        
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
        // Buscar CompactMapPanel
        Transform existing = parent.transform.Find("CompactMapPanel");
        if (existing != null)
        {
            Debug.Log("[ExpandableMapSetup] ? CompactMapPanel ya existe");
            return existing.gameObject;
        }
        
        // Buscar MapPanel y renombrar
        Transform mapPanel = parent.transform.Find("MapPanel");
        if (mapPanel != null)
        {
            mapPanel.name = "CompactMapPanel";
            Debug.Log("[ExpandableMapSetup] ? MapPanel renombrado a CompactMapPanel");
            return mapPanel.gameObject;
        }
        
        Debug.LogError("[ExpandableMapSetup] ? No se encontró MapPanel");
        return null;
    }
    
    private static GameObject CreateExpandedPanel(GameObject parent)
    {
        // Buscar si ya existe
        Transform existing = parent.transform.Find("ExpandedMapPanel");
        if (existing != null)
        {
            Debug.Log("[ExpandableMapSetup] ?? ExpandedMapPanel ya existe, eliminando para recrear...");
            Object.DestroyImmediate(existing.gameObject);
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
        panelImage.color = new Color(0, 0, 0, 0.9f);
        
        // Canvas Group para animaciones
        expandedPanel.AddComponent<CanvasGroup>();
        
        expandedPanel.SetActive(false);
        
        Debug.Log("[ExpandableMapSetup] ? ExpandedMapPanel creado");
        
        return expandedPanel;
    }
    
    private static void CreateExpandedControls(GameObject parent, ExpandableMapController controller)
    {
        // Crear panel de controles lateral derecho (más usable)
        CreateControlPanel(parent, controller);
    }
    
    private static void CreateControlPanel(GameObject parent, ExpandableMapController controller)
    {
        // Panel de controles en el lado derecho
        GameObject controlPanel = new GameObject("ControlPanel");
        RectTransform controlRect = controlPanel.AddComponent<RectTransform>();
        controlRect.SetParent(parent.transform, false);
        
        // Posicionado en el lado derecho, ocupa todo el alto
        controlRect.anchorMin = new Vector2(1, 0);
        controlRect.anchorMax = new Vector2(1, 1);
        controlRect.pivot = new Vector2(1, 0.5f);
        controlRect.sizeDelta = new Vector2(200, 0); // 200px de ancho
        controlRect.anchoredPosition = Vector2.zero;
        
        // Fondo semi-transparente
        Image panelBg = controlPanel.AddComponent<Image>();
        panelBg.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        
        // Agregar layout vertical para organizar botones
        VerticalLayoutGroup layout = controlPanel.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 15;
        layout.padding = new RectOffset(15, 15, 20, 20);
        layout.childAlignment = TextAnchor.MiddleCenter; // CAMBIAR: Centrar verticalmente
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        
        // NUEVO: Agregar espaciador flexible arriba para empujar contenido hacia el centro
        CreateFlexibleSpacer(controlPanel.transform);
        
        // 1. Título del panel
        CreatePanelTitle(controlPanel.transform);
        
        // 2. Botón Cerrar (grande)
        GameObject closeBtn = CreateControlButton(controlPanel.transform, "? CERRAR", 80);
        closeBtn.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 1f);
        controller.closeButton = closeBtn.GetComponent<Button>();
        
        // 3. Separador
        CreateSeparator(controlPanel.transform);
        
        // 4. Botón Centrar
        GameObject centerBtn = CreateControlButton(controlPanel.transform, "?? CENTRAR", 70);
        centerBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f, 1f);
        controller.centerButton = centerBtn.GetComponent<Button>();
        
        // 5. Separador
        CreateSeparator(controlPanel.transform);
        
        // 6. Label de Zoom
        CreateLabel(controlPanel.transform, "?? ZOOM");
        
        // 7. Slider de Zoom (más grande)
        CreateZoomSliderVertical(controlPanel.transform, controller);
        
        // 8. Texto de zoom
        GameObject zoomText = CreateLabel(controlPanel.transform, "1.0x");
        controller.zoomText = zoomText.GetComponent<TextMeshProUGUI>();
        
        // NUEVO: Agregar espaciador flexible abajo para balancear
        CreateFlexibleSpacer(controlPanel.transform);
        
        Debug.Log("[ExpandableMapSetup] ? Panel de controles lateral creado (centrado verticalmente)");
    }
    
    /// <summary>
    /// Crea un espaciador flexible que se expande automáticamente
    /// </summary>
    private static void CreateFlexibleSpacer(Transform parent)
    {
        GameObject spacer = new GameObject("FlexibleSpacer");
        RectTransform spacerRect = spacer.AddComponent<RectTransform>();
        spacerRect.SetParent(parent, false);
        
        LayoutElement layoutElement = spacer.AddComponent<LayoutElement>();
        layoutElement.flexibleHeight = 1; // Se expande para llenar espacio disponible
        layoutElement.preferredHeight = 0;
        layoutElement.minHeight = 0;
    }
    
    private static void CreatePanelTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("PanelTitle");
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "CONTROLES";
        title.fontSize = 20;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.SetParent(parent, false);
        titleRect.sizeDelta = new Vector2(0, 30);
        
        // Agregar LayoutElement para controlar tamaño
        LayoutElement layoutElement = titleObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 30;
        layoutElement.minHeight = 30;
    }
    
    private static GameObject CreateControlButton(Transform parent, string text, float height)
    {
        GameObject btn = new GameObject("Button_" + text.Replace(" ", "").Replace("?", "Close").Replace("??", "Center"));
        RectTransform btnRect = btn.AddComponent<RectTransform>();
        btnRect.SetParent(parent, false);
        btnRect.sizeDelta = new Vector2(0, height);
        
        Image btnImg = btn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        
        btn.AddComponent<Button>();
        
        // Agregar LayoutElement
        LayoutElement layoutElement = btn.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
        layoutElement.minHeight = height;
        
        // Texto del botón
        GameObject btnText = new GameObject("Text");
        TextMeshProUGUI tmpText = btnText.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 24;
        tmpText.fontStyle = FontStyles.Bold;
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
    
    private static void CreateSeparator(Transform parent)
    {
        GameObject separator = new GameObject("Separator");
        RectTransform sepRect = separator.AddComponent<RectTransform>();
        sepRect.SetParent(parent, false);
        sepRect.sizeDelta = new Vector2(0, 2);
        
        Image sepImg = separator.AddComponent<Image>();
        sepImg.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        
        LayoutElement layoutElement = separator.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 2;
        layoutElement.minHeight = 2;
    }
    
    private static GameObject CreateLabel(Transform parent, string text)
    {
        GameObject labelObj = new GameObject("Label_" + text.Replace(" ", ""));
        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 18;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.SetParent(parent, false);
        labelRect.sizeDelta = new Vector2(0, 25);
        
        LayoutElement layoutElement = labelObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 25;
        layoutElement.minHeight = 25;
        
        return labelObj;
    }
    
    private static void CreateZoomSliderVertical(Transform parent, ExpandableMapController controller)
    {
        GameObject sliderObj = new GameObject("ZoomSlider");
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.SetParent(parent, false);
        sliderRect.sizeDelta = new Vector2(0, 150); // Slider vertical más grande
        
        LayoutElement layoutElement = sliderObj.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 150;
        layoutElement.minHeight = 150;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.direction = Slider.Direction.BottomToTop; // Vertical
        slider.minValue = 0.5f;
        slider.maxValue = 3.0f;
        slider.value = 1.0f;
        
        // Background
        GameObject bg = new GameObject("Background");
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.SetParent(sliderObj.transform, false);
        bgRect.anchorMin = new Vector2(0.4f, 0);
        bgRect.anchorMax = new Vector2(0.6f, 1);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.SetParent(sliderObj.transform, false);
        fillAreaRect.anchorMin = new Vector2(0.4f, 0);
        fillAreaRect.anchorMax = new Vector2(0.6f, 1);
        fillAreaRect.offsetMin = new Vector2(0, 10);
        fillAreaRect.offsetMax = new Vector2(0, -10);
        
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
        handleAreaRect.anchorMin = new Vector2(0, 0);
        handleAreaRect.anchorMax = new Vector2(1, 1);
        handleAreaRect.offsetMin = new Vector2(0, 10);
        handleAreaRect.offsetMax = new Vector2(0, -10);
        
        GameObject handle = new GameObject("Handle");
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.SetParent(handleArea.transform, false);
        handleRect.sizeDelta = new Vector2(50, 30);
        
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        
        // Configurar slider
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        
        controller.zoomSlider = slider;
        
        Debug.Log("[ExpandableMapSetup] ? Slider de zoom vertical creado");
    }
    
    private static void CreateHeader(GameObject parent, ExpandableMapController controller)
    {
        // Header simplificado solo con el título
        GameObject header = new GameObject("Header");
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.SetParent(parent.transform, false);
        
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 80);
        headerRect.anchoredPosition = Vector2.zero;
        
        Image headerBg = header.AddComponent<Image>();
        headerBg.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        
        // Título centrado
        GameObject titleObj = new GameObject("Title");
        TextMeshProUGUI title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "??? MAPA DEL CAMPUS";
        title.fontSize = 32;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.SetParent(header.transform, false);
        titleRect.anchorMin = new Vector2(0.1f, 0);
        titleRect.anchorMax = new Vector2(0.8f, 1); // Dejar espacio para el panel lateral
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        Debug.Log("[ExpandableMapSetup] ? Header simplificado creado");
    }
    
    private static void CreateFooter(GameObject parent, ExpandableMapController controller)
    {
        // Footer eliminado - controles movidos al panel lateral
        // Solo crear un footer vacío por si se necesita después
        GameObject footer = new GameObject("Footer");
        RectTransform footerRect = footer.AddComponent<RectTransform>();
        footerRect.SetParent(parent.transform, false);
        
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0);
        footerRect.pivot = new Vector2(0.5f, 0);
        footerRect.sizeDelta = new Vector2(0, 0); // Sin altura
        footerRect.anchoredPosition = Vector2.zero;
        
        Debug.Log("[ExpandableMapSetup] ? Footer placeholder creado");
    }
    
    private static GameObject CreateButton(Transform parent, string text, Vector2 position, Vector2 anchor)
    {
        GameObject btn = new GameObject("Button_" + text.Replace(" ", "").Replace("??", "Centrar"));
        RectTransform btnRect = btn.AddComponent<RectTransform>();
        btnRect.SetParent(parent, false);
        btnRect.anchorMin = anchor;
        btnRect.anchorMax = anchor;
        btnRect.pivot = new Vector2(0, 0.5f);
        btnRect.sizeDelta = new Vector2(180, 80);
        btnRect.anchoredPosition = position;
        
        Image btnImg = btn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.4f, 0.8f, 1f);
        
        btn.AddComponent<Button>();
        
        GameObject btnText = new GameObject("Text");
        TextMeshProUGUI tmpText = btnText.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 28;
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
    
    private static void CreateExpandButton(GameObject compactPanel, ExpandableMapController controller)
    {
        // Eliminar existente
        Transform existing = compactPanel.transform.Find("ExpandButton");
        if (existing != null)
        {
            Object.DestroyImmediate(existing.gameObject);
        }
        
        GameObject expandBtn = new GameObject("ExpandButton");
        RectTransform btnRect = expandBtn.AddComponent<RectTransform>();
        btnRect.SetParent(compactPanel.transform, false);
        
        btnRect.anchorMin = new Vector2(1, 0);
        btnRect.anchorMax = new Vector2(1, 0);
        btnRect.pivot = new Vector2(1, 0);
        btnRect.sizeDelta = new Vector2(60, 60);
        btnRect.anchoredPosition = new Vector2(-5, 5);
        
        Image btnImg = expandBtn.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 1f, 0.9f);
        
        Button button = expandBtn.AddComponent<Button>();
        controller.expandButton = button;
        
        GameObject btnText = new GameObject("Text");
        TextMeshProUGUI tmpText = btnText.AddComponent<TextMeshProUGUI>();
        tmpText.text = "??";
        tmpText.fontSize = 36;
        tmpText.alignment = TextAlignmentOptions.Center;
        tmpText.color = Color.white;
        
        RectTransform textRect = btnText.GetComponent<RectTransform>();
        textRect.SetParent(expandBtn.transform, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        Debug.Log("[ExpandableMapSetup] ? Botón expandir creado");
    }
}
