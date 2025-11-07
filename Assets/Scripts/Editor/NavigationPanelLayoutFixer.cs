using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Herramienta para arreglar el layout del panel de selección de lugares
/// Usa: AR Tools > Fix Navigation Panel Layout
/// </summary>
public class NavigationPanelLayoutFixer : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Arreglar Layout del Panel", false, 50)]
    static void FixPanelLayout()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? ARREGLANDO LAYOUT DEL PANEL DE SELECCIÓN");
        Debug.Log("???????????????????????????????????????????????");
        
        // Buscar NavigationUIManager
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        
        if (uiManager == null)
        {
            Debug.LogError("? NavigationUIManager no encontrado");
            return;
        }
        
        if (uiManager.locationListContent == null)
        {
            Debug.LogError("? locationListContent es NULL");
            return;
        }
        
        Transform content = uiManager.locationListContent;
        
        // 1. Configurar Vertical Layout Group
        Debug.Log("\n1?? Configurando Vertical Layout Group...");
        VerticalLayoutGroup vlg = content.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            Debug.Log("? Vertical Layout Group añadido");
        }
        
        // Configuración optimizada
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childScaleWidth = false;
        vlg.childScaleHeight = false;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.spacing = 10f; // Espacio entre botones
        vlg.padding = new RectOffset(10, 10, 10, 10); // Márgenes
        
        Debug.Log("? Vertical Layout Group configurado");
        
        // 2. Configurar Content Size Fitter
        Debug.Log("\n2?? Configurando Content Size Fitter...");
        ContentSizeFitter csf = content.GetComponent<ContentSizeFitter>();
        if (csf == null)
        {
            csf = content.gameObject.AddComponent<ContentSizeFitter>();
            Debug.Log("? Content Size Fitter añadido");
        }
        
        csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        
        Debug.Log("? Content Size Fitter configurado");
        
        // 3. Configurar RectTransform del Content
        Debug.Log("\n3?? Configurando RectTransform del Content...");
        RectTransform contentRect = content.GetComponent<RectTransform>();
        if (contentRect != null)
        {
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            
            Debug.Log("? RectTransform del Content configurado");
        }
        
        // 4. Verificar ScrollRect del padre
        Debug.Log("\n4?? Verificando ScrollView...");
        Transform scrollView = content.parent?.parent; // Content -> Viewport -> ScrollView
        if (scrollView != null)
        {
            ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.vertical = true;
                scrollRect.horizontal = false;
                scrollRect.movementType = ScrollRect.MovementType.Elastic;
                scrollRect.elasticity = 0.1f;
                scrollRect.inertia = true;
                scrollRect.decelerationRate = 0.135f;
                scrollRect.scrollSensitivity = 20f;
                
                Debug.Log("? ScrollRect configurado");
            }
            else
            {
                Debug.LogWarning("?? No se encontró ScrollRect");
            }
        }
        
        // 5. Marcar como modificado
        EditorUtility.SetDirty(content.gameObject);
        if (scrollView != null)
        {
            EditorUtility.SetDirty(scrollView.gameObject);
        }
        
        Debug.Log("\n???????????????????????????????????????????????");
        Debug.Log("? LAYOUT ARREGLADO");
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? Ahora los botones deberían:");
        Debug.Log("   - Aparecer verticalmente (uno debajo del otro)");
        Debug.Log("   - Tener espacio entre ellos");
        Debug.Log("   - Hacer scroll correctamente");
        Debug.Log("   - No superponerse");
        Debug.Log("\n?? Presiona Play y abre el panel para ver los cambios");
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Mejorar Botones de Lugares", false, 51)]
    static void ImproveLocationButtons()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? MEJORANDO PREFAB DE BOTONES");
        Debug.Log("???????????????????????????????????????????????");
        
        // Buscar el prefab
        string prefabPath = "Assets/Prefabs/LocationButton.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        
        if (prefab == null)
        {
            Debug.LogError($"? No se encontró el prefab en: {prefabPath}");
            return;
        }
        
        Debug.Log($"? Prefab encontrado: {prefabPath}");
        
        // Abrir el prefab para edición
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
        
        // Configurar el botón raíz
        RectTransform buttonRect = prefabRoot.GetComponent<RectTransform>();
        if (buttonRect != null)
        {
            buttonRect.sizeDelta = new Vector2(0, 100); // Altura fija, ancho flexible
            Debug.Log("? Tamaño del botón ajustado: altura 100");
        }
        
        // Configurar Image del botón
        Image buttonImage = prefabRoot.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = new Color(0.15f, 0.4f, 0.7f, 1f); // Azul más oscuro y sólido
            Debug.Log("? Color del botón mejorado");
        }
        
        // Configurar LayoutElement
        LayoutElement layoutElement = prefabRoot.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = prefabRoot.AddComponent<LayoutElement>();
            Debug.Log("? LayoutElement añadido");
        }
        
        layoutElement.minHeight = 100;
        layoutElement.preferredHeight = 100;
        layoutElement.flexibleWidth = 1;
        
        Debug.Log("? LayoutElement configurado");
        
        // Configurar el texto
        TMP_Text textComponent = prefabRoot.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            RectTransform textRect = textComponent.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 10);
            textRect.offsetMax = new Vector2(-15, -10);
            
            textComponent.alignment = TextAlignmentOptions.TopLeft;
            textComponent.fontSize = 16;
            textComponent.enableWordWrapping = true;
            textComponent.overflowMode = TextOverflowModes.Ellipsis;
            
            Debug.Log("? Texto configurado");
        }
        
        // Guardar cambios
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);
        
        Debug.Log("\n???????????????????????????????????????????????");
        Debug.Log("? PREFAB MEJORADO");
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? Los botones ahora tendrán:");
        Debug.Log("   - Altura fija de 100px");
        Debug.Log("   - Color azul más bonito");
        Debug.Log("   - Texto bien alineado a la izquierda");
        Debug.Log("   - Márgenes internos correctos");
        Debug.Log("\n?? Los botones existentes se actualizarán automáticamente");
    }
    
    #endif
}
