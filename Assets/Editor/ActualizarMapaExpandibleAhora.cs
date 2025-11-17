using UnityEngine;
using UnityEditor;

/// <summary>
/// Script temporal para actualizar el diseño del mapa expandible
/// Uso: Tools ? Actualizar Diseño Mapa AHORA
/// Eliminar después de usar
/// </summary>
public class ActualizarMapaExpandibleAhora
{
    [MenuItem("Tools/Actualizar Diseño Mapa AHORA")]
    public static void ActualizarAhora()
    {
        Debug.Log("?? Iniciando actualización del diseño del mapa expandible...");
        
        // Llamar al setup
        ExpandableMapSetupUtility.SetupExpandableMapSystem();
        
        Debug.Log("? Actualización completada!");
    }
}
