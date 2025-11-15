#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Script de editor para configurar automáticamente la UI de Nearby Places en el InfoPanel
/// Uso: AR Tools ? Configurar Nearby Places UI
/// </summary>
public class ConfigureNearbyPlacesUI : EditorWindow
{
    [MenuItem("AR Tools/Configurar Nearby Places UI")]
    public static void Configure()
    {
        // Cargar el prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InfoPanel.prefab");
        if (prefab == null)
        {
            Debug.LogError("? No se encontró el prefab InfoPanel.prefab en Assets/Prefabs/");
            return;
        }

        // Abrir el prefab para edición
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

        try
        {
            // Buscar BackgroundPanel
            Transform backgroundPanel = prefabInstance.transform.Find("Canvas/BackgroundPanel");
            if (backgroundPanel == null)
            {
                Debug.LogError("? No se encontró Canvas/BackgroundPanel en el prefab");
                return;
            }

            // Verificar si ya existe
            Transform existingContainer = backgroundPanel.Find("NearbyPlacesContainer");
            if (existingContainer != null)
            {
                Debug.LogWarning("?? NearbyPlacesContainer ya existe. Se eliminará y recreará.");
                DestroyImmediate(existingContainer.gameObject);
            }

            // Crear contenedor
            GameObject container = new GameObject("NearbyPlacesContainer");
            container.transform.SetParent(backgroundPanel, false);
            RectTransform containerRect = container.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0, 0);
            containerRect.anchorMax = new Vector2(1, 0);
            containerRect.anchoredPosition = new Vector2(0, 60);
            containerRect.sizeDelta = new Vector2(-40, 80);
            containerRect.pivot = new Vector2(0.5f, 0);

            // Crear título
            GameObject title = new GameObject("NearbyPlacesTitle");
            title.transform.SetParent(container.transform, false);
            RectTransform titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.anchoredPosition = new Vector2(0, -5);
            titleRect.sizeDelta = new Vector2(0, 20);
            
            TextMeshProUGUI titleText = title.AddComponent<TextMeshProUGUI>();
            titleText.text = "Lugares Cercanos:";
            titleText.fontSize = 16;
            titleText.fontStyle = FontStyles.Bold;
            titleText.color = new Color(1f, 0.9f, 0.5f);
            titleText.alignment = TextAlignmentOptions.TopLeft;

            // Crear texto de lugares
            GameObject textObj = new GameObject("NearbyPlacesText");
            textObj.transform.SetParent(container.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -25);
            
            TextMeshProUGUI nearbyText = textObj.AddComponent<TextMeshProUGUI>();
            nearbyText.text = "Cafetería Central, Auditorio Principal, Biblioteca";
            nearbyText.fontSize = 14;
            nearbyText.color = Color.white;
            nearbyText.alignment = TextAlignmentOptions.TopLeft;
            nearbyText.enableWordWrapping = true;

            // Conectar referencias en InfoPanelController
            InfoPanelController controller = prefabInstance.GetComponent<InfoPanelController>();
            if (controller != null)
            {
                // Usar SerializedObject para asegurar que Unity detecte los cambios
                SerializedObject so = new SerializedObject(controller);
                so.FindProperty("nearbyPlacesText").objectReferenceValue = nearbyText;
                so.FindProperty("nearbyPlacesContainer").objectReferenceValue = container;
                so.ApplyModifiedProperties();
                
                Debug.Log("? Referencias conectadas en InfoPanelController");
            }
            else
            {
                Debug.LogWarning("?? No se encontró InfoPanelController en el prefab");
            }

            // Guardar cambios
            PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
            
            Debug.Log("? Nearby Places UI configurado correctamente en InfoPanel.prefab");
            Debug.Log("?? Elementos creados:");
            Debug.Log("   • NearbyPlacesContainer");
            Debug.Log("   • NearbyPlacesTitle");
            Debug.Log("   • NearbyPlacesText");
            Debug.Log("\n?? ¡Listo! Ya puedes agregar el campo 'nearby_places' en Firestore.");
        }
        finally
        {
            // Asegurarse de liberar el prefab
            PrefabUtility.UnloadPrefabContents(prefabInstance);
        }
    }
    
    [MenuItem("AR Tools/Verificar Configuración Nearby Places")]
    public static void Verify()
    {
        // Cargar el prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InfoPanel.prefab");
        if (prefab == null)
        {
            Debug.LogError("? No se encontró el prefab InfoPanel.prefab");
            return;
        }

        InfoPanelController controller = prefab.GetComponent<InfoPanelController>();
        if (controller == null)
        {
            Debug.LogError("? No se encontró InfoPanelController en el prefab");
            return;
        }

        Debug.Log("?? VERIFICANDO CONFIGURACIÓN DE NEARBY PLACES:");
        Debug.Log("????????????????????????????????????????????");
        
        bool allGood = true;

        // Verificar nearbyPlacesText
        if (controller.nearbyPlacesText != null)
        {
            Debug.Log("? nearbyPlacesText: Configurado");
        }
        else
        {
            Debug.LogWarning("? nearbyPlacesText: NO configurado");
            allGood = false;
        }

        // Verificar nearbyPlacesContainer
        if (controller.nearbyPlacesContainer != null)
        {
            Debug.Log("? nearbyPlacesContainer: Configurado");
        }
        else
        {
            Debug.LogWarning("? nearbyPlacesContainer: NO configurado");
            allGood = false;
        }

        // Verificar otras referencias
        Debug.Log($"??  titleText: {(controller.titleText != null ? "?" : "?")}");
        Debug.Log($"??  descriptionText: {(controller.descriptionText != null ? "?" : "?")}");
        Debug.Log($"??  coordinatesText: {(controller.coordinatesText != null ? "?" : "?")}");

        Debug.Log("????????????????????????????????????????????");
        
        if (allGood)
        {
            Debug.Log("?? ¡Todo configurado correctamente!");
        }
        else
        {
            Debug.LogWarning("?? Faltan configuraciones. Usa: AR Tools ? Configurar Nearby Places UI");
        }
    }
}
#endif
