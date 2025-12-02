using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controla el panel de feedback visual sobre el estado de la ruta.
/// </summary>
public class NavigationFeedbackController : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Mensajes")]
    [SerializeField] private string onRouteMessage = "Vas bien";
    [SerializeField] private string nearDeviationMessage = "Te estas desviando";
    [SerializeField] private string offRouteMessage = "Fuera de ruta";
    
    [Header("Colores")]
    [SerializeField] private Color onRouteColor = new Color(0.1f, 0.55f, 0.25f, 0.9f);
    [SerializeField] private Color nearDeviationColor = new Color(0.9f, 0.65f, 0.1f, 0.9f);
    [SerializeField] private Color offRouteColor = new Color(0.8f, 0.2f, 0.2f, 0.95f);
    
    [Header("Comportamiento")]
    [SerializeField] private float fadeDuration = 0.25f;
    
    private Coroutine fadeRoutine;
    private Coroutine subscriptionRoutine;
    private RouteStatus lastStatus = RouteStatus.Hidden;
    
    void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }
    
    void OnEnable()
    {
        if (subscriptionRoutine == null)
        {
            subscriptionRoutine = StartCoroutine(SubscribeWhenReady());
        }
    }
    
    void OnDisable()
    {
        if (subscriptionRoutine != null)
        {
            StopCoroutine(subscriptionRoutine);
            subscriptionRoutine = null;
        }
        
        UnsubscribeFromGraphManager();
    }
    
    private IEnumerator SubscribeWhenReady()
    {
        while (GraphNavigationManager.Instance == null)
        {
            yield return null;
        }
        
        SubscribeToGraphManager();
        subscriptionRoutine = null;
    }
    
    private void SubscribeToGraphManager()
    {
        if (GraphNavigationManager.Instance == null)
            return;
        
        GraphNavigationManager.Instance.OnRouteStatusChanged += HandleRouteStatusChanged;
        GraphNavigationManager.Instance.OnDestinationReached += HandleDestinationReached;
        
        // Sincronizar estado inicial
        HandleRouteStatusChanged(GraphNavigationManager.Instance.GetCurrentRouteStatus(), 0f);
    }
    
    private void UnsubscribeFromGraphManager()
    {
        if (GraphNavigationManager.Instance == null)
            return;
        
        GraphNavigationManager.Instance.OnRouteStatusChanged -= HandleRouteStatusChanged;
        GraphNavigationManager.Instance.OnDestinationReached -= HandleDestinationReached;
    }
    
    private void HandleRouteStatusChanged(RouteStatus status, float _)
    {
        lastStatus = status;
        
        switch (status)
        {
            case RouteStatus.OnRoute:
                ApplyVisualState(onRouteMessage, onRouteColor, true);
                break;
            case RouteStatus.NearDeviation:
                ApplyVisualState(nearDeviationMessage, nearDeviationColor, true);
                break;
            case RouteStatus.OffRoute:
                ApplyVisualState(offRouteMessage, offRouteColor, true);
                break;
            default:
                ApplyVisualState(string.Empty, onRouteColor, false);
                break;
        }
    }
    
    private void HandleDestinationReached(GraphNode _)
    {
        ApplyVisualState(string.Empty, onRouteColor, false);
    }
    
    private void ApplyVisualState(string message, Color color, bool shouldShow)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
        
        SetVisible(shouldShow);
    }
    
    private void SetVisible(bool visible)
    {
        if (canvasGroup == null)
            return;
        
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        
        fadeRoutine = StartCoroutine(FadeCanvas(visible ? 1f : 0f));
    }
    
    private IEnumerator FadeCanvas(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = fadeDuration > 0f ? Mathf.Clamp01(elapsed / fadeDuration) : 1f;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        canvasGroup.interactable = targetAlpha > 0.001f;
        canvasGroup.blocksRaycasts = targetAlpha > 0.001f;
        fadeRoutine = null;
    }
}



