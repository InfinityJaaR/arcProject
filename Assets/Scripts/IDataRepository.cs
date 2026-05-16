using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Contrato de acceso a datos del campus (implementado por FirebaseManager).
/// </summary>
public interface IDataRepository
{
    bool IsReady();
    Task<BuildingData> GetBuildingDataAsync(string documentId);
    Task<List<BuildingData>> GetAllBuildingsAsync();
    Task<(Dictionary<string, GraphNode> nodes, List<GraphEdge> edges)> BuildNavigationGraphAsync();
    void ClearCache();
}
