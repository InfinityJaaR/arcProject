using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor script para crear fácilmente el prefab de la flecha 3D
/// </summary>
public class Arrow3DCreator : EditorWindow
{
    private string prefabName = "Arrow3D_Compass";
    private Color arrowColor = new Color(1f, 0.2f, 0.2f); // Rojo brillante
    private float shaftLength = 1.2f;
    private float shaftRadius = 0.12f;
    private float headLength = 0.5f;
    private float headRadius = 0.25f;
    private int segments = 16;
    private bool useEmission = true;
    private float emissionIntensity = 0.3f;

    [MenuItem("GameObject/3D Object/Arrow 3D Compass")]
    private static void CreateArrow3DInScene()
    {
        // Crear GameObject en la escena
        GameObject arrow = new GameObject("Arrow3D_Compass");
        
        // Agregar componentes necesarios
        arrow.AddComponent<MeshFilter>();
        arrow.AddComponent<MeshRenderer>();
        Arrow3DGenerator generator = arrow.AddComponent<Arrow3DGenerator>();
        
        // Generar la flecha
        generator.GenerateArrow();
        
        // Seleccionar el objeto creado
        Selection.activeGameObject = arrow;
        
        // Enfocar en la Scene view
        SceneView.FrameLastActiveSceneView();
        
        Debug.Log("Flecha 3D creada. Ahora puedes ajustar sus propiedades en el Inspector y luego crear un prefab arrastrándola a la carpeta Assets.");
    }

    [MenuItem("Tools/AR Compass/Create Arrow 3D Prefab")]
    private static void ShowWindow()
    {
        Arrow3DCreator window = GetWindow<Arrow3DCreator>("Arrow 3D Creator");
        window.minSize = new Vector2(350, 400);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Crear Prefab de Flecha 3D", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Esta herramienta te ayuda a crear un prefab de flecha 3D con buen cuerpo y muy visible para usar como compass en AR móvil.",
            MessageType.Info);
        
        GUILayout.Space(10);

        // Configuración
        prefabName = EditorGUILayout.TextField("Nombre del Prefab:", prefabName);
        GUILayout.Space(5);
        
        arrowColor = EditorGUILayout.ColorField("Color:", arrowColor);
        GUILayout.Space(5);

        EditorGUILayout.LabelField("Dimensiones:", EditorStyles.boldLabel);
        shaftLength = EditorGUILayout.Slider("Longitud del Cuerpo:", shaftLength, 0.5f, 3f);
        shaftRadius = EditorGUILayout.Slider("Grosor del Cuerpo:", shaftRadius, 0.05f, 0.3f);
        headLength = EditorGUILayout.Slider("Longitud de la Punta:", headLength, 0.2f, 1.5f);
        headRadius = EditorGUILayout.Slider("Grosor de la Punta:", headRadius, 0.1f, 0.6f);
        segments = EditorGUILayout.IntSlider("Segmentos (calidad):", segments, 6, 32);

        GUILayout.Space(10);
        
        EditorGUILayout.LabelField("Visibilidad (Móvil):", EditorStyles.boldLabel);
        useEmission = EditorGUILayout.Toggle("Usar Brillo Emisivo:", useEmission);
        if (useEmission)
        {
            emissionIntensity = EditorGUILayout.Slider("Intensidad del Brillo:", emissionIntensity, 0f, 1f);
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Crear Prefab en Assets/Prefabs", GUILayout.Height(40)))
        {
            CreateArrowPrefab();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Crear Solo en Escena (sin Prefab)", GUILayout.Height(30)))
        {
            CreateArrowInScene();
        }

        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "?? Tips para Móvil:\n" +
            "• Usa dimensiones más grandes para mejor visibilidad\n" +
            "• El brillo emisivo ayuda en ambientes oscuros\n" +
            "• Colores brillantes (rojo, amarillo, cyan) son más visibles\n" +
            "• Recomendado: Grosor cuerpo 0.12, Grosor punta 0.25",
            MessageType.None);
    }

    private void CreateArrowPrefab()
    {
        // Crear directorio Prefabs si no existe
        string prefabFolder = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        // Crear GameObject temporal
        GameObject arrow = CreateArrowGameObject();

        // Crear prefab
        string prefabPath = $"{prefabFolder}/{prefabName}.prefab";
        
        // Si ya existe, preguntar si sobrescribir
        if (File.Exists(prefabPath))
        {
            if (!EditorUtility.DisplayDialog("Prefab ya existe",
                $"El prefab '{prefabName}' ya existe. ¿Deseas sobrescribirlo?",
                "Sí", "Cancelar"))
            {
                DestroyImmediate(arrow);
                return;
            }
        }

        // Guardar como prefab
        PrefabUtility.SaveAsPrefabAsset(arrow, prefabPath);
        
        // Limpiar GameObject temporal
        DestroyImmediate(arrow);

        // Seleccionar el prefab creado
        Object prefabAsset = AssetDatabase.LoadAssetAtPath<Object>(prefabPath);
        Selection.activeObject = prefabAsset;
        EditorGUIUtility.PingObject(prefabAsset);

        Debug.Log($"? Prefab de flecha 3D creado exitosamente en: {prefabPath}");
        
        EditorUtility.DisplayDialog("Éxito",
            $"Prefab '{prefabName}' creado exitosamente en {prefabFolder}",
            "OK");
    }

    private void CreateArrowInScene()
    {
        GameObject arrow = CreateArrowGameObject();
        Selection.activeGameObject = arrow;
        SceneView.FrameLastActiveSceneView();
        
        Debug.Log("Flecha 3D creada en la escena");
    }

    private GameObject CreateArrowGameObject()
    {
        GameObject arrow = new GameObject(prefabName);
        
        // Agregar componentes
        MeshFilter meshFilter = arrow.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = arrow.AddComponent<MeshRenderer>();
        Arrow3DGenerator generator = arrow.AddComponent<Arrow3DGenerator>();
        
        // Configurar propiedades usando reflexión
        SerializedObject serializedObject = new SerializedObject(generator);
        
        serializedObject.FindProperty("shaftLength").floatValue = shaftLength;
        serializedObject.FindProperty("shaftRadius").floatValue = shaftRadius;
        serializedObject.FindProperty("headLength").floatValue = headLength;
        serializedObject.FindProperty("headRadius").floatValue = headRadius;
        serializedObject.FindProperty("segments").intValue = segments;
        serializedObject.FindProperty("arrowColor").colorValue = arrowColor;
        serializedObject.FindProperty("useEmission").boolValue = useEmission;
        serializedObject.FindProperty("emissionIntensity").floatValue = emissionIntensity;
        
        serializedObject.ApplyModifiedProperties();
        
        // Generar la flecha
        generator.GenerateArrow();
        
        return arrow;
    }
}
