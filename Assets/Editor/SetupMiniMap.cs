using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Editor script para configurar automáticamente el sistema de minimapa
/// </summary>
public class SetupMiniMap : EditorWindow
{
    [MenuItem("AR Navigation/Setup MiniMap System")]
    public static void ShowWindow()
    {
        SetupMiniMap window = GetWindow<SetupMiniMap>("Setup MiniMap");
        window.minSize = new Vector2(450, 400);
    }
    
    private Vector2 scrollPosition;
    
    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        GUILayout.Space(10);
        
        // Título
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
        titleStyle.fontSize = 16;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        
        EditorGUILayout.LabelField("??? Setup MiniMap System", titleStyle);
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Este asistente configurará automáticamente el minimapa del grafo:\n\n" +
            "? Creará el Canvas del minimapa\n" +
            "? Configurará el MiniMapController\n" +
            "? Creará el prefab de nodos\n" +
            "? Añadirá botón de toggle para mostrar/ocultar\n" +
            "? Lo integrará con el sistema de navegación existente",
            MessageType.Info
        );
        
        GUILayout.Space(20);
        
        // Botón principal
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("?? Configurar MiniMap Automáticamente", GUILayout.Height(40)))
        {
            SetupMiniMapSystem();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(10);
        
        // Botón de verificación
        if (GUILayout.Button("?? Verificar Configuración Actual", GUILayout.Height(30)))
        {
            VerifySetup();
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.LabelField("Opciones Adicionales:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("?? Personalizar Colores", GUILayout.Height(30)))
        {
            PersonalizeColors();
        }
        
        if (GUILayout.Button("??? Eliminar MiniMap", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Confirmar", 
                "¿Estás seguro de que quieres eliminar el minimapa?", "Sí", "No"))
            {
                RemoveMiniMap();
            }
        }
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "?? Tip: El minimapa aparecerá en la esquina superior derecha.\n" +
            "Puedes moverlo editando el Canvas después de la configuración.",
            MessageType.Info
        );
        
        EditorGUILayout.EndScrollView();
    }
    
    private static void SetupMiniMapSystem()
    {
        Debug.Log("???????????????????????????????????????????????????");
        Debug.Log("??? CONFIGURANDO SISTEMA DE MINIMAPA");
        Debug.Log("???????????????????????????????????????????????????");
        
        int step = 0;
        
        // PASO 1: Crear Canvas del minimapa
        step++;
        Debug.Log($"\n[{step}] ?? Creando Canvas del minimapa...");
        
        GameObject miniMapCanvas = CreateMiniMapCanvas();
        
        if (miniMapCanvas == null)
        {
            Debug.LogError("? Error al crear el canvas");
            return;
        }
        
        Debug.Log("   ? Canvas creado");
        
        // PASO 2: Crear prefab de nodo
        step++;
        Debug.Log($"\n[{step}] ?? Creando prefab de nodo...");
        
        GameObject nodePrefab = CreateNodeDotPrefab();
        Debug.Log("   ? Prefab de nodo creado");
        
        // PASO 3: Configurar MiniMapController
        step++;
        Debug.Log($"\n[{step}] ?? Configurando MiniMapController...");
        
        MiniMapController controller = miniMapCanvas.GetComponent<MiniMapController>();
        
        if (controller != null)
        {
            RectTransform mapContainer = miniMapCanvas.transform.Find("MapPanel/MapContainer") as RectTransform;
            
            SerializedObject so = new SerializedObject(controller);
            
            so.FindProperty("mapContainer").objectReferenceValue = mapContainer;
            so.FindProperty("nodeDotPrefab").objectReferenceValue = nodePrefab;
            
            // Configurar colores por defecto
            so.FindProperty("buildingNodeColor").colorValue = new Color(0.2f, 0.6f, 1f, 1f);
            so.FindProperty("inflectionNodeColor").colorValue = new Color(0.8f, 0.8f, 0.8f, 1f);
            so.FindProperty("edgeColor").colorValue = new Color(0.5f, 0.5f, 0.5f, 0.3f);
            so.FindProperty("activePathColor").colorValue = new Color(0f, 1f, 0f, 0.8f);
            so.FindProperty("currentTargetColor").colorValue = new Color(1f, 0.5f, 0f, 1f);
            so.FindProperty("userPositionColor").colorValue = new Color(1f, 0f, 0f, 1f);
            
            so.FindProperty("nodeSize").floatValue = 8f;
            so.FindProperty("lineWidth").floatValue = 2f;
            so.FindProperty("activePathWidth").floatValue = 4f;
            so.FindProperty("mapPadding").floatValue = 20f;
            so.FindProperty("updateInterval").floatValue = 0.5f;
            so.FindProperty("showMiniMap").boolValue = true;
            
            so.ApplyModifiedProperties();
            
            Debug.Log("   ? MiniMapController configurado");
        }
        
        // PASO 4: Añadir botón de toggle
        step++;
        Debug.Log($"\n[{step}] ?? Añadiendo botón de toggle...");
        
        AddToggleButton(miniMapCanvas, controller);
        Debug.Log("   ? Botón de toggle añadido");
        
        // PASO 5: Marcar como modificado
        EditorUtility.SetDirty(miniMapCanvas);
        if (nodePrefab != null) EditorUtility.SetDirty(nodePrefab);
        
        // Resumen
        Debug.Log("\n???????????????????????????????????????????????????");
        Debug.Log("? CONFIGURACIÓN COMPLETADA");
        Debug.Log("???????????????????????????????????????????????????");
        Debug.Log("\n?? RESUMEN:");
        Debug.Log($"   ? Canvas del minimapa creado");
        Debug.Log($"   ? MiniMapController configurado");
        Debug.Log($"   ? Prefab de nodos creado");
        Debug.Log($"   ? Botón de toggle añadido");
        
        Debug.Log("\n?? SIGUIENTE PASO:");
        Debug.Log("   1. Presiona Play ??");
        Debug.Log("   2. Inicia navegación");
        Debug.Log("   3. Observa el minimapa en la esquina superior derecha");
        Debug.Log("   4. Usa el botón 'Minimapa' para mostrar/ocultar");
        
        Debug.Log("\n???????????????????????????????????????????????????\n");
        
        EditorUtility.DisplayDialog(
            "MiniMap Configurado ?",
            "El sistema de minimapa ha sido configurado exitosamente.\n\n" +
            "El minimapa aparecerá en la esquina superior derecha cuando:\n" +
            "• Los datos del grafo estén cargados\n" +
            "• Inicies una navegación\n\n" +
            "Características:\n" +
            "? Muestra todos los nodos del grafo\n" +
            "? Muestra todas las conexiones\n" +
            "? Resalta la ruta activa en verde\n" +
            "? Muestra tu posición en rojo\n" +
            "? Marca el objetivo actual en naranja\n\n" +
            "Presiona Play para probarlo!",
            "Excelente!"
        );
        
        Selection.activeGameObject = miniMapCanvas;
    }
    
    private static GameObject CreateMiniMapCanvas()
    {
        // Buscar si ya existe
        MiniMapController existing = Object.FindObjectOfType<MiniMapController>();
        if (existing != null)
        {
            Debug.Log("   ?? MiniMapCanvas ya existe, usando el existente");
            return existing.gameObject;
        }
        
        // Crear Canvas principal
        GameObject canvasObj = new GameObject("MiniMapCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Asegurar que esté encima
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Añadir MiniMapController al canvas
        canvasObj.AddComponent<MiniMapController>();
        
        // Crear panel de fondo
        GameObject panelObj = new GameObject("MapPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        
        // CAMBIO: Posición configurable del minimapa
        // Para esquina superior derecha MÁS ABAJO:
        panelRect.anchorMin = new Vector2(1, 1); // Esquina superior derecha
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(1, 1);
        panelRect.anchoredPosition = new Vector2(-20, -100); // ? CAMBIO AQUÍ: -100 en vez de -20 (más abajo)
        panelRect.sizeDelta = new Vector2(300, 300); // 300x300px
        
        // Si quieres esquina INFERIOR derecha, usa esto en su lugar:
        // panelRect.anchorMin = new Vector2(1, 0); // Esquina inferior derecha
        // panelRect.anchorMax = new Vector2(1, 0);
        // panelRect.pivot = new Vector2(1, 0);
        // panelRect.anchoredPosition = new Vector2(-20, 20); // 20px desde abajo
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f); // Fondo semi-transparente
        
        // Crear contenedor del mapa
        GameObject containerObj = new GameObject("MapContainer");
        containerObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.offsetMin = Vector2.zero;
        containerRect.offsetMax = Vector2.zero;
        
        // Añadir título
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, 0);
        titleRect.sizeDelta = new Vector2(0, 30);
        
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "??? Mapa";
        titleText.fontSize = 18;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        return canvasObj;
    }
    
    private static GameObject CreateNodeDotPrefab()
    {
        // Crear en Assets/Resources si no existe
        string resourcesPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        string prefabPath = "Assets/Resources/NodeDot.prefab";
        
        // Si ya existe, usarlo
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            Debug.Log("   ?? Usando prefab existente");
            return existingPrefab;
        }
        
        // Crear nuevo prefab
        GameObject dot = new GameObject("NodeDot");
        RectTransform rect = dot.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(8, 8);
        
        Image image = dot.AddComponent<Image>();
        image.color = Color.white;
        
        // Crear sprite circular
        Texture2D circleTexture = CreateCircleTexture(32);
        Sprite circleSprite = Sprite.Create(
            circleTexture,
            new Rect(0, 0, 32, 32),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        image.sprite = circleSprite;
        
        // Guardar como prefab
        PrefabUtility.SaveAsPrefabAsset(dot, prefabPath);
        
        // Destruir el objeto temporal
        Object.DestroyImmediate(dot);
        
        // Cargar el prefab guardado
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        Debug.Log($"   ? Prefab guardado en: {prefabPath}");
        
        return prefab;
    }
    
    private static Texture2D CreateCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[size * size];
        
        float center = size / 2f;
        float radius = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distance = Mathf.Sqrt(dx * dx + dy * dy);
                
                if (distance <= radius)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        
        return texture;
    }
    
    private static void AddToggleButton(GameObject canvas, MiniMapController controller)
    {
        GameObject buttonObj = new GameObject("ToggleMiniMapButton");
        buttonObj.transform.SetParent(canvas.transform, false);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(1, 1);
        buttonRect.anchorMax = new Vector2(1, 1);
        buttonRect.pivot = new Vector2(1, 1);
        buttonRect.anchoredPosition = new Vector2(-20, -340); // Debajo del panel
        buttonRect.sizeDelta = new Vector2(120, 40);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        // Añadir texto
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "??? Mapa";
        buttonText.fontSize = 16;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        // Configurar evento
        if (controller != null)
        {
            button.onClick.AddListener(() => controller.ToggleMiniMap());
        }
    }
    
    private static void VerifySetup()
    {
        Debug.Log("???????????????????????????????????????????????????");
        Debug.Log("?? VERIFICANDO CONFIGURACIÓN DEL MINIMAPA");
        Debug.Log("???????????????????????????????????????????????????\n");
        
        bool allGood = true;
        
        // Verificar MiniMapController
        MiniMapController controller = Object.FindObjectOfType<MiniMapController>();
        
        if (controller != null)
        {
            Debug.Log("? MiniMapController encontrado");
            
            SerializedObject so = new SerializedObject(controller);
            
            if (so.FindProperty("mapContainer").objectReferenceValue != null)
            {
                Debug.Log("   ? mapContainer asignado");
            }
            else
            {
                Debug.LogWarning("   ?? mapContainer NO asignado");
                allGood = false;
            }
            
            if (so.FindProperty("nodeDotPrefab").objectReferenceValue != null)
            {
                Debug.Log("   ? nodeDotPrefab asignado");
            }
            else
            {
                Debug.LogWarning("   ?? nodeDotPrefab NO asignado");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("? MiniMapController NO encontrado");
            allGood = false;
        }
        
        // Verificar GraphNavigationManager
        GraphNavigationManager graphNav = Object.FindObjectOfType<GraphNavigationManager>();
        
        if (graphNav != null)
        {
            Debug.Log("? GraphNavigationManager encontrado");
        }
        else
        {
            Debug.LogWarning("?? GraphNavigationManager NO encontrado (necesario para el minimapa)");
            allGood = false;
        }
        
        Debug.Log("\n???????????????????????????????????????????????????");
        
        if (allGood)
        {
            Debug.Log("? VERIFICACIÓN EXITOSA");
            EditorUtility.DisplayDialog(
                "Verificación Exitosa ?",
                "El minimapa está configurado correctamente.\n\n" +
                "Todo listo para usarse!",
                "Excelente"
            );
        }
        else
        {
            Debug.LogWarning("?? VERIFICACIÓN INCOMPLETA");
            EditorUtility.DisplayDialog(
                "Verificación Incompleta ??",
                "Algunos componentes no están configurados.\n\n" +
                "Ejecuta 'Configurar MiniMap Automáticamente' para corregir.",
                "Entendido"
            );
        }
        
        Debug.Log("???????????????????????????????????????????????????\n");
    }
    
    private static void PersonalizeColors()
    {
        MiniMapController controller = Object.FindObjectOfType<MiniMapController>();
        
        if (controller != null)
        {
            Selection.activeGameObject = controller.gameObject;
            EditorGUIUtility.PingObject(controller);
            
            EditorUtility.DisplayDialog(
                "Personalizar Colores",
                "El MiniMapController está ahora seleccionado en el Inspector.\n\n" +
                "Puedes modificar los colores en la sección 'Configuración Visual':\n\n" +
                "• Building Node Color (nodos de edificios)\n" +
                "• Inflection Node Color (nodos de camino)\n" +
                "• Edge Color (conexiones normales)\n" +
                "• Active Path Color (ruta activa)\n" +
                "• Current Target Color (objetivo actual)\n" +
                "• User Position Color (tu posición)",
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Error",
                "No se encontró el MiniMapController.\n\n" +
                "Ejecuta 'Configurar MiniMap Automáticamente' primero.",
                "OK"
            );
        }
    }
    
    private static void RemoveMiniMap()
    {
        MiniMapController controller = Object.FindObjectOfType<MiniMapController>();
        
        if (controller != null)
        {
            Object.DestroyImmediate(controller.gameObject);
            Debug.Log("??? MiniMap eliminado");
            
            EditorUtility.DisplayDialog(
                "MiniMap Eliminado",
                "El minimapa ha sido eliminado de la escena.",
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "No Encontrado",
                "No se encontró ningún minimapa para eliminar.",
                "OK"
            );
        }
    }
}
