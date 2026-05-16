using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Persists campus data snapshot to disk for offline navigation and menus.
/// </summary>
public static class LocalDataStore
{
    private const string FileName = "campus_data_snapshot.json";

    [Serializable]
    public class BuildingEntry
    {
        public string id;
        public string name;
        public string description;
        public double latitude;
        public double longitude;
        public string nearby_places;
        public string type;
    }

    [Serializable]
    public class CampusSnapshot
    {
        public string lastSyncUtc;
        public BuildingEntry[] buildings = Array.Empty<BuildingEntry>();
        public GraphEdge[] edges = Array.Empty<GraphEdge>();
    }

    public static string SnapshotPath => Path.Combine(Application.persistentDataPath, FileName);

    public static bool HasSnapshot()
    {
        return File.Exists(SnapshotPath);
    }

    public static DateTime? GetLastSyncTime()
    {
        var snapshot = Load();
        if (snapshot == null || string.IsNullOrEmpty(snapshot.lastSyncUtc))
            return null;
        if (DateTime.TryParse(snapshot.lastSyncUtc, out var dt))
            return dt.ToUniversalTime();
        return null;
    }

    public static void Save(List<BuildingData> buildings, Dictionary<string, string> idByBuilding, List<GraphEdge> edges)
    {
        var entries = new List<BuildingEntry>();
        foreach (var b in buildings)
        {
            string id = !string.IsNullOrEmpty(b.documentId)
                ? b.documentId
                : (idByBuilding != null && idByBuilding.TryGetValue(b.name, out var mapped) ? mapped : "");
            entries.Add(new BuildingEntry
            {
                id = id,
                name = b.name,
                description = b.description,
                latitude = b.latitude,
                longitude = b.longitude,
                nearby_places = b.nearby_places ?? "",
                type = b.type ?? ""
            });
        }

        var snapshot = new CampusSnapshot
        {
            lastSyncUtc = DateTime.UtcNow.ToString("o"),
            buildings = entries.ToArray(),
            edges = edges?.ToArray() ?? Array.Empty<GraphEdge>()
        };

        try
        {
            File.WriteAllText(SnapshotPath, JsonUtility.ToJson(snapshot, true));
            Debug.Log($"[LocalDataStore] Snapshot saved: {entries.Count} buildings, {snapshot.edges.Length} edges");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LocalDataStore] Save failed: {ex.Message}");
        }
    }

    public static void SaveFromNodes(Dictionary<string, GraphNode> nodes, List<GraphEdge> edges)
    {
        var buildings = new List<BuildingData>();
        var ids = new Dictionary<string, string>();
        foreach (var kv in nodes)
        {
            var data = kv.Value.buildingData;
            data.documentId = kv.Key;
            buildings.Add(data);
            ids[data.name] = kv.Key;
        }
        Save(buildings, ids, edges);
    }

    public static CampusSnapshot Load()
    {
        if (!HasSnapshot())
            return null;
        try
        {
            string json = File.ReadAllText(SnapshotPath);
            return JsonUtility.FromJson<CampusSnapshot>(json);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[LocalDataStore] Load failed: {ex.Message}");
            return null;
        }
    }

    public static List<BuildingData> LoadBuildings()
    {
        var snapshot = Load();
        if (snapshot?.buildings == null)
            return new List<BuildingData>();

        var list = new List<BuildingData>();
        foreach (var e in snapshot.buildings)
        {
            list.Add(new BuildingData(e.name, e.description, e.latitude, e.longitude, e.nearby_places, e.type)
            {
                documentId = e.id
            });
        }
        return list;
    }

    public static List<GraphEdge> LoadEdges()
    {
        var snapshot = Load();
        if (snapshot?.edges == null)
            return new List<GraphEdge>();
        return new List<GraphEdge>(snapshot.edges);
    }

    public static (Dictionary<string, GraphNode> nodes, List<GraphEdge> edges) LoadGraph()
    {
        var snapshot = Load();
        var nodes = new Dictionary<string, GraphNode>();
        var edges = new List<GraphEdge>();

        if (snapshot == null)
            return (nodes, edges);

        if (snapshot.buildings != null)
        {
            foreach (var e in snapshot.buildings)
            {
                if (string.IsNullOrEmpty(e.id)) continue;
                var data = new BuildingData(e.name, e.description, e.latitude, e.longitude, e.nearby_places, e.type)
                {
                    documentId = e.id
                };
                nodes[e.id] = new GraphNode(e.id, data);
            }
        }

        if (snapshot.edges != null)
            edges.AddRange(snapshot.edges);

        return (nodes, edges);
    }
}
