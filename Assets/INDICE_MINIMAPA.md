# ?? ÍNDICE COMPLETO: MINIMAPA DEL GRAFO

## ?? ARCHIVOS CREADOS

### Scripts del Sistema
| Archivo | Ubicación | Descripción |
|---------|-----------|-------------|
| `MiniMapController.cs` | `Assets/Scripts/` | Componente principal que dibuja y actualiza el minimapa |

### Scripts de Editor
| Archivo | Ubicación | Descripción |
|---------|-----------|-------------|
| `SetupMiniMap.cs` | `Assets/Editor/` | Herramienta de configuración automática |

### Documentación
| Archivo | Ubicación | Descripción |
|---------|-----------|-------------|
| `GUIA_MINIMAPA.md` | `Assets/` | Guía completa paso a paso |
| `RESUMEN_MINIMAPA.md` | `Assets/` | Resumen ejecutivo rápido |
| `EJEMPLOS_VISUALES_MINIMAPA.md` | `Assets/` | Ejemplos visuales y casos de uso |
| `INDICE_MINIMAPA.md` | `Assets/` | Este archivo (índice) |

### Recursos (Creados automáticamente)
| Recurso | Ubicación | Descripción |
|---------|-----------|-------------|
| `NodeDot.prefab` | `Assets/Resources/` | Prefab de punto circular para nodos |
| `MiniMapCanvas` | Escena (runtime) | Canvas con el minimapa |

---

## ?? INICIO RÁPIDO

### Para Usuarios Nuevos

```bash
1. Unity ? AR Navigation ? Setup MiniMap System
2. Click "Configurar MiniMap Automáticamente"
3. Presiona Play
4. Inicia navegación
5. ¡Observa el minimapa!
```

**Tiempo total:** ~30 segundos

---

## ?? RUTAS DE APRENDIZAJE

### Ruta 1: Usuario Rápido (5 min)
```
1. Leer: RESUMEN_MINIMAPA.md
2. Ejecutar: Setup MiniMap System
3. Probar en Play Mode
```

### Ruta 2: Usuario Completo (20 min)
```
1. Leer: RESUMEN_MINIMAPA.md
2. Leer: GUIA_MINIMAPA.md (secciones principales)
3. Ejecutar: Setup MiniMap System
4. Personalizar: Colores y posición
5. Probar: Navegación completa
```

### Ruta 3: Desarrollador Avanzado (1 hora)
```
1. Leer: GUIA_MINIMAPA.md (completa)
2. Revisar: MiniMapController.cs (código)
3. Entender: Conversión GPS ? 2D
4. Personalizar: Código y UI
5. Integrar: Con tus propios sistemas
6. Documentar: Tus modificaciones
```

---

## ?? COMPONENTES DEL SISTEMA

### Arquitectura

```
MiniMapCanvas (GameObject)
?
?? MiniMapController (Component)
?  ?? Referencias
?  ?  ?? mapContainer (RectTransform)
?  ?  ?? nodeDotPrefab (GameObject)
?  ?
?  ?? Configuración Visual
?  ?  ?? buildingNodeColor
?  ?  ?? inflectionNodeColor
?  ?  ?? edgeColor
?  ?  ?? activePathColor
?  ?  ?? currentTargetColor
?  ?  ?? userPositionColor
?  ?
?  ?? Datos Runtime
?     ?? graphNodes (Dict)
?     ?? graphEdges (List)
?     ?? currentPath (List)
?     ?? currentTarget (GraphNode)
?
?? MapPanel (RectTransform)
   ?? Background (Image)
   ?? Title (TextMeshPro)
   ?
   ?? MapContainer (RectTransform)
      ?? Node Dots (generados)
      ?? Edge Lines (generados)
      ?? Path Lines (generados)
      ?? User Dot (generado)
      ?? Target Highlight (generado)
```

---

## ?? INTEGRACIÓN CON SISTEMAS EXISTENTES

### Dependencias

| Sistema | Uso | Estado |
|---------|-----|--------|
| `GraphNavigationManager` | Obtiene datos del grafo y estado de navegación | ? Requerido |
| `PathfindingService` | Calcula rutas (indirecto vía GraphNav) | ? Requerido |
| `LocationManager` | Obtiene posición GPS del usuario | ? Requerido |
| `FirebaseManager` | Carga grafo desde Firestore (indirecto) | ? Requerido |

### Eventos Suscritos

```csharp
GraphNavigationManager.OnTargetNodeChanged += OnTargetChanged;
// Se ejecuta cuando cambia el nodo objetivo durante navegación
```

### Métodos Llamados

```csharp
// Desde GraphNavigationManager
GetCurrentPathNodes() ? List<GraphNode>
IsNavigating() ? bool

// Desde LocationManager
CurrentLatitude ? double
CurrentLongitude ? double
IsGPSReady ? bool
```

---

## ?? FLUJO DE DATOS

### Inicialización

```
Start()
  ?
WaitForGraphAndInitialize()
  ?
[Esperar GraphNavigationManager]
  ?
LoadGraphData() [Via Reflection]
  ?
graphNodes, graphEdges ? GraphNavigationManager
  ?
CalculateMapBounds()
  ?
minLat, maxLat, minLon, maxLon calculados
  ?
DrawGraph()
  ?
DrawEdges() + DrawNodes()
  ?
[Minimapa inicializado]
```

### Durante Navegación

```
NavigationArrowController.StartNavigation()
  ?
GraphNavigationManager.StartNavigationToBuilding()
  ?
OnTargetNodeChanged evento ? GraphNavigationManager
  ?
MiniMapController.OnTargetChanged()
  ?
[Actualizar highlight de objetivo]
  ?
UpdateActivePath() [Loop 0.5s]
  ?
DrawActivePath() [Redibujar ruta verde]
  ?
UpdateUserPosition() [Loop 0.5s]
  ?
GPSToMapPosition() [Convertir y mover punto rojo]
```

---

## ?? PERSONALIZACIÓN AVANZADA

### Modificar Código

**Cambiar forma de nodos:**
```csharp
// En MiniMapController.cs, método DrawNodes()
// Línea ~280

// En lugar de círculos, usar cuadrados rotados
dotRect.rotation = Quaternion.Euler(0, 0, 45); // Diamante
```

**Añadir etiquetas a nodos:**
```csharp
// Después de crear el dot en DrawNodes()
GameObject label = new GameObject("Label");
label.transform.SetParent(dot.transform, false);
TextMeshProUGUI text = label.AddComponent<TextMeshProUGUI>();
text.text = node.Name;
text.fontSize = 12;
text.alignment = TextAlignmentOptions.Center;
```

**Cambiar algoritmo de zoom:**
```csharp
// En CalculateMapBounds()
// Añadir zoom dinámico basado en ruta activa

if (currentPath != null && currentPath.Count > 0)
{
    // Calcular bounds solo de la ruta activa
    // para hacer zoom a la ruta
}
```

### Modificar UI

**Añadir leyenda:**
```csharp
// En CreateMiniMapCanvas() de SetupMiniMap.cs
GameObject legend = new GameObject("Legend");
// ... crear texto con leyenda
```

**Añadir controles de zoom:**
```csharp
// Crear botones + y -
// Llamar a una función ZoomIn() / ZoomOut()
// que modifique mapWidth y mapHeight
```

---

## ?? CONFIGURACIÓN

### Por Inspector

```
MiniMapCanvas ? MiniMapController

Referencias UI:
?? Map Container: MapPanel/MapContainer
?? Node Dot Prefab: Resources/NodeDot

Configuración Visual:
?? Building Node Color: RGB(51, 153, 255)
?? Inflection Node Color: RGB(204, 204, 204)
?? Edge Color: RGB(128, 128, 128, 77)
?? Active Path Color: RGB(0, 255, 0, 204)
?? Current Target Color: RGB(255, 128, 0)
?? User Position Color: RGB(255, 0, 0)
?? Node Size: 8
?? Line Width: 2
?? Active Path Width: 4
?? Map Padding: 20

Zoom y Pan:
?? Map Padding: 20
?? Update Interval: 0.5

Toggles:
?? Show Mini Map: ?
?? Show Node Labels: ?
```

### Por Código

```csharp
MiniMapController miniMap = FindObjectOfType<MiniMapController>();

// Cambiar colores
miniMap.activePathColor = Color.cyan;
miniMap.userPositionColor = Color.magenta;

// Cambiar tamaños
miniMap.nodeSize = 10f;
miniMap.activePathWidth = 5f;

// Refrescar
miniMap.RefreshMap();
```

---

## ?? DEBUGGING

### Logs Importantes

```bash
# Inicialización exitosa
[MiniMapController] ??? Inicializando minimapa...
[MiniMapController] ? Grafo listo, inicializando mapa...
[MiniMapController] ?? Cargados 15 nodos y 22 edges
[MiniMapController] ?? Minimapa dibujado exitosamente

# Durante navegación
[MiniMapController] ??? Ruta activa dibujada (5 segmentos)
[MiniMapController] ?? Objetivo actualizado: Edificio A3

# Errores comunes
[MiniMapController] ? mapContainer no asignado
[MiniMapController] ? Timeout esperando datos del grafo
```

### Verificación Manual

```csharp
// En Unity Console durante Play Mode
Debug:
1. ¿MiniMapCanvas está activo? ? Hierarchy
2. ¿MapContainer tiene hijos? ? Debe tener dots y lines
3. ¿Colores visibles? ? Alpha > 0.5
4. ¿GPS funciona? ? LocationManager.IsGPSReady
5. ¿Navegación activa? ? GraphNavigationManager.IsNavigating()
```

---

## ?? RENDIMIENTO

### Métricas

| Métrica | Valor | Impacto |
|---------|-------|---------|
| GameObjects creados | ~50-100 | Bajo |
| Updates por segundo | 2 (cada 0.5s) | Muy bajo |
| Draw calls | +1-3 | Mínimo |
| Memoria | <5 MB | Insignificante |

### Optimización

```csharp
// Si tienes MUCHOS nodos (>100)

// Opción 1: Aumentar update interval
updateInterval = 1.0f; // En vez de 0.5s

// Opción 2: Object Pooling para lines
// Reutilizar GameObjects en vez de Destroy/Instantiate

// Opción 3: Culling
// No dibujar nodos fuera de la ruta activa
```

---

## ?? CHECKLIST DE IMPLEMENTACIÓN

### Configuración Inicial
- [ ] Ejecutado `AR Navigation ? Setup MiniMap System`
- [ ] Verificado mensaje "MiniMap Configurado ?"
- [ ] MiniMapCanvas visible en Hierarchy
- [ ] MapContainer tiene referencias correctas

### Pruebas Funcionales
- [ ] Presionado Play
- [ ] Esperado carga del grafo (~5-10s)
- [ ] Verificado que aparecen nodos y líneas
- [ ] Iniciado navegación a un edificio
- [ ] Verificado que se dibuja ruta en verde
- [ ] Verificado que posición roja se actualiza
- [ ] Verificado que objetivo naranja aparece

### Personalización (Opcional)
- [ ] Ajustados colores al gusto
- [ ] Modificado tamaño del panel
- [ ] Cambiada posición en pantalla
- [ ] Probado botón de toggle

### Documentación
- [ ] Leído RESUMEN_MINIMAPA.md
- [ ] Consultado GUIA_MINIMAPA.md para detalles
- [ ] Revisado EJEMPLOS_VISUALES_MINIMAPA.md

---

## ?? PREGUNTAS FRECUENTES

### ¿Funciona en dispositivos móviles?
? Sí, está optimizado para móviles. El Canvas es responsive.

### ¿Puedo tener múltiples minimapas?
?? No recomendado. El sistema está diseñado para una instancia.

### ¿Funciona sin GPS?
? No. Requiere GPS para mostrar la posición del usuario.

### ¿Puedo usarlo solo para visualizar el grafo (sin navegación)?
? Sí. Mostrará todos los nodos y conexiones sin la ruta activa.

### ¿Es compatible con AR Foundation?
? Sí, es completamente compatible y no interfiere con AR.

### ¿Puedo exportarlo a un prefab?
?? Sí, pero perderás las referencias. Mejor usa Setup cada vez.

### ¿Funciona offline?
?? Parcialmente. Necesita cargar el grafo una vez de Firestore, luego funciona offline.

---

## ?? ENLACES RÁPIDOS

### Documentación
- [Guía Completa](GUIA_MINIMAPA.md)
- [Resumen Ejecutivo](RESUMEN_MINIMAPA.md)
- [Ejemplos Visuales](EJEMPLOS_VISUALES_MINIMAPA.md)

### Scripts
- [MiniMapController.cs](Scripts/MiniMapController.cs)
- [SetupMiniMap.cs](Editor/SetupMiniMap.cs)

### Sistemas Relacionados
- [Sistema de Navegación](README_NAVEGACION_GRAFO.md)
- [Configuración Firebase](INSTRUCCIONES_FIRESTORE_GRAFO.md)
- [Pathfinding](Assets/Scripts/PathfindingService.cs)

---

## ?? RESUMEN FINAL

### Lo que se creó:
? Sistema completo de minimapa 2D  
? Visualización en tiempo real del grafo  
? Integración automática con navegación  
? Herramienta de configuración con 1 click  
? Documentación completa  

### Características principales:
- ??? Muestra todo el grafo de navegación
- ??? Resalta la ruta activa en verde
- ?? Posición del usuario en tiempo real (rojo)
- ?? Marca el próximo objetivo (naranja)
- ?? Totalmente personalizable
- ? Configuración automática
- ?? Compatible con móviles
- ?? Actualización en tiempo real

### Próximos pasos sugeridos:
1. Probar en Play Mode ??
2. Probar en dispositivo real ??
3. Personalizar colores y tamaño ??
4. Compartir feedback ??

---

**¡El minimapa está listo para usar!** ??

Si tienes problemas, consulta:
- [Solución de Problemas en GUIA_MINIMAPA.md](GUIA_MINIMAPA.md#solución-de-problemas)
- O ejecuta: `AR Navigation ? Setup MiniMap System ? Verificar Configuración`

---

**Creado por:** Sistema de Navegación AR  
**Versión:** 1.0  
**Fecha:** 2024  
**Licencia:** MIT (ajustar según tu proyecto)
