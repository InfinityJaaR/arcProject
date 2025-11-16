using System;
using UnityEngine;

/// <summary>
/// Representa un nodo en el grafo de navegación
/// Combina la información de BuildingData con su ID
/// </summary>
[Serializable]
public class GraphNode
{
    public string id;                  // ID del documento en Firestore
    public BuildingData buildingData;  // Datos del edificio/nodo (lat, lon, etc.)
    
    // Propiedades de acceso rápido
    public string Name => buildingData?.name ?? "Nodo sin nombre";
    public double Latitude => buildingData?.latitude ?? 0;
    public double Longitude => buildingData?.longitude ?? 0;
    public string Type => buildingData?.type ?? "";
    
    /// <summary>
    /// Verifica si este nodo es un edificio (destino final)
    /// </summary>
    public bool IsBuilding => buildingData != null && buildingData.IsEdificio();
    
    /// <summary>
    /// Verifica si este nodo es un nodo de inflexión (punto intermedio)
    /// </summary>
    public bool IsInflectionNode => !string.IsNullOrEmpty(Type) && Type.ToLower() == "nodo_de_inflexion";
    
    /// <summary>
    /// Constructor
    /// </summary>
    public GraphNode(string id, BuildingData buildingData)
    {
        this.id = id;
        this.buildingData = buildingData;
    }
    
    /// <summary>
    /// Calcula la distancia en metros desde este nodo a una coordenada específica
    /// </summary>
    public float DistanceTo(double lat, double lon)
    {
        return GeoUtils.CalculateDistance(Latitude, Longitude, lat, lon);
    }
    
    /// <summary>
    /// Calcula la distancia en metros desde este nodo a otro nodo
    /// </summary>
    public float DistanceTo(GraphNode other)
    {
        return DistanceTo(other.Latitude, other.Longitude);
    }
    
    public override string ToString()
    {
        return $"GraphNode({id}, {Name}, {Type})";
    }
}
