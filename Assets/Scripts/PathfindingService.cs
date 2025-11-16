using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Servicio que implementa el algoritmo de Dijkstra para encontrar el camino más corto
/// en el grafo de navegación del campus
/// </summary>
public class PathfindingService
{
    private Dictionary<string, GraphNode> nodes;
    private List<GraphEdge> edges;
    
    /// <summary>
    /// Constructor
    /// </summary>
    public PathfindingService(Dictionary<string, GraphNode> nodes, List<GraphEdge> edges)
    {
        this.nodes = nodes;
        this.edges = edges;
        
        Debug.Log($"[PathfindingService] ? Inicializado con {nodes.Count} nodos y {edges.Count} aristas");
    }
    
    /// <summary>
    /// Encuentra el nodo más cercano a una posición GPS dada
    /// </summary>
    /// <param name="latitude">Latitud actual</param>
    /// <param name="longitude">Longitud actual</param>
    /// <param name="excludeBuildings">Si es true, solo considera nodos de inflexión</param>
    /// <returns>Nodo más cercano o null si no hay nodos</returns>
    public GraphNode FindNearestNode(double latitude, double longitude, bool excludeBuildings = false)
    {
        GraphNode nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (var node in nodes.Values)
        {
            // Filtrar edificios si se solicita
            if (excludeBuildings && node.IsBuilding)
                continue;
            
            float distance = node.DistanceTo(latitude, longitude);
            
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = node;
            }
        }
        
        if (nearest != null)
        {
            Debug.Log($"[PathfindingService] ?? Nodo más cercano: {nearest.Name} a {minDistance:F1}m");
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Encuentra el camino más corto entre dos nodos usando el algoritmo de Dijkstra
    /// </summary>
    /// <param name="startNodeId">ID del nodo de inicio</param>
    /// <param name="endNodeId">ID del nodo de destino</param>
    /// <returns>Lista de IDs de nodos que forman el camino, o null si no hay camino</returns>
    public List<string> FindShortestPath(string startNodeId, string endNodeId)
    {
        if (!nodes.ContainsKey(startNodeId) || !nodes.ContainsKey(endNodeId))
        {
            Debug.LogError($"[PathfindingService] ? Nodo no encontrado: start={startNodeId}, end={endNodeId}");
            return null;
        }
        
        Debug.Log($"[PathfindingService] ?? Buscando camino: {nodes[startNodeId].Name} ? {nodes[endNodeId].Name}");
        
        // Diccionario de distancias (inicialmente infinito excepto el nodo inicial)
        var distances = new Dictionary<string, double>();
        var previous = new Dictionary<string, string>();
        var unvisited = new HashSet<string>();
        
        // Inicializar
        foreach (var nodeId in nodes.Keys)
        {
            distances[nodeId] = double.PositiveInfinity;
            previous[nodeId] = null;
            unvisited.Add(nodeId);
        }
        
        distances[startNodeId] = 0;
        
        // Algoritmo de Dijkstra
        while (unvisited.Count > 0)
        {
            // Encontrar el nodo no visitado con menor distancia
            string current = null;
            double minDist = double.PositiveInfinity;
            
            foreach (var nodeId in unvisited)
            {
                if (distances[nodeId] < minDist)
                {
                    minDist = distances[nodeId];
                    current = nodeId;
                }
            }
            
            // Si no hay más nodos alcanzables
            if (current == null || minDist == double.PositiveInfinity)
                break;
            
            // Si llegamos al destino, podemos terminar
            if (current == endNodeId)
                break;
            
            unvisited.Remove(current);
            
            // Revisar todos los vecinos del nodo actual
            foreach (var edge in GetAdjacentEdges(current))
            {
                string neighbor = edge.GetOtherNode(current);
                
                if (!unvisited.Contains(neighbor))
                    continue;
                
                double alt = distances[current] + edge.distance;
                
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }
        
        // Reconstruir el camino
        if (previous[endNodeId] == null && startNodeId != endNodeId)
        {
            Debug.LogWarning($"[PathfindingService] ?? No hay camino entre {startNodeId} y {endNodeId}");
            return null;
        }
        
        var path = new List<string>();
        string currentNode = endNodeId;
        
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = previous[currentNode];
        }
        
        path.Reverse();
        
        // Log del camino encontrado
        Debug.Log($"[PathfindingService] ? Camino encontrado ({path.Count} nodos, {distances[endNodeId]:F1}m):");
        for (int i = 0; i < path.Count; i++)
        {
            string nodeId = path[i];
            string nodeName = nodes.ContainsKey(nodeId) ? nodes[nodeId].Name : nodeId;
            string arrow = i < path.Count - 1 ? " ? " : "";
            Debug.Log($"[PathfindingService]    {i + 1}. {nodeName}{arrow}");
        }
        
        return path;
    }
    
    /// <summary>
    /// Obtiene todas las aristas conectadas a un nodo
    /// Considera que las aristas son bidireccionales
    /// </summary>
    private List<GraphEdge> GetAdjacentEdges(string nodeId)
    {
        var adjacentEdges = new List<GraphEdge>();
        
        foreach (var edge in edges)
        {
            if (edge.source == nodeId || edge.target == nodeId)
            {
                adjacentEdges.Add(edge);
            }
        }
        
        return adjacentEdges;
    }
    
    /// <summary>
    /// Obtiene información del grafo para debugging
    /// </summary>
    public void PrintGraphInfo()
    {
        Debug.Log($"[PathfindingService] ?? Información del Grafo:");
        Debug.Log($"[PathfindingService]    Total de nodos: {nodes.Count}");
        Debug.Log($"[PathfindingService]    Total de aristas: {edges.Count}");
        
        int buildingCount = nodes.Values.Count(n => n.IsBuilding);
        int inflectionCount = nodes.Values.Count(n => n.IsInflectionNode);
        
        Debug.Log($"[PathfindingService]    Edificios: {buildingCount}");
        Debug.Log($"[PathfindingService]    Nodos de inflexión: {inflectionCount}");
        
        Debug.Log($"[PathfindingService] ?? ANÁLISIS DE CONECTIVIDAD:");
        
        // Verificar conectividad
        int connectedNodes = 0;
        int isolatedNodes = 0;
        
        foreach (var node in nodes.Values)
        {
            var adjacentEdges = GetAdjacentEdges(node.id);
            int connections = adjacentEdges.Count;
            
            if (connections > 0)
            {
                connectedNodes++;
                Debug.Log($"[PathfindingService]    ? {node.Name} ({node.Type}): {connections} conexiones");
                
                // Mostrar a qué nodos está conectado
                foreach (var edge in adjacentEdges)
                {
                    string otherNodeId = edge.GetOtherNode(node.id);
                    string otherNodeName = nodes.ContainsKey(otherNodeId) ? nodes[otherNodeId].Name : "???";
                    Debug.Log($"[PathfindingService]       ? {otherNodeName} ({edge.distance:F1}m)");
                }
            }
            else
            {
                isolatedNodes++;
                Debug.LogWarning($"[PathfindingService]    ? {node.Name} ({node.Type}): SIN CONEXIONES (nodo aislado)");
            }
        }
        
        Debug.Log($"[PathfindingService] ?? Resumen:");
        Debug.Log($"[PathfindingService]    Nodos conectados: {connectedNodes}");
        Debug.Log($"[PathfindingService]    Nodos aislados: {isolatedNodes}");
        
        if (isolatedNodes > 0)
        {
            Debug.LogWarning($"[PathfindingService] ?? HAY {isolatedNodes} NODOS SIN CONEXIONES!");
            Debug.LogWarning($"[PathfindingService] Esto causará errores de navegación.");
            Debug.LogWarning($"[PathfindingService] Verifica los IDs en graphEdges.");
        }
        
        // Verificar que los IDs en edges existen en nodes
        Debug.Log($"[PathfindingService] ?? VERIFICANDO IDS EN EDGES:");
        int invalidEdges = 0;
        
        foreach (var edge in edges)
        {
            bool sourceExists = nodes.ContainsKey(edge.source);
            bool targetExists = nodes.ContainsKey(edge.target);
            
            if (!sourceExists || !targetExists)
            {
                invalidEdges++;
                Debug.LogError($"[PathfindingService]    ? Edge inválido:");
                Debug.LogError($"[PathfindingService]       source: {edge.source} {(sourceExists ? "?" : "? NO EXISTE")}");
                Debug.LogError($"[PathfindingService]       target: {edge.target} {(targetExists ? "?" : "? NO EXISTE")}");
                Debug.LogError($"[PathfindingService]       distance: {edge.distance}m");
            }
        }
        
        if (invalidEdges > 0)
        {
            Debug.LogError($"[PathfindingService] ? HAY {invalidEdges} EDGES CON IDS INVÁLIDOS!");
            Debug.LogError($"[PathfindingService] Los IDs en source/target deben coincidir con los IDs de documentos en buildingLocations.");
        }
        else
        {
            Debug.Log($"[PathfindingService] ? Todos los edges tienen IDs válidos");
        }
    }
}
