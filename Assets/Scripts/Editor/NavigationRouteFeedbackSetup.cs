using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endif

/// <summary>
/// Utilidad para agregar el indicador de estado de ruta automáticamente.
/// </summary>
public static class NavigationRouteFeedbackSetup
{
#if UNITY_EDITOR
    private const string PrefabPath = "Assets/Prefabs/RouteStatusIndicator.prefab";
    
    [MenuItem("AR Tools/Navigation/Add Route Feedback UI")]
    public static void AddRouteFeedbackUI()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null)
        {
            Debug.LogError("[RouteFeedbackSetup] No se encontró el prefab RouteStatusIndicator en Assets/Prefabs");
            return;
        }
        
        Canvas targetCanvas = GetOrCreateCanvas();
        if (targetCanvas == null)
        {
            Debug.LogError("[RouteFeedbackSetup] No se pudo crear o encontrar un Canvas para la UI");
            return;
        }
        
        Transform existing = targetCanvas.transform.Find("RouteStatusIndicator");
        if (existing != null)
        {
            Debug.LogWarning("[RouteFeedbackSetup] Ya existe un RouteStatusIndicator en la escena");
            Selection.activeTransform = existing;
            return;
        }
        
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, targetCanvas.transform);
        instance.name = "RouteStatusIndicator";
        RectTransform rect = instance.GetComponent<RectTransform>();
        rect.SetAsLastSibling();
        
        Selection.activeGameObject = instance;
        Debug.Log("[RouteFeedbackSetup] Indicador de estado de ruta agregado correctamente");
    }
    
    private static Canvas GetOrCreateCanvas()
    {
        Canvas navigationCanvas = GameObject.Find("NavigationCanvas")?.GetComponent<Canvas>();
        if (navigationCanvas != null)
        {
            EnsureEventSystem();
            return navigationCanvas;
        }
        
        Canvas anyCanvas = Object.FindObjectOfType<Canvas>();
        if (anyCanvas != null)
        {
            EnsureEventSystem();
            return anyCanvas;
        }
        
        GameObject canvasObj = new GameObject("RouteFeedbackCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        EnsureEventSystem();
        
        Debug.Log("[RouteFeedbackSetup] Canvas nuevo creado para la UI de feedback");
        return canvas;
    }
    
    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
            return;
        
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }
#endif
}



