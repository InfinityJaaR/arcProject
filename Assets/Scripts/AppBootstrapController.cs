using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Startup UX: permissions → GPS → Firebase → ready.
/// Attach to a bootstrap panel in SampleScene.
/// </summary>
public class AppBootstrapController : MonoBehaviour
{
    [Header("UI")]
    public GameObject bootstrapPanel;
    public TextMeshProUGUI statusText;
    public Slider progressBar;

    [Header("Timing")]
    public float minDisplaySeconds = 1.5f;
    public float firebaseTimeoutSeconds = 15f;

    private bool _ready;

    void Start()
    {
        if (bootstrapPanel != null)
            bootstrapPanel.SetActive(true);
        StartCoroutine(BootstrapSequence());
    }

    private IEnumerator BootstrapSequence()
    {
        float started = Time.time;
        SetProgress(0.1f, "Solicitando permisos...");

        while (PermissionsManager.Instance == null)
            yield return null;

        yield return new WaitForSeconds(0.5f);
        SetProgress(0.35f, "Iniciando GPS y brújula...");

        while (LocationManager.Instance == null)
            yield return null;

        float gpsWait = 0f;
        while (LocationManager.Instance != null && !LocationManager.Instance.IsGPSReady && gpsWait < 8f)
        {
            gpsWait += Time.deltaTime;
            yield return null;
        }

        SetProgress(0.6f, "Conectando con campus...");

        float fbWait = 0f;
        while (FirebaseManager.Instance == null && fbWait < 3f)
        {
            fbWait += Time.deltaTime;
            yield return null;
        }

        if (FirebaseManager.Instance != null)
        {
            while (!FirebaseManager.Instance.IsReady() && fbWait < firebaseTimeoutSeconds)
            {
                fbWait += Time.deltaTime;
                yield return null;
            }

            if (!FirebaseManager.Instance.IsReady() && LocalDataStore.HasSnapshot())
                SetProgress(0.85f, "Sin conexión — usando datos guardados");
            else if (!FirebaseManager.Instance.IsReady())
                SetProgress(0.85f, "Firebase no disponible — modo limitado");
            else
                SetProgress(0.9f, "Datos del campus listos");
        }
        else if (LocalDataStore.HasSnapshot())
        {
            SetProgress(0.85f, "Usando datos guardados");
        }

        float elapsed = Time.time - started;
        if (elapsed < minDisplaySeconds)
            yield return new WaitForSeconds(minDisplaySeconds - elapsed);

        SetProgress(1f, "Listo");
        yield return new WaitForSeconds(0.4f);

        if (bootstrapPanel != null)
            bootstrapPanel.SetActive(false);

        _ready = true;
    }

    private void SetProgress(float value, string message)
    {
        if (progressBar != null)
            progressBar.value = value;
        if (statusText != null)
        {
            statusText.text = message;
            UesTheme.ApplyBody(statusText);
        }
    }

    public bool IsReady => _ready;
}
