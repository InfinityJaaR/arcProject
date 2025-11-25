using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestor de navegación basado en grafo.
/// Calcula rutas usando Dijkstra y gestiona el seguimiento de nodos 8
/// </summary>
public class GraphNavigationManager : MonoBehaviour
{
    public static GraphNavigationManager Instance { get; private set; }
    
    [Header("Configuración")]
    [Tooltip("Distancia en metros para considerar que llegaste a un nodo")]
    public float nodeReachedDistance = 10f;
    
    [Tooltip("Actualizar el nodo objetivo cada N segundos")]
    public float updateInterval = 1f;
    
    [Header("Recalculación de Ruta")]
    [Tooltip("Activar recalculación automática si el usuario se desvía")]
    public bool enableRouteRecalculation = true;
    
    [Tooltip("Distancia máxima en metros permitida desde la ruta antes de recalcular")]
    public float maxDeviationDistance = 30f;
    
    [Tooltip("Tiempo mínimo en segundos entre recalculaciones (evita spam)")]
    public float recalculationCooldown = 5f;
    
    [Tooltip("Verificar desviación cada N segundos")]
    public float deviationCheckInterval = 2f;
    
    [Header("Estado")]
    [SerializeField] private bool isNavigating = false;
    [SerializeField] private string destinationNodeId;
    [SerializeField] private int currentPathIndex = 0;
    
    // Sistema de pathfinding
    private PathfindingService pathfindingService;
    private Dictionary<string, GraphNode> graphNodes;
    private List<GraphEdge> graphEdges;
    
    // Ruta actual
    private List<string> currentPath;
    private GraphNode currentTargetNode;
    private GraphNode finalDestinationNode;
    
    // Control de recalculación
    private float lastRecalculationTime = 0f;
    private float lastDeviationCheckTime = 0f;
    
    // Eventos
    public System.Action<GraphNode> OnTargetNodeChanged;
    public System.Action<GraphNode> OnDestinationReached;
    public System.Action<int, int> OnPathProgressChanged; // (current, total)
    public System.Action OnRouteRecalculated;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    void Start()
    {
        StartCoroutine(InitializeGraph());
    }
    
    /// <summary>
    /// Inicializa el grafo de navegación desde Firebase
    /// </summary>
    private IEnumerator InitializeGraph()
    {
        Debug.Log("[GraphNavigationManager] ?? Inicializando grafo de navegación...");
        
        if (FirebaseManager.Instance == null)
        {
            Debug.LogError("[GraphNavigationManager] ? FirebaseManager no encontrado");
            Debug.LogError("[GraphNavigationManager] ?? Verifica que exista un GameObject con FirebaseManager en la escena");
            yield break;
        }
        
        Debug.Log("[GraphNavigationManager] ? FirebaseManager encontrado");
        
        // Esperar a que Firebase esté listo
        int waitCount = 0;
        while (!FirebaseManager.Instance.IsReady())
        {
            waitCount++;
            if (waitCount % 10 == 0)
            {
                Debug.Log($"[GraphNavigationManager] ? Esperando Firebase... ({waitCount/2}s)");
            }
            yield return new WaitForSeconds(0.5f);
            
            if (waitCount > 60) // 30 segundos timeout
            {
                Debug.LogError("[GraphNavigationManager] ? Timeout esperando Firebase");
                yield break;
            }
        }
        
        Debug.Log("[GraphNavigationManager] ? Firebase listo");
        Debug.Log("[GraphNavigationManager] ?? Descargando grafo desde Firestore...");
        
        // Construir el grafo
        var graphTask = FirebaseManager.Instance.BuildNavigationGraphAsync();
        
        yield return new WaitUntil(() => graphTask.IsCompleted);
        
        if (graphTask.IsFaulted)
        {
            Debug.LogError($"[GraphNavigationManager] ? Error al construir grafo: {graphTask.Exception}");
            yield break;
        }
        
        var result = graphTask.Result;
        graphNodes = result.nodes;
        graphEdges = result.edges;
        
        Debug.Log($"[GraphNavigationManager] ?? Nodos descargados: {graphNodes.Count}");
        Debug.Log($"[GraphNavigationManager] ?? Edges descargados: {graphEdges.Count}");
        
        if (graphNodes.Count == 0 || graphEdges.Count == 0)
        {
            Debug.LogError("[GraphNavigationManager] ? Grafo vacío - verifica Firestore");
            Debug.LogError("[GraphNavigationManager] ?? Instrucciones: Assets/INSTRUCCIONES_FIRESTORE_GRAFO.md");
            yield break;
        }
        
        // Crear el servicio de pathfinding
        pathfindingService = new PathfindingService(graphNodes, graphEdges);
        pathfindingService.PrintGraphInfo();
        
        Debug.Log("[GraphNavigationManager] ? Grafo inicializado correctamente");
        Debug.Log($"[GraphNavigationManager] ?? Sistema listo para navegar");
    }
    
    /// <summary>
    /// Inicia la navegación hacia un edificio destino
    /// </summary>
    public void StartNavigationToBuilding(BuildingData destination)
    {
        Debug.Log($"[GraphNavigationManager] ?? StartNavigationToBuilding llamado: {destination.name}");
        
        if (pathfindingService == null)
        {
            Debug.LogError("[GraphNavigationManager] ? Grafo no inicializado - pathfindingService es null");
            Debug.LogError("[GraphNavigationManager] ?? Asegúrate de que Firestore esté configurado correctamente");
            return;
        }
        
        if (LocationManager.Instance == null)
        {
            Debug.LogError("[GraphNavigationManager] ? LocationManager.Instance es null");
            return;
        }
        
        if (!LocationManager.Instance.IsGPSReady)
        {
            Debug.LogError("[GraphNavigationManager] ? GPS no está listo");
            Debug.LogError($"[GraphNavigationManager] ?? Lat: {LocationManager.Instance.CurrentLatitude}, Lon: {LocationManager.Instance.CurrentLongitude}");
            return;
        }
        
        Debug.Log($"[GraphNavigationManager] ? Todos los requisitos OK");
        Debug.Log($"[GraphNavigationManager] ?? Buscando nodo para: {destination.name}");
        
        // Buscar el nodo del destino
        GraphNode destinationNode = FindNodeByBuildingData(destination);
        
        if (destinationNode == null)
        {
            Debug.LogError($"[GraphNavigationManager] ? No se encontró el nodo para {destination.name}");
            Debug.LogError($"[GraphNavigationManager] ?? Nodos disponibles en el grafo: {graphNodes.Count}");
            
            if (graphNodes.Count > 0)
            {
                Debug.Log("[GraphNavigationManager] ?? Lista de nodos en el grafo:");
                int count = 0;
                foreach (var node in graphNodes.Values)
                {
                    Debug.Log($"[GraphNavigationManager]    {count++}. {node.Name} ({node.Latitude:F6}, {node.Longitude:F6})");
                    if (count >= 5) break; // Solo mostrar los primeros 5
                }
            }
            return;
        }
        
        Debug.Log($"[GraphNavigationManager] ? Nodo encontrado: {destinationNode.Name}");
        StartNavigationToNode(destinationNode.id);
    }
    
    /// <summary>
    /// Inicia la navegación hacia un nodo específico
    /// </summary>
    public void StartNavigationToNode(string nodeId)
    {
        Debug.Log($"[GraphNavigationManager] ?? StartNavigationToNode: {nodeId}");
        
        try
        {
            if (pathfindingService == null)
            {
                Debug.LogError("[GraphNavigationManager] ? Pathfinding no inicializado");
                return;
            }
            
            if (!graphNodes.ContainsKey(nodeId))
            {
                Debug.LogError($"[GraphNavigationManager] ? Nodo no existe: {nodeId}");
                return;
            }
            
            if (LocationManager.Instance == null || !LocationManager.Instance.IsGPSReady)
            {
                Debug.LogError("[GraphNavigationManager] ? GPS no disponible en StartNavigationToNode");
                return;
            }
            
            // Encontrar el nodo más cercano a la posición actual
            double currentLat = LocationManager.Instance.CurrentLatitude;
            double currentLon = LocationManager.Instance.CurrentLongitude;
            
            Debug.Log($"[GraphNavigationManager] ?? Posición actual: {currentLat:F6}, {currentLon:F6}");
            
            GraphNode startNode = pathfindingService.FindNearestNode(currentLat, currentLon);
            
            if (startNode == null)
            {
                Debug.LogError("[GraphNavigationManager] ? No se pudo encontrar nodo cercano");
                return;
            }
            
            Debug.Log($"[GraphNavigationManager] ?? Nodo más cercano a tu posición: {startNode.Name}");
            
            // ? NUEVO: Si ya estás en el nodo destino o muy cerca, navega directo
            if (startNode.id == nodeId)
            {
                Debug.Log($"[GraphNavigationManager] ?? Ya estás en el destino o muy cerca");
                Debug.Log($"[GraphNavigationManager] ?? Navegación directa al destino");
                
                // Crear una ruta de un solo nodo (directo al destino)
                currentPath = new List<string> { nodeId };
                destinationNodeId = nodeId;
                finalDestinationNode = graphNodes[nodeId];
                currentPathIndex = 0;
                isNavigating = true;
                
                Debug.Log($"[GraphNavigationManager] ? Navegación iniciada (directa)");
                Debug.Log($"[GraphNavigationManager] ??? Ruta: Directo al destino");
                
                // Establecer el nodo objetivo
                UpdateCurrentTarget();
                
                // Iniciar corrutina de actualización
                StartCoroutine(NavigationUpdateLoop());
                return;
            }
            
            Debug.Log($"[GraphNavigationManager] ?? Calculando ruta de {startNode.id} a {nodeId}...");
            
            // Calcular la ruta
            currentPath = pathfindingService.FindShortestPath(startNode.id, nodeId);
            
            if (currentPath == null || currentPath.Count == 0)
            {
                Debug.LogError("[GraphNavigationManager] ? No se encontró ruta");
                Debug.LogError($"[GraphNavigationManager] Nodo inicio: {startNode.Name} ({startNode.id})");
                Debug.LogError($"[GraphNavigationManager] Nodo destino: {graphNodes[nodeId].Name} ({nodeId})");
                return;
            }
            
            Debug.Log($"[GraphNavigationManager] ? Ruta calculada exitosamente");
            
            // Configurar la navegación
            destinationNodeId = nodeId;
            finalDestinationNode = graphNodes[nodeId];
            currentPathIndex = 0;
            isNavigating = true;
            
            Debug.Log($"[GraphNavigationManager] ? Navegación iniciada");
            Debug.Log($"[GraphNavigationManager] ??? Ruta calculada: {currentPath.Count} nodos");
            
            // Establecer el primer nodo objetivo
            Debug.Log($"[GraphNavigationManager] ?? Llamando UpdateCurrentTarget()...");
            UpdateCurrentTarget();
            Debug.Log($"[GraphNavigationManager] ? UpdateCurrentTarget() completado");
            
            // Iniciar corrutina de actualización
            Debug.Log($"[GraphNavigationManager] ?? Iniciando NavigationUpdateLoop...");
            StartCoroutine(NavigationUpdateLoop());
            Debug.Log($"[GraphNavigationManager] ? NavigationUpdateLoop iniciado");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GraphNavigationManager] ? EXCEPCIÓN en StartNavigationToNode:");
            Debug.LogError($"[GraphNavigationManager] Mensaje: {ex.Message}");
            Debug.LogError($"[GraphNavigationManager] Stack: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Detiene la navegación actual
    /// </summary>
    public void StopNavigation()
    {
        isNavigating = false;
        currentPath = null;
        currentTargetNode = null;
        finalDestinationNode = null;
        currentPathIndex = 0;
        
        StopAllCoroutines();
        
        Debug.Log("[GraphNavigationManager] ?? Navegación detenida");
    }
    
    /// <summary>
    /// Loop principal de actualización de navegación
    /// </summary>
    private IEnumerator NavigationUpdateLoop()
    {
        while (isNavigating)
        {
            yield return new WaitForSeconds(updateInterval);
            
            if (LocationManager.Instance == null || !LocationManager.Instance.IsGPSReady)
                continue;
            
            // Verificar progreso del nodo actual
            CheckNodeProgress();
            
            // Verificar desviación de la ruta
            if (enableRouteRecalculation)
            {
                CheckRouteDeviation();
            }
        }
    }
    
    /// <summary>
    /// Verifica si el usuario se ha desviado significativamente de la ruta
    /// y recalcula si es necesario
    /// </summary>
    private void CheckRouteDeviation()
    {
        // Throttle: solo verificar cada deviationCheckInterval segundos.
        if (Time.time - lastDeviationCheckTime < deviationCheckInterval)
            return;
        
        lastDeviationCheckTime = Time.time;
        
        try
        {
            // Obtener posición actual del usuario
            double currentLat = LocationManager.Instance.CurrentLatitude;
            double currentLon = LocationManager.Instance.CurrentLongitude;
            
            Debug.Log($"[GraphNavigationManager] 🔍 Verificación de desviación:");
            Debug.Log($"[GraphNavigationManager]    📍 Posición GPS actual: {currentLat:F6}, {currentLon:F6}");
            
            // NUEVA LÓGICA: Primero verificar si está cerca de algún nodo de la ruta actual
            bool isNearRouteNode = false;
            GraphNode nearestRouteNode = null;
            float minDistanceToRoute = float.MaxValue;
            
            Debug.Log($"[GraphNavigationManager]    🔎 Verificando distancia a nodos de la ruta actual...");
            
            foreach (string nodeId in currentPath)
            {
                if (graphNodes.ContainsKey(nodeId))
                {
                    GraphNode routeNode = graphNodes[nodeId];
                    float distanceToRouteNode = routeNode.DistanceTo(currentLat, currentLon);
                    
                    if (distanceToRouteNode < minDistanceToRoute)
                    {
                        minDistanceToRoute = distanceToRouteNode;
                        nearestRouteNode = routeNode;
                    }
                    
                    // Si está dentro de la tolerancia de algún nodo de la ruta, está OK
                    if (distanceToRouteNode <= maxDeviationDistance)
                    {
                        isNearRouteNode = true;
                    }
                }
            }
            
            Debug.Log($"[GraphNavigationManager]    📏 Distancia mínima a ruta: {minDistanceToRoute:F1}m (nodo: {nearestRouteNode?.Name})");
            Debug.Log($"[GraphNavigationManager]    ✓ ¿Está cerca de la ruta?: {isNearRouteNode}");
            
            // Si está cerca de algún nodo de la ruta, NO es desviación
            if (isNearRouteNode)
            {
                Debug.Log($"[GraphNavigationManager]    ✅ Usuario cerca de la ruta (dentro de {maxDeviationDistance}m)");
                Debug.Log($"  - Nodo objetivo actual: {currentTargetNode?.Name}");
                Debug.Log($"  - Distancia al objetivo: {currentTargetNode?.DistanceTo(currentLat, currentLon):F1}m");
                Debug.Log($"  - Nodo más cercano en ruta: {nearestRouteNode?.Name}");
                Debug.Log($"  - Distancia al más cercano: {minDistanceToRoute:F1}m");
                Debug.Log($"  - ¿Está en la ruta?: true");
                Debug.Log($"[GraphNavigationManager]    ✅ Usuario en ruta correcta - No recalcular");
                return;
            }
            
            // Si NO está cerca de ningún nodo de la ruta, buscar el nodo más cercano en TODO el grafo
            Debug.Log($"[GraphNavigationManager]    ⚠️ Usuario lejos de la ruta (>{maxDeviationDistance}m)");
            Debug.Log($"[GraphNavigationManager]    🔎 Buscando nodo más cercano en todo el grafo...");
            
            GraphNode nearestNode = pathfindingService.FindNearestNode(currentLat, currentLon);
            
            if (nearestNode == null)
            {
                Debug.LogWarning($"[GraphNavigationManager]    ⚠️ No se encontró nodo cercano - Abortando verificación");
                return;
            }
            
            Debug.Log($"[GraphNavigationManager]    ✅ Nodo más cercano encontrado: {nearestNode.Name}");
            
            // Calcular distancia al nodo más cercano global
            float distanceToNearest = nearestNode.DistanceTo(currentLat, currentLon);
            
            // Calcular distancia al nodo objetivo actual
            float distanceToCurrentTarget = 0f;
            if (currentTargetNode != null)
            {
                distanceToCurrentTarget = currentTargetNode.DistanceTo(currentLat, currentLon);
            }
            
            Debug.Log($"  - Nodo objetivo actual: {currentTargetNode?.Name}");
            Debug.Log($"  - Distancia al objetivo: {distanceToCurrentTarget:F1}m");
            Debug.Log($"  - Nodo más cercano (global): {nearestNode.Name}");
            Debug.Log($"  - Distancia al más cercano: {distanceToNearest:F1}m");
            Debug.Log($"  - Distancia mínima a ruta: {minDistanceToRoute:F1}m");
            Debug.Log($"  - ¿Está en la ruta?: false");
            
            // CONDICIÓN DE RECALCULACIÓN:
            // El nodo más cercano global debe estar significativamente más cerca que cualquier nodo de la ruta
            // Y el usuario debe estar fuera de la tolerancia de la ruta
            bool shouldRecalculate = distanceToNearest < minDistanceToRoute - 10f; // 10m de margen
            
            Debug.Log($"[GraphNavigationManager]    📊 ¿Debe recalcular? {shouldRecalculate}");
            Debug.Log($"[GraphNavigationManager]       Razón: Nodo global ({distanceToNearest:F1}m) vs Ruta ({minDistanceToRoute:F1}m)");
            
            if (shouldRecalculate)
            {
                // Cooldown: no recalcular demasiado seguido
                if (Time.time - lastRecalculationTime < recalculationCooldown)
                {
                    float timeRemaining = recalculationCooldown - (Time.time - lastRecalculationTime);
                    Debug.Log($"[GraphNavigationManager] ⏸️ Cooldown activo, esperando {timeRemaining:F1}s");
                    return;
                }
                
                Debug.LogWarning($"[GraphNavigationManager] ⚠️ DESVIACIÓN DETECTADA!");
                Debug.LogWarning($"[GraphNavigationManager]    📍 Usuario se desvió de la ruta planeada");
                Debug.LogWarning($"[GraphNavigationManager]    🔄 Recalculando ruta desde {nearestNode.Name}...");
                
                RecalculateRouteFromCurrentPosition(nearestNode);
            }
            else
            {
                Debug.Log($"[GraphNavigationManager]    ⚠️ Usuario fuera de ruta pero no hay mejor alternativa");
                Debug.Log($"[GraphNavigationManager]    ➡️ Continuar con ruta actual");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GraphNavigationManager] ❌ EXCEPCIÓN en CheckRouteDeviation:");
            Debug.LogError($"[GraphNavigationManager]    Mensaje: {ex.Message}");
            Debug.LogError($"[GraphNavigationManager]    Stack: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Recalcula la ruta desde la posición actual (nodo más cercano)
    /// hacia el destino final
    /// </summary>
    private void RecalculateRouteFromCurrentPosition(GraphNode startNode)
    {
        if (finalDestinationNode == null)
        {
            Debug.LogError("[GraphNavigationManager] ❌ No hay destino final para recalcular");
            return;
        }
        
        Debug.Log($"[GraphNavigationManager] 🔄 RECALCULANDO RUTA:");
        Debug.Log($"  - Desde: {startNode.Name}");
        Debug.Log($"  - Hasta: {finalDestinationNode.Name}");
        
        // Calcular nueva ruta
        List<string> newPath = pathfindingService.FindShortestPath(startNode.id, finalDestinationNode.id);
        
        if (newPath == null || newPath.Count == 0)
        {
            Debug.LogError("[GraphNavigationManager] ❌ No se pudo recalcular la ruta");
            return;
        }
        
        // Actualizar la ruta
        currentPath = newPath;
        currentPathIndex = 0;
        lastRecalculationTime = Time.time;
        
        Debug.Log($"[GraphNavigationManager] ✅ Ruta recalculada: {currentPath.Count} nodos");
        Debug.Log($"[GraphNavigationManager] 📍 Nueva ruta:");
        
        for (int i = 0; i < currentPath.Count; i++)
        {
            string nodeId = currentPath[i];
            string nodeName = graphNodes.ContainsKey(nodeId) ? graphNodes[nodeId].Name : nodeId;
            Debug.Log($"  {i + 1}. {nodeName}");
        }
        
        // Actualizar el nodo objetivo
        UpdateCurrentTarget();
        
        // Notificar recalculación
        OnRouteRecalculated?.Invoke();
        
        Debug.Log($"[GraphNavigationManager] 🎯 Nuevo objetivo: {currentTargetNode.Name}");
    }
    
    /// <summary>
    /// Verifica si llegamos al nodo actual y actualiza al siguiente
    /// </summary>
    private void CheckNodeProgress()
    {
        if (currentTargetNode == null)
            return;
        
        double currentLat = LocationManager.Instance.CurrentLatitude;
        double currentLon = LocationManager.Instance.CurrentLongitude;
        
        float distanceToTarget = currentTargetNode.DistanceTo(currentLat, currentLon);
        
        Debug.Log($"[GraphNavigationManager] ?? Distancia al nodo actual ({currentTargetNode.Name}): {distanceToTarget:F1}m");
        
        // Si llegamos al nodo actual
        if (distanceToTarget <= nodeReachedDistance)
        {
            Debug.Log($"[GraphNavigationManager] ? Llegaste al nodo: {currentTargetNode.Name}");
            
            // Avanzar al siguiente nodo
            currentPathIndex++;
            
            // Verificar si llegamos al destino final
            if (currentPathIndex >= currentPath.Count)
            {
                Debug.Log($"[GraphNavigationManager] ?? DESTINO ALCANZADO: {finalDestinationNode.Name}");
                
                OnDestinationReached?.Invoke(finalDestinationNode);
                StopNavigation();
                return;
            }
            
            // Actualizar al siguiente nodo
            UpdateCurrentTarget();
        }
    }
    
    /// <summary>
    /// Actualiza el nodo objetivo actual
    /// </summary>
    private void UpdateCurrentTarget()
    {
        try
        {
            Debug.Log($"[GraphNavigationManager] ?? UpdateCurrentTarget() iniciado");
            Debug.Log($"[GraphNavigationManager]    currentPath: {(currentPath == null ? "null" : currentPath.Count.ToString())}");
            Debug.Log($"[GraphNavigationManager]    currentPathIndex: {currentPathIndex}");
            
            if (currentPath == null || currentPathIndex >= currentPath.Count)
            {
                Debug.LogWarning($"[GraphNavigationManager] ?? currentPath inválido o índice fuera de rango");
                return;
            }
            
            string targetNodeId = currentPath[currentPathIndex];
            Debug.Log($"[GraphNavigationManager]    targetNodeId: {targetNodeId}");
            
            if (!graphNodes.ContainsKey(targetNodeId))
            {
                Debug.LogError($"[GraphNavigationManager] ? Nodo en la ruta no existe: {targetNodeId}");
                return;
            }
            
            currentTargetNode = graphNodes[targetNodeId];
            Debug.Log($"[GraphNavigationManager]    currentTargetNode: {currentTargetNode.Name}");
            
            Debug.Log($"[GraphNavigationManager] ?? Nuevo nodo objetivo: {currentTargetNode.Name}");
            Debug.Log($"[GraphNavigationManager] ?? Progreso: {currentPathIndex + 1}/{currentPath.Count}");
            
            // Notificar cambio
            Debug.Log($"[GraphNavigationManager] ?? Emitiendo OnTargetNodeChanged...");
            OnTargetNodeChanged?.Invoke(currentTargetNode);
            Debug.Log($"[GraphNavigationManager] ? OnTargetNodeChanged emitido");
            
            Debug.Log($"[GraphNavigationManager] ?? Emitiendo OnPathProgressChanged...");
            OnPathProgressChanged?.Invoke(currentPathIndex + 1, currentPath.Count);
            Debug.Log($"[GraphNavigationManager] ? OnPathProgressChanged emitido");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GraphNavigationManager] ? EXCEPCIÓN en UpdateCurrentTarget:");
            Debug.LogError($"[GraphNavigationManager] Mensaje: {ex.Message}");
            Debug.LogError($"[GraphNavigationManager] Stack: {ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Busca un nodo por su BuildingData
    /// </summary>
    private GraphNode FindNodeByBuildingData(BuildingData buildingData)
    {
        foreach (var node in graphNodes.Values)
        {
            if (node.buildingData != null &&
                Mathf.Approximately((float)node.Latitude, (float)buildingData.latitude) &&
                Mathf.Approximately((float)node.Longitude, (float)buildingData.longitude))
            {
                return node;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Obtiene el nodo objetivo actual
    /// </summary>
    public GraphNode GetCurrentTargetNode()
    {
        return currentTargetNode;
    }
    
    /// <summary>
    /// Obtiene el nodo de destino final
    /// </summary>
    public GraphNode GetFinalDestinationNode()
    {
        return finalDestinationNode;
    }
    
    /// <summary>
    /// Verifica si hay una navegación activa
    /// </summary>
    public bool IsNavigating()
    {
        return isNavigating;
    }
    
    /// <summary>
    /// Obtiene el progreso actual en la ruta
    /// </summary>
    public (int current, int total) GetProgress()
    {
        if (currentPath == null)
            return (0, 0);
        
        return (currentPathIndex + 1, currentPath.Count);
    }
    
    /// <summary>
    /// Obtiene la ruta completa actual
    /// </summary>
    public List<GraphNode> GetCurrentPathNodes()
    {
        if (currentPath == null)
            return new List<GraphNode>();
        
        var pathNodes = new List<GraphNode>();
        
        foreach (var nodeId in currentPath)
        {
            if (graphNodes.ContainsKey(nodeId))
            {
                pathNodes.Add(graphNodes[nodeId]);
            }
        }
        
        return pathNodes;
    }
    
    /// <summary>
    /// Obtiene todos los nodos del grafo (para minimapa)
    /// </summary>
    public Dictionary<string, GraphNode> GetAllGraphNodes()
    {
        return graphNodes;
    }
    
    /// <summary>
    /// Obtiene todos los edges del grafo (para minimapa)
    /// </summary>
    public List<GraphEdge> GetAllGraphEdges()
    {
        return graphEdges;
    }
    
    /// <summary>
    /// Verifica si el grafo está inicializado
    /// </summary>
    public bool IsGraphReady()
    {
        return graphNodes != null && graphNodes.Count > 0 && 
               graphEdges != null && graphEdges.Count > 0;
    }
}
