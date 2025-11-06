using System.Collections.Generic;
using System.Threading.Tasks;
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

    [Header("Modo de Operación")]
    [Tooltip("Usar Firebase para cargar datos dinámicos (requiere FirebaseManager en escena)")]
    public bool useFirebase = true;
    
    [Tooltip("Prefab genérico que se instancia para todos los marcadores cuando Firebase está activo")]
    public GameObject infoPanelPrefab;

    [Header("Configuración Legacy (Sin Firebase)")]
    [Tooltip("Mapeo manual de patrones a prefabs - Solo se usa si Firebase está desactivado")]
    public List<Entry> mappings = new();
    
    [Header("Configuración de Tracking")]
    [Tooltip("Si está activado, muestra objetos incluso con tracking limitado (útil para pantallas)")]
    public bool showWithLimitedTracking = true;

    Dictionary<string, Entry> byId;
    Dictionary<TrackableId, GameObject> spawned = new();
    ARTrackedImageManager mgr;

    void Awake()
    {
        mgr = GetComponent<ARTrackedImageManager>();
        byId = new Dictionary<string, Entry>();
        
        // Solo inicializar mapeo manual si no se usa Firebase
        if (!useFirebase)
        {
            foreach (var e in mappings)
            {
                if (!string.IsNullOrEmpty(e.id) && e.prefab)
                {
                    byId[e.id] = e;
                    Debug.Log($"[MultiImageSpawner] Mapeado: '{e.id}' -> Prefab: {e.prefab.name}");
                }
            }
            Debug.Log($"[MultiImageSpawner] Total de mapeos configurados: {byId.Count}");
        }
        else
        {
            Debug.Log("[MultiImageSpawner] ?? Modo Firebase ACTIVADO");
            
            if (infoPanelPrefab == null)
            {
                Debug.LogError("[MultiImageSpawner] ? infoPanelPrefab NO está asignado! Asigna el prefab en el Inspector");
            }
            
            if (FirebaseManager.Instance == null)
            {
                Debug.LogWarning("[MultiImageSpawner] ?? FirebaseManager no encontrado en la escena");
            }
        }
        
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

    async void HandleAdded(ARTrackedImage img)
    {
        string documentId = img.referenceImage.name;
        Debug.Log($"[MultiImageSpawner] ?? Imagen DETECTADA: '{documentId}' | TrackingState: {img.trackingState}");
        
        GameObject spawnedObject = null;
        
        // MODO FIREBASE: Consultar datos y usar InfoPanel
        if (useFirebase && infoPanelPrefab != null && FirebaseManager.Instance != null)
        {
            Debug.Log($"[MultiImageSpawner] ?? Usando Firebase para '{documentId}'");
            
            // Instanciar prefab InfoPanel
            spawnedObject = Instantiate(infoPanelPrefab, img.transform);
            spawnedObject.transform.localPosition = Vector3.zero;
            spawnedObject.transform.localRotation = Quaternion.identity;
            spawnedObject.transform.localScale = Vector3.one;
            
            // Obtener controlador del panel
            var controller = spawnedObject.GetComponent<InfoPanelController>();
            
            if (controller != null)
            {
                // Mostrar loading mientras consulta Firebase
                controller.ShowLoading();
                
                // Consultar Firebase de forma asíncrona
                Debug.Log($"[MultiImageSpawner] ? Consultando Firebase para '{documentId}'...");
                BuildingData data = await FirebaseManager.Instance.GetBuildingDataAsync(documentId);
                
                // Actualizar panel con datos obtenidos
                controller.SetData(data);
                Debug.Log($"[MultiImageSpawner] ? Panel actualizado con datos de '{data.name}'");
            }
            else
            {
                Debug.LogError("[MultiImageSpawner] ? InfoPanelController no encontrado en el prefab!");
            }
        }
        // MODO LEGACY: Usar mapeo manual
        else if (!useFirebase && byId.TryGetValue(documentId, out var map))
        {
            Debug.Log($"[MultiImageSpawner] ?? Usando mapeo manual para '{documentId}' -> Prefab: {map.prefab.name}");
            
            spawnedObject = Instantiate(map.prefab, img.transform);
            spawnedObject.transform.localPosition = map.localOffset;
            spawnedObject.transform.localRotation = Quaternion.Euler(map.localEuler);
            
            // Corregir escala: si está en (0,0,0) usar (1,1,1) por defecto
            if (map.localScale == Vector3.zero)
            {
                spawnedObject.transform.localScale = Vector3.one;
            }
            else
            {
                spawnedObject.transform.localScale = map.localScale;
            }
        }
        else
        {
            Debug.LogWarning($"[MultiImageSpawner] ?? No hay prefab/configuración para '{documentId}'");
            return;
        }
        
        // Guardar referencia del objeto spawneado
        if (spawnedObject != null)
        {
            spawned[img.trackableId] = spawnedObject;
            
            // Decidir si mostrar el objeto basado en el tracking state
            bool shouldShow = ShouldShowObject(img.trackingState);
            spawnedObject.SetActive(shouldShow);
            
            Debug.Log($"[MultiImageSpawner] ? Objeto spawneado | Activo: {shouldShow} | Posición: {spawnedObject.transform.position}");
        }
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
