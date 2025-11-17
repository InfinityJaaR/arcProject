using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador del minimapa 2D que muestra el grafo de navegación
/// Dibuja todos los nodos, edges, y resalta la ruta activa
/// </summary>
public class MiniMapController : MonoBehaviour
{
    [Header("Referencias UI")]
    [Tooltip("El RectTransform donde se dibujará el mapa")]
    public RectTransform mapContainer;
    
    [Tooltip("Prefab de punto para representar nodos")]
    public GameObject nodeDotPrefab;
    
    [Header("Configuración Visual")]
    [Tooltip("Color para nodos de edificios")]
    public Color buildingNodeColor = new Color(0.2f, 0.6f, 1f, 1f); // Azul
    
    [Tooltip("Color para nodos de inflexión")]
    public Color inflectionNodeColor = new Color(0.8f, 0.8f, 0.8f, 1f); // Gris claro
    
    [Tooltip("Color para conexiones del grafo")]
    public Color edgeColor = new Color(0.5f, 0.5f, 0.5f, 0.3f); // Gris translúcido
    
    [Tooltip("Color para la ruta activa")]
    public Color activePathColor = new Color(0f, 1f, 0f, 0.8f); // Verde
    
    [Tooltip("Color para el nodo actual objetivo")]
    public Color currentTargetColor = new Color(1f, 0.5f, 0f, 1f); // Naranja
    
    [Tooltip("Color para la posición del usuario")]
    public Color userPositionColor = new Color(1f, 0f, 0f, 1f); // Rojo
    
    [Tooltip("Tamaño de los puntos de nodos")]
    public float nodeSize = 8f;
    
    [Tooltip("Grosor de las líneas")]
    public float lineWidth = 2f;
    
    [Tooltip("Grosor de la ruta activa")]
    public float activePathWidth = 4f;
    
    [Header("Zoom y Pan")]
    [Tooltip("Margen alrededor del grafo (píxeles)")]
    public float mapPadding = 20f;
    
    [Tooltip("Actualizar posición del usuario cada N segundos")]
    public float updateInterval = 0.5f;
    
    [Header("Toggles")]
    [Tooltip("Mostrar/ocultar el minimapa")]
    public bool showMiniMap = true;
    
    [Tooltip("Mostrar nombres de nodos")]
    public bool showNodeLabels = false;
    
    [Tooltip("Mostrar SOLO la ruta activa (sin el grafo completo)")]
    public bool showOnlyActivePath = false; // ? NUEVO TOGGLE
    
    [Header("Configuración de Área")]
    [Tooltip("Usar bounds automáticos (puede incluir outliers) o bounds manuales")]
    public bool useAutoBounds = false;
    
    [Tooltip("Latitud mínima del área del campus (solo si useAutoBounds = false)")]
    public double manualMinLat = 13.7195;
    
    [Tooltip("Latitud máxima del área del campus (solo si useAutoBounds = false)")]
    public double manualMaxLat = 13.7215;
    
    [Tooltip("Longitud mínima del área del campus (solo si useAutoBounds = false)")]
    public double manualMinLon = -89.2015;
    
    [Tooltip("Longitud máxima del área del campus (solo si useAutoBounds = false)")]
    public double manualMaxLon = -89.1995;
    
    // Datos del grafo
    private Dictionary<string, GraphNode> graphNodes;
    private List<GraphEdge> graphEdges;
    private List<GraphNode> currentPath;
    private GraphNode currentTarget;
    
    // Objetos visuales
    private Dictionary<string, GameObject> nodeDots = new Dictionary<string, GameObject>();
    private List<GameObject> edgeLines = new List<GameObject>();
    private List<GameObject> pathLines = new List<GameObject>();
    private GameObject userDot;
    private GameObject targetHighlight;
    
    // Coordenadas del mapa
    private double minLat, maxLat, minLon, maxLon;
    private float mapWidth, mapHeight;
    
    void Start()
    {
        Debug.Log("[MiniMapController] ??? ============================================");
        Debug.Log("[MiniMapController] ??? INICIALIZANDO MINIMAPA");
        Debug.Log("[MiniMapController] ??? ============================================");
        
        if (mapContainer == null)
        {
            Debug.LogError("[MiniMapController] ? mapContainer NO ASIGNADO!");
            Debug.LogError("[MiniMapController] ? Solución: Ejecuta AR Navigation ? Setup MiniMap System");
            enabled = false;
            return;
        }
        
        Debug.Log($"[MiniMapController] ? mapContainer asignado: {mapContainer.name}");
        
        // Crear prefab de punto si no existe
        if (nodeDotPrefab == null)
        {
            Debug.LogWarning("[MiniMapController] ?? nodeDotPrefab no asignado, creando por defecto...");
            CreateDefaultNodePrefab();
        }
        else
        {
            Debug.Log($"[MiniMapController] ? nodeDotPrefab asignado: {nodeDotPrefab.name}");
        }
        
        // Verificar GraphNavigationManager
        if (GraphNavigationManager.Instance == null)
        {
            Debug.LogError("[MiniMapController] ? GraphNavigationManager.Instance es NULL!");
            Debug.LogError("[MiniMapController] ? Asegúrate de tener GraphNavigationManager en la escena");
            enabled = false;
            return;
        }
        
        Debug.Log("[MiniMapController] ? GraphNavigationManager encontrado");
        
        // Suscribirse a eventos de navegación
        GraphNavigationManager.Instance.OnTargetNodeChanged += OnTargetChanged;
        Debug.Log("[MiniMapController] ? Suscrito a eventos de GraphNavigationManager");
        
        // Esperar a que el grafo esté listo
        StartCoroutine(WaitForGraphAndInitialize());
    }
    
    void OnDestroy()
    {
        // Desuscribirse de eventos
        if (GraphNavigationManager.Instance != null)
        {
            GraphNavigationManager.Instance.OnTargetNodeChanged -= OnTargetChanged;
        }
    }
    
    /// <summary>
    /// Espera a que el grafo esté cargado y lo inicializa
    /// </summary>
    private System.Collections.IEnumerator WaitForGraphAndInitialize()
    {
        Debug.Log("[MiniMapController] ? Esperando a que el grafo esté listo...");
        
        // Esperar a que GraphNavigationManager tenga datos
        int waitCount = 0;
        while (GraphNavigationManager.Instance == null || !HasGraphData())
        {
            yield return new WaitForSeconds(0.5f);
            waitCount++;
            
            if (waitCount % 4 == 0) // Cada 2 segundos
            {
                Debug.Log($"[MiniMapController] ? Esperando grafo... ({waitCount * 0.5f}s)");
            }
            
            if (waitCount > 40) // 20 segundos timeout
            {
                Debug.LogError("[MiniMapController] ? TIMEOUT esperando datos del grafo (20 segundos)");
                Debug.LogError("[MiniMapController] ? Verifica que:");
                Debug.LogError("[MiniMapController]    1. FirebaseManager esté configurado");
                Debug.LogError("[MiniMapController]    2. Firestore tenga datos en graphEdges y buildingLocations");
                Debug.LogError("[MiniMapController]    3. Internet esté conectado");
                yield break;
            }
        }
        
        Debug.Log($"[MiniMapController] ? Grafo listo después de {waitCount * 0.5f}s");
        
        // Obtener datos del grafo via reflection (ya que no hay métodos públicos)
        LoadGraphData();
        
        if (graphNodes == null || graphNodes.Count == 0)
        {
            Debug.LogError("[MiniMapController] ? graphNodes es NULL o vacío después de LoadGraphData");
            Debug.LogError("[MiniMapController] ? El grafo no se pudo cargar desde GraphNavigationManager");
            yield break;
        }
        
        if (graphEdges == null || graphEdges.Count == 0)
        {
            Debug.LogError("[MiniMapController] ? graphEdges es NULL o vacío después de LoadGraphData");
            Debug.LogError("[MiniMapController] ? Verifica que tengas edges en Firestore");
            yield break;
        }
        
        Debug.Log($"[MiniMapController] ? Datos cargados: {graphNodes.Count} nodos, {graphEdges.Count} edges");
        
        // Calcular bounds del mapa
        CalculateMapBounds();
        
        // Dibujar el grafo
        DrawGraph();
        
        Debug.Log("[MiniMapController] ?? ============================================");
        Debug.Log("[MiniMapController] ?? MINIMAPA DIBUJADO EXITOSAMENTE");
        Debug.Log("[MiniMapController] ?? ============================================");
        
        // Iniciar actualización de posición del usuario
        StartCoroutine(UpdateUserPosition());
    }
    
    /// <summary>
    /// Verifica si hay datos del grafo disponibles
    /// </summary>
    private bool HasGraphData()
    {
        if (GraphNavigationManager.Instance == null)
            return false;
        
        // Usar el nuevo método público
        return GraphNavigationManager.Instance.IsGraphReady();
    }
    
    /// <summary>
    /// Carga los datos del grafo desde GraphNavigationManager
    /// </summary>
    private void LoadGraphData()
    {
        Debug.Log("[MiniMapController] ?? Cargando datos del grafo...");
        
        if (GraphNavigationManager.Instance == null)
        {
            Debug.LogError("[MiniMapController] ? GraphNavigationManager.Instance es NULL en LoadGraphData");
            return;
        }
        
        // Usar métodos públicos en vez de reflection
        graphNodes = GraphNavigationManager.Instance.GetAllGraphNodes();
        graphEdges = GraphNavigationManager.Instance.GetAllGraphEdges();
        
        if (graphNodes != null && graphEdges != null)
        {
            Debug.Log($"[MiniMapController] ? Datos cargados exitosamente:");
            Debug.Log($"[MiniMapController]    ?? {graphNodes.Count} nodos cargados");
            Debug.Log($"[MiniMapController]    ?? {graphEdges.Count} edges cargados");
            
            // Listar algunos nodos para verificación
            if (graphNodes.Count > 0)
            {
                Debug.Log("[MiniMapController] ?? Primeros nodos:");
                int count = 0;
                foreach (var node in graphNodes.Values)
                {
                    Debug.Log($"[MiniMapController]    - {node.Name} ({node.Latitude:F6}, {node.Longitude:F6})");
                    count++;
                    if (count >= 3) break; // Solo mostrar 3
                }
                
                // Diagnosticar outliers
                DiagnoseOutliers();
            }
        }
        else
        {
            Debug.LogError("[MiniMapController] ? Falló la carga de datos:");
            Debug.LogError($"[MiniMapController]    graphNodes = {(graphNodes == null ? "NULL" : graphNodes.Count + " nodos")}");
            Debug.LogError($"[MiniMapController]    graphEdges = {(graphEdges == null ? "NULL" : graphEdges.Count + " edges")}");
        }
    }
    
    /// <summary>
    /// Diagnostica nodos outliers que pueden estar causando problemas de visualización
    /// </summary>
    private void DiagnoseOutliers()
    {
        if (graphNodes == null || graphNodes.Count == 0)
            return;
        
        Debug.Log("[MiniMapController] ?? ============================================");
        Debug.Log("[MiniMapController] ?? DIAGNÓSTICO DE OUTLIERS");
        Debug.Log("[MiniMapController] ?? ============================================");
        
        // Calcular centro aproximado (mediana)
        List<double> lats = graphNodes.Values.Select(n => n.Latitude).OrderBy(x => x).ToList();
        List<double> lons = graphNodes.Values.Select(n => n.Longitude).OrderBy(x => x).ToList();
        
        double medianLat = lats[lats.Count / 2];
        double medianLon = lons[lons.Count / 2];
        
        Debug.Log($"[MiniMapController] ?? Centro del campus (mediana): ({medianLat:F6}, {medianLon:F6})");
        
        // Buscar nodos que estén muy lejos del centro
        List<(GraphNode node, double distance)> nodeDistances = new List<(GraphNode, double)>();
        
        foreach (var node in graphNodes.Values)
        {
            double latDiff = Math.Abs(node.Latitude - medianLat);
            double lonDiff = Math.Abs(node.Longitude - medianLon);
            double distance = Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff);
            
            nodeDistances.Add((node, distance));
        }
        
        // Ordenar por distancia
        nodeDistances.Sort((a, b) => b.distance.CompareTo(a.distance));
        
        // Mostrar los 5 nodos más lejanos
        Debug.Log("[MiniMapController] ?? NODOS MÁS ALEJADOS DEL CENTRO:");
        for (int i = 0; i < Mathf.Min(5, nodeDistances.Count); i++)
        {
            var (node, distance) = nodeDistances[i];
            Debug.Log($"[MiniMapController]    {i+1}. {node.Name}");
            Debug.Log($"[MiniMapController]       ?? ({node.Latitude:F6}, {node.Longitude:F6})");
            Debug.Log($"[MiniMapController]       ?? Distancia del centro: {distance:F6}°");
            
            // Calcular distancia aproximada en metros
            double metersPerDegree = 111000; // Aproximado
            double distanceMeters = distance * metersPerDegree;
            Debug.Log($"[MiniMapController]       ?? ~{distanceMeters:F0} metros del centro");
            
            if (distanceMeters > 500) // Más de 500m es sospechoso
            {
                Debug.LogWarning($"[MiniMapController]       ?? POSIBLE OUTLIER (>500m del centro)");
            }
        }
        
        Debug.Log("[MiniMapController] ?? ============================================");
    }
    
    /// <summary>
    /// Calcula los límites del mapa basándose en las coordenadas de los nodos
    /// </summary>
    private void CalculateMapBounds()
    {
        if (graphNodes == null || graphNodes.Count == 0)
            return;
        
        if (!useAutoBounds)
        {
            // Usar bounds manuales (recomendado para evitar outliers)
            minLat = manualMinLat;
            maxLat = manualMaxLat;
            minLon = manualMinLon;
            maxLon = manualMaxLon;
            
            Debug.Log($"[MiniMapController] ?? Usando BOUNDS MANUALES:");
            Debug.Log($"[MiniMapController]    Lat[{minLat:F6}, {maxLat:F6}]");
            Debug.Log($"[MiniMapController]    Lon[{minLon:F6}, {maxLon:F6}]");
        }
        else
        {
            // Calcular bounds automáticamente, pero filtrar outliers
            List<double> latitudes = new List<double>();
            List<double> longitudes = new List<double>();
            
            foreach (var node in graphNodes.Values)
            {
                latitudes.Add(node.Latitude);
                longitudes.Add(node.Longitude);
            }
            
            // Ordenar para encontrar percentiles
            latitudes.Sort();
            longitudes.Sort();
            
            // Usar percentil 5 y 95 para eliminar outliers extremos
            int percentile5Index = Mathf.FloorToInt(latitudes.Count * 0.05f);
            int percentile95Index = Mathf.FloorToInt(latitudes.Count * 0.95f);
            
            minLat = latitudes[percentile5Index];
            maxLat = latitudes[percentile95Index];
            minLon = longitudes[percentile5Index];
            maxLon = longitudes[percentile95Index];
            
            // Agregar un pequeño margen (2%)
            double latMargin = (maxLat - minLat) * 0.02;
            double lonMargin = (maxLon - minLon) * 0.02;
            
            minLat -= latMargin;
            maxLat += latMargin;
            minLon -= lonMargin;
            maxLon += lonMargin;
            
            Debug.Log($"[MiniMapController] ?? Bounds AUTO (filtrados):");
            Debug.Log($"[MiniMapController]    Lat[{minLat:F6}, {maxLat:F6}]");
            Debug.Log($"[MiniMapController]    Lon[{minLon:F6}, {maxLon:F6}]");
            Debug.Log($"[MiniMapController]    Filtrados {percentile5Index} nodos del inicio y {latitudes.Count - percentile95Index} del final");
        }
        
        // Calcular dimensiones del contenedor
        Rect containerRect = mapContainer.rect;
        mapWidth = containerRect.width - (mapPadding * 2);
        mapHeight = containerRect.height - (mapPadding * 2);
        
        Debug.Log($"[MiniMapController] ?? Container: {containerRect.width}x{containerRect.height}");
        Debug.Log($"[MiniMapController] ?? Map area (con padding): {mapWidth}x{mapHeight}");
        
        // Calcular área geográfica en grados
        double latRange = maxLat - minLat;
        double lonRange = maxLon - minLon;
        Debug.Log($"[MiniMapController] ?? Área geográfica: {latRange:F6}° lat x {lonRange:F6}° lon");
        
        // Verificar si el área es razonable (un campus típico es ~0.002 grados)
        if (latRange > 0.1 || lonRange > 0.1)
        {
            Debug.LogWarning($"[MiniMapController] ?? ÁREA MUY GRANDE detectada!");
            Debug.LogWarning($"[MiniMapController] ?? Esto indica que hay nodos outliers en tus datos");
            Debug.LogWarning($"[MiniMapController] ?? Considera activar useAutoBounds=false y ajustar bounds manualmente");
        }
    }
    
    /// <summary>
    /// Convierte coordenadas GPS a posición 2D en el minimapa
    /// </summary>
    private Vector2 GPSToMapPosition(double latitude, double longitude)
    {
        // Normalizar coordenadas GPS a rango [0, 1]
        double latRange = maxLat - minLat;
        double lonRange = maxLon - minLon;
        
        // Evitar división por cero
        if (latRange < 0.000001) latRange = 0.000001;
        if (lonRange < 0.000001) lonRange = 0.000001;
        
        // Normalizar (0 = min, 1 = max)
        double normalizedLat = (latitude - minLat) / latRange;
        double normalizedLon = (longitude - minLon) / lonRange;
        
        // Ajustar aspect ratio para mantener proporciones geográficas
        // Los grados de longitud son más cortos en latitudes altas
        double avgLat = (minLat + maxLat) / 2.0;
        double cosLat = Math.Cos(avgLat * Math.PI / 180.0);
        
        // Calcular qué dimensión es el factor limitante
        double geoAspect = (lonRange * cosLat) / latRange;
        double containerAspect = mapWidth / mapHeight;
        
        float finalWidth = mapWidth;
        float finalHeight = mapHeight;
        
        // Ajustar dimensiones para mantener aspect ratio geográfico
        if (geoAspect > containerAspect)
        {
            // Limitar por ancho
            finalHeight = mapWidth / (float)geoAspect;
        }
        else
        {
            // Limitar por alto
            finalWidth = mapHeight * (float)geoAspect;
        }
        
        // Convertir coordenadas normalizadas a píxeles
        // IMPORTANTE: Y crece hacia arriba en coordenadas geográficas
        float x = ((float)normalizedLon - 0.5f) * finalWidth;
        float y = ((float)normalizedLat - 0.5f) * finalHeight;
        
        return new Vector2(x, y);
    }
    
    /// <summary>
    /// Dibuja todo el grafo (nodos y edges)
    /// </summary>
    private void DrawGraph()
    {
        Debug.Log("[MiniMapController] ?? ============================================");
        Debug.Log("[MiniMapController] ?? DIBUJANDO GRAFO");
        Debug.Log("[MiniMapController] ?? ============================================");
        
        if (graphNodes == null || graphEdges == null)
        {
            Debug.LogError("[MiniMapController] ? No se puede dibujar: graphNodes o graphEdges es NULL");
            return;
        }
        
        if (graphNodes.Count == 0)
        {
            Debug.LogError("[MiniMapController] ? No hay nodos para dibujar");
            return;
        }
        
        Debug.Log($"[MiniMapController] ?? Dibujando {graphNodes.Count} nodos y {graphEdges.Count} edges");
        
        // Limpiar objetos anteriores
        Debug.Log("[MiniMapController] ?? Limpiando mapa anterior...");
        ClearMap();
        
        // Solo dibujar el grafo completo si NO está en modo "solo ruta activa"
        if (!showOnlyActivePath)
        {
            Debug.Log("[MiniMapController] ?? Modo: GRAFO COMPLETO");
            
            // Dibujar todas las conexiones (edges)
            DrawEdges();
            
            // Dibujar todos los nodos
            DrawNodes();
        }
        else
        {
            Debug.Log("[MiniMapController] ?? Modo: SOLO RUTA ACTIVA");
            Debug.Log("[MiniMapController] ?? Nodos y edges se dibujarán cuando haya navegación activa");
        }
        
        // Crear indicador de usuario
        Debug.Log("[MiniMapController] ?? Creando indicador de usuario...");
        CreateUserPositionIndicator();
        
        // Crear indicador de objetivo actual
        Debug.Log("[MiniMapController] ?? Creando indicador de objetivo...");
        CreateTargetHighlight();
        
        Debug.Log("[MiniMapController] ? Grafo dibujado exitosamente");
        Debug.Log("[MiniMapController] ?? ============================================");
    }
    
    /// <summary>
    /// Dibuja todas las conexiones (edges) del grafo
    /// </summary>
    private void DrawEdges()
    {
        foreach (var edge in graphEdges)
        {
            if (!graphNodes.ContainsKey(edge.source) || !graphNodes.ContainsKey(edge.target))
                continue;
            
            GraphNode sourceNode = graphNodes[edge.source];
            GraphNode targetNode = graphNodes[edge.target];
            
            Vector2 startPos = GPSToMapPosition(sourceNode.Latitude, sourceNode.Longitude);
            Vector2 endPos = GPSToMapPosition(targetNode.Latitude, targetNode.Longitude);
            
            GameObject lineObj = CreateLine(startPos, endPos, edgeColor, lineWidth);
            lineObj.name = $"Edge_{edge.source}_{edge.target}";
            lineObj.transform.SetParent(mapContainer, false);
            lineObj.transform.SetAsFirstSibling(); // Poner las líneas al fondo
            
            edgeLines.Add(lineObj);
        }
        
        Debug.Log($"[MiniMapController] ?? Dibujadas {edgeLines.Count} conexiones");
    }
    
    /// <summary>
    /// Dibuja todos los nodos del grafo
    /// </summary>
    private void DrawNodes()
    {
        Debug.Log("[MiniMapController] ?? Dibujando nodos:");
        int nodesInBounds = 0;
        int nodesOutOfBounds = 0;
        
        foreach (var kvp in graphNodes)
        {
            string nodeId = kvp.Key;
            GraphNode node = kvp.Value;
            
            // Verificar si el nodo está dentro de los bounds
            bool inBounds = node.Latitude >= minLat && node.Latitude <= maxLat &&
                           node.Longitude >= minLon && node.Longitude <= maxLon;
            
            Vector2 pos = GPSToMapPosition(node.Latitude, node.Longitude);
            
            if (!inBounds)
            {
                nodesOutOfBounds++;
                if (nodesOutOfBounds <= 3) // Solo mostrar los primeros 3
                {
                    Debug.LogWarning($"[MiniMapController]    ?? Nodo fuera de bounds: {node.Name}");
                    Debug.LogWarning($"[MiniMapController]       GPS: ({node.Latitude:F6}, {node.Longitude:F6})");
                    Debug.LogWarning($"[MiniMapController]       Screen: ({pos.x:F1}, {pos.y:F1})");
                }
                // Continuar dibujándolo de todas formas, pero con advertencia
            }
            else
            {
                nodesInBounds++;
            }
            
            GameObject dot = Instantiate(nodeDotPrefab, mapContainer);
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            dotRect.anchoredPosition = pos;
            dotRect.sizeDelta = new Vector2(nodeSize, nodeSize);
            
            // Color según tipo de nodo
            Image dotImage = dot.GetComponent<Image>();
            if (dotImage != null)
            {
                dotImage.color = node.IsBuilding ? buildingNodeColor : inflectionNodeColor;
            }
            
            dot.name = $"Node_{node.Name}";
            nodeDots[nodeId] = dot;
        }
        
        Debug.Log($"[MiniMapController] ?? Dibujados {nodeDots.Count} nodos:");
        Debug.Log($"[MiniMapController]    ? {nodesInBounds} dentro de bounds");
        Debug.Log($"[MiniMapController]    ?? {nodesOutOfBounds} fuera de bounds");
        
        if (nodesOutOfBounds > 0)
        {
            Debug.LogWarning($"[MiniMapController] ?? HAY {nodesOutOfBounds} NODOS FUERA DE BOUNDS!");
            Debug.LogWarning($"[MiniMapController] ?? Estos nodos no se visualizarán correctamente");
            Debug.LogWarning($"[MiniMapController] ?? Considera:");
            Debug.LogWarning($"[MiniMapController]    1. Corregir coordenadas en Firestore");
            Debug.LogWarning($"[MiniMapController]    2. Ajustar bounds manuales en Inspector");
        }
    }
    
    /// <summary>
    /// Crea una línea entre dos puntos
    /// </summary>
    private GameObject CreateLine(Vector2 start, Vector2 end, Color color, float width)
    {
        GameObject lineObj = new GameObject("Line");
        RectTransform lineRect = lineObj.AddComponent<RectTransform>();
        Image lineImage = lineObj.AddComponent<Image>();
        lineImage.color = color;
        
        // Calcular posición, longitud y ángulo
        Vector2 direction = end - start;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Configurar rectTransform
        lineRect.pivot = new Vector2(0, 0.5f);
        lineRect.anchoredPosition = start;
        lineRect.sizeDelta = new Vector2(distance, width);
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
        
        return lineObj;
    }
    
    /// <summary>
    /// Crea el indicador de posición del usuario
    /// </summary>
    private void CreateUserPositionIndicator()
    {
        userDot = Instantiate(nodeDotPrefab, mapContainer);
        RectTransform dotRect = userDot.GetComponent<RectTransform>();
        dotRect.sizeDelta = new Vector2(nodeSize * 1.5f, nodeSize * 1.5f);
        
        Image dotImage = userDot.GetComponent<Image>();
        if (dotImage != null)
        {
            dotImage.color = userPositionColor;
        }
        
        userDot.name = "UserPosition";
        userDot.SetActive(false); // Se mostrará cuando haya posición GPS
    }
    
    /// <summary>
    /// Crea el indicador del nodo objetivo actual
    /// </summary>
    private void CreateTargetHighlight()
    {
        targetHighlight = new GameObject("TargetHighlight");
        targetHighlight.transform.SetParent(mapContainer, false);
        
        RectTransform rect = targetHighlight.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(nodeSize * 2f, nodeSize * 2f);
        
        Image ring = targetHighlight.AddComponent<Image>();
        ring.color = currentTargetColor;
        
        // Hacer que sea un anillo (requiere un sprite apropiado o usar Outline)
        Outline outline = targetHighlight.AddComponent<Outline>();
        outline.effectColor = currentTargetColor;
        outline.effectDistance = new Vector2(2, 2);
        
        targetHighlight.SetActive(false);
    }
    
    /// <summary>
    /// Actualiza la posición del usuario en el mapa
    /// </summary>
    private System.Collections.IEnumerator UpdateUserPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            
            if (!showMiniMap || userDot == null)
                continue;
            
            if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
            {
                double lat = LocationManager.Instance.CurrentLatitude;
                double lon = LocationManager.Instance.CurrentLongitude;
                
                Vector2 pos = GPSToMapPosition(lat, lon);
                userDot.GetComponent<RectTransform>().anchoredPosition = pos;
                
                if (!userDot.activeSelf)
                    userDot.SetActive(true);
            }
            else
            {
                if (userDot.activeSelf)
                    userDot.SetActive(false);
            }
            
            // Actualizar ruta activa si está navegando
            UpdateActivePath();
        }
    }
    
    /// <summary>
    /// Actualiza la visualización de la ruta activa
    /// </summary>
    private void UpdateActivePath()
    {
        if (GraphNavigationManager.Instance == null || !GraphNavigationManager.Instance.IsNavigating())
        {
            ClearActivePath();
            return;
        }
        
        List<GraphNode> path = GraphNavigationManager.Instance.GetCurrentPathNodes();
        
        if (path == null || path.Count == 0)
        {
            ClearActivePath();
            return;
        }
        
        // Si la ruta cambió, redibujarla
        if (currentPath != path)
        {
            currentPath = path;
            DrawActivePath();
        }
    }
    
    /// <summary>
    /// Dibuja la ruta activa de navegación
    /// </summary>
    private void DrawActivePath()
    {
        ClearActivePath();
        
        if (currentPath == null || currentPath.Count < 2)
            return;
        
        // Si está en modo "solo ruta activa", también dibujar los nodos de la ruta
        if (showOnlyActivePath)
        {
            // Dibujar solo los nodos que están en la ruta activa
            foreach (var node in currentPath)
            {
                if (!nodeDots.ContainsKey(node.id))
                {
                    Vector2 pos = GPSToMapPosition(node.Latitude, node.Longitude);
                    
                    GameObject dot = Instantiate(nodeDotPrefab, mapContainer);
                    RectTransform dotRect = dot.GetComponent<RectTransform>();
                    dotRect.anchoredPosition = pos;
                    dotRect.sizeDelta = new Vector2(nodeSize, nodeSize);
                    
                    // Color según tipo de nodo
                    Image dotImage = dot.GetComponent<Image>();
                    if (dotImage != null)
                    {
                        dotImage.color = node.IsBuilding ? buildingNodeColor : inflectionNodeColor;
                    }
                    
                    dot.name = $"Node_{node.Name}";
                    nodeDots[node.id] = dot;
                }
            }
        }
        
        // Dibujar líneas entre nodos consecutivos de la ruta
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            GraphNode nodeA = currentPath[i];
            GraphNode nodeB = currentPath[i + 1];
            
            Vector2 startPos = GPSToMapPosition(nodeA.Latitude, nodeA.Longitude);
            Vector2 endPos = GPSToMapPosition(nodeB.Latitude, nodeB.Longitude);
            
            GameObject lineObj = CreateLine(startPos, endPos, activePathColor, activePathWidth);
            lineObj.name = $"PathLine_{i}";
            lineObj.transform.SetParent(mapContainer, false);
            
            pathLines.Add(lineObj);
        }
        
        Debug.Log($"[MiniMapController] ??? Ruta activa dibujada ({pathLines.Count} segmentos)");
    }
    
    /// <summary>
    /// Limpia la visualización de la ruta activa
    /// </summary>
    private void ClearActivePath()
    {
        foreach (var line in pathLines)
        {
            if (line != null)
                Destroy(line);
        }
        pathLines.Clear();
    }
    
    /// <summary>
    /// Callback cuando cambia el nodo objetivo
    /// </summary>
    private void OnTargetChanged(GraphNode newTarget)
    {
        currentTarget = newTarget;
        
        if (targetHighlight != null && newTarget != null)
        {
            Vector2 pos = GPSToMapPosition(newTarget.Latitude, newTarget.Longitude);
            targetHighlight.GetComponent<RectTransform>().anchoredPosition = pos;
            targetHighlight.SetActive(true);
            
            Debug.Log($"[MiniMapController] ?? Objetivo actualizado: {newTarget.Name}");
        }
    }
    
    /// <summary>
    /// Limpia todos los objetos visuales del mapa
    /// </summary>
    private void ClearMap()
    {
        foreach (var dot in nodeDots.Values)
        {
            if (dot != null)
                Destroy(dot);
        }
        nodeDots.Clear();
        
        foreach (var line in edgeLines)
        {
            if (line != null)
                Destroy(line);
        }
        edgeLines.Clear();
        
        ClearActivePath();
        
        if (userDot != null)
            Destroy(userDot);
        
        if (targetHighlight != null)
            Destroy(targetHighlight);
    }
    
    /// <summary>
    /// Crea un prefab básico de punto si no existe
    /// </summary>
    private void CreateDefaultNodePrefab()
    {
        nodeDotPrefab = new GameObject("NodeDot");
        nodeDotPrefab.AddComponent<RectTransform>();
        
        Image image = nodeDotPrefab.AddComponent<Image>();
        image.color = Color.white;
        
        // Hacer el sprite circular (requiere un sprite, pero funcionará como cuadrado por ahora)
        Debug.Log("[MiniMapController] ?? Usando prefab de punto por defecto (cuadrado)");
    }
    
    /// <summary>
    /// Toggle para mostrar/ocultar el minimapa
    /// </summary>
    public void ToggleMiniMap()
    {
        showMiniMap = !showMiniMap;
        gameObject.SetActive(showMiniMap);
        
        Debug.Log($"[MiniMapController] ??? Minimapa {(showMiniMap ? "activado" : "desactivado")}");
    }
    
    /// <summary>
    /// Fuerza una actualización completa del mapa
    /// </summary>
    public void RefreshMap()
    {
        Debug.Log("[MiniMapController] ?? Refrescando mapa...");
        LoadGraphData();
        CalculateMapBounds();
        DrawGraph();
    }
}
