using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Menú de herramientas para el sistema de navegación por grafo
/// Proporciona utilidades para verificar, corregir y documentar el sistema
/// </summary>
public class GraphNavigationTools
{
    [MenuItem("AR Navigation/?? Abrir Documentación/README Principal")]
    public static void OpenReadme()
    {
        OpenFile("Assets/docs/README_NAVEGACION_GRAFO.md");
    }
    
    [MenuItem("AR Navigation/?? Abrir Documentación/Guía Completa")]
    public static void OpenGuia()
    {
        OpenFile("Assets/docs/GUIA_NAVEGACION_GRAFO.md");
    }
    
    [MenuItem("AR Navigation/?? Abrir Documentación/Instrucciones Firestore")]
    public static void OpenInstrucciones()
    {
        OpenFile("Assets/docs/INSTRUCCIONES_FIRESTORE_GRAFO.md");
    }
    
    [MenuItem("AR Navigation/?? Abrir Documentación/Ejemplos Firestore")]
    public static void OpenEjemplos()
    {
        OpenFile("Assets/docs/EJEMPLO_FIRESTORE_GRAFO.md");
    }
    
    [MenuItem("AR Navigation/?? Abrir Documentación/Diagrama Arquitectura")]
    public static void OpenDiagrama()
    {
        OpenFile("Assets/docs/DIAGRAMA_ARQUITECTURA_GRAFO.md");
    }
    
    [MenuItem("AR Navigation/?? Configurar Sistema Completo")]
    public static void ConfigurarSistema()
    {
        SetupGraphNavigation.ShowWindow();
    }
    
    [MenuItem("AR Navigation/?? Verificar Configuración")]
    public static void VerificarConfiguracion()
    {
        VerifySetup();
    }
    
    [MenuItem("AR Navigation/?? Limpiar Cache de Firebase")]
    public static void LimpiarCache()
    {
        FirebaseManager fbManager = Object.FindObjectOfType<FirebaseManager>();
        
        if (fbManager != null)
        {
            fbManager.ClearCache();
            Debug.Log("? Cache de Firebase limpiado");
            
            EditorUtility.DisplayDialog(
                "Cache Limpiado ?",
                "El cache de Firebase ha sido limpiado.\n\n" +
                "La próxima vez que ejecutes la app, se volverán a descargar todos los datos.",
                "OK"
            );
        }
        else
        {
            Debug.LogError("? FirebaseManager no encontrado");
            
            EditorUtility.DisplayDialog(
                "Error ?",
                "No se encontró FirebaseManager en la escena actual.",
                "OK"
            );
        }
    }
    
    [MenuItem("AR Navigation/?? Generar Reporte del Sistema")]
    public static void GenerarReporte()
    {
        string report = "???????????????????????????????????????????????????????\n";
        report += "?? REPORTE DEL SISTEMA DE NAVEGACIÓN POR GRAFO\n";
        report += "???????????????????????????????????????????????????????\n\n";
        
        // Información general
        report += "?? Fecha: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
        report += "?? Escena Actual: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "\n\n";
        
        // Verificar componentes
        report += "??????????????????????????????????????????????????\n";
        report += "COMPONENTES PRINCIPALES\n";
        report += "??????????????????????????????????????????????????\n\n";
        
        // GraphNavigationManager
        GraphNavigationManager graphNav = Object.FindObjectOfType<GraphNavigationManager>();
        if (graphNav != null)
        {
            report += "? GraphNavigationManager\n";
            report += $"    GameObject: {graphNav.gameObject.name}\n";
            report += $"    Node Reached Distance: {graphNav.nodeReachedDistance}m\n";
            report += $"    Update Interval: {graphNav.updateInterval}s\n";
        }
        else
        {
            report += "? GraphNavigationManager - NO ENCONTRADO\n";
        }
        
        report += "\n";
        
        // AppModeManager
        AppModeManager appMode = Object.FindObjectOfType<AppModeManager>();
        if (appMode != null)
        {
            report += "? AppModeManager\n";
            report += $"    GameObject: {appMode.gameObject.name}\n";
            
            SerializedObject so = new SerializedObject(appMode);
            SerializedProperty graphNavProp = so.FindProperty("graphNavigationManager");
            
            if (graphNavProp != null && graphNavProp.objectReferenceValue != null)
            {
                report += "    GraphNavigationManager: ? Asignado\n";
            }
            else
            {
                report += "    GraphNavigationManager: ? NO ASIGNADO\n";
            }
        }
        else
        {
            report += "? AppModeManager - NO ENCONTRADO\n";
        }
        
        report += "\n";
        
        // NavigationArrowController
        NavigationArrowController arrow = Object.FindObjectOfType<NavigationArrowController>();
        if (arrow != null)
        {
            report += "? NavigationArrowController\n";
            report += $"    GameObject: {arrow.gameObject.name}\n";
            
            SerializedObject so = new SerializedObject(arrow);
            SerializedProperty progressTextProp = so.FindProperty("progressText");
            
            if (progressTextProp != null && progressTextProp.objectReferenceValue != null)
            {
                report += "    Progress Text: ? Asignado\n";
            }
            else
            {
                report += "    Progress Text: ?? No asignado (opcional)\n";
            }
        }
        else
        {
            report += "? NavigationArrowController - NO ENCONTRADO\n";
        }
        
        report += "\n";
        
        // FirebaseManager
        FirebaseManager firebase = Object.FindObjectOfType<FirebaseManager>();
        if (firebase != null)
        {
            report += "? FirebaseManager\n";
            report += $"    GameObject: {firebase.gameObject.name}\n";
            
            SerializedObject so = new SerializedObject(firebase);
            SerializedProperty collectionProp = so.FindProperty("collectionName");
            SerializedProperty graphEdgesProp = so.FindProperty("graphEdgesCollectionName");
            
            if (collectionProp != null)
            {
                report += $"    Collection Name: '{collectionProp.stringValue}'\n";
            }
            
            if (graphEdgesProp != null)
            {
                report += $"    Graph Edges Collection: '{graphEdgesProp.stringValue}'\n";
            }
            
            report += $"    Cache Count: {firebase.GetCacheCount()}\n";
        }
        else
        {
            report += "? FirebaseManager - NO ENCONTRADO\n";
        }
        
        report += "\n";
        report += "??????????????????????????????????????????????????\n";
        report += "SCRIPTS INSTALADOS\n";
        report += "??????????????????????????????????????????????????\n\n";
        
        string[] scripts = new string[]
        {
            "GraphEdge.cs",
            "GraphNode.cs",
            "PathfindingService.cs",
            "GraphNavigationManager.cs"
        };
        
        foreach (string script in scripts)
        {
            string path = "Assets/Scripts/" + script;
            if (File.Exists(path))
            {
                report += $"? {script}\n";
            }
            else
            {
                report += $"? {script} - NO ENCONTRADO\n";
            }
        }
        
        report += "\n";
        report += "??????????????????????????????????????????????????\n";
        report += "DOCUMENTACIÓN DISPONIBLE\n";
        report += "??????????????????????????????????????????????????\n\n";
        
        string[] docs = new string[]
        {
            "README_NAVEGACION_GRAFO.md",
            "GUIA_NAVEGACION_GRAFO.md",
            "INSTRUCCIONES_FIRESTORE_GRAFO.md",
            "EJEMPLO_FIRESTORE_GRAFO.md",
            "DIAGRAMA_ARQUITECTURA_GRAFO.md",
            "RESUMEN_IMPLEMENTACION_GRAFO.md"
        };
        
        foreach (string doc in docs)
        {
            string path = "Assets/" + doc;
            if (File.Exists(path))
            {
                report += $"? {doc}\n";
            }
            else
            {
                report += $"? {doc} - NO ENCONTRADO\n";
            }
        }
        
        report += "\n";
        report += "??????????????????????????????????????????????????\n";
        report += "RECOMENDACIONES\n";
        report += "??????????????????????????????????????????????????\n\n";
        
        if (graphNav == null)
        {
            report += "?? Ejecuta 'AR Navigation > Configurar Sistema Completo'\n";
        }
        
        if (firebase == null)
        {
            report += "?? Crea un GameObject con FirebaseManager\n";
        }
        
        report += "?? Lee Assets/docs/README_NAVEGACION_GRAFO.md para comenzar\n";
        report += "?? Configura Firestore según INSTRUCCIONES_FIRESTORE_GRAFO.md\n";
        
        report += "\n???????????????????????????????????????????????????????\n";
        
        Debug.Log(report);
        
        // Guardar reporte en archivo
        string reportPath = "Assets/REPORTE_SISTEMA_GRAFO.txt";
        File.WriteAllText(reportPath, report);
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "Reporte Generado ?",
            $"El reporte ha sido generado exitosamente.\n\n" +
            $"Archivo guardado en:\n{reportPath}\n\n" +
            $"También se imprimió en la consola.",
            "OK"
        );
        
        // Seleccionar el archivo
        Object reportAsset = AssetDatabase.LoadAssetAtPath<Object>(reportPath);
        Selection.activeObject = reportAsset;
        EditorGUIUtility.PingObject(reportAsset);
    }
    
    [MenuItem("AR Navigation/?? Solucionar Problemas Comunes")]
    public static void SolucionarProblemas()
    {
        int fixes = 0;
        string report = "?? SOLUCIONANDO PROBLEMAS COMUNES...\n\n";
        
        // Problema 1: GraphNavigationManager faltante
        GraphNavigationManager graphNav = Object.FindObjectOfType<GraphNavigationManager>();
        if (graphNav == null)
        {
            report += "? GraphNavigationManager no encontrado\n";
            report += "   ? Ejecuta 'AR Navigation > Configurar Sistema Completo'\n\n";
        }
        else
        {
            report += "? GraphNavigationManager OK\n\n";
        }
        
        // Problema 2: AppModeManager sin referencia
        AppModeManager appMode = Object.FindObjectOfType<AppModeManager>();
        if (appMode != null)
        {
            SerializedObject so = new SerializedObject(appMode);
            SerializedProperty graphNavProp = so.FindProperty("graphNavigationManager");
            
            if (graphNavProp != null && graphNavProp.objectReferenceValue == null && graphNav != null)
            {
                report += "?? Asignando GraphNavigationManager a AppModeManager...\n";
                graphNavProp.objectReferenceValue = graphNav;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(appMode);
                fixes++;
                report += "   ? Corregido\n\n";
            }
        }
        
        // Problema 3: FirebaseManager con campos vacíos
        FirebaseManager firebase = Object.FindObjectOfType<FirebaseManager>();
        if (firebase != null)
        {
            SerializedObject so = new SerializedObject(firebase);
            SerializedProperty graphEdgesProp = so.FindProperty("graphEdgesCollectionName");
            
            if (graphEdgesProp != null && string.IsNullOrEmpty(graphEdgesProp.stringValue))
            {
                report += "?? Configurando graphEdgesCollectionName...\n";
                graphEdgesProp.stringValue = "graphEdges";
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(firebase);
                fixes++;
                report += "   ? Corregido\n\n";
            }
        }
        
        report += $"\n? {fixes} problema(s) corregido(s)\n";
        
        Debug.Log(report);
        
        EditorUtility.DisplayDialog(
            "Problemas Solucionados",
            $"{fixes} problema(s) fueron corregidos.\n\n" +
            "Revisa la consola para ver los detalles.",
            "OK"
        );
    }
    
    [MenuItem("AR Navigation/?? Reparar Referencias/Asignar Arrow Prefab")]
    public static void AsignarArrowPrefab()
    {
        Debug.Log("?? Buscando NavigationArrowController y asignando Arrow Prefab...");
        
        NavigationArrowController navController = Object.FindObjectOfType<NavigationArrowController>();
        
        if (navController == null)
        {
            EditorUtility.DisplayDialog(
                "Error",
                "No se encontró NavigationArrowController en la escena.\n\n" +
                "Asegúrate de tener el GameObject con este componente.",
                "OK"
            );
            return;
        }
        
        // Buscar el prefab de Arrow
        string[] guids = AssetDatabase.FindAssets("Arrow t:Prefab");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "Error",
                "No se encontró el prefab 'Arrow' en el proyecto.\n\n" +
                "Verifica que exista en Assets/Prefabs/Arrow.prefab",
                "OK"
            );
            return;
        }
        
        string arrowPrefabPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        GameObject arrowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(arrowPrefabPath);
        
        if (arrowPrefab == null)
        {
            EditorUtility.DisplayDialog(
                "Error",
                $"No se pudo cargar el prefab desde:\n{arrowPrefabPath}",
                "OK"
            );
            return;
        }
        
        // Asignar el prefab
        SerializedObject so = new SerializedObject(navController);
        SerializedProperty arrowPrefabProp = so.FindProperty("arrowPrefab");
        
        if (arrowPrefabProp != null)
        {
            arrowPrefabProp.objectReferenceValue = arrowPrefab;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(navController);
            
            Debug.Log($"? Arrow Prefab asignado correctamente:");
            Debug.Log($"   GameObject: {navController.gameObject.name}");
            Debug.Log($"   Prefab: {arrowPrefabPath}");
            
            // Seleccionar el objeto
            Selection.activeGameObject = navController.gameObject;
            EditorGUIUtility.PingObject(navController.gameObject);
            
            EditorUtility.DisplayDialog(
                "Éxito ?",
                $"Arrow Prefab asignado correctamente.\n\n" +
                $"GameObject: {navController.gameObject.name}\n" +
                $"Prefab: Arrow.prefab\n\n" +
                $"Verifica en el Inspector que aparezca correctamente.",
                "OK"
            );
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Error",
                "No se encontró el campo 'arrowPrefab' en NavigationArrowController.\n\n" +
                "Verifica que el script esté actualizado.",
                "OK"
            );
        }
    }
    
    [MenuItem("AR Navigation/?? Reparar Referencias/Verificar Arrow Prefab")]
    public static void VerificarArrowPrefab()
    {
        NavigationArrowController navController = Object.FindObjectOfType<NavigationArrowController>();
        
        if (navController == null)
        {
            EditorUtility.DisplayDialog(
                "Verificación",
                "? NavigationArrowController no encontrado en la escena.",
                "OK"
            );
            return;
        }
        
        SerializedObject so = new SerializedObject(navController);
        SerializedProperty arrowPrefabProp = so.FindProperty("arrowPrefab");
        
        string mensaje = $"?? VERIFICACIÓN DE ARROW PREFAB\n\n";
        mensaje += $"GameObject: {navController.gameObject.name}\n\n";
        
        if (arrowPrefabProp != null && arrowPrefabProp.objectReferenceValue != null)
        {
            mensaje += $"? Arrow Prefab: ASIGNADO\n";
            mensaje += $"   Prefab: {arrowPrefabProp.objectReferenceValue.name}\n\n";
            mensaje += "Todo está correcto.";
            
            Debug.Log("? Arrow Prefab está asignado correctamente");
        }
        else
        {
            mensaje += $"? Arrow Prefab: NO ASIGNADO\n\n";
            mensaje += "Ejecuta:\n";
            mensaje += "'AR Navigation ? Reparar Referencias ? Asignar Arrow Prefab'";
            
            Debug.LogWarning("? Arrow Prefab NO está asignado");
        }
        
        EditorUtility.DisplayDialog("Verificación de Arrow Prefab", mensaje, "OK");
        
        // Seleccionar el objeto
        Selection.activeGameObject = navController.gameObject;
        EditorGUIUtility.PingObject(navController.gameObject);
    }
    
    private static void VerifySetup()
    {
        bool hasErrors = false;
        string message = "";
        
        // Verificar componentes críticos
        if (Object.FindObjectOfType<GraphNavigationManager>() == null)
        {
            hasErrors = true;
            message += "? GraphNavigationManager faltante\n";
        }
        
        if (Object.FindObjectOfType<AppModeManager>() == null)
        {
            hasErrors = true;
            message += "? AppModeManager faltante\n";
        }
        
        if (Object.FindObjectOfType<FirebaseManager>() == null)
        {
            hasErrors = true;
            message += "? FirebaseManager faltante\n";
        }
        
        if (hasErrors)
        {
            Debug.LogWarning(message);
            EditorUtility.DisplayDialog(
                "Verificación Fallida ??",
                "Faltan componentes necesarios:\n\n" + message + "\n" +
                "Ejecuta 'AR Navigation > Configurar Sistema Completo'",
                "Entendido"
            );
        }
        else
        {
            Debug.Log("? Todos los componentes principales están presentes");
            EditorUtility.DisplayDialog(
                "Verificación Exitosa ?",
                "Todos los componentes principales están presentes.\n\n" +
                "Para una verificación detallada, usa:\n" +
                "'AR Navigation > Generar Reporte del Sistema'",
                "OK"
            );
        }
    }
    
    private static void OpenFile(string path)
    {
        if (File.Exists(path))
        {
            Object fileAsset = AssetDatabase.LoadAssetAtPath<Object>(path);
            AssetDatabase.OpenAsset(fileAsset);
        }
        else
        {
            Debug.LogError($"Archivo no encontrado: {path}");
            EditorUtility.DisplayDialog(
                "Error",
                $"El archivo no existe:\n{path}",
                "OK"
            );
        }
    }
}
