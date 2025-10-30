using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class MultiImageSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public string id;            // Debe ser igual al Name en la RIL
        public GameObject prefab;    // Lo que quieres que aparezca
        public Vector3 localOffset;  // Opcional (ej: 0,0.15,0)
        public Vector3 localEuler;   // Opcional (grados)
        public Vector3 localScale;   // Opcional (1,1,1 si no tocas)
    }

    [Header("Configuración de Tracking")]
    [Tooltip("Si está activado, muestra objetos incluso con tracking limitado (útil para pantallas)")]
    public bool showWithLimitedTracking = true;

    public List<Entry> mappings = new();
    Dictionary<string, Entry> byId;
    Dictionary<TrackableId, GameObject> spawned = new();
    ARTrackedImageManager mgr;

    void Awake()
    {
        mgr = GetComponent<ARTrackedImageManager>();
        byId = new Dictionary<string, Entry>();
        foreach (var e in mappings)
        {
            if (!string.IsNullOrEmpty(e.id) && e.prefab)
            {
                byId[e.id] = e;
                Debug.Log($"[MultiImageSpawner] Mapeado: '{e.id}' -> Prefab: {e.prefab.name}");
            }
        }
        Debug.Log($"[MultiImageSpawner] Total de mapeos configurados: {byId.Count}");
        Debug.Log($"[MultiImageSpawner] Modo tracking limitado: {showWithLimitedTracking}");
    }

    void OnEnable()
    {
        mgr.trackedImagesChanged += OnChanged;
        Debug.Log("[MultiImageSpawner] Escuchando eventos de imágenes detectadas");
    }

    void OnDisable()
    {
        mgr.trackedImagesChanged -= OnChanged;
    }

    void OnChanged(ARTrackedImagesChangedEventArgs e)
    {
        Debug.Log($"[MultiImageSpawner] Evento: {e.added.Count} añadidas, {e.updated.Count} actualizadas, {e.removed.Count} removidas");
        
        foreach (var img in e.added) HandleAdded(img);
        foreach (var img in e.updated) HandleUpdated(img);
        foreach (var img in e.removed) HandleRemoved(img);
    }

    void HandleAdded(ARTrackedImage img)
    {
        string id = img.referenceImage.name;
        Debug.Log($"[MultiImageSpawner] Imagen DETECTADA: '{id}' | TrackingState: {img.trackingState}");
        
        if (!byId.TryGetValue(id, out var map))
        {
            Debug.LogWarning($"[MultiImageSpawner] ?? No hay prefab mapeado para '{id}'");
            return;
        }

        Debug.Log($"[MultiImageSpawner] ? Spawneando prefab '{map.prefab.name}' para imagen '{id}'");
        
        var go = Instantiate(map.prefab, img.transform);
        go.transform.localPosition = map.localOffset;
        go.transform.localRotation = Quaternion.Euler(map.localEuler);
        
        // Corregir escala: si está en (0,0,0) usar (1,1,1) por defecto
        if (map.localScale == Vector3.zero)
        {
            go.transform.localScale = Vector3.one;
            Debug.Log($"[MultiImageSpawner] Escala por defecto aplicada (1,1,1)");
        }
        else
        {
            go.transform.localScale = map.localScale;
            Debug.Log($"[MultiImageSpawner] Escala personalizada aplicada: {map.localScale}");
        }
        
        spawned[img.trackableId] = go;

        // Decidir si mostrar el objeto basado en el tracking state
        bool shouldShow = ShouldShowObject(img.trackingState);
        go.SetActive(shouldShow);
        
        Debug.Log($"[MultiImageSpawner] Objeto '{go.name}' spawneado | Activo: {shouldShow} | Posición: {go.transform.position} | TrackingState: {img.trackingState}");
    }

    void HandleUpdated(ARTrackedImage img)
    {
        if (!spawned.TryGetValue(img.trackableId, out var go)) return;
        
        bool shouldShow = ShouldShowObject(img.trackingState);
        bool wasActive = go.activeSelf;
        
        if (wasActive != shouldShow)
        {
            go.SetActive(shouldShow);
            Debug.Log($"[MultiImageSpawner] Imagen '{img.referenceImage.name}' | TrackingState: {img.trackingState} | Visible: {shouldShow}");
        }
    }

    void HandleRemoved(ARTrackedImage img)
    {
        if (spawned.TryGetValue(img.trackableId, out var go))
        {
            Debug.Log($"[MultiImageSpawner] Imagen '{img.referenceImage.name}' REMOVIDA - Destruyendo objeto");
            Destroy(go);
            spawned.Remove(img.trackableId);
        }
    }

    bool ShouldShowObject(TrackingState state)
    {
        if (showWithLimitedTracking)
        {
            // Mostrar siempre cuando se detecta, incluso con TrackingState.None
            // Esto es necesario para imágenes mostradas en pantallas
            return true;
        }
        else
        {
            // Solo mostrar si el tracking es perfecto
            return state == TrackingState.Tracking;
        }
    }
}
