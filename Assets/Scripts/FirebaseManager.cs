using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

/// <summary>
/// Gestor singleton para todas las operaciones con Firebase Firestore.
/// Implementa IDataRepository para edificios y grafo de navegaci�n.
/// </summary>
public class FirebaseManager : MonoBehaviour, IDataRepository
{
    public static FirebaseManager Instance { get; private set; }

    [Header("Configuraci�n de Firestore")]
    [Tooltip("Nombre de la colecci�n en Firestore donde est�n los edificios")]
    public string collectionName = "buildingLocations";

    [Tooltip("Nombre de la colecci�n donde est�n las aristas del grafo")]
    public string graphEdgesCollectionName = "graphEdges";

    [Tooltip("Habilitar cach� local para evitar consultas repetidas al mismo documento")]
    public bool enableCache = true;

    [Header("Offline")]
    public bool enableLocalPersistence = true;
    public bool enableFirestorePersistence = true;
    public bool IsUsingLocalSnapshot { get; private set; }

    [Header("Debug")]
    [Tooltip("Simular datos en Unity Editor (sin necesidad de Firebase)")]
    public bool simulateDataInEditor = false;

    private FirebaseFirestore db;
    private bool isInitialized = false;
    private Dictionary<string, BuildingData> cache = new Dictionary<string, BuildingData>();
    private Dictionary<string, GraphNode> graphNodesCache = new Dictionary<string, GraphNode>();
    private List<GraphEdge> graphEdgesCache = new List<GraphEdge>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeFirebase();
    }

    public bool IsReady() => isInitialized;

    public void ClearCache()
    {
        cache.Clear();
        graphNodesCache.Clear();
        graphEdgesCache.Clear();
        Debug.Log("[FirebaseManager] Cache limpiada");
    }

    public int GetCacheCount()
    {
        return cache.Count + graphNodesCache.Count + graphEdgesCache.Count;
    }

    private void InitializeFirebase()
    {
        Debug.Log("[FirebaseManager] Inicializando Firebase...");

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;

                if (enableFirestorePersistence)
                {
                    db.Settings.PersistenceEnabled = true;
                }

                isInitialized = true;
                Debug.Log("[FirebaseManager] Firebase inicializado correctamente");
                Debug.Log($"[FirebaseManager] Colecci�n: '{collectionName}'");
                Debug.Log($"[FirebaseManager] Cach�: {(enableCache ? "Activado" : "Desactivado")}");
            }
            else
            {
                Debug.LogError($"[FirebaseManager] Error al inicializar Firebase: {dependencyStatus}");
                Debug.LogError("[FirebaseManager] Verifica que google-services.json est� en Assets/ y sea v�lido");
            }
        });
    }

    public async Task<BuildingData> GetBuildingDataAsync(string documentId)
    {
        if (string.IsNullOrEmpty(documentId))
        {
            Debug.LogWarning("[FirebaseManager] documentId est� vac�o");
            return BuildingData.GetFallback(documentId);
        }

#if UNITY_EDITOR
        if (simulateDataInEditor)
        {
            Debug.Log("[FirebaseManager] Modo simulaci�n activo - Retornando datos de prueba");
            await Task.Delay(500);
            return new BuildingData(
                $"Edificio de Prueba ({documentId.Substring(0, Math.Min(5, documentId.Length))}...)",
                "Este es un dato simulado para testing en Unity Editor.\n\nEn build real se consultar� Firebase.",
                13.7181033,
                -89.2040915,
                "",
                "edificio"
            ) { documentId = documentId };
        }
#endif

        if (enableCache && cache.TryGetValue(documentId, out var cached))
        {
            Debug.Log($"[FirebaseManager] Datos de '{documentId}' obtenidos desde CACH�");
            return cached;
        }

        if (!isInitialized)
        {
            Debug.LogWarning("[FirebaseManager] Firebase no est� inicializado a�n, esperando...");
            await Task.Delay(1000);

            if (!isInitialized)
            {
                var offline = TryGetBuildingFromLocal(documentId);
                if (offline != null) return offline;
                Debug.LogError("[FirebaseManager] Firebase no se pudo inicializar");
                return BuildingData.GetFallback(documentId);
            }
        }

        try
        {
            Debug.Log($"[FirebaseManager] Consultando Firestore: {collectionName}/{documentId}");

            DocumentSnapshot snapshot = await db.Collection(collectionName).Document(documentId).GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Debug.LogWarning($"[FirebaseManager] Documento '{documentId}' NO EXISTE en Firestore");
                return BuildingData.GetNotFound(documentId);
            }

            BuildingData buildingData = ParseDocument(snapshot);
            buildingData.documentId = documentId;
            IsUsingLocalSnapshot = false;

            if (enableCache)
                cache[documentId] = buildingData;

            Debug.Log($"[FirebaseManager] Datos obtenidos: '{buildingData.name}'");
            return buildingData;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[FirebaseManager] Error Firebase: {ex.ErrorCode} - {ex.Message}");

            if (ex.ErrorCode == 7)
            {
                Debug.LogError("[FirebaseManager] PERMISSION_DENIED - Revisa reglas de Firestore");
                return BuildingData.GetPermissionDenied(documentId);
            }

            var offline = TryGetBuildingFromLocal(documentId);
            return offline ?? BuildingData.GetFallback(documentId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al consultar Firestore: {ex.Message}");
            var offline = TryGetBuildingFromLocal(documentId);
            return offline ?? BuildingData.GetFallback(documentId);
        }
    }

    public async Task<List<BuildingData>> GetAllBuildingsAsync()
    {
        Debug.Log("[FirebaseManager] Obteniendo lista de todos los edificios...");

#if UNITY_EDITOR
        if (simulateDataInEditor)
        {
            await Task.Delay(300);
            return GetSimulatedBuildings();
        }
#endif

        if (!isInitialized)
        {
            await Task.Delay(1000);
            if (!isInitialized)
                return LoadBuildingsFromLocalOrEmpty();
        }

        try
        {
            Debug.Log($"[FirebaseManager] Consultando colecci�n '{collectionName}'...");
            QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();
            var buildings = new List<BuildingData>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (!document.Exists) continue;

                BuildingData building = ParseDocument(document);
                building.documentId = document.Id;
                buildings.Add(building);

                if (enableCache)
                    cache[document.Id] = building;
            }

            IsUsingLocalSnapshot = false;
            Debug.Log($"[FirebaseManager] Se obtuvieron {buildings.Count} edificios");
            return buildings;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[FirebaseManager] Error Firebase: {ex.ErrorCode} - {ex.Message}");
            return LoadBuildingsFromLocalOrEmpty();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al obtener edificios: {ex.Message}");
            return LoadBuildingsFromLocalOrEmpty();
        }
    }

    public async Task<List<GraphEdge>> GetAllGraphEdgesAsync()
    {
#if UNITY_EDITOR
        if (simulateDataInEditor)
        {
            await Task.Delay(200);
            var simulated = GetSimulatedGraphEdges();
            graphEdgesCache = simulated;
            return simulated;
        }
#endif

        if (enableCache && graphEdgesCache.Count > 0)
            return graphEdgesCache;

        if (!isInitialized)
        {
            await Task.Delay(1000);
            if (!isInitialized)
                return LoadEdgesFromLocalOrEmpty();
        }

        try
        {
            Debug.Log($"[FirebaseManager] Consultando colecci�n '{graphEdgesCollectionName}'...");
            QuerySnapshot snapshot = await db.Collection(graphEdgesCollectionName).GetSnapshotAsync();
            var edges = new List<GraphEdge>();
            int skipped = 0;

            Debug.Log($"[FirebaseManager] Documentos en {graphEdgesCollectionName}: {snapshot.Count}");

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (!document.Exists) continue;

                GraphEdge edge = ParseGraphEdge(document);
                if (edge != null)
                    edges.Add(edge);
                else
                    skipped++;
            }

            graphEdgesCache = edges;
            IsUsingLocalSnapshot = false;
            Debug.Log($"[FirebaseManager] Edges parseados: {edges.Count}, saltados: {skipped}");
            return edges;
        }
        catch (FirebaseException ex)
        {
            Debug.LogError($"[FirebaseManager] Error Firebase edges: {ex.ErrorCode}");
            return LoadEdgesFromLocalOrEmpty();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al obtener edges: {ex.Message}");
            return LoadEdgesFromLocalOrEmpty();
        }
    }

    public async Task<(Dictionary<string, GraphNode> nodes, List<GraphEdge> edges)> BuildNavigationGraphAsync()
    {
        Debug.Log("[FirebaseManager] Construyendo grafo de navegaci�n...");

        if (enableCache && graphNodesCache.Count > 0 && graphEdgesCache.Count > 0)
        {
            Debug.Log("[FirebaseManager] Grafo desde cach� en memoria");
            return (graphNodesCache, graphEdgesCache);
        }

        if (!isInitialized && enableLocalPersistence && LocalDataStore.HasSnapshot())
        {
            var local = LocalDataStore.LoadGraph();
            graphNodesCache = local.nodes;
            graphEdgesCache = local.edges;
            IsUsingLocalSnapshot = true;
            return local;
        }

#if UNITY_EDITOR
        if (simulateDataInEditor)
        {
            await Task.Delay(300);
            var nodes = new Dictionary<string, GraphNode>();
            foreach (var b in GetSimulatedBuildings())
            {
                string id = string.IsNullOrEmpty(b.documentId) ? b.name : b.documentId;
                nodes[id] = new GraphNode(id, b);
            }
            var simEdges = GetSimulatedGraphEdges();
            graphNodesCache = nodes;
            graphEdgesCache = simEdges;
            return (nodes, simEdges);
        }
#endif

        if (!isInitialized)
        {
            Debug.LogError("[FirebaseManager] Firebase no inicializado");
            return (new Dictionary<string, GraphNode>(), new List<GraphEdge>());
        }

        var graphNodes = new Dictionary<string, GraphNode>();

        try
        {
            QuerySnapshot snapshot = await db.Collection(collectionName).GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (!document.Exists) continue;

                BuildingData buildingData = ParseDocument(document);
                buildingData.documentId = document.Id;
                graphNodes[document.Id] = new GraphNode(document.Id, buildingData);
            }

            Debug.Log($"[FirebaseManager] Nodos creados: {graphNodes.Count}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al construir nodos: {ex.Message}");
            if (enableLocalPersistence && LocalDataStore.HasSnapshot())
            {
                var local = LocalDataStore.LoadGraph();
                graphNodesCache = local.nodes;
                graphEdgesCache = local.edges;
                IsUsingLocalSnapshot = true;
                return local;
            }
            return (graphNodes, new List<GraphEdge>());
        }

        List<GraphEdge> edges = await GetAllGraphEdgesAsync();

        if (enableCache)
            graphNodesCache = graphNodes;

        if (enableLocalPersistence && graphNodes.Count > 0)
        {
            LocalDataStore.SaveFromNodes(graphNodes, edges);
            IsUsingLocalSnapshot = false;
        }

        Debug.Log($"[FirebaseManager] Grafo construido: {graphNodes.Count} nodos, {edges.Count} edges");
        return (graphNodes, edges);
    }

    private BuildingData TryGetBuildingFromLocal(string documentId)
    {
        if (!enableLocalPersistence || !LocalDataStore.HasSnapshot())
            return null;

        foreach (var b in LocalDataStore.LoadBuildings())
        {
            if (b.documentId == documentId)
            {
                IsUsingLocalSnapshot = true;
                if (enableCache)
                    cache[documentId] = b;
                Debug.Log($"[FirebaseManager] Building '{documentId}' from local snapshot");
                return b;
            }
        }

        return null;
    }

    private List<BuildingData> LoadBuildingsFromLocalOrEmpty()
    {
        if (!enableLocalPersistence || !LocalDataStore.HasSnapshot())
            return new List<BuildingData>();

        var list = LocalDataStore.LoadBuildings();
        IsUsingLocalSnapshot = true;

        if (enableCache)
        {
            foreach (var b in list)
            {
                if (!string.IsNullOrEmpty(b.documentId))
                    cache[b.documentId] = b;
            }
        }

        Debug.Log($"[FirebaseManager] {list.Count} edificios desde snapshot local");
        return list;
    }

    private List<GraphEdge> LoadEdgesFromLocalOrEmpty()
    {
        if (!enableLocalPersistence || !LocalDataStore.HasSnapshot())
            return new List<GraphEdge>();

        var edges = LocalDataStore.LoadEdges();
        graphEdgesCache = edges;
        IsUsingLocalSnapshot = true;
        Debug.Log($"[FirebaseManager] {edges.Count} edges desde snapshot local");
        return edges;
    }

    private BuildingData ParseDocument(DocumentSnapshot snapshot)
    {
        var data = new BuildingData();

        try
        {
            if (snapshot.ContainsField("name"))
                data.name = snapshot.GetValue<string>("name") ?? "";

            if (snapshot.ContainsField("description"))
                data.description = snapshot.GetValue<string>("description") ?? "";

            if (snapshot.ContainsField("latitude"))
                data.latitude = Convert.ToDouble(snapshot.GetValue<object>("latitude"));

            if (snapshot.ContainsField("longitude"))
                data.longitude = Convert.ToDouble(snapshot.GetValue<object>("longitude"));

            if (snapshot.ContainsField("nearby_places"))
                data.nearby_places = snapshot.GetValue<string>("nearby_places") ?? "";

            if (snapshot.ContainsField("type"))
                data.type = snapshot.GetValue<string>("type") ?? "";

            if (!string.IsNullOrEmpty(snapshot.Id))
                data.documentId = snapshot.Id;

            if (!string.IsNullOrEmpty(data.nearby_places))
                Debug.Log($"[FirebaseManager] Lugares cercanos: {data.nearby_places}");

            Debug.Log($"[FirebaseManager] Documento parseado: {data.name} (type={data.type})");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al parsear documento: {ex.Message}");
        }

        return data;
    }

    private GraphEdge ParseGraphEdge(DocumentSnapshot snapshot)
    {
        try
        {
            Debug.Log($"[FirebaseManager] Parseando edge: {snapshot.Id}");

            if (!snapshot.ContainsField("source") || !snapshot.ContainsField("target"))
            {
                Debug.LogWarning($"[FirebaseManager] Edge sin source/target: {snapshot.Id}");
                return null;
            }

            string source = snapshot.GetValue<string>("source");
            string target = snapshot.GetValue<string>("target");

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                Debug.LogWarning($"[FirebaseManager] Edge con source/target vac�o: {snapshot.Id}");
                return null;
            }

            double distance = 0;
            if (snapshot.ContainsField("distance"))
                distance = Convert.ToDouble(snapshot.GetValue<object>("distance"));

            Debug.Log($"[FirebaseManager]    source: '{source}'");
            Debug.Log($"[FirebaseManager]    target: '{target}'");
            Debug.Log($"[FirebaseManager]    distance: {distance}m");

            var edge = new GraphEdge(source, target, distance);
            Debug.Log("[FirebaseManager] Edge parseado correctamente");
            return edge;
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] Error al parsear edge {snapshot.Id}: {ex.Message}");
            return null;
        }
    }

    private List<BuildingData> GetSimulatedBuildings()
    {
        return new List<BuildingData>
        {
            new BuildingData("Biblioteca Central", "Edificio simulado", 13.7181033, -89.2040915, "Cafeter�a", "edificio")
                { documentId = "building_biblioteca" },
            new BuildingData("Facultad de Ingenier�a", "Edificio simulado", 13.7185, -89.2045, "", "edificio")
                { documentId = "building_ingenieria" },
            new BuildingData("Rector�a", "Edificio simulado", 13.7190, -89.2050, "", "edificio")
                { documentId = "building_rectoria" },
            new BuildingData("Nodo 1", "Nodo de inflexi�n simulado", 13.7183, -89.2043, "", "nodo_de_inflexion")
                { documentId = "node_1" },
            new BuildingData("Nodo 2", "Nodo de inflexi�n simulado", 13.7187, -89.2047, "", "nodo_de_inflexion")
                { documentId = "node_2" }
        };
    }

    private List<GraphEdge> GetSimulatedGraphEdges()
    {
        return new List<GraphEdge>
        {
            new GraphEdge("building_biblioteca", "node_1", 50),
            new GraphEdge("node_1", "building_ingenieria", 80),
            new GraphEdge("node_1", "node_2", 40),
            new GraphEdge("node_2", "building_rectoria", 60)
        };
    }
}
