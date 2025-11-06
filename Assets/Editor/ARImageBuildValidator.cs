using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Previene builds con imágenes que probablemente fallarán en ARCore
/// Se ejecuta automáticamente antes de cada build
/// ?? TEMPORALMENTE DESHABILITADO - Solo muestra warnings
/// </summary>
public class ARImageBuildValidator : UnityEditor.Build.IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
    {
        // Solo validar en builds de Android
        if (report.summary.platform != UnityEditor.BuildTarget.Android)
            return;

        Debug.LogWarning("[ARImageValidator] ?? VALIDACIÓN EN MODO PERMISIVO");
        Debug.LogWarning("[ARImageValidator] El build continuará incluso si hay imágenes problemáticas");
        
        // Buscar todas las Reference Image Libraries en el proyecto
        string[] guids = AssetDatabase.FindAssets("t:XRReferenceImageLibrary");
        
        if (guids.Length == 0)
        {
            Debug.LogWarning("[ARImageValidator] ?? No se encontraron Reference Image Libraries");
            return;
        }

        List<string> warnings = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            XRReferenceImageLibrary library = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(path);

            if (library == null)
                continue;

            Debug.Log($"[ARImageValidator] ?? Analizando: {library.name} ({library.count} imágenes)");

            for (int i = 0; i < library.count; i++)
            {
                var refImage = library[i];
                ValidateImage(refImage, warnings);
            }
        }

        // Solo mostrar advertencias, no bloquear el build
        if (warnings.Count > 0)
        {
            string warningMsg = "?? ADVERTENCIAS en imágenes AR:\n" + string.Join("\n", warnings);
            Debug.LogWarning(warningMsg);
            Debug.LogWarning("[ARImageValidator] Si el build falla en ARCore, usa: AR Tools > Generar Marcador de Prueba");
        }
        else
        {
            Debug.Log("[ARImageValidator] ? Todas las imágenes AR pasaron la validación básica");
        }
    }

    void ValidateImage(XRReferenceImage refImage, List<string> warnings)
    {
        string name = refImage.name;
        Texture2D texture = refImage.texture;

        // Sin textura
        if (texture == null)
        {
            warnings.Add($"• '{name}': NO tiene textura asignada - El build FALLARÁ");
            return;
        }

        // Resolución muy baja
        if (texture.width < 300 || texture.height < 300)
        {
            warnings.Add($"• '{name}': Resolución baja ({texture.width}x{texture.height}px)");
        }

        // Tamaño físico no especificado
        if (!refImage.specifySize || refImage.size.x <= 0 || refImage.size.y <= 0)
        {
            warnings.Add($"• '{name}': Tamaño físico no especificado");
        }
    }
}

/// <summary>
/// Menu personalizado para validar imágenes manualmente
/// </summary>
public class ARImageMenu
{
    [MenuItem("AR Tools/Validar Imágenes AR Ahora")]
    public static void ValidateNow()
    {
        Debug.Log("========== VALIDACIÓN MANUAL DE IMÁGENES AR ==========");
        
        string[] guids = AssetDatabase.FindAssets("t:XRReferenceImageLibrary");
        
        if (guids.Length == 0)
        {
            EditorUtility.DisplayDialog("Sin librerías", "No se encontraron Reference Image Libraries", "OK");
            return;
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            XRReferenceImageLibrary library = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(path);
            Selection.activeObject = library;
            EditorGUIUtility.PingObject(library);
            break;
        }

        Debug.Log("? Seleccionada la Reference Image Library en el Inspector");
        Debug.Log("Revisa la consola para más detalles");
    }

    [MenuItem("AR Tools/Abrir Documentación de Solución")]
    public static void OpenDocumentation()
    {
        string path = "Assets/SOLUCION_ERROR_KEYPOINTS.md";
        if (System.IO.File.Exists(path))
        {
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
        }
        else
        {
            EditorUtility.DisplayDialog(
                "Documentación", 
                "El error 'Failed to get enough keypoints' ocurre cuando las imágenes son muy simples.\n\n" +
                "Soluciones:\n" +
                "1. Aumenta el contraste de las imágenes\n" +
                "2. Añade detalles distintivos (texto, bordes)\n" +
                "3. Evita colores sólidos\n" +
                "4. Usa imágenes con esquinas y texturas",
                "Entendido"
            );
        }
    }
}
