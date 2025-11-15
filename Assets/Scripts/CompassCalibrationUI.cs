using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Muestra instrucciones de calibración de brújula al usuario
/// cuando el magnetómetro no está funcionando correctamente
/// </summary>
public class CompassCalibrationUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("Panel de calibración (se activa cuando se necesita calibrar)")]
    public GameObject calibrationPanel;
    
    [Tooltip("Texto con instrucciones")]
    public TextMeshProUGUI instructionsText;
    
    [Tooltip("Botón para cerrar el panel")]
    public Button closeButton;
    
    [Tooltip("Botón para reintentar calibración")]
    public Button retryButton;
    
    [Tooltip("Texto de estado de la brújula")]
    public TextMeshProUGUI statusText;
    
    [Header("Configuración")]
    [Tooltip("Mostrar automáticamente si brújula no funciona después de X segundos")]
    public float autoShowDelay = 5f;
    
    [Tooltip("Cerrar automáticamente cuando brújula funcione")]
    public bool autoCloseWhenReady = true;
    
    private bool isShowing = false;
    private float timeWaiting = 0f;
    
    void Start()
    {
        // Ocultar panel inicialmente
        if (calibrationPanel != null)
        {
            calibrationPanel.SetActive(false);
        }
        
        // Configurar botones
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
        
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RecalibrateCompass);
        }
        
        // Configurar texto de instrucciones
        if (instructionsText != null)
        {
            instructionsText.text = 
                "?? <b>CALIBRACIÓN DE BRÚJULA REQUERIDA</b>\n\n" +
                "<size=18><b>Sigue estos pasos:</b></size>\n\n" +
                "1?? Aleja el teléfono de objetos metálicos\n" +
                "   (computadoras, mesas metálicas, altavoces)\n\n" +
                "2?? Mueve el dispositivo formando una <b>FIGURA 8</b>\n" +
                "   en el aire con movimientos amplios\n\n" +
                "3?? Repite el movimiento varias veces\n\n" +
                "4?? Si es posible, sal al exterior\n\n" +
                "<color=#FFD700>?? La brújula funciona mejor al aire libre</color>";
        }
    }
    
    void Update()
    {
        if (LocationManager.Instance == null) return;
        
        // Verificar estado de la brújula
        bool compassReady = LocationManager.Instance.IsCompassReady;
        
        if (!compassReady && LocationManager.Instance.IsGPSReady)
        {
            timeWaiting += Time.deltaTime;
            
            // Mostrar automáticamente después del delay
            if (timeWaiting >= autoShowDelay && !isShowing)
            {
                Show();
            }
        }
        else if (compassReady && isShowing && autoCloseWhenReady)
        {
            // Brújula funcionando - cerrar panel
            Hide();
            timeWaiting = 0f;
        }
        
        // Actualizar texto de estado
        UpdateStatusText();
    }
    
    /// <summary>
    /// Muestra el panel de calibración
    /// </summary>
    public void Show()
    {
        if (calibrationPanel != null)
        {
            calibrationPanel.SetActive(true);
            isShowing = true;
            Debug.Log("[CompassCalibrationUI] ?? Mostrando instrucciones de calibración");
        }
    }
    
    /// <summary>
    /// Oculta el panel de calibración
    /// </summary>
    public void Hide()
    {
        if (calibrationPanel != null)
        {
            calibrationPanel.SetActive(false);
            isShowing = false;
            Debug.Log("[CompassCalibrationUI] ?? Ocultando instrucciones de calibración");
        }
    }
    
    /// <summary>
    /// Fuerza recalibración de la brújula
    /// </summary>
    private void RecalibrateCompass()
    {
        if (LocationManager.Instance != null)
        {
            LocationManager.Instance.RecalibrateCompass();
            timeWaiting = 0f;
        }
    }
    
    /// <summary>
    /// Actualiza el texto de estado
    /// </summary>
    private void UpdateStatusText()
    {
        if (statusText == null || LocationManager.Instance == null) return;
        
        if (!isShowing) return;
        
        bool compassReady = LocationManager.Instance.IsCompassReady;
        
        if (compassReady)
        {
            statusText.text = "<color=#00FF00>? Brújula funcionando correctamente</color>";
        }
        else
        {
            statusText.text = 
                $"<color=#FF6B6B>?? Brújula no detectada ({timeWaiting:F0}s)</color>\n" +
                "<size=14><i>Sigue las instrucciones arriba</i></size>";
        }
    }
}
