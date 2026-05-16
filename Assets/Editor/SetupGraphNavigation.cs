using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using TMPro;

/// <summary>
/// Editor script para configurar automáticamente el sistema de navegación por grafo
/// Crea el GameObject GraphNavigationManager y asigna todas las referencias necesarias
/// </summary>
public class SetupGraphNavigation : EditorWindow
{
    [MenuItem("AR Navigation/Setup Graph Navigation System")]
    public static void ShowWindow()
    {
        SetupGraphNavigation window = GetWindow<SetupGraphNavigation>("Setup Graph Navigation");
        window.minSize = new Vector2(400, 300);
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
        
        EditorGUILayout.LabelField("??? Setup Graph Navigation System", titleStyle);
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Este asistente configurará automáticamente el sistema de navegación por grafo:\n\n" +
            "? Creará el GameObject GraphNavigationManager\n" +
            "? Asignará todas las referencias necesarias\n" +
            "? Configurará los parámetros por defecto\n" +
            "? Verificará que todo esté correcto",
            MessageType.Info
        );
        
        GUILayout.Space(20);
        
        // Botón principal
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("?? Configurar Automáticamente", GUILayout.Height(40)))
        {
            SetupGraphNavigationSystem();
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "?? Asegúrate de que estés en la escena correcta antes de ejecutar esto.",
            MessageType.Warning
        );
        
        GUILayout.Space(10);
        
        // Botón de verificación
        if (GUILayout.Button("?? Verificar Configuración Actual", GUILayout.Height(30)))
        {
            VerifyCurrentSetup();
        }
        
        GUILayout.Space(20);
        
        // Información adicional
        EditorGUILayout.LabelField("Documentación:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Después de la configuración, revisa:\n" +
            " Assets/docs/README_NAVEGACION_GRAFO.md\n" +
            " Assets/docs/INSTRUCCIONES_FIRESTORE_GRAFO.md",
            MessageType.Info
        );
        
        EditorGUILayout.EndScrollView();
    }
    
    private static void SetupGraphNavigationSystem()
    {
        Debug.Log("???????????????????????????????????????????????????????");
        Debug.Log("?? INICIANDO CONFIGURACIÓN AUTOMÁTICA DEL SISTEMA");
        Debug.Log("???????????????????????????????????????????????????????");
        
        int stepCount = 0;
        
        // PASO 1: Crear o encontrar GraphNavigationManager
        stepCount++;
        Debug.Log($"\n[{stepCount}] ?? Buscando GraphNavigationManager...");
        
        GraphNavigationManager graphNavManager = FindObjectOfType<GraphNavigationManager>();
        GameObject graphNavObject;
        
        if (graphNavManager == null)
        {
            Debug.Log("   ? No encontrado. Creando nuevo GameObject...");
            graphNavObject = new GameObject("GraphNavigationManager");
            graphNavManager = graphNavObject.AddComponent<GraphNavigationManager>();
            Debug.Log("   ? GameObject creado exitosamente");
        }
        else
        {
            graphNavObject = graphNavManager.gameObject;
            Debug.Log("   ? GraphNavigationManager ya existe");
        }
        
        // PASO 2: Configurar parámetros por defecto
        stepCount++;
        Debug.Log($"\n[{stepCount}] ?? Configurando parámetros...");
        
        SerializedObject so = new SerializedObject(graphNavManager);
        
        SerializedProperty nodeReachedDistance = so.FindProperty("nodeReachedDistance");
        SerializedProperty updateInterval = so.FindProperty("updateInterval");
        
        if (nodeReachedDistance != null)
        {
            nodeReachedDistance.floatValue = 10f;
            Debug.Log("   ? nodeReachedDistance = 10m");
        }
        
        if (updateInterval != null)
        {
            updateInterval.floatValue = 1f;
            Debug.Log("   ? updateInterval = 1s");
        }
        
        so.ApplyModifiedProperties();
        
        // PASO 3: Buscar y asignar AppModeManager
        stepCount++;
        Debug.Log($"\n[{stepCount}] ?? Buscando AppModeManager...");
        
        AppModeManager appModeManager = FindObjectOfType<AppModeManager>();
        
        if (appModeManager != null)
        {
            SerializedObject appModeSO = new SerializedObject(appModeManager);
            SerializedProperty graphNavManagerProp = appModeSO.FindProperty("graphNavigationManager");
            
            if (graphNavManagerProp != null)
            {
                graphNavManagerProp.objectReferenceValue = graphNavManager;
                appModeSO.ApplyModifiedProperties();
                Debug.Log("   ? GraphNavigationManager asignado en AppModeManager");
            }
            else
            {
                Debug.LogWarning("   ?? No se encontró el campo graphNavigationManager en AppModeManager");
            }
        }
        else
        {
            Debug.LogWarning("   ?? AppModeManager no encontrado en la escena");
        }
        
        // PASO 4: Buscar y configurar NavigationArrowController
        stepCount++;
        Debug.Log($"\n[{stepCount}] ?? Buscando NavigationArrowController...");
        
        NavigationArrowController arrowController = FindObjectOfType<NavigationArrowController>();
        
        if (arrowController != null)
        {
            Debug.Log("   ? NavigationArrowController encontrado");
            
            // Intentar encontrar un TextMeshProUGUI para progressText si no está asignado
            SerializedObject arrowSO = new SerializedObject(arrowController);
            SerializedProperty progressTextProp = arrowSO.FindProperty("progressText");
            
            if (progressTextProp != null && progressTextProp.objectReferenceValue == null)
            {
                Debug.Log("   ?? Buscando TextMeshProUGUI para progressText...");
                
                TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
                TextMeshProUGUI progressText = null;
                
                // Buscar por nombre
                foreach (var text in allTexts)
                {
                    if (text.gameObject.name.ToLower().Contains("progress") ||
                        text.gameObject.name.ToLower().Contains("nodo"))
                    {
                        progressText = text;
                        break;
                    }
                }
                
                if (progressText != null)
                {
                    progressTextProp.objectReferenceValue = progressText;
                    arrowSO.ApplyModifiedProperties();
                    Debug.Log($"   ? progressText asignado automáticamente: {progressText.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("   ?? No se encontró un TextMeshProUGUI apropiado para progressText");
                    Debug.LogWarning("   ?? Asígnalo manualmente si lo necesitas");
                }
            }
            else if (progressTextProp != null && progressTextProp.objectReferenceValue != null)
            {
                Debug.Log("   ? progressText ya está asignado");
            }
        }
        else
        {
            Debug.LogWarning("   ?? NavigationArrowController no encontrado en la escena");
        }
        
        // PASO 5: Verificar FirebaseManager
        stepCount++;
        Debug.Log($"\n[{stepCount}] ?? Verificando FirebaseManager...");
        
        FirebaseManager firebaseManager = FindObjectOfType<FirebaseManager>();
        
        if (firebaseManager != null)
        {
            SerializedObject fbSO = new SerializedObject(firebaseManager);
            
            SerializedProperty collectionName = fbSO.FindProperty("collectionName");
            SerializedProperty graphEdgesCollectionName = fbSO.FindProperty("graphEdgesCollectionName");
            
            if (collectionName != null)
            {
                if (string.IsNullOrEmpty(collectionName.stringValue))
                {
                    collectionName.stringValue = "buildingLocations";
                    Debug.Log("   ? collectionName = 'buildingLocations'");
                }
                else
                {
                    Debug.Log($"   ? collectionName = '{collectionName.stringValue}'");
                }
            }
            
            if (graphEdgesCollectionName != null)
            {
                if (string.IsNullOrEmpty(graphEdgesCollectionName.stringValue))
                {
                    graphEdgesCollectionName.stringValue = "graphEdges";
                    Debug.Log("   ? graphEdgesCollectionName = 'graphEdges'");
                }
                else
                {
                    Debug.Log($"   ? graphEdgesCollectionName = '{graphEdgesCollectionName.stringValue}'");
                }
            }
            
            fbSO.ApplyModifiedProperties();
        }
        else
        {
            Debug.LogWarning("   ?? FirebaseManager no encontrado en la escena");
        }
        
        // PASO 6: Marcar la escena como modificada
        EditorUtility.SetDirty(graphNavObject);
        if (appModeManager != null) EditorUtility.SetDirty(appModeManager);
        if (arrowController != null) EditorUtility.SetDirty(arrowController);
        if (firebaseManager != null) EditorUtility.SetDirty(firebaseManager);
        
        // PASO 7: Resumen final
        Debug.Log("\n???????????????????????????????????????????????????????");
        Debug.Log("? CONFIGURACIÓN COMPLETADA");
        Debug.Log("???????????????????????????????????????????????????????");
        Debug.Log("\n?? RESUMEN:");
        Debug.Log($"   ? GraphNavigationManager: {(graphNavManager != null ? "Configurado" : "No configurado")}");
        Debug.Log($"   ? AppModeManager: {(appModeManager != null ? "Configurado" : "No encontrado")}");
        Debug.Log($"   ? NavigationArrowController: {(arrowController != null ? "Configurado" : "No encontrado")}");
        Debug.Log($"   ? FirebaseManager: {(firebaseManager != null ? "Configurado" : "No encontrado")}");
        
        Debug.Log("\n?? PRÓXIMOS PASOS:");
        Debug.Log("   1. Configura Firestore (ver: INSTRUCCIONES_FIRESTORE_GRAFO.md)");
        Debug.Log("   2. Agrega campo 'type' a buildingLocations");
        Debug.Log("   3. Crea collection 'graphEdges'");
        Debug.Log("   4. Build and Run");
        
        Debug.Log("\n???????????????????????????????????????????????????????\n");
        
        EditorUtility.DisplayDialog(
            "Configuración Completada ?",
            "El sistema de navegación por grafo ha sido configurado exitosamente.\n\n" +
            "Revisa la consola para ver el resumen completo.\n\n" +
            "Próximo paso:\n" +
            "Configura Firestore según INSTRUCCIONES_FIRESTORE_GRAFO.md",
            "Entendido"
        );
        
        // Seleccionar el objeto creado
        Selection.activeGameObject = graphNavObject;
    }
    
    private static void VerifyCurrentSetup()
    {
        Debug.Log("???????????????????????????????????????????????????????");
        Debug.Log("?? VERIFICANDO CONFIGURACIÓN ACTUAL");
        Debug.Log("???????????????????????????????????????????????????????\n");
        
        bool allGood = true;
        
        // Verificar GraphNavigationManager
        GraphNavigationManager graphNavManager = FindObjectOfType<GraphNavigationManager>();
        
        if (graphNavManager != null)
        {
            Debug.Log("? GraphNavigationManager encontrado");
            Debug.Log($"    Node Reached Distance: {graphNavManager.nodeReachedDistance}m");
            Debug.Log($"    Update Interval: {graphNavManager.updateInterval}s");
        }
        else
        {
            Debug.LogError("? GraphNavigationManager NO encontrado");
            allGood = false;
        }
        
        // Verificar AppModeManager
        AppModeManager appModeManager = FindObjectOfType<AppModeManager>();
        
        if (appModeManager != null)
        {
            Debug.Log("? AppModeManager encontrado");
            
            SerializedObject so = new SerializedObject(appModeManager);
            SerializedProperty graphNavProp = so.FindProperty("graphNavigationManager");
            
            if (graphNavProp != null && graphNavProp.objectReferenceValue != null)
            {
                Debug.Log("   ? graphNavigationManager está asignado");
            }
            else
            {
                Debug.LogWarning("   ?? graphNavigationManager NO está asignado");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("? AppModeManager NO encontrado");
            allGood = false;
        }
        
        // Verificar NavigationArrowController
        NavigationArrowController arrowController = FindObjectOfType<NavigationArrowController>();
        
        if (arrowController != null)
        {
            Debug.Log("? NavigationArrowController encontrado");
            
            SerializedObject so = new SerializedObject(arrowController);
            SerializedProperty progressTextProp = so.FindProperty("progressText");
            
            if (progressTextProp != null && progressTextProp.objectReferenceValue != null)
            {
                Debug.Log("   ? progressText está asignado");
            }
            else
            {
                Debug.LogWarning("   ?? progressText NO está asignado (opcional)");
            }
        }
        else
        {
            Debug.LogError("? NavigationArrowController NO encontrado");
            allGood = false;
        }
        
        // Verificar FirebaseManager
        FirebaseManager firebaseManager = FindObjectOfType<FirebaseManager>();
        
        if (firebaseManager != null)
        {
            Debug.Log("? FirebaseManager encontrado");
            
            SerializedObject so = new SerializedObject(firebaseManager);
            SerializedProperty graphEdgesCollection = so.FindProperty("graphEdgesCollectionName");
            
            if (graphEdgesCollection != null && !string.IsNullOrEmpty(graphEdgesCollection.stringValue))
            {
                Debug.Log($"   ? graphEdgesCollectionName = '{graphEdgesCollection.stringValue}'");
            }
            else
            {
                Debug.LogWarning("   ?? graphEdgesCollectionName NO está configurado");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("? FirebaseManager NO encontrado");
            allGood = false;
        }
        
        Debug.Log("\n???????????????????????????????????????????????????????");
        
        if (allGood)
        {
            Debug.Log("? VERIFICACIÓN EXITOSA - Todo está configurado correctamente");
            
            EditorUtility.DisplayDialog(
                "Verificación Exitosa ?",
                "Todos los componentes están configurados correctamente.\n\n" +
                "El sistema está listo para usarse.\n\n" +
                "Siguiente paso:\n" +
                "Configurar Firestore y hacer Build and Run",
                "Excelente"
            );
        }
        else
        {
            Debug.LogWarning("?? VERIFICACIÓN INCOMPLETA - Hay componentes faltantes o mal configurados");
            
            EditorUtility.DisplayDialog(
                "Verificación Incompleta ??",
                "Algunos componentes no están configurados correctamente.\n\n" +
                "Revisa la consola para ver los detalles.\n\n" +
                "Ejecuta 'Configurar Automáticamente' para corregir.",
                "Entendido"
            );
        }
        
        Debug.Log("???????????????????????????????????????????????????????\n");
    }
}
