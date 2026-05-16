# ?? EJEMPLO: Estructura de Firestore para Navegación por Grafo

## ??? Diagrama del Grafo de Ejemplo

Imaginemos un campus simple con 3 edificios y 2 nodos de conexión:

```
    [Biblioteca]
         |
         | 50m
         |
      [Nodo A]
       /    \
    80m      40m
     /          \
[Ingeniería]   [Nodo B]
                  |
                 60m
                  |
              [Rectoría]
```

---

## ?? Collection: `buildingLocations`

### Documento 1: Biblioteca (Edificio)

**Document ID:** `2MpVGjui5ZOxKK1tHzMO`

```json
{
  "name": "Biblioteca Central",
  "description": "Sistema bibliotecario moderno",
  "latitude": 13.7207011622727,
  "longitude": -89.2006180851595,
  "type": "edificio",
  "nearby_places": "Enfrente tiene el edificio C y atrás tiene el edificio de Arquitectura",
  "arPatternId": "",
  "patternUrl": "https://firebasestorage.googleapis.com/..."
}
```

---

### Documento 2: Nodo A (Punto de Conexión)

**Document ID:** `5spW3JxHmemJuPAhNdLc`

```json
{
  "name": "Nodo A",
  "description": "Punto de conexión central",
  "latitude": 13.7202253348720,
  "longitude": -89.2003825607049,
  "type": "nodo_de_inflexion",
  "nearby_places": "Biblioteca-Ingeniería-Nodo B",
  "arPatternId": "",
  "patternUrl": ""
}
```

**Nota:** Los nodos de inflexión típicamente no tienen `arPatternId` ni `patternUrl`

---

### Documento 3: Facultad de Ingeniería (Edificio)

**Document ID:** `8YLwj6VOhzT2KPzZDMF9`

```json
{
  "name": "Facultad de Ingeniería",
  "description": "Edificio principal de Ingeniería",
  "latitude": 13.7185000000000,
  "longitude": -89.2045000000000,
  "type": "edificio",
  "nearby_places": "Cerca de la Cafetería Central",
  "arPatternId": "",
  "patternUrl": "https://firebasestorage.googleapis.com/..."
}
```

---

### Documento 4: Nodo B (Punto de Conexión)

**Document ID:** `N0UxoKqwo98gRg3cuZUZ`

```json
{
  "name": "Nodo B",
  "description": "Punto de conexión sur",
  "latitude": 13.7180500000000,
  "longitude": -89.2040000000000,
  "type": "nodo_de_inflexion",
  "nearby_places": "Nodo A-Rectoría",
  "arPatternId": "",
  "patternUrl": ""
}
```

---

### Documento 5: Rectoría (Edificio)

**Document ID:** `Ki03NOqDgcIIUkIelNnF`

```json
{
  "name": "Rectoría",
  "description": "Edificio administrativo central",
  "latitude": 13.7178000000000,
  "longitude": -89.2038000000000,
  "type": "edificio",
  "nearby_places": "Plaza Central",
  "arPatternId": "",
  "patternUrl": "https://firebasestorage.googleapis.com/..."
}
```

---

## ?? Collection: `graphEdges`

### Edge 1: Biblioteca ? Nodo A

**Document ID:** (auto-generado, ej: `edge_001`)

```json
{
  "source": "2MpVGjui5ZOxKK1tHzMO",
  "target": "5spW3JxHmemJuPAhNdLc",
  "distance": 50.0
}
```

**Significa:** Hay un camino de 50 metros entre la Biblioteca y el Nodo A (bidireccional)

---

### Edge 2: Nodo A ? Ingeniería

**Document ID:** (auto-generado, ej: `edge_002`)

```json
{
  "source": "5spW3JxHmemJuPAhNdLc",
  "target": "8YLwj6VOhzT2KPzZDMF9",
  "distance": 80.0
}
```

---

### Edge 3: Nodo A ? Nodo B

**Document ID:** (auto-generado, ej: `edge_003`)

```json
{
  "source": "5spW3JxHmemJuPAhNdLc",
  "target": "N0UxoKqwo98gRg3cuZUZ",
  "distance": 40.0
}
```

---

### Edge 4: Nodo B ? Rectoría

**Document ID:** (auto-generado, ej: `edge_004`)

```json
{
  "source": "N0UxoKqwo98gRg3cuZUZ",
  "target": "Ki03NOqDgcIIUkIelNnF",
  "distance": 60.0
}
```

---

## ?? Ejemplo de Navegación

### Escenario: Usuario quiere ir de Biblioteca a Rectoría

**Paso 1:** Usuario selecciona "Rectoría" como destino

**Paso 2:** Sistema encuentra nodo más cercano a la posición del usuario
- Asumiendo que está cerca de la Biblioteca ? Nodo más cercano: `Biblioteca`

**Paso 3:** Dijkstra calcula la ruta más corta:
```
Biblioteca ? Nodo A ? Nodo B ? Rectoría
(50m)        (40m)     (60m)
Total: 150 metros
```

**Paso 4:** Navegación paso a paso

| Paso | Objetivo Actual | Distancia | Acción |
|------|----------------|-----------|--------|
| 1 | **Nodo A** | 50m | Flecha apunta al Nodo A |
| 2 | Usuario camina hacia Nodo A | - | Distancia disminuye |
| 3 | Usuario llega a ?10m del Nodo A | - | ? Nodo alcanzado |
| 4 | **Nodo B** | 40m | Flecha cambia a Nodo B |
| 5 | Usuario camina hacia Nodo B | - | Distancia disminuye |
| 6 | Usuario llega a ?10m del Nodo B | - | ? Nodo alcanzado |
| 7 | **Rectoría** | 60m | Flecha apunta a Rectoría (destino final) |
| 8 | Usuario llega a ?10m de Rectoría | - | ?? DESTINO ALCANZADO |

---

## ?? Ejemplo de Ruta Alternativa

Si agregamos un edge directo:

### Edge 5: Biblioteca ? Rectoría (camino largo)

```json
{
  "source": "2MpVGjui5ZOxKK1tHzMO",
  "target": "Ki03NOqDgcIIUkIelNnF",
  "distance": 200.0
}
```

Ahora Dijkstra podría elegir entre:

**Ruta A:** `Biblioteca ? Nodo A ? Nodo B ? Rectoría` (150m) ? MÁS CORTA
**Ruta B:** `Biblioteca ? Rectoría` (200m)

El algoritmo elegirá automáticamente la **Ruta A** porque es más corta.

---

## ?? Cómo Calcular la Distancia

Puedes usar una calculadora de distancia GPS online, o usar esta fórmula (Haversine):

### Ejemplo usando Google Maps:

1. Clic derecho en el punto A ? "¿Qué hay aquí?"
2. Copia las coordenadas (ej: `13.7207011622727, -89.2006180851595`)
3. Clic derecho en el punto B ? "Medir distancia"
4. Clic en el punto A
5. Google te mostrará la distancia en metros

### Ejemplo usando código:

```csharp
double lat1 = 13.7207011622727;
double lon1 = -89.2006180851595;
double lat2 = 13.7202253348720;
double lon2 = -89.2003825607049;

float distance = GeoUtils.CalculateDistance(lat1, lon1, lat2, lon2);
Debug.Log($"Distancia: {distance}m");
```

---

## ??? Plantilla para Firebase Console

Puedes copiar y pegar esto directamente en Firebase:

### buildingLocations (5 documentos)

| Document ID | name | type | latitude | longitude |
|-------------|------|------|----------|-----------|
| 2MpVGjui5ZOxKK1tHzMO | Biblioteca Central | edificio | 13.7207011622727 | -89.2006180851595 |
| 5spW3JxHmemJuPAhNdLc | Nodo A | nodo_de_inflexion | 13.7202253348720 | -89.2003825607049 |
| 8YLwj6VOhzT2KPzZDMF9 | Facultad de Ingeniería | edificio | 13.7185000000000 | -89.2045000000000 |
| N0UxoKqwo98gRg3cuZUZ | Nodo B | nodo_de_inflexion | 13.7180500000000 | -89.2040000000000 |
| Ki03NOqDgcIIUkIelNnF | Rectoría | edificio | 13.7178000000000 | -89.2038000000000 |

### graphEdges (4 documentos)

| Document ID | source | target | distance |
|-------------|--------|--------|----------|
| edge_001 | 2MpVGjui5ZOxKK1tHzMO | 5spW3JxHmemJuPAhNdLc | 50.0 |
| edge_002 | 5spW3JxHmemJuPAhNdLc | 8YLwj6VOhzT2KPzZDMF9 | 80.0 |
| edge_003 | 5spW3JxHmemJuPAhNdLc | N0UxoKqwo98gRg3cuZUZ | 40.0 |
| edge_004 | N0UxoKqwo98gRg3cuZUZ | Ki03NOqDgcIIUkIelNnF | 60.0 |

---

## ? Checklist de Validación

Antes de hacer build, verifica:

### buildingLocations
- [ ] Todos los edificios tienen `type: "edificio"`
- [ ] Todos los nodos tienen `type: "nodo_de_inflexion"`
- [ ] Todas las coordenadas son correctas
- [ ] Los nombres son descriptivos

### graphEdges
- [ ] Todos los `source` existen en `buildingLocations`
- [ ] Todos los `target` existen en `buildingLocations`
- [ ] Las distancias son realistas (no negativas, no 0)
- [ ] Cada nodo tiene al menos 1 edge (para estar conectado)

### Conectividad del Grafo
- [ ] Todos los edificios destino son alcanzables desde cualquier punto
- [ ] No hay nodos aislados (sin conexiones)
- [ ] Hay al menos un camino entre cualquier par de nodos

---

## ?? Errores Comunes

### Error: "No se encontró ruta"

**Causa:** IDs en `graphEdges` no coinciden con `buildingLocations`

**Ejemplo incorrecto:**
```json
{
  "source": "biblioteca_001",  // ? Este ID no existe
  "target": "5spW3JxHmemJuPAhNdLc",
  "distance": 50.0
}
```

**Ejemplo correcto:**
```json
{
  "source": "2MpVGjui5ZOxKK1tHzMO",  // ? ID exacto del documento
  "target": "5spW3JxHmemJuPAhNdLc",
  "distance": 50.0
}
```

---

### Error: "Nodo sin conexiones"

**Causa:** Un nodo no tiene ningún edge que lo conecte

**Solución:** Asegúrate de que cada nodo aparezca al menos una vez en `source` O `target` de algún edge

---

## ?? ¡Listo!

Con esta estructura, tu sistema de navegación funcionará perfectamente. El algoritmo de Dijkstra encontrará automáticamente la ruta más corta y la flecha guiará al usuario nodo por nodo.
