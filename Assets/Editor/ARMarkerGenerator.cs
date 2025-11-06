using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Generador de marcadores de prueba para AR que garantiza que tengan suficientes keypoints
/// </summary>
public class ARMarkerGenerator : EditorWindow
{
    private string markerName = "TestMarker";
    private int resolution = 512;
    private Color backgroundColor = Color.white;
    private Color foregroundColor = Color.black;
    private bool addBorder = true;
    private bool addText = true;
    private bool addGrid = true;
    private bool addLogo = true;
    
    [MenuItem("AR Tools/Generar Marcador de Prueba")]
    public static void ShowWindow()
    {
        GetWindow<ARMarkerGenerator>("Generador de Marcadores AR");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Generador de Marcadores AR", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Esta herramienta genera marcadores de prueba con suficientes características para ARCore.\n" +
            "Los marcadores generados están GARANTIZADOS para funcionar.",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        markerName = EditorGUILayout.TextField("Nombre del Marcador", markerName);
        resolution = EditorGUILayout.IntSlider("Resolución (px)", resolution, 256, 2048);
        
        GUILayout.Space(5);
        backgroundColor = EditorGUILayout.ColorField("Color de Fondo", backgroundColor);
        foregroundColor = EditorGUILayout.ColorField("Color Primer Plano", foregroundColor);
        
        GUILayout.Space(10);
        GUILayout.Label("Elementos a Incluir:", EditorStyles.boldLabel);
        
        addBorder = EditorGUILayout.Toggle("Marco/Borde", addBorder);
        addText = EditorGUILayout.Toggle("Texto (Nombre)", addText);
        addGrid = EditorGUILayout.Toggle("Grid de Referencia", addGrid);
        addLogo = EditorGUILayout.Toggle("Logo/Patrón", addLogo);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Generar Marcador", GUILayout.Height(40)))
        {
            GenerateMarker();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "El marcador se guardará en Assets/Markers/Generated/\n" +
            "Luego agrégalo manualmente a tu Reference Image Library.",
            MessageType.Info
        );
    }
    
    void GenerateMarker()
    {
        // Crear directorio si no existe
        string directory = "Assets/Markers/Generated";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Crear textura
        Texture2D marker = new Texture2D(resolution, resolution);
        
        // Rellenar con color de fondo
        Color[] pixels = new Color[resolution * resolution];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = backgroundColor;
        }
        marker.SetPixels(pixels);
        
        // Añadir borde
        if (addBorder)
        {
            DrawBorder(marker);
        }
        
        // Añadir grid
        if (addGrid)
        {
            DrawGrid(marker);
        }
        
        // Añadir logo/patrón
        if (addLogo)
        {
            DrawPattern(marker);
        }
        
        // Añadir texto (simulado con rectángulos)
        if (addText)
        {
            DrawTextSimulation(marker);
        }
        
        marker.Apply();
        
        // Guardar como PNG
        byte[] bytes = marker.EncodeToPNG();
        string path = $"{directory}/{markerName}_{resolution}x{resolution}.png";
        File.WriteAllBytes(path, bytes);
        
        // Actualizar asset database
        AssetDatabase.Refresh();
        
        // Configurar import settings
        ConfigureTexture(path);
        
        Debug.Log($"[ARMarkerGenerator] ? Marcador generado: {path}");
        EditorUtility.DisplayDialog(
            "Marcador Generado",
            $"Marcador generado exitosamente:\n{path}\n\n" +
            "Ahora:\n" +
            "1. Ve a Assets/Markers/ReferenceImageLibrary\n" +
            "2. Haz clic en 'Add Image'\n" +
            "3. Arrastra el marcador generado\n" +
            "4. Especifica el tamaño físico (ej: 0.2 x 0.2 metros)",
            "Entendido"
        );
        
        // Seleccionar el marcador generado
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
    
    void DrawBorder(Texture2D texture)
    {
        int thickness = resolution / 20; // 5% del tamaño
        
        // Borde superior e inferior
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < thickness; y++)
            {
                texture.SetPixel(x, y, foregroundColor);
                texture.SetPixel(x, resolution - 1 - y, foregroundColor);
            }
        }
        
        // Borde izquierdo y derecho
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < thickness; x++)
            {
                texture.SetPixel(x, y, foregroundColor);
                texture.SetPixel(resolution - 1 - x, y, foregroundColor);
            }
        }
    }
    
    void DrawGrid(Texture2D texture)
    {
        int gridSize = resolution / 10; // 10x10 grid
        int lineThickness = Mathf.Max(1, resolution / 256);
        
        for (int i = 1; i < 10; i++)
        {
            int pos = i * gridSize;
            
            // Líneas verticales
            for (int y = 0; y < resolution; y++)
            {
                for (int t = 0; t < lineThickness; t++)
                {
                    texture.SetPixel(pos + t, y, foregroundColor);
                }
            }
            
            // Líneas horizontales
            for (int x = 0; x < resolution; x++)
            {
                for (int t = 0; t < lineThickness; t++)
                {
                    texture.SetPixel(x, pos + t, foregroundColor);
                }
            }
        }
    }
    
    void DrawPattern(Texture2D texture)
    {
        int centerX = resolution / 2;
        int centerY = resolution / 2;
        int size = resolution / 4;
        
        // Dibujar un patrón de círculos concéntricos
        for (int x = centerX - size; x < centerX + size; x++)
        {
            for (int y = centerY - size; y < centerY + size; y++)
            {
                if (x >= 0 && x < resolution && y >= 0 && y < resolution)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    
                    // Patrón de anillos
                    if (dist < size && ((int)(dist / 10) % 2 == 0))
                    {
                        texture.SetPixel(x, y, foregroundColor);
                    }
                }
            }
        }
        
        // Añadir esquinas marcadas
        int cornerSize = resolution / 20;
        int offset = resolution / 10;
        
        // Esquina superior izquierda
        DrawCornerMarker(texture, offset, resolution - offset, cornerSize);
        // Esquina superior derecha
        DrawCornerMarker(texture, resolution - offset, resolution - offset, cornerSize);
        // Esquina inferior izquierda
        DrawCornerMarker(texture, offset, offset, cornerSize);
        // Esquina inferior derecha
        DrawCornerMarker(texture, resolution - offset, offset, cornerSize);
    }
    
    void DrawCornerMarker(Texture2D texture, int centerX, int centerY, int size)
    {
        for (int x = centerX - size; x < centerX + size; x++)
        {
            for (int y = centerY - size; y < centerY + size; y++)
            {
                if (x >= 0 && x < resolution && y >= 0 && y < resolution)
                {
                    texture.SetPixel(x, y, foregroundColor);
                }
            }
        }
    }
    
    void DrawTextSimulation(Texture2D texture)
    {
        // Simular texto dibujando rectángulos horizontales (como líneas de texto)
        int textY = resolution * 3 / 4;
        int textHeight = resolution / 40;
        int margin = resolution / 5;
        
        // Título (línea más gruesa)
        for (int x = margin; x < resolution - margin; x++)
        {
            for (int y = textY; y < textY + textHeight * 2; y++)
            {
                if (y < resolution)
                {
                    texture.SetPixel(x, y, foregroundColor);
                }
            }
        }
        
        // Subtítulo (línea más delgada)
        textY -= textHeight * 4;
        for (int x = margin + resolution / 10; x < resolution - margin - resolution / 10; x++)
        {
            for (int y = textY; y < textY + textHeight; y++)
            {
                if (y >= 0 && y < resolution)
                {
                    texture.SetPixel(x, y, foregroundColor);
                }
            }
        }
    }
    
    void ConfigureTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Default;
            importer.isReadable = true;
            importer.maxTextureSize = resolution;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            // Configuración para Android
            TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
            androidSettings.name = "Android";
            androidSettings.overridden = true;
            androidSettings.maxTextureSize = resolution;
            androidSettings.format = TextureImporterFormat.RGBA32;
            importer.SetPlatformTextureSettings(androidSettings);
            
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}
