using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controlador del panel AR que muestra la información de un edificio/lugar
/// Se instancia sobre cada marcador detectado y actualiza su contenido con datos de Firebase
/// </summary>
public class InfoPanelController : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("TextMeshPro que muestra el nombre del edificio")]
    public TextMeshProUGUI titleText;
    
    [Tooltip("TextMeshPro que muestra la descripción")]
    public TextMeshProUGUI descriptionText;
    
    [Tooltip("TextMeshPro que muestra las coordenadas")]
    public TextMeshProUGUI coordinatesText;
    
    [Header("Loading")]
    [Tooltip("GameObject que contiene el indicador de carga")]
    public GameObject loadingPanel;
    
    [Tooltip("Texto del loading")]
    public TextMeshProUGUI loadingText;
    
    [Header("Animación")]
    [Tooltip("Habilitar fade-in al aparecer")]
    public bool enableFadeIn = false;
    
    [Tooltip("Duración del fade-in en segundos")]
    public float fadeInDuration = 0.5f;
    
    [Header("Billboard (Opcional)")]
    [Tooltip("Hacer que el panel siempre mire a la cámara")]
    public bool lookAtCamera = false;
    
    [Header("?? DEBUG: Forzar Opacidad")]
    [Tooltip("Fuerza alpha 1.0 en TODOS los componentes cada frame")]
    public bool forceOpaqueEveryFrame = true;
    
    private CanvasGroup canvasGroup;
    private Camera mainCamera;
    
    void Awake()
    {
        // Obtener o agregar CanvasGroup
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        if (canvasGroup == null)
        {
            var canvas = GetComponentInChildren<Canvas>();
            if (canvas != null)
                canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();
        }
        
        // FORZAR OPACIDAD INMEDIATAMENTE
        ForceOpaque();
        
        mainCamera = Camera.main;
        
        Debug.Log("[InfoPanelController] Inicializado - Opacidad forzada a 100%");
    }
    
    void Update()
    {
        // FORZAR OPACIDAD CADA FRAME SI ESTÁ HABILITADO
        if (forceOpaqueEveryFrame)
        {
            ForceOpaque();
        }
        
        // Billboard: hacer que el panel mire a la cámara
        if (lookAtCamera && mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
    
    /// <summary>
    /// FUERZA alpha 1.0 en TODOS los componentes visuales
    /// </summary>
    private void ForceOpaque()
    {
        // Forzar CanvasGroup
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
        
        // Forzar TODOS los CanvasGroups (por si hay varios)
        CanvasGroup[] allCanvasGroups = GetComponentsInChildren<CanvasGroup>(true);
        foreach (var cg in allCanvasGroups)
        {
            cg.alpha = 1f;
        }
        
        // Forzar TODOS los Image components
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach (var img in allImages)
        {
            Color c = img.color;
            if (c.a < 1f)
            {
                img.color = new Color(c.r, c.g, c.b, 1f);
            }
        }
        
        // Forzar TODOS los TextMeshProUGUI
        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var txt in allTexts)
        {
            Color c = txt.color;
            if (c.a < 1f)
            {
                txt.color = new Color(c.r, c.g, c.b, 1f);
            }
        }
    }
    
    /// <summary>
    /// Configura el panel con los datos obtenidos de Firebase
    /// </summary>
    public void SetData(BuildingData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[InfoPanelController] BuildingData es null");
            return;
        }
        
        Debug.Log($"[InfoPanelController] ?? Configurando panel con: {data.name}");
        
        // Actualizar textos
        if (titleText != null)
            titleText.text = data.name;
        
        if (descriptionText != null)
            descriptionText.text = data.description;
        
        if (coordinatesText != null)
            coordinatesText.text = data.GetFormattedCoordinates();
        
        // Ocultar loading
        HideLoading();
        
        // FORZAR OPACIDAD COMPLETA (sin animación)
        ForceOpaque();
        
        // NO iniciar fade-in, siempre opaco
        Debug.Log("[InfoPanelController] ? Panel configurado - Forzando opacidad 100%");
    }
    
    /// <summary>
    /// Muestra el indicador de carga
    /// </summary>
    public void ShowLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("[InfoPanelController] ? Mostrando loading...");
        }
        
        // Ocultar contenido mientras carga
        if (titleText != null) titleText.gameObject.SetActive(false);
        if (descriptionText != null) descriptionText.gameObject.SetActive(false);
        if (coordinatesText != null) coordinatesText.gameObject.SetActive(false);
        
        // FORZAR OPACIDAD
        ForceOpaque();
    }
    
    /// <summary>
    /// Oculta el indicador de carga y muestra el contenido
    /// </summary>
    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
        
        // Mostrar contenido
        if (titleText != null) titleText.gameObject.SetActive(true);
        if (descriptionText != null) descriptionText.gameObject.SetActive(true);
        if (coordinatesText != null) coordinatesText.gameObject.SetActive(true);
        
        // FORZAR OPACIDAD
        ForceOpaque();
        
        Debug.Log("[InfoPanelController] ? Loading ocultado, contenido visible - Opacidad forzada");
    }
    
    /// <summary>
    /// Corrutina que anima el fade-in del panel
    /// </summary>
    private IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        
        float elapsed = 0f;
        
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        Debug.Log("[InfoPanelController] Fade-in completado");
    }
    
    /// <summary>
    /// Método de utilidad para debugging - Muestra datos de prueba
    /// </summary>
    public void SetTestData()
    {
        var testData = new BuildingData(
            "Edificio de Prueba",
            "Esta es una descripción de prueba para verificar que el panel funciona correctamente.",
            13.7181033,
            -89.2040915
        );
        
        SetData(testData);
    }
}
