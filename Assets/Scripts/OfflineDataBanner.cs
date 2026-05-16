using System;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows when navigation data comes from local snapshot instead of live Firebase.
/// </summary>
public class OfflineDataBanner : MonoBehaviour
{
    public GameObject bannerRoot;
    public TextMeshProUGUI bannerText;
    public float refreshIntervalSeconds = 5f;

    private float _nextRefresh;

    void Update()
    {
        if (Time.time < _nextRefresh) return;
        _nextRefresh = Time.time + refreshIntervalSeconds;
        Refresh();
    }

    void OnEnable() => Refresh();

    public void Refresh()
    {
        bool show = false;
        string message = "";

        if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsUsingLocalSnapshot)
        {
            show = true;
            var t = LocalDataStore.GetLastSyncTime();
            message = t.HasValue
                ? $"Modo sin conexión — datos del {t.Value.ToLocalTime():g}"
                : "Modo sin conexión — datos guardados";
        }

        if (bannerRoot != null)
            bannerRoot.SetActive(show);
        if (bannerText != null && show)
            bannerText.text = message;
    }
}
