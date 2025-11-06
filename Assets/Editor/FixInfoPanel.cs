using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class FixInfoPanel : EditorWindow
{
    [MenuItem("AR Tools/?? Reparar InfoPanel")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog(
            "Reparar InfoPanel",
            "Esto recreará el prefab InfoPanel con la configuración correcta.\n\n" +
            "ESTO SOBRESCRIBIRÁ el prefab actual.\n\n¿Continuar?",
            "Sí, reparar",
            "Cancelar"))
        {
            CreateInfoPanel();
        }
    }
    
    [MenuItem("AR Tools/?? Forzar Alpha 1.0 en InfoPanel")]
    public static void ForceAlphaOne()
    {
        // Cargar el prefab existente
        string path = "Assets/Prefabs/InfoPanel.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", "No se encontró InfoPanel.prefab en Assets/Prefabs/", "OK");
            return;
        }
        
        // Instanciar el prefab para editarlo
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        
        if (instance == null)
        {
            EditorUtility.DisplayDialog("Error", "No se pudo instanciar el prefab", "OK");
            return;
        }
        
        int changesCount = 0;
        
        // Forzar alpha 1.0 en TODOS los componentes Image
        Image[] images = instance.GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            Color c = img.color;
            if (c.a != 1f)
            {
                img.color = new Color(c.r, c.g, c.b, 1f);
                changesCount++;
                Debug.Log($"? Forzado alpha 1.0 en Image: {img.gameObject.name}");
            }
        }
        
        // Forzar alpha 1.0 en TODOS los componentes TextMeshProUGUI
        TextMeshProUGUI[] texts = instance.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI txt in texts)
        {
            Color c = txt.color;
            if (c.a != 1f)
            {
                txt.color = new Color(c.r, c.g, c.b, 1f);
                changesCount++;
                Debug.Log($"? Forzado alpha 1.0 en Text: {txt.gameObject.name}");
            }
        }
        
        // Forzar alpha 1.0 en TODOS los CanvasGroup
        CanvasGroup[] canvasGroups = instance.GetComponentsInChildren<CanvasGroup>(true);
        foreach (CanvasGroup cg in canvasGroups)
        {
            if (cg.alpha != 1f)
            {
                cg.alpha = 1f;
                changesCount++;
                Debug.Log($"? Forzado alpha 1.0 en CanvasGroup: {cg.gameObject.name}");
            }
        }
        
        // Si no hay CanvasGroup, agregar uno al Canvas y ponerlo en 1.0
        Canvas canvas = instance.GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
            if (cg == null)
            {
                cg = canvas.gameObject.AddComponent<CanvasGroup>();
                changesCount++;
                Debug.Log($"? Agregado CanvasGroup al Canvas");
            }
            cg.alpha = 1f;
            Debug.Log($"? Forzado alpha 1.0 en CanvasGroup principal");
        }
        
        // Guardar cambios al prefab
        PrefabUtility.SaveAsPrefabAsset(instance, path);
        DestroyImmediate(instance);
        
        Debug.Log($"???????????????????????????????????????????????");
        Debug.Log($"? ALPHA 1.0 FORZADO EN TODO EL PREFAB");
        Debug.Log($"   Total de cambios: {changesCount}");
        Debug.Log($"???????????????????????????????????????????????");
        
        EditorUtility.DisplayDialog(
            "? Éxito", 
            $"Alpha 1.0 forzado en {changesCount} componentes!\n\n" +
            "El prefab ahora es 100% OPACO en TODOS sus elementos.\n\n" +
            "Build and Run para ver los cambios.",
            "OK"
        );
        
        // Seleccionar el prefab
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }

    static void CreateInfoPanel()
    {
        // Crear GameObject raíz
        GameObject root = new GameObject("InfoPanel");
        root.transform.position = new Vector3(0, 0.15f, 0);
        
        // Agregar InfoPanelController
        var controller = root.AddComponent<InfoPanelController>();
        
        // Crear Canvas
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(root.transform);
        canvasGO.transform.localPosition = Vector3.zero;
        canvasGO.transform.localRotation = Quaternion.identity;
        canvasGO.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        canvasGO.layer = 5; // UI layer
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // AGREGAR CANVASGROUP CON ALPHA 1.0
        CanvasGroup canvasGroup = canvasGO.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 1;
        
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(500, 350);
        
        // Crear BackgroundPanel
        GameObject bgPanel = new GameObject("BackgroundPanel");
        bgPanel.transform.SetParent(canvasGO.transform);
        bgPanel.transform.localPosition = Vector3.zero;
        bgPanel.transform.localRotation = Quaternion.identity;
        bgPanel.transform.localScale = Vector3.one;
        bgPanel.layer = 5;
        
        RectTransform bgRect = bgPanel.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(480, 330);
        bgRect.pivot = new Vector2(0.5f, 0.5f);
        
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // ALPHA 1.0 - SÓLIDO
        bgImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
        bgImage.type = Image.Type.Sliced;
        
        // Crear TitleText
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(bgPanel.transform);
        titleGO.transform.localPosition = Vector3.zero;
        titleGO.transform.localRotation = Quaternion.identity;
        titleGO.transform.localScale = Vector3.one;
        titleGO.layer = 5;
        
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -30);
        titleRect.sizeDelta = new Vector2(-40, 40);
        
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "Nombre del Edificio";
        titleText.fontSize = 28;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 1f, 1f, 1f); // ALPHA 1.0
        titleText.enableWordWrapping = true;
        
        // Crear DescriptionText
        GameObject descGO = new GameObject("DescriptionText");
        descGO.transform.SetParent(bgPanel.transform);
        descGO.transform.localPosition = Vector3.zero;
        descGO.transform.localRotation = Quaternion.identity;
        descGO.transform.localScale = Vector3.one;
        descGO.layer = 5;
        
        RectTransform descRect = descGO.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.pivot = new Vector2(0.5f, 0.5f);
        descRect.anchoredPosition = new Vector2(0, -10);
        descRect.sizeDelta = new Vector2(-40, -120);
        
        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
        descText.text = "Descripción detallada del edificio o lugar. Aquí se mostrará la información obtenida desde Firebase Firestore.";
        descText.fontSize = 18;
        descText.alignment = TextAlignmentOptions.TopLeft;
        descText.color = new Color(0.9f, 0.9f, 0.9f, 1f); // ALPHA 1.0
        descText.enableWordWrapping = true;
        
        // Crear CoordinatesText
        GameObject coordsGO = new GameObject("CoordinatesText");
        coordsGO.transform.SetParent(bgPanel.transform);
        coordsGO.transform.localPosition = Vector3.zero;
        coordsGO.transform.localRotation = Quaternion.identity;
        coordsGO.transform.localScale = Vector3.one;
        coordsGO.layer = 5;
        
        RectTransform coordsRect = coordsGO.AddComponent<RectTransform>();
        coordsRect.anchorMin = new Vector2(0, 0);
        coordsRect.anchorMax = new Vector2(1, 0);
        coordsRect.pivot = new Vector2(0.5f, 0);
        coordsRect.anchoredPosition = new Vector2(0, 20);
        coordsRect.sizeDelta = new Vector2(-40, 30);
        
        TextMeshProUGUI coordsText = coordsGO.AddComponent<TextMeshProUGUI>();
        coordsText.text = "?? Lat: 13.718103, Lon: -89.204092";
        coordsText.fontSize = 14;
        coordsText.fontStyle = FontStyles.Italic;
        coordsText.alignment = TextAlignmentOptions.Center;
        coordsText.color = new Color(0.7f, 0.7f, 0.7f, 1f); // ALPHA 1.0
        
        // Crear LoadingPanel
        GameObject loadingGO = new GameObject("LoadingPanel");
        loadingGO.transform.SetParent(canvasGO.transform);
        loadingGO.transform.localPosition = Vector3.zero;
        loadingGO.transform.localRotation = Quaternion.identity;
        loadingGO.transform.localScale = Vector3.one;
        loadingGO.SetActive(false);
        loadingGO.layer = 5;
        
        RectTransform loadingRect = loadingGO.AddComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.5f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.5f);
        loadingRect.anchoredPosition = Vector2.zero;
        loadingRect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI loadingText = loadingGO.AddComponent<TextMeshProUGUI>();
        loadingText.text = "? Cargando...";
        loadingText.fontSize = 20;
        loadingText.alignment = TextAlignmentOptions.Center;
        loadingText.color = new Color(1f, 1f, 1f, 1f); // ALPHA 1.0
        
        // Asignar referencias al controller
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.coordinatesText = coordsText;
        controller.loadingPanel = loadingGO;
        controller.loadingText = loadingText;
        controller.enableFadeIn = false;
        controller.lookAtCamera = false;
        
        // Guardar como prefab
        string path = "Assets/Prefabs/InfoPanel.prefab";
        
        // Crear carpeta si no existe
        if (!System.IO.Directory.Exists("Assets/Prefabs"))
        {
            System.IO.Directory.CreateDirectory("Assets/Prefabs");
            AssetDatabase.Refresh();
        }
        
        PrefabUtility.SaveAsPrefabAsset(root, path);
        
        DestroyImmediate(root);
        
        Debug.Log($"???????????????????????????????????????????????");
        Debug.Log($"? InfoPanel recreado exitosamente");
        Debug.Log($"   ? Fondo: Gris oscuro ALPHA 1.0 (100% SÓLIDO)");
        Debug.Log($"   ? Textos: Todos con ALPHA 1.0");
        Debug.Log($"   ? CanvasGroup: ALPHA 1.0");
        Debug.Log($"   ? Controller: Referencias asignadas");
        Debug.Log($"???????????????????????????????????????????????");
        
        EditorUtility.DisplayDialog(
            "? Éxito", 
            "InfoPanel recreado con ALPHA 1.0 en TODOS los elementos!\n\n" +
            "Ahora asegúrate de:\n" +
            "1. Asignar el prefab en MultiImageSpawner\n" +
            "2. Hacer Build and Run\n" +
            "3. El panel será 100% OPACO y SÓLIDO",
            "OK"
        );
        
        // Seleccionar el prefab
        Object prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
}
