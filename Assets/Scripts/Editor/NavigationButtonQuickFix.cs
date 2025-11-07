using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Menú de herramientas para arreglar rápidamente el botón de navegación
/// Usa: AR Tools > Fix Navigation Button > ...
/// </summary>
public class NavigationButtonQuickFix : MonoBehaviour
{
    #if UNITY_EDITOR
    
    [MenuItem("AR Tools/Fix Navigation Button/1. Diagnóstico Completo", false, 1)]
    static void RunFullDiagnostic()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? EJECUTANDO DIAGNÓSTICO COMPLETO");
        Debug.Log("???????????????????????????????????????????????");
        
        // Encontrar o crear NavigationButtonFixer
        NavigationButtonFixer fixer = FindObjectOfType<NavigationButtonFixer>();
        
        if (fixer == null)
        {
            GameObject fixerObj = new GameObject("NavigationButtonFixer");
            fixer = fixerObj.AddComponent<NavigationButtonFixer>();
            fixer.autoFix = true;
            fixer.verboseLogging = true;
            
            Debug.Log("? NavigationButtonFixer creado y configurado");
        }
        else
        {
            Debug.Log("? NavigationButtonFixer ya existe");
        }
        
        // Si estamos en Play mode, ejecutar inmediatamente
        if (EditorApplication.isPlaying)
        {
            Debug.Log("?? Modo Play detectado - Diagnóstico se ejecutará automáticamente");
        }
        else
        {
            Debug.Log("?? No estás en Play mode");
            Debug.Log("?? SOLUCIÓN: Presiona Play ?? para ejecutar el diagnóstico");
        }
        
        Selection.activeGameObject = fixer.gameObject;
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/2. Añadir ForceButtonWork al Botón", false, 2)]
    static void AddForceButtonWork()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? AÑADIENDO FORCEBUTTONWORK");
        Debug.Log("???????????????????????????????????????????????");
        
        // Buscar el botón
        Button button = FindNavigationButton();
        
        if (button == null)
        {
            Debug.LogError("? No se encontró el botón de navegación");
            Debug.LogError("?? SOLUCIÓN: Selecciona el botón manualmente y vuelve a ejecutar");
            return;
        }
        
        Debug.Log($"? Botón encontrado: {button.gameObject.name}");
        
        // Verificar si ya tiene ForceButtonWork
        ForceButtonWork existing = button.GetComponent<ForceButtonWork>();
        if (existing != null)
        {
            Debug.LogWarning("?? ForceButtonWork ya está en el botón");
            Debug.Log("?? Si el botón sigue sin funcionar, revisa la configuración");
            Selection.activeGameObject = button.gameObject;
            return;
        }
        
        // Añadir ForceButtonWork
        ForceButtonWork forceWork = button.gameObject.AddComponent<ForceButtonWork>();
        forceWork.useManualTouchDetection = true;
        forceWork.usePointerInterfaces = true;
        forceWork.forceInteractable = true;
        forceWork.debugLogs = true;
        
        Debug.Log("? ForceButtonWork añadido al botón");
        Debug.Log("? Configuración aplicada:");
        Debug.Log("   - useManualTouchDetection: true");
        Debug.Log("   - usePointerInterfaces: true");
        Debug.Log("   - forceInteractable: true");
        Debug.Log("   - debugLogs: true");
        Debug.Log("?? Presiona Play ?? y prueba el botón");
        
        Selection.activeGameObject = button.gameObject;
        EditorUtility.SetDirty(button.gameObject);
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/3. Reparación Rápida (Todo en Uno)", false, 3)]
    static void QuickFixAll()
    {
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("? REPARACIÓN RÁPIDA - TODO EN UNO");
        Debug.Log("???????????????????????????????????????????????");
        
        int fixes = 0;
        
        // 1. EventSystem
        Debug.Log("\n1?? Verificando EventSystem...");
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            
            // Añadir el módulo correcto
            System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModuleType != null)
            {
                esObj.AddComponent(inputSystemModuleType);
                Debug.Log("? EventSystem creado con InputSystemUIInputModule");
            }
            else
            {
                esObj.AddComponent<StandaloneInputModule>();
                Debug.Log("? EventSystem creado con StandaloneInputModule");
            }
            
            fixes++;
        }
        else
        {
            Debug.Log("? EventSystem ya existe");
            
            // ? NUEVO: Verificar y corregir InputModule
            StandaloneInputModule standaloneModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneModule != null)
            {
                Debug.LogWarning("?? EventSystem usa StandaloneInputModule (antiguo)");
                
                System.Type inputSystemModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputSystemModuleType != null)
                {
                    DestroyImmediate(standaloneModule);
                    eventSystem.gameObject.AddComponent(inputSystemModuleType);
                    Debug.Log("? Reemplazado con InputSystemUIInputModule");
                    fixes++;
                }
                else
                {
                    Debug.LogError("? No se pudo encontrar InputSystemUIInputModule");
                    Debug.LogError("?? SOLUCIÓN MANUAL: Selecciona EventSystem y click en 'Replace with InputSystemUIInputModule'");
                }
            }
        }
        
        // 2. Canvas con GraphicRaycaster
        Debug.Log("\n2?? Verificando Canvas...");
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Debug.Log($"? GraphicRaycaster añadido a: {canvas.gameObject.name}");
                fixes++;
            }
        }
        
        // 3. NavigationUIManager
        Debug.Log("\n3?? Verificando NavigationUIManager...");
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        if (uiManager == null)
        {
            Debug.LogWarning("?? NavigationUIManager no encontrado");
            Debug.LogWarning("?? Asegúrate de tener NavigationCanvas en la escena");
        }
        else
        {
            Debug.Log("? NavigationUIManager encontrado");
            
            // 4. Botón
            Debug.Log("\n4?? Verificando y reparando botón...");
            Button button = null;
            
            if (uiManager.openLocationPanelButton != null)
            {
                button = uiManager.openLocationPanelButton;
            }
            else
            {
                button = FindNavigationButton();
                if (button != null)
                {
                    uiManager.openLocationPanelButton = button;
                    Debug.Log("? Botón asignado en NavigationUIManager");
                    fixes++;
                }
            }
            
            if (button != null)
            {
                // Verificar interactable
                if (!button.interactable)
                {
                    button.interactable = true;
                    Debug.Log("? Botón hecho interactable");
                    fixes++;
                }
                
                // Verificar raycastTarget
                Image image = button.GetComponent<Image>();
                if (image != null && !image.raycastTarget)
                {
                    image.raycastTarget = true;
                    Debug.Log("? raycastTarget activado");
                    fixes++;
                }
                
                // Añadir ForceButtonWork si no lo tiene
                if (button.GetComponent<ForceButtonWork>() == null)
                {
                    ForceButtonWork forceWork = button.gameObject.AddComponent<ForceButtonWork>();
                    forceWork.useManualTouchDetection = true;
                    forceWork.usePointerInterfaces = true;
                    forceWork.forceInteractable = true;
                    forceWork.debugLogs = true;
                    Debug.Log("? ForceButtonWork añadido");
                    fixes++;
                }
                
                EditorUtility.SetDirty(button.gameObject);
            }
            else
            {
                Debug.LogWarning("?? No se pudo encontrar el botón");
            }
            
            EditorUtility.SetDirty(uiManager.gameObject);
        }
        
        // 5. Añadir NavigationButtonFixer
        Debug.Log("\n5?? Añadiendo herramientas de diagnóstico...");
        if (FindObjectOfType<NavigationButtonFixer>() == null)
        {
            GameObject fixerObj = new GameObject("NavigationButtonFixer");
            NavigationButtonFixer fixer = fixerObj.AddComponent<NavigationButtonFixer>();
            fixer.autoFix = true;
            fixer.verboseLogging = true;
            Debug.Log("? NavigationButtonFixer añadido");
            fixes++;
        }
        
        // Resumen
        Debug.Log("\n???????????????????????????????????????????????");
        Debug.Log($"? REPARACIÓN COMPLETADA");
        Debug.Log($"?? Se aplicaron {fixes} correcciones");
        Debug.Log("???????????????????????????????????????????????");
        Debug.Log("?? SIGUIENTE PASO: Presiona Play ?? y prueba el botón");
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/4. Añadir Info en Pantalla", false, 4)]
    static void AddStatusDisplay()
    {
        NavigationSystemStatus status = FindObjectOfType<NavigationSystemStatus>();
        
        if (status != null)
        {
            Debug.Log("?? NavigationSystemStatus ya existe");
            Selection.activeGameObject = status.gameObject;
            return;
        }
        
        GameObject statusObj = new GameObject("NavigationSystemStatus");
        status = statusObj.AddComponent<NavigationSystemStatus>();
        status.showOnStart = true;
        
        Debug.Log("? NavigationSystemStatus añadido");
        Debug.Log("?? En Play mode, presiona [I] para mostrar/ocultar info");
        
        Selection.activeGameObject = statusObj;
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/5. Añadir Auto-Tester", false, 5)]
    static void AddAutoTester()
    {
        AutoButtonTester tester = FindObjectOfType<AutoButtonTester>();
        
        if (tester != null)
        {
            Debug.Log("?? AutoButtonTester ya existe");
            Selection.activeGameObject = tester.gameObject;
            return;
        }
        
        GameObject testerObj = new GameObject("AutoButtonTester");
        tester = testerObj.AddComponent<AutoButtonTester>();
        tester.autoTestOnStart = false; // No auto-test por defecto
        
        Debug.Log("? AutoButtonTester añadido");
        Debug.Log("?? En Play mode, presiona [T] para ejecutar test");
        
        Selection.activeGameObject = testerObj;
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?????????", false, 10)]
    static void Separator() { }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Abrir Guía Rápida", false, 11)]
    static void OpenQuickGuide()
    {
        string path = "Assets/GUIA_RAPIDA_BOTON.md";
        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?? Abrir Guía Completa", false, 12)]
    static void OpenFullGuide()
    {
        string path = "Assets/SOLUCION_BOTON_NAVEGACION.md";
        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 1);
    }
    
    [MenuItem("AR Tools/Fix Navigation Button/?????????", false, 20)]
    static void Separator2() { }
    
    [MenuItem("AR Tools/Fix Navigation Button/??? Limpiar Herramientas de Debug", false, 21)]
    static void CleanupDebugTools()
    {
        int removed = 0;
        
        NavigationButtonFixer fixer = FindObjectOfType<NavigationButtonFixer>();
        if (fixer != null)
        {
            DestroyImmediate(fixer.gameObject);
            removed++;
        }
        
        NavigationSystemStatus status = FindObjectOfType<NavigationSystemStatus>();
        if (status != null)
        {
            DestroyImmediate(status.gameObject);
            removed++;
        }
        
        AutoButtonTester tester = FindObjectOfType<AutoButtonTester>();
        if (tester != null)
        {
            DestroyImmediate(tester.gameObject);
            removed++;
        }
        
        // Buscar ForceButtonWork en botones
        ForceButtonWork[] forceWorks = FindObjectsOfType<ForceButtonWork>();
        foreach (ForceButtonWork fw in forceWorks)
        {
            DestroyImmediate(fw);
            removed++;
        }
        
        // Buscar ManualButtonClicker en botones
        ManualButtonClicker[] manualClickers = FindObjectsOfType<ManualButtonClicker>();
        foreach (ManualButtonClicker mc in manualClickers)
        {
            DestroyImmediate(mc);
            removed++;
        }
        
        // Buscar ButtonDebugger en botones
        ButtonDebugger[] debuggers = FindObjectsOfType<ButtonDebugger>();
        foreach (ButtonDebugger bd in debuggers)
        {
            DestroyImmediate(bd);
            removed++;
        }
        
        Debug.Log($"? {removed} herramientas de debug eliminadas");
        Debug.Log("?? Usa esto antes de hacer el build final");
    }
    
    // Utilidad: Buscar el botón de navegación
    static Button FindNavigationButton()
    {
        // Primero buscar en NavigationUIManager
        NavigationUIManager uiManager = FindObjectOfType<NavigationUIManager>();
        if (uiManager != null && uiManager.openLocationPanelButton != null)
        {
            return uiManager.openLocationPanelButton;
        }
        
        // Buscar por nombre
        GameObject buttonObj = GameObject.Find("OpenLocationPanelButton");
        if (buttonObj != null)
        {
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                return button;
            }
        }
        
        // Buscar en todos los botones
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            if (btn.gameObject.name.Contains("Location") || 
                btn.gameObject.name.Contains("Navegar") ||
                btn.gameObject.name.Contains("Navigation"))
            {
                return btn;
            }
        }
        
        return null;
    }
    
    #endif
}
