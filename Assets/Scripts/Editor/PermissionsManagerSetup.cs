using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Herramienta de Editor para añadir y configurar PermissionsManager automáticamente
/// Menú: AR Tools > Add Permissions Manager
/// </summary>
public class PermissionsManagerSetup : EditorWindow
{
    [MenuItem("AR Tools/Permissions/?? Añadir Permissions Manager", false, 100)]
    static void AddPermissionsManager()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AÑADIENDO PERMISSIONS MANAGER");
        Debug.Log("???????????????????????????????????????????????");
        
        // Verificar si ya existe
        PermissionsManager existing = FindObjectOfType<PermissionsManager>();
        
        if (existing != null)
        {
            Debug.LogWarning("?? PermissionsManager ya existe en la escena");
            
            bool overwrite = EditorUtility.DisplayDialog(
                "PermissionsManager ya existe",
                "Ya hay un PermissionsManager en la escena.\n\n¿Quieres seleccionarlo para verificar su configuración?",
                "Sí, seleccionar",
                "Cancelar"
            );
            
            if (overwrite)
            {
                Selection.activeGameObject = existing.gameObject;
                EditorGUIUtility.PingObject(existing.gameObject);
            }
            
            return;
        }
        
        // Crear GameObject
        GameObject permissionsManagerObj = new GameObject("PermissionsManager");
        
        // Añadir componente
        PermissionsManager pm = permissionsManagerObj.AddComponent<PermissionsManager>();
        
        // Configurar valores por defecto
        SerializedObject serializedPM = new SerializedObject(pm);
        serializedPM.FindProperty("requestOnStart").boolValue = true;
        serializedPM.FindProperty("showRationale").boolValue = true;
        serializedPM.ApplyModifiedProperties();
        
        Debug.Log("? PermissionsManager añadido a la escena");
        Debug.Log("?? Configuración:");
        Debug.Log("   ? Request On Start: TRUE");
        Debug.Log("   ? Show Rationale: TRUE");
        
        // Configurar Script Execution Order
        ConfigureScriptExecutionOrder();
        
        // Seleccionar el GameObject creado
        Selection.activeGameObject = permissionsManagerObj;
        EditorGUIUtility.PingObject(permissionsManagerObj);
        
        // Marcar escena como modificada
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("\n???????????????????????????????????????????????");
        Debug.Log("? PERMISSIONS MANAGER CONFIGURADO");
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? PRÓXIMOS PASOS:");
        Debug.Log("   1. Verifica la configuración en el Inspector");
        Debug.Log("   2. Build & Run en Android");
        Debug.Log("   3. La app pedirá permisos de UBICACIÓN + CÁMARA");
        Debug.Log("   4. Concede los permisos");
        Debug.Log("   5. GPS inicializará automáticamente");
        
        // Mostrar diálogo de confirmación
        EditorUtility.DisplayDialog(
            "PermissionsManager Añadido",
            "? PermissionsManager ha sido añadido y configurado.\n\n" +
            "Script Execution Order configurado:\n" +
            "   PermissionsManager: -100\n" +
            "   LocationManager: 0\n\n" +
            "Próximos pasos:\n" +
            "  1. Build & Run en Android\n" +
            "  2. Concede permisos cuando la app los pida\n" +
            "  3. GPS inicializará automáticamente",
            "Entendido"
        );
    }
    
    [MenuItem("AR Tools/Permissions/?? Configurar Script Execution Order", false, 101)]
    static void ConfigureScriptExecutionOrder()
    {
        Debug.Log("?? Configurando Script Execution Order...");
        
        // Obtener MonoScripts
        MonoScript permissionsManagerScript = GetMonoScript("PermissionsManager");
        MonoScript locationManagerScript = GetMonoScript("LocationManager");
        
        if (permissionsManagerScript == null)
        {
            Debug.LogError("? No se encontró el script PermissionsManager.cs");
            return;
        }
        
        if (locationManagerScript == null)
        {
            Debug.LogError("? No se encontró el script LocationManager.cs");
            return;
        }
        
        // Configurar orden de ejecución
        MonoImporter.SetExecutionOrder(permissionsManagerScript, -100);
        MonoImporter.SetExecutionOrder(locationManagerScript, 0);
        
        Debug.Log("? Script Execution Order configurado:");
        Debug.Log("    PermissionsManager: -100 (se ejecuta primero)");
        Debug.Log("    LocationManager: 0 (se ejecuta después)");
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    [MenuItem("AR Tools/Permissions/?? Verificar Permisos en AndroidManifest", false, 102)]
    static void VerifyAndroidManifest()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? VERIFICANDO ANDROIDMANIFEST.XML");
        Debug.Log("???????????????????????????????????????????????");
        
        string manifestPath = "Assets/Plugins/Android/AndroidManifest.xml";
        
        if (!System.IO.File.Exists(manifestPath))
        {
            Debug.LogWarning("?? AndroidManifest.xml no existe");
            
            bool create = EditorUtility.DisplayDialog(
                "AndroidManifest.xml no encontrado",
                "No se encontró AndroidManifest.xml en:\n" +
                "Assets/Plugins/Android/\n\n" +
                "¿Quieres crear uno con los permisos necesarios?",
                "Sí, crear",
                "Cancelar"
            );
            
            if (create)
            {
                CreateAndroidManifest();
            }
            
            return;
        }
        
        // Leer contenido
        string content = System.IO.File.ReadAllText(manifestPath);
        
        // Verificar permisos
        bool hasFineLocation = content.Contains("ACCESS_FINE_LOCATION");
        bool hasCoarseLocation = content.Contains("ACCESS_COARSE_LOCATION");
        bool hasCamera = content.Contains("CAMERA");
        
        Debug.Log($"?? ACCESS_FINE_LOCATION: {(hasFineLocation ? "?" : "?")}");
        Debug.Log($"?? ACCESS_COARSE_LOCATION: {(hasCoarseLocation ? "?" : "?")}");
        Debug.Log($"?? CAMERA: {(hasCamera ? "?" : "?")}");
        
        if (!hasFineLocation || !hasCoarseLocation)
        {
            Debug.LogWarning("?? Faltan permisos de ubicación en AndroidManifest.xml");
            
            bool add = EditorUtility.DisplayDialog(
                "Permisos Faltantes",
                "AndroidManifest.xml no tiene los permisos de ubicación necesarios.\n\n" +
                "¿Quieres que los añada automáticamente?",
                "Sí, añadir",
                "No"
            );
            
            if (add)
            {
                AddPermissionsToManifest(manifestPath, content);
            }
        }
        else
        {
            Debug.Log("? Todos los permisos están en AndroidManifest.xml");
            
            EditorUtility.DisplayDialog(
                "Permisos Verificados",
                "? AndroidManifest.xml tiene todos los permisos necesarios:\n\n" +
                "   ACCESS_FINE_LOCATION\n" +
                "   ACCESS_COARSE_LOCATION\n" +
                "   CAMERA",
                "Perfecto"
            );
        }
    }
    
    [MenuItem("AR Tools/Permissions/?? Abrir Guía de Permisos", false, 103)]
    static void OpenPermissionsGuide()
    {
        string guidePath = "Assets/docs/SOLUCION_PERMISOS_UBICACION.md";
        
        if (System.IO.File.Exists(guidePath))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(guidePath, 1);
            Debug.Log("?? Abriendo guía de permisos...");
        }
        else
        {
            Debug.LogError("? No se encontró SOLUCION_PERMISOS_UBICACION.md");
        }
    }
    
    // Método auxiliar para obtener MonoScript
    static MonoScript GetMonoScript(string className)
    {
        string[] guids = AssetDatabase.FindAssets($"t:MonoScript {className}");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            
            if (script != null && script.GetClass() != null && script.GetClass().Name == className)
            {
                return script;
            }
        }
        
        return null;
    }
    
    // Crear AndroidManifest.xml
    static void CreateAndroidManifest()
    {
        string directory = "Assets/Plugins/Android";
        string path = directory + "/AndroidManifest.xml";
        
        // Crear directorio si no existe
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            Debug.Log($"? Directorio creado: {directory}");
        }
        
        // Contenido del manifest
        string manifestContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"">
    
    <!-- Permisos de ubicación para navegación AR -->
    <uses-permission android:name=""android.permission.ACCESS_FINE_LOCATION"" />
    <uses-permission android:name=""android.permission.ACCESS_COARSE_LOCATION"" />
    
    <!-- Permiso de cámara para AR -->
    <uses-permission android:name=""android.permission.CAMERA"" />
    
    <!-- Características del dispositivo -->
    <uses-feature android:name=""android.hardware.location.gps"" android:required=""false"" />
    <uses-feature android:name=""android.hardware.camera.ar"" android:required=""true"" />
    
</manifest>";
        
        // Escribir archivo
        System.IO.File.WriteAllText(path, manifestContent);
        
        Debug.Log($"? AndroidManifest.xml creado en: {path}");
        Debug.Log("?? Permisos añadidos:");
        Debug.Log("    ACCESS_FINE_LOCATION");
        Debug.Log("    ACCESS_COARSE_LOCATION");
        Debug.Log("    CAMERA");
        
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog(
            "AndroidManifest Creado",
            "? AndroidManifest.xml ha sido creado con todos los permisos necesarios.\n\n" +
            "Ubicación:\n" +
            "Assets/Plugins/Android/AndroidManifest.xml\n\n" +
            "Próximo paso:\n" +
            "Build & Run para probar los permisos",
            "Entendido"
        );
    }
    
    // Añadir permisos a manifest existente
    static void AddPermissionsToManifest(string path, string content)
    {
        // Buscar dónde insertar los permisos
        string permissionsToAdd = @"    <!-- Permisos de ubicación para navegación AR -->
    <uses-permission android:name=""android.permission.ACCESS_FINE_LOCATION"" />
    <uses-permission android:name=""android.permission.ACCESS_COARSE_LOCATION"" />
    ";
        
        // Insertar después de <manifest>
        int insertIndex = content.IndexOf("<manifest");
        if (insertIndex != -1)
        {
            insertIndex = content.IndexOf(">", insertIndex) + 1;
            content = content.Insert(insertIndex, "\n" + permissionsToAdd);
            
            // Guardar
            System.IO.File.WriteAllText(path, content);
            
            Debug.Log("? Permisos añadidos a AndroidManifest.xml");
            
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog(
                "Permisos Añadidos",
                "? Los permisos de ubicación han sido añadidos a AndroidManifest.xml\n\n" +
                "Permisos añadidos:\n" +
                "   ACCESS_FINE_LOCATION\n" +
                "   ACCESS_COARSE_LOCATION",
                "Perfecto"
            );
        }
        else
        {
            Debug.LogError("? No se pudo encontrar la etiqueta <manifest> en el archivo");
        }
    }
}
