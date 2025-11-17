using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

/// <summary>
/// Filtro personalizado para logs del mapa
/// Uso: Window ? AR Navigation ? Map Logs Filter
/// </summary>
public class MapLogsFilter : EditorWindow
{
    private string filterText = "";
    private bool showInit = true;
    private bool showBounds = true;
    private bool showNodes = true;
    private bool showDiagnostics = true;
    private bool showErrors = true;
    private bool showWarnings = true;
    private bool showSuccess = true;
    
    [MenuItem("Window/AR Navigation/Map Logs Filter")]
    public static void ShowWindow()
    {
        GetWindow<MapLogsFilter>("Map Logs");
    }
    
    void OnGUI()
    {
        GUILayout.Label("?? FILTRO DE LOGS DEL MAPA", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Activa/desactiva categorías de logs del mapa.\n" +
            "Usa la Console de Unity para ver los resultados.",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        // Búsqueda manual
        GUILayout.Label("Búsqueda Manual en Console:", EditorStyles.boldLabel);
        filterText = EditorGUILayout.TextField("Texto a buscar:", filterText);
        
        if (GUILayout.Button("Copiar Filtro al Clipboard"))
        {
            EditorGUIUtility.systemCopyBuffer = filterText;
            Debug.Log($"? Filtro copiado: {filterText}");
        }
        
        GUILayout.Space(20);
        
        // Filtros rápidos
        GUILayout.Label("? Filtros Rápidos:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("??? Todos los Logs del Mapa"))
        {
            ApplyFilter("Map");
        }
        
        if (GUILayout.Button("?? Solo Inicialización"))
        {
            ApplyFilter("INICIALIZANDO");
        }
        
        if (GUILayout.Button("?? Solo Bounds"))
        {
            ApplyFilter("Bounds");
        }
        
        if (GUILayout.Button("?? Solo Diagnóstico"))
        {
            ApplyFilter("DIAGNÓSTICO");
        }
        
        if (GUILayout.Button("?? Solo Nodos"))
        {
            ApplyFilter("Dibujados");
        }
        
        if (GUILayout.Button("? Solo Errores"))
        {
            ApplyFilter("[MiniMap");
            Debug.LogWarning("Activa solo ?? Errors en Console");
        }
        
        if (GUILayout.Button("? Solo Éxitos"))
        {
            ApplyFilter("?");
        }
        
        GUILayout.Space(20);
        
        // Limpiar console
        if (GUILayout.Button("?? Limpiar Console", GUILayout.Height(30)))
        {
            ClearConsole();
        }
        
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "TIP: Para mejores resultados, usa el campo de búsqueda " +
            "directamente en la ventana Console de Unity.",
            MessageType.Info
        );
    }
    
    private void ApplyFilter(string filter)
    {
        filterText = filter;
        EditorGUIUtility.systemCopyBuffer = filter;
        Debug.Log($"?? Filtro aplicado: '{filter}' (copiado al clipboard)");
        Debug.Log($"?? Pégalo en el campo de búsqueda de Console");
    }
    
    private void ClearConsole()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        
        Debug.Log("?? Console limpiada");
    }
}
