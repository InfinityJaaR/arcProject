using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu de acceso rápido a todas las herramientas AR
/// </summary>
public class ARToolsMenu
{
    [MenuItem("AR Tools/?? Ver Resumen de Solución")]
    public static void OpenSolutionSummary()
    {
        string path = "Assets/README_SOLUCION.md";
        if (System.IO.File.Exists(path))
        {
            Application.OpenURL("file:///" + System.IO.Path.GetFullPath(path));
        }
        else
        {
            ShowQuickHelp();
        }
    }

    [MenuItem("AR Tools/?? Ver Guía de Imágenes")]
    public static void OpenImageGuide()
    {
        string path = "Assets/GUIA_IMAGENES_AR.md";
        if (System.IO.File.Exists(path))
        {
            Application.OpenURL("file:///" + System.IO.Path.GetFullPath(path));
        }
    }

    [MenuItem("AR Tools/?? Ver Solución Detallada")]
    public static void OpenDetailedSolution()
    {
        string path = "Assets/SOLUCION_ERROR_KEYPOINTS.md";
        if (System.IO.File.Exists(path))
        {
            Application.OpenURL("file:///" + System.IO.Path.GetFullPath(path));
        }
    }

    [MenuItem("AR Tools/?? Seleccionar Reference Image Library")]
    public static void SelectRIL()
    {
        string[] guids = AssetDatabase.FindAssets("t:XRReferenceImageLibrary");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "No encontrado",
                "No se encontró ninguna Reference Image Library en el proyecto.",
                "OK"
            );
            return;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object library = AssetDatabase.LoadAssetAtPath<Object>(path);
            Selection.activeObject = library;
            EditorGUIUtility.PingObject(library);
            
            Debug.Log($"[ARTools] Seleccionada Reference Image Library: {path}");
            break;
        }
    }

    [MenuItem("AR Tools/?? Abrir Carpeta de Marcadores")]
    public static void OpenMarkersFolder()
    {
        string path = "Assets/Markers";
        if (System.IO.Directory.Exists(path))
        {
            EditorUtility.RevealInFinder(path);
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Carpeta no encontrada",
                "No se encontró la carpeta Assets/Markers",
                "OK"
            );
        }
    }

    [MenuItem("AR Tools/?? Ayuda Rápida")]
    public static void ShowQuickHelp()
    {
        EditorUtility.DisplayDialog(
            "Herramientas AR - Ayuda Rápida",
            "ERROR: 'Failed to get enough keypoints'\n\n" +
            "SOLUCIONES RÁPIDAS:\n\n" +
            "1. GENERAR MARCADORES NUEVOS:\n" +
            "   ? AR Tools > Generar Marcador de Prueba\n\n" +
            "2. VALIDAR IMÁGENES ACTUALES:\n" +
            "   ? AR Tools > Image Tracking Validator\n\n" +
            "3. MEJORAR IMÁGENES:\n" +
            "   • Aumentar contraste\n" +
            "   • Añadir bordes y texto\n" +
            "   • Evitar colores sólidos\n\n" +
            "4. ESPECIFICAR TAMAÑO:\n" +
            "   • Abrir Reference Image Library\n" +
            "   • Marcar 'Specify Size'\n" +
            "   • Configurar: 0.2 x 0.2 metros\n\n" +
            "DOCUMENTACIÓN COMPLETA:\n" +
            "? AR Tools > Ver Guía de Imágenes",
            "Entendido"
        );
    }

    [MenuItem("AR Tools/?? Test Completo del Sistema")]
    public static void RunFullSystemTest()
    {
        Debug.Log("========== TEST COMPLETO DEL SISTEMA AR ==========\n");
        
        // Test 1: Reference Image Library
        Debug.Log("1. Verificando Reference Image Library...");
        string[] rilGuids = AssetDatabase.FindAssets("t:XRReferenceImageLibrary");
        
        if (rilGuids.Length == 0)
        {
            Debug.LogError("   ? NO se encontró Reference Image Library");
        }
        else
        {
            Debug.Log($"   ? Encontradas {rilGuids.Length} Reference Image Libraries");
        }

        // Test 2: Scripts
        Debug.Log("\n2. Verificando scripts principales...");
        string[] scripts = new string[]
        {
            "Assets/Scripts/MultiImageSpawner.cs",
            "Assets/Scripts/FirebaseManager.cs",
            "Assets/Scripts/InfoPanelController.cs",
            "Assets/Scripts/BuildingData.cs"
        };

        foreach (string script in scripts)
        {
            if (System.IO.File.Exists(script))
            {
                Debug.Log($"   ? {script}");
            }
            else
            {
                Debug.LogWarning($"   ?? {script} - No encontrado");
            }
        }

        // Test 3: Prefabs
        Debug.Log("\n3. Verificando prefabs...");
        if (System.IO.File.Exists("Assets/Prefabs/InfoPanel.prefab"))
        {
            Debug.Log("   ? InfoPanel.prefab");
        }
        else
        {
            Debug.LogWarning("   ?? InfoPanel.prefab - No encontrado");
        }

        // Test 4: Herramientas
        Debug.Log("\n4. Verificando herramientas instaladas...");
        string[] tools = new string[]
        {
            "Assets/Editor/ImageTrackingValidator.cs",
            "Assets/Editor/ARImageBuildValidator.cs",
            "Assets/Editor/ARMarkerGenerator.cs"
        };

        foreach (string tool in tools)
        {
            if (System.IO.File.Exists(tool))
            {
                Debug.Log($"   ? {tool}");
            }
            else
            {
                Debug.LogWarning($"   ?? {tool} - No encontrado");
            }
        }

        // Test 5: Documentación
        Debug.Log("\n5. Verificando documentación...");
        string[] docs = new string[]
        {
            "Assets/README_SOLUCION.md",
            "Assets/SOLUCION_ERROR_KEYPOINTS.md",
            "Assets/GUIA_IMAGENES_AR.md"
        };

        foreach (string doc in docs)
        {
            if (System.IO.File.Exists(doc))
            {
                Debug.Log($"   ? {doc}");
            }
            else
            {
                Debug.LogWarning($"   ?? {doc} - No encontrado");
            }
        }

        Debug.Log("\n========== TEST COMPLETO ==========");
        Debug.Log("Revisa los resultados arriba. ? = OK, ?? = Advertencia, ? = Error\n");

        EditorUtility.DisplayDialog(
            "Test Completo",
            "Test del sistema completado.\nRevisa la consola para ver los resultados detallados.",
            "OK"
        );
    }

    [MenuItem("AR Tools/?? Crear Backup de Configuración")]
    public static void CreateBackup()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string backupPath = $"Assets/Backups/ARConfig_{timestamp}";
        
        if (!System.IO.Directory.Exists("Assets/Backups"))
        {
            System.IO.Directory.CreateDirectory("Assets/Backups");
        }
        
        System.IO.Directory.CreateDirectory(backupPath);
        
        // Copiar RIL
        string[] rilGuids = AssetDatabase.FindAssets("t:XRReferenceImageLibrary");
        foreach (string guid in rilGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string destPath = $"{backupPath}/{System.IO.Path.GetFileName(path)}";
            AssetDatabase.CopyAsset(path, destPath);
        }
        
        AssetDatabase.Refresh();
        
        Debug.Log($"[ARTools] ? Backup creado: {backupPath}");
        EditorUtility.DisplayDialog(
            "Backup Creado",
            $"Backup de configuración AR creado exitosamente:\n{backupPath}",
            "OK"
        );
    }
}
