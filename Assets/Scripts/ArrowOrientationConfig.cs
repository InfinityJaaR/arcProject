using UnityEngine;

/// <summary>
/// Configuración de corrección de orientación de la flecha AR
/// Usa este componente para ajustar si el modelo está invertido
/// </summary>
[RequireComponent(typeof(NavigationArrowController))]
public class ArrowOrientationConfig : MonoBehaviour
{
    [Header("Corrección de Orientación")]
    [Tooltip("Marcar si el modelo de la flecha apunta hacia atrás (invertido 180°)")]
    public bool modelIsInverted = true; // Por defecto TRUE basado en tu reporte
    
    [Tooltip("Offset adicional de rotación (en grados) si es necesario")]
    [Range(-180f, 180f)]
    public float additionalRotationOffset = 0f;
    
    [Header("Debug Visual")]
    [Tooltip("Mostrar línea de debug mostrando hacia dónde apunta la flecha")]
    public bool showDebugLine = true;
    
    [Tooltip("Color de la línea de debug")]
    public Color debugLineColor = Color.cyan;
    
    private NavigationArrowController arrowController;
    private GameObject currentArrow;
    
    void Start()
    {
        arrowController = GetComponent<NavigationArrowController>();
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugLine) return;
        
        // Encontrar la flecha actual en la escena
        currentArrow = GameObject.Find("NavigationArrow");
        
        if (currentArrow == null) return;
        
        // Dibujar línea mostrando hacia dónde apunta
        Gizmos.color = debugLineColor;
        Vector3 arrowPos = currentArrow.transform.position;
        Vector3 forward = currentArrow.transform.forward;
        
        // Línea larga mostrando dirección
        Gizmos.DrawLine(arrowPos, arrowPos + forward * 3f);
        
        // Esfera en la punta
        Gizmos.DrawWireSphere(arrowPos + forward * 3f, 0.15f);
        
        // Flecha más pequeña para mejor visualización
        Vector3 right = currentArrow.transform.right * 0.3f;
        Gizmos.DrawLine(arrowPos + forward * 3f, arrowPos + forward * 2.5f + right);
        Gizmos.DrawLine(arrowPos + forward * 3f, arrowPos + forward * 2.5f - right);
    }
    
    /// <summary>
    /// Obtiene la corrección total de rotación
    /// </summary>
    public float GetRotationCorrection()
    {
        float correction = 0f;
        
        if (modelIsInverted)
        {
            correction += 180f;
        }
        
        correction += additionalRotationOffset;
        
        return correction;
    }
    
    /// <summary>
    /// Valida si la configuración es correcta mostrando información
    /// </summary>
    void OnGUI()
    {
        if (!Application.isPlaying) return;
        if (currentArrow == null) return;
        
        // Panel de información en pantalla
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        
        string info = "?? CONFIGURACIÓN DE FLECHA\n\n";
        info += $"Modelo Invertido: {(modelIsInverted ? "? SÍ" : "? NO")}\n";
        info += $"Offset Adicional: {additionalRotationOffset:F0}°\n";
        info += $"Corrección Total: {GetRotationCorrection():F0}°\n\n";
        info += "Rotación Actual:\n";
        info += $"  Y: {currentArrow.transform.eulerAngles.y:F1}°\n\n";
        info += "?? Si la flecha apunta al revés:\n";
        info += "   Cambia 'Model Is Inverted'";
        
        GUI.Box(new Rect(10, 10, 350, 200), info, style);
    }
}
