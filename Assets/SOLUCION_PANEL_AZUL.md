# ?? SOLUCIÓN: Panel Aparece Azul/Transparente

## ? Problema Actual
El panel aparece como un **rectángulo azul semitransparente** en lugar de mostrar el texto y el fondo oscuro.

## ?? Causas Posibles

### **Causa 1: Prefab Incorrecto Asignado**
El `infoPanelPrefab` en `MultiImageSpawner` podría estar apuntando al prefab incorrecto.

### **Causa 2: Canvas sin Contenido**
El Canvas se está mostrando pero el `BackgroundPanel` y los textos no están visibles.

### **Causa 3: Escala del Canvas**
El Canvas tiene `scale: 0.001` lo que hace todo muy pequeño o invisible.

---

## ? SOLUCIÓN PASO A PASO

### **PASO 1: Verificar Asignación del Prefab**

1. En Unity, selecciona el GameObject: **AR Session Origin** (o el que tenga `MultiImageSpawner`)
2. En el Inspector, busca el componente: **Multi Image Spawner (Script)**
3. Verifica que en **Info Panel Prefab** esté asignado: `Assets/Prefabs/InfoPanel`
4. Si está vacío o tiene otro prefab, **arrastra** `InfoPanel.prefab` desde la carpeta `Assets/Prefabs/`

### **PASO 2: Verificar Estructura del Prefab**

1. **Abre el prefab:** `Assets/Prefabs/InfoPanel.prefab`
2. **Verifica esta estructura:**

```
InfoPanel (GameObject raíz)
?? Canvas
   ?? BackgroundPanel (Image - gris oscuro)
   ?  ?? TitleText (TextMeshPro)
   ?  ?? DescriptionText (TextMeshPro)
   ?  ?? CoordinatesText (TextMeshPro)
   ?? LoadingPanel (GameObject)
      ?? LoadingText (TextMeshPro)
```

3. **Si falta algo**, el panel no se mostrará correctamente

### **PASO 3: Verificar Configuración del Canvas**

Con el prefab abierto:

1. Selecciona: **Canvas**
2. En el Inspector, verifica:
   - **Render Mode:** `World Space`
   - **Scale:** `(0.001, 0.001, 0.001)` ? Esto está bien
   - **Canvas (Component):** Debe estar habilitado

3. Selecciona: **BackgroundPanel**
4. Verifica:
   - **GameObject:** Activado (checkbox marcado)
   - **Image Component:** Color con **Alpha 255** (1.0)
   - **Rect Transform:** Width `480`, Height `330`

### **PASO 4: Verificar Textos**

1. Selecciona cada texto: **TitleText**, **DescriptionText**, **CoordinatesText**
2. Verifica que:
   - **GameObject:** Activado
   - **TextMeshProUGUI:** Tiene texto por defecto
   - **Color:** Blanco (255, 255, 255, 255)
   - **Font Asset:** Asignado (LiberationSans SDF o similar)

---

## ?? SOLUCIÓN RÁPIDA: Recrear el Prefab

Si el prefab está corrupto, vamos a recrearlo desde cero:

### **Opción A: Usar el Script Automático**

Crea este archivo:

**Assets/Editor/FixInfoPanel.cs**

```csharp
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class FixInfoPanel : EditorWindow
{
    [MenuItem("AR Tools/?? Reparar InfoPanel")]
    public static void ShowWindow()
    {
        if (EditorUtility.DisplayDialog(
            "Reparar InfoPanel",
            "Esto recreará el prefab InfoPanel con la configuración correcta.\n\n¿Continuar?",
            "Sí, reparar",
            "Cancelar"))
        {
            CreateInfoPanel();
        }
    }

    static void CreateInfoPanel()
    {
        // Crear GameObject raíz
        GameObject root = new GameObject("InfoPanel");
        root.transform.position = new Vector3(0, 0.15f, 0);
        
        // Agregar InfoPanelController
        var controller = root.AddComponent<InfoPanelController>();
        
        // Crear Canvas
        GameObject canvasGO = new GameObject("Canvas");
        canvasGO.transform.SetParent(root.transform);
        canvasGO.transform.localPosition = Vector3.zero;
        canvasGO.transform.localRotation = Quaternion.identity;
        canvasGO.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
        
        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(500, 350);
        
        // Crear BackgroundPanel
        GameObject bgPanel = new GameObject("BackgroundPanel");
        bgPanel.transform.SetParent(canvasGO.transform);
        bgPanel.transform.localPosition = Vector3.zero;
        bgPanel.transform.localRotation = Quaternion.identity;
        bgPanel.transform.localScale = Vector3.one;
        
        RectTransform bgRect = bgPanel.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        bgRect.sizeDelta = new Vector2(480, 330);
        
        Image bgImage = bgPanel.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Gris oscuro sólido
        
        // Crear TitleText
        GameObject titleGO = new GameObject("TitleText");
        titleGO.transform.SetParent(bgPanel.transform);
        titleGO.transform.localPosition = Vector3.zero;
        titleGO.transform.localRotation = Quaternion.identity;
        titleGO.transform.localScale = Vector3.one;
        
        RectTransform titleRect = titleGO.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.anchoredPosition = new Vector2(0, -30);
        titleRect.sizeDelta = new Vector2(-40, 40);
        
        TextMeshProUGUI titleText = titleGO.AddComponent<TextMeshProUGUI>();
        titleText.text = "Nombre del Edificio";
        titleText.fontSize = 28;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // Crear DescriptionText
        GameObject descGO = new GameObject("DescriptionText");
        descGO.transform.SetParent(bgPanel.transform);
        descGO.transform.localPosition = Vector3.zero;
        descGO.transform.localRotation = Quaternion.identity;
        descGO.transform.localScale = Vector3.one;
        
        RectTransform descRect = descGO.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.anchoredPosition = new Vector2(0, -10);
        descRect.sizeDelta = new Vector2(-40, -120);
        
        TextMeshProUGUI descText = descGO.AddComponent<TextMeshProUGUI>();
        descText.text = "Descripción del edificio";
        descText.fontSize = 18;
        descText.alignment = TextAlignmentOptions.TopLeft;
        descText.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        // Crear CoordinatesText
        GameObject coordsGO = new GameObject("CoordinatesText");
        coordsGO.transform.SetParent(bgPanel.transform);
        coordsGO.transform.localPosition = Vector3.zero;
        coordsGO.transform.localRotation = Quaternion.identity;
        coordsGO.transform.localScale = Vector3.one;
        
        RectTransform coordsRect = coordsGO.AddComponent<RectTransform>();
        coordsRect.anchorMin = new Vector2(0, 0);
        coordsRect.anchorMax = new Vector2(1, 0);
        coordsRect.anchoredPosition = new Vector2(0, 20);
        coordsRect.sizeDelta = new Vector2(-40, 30);
        
        TextMeshProUGUI coordsText = coordsGO.AddComponent<TextMeshProUGUI>();
        coordsText.text = "?? Lat: 0.0, Lon: 0.0";
        coordsText.fontSize = 14;
        coordsText.fontStyle = FontStyles.Italic;
        coordsText.alignment = TextAlignmentOptions.Center;
        coordsText.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        
        // Crear LoadingPanel
        GameObject loadingGO = new GameObject("LoadingPanel");
        loadingGO.transform.SetParent(canvasGO.transform);
        loadingGO.transform.localPosition = Vector3.zero;
        loadingGO.transform.localRotation = Quaternion.identity;
        loadingGO.transform.localScale = Vector3.one;
        loadingGO.SetActive(false);
        
        RectTransform loadingRect = loadingGO.AddComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.5f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.5f);
        loadingRect.anchoredPosition = Vector2.zero;
        loadingRect.sizeDelta = new Vector2(200, 50);
        
        TextMeshProUGUI loadingText = loadingGO.AddComponent<TextMeshProUGUI>();
        loadingText.text = "? Cargando...";
        loadingText.fontSize = 20;
        loadingText.alignment = TextAlignmentOptions.Center;
        loadingText.color = Color.white;
        
        // Asignar referencias al controller
        controller.titleText = titleText;
        controller.descriptionText = descText;
        controller.coordinatesText = coordsText;
        controller.loadingPanel = loadingGO;
        controller.loadingText = loadingText;
        controller.enableFadeIn = false;
        controller.lookAtCamera = false;
        
        // Guardar como prefab
        string path = "Assets/Prefabs/InfoPanel.prefab";
        PrefabUtility.SaveAsPrefabAsset(root, path);
        
        DestroyImmediate(root);
        
        Debug.Log($"? InfoPanel recreado exitosamente en: {path}");
        EditorUtility.DisplayDialog("Éxito", "InfoPanel recreado correctamente!", "OK");
        
        // Seleccionar el prefab
        Object prefab = AssetDatabase.LoadAssetAtPath<Object>(path);
        Selection.activeObject = prefab;
        EditorGUIUtility.PingObject(prefab);
    }
}
```

Después de crear este archivo:

1. **Unity ? AR Tools ? ?? Reparar InfoPanel**
2. Click: **"Sí, reparar"**
3. El prefab se recreará correctamente

### **Opción B: Verificar Logs**

1. Conecta el dispositivo por USB
2. Abre **Android Logcat** (Window ? Analysis ? Android Logcat)
3. Filtra por: `MultiImageSpawner`
4. Busca mensajes como:
   - `? infoPanelPrefab NO está asignado`
   - `? InfoPanelController no encontrado`

---

## ?? Test en Unity Editor

Antes de hacer build, prueba en el Editor:

1. **Play** en Unity
2. En la jerarquía, busca el panel instanciado
3. Verifica que todos los elementos sean visibles
4. Ajusta colores y tamaños si es necesario

---

## ? Checklist de Verificación

- [ ] `infoPanelPrefab` asignado en `MultiImageSpawner`
- [ ] Prefab tiene estructura completa (Canvas ? BackgroundPanel ? Textos)
- [ ] `BackgroundPanel` tiene color sólido (Alpha 255)
- [ ] Todos los textos tienen fuente asignada
- [ ] `InfoPanelController` asignado al GameObject raíz
- [ ] Referencias del controller configuradas correctamente

---

**¿Cuál es el problema específico que ves? Puedo ayudarte a solucionarlo.**
