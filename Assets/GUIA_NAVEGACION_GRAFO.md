# ??? GUÍA: Sistema de Navegación Basado en Grafo

## ?? Resumen del Cambio

Se ha implementado un sistema de navegación basado en grafo que reemplaza la navegación directa punto a punto con una navegación inteligente que sigue nodos intermedios.

### ? Características Nuevas

1. **Navegación por Grafo**: Usa algoritmo de Dijkstra para encontrar el camino más corto
2. **Nodos Intermedios**: La flecha apunta al nodo más cercano en la ruta calculada
3. **Progreso Automático**: Cuando llegas a un nodo (?10m), automáticamente apunta al siguiente
4. **Visualización de Progreso**: Muestra "Nodo X/Y" para saber tu avance
5. **Destino Final Visible**: Siempre muestra el destino final mientras navegas por nodos intermedios

---

## ??? Estructura de Firestore

### Collection: `buildingLocations`

Contiene TODOS los puntos (edificios y nodos de inflexión):

```json
{
  "name": "Edificio D",
  "description": "Facultad de Ingeniería",
  "latitude": 13.7207011622727,
  "longitude": -89.2006180851595,
  "type": "edificio",
  "nearby_places": "...",
  "arPatternId": "...",
  "patternUrl": "..."
}
```

```json
{
  "name": "Nodo N",
  "description": "Punto de conexión",
  "latitude": 13.7202253348,
  "longitude": -89.2003825607,
  "type": "nodo_de_inflexion",
  "nearby_places": "M-K-L-Q-P",
  "arPatternId": "",
  "patternUrl": ""
}
```

**Campos importantes:**
- `type`: **"edificio"** o **"nodo_de_inflexion"**
- `latitude` / `longitude`: Coordenadas GPS exactas

---

### Collection: `graphEdges`

Define las conexiones (aristas) entre nodos:

```json
{
  "source": "qj6ETerSWVh29DiYBQTM",      // ID del nodo origen
  "target": "N0UxoKqwo98gRg3cuZUZ",      // ID del nodo destino
  "distance": 34.42219996817136          // Distancia en metros
}
```

**Importante:**
- Los IDs (`source` y `target`) deben corresponder a documentos existentes en `buildingLocations`
- La arista es **bidireccional**: A?B también funciona como B?A
- `distance` es usado por el algoritmo de Dijkstra para encontrar el camino más corto

---

## ?? Archivos Nuevos Creados

### 1. `GraphEdge.cs`
Modelo de datos para las aristas del grafo.

### 2. `GraphNode.cs`
Representa un nodo en el grafo (edificio o nodo de inflexión).

### 3. `PathfindingService.cs`
Implementa el algoritmo de Dijkstra para encontrar rutas óptimas.

### 4. `GraphNavigationManager.cs`
Gestor principal de la navegación por grafo. Coordina todo el sistema.

---

## ?? Archivos Modificados

### 1. `FirebaseManager.cs`
**Cambios:**
- Nuevo campo: `graphEdgesCollectionName = "graphEdges"`
- Nuevo método: `GetAllGraphEdgesAsync()` - Carga todas las aristas
- Nuevo método: `BuildNavigationGraphAsync()` - Construye el grafo completo
- Nuevos métodos de parseo para `GraphEdge`

### 2. `NavigationArrowController.cs`
**Cambios:**
- Nuevo campo: `progressText` - Para mostrar "Nodo X/Y"
- Modificado: `UpdateArrowRotation()` - Ahora apunta al nodo actual del grafo
- Modificado: `UpdateDistanceDisplay()` - Muestra nodo actual Y destino final
- Nuevos callbacks: `OnTargetNodeChanged`, `OnDestinationReached`, `OnPathProgressChanged`

### 3. `AppModeManager.cs`
**Cambios:**
- Nueva referencia: `GraphNavigationManager`
- Modificado: `StartNavigation()` - Usa `GraphNavigationManager` si está disponible
- Modificado: `DisableNavigation()` - Detiene el `GraphNavigationManager`

---

## ?? Configuración en Unity

### Paso 1: Crear GameObject para GraphNavigationManager

1. En la jerarquía, busca o crea un objeto llamado **"Managers"**
2. Clic derecho ? **Create Empty**
3. Nombra el nuevo objeto: **"GraphNavigationManager"**
4. **Add Component** ? `GraphNavigationManager`

### Paso 2: Configurar GraphNavigationManager

En el Inspector del `GraphNavigationManager`:

| Campo | Valor | Descripción |
|-------|-------|-------------|
| **Node Reached Distance** | `10` | Distancia en metros para considerar que llegaste a un nodo |
| **Update Interval** | `1` | Cada cuántos segundos verificar el progreso |

### Paso 3: Actualizar AppModeManager

En el Inspector del `AppModeManager`:

1. Busca el campo **Graph Navigation Manager**
2. Arrastra el GameObject `GraphNavigationManager` creado

### Paso 4: Actualizar NavigationArrowController

En el Inspector del `NavigationArrowController`:

1. Busca el campo **Progress Text**
2. Arrastra el componente `TextMeshProUGUI` que mostrará "Nodo X/Y"

### Paso 5: Actualizar FirebaseManager

En el Inspector del `FirebaseManager`:

Verifica que existan estos campos:

| Campo | Valor |
|-------|-------|
| **Collection Name** | `buildingLocations` |
| **Graph Edges Collection Name** | `graphEdges` |

---

## ?? Funcionamiento

### Flujo de Navegación

1. **Usuario selecciona destino** (ej: "Edificio D")
2. **GraphNavigationManager** encuentra el nodo más cercano a la posición actual del usuario
3. **Algoritmo de Dijkstra** calcula la ruta más corta desde ese nodo hasta el destino
4. **NavigationArrowController** apunta al primer nodo de la ruta
5. **Cuando el usuario llega a ?10m del nodo**, automáticamente cambia al siguiente
6. **Repite hasta llegar al destino final**

### Ejemplo de Ruta Calculada

```
Posición Actual (GPS)
    ?
[Nodo A] (más cercano) ? 50m
    ?
[Nodo B] ? 80m
    ?
[Nodo C] ? 40m
    ?
[Edificio D] (destino) ? 60m
```

**Total: 230 metros, 4 nodos**

---

## ?? Logs de Debug

El sistema genera logs detallados:

### GraphNavigationManager
```
[GraphNavigationManager] ??? Construyendo grafo de navegación...
[GraphNavigationManager] ? Nodos creados: 15
[GraphNavigationManager] ? Grafo construido: 15 nodos, 20 edges
[GraphNavigationManager] ?? Iniciando navegación a: Edificio D
[GraphNavigationManager] ?? Nodo más cercano a tu posición: Nodo A
[GraphNavigationManager] ? Navegación iniciada
[GraphNavigationManager] ??? Ruta calculada: 4 nodos
```

### PathfindingService
```
[PathfindingService] ? Inicializado con 15 nodos y 20 aristas
[PathfindingService] ?? Buscando camino: Nodo A ? Edificio D
[PathfindingService] ? Camino encontrado (4 nodos, 230.0m):
[PathfindingService]    1. Nodo A ? 
[PathfindingService]    2. Nodo B ? 
[PathfindingService]    3. Nodo C ? 
[PathfindingService]    4. Edificio D
```

### NavigationArrowController
```
[NavigationArrowController] ?? Nuevo nodo objetivo: Nodo A
[NavigationArrowController] ?? Progreso: 1/4 nodos
[NavigationArrowController] ?? Distancia al nodo actual (Nodo A): 45.2m
[NavigationArrowController] ? Llegaste al nodo: Nodo A
[NavigationArrowController] ?? Nuevo nodo objetivo: Nodo B
```

---

## ?? Troubleshooting

### Problema: "Grafo vacío - verifica Firestore"

**Causa:** Las colecciones `buildingLocations` o `graphEdges` están vacías

**Solución:**
1. Ve a Firebase Console ? Firestore
2. Verifica que existan ambas colecciones
3. Verifica que `buildingLocations` tenga documentos con campo `type`
4. Verifica que `graphEdges` tenga documentos con `source`, `target`, `distance`

---

### Problema: "No se encontró ruta"

**Causas posibles:**
1. Los nodos no están conectados en el grafo
2. Los IDs en `graphEdges` no coinciden con los de `buildingLocations`
3. El nodo de inicio o destino no tiene aristas

**Solución:**
1. Revisa los logs de `PathfindingService.PrintGraphInfo()`
2. Verifica que cada nodo tenga al menos 1 conexión
3. Verifica que los IDs en `source`/`target` existan en `buildingLocations`

---

### Problema: "Nodo objetivo es null"

**Causa:** `GraphNavigationManager` no se inicializó correctamente

**Solución:**
1. Verifica que el GameObject `GraphNavigationManager` esté en la escena
2. Verifica que Firebase se haya inicializado correctamente
3. Revisa los logs al inicio para ver si el grafo se cargó

---

## ? Checklist de Verificación

Antes de hacer build:

- [ ] `GraphNavigationManager` agregado a la escena
- [ ] `AppModeManager.graphNavigationManager` está asignado
- [ ] `NavigationArrowController.progressText` está asignado (opcional)
- [ ] `FirebaseManager.graphEdgesCollectionName` = "graphEdges"
- [ ] Firestore tiene documentos en `buildingLocations` con `type` correcto
- [ ] Firestore tiene documentos en `graphEdges` con `source`, `target`, `distance`
- [ ] Los IDs en `graphEdges` coinciden con documentos en `buildingLocations`

---

## ?? Próximos Pasos (Opcional)

### Mejoras Sugeridas

1. **Visualización de Ruta Completa**
   - Mostrar línea AR con toda la ruta
   - Mostrar marcadores en cada nodo

2. **Recalcular Ruta Dinámicamente**
   - Si el usuario se desvía, recalcular desde su posición

3. **Diferentes Algoritmos**
   - A* (más eficiente que Dijkstra)
   - Peso por tipo de camino (preferir caminos peatonales)

4. **Navegación por Voz**
   - "En 20 metros, gira a la derecha"

5. **Historial de Rutas**
   - Guardar rutas frecuentes

---

## ?? Notas Importantes

1. **Distancia de Nodo**: El valor por defecto es **10 metros**. Ajusta `nodeReachedDistance` si necesitas más/menos precisión.

2. **GPS Accuracy**: La precisión del GPS afecta la detección de llegada a nodos. En interiores puede ser menos preciso.

3. **Performance**: El grafo se carga una sola vez al inicio y se guarda en caché. No hay consultas repetidas a Firebase.

4. **Bidireccionalidad**: Las aristas son bidireccionales por defecto. No necesitas crear A?B y B?A por separado.

5. **Modo Fallback**: Si `GraphNavigationManager` no está disponible, el sistema usa navegación directa (modo antiguo).

---

## ?? Contacto

Si tienes dudas sobre la implementación, revisa los logs en la consola de Unity o en Android Logcat.

**¡El sistema está listo para usarse!** ??
