using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Ensures EventSystem exists and wires OpenLocationPanelButton once at startup.
/// Attach to the same GameObject as NavigationUIManager or a UI root.
/// </summary>
[RequireComponent(typeof(NavigationUIManager))]
public class NavigationUIHelper : MonoBehaviour
{
    void Start()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            Debug.Log("[NavigationUIHelper] EventSystem created");
        }

        var nav = GetComponent<NavigationUIManager>();
        if (nav == null || nav.openLocationPanelButton == null) return;

        var btn = nav.openLocationPanelButton;
        btn.onClick.RemoveListener(nav.OnOpenLocationPanelClicked);
        btn.onClick.AddListener(nav.OnOpenLocationPanelClicked);
        btn.interactable = true;

        var img = btn.GetComponent<Image>();
        if (img != null)
            UesTheme.ApplyButton(btn, isAccent: false);
    }
}
