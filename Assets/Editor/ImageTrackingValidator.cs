using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Herramienta para validar la calidad de las imágenes de referencia para ARCore
/// </summary>
public class ImageTrackingValidator : EditorWindow
{
    private XRReferenceImageLibrary imageLibrary;
    
    [MenuItem("AR Tools/Image Tracking Validator")]
    public static void ShowWindow()
    {
        GetWindow<ImageTrackingValidator>("AR Image Validator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Validador de Imágenes para AR Tracking", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        imageLibrary = EditorGUILayout.ObjectField("Reference Image Library", imageLibrary, typeof(XRReferenceImageLibrary), false) as XRReferenceImageLibrary;
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Analizar Imágenes"))
        {
            if (imageLibrary != null)
            {
                AnalyzeImages();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Por favor asigna una Reference Image Library primero", "OK");
            }
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "RECOMENDACIONES PARA IMÁGENES AR:\n\n" +
            "? Alto contraste\n" +
            "? Detalles distintivos (esquinas, bordes)\n" +
            "? Textura variada\n" +
            "? Resolución mínima 300x300px\n" +
            "? No usar colores sólidos\n" +
            "? Evitar imágenes borrosas\n" +
            "? Evitar patrones repetitivos\n" +
            "? Evitar imágenes simétricas",
            MessageType.Info
        );
    }
    
    void AnalyzeImages()
    {
        Debug.Log("========== ANÁLISIS DE IMÁGENES AR ==========");
        
        for (int i = 0; i < imageLibrary.count; i++)
        {
            var refImage = imageLibrary[i];
            string imageName = refImage.name;
            Texture2D texture = refImage.texture;
            
            Debug.Log($"\n--- Analizando: {imageName} ---");
            
            if (texture == null)
            {
                Debug.LogError($"? {imageName}: NO tiene textura asignada!");
                continue;
            }
            
            // Verificar tamaño físico especificado
            Vector2 size = refImage.size;
            bool hasPhysicalSize = refImage.specifySize;
            
            if (hasPhysicalSize)
            {
                Debug.Log($"?? Tamaño físico especificado: {size.x}m x {size.y}m");
            }
            else
            {
                Debug.LogWarning($"?? {imageName}: No tiene tamaño físico especificado (se usará detección automática)");
            }
            
            // Verificar resolución de la textura
            int width = texture.width;
            int height = texture.height;
            
            Debug.Log($"?? Resolución: {width}x{height}px");
            
            if (width < 300 || height < 300)
            {
                Debug.LogWarning($"?? {imageName}: Resolución baja (mínimo recomendado 300x300px)");
            }
            else
            {
                Debug.Log($"? Resolución adecuada");
            }
            
            // Verificar aspect ratio
            float aspectRatio = (float)width / height;
            if (aspectRatio < 0.5f || aspectRatio > 2.0f)
            {
                Debug.LogWarning($"?? {imageName}: Aspect ratio extremo ({aspectRatio:F2}:1)");
            }
            
            // Verificar compresión
            if (texture.format == TextureFormat.DXT1 || texture.format == TextureFormat.DXT5)
            {
                Debug.LogWarning($"?? {imageName}: Usa compresión DXT - puede reducir la detección de características");
            }
            
            Debug.Log("------------------------");
        }
        
        Debug.Log("\n========== ANÁLISIS COMPLETO ==========");
        Debug.Log($"Total de imágenes analizadas: {imageLibrary.count}");
        Debug.Log("\nSI RECIBES EL ERROR 'Failed to get enough keypoints':");
        Debug.Log("1. Abre las imágenes problemáticas en un editor");
        Debug.Log("2. Aumenta el contraste y agrega detalles visuales");
        Debug.Log("3. Asegúrate de que no sean imágenes de color plano o muy simples");
        Debug.Log("4. Usa imágenes con esquinas y bordes bien definidos");
    }
}
