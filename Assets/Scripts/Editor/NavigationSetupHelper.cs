using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script de utilidad para configurar rápidamente el sistema de navegación
/// Solo disponible en el Editor de Unity
/// </summary>
public class NavigationSetupHelper : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("AR Tools/Navigation/Quick Setup")]
    public static void QuickSetup()
    {
        Debug.Log("[NavigationSetup] ?? Iniciando configuración rápida...");
        
        // 1. Crear LocationManager si no existe
        LocationManager locationMgr = FindAnyObjectByType<LocationManager>();
        if (locationMgr == null)
        {
            GameObject locObj = new GameObject("LocationManager");
            locationMgr = locObj.AddComponent<LocationManager>();
            Debug.Log("[NavigationSetup] ? LocationManager creado");
        }
        
        // 2. Crear AppModeManager si no existe
        AppModeManager appModeMgr = FindAnyObjectByType<AppModeManager>();
        if (appModeMgr == null)
        {
            GameObject appObj = new GameObject("AppModeManager");
            appModeMgr = appObj.AddComponent<AppModeManager>();
            Debug.Log("[NavigationSetup] ? AppModeManager creado");
        }
        
        // 3. Crear NavigationController si no existe
        NavigationArrowController navController = FindAnyObjectByType<NavigationArrowController>();
        if (navController == null)
        {
            GameObject navObj = new GameObject("NavigationController");
            navController = navObj.AddComponent<NavigationArrowController>();
            
            // Intentar asignar prefab de flecha
            GameObject arrowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Flecha-Prefab.prefab");
            if (arrowPrefab != null)
            {
                navController.arrowPrefab = arrowPrefab;
                Debug.Log("[NavigationSetup] ? Prefab de flecha asignado");
            }
            
            // Asignar cámara
            navController.arCamera = Camera.main;
            
            Debug.Log("[NavigationSetup] ? NavigationController creado");
        }
        
        // 4. Conectar referencias en AppModeManager
        if (appModeMgr != null)
        {
            appModeMgr.arTrackedImageManager = FindAnyObjectByType<UnityEngine.XR.ARFoundation.ARTrackedImageManager>();
            appModeMgr.multiImageSpawner = FindAnyObjectByType<MultiImageSpawner>();
            appModeMgr.navigationController = navController;
            
            Debug.Log("[NavigationSetup] ? Referencias conectadas en AppModeManager");
        }
        
        Debug.Log("[NavigationSetup] ? Configuración básica completada!");
        Debug.Log("[NavigationSetup] ?? Todavía necesitas crear la UI manualmente (ver NAVEGACION_AR_SETUP.md)");
    }
    
    [MenuItem("AR Tools/Navigation/Create Debug Panel")]
    public static void CreateDebugPanel()
    {
        // Buscar o crear canvas
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("DebugCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Crear EventSystem si no existe
            if (FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            
            Debug.Log("[NavigationSetup] ? Canvas creado");
        }
        
        // Crear NavigationDebugPanel
        GameObject debugObj = new GameObject("NavigationDebugPanel");
        debugObj.transform.SetParent(canvas.transform, false);
        debugObj.AddComponent<NavigationDebugPanel>();
        
        Debug.Log("[NavigationSetup] ? Panel de debug creado. Presiona 'D' para mostrar/ocultar");
    }
    
    [MenuItem("AR Tools/Navigation/Verify Setup")]
    public static void VerifySetup()
    {
        Debug.Log("[NavigationSetup] ?? Verificando configuración...");
        Debug.Log("????????????????????????????????????????");
        
        // Verificar componentes principales
        bool hasLocationManager = FindAnyObjectByType<LocationManager>() != null;
        bool hasAppModeManager = FindAnyObjectByType<AppModeManager>() != null;
        bool hasNavigationController = FindAnyObjectByType<NavigationArrowController>() != null;
        bool hasFirebaseManager = FindAnyObjectByType<FirebaseManager>() != null;
        bool hasMultiImageSpawner = FindAnyObjectByType<MultiImageSpawner>() != null;
        
        Debug.Log($"LocationManager: {(hasLocationManager ? "?" : "?")}");
        Debug.Log($"AppModeManager: {(hasAppModeManager ? "?" : "?")}");
        Debug.Log($"NavigationController: {(hasNavigationController ? "?" : "?")}");
        Debug.Log($"FirebaseManager: {(hasFirebaseManager ? "?" : "?")}");
        Debug.Log($"MultiImageSpawner: {(hasMultiImageSpawner ? "?" : "?")}");
        
        // Verificar prefab de flecha
        GameObject arrowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Flecha-Prefab.prefab");
        Debug.Log($"Prefab de Flecha: {(arrowPrefab != null ? "?" : "?")}");
        
        // Verificar prefab de botón
        GameObject buttonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LocationButton.prefab");
        Debug.Log($"Prefab de Botón: {(buttonPrefab != null ? "?" : "?")}");
        
        Debug.Log("????????????????????????????????????????");
        
        if (hasLocationManager && hasAppModeManager && hasNavigationController && hasFirebaseManager)
        {
            Debug.Log("[NavigationSetup] ? Componentes principales OK");
        }
        else
        {
            Debug.LogWarning("[NavigationSetup] ?? Faltan componentes. Usa: AR Tools > Navigation > Quick Setup");
        }
        
        // Verificar referencias en AppModeManager
        if (hasAppModeManager)
        {
            AppModeManager mgr = FindAnyObjectByType<AppModeManager>();
            Debug.Log("\n[AppModeManager] Referencias:");
            Debug.Log($"  ARTrackedImageManager: {(mgr.arTrackedImageManager != null ? "?" : "?")}");
            Debug.Log($"  MultiImageSpawner: {(mgr.multiImageSpawner != null ? "?" : "?")}");
            Debug.Log($"  NavigationController: {(mgr.navigationController != null ? "?" : "?")}");
            Debug.Log($"  NavigationUIManager: {(mgr.navigationUIManager != null ? "? (Requiere UI manual)" : "?")}");
        }
        
        // Verificar referencias en NavigationController
        if (hasNavigationController)
        {
            NavigationArrowController nav = FindAnyObjectByType<NavigationArrowController>();
            Debug.Log("\n[NavigationController] Referencias:");
            Debug.Log($"  Arrow Prefab: {(nav.arrowPrefab != null ? "?" : "?")}");
            Debug.Log($"  AR Camera: {(nav.arCamera != null ? "?" : "?")}");
        }
    }
    
    [MenuItem("AR Tools/Navigation/Open Setup Guide")]
    public static void OpenSetupGuide()
    {
        string path = "Assets/NAVEGACION_AR_SETUP.md";
        if (System.IO.File.Exists(path))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
            Debug.Log("[NavigationSetup] ?? Abriendo guía de configuración...");
        }
        else
        {
            Debug.LogWarning("[NavigationSetup] ?? No se encontró NAVEGACION_AR_SETUP.md");
        }
    }
    
    #endif
}
