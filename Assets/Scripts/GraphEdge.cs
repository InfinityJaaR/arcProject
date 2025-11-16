using System;
using UnityEngine;

/// <summary>
/// Modelo de datos que representa una arista del grafo de navegación
/// Estructura coincide con los documentos en la colección "graphEdges" de Firestore
/// </summary>
[Serializable]
public class GraphEdge
{
    public string source;      // ID del nodo origen
    public string target;      // ID del nodo destino
    public double distance;    // Distancia entre los dos nodos (para Dijkstra)
    
    // Constructor vacío para deserialización de Firebase
    public GraphEdge() { }
    
    /// <summary>
    /// Constructor para crear edges manualmente
    /// </summary>
    public GraphEdge(string source, string target, double distance)
    {
        this.source = source;
        this.target = target;
        this.distance = distance;
    }
    
    /// <summary>
    /// Verifica si esta arista conecta dos nodos específicos (en cualquier dirección)
    /// </summary>
    public bool ConnectsNodes(string nodeA, string nodeB)
    {
        return (source == nodeA && target == nodeB) || (source == nodeB && target == nodeA);
    }
    
    /// <summary>
    /// Obtiene el nodo del otro extremo de la arista
    /// </summary>
    public string GetOtherNode(string currentNode)
    {
        if (source == currentNode)
            return target;
        else if (target == currentNode)
            return source;
        else
            return null;
    }
}
