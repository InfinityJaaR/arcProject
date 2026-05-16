# ?? INSTRUCCIONES: Configurar Firestore para Navegación por Grafo

## ?? Paso a Paso para Firebase Console

### PASO 1: Acceder a Firebase Console

1. Ve a: https://console.firebase.google.com/
2. Selecciona tu proyecto
3. En el menú lateral, click en **"Firestore Database"**

---

### PASO 2: Verificar Collection `buildingLocations`

Esta collection ya debería existir. Solo necesitas **agregar el campo `type`** a todos los documentos.

#### Para cada EDIFICIO (destino navegable):

1. Click en el documento
2. Click en **"Add field"** (si no tiene `type`)
3. Field name: `type`
4. Field type: **string**
5. Value: `edificio`
6. Click **"Add"**

#### Para cada NODO DE INFLEXIÓN:

1. Crear nuevo documento o editar existente
2. Agregar estos campos:

| Field name | Type | Value (ejemplo) |
|------------|------|-----------------|
| `name` | string | `"Nodo A"` |
| `description` | string | `"Punto de conexión"` |
| `latitude` | number | `13.7202253348720` |
| `longitude` | number | `-89.2003825607049` |
| `type` | string | `nodo_de_inflexion` |
| `nearby_places` | string | `"Edificio A-Edificio B"` |
| `arPatternId` | string | `""` (vacío) |
| `patternUrl` | string | `""` (vacío) |

**IMPORTANTE:** Los nodos de inflexión NO necesitan `arPatternId` ni `patternUrl`

---

### PASO 3: Crear Collection `graphEdges`

1. En Firestore Database, click **"Start collection"**
2. Collection ID: `graphEdges`
3. Click **"Next"**

---

### PASO 4: Agregar Edges (Conexiones)

Para cada conexión entre dos nodos:

#### Ejemplo: Conectar "Edificio A" con "Nodo 1"

1. Click **"Add document"**
2. Document ID: (dejar que Firebase lo auto-genere, O nombrar descriptivamente como `edge_edificioA_nodo1`)

3. Agregar estos campos:

| Field name | Type | Value |
|------------|------|-------|
| `source` | string | **ID exacto** del primer nodo (ej: `2MpVGjui5ZOxKK1tHzMO`) |
| `target` | string | **ID exacto** del segundo nodo (ej: `5spW3JxHmemJuPAhNdLc`) |
| `distance` | number | Distancia en metros (ej: `50.0`) |

4. Click **"Save"**

**CRUCIAL:** Los valores de `source` y `target` deben ser **exactamente** los IDs de documentos en `buildingLocations`

---

## ?? Cómo Obtener los IDs de Documentos

### Método 1: Desde Firestore Console

1. Ve a collection `buildingLocations`
2. Los IDs están en la columna de la izquierda
3. Click derecho en el ID ? "Copy"
4. Pega ese ID en `source` o `target`

**Ejemplo de IDs:**
```
2MpVGjui5ZOxKK1tHzMO
5spW3JxHmemJuPAhNdLc
8YLwj6VOhzT2KPzZDMF9
N0UxoKqwo98gRg3cuZUZ
Ki03NOqDgcIIUkIelNnF
```

---

## ?? Cómo Calcular la Distancia

### Opción 1: Google Maps

1. Abre Google Maps
2. Click derecho en el primer punto ? "Medir distancia"
3. Click en el segundo punto
4. Google muestra la distancia en metros

### Opción 2: Calculadora Online

1. Ve a: https://www.movable-type.co.uk/scripts/latlong.html
2. Ingresa las coordenadas del punto A
3. Ingresa las coordenadas del punto B
4. Selecciona "Distance"
5. Copia el resultado (en metros)

### Opción 3: Script de Unity (para verificar)

Agregar temporalmente en algún script:

```csharp
void Start()
{
    double lat1 = 13.7207011622727;
    double lon1 = -89.2006180851595;
    double lat2 = 13.7202253348720;
    double lon2 = -89.2003825607049;
    
    float distance = GeoUtils.CalculateDistance(lat1, lon1, lat2, lon2);
    Debug.Log($"Distancia entre puntos: {distance:F2}m");
}
```

---

## ??? Ejemplo Completo

Imaginemos que tienes 3 edificios y 2 nodos de conexión:

```
Edificio A
    |
   50m
    |
  Nodo 1
   /  \
 80m  40m
 /      \
Edificio B  Nodo 2
              |
             60m
              |
          Edificio C
```

### buildingLocations (5 documentos)

| Document ID | name | type | lat | lon |
|-------------|------|------|-----|-----|
| `ABC123` | Edificio A | edificio | 13.7207 | -89.2006 |
| `DEF456` | Nodo 1 | nodo_de_inflexion | 13.7202 | -89.2003 |
| `GHI789` | Edificio B | edificio | 13.7185 | -89.2045 |
| `JKL012` | Nodo 2 | nodo_de_inflexion | 13.7180 | -89.2040 |
| `MNO345` | Edificio C | edificio | 13.7178 | -89.2038 |

### graphEdges (4 documentos)

| Document ID | source | target | distance |
|-------------|--------|--------|----------|
| `edge_1` | `ABC123` | `DEF456` | 50.0 |
| `edge_2` | `DEF456` | `GHI789` | 80.0 |
| `edge_3` | `DEF456` | `JKL012` | 40.0 |
| `edge_4` | `JKL012` | `MNO345` | 60.0 |

---

## ? Validación

### Checklist de Verificación

Antes de cerrar Firebase Console, verifica:

#### buildingLocations
- [ ] Todos los documentos tienen el campo `type`
- [ ] Edificios destino tienen `type: "edificio"`
- [ ] Nodos intermedios tienen `type: "nodo_de_inflexion"`
- [ ] Todas las coordenadas (latitude, longitude) son correctas

#### graphEdges
- [ ] Cada edge tiene los 3 campos: `source`, `target`, `distance`
- [ ] Los IDs en `source` existen en `buildingLocations`
- [ ] Los IDs en `target` existen en `buildingLocations`
- [ ] Las distancias son realistas (> 0, no demasiado grandes)

#### Conectividad
- [ ] Cada edificio destino tiene al menos 1 edge conectándolo
- [ ] Cada nodo intermedio tiene al menos 2 edges (entrada y salida)
- [ ] Es posible llegar de cualquier edificio a cualquier otro

---

## ?? Probar Conectividad

Para verificar que todo está bien conectado, imagina que quieres ir del **Edificio A** al **Edificio C**:

1. ¿El Edificio A tiene un edge? ? Sí (edge_1 lo conecta a Nodo 1)
2. ¿Nodo 1 tiene edges? ? Sí (edge_1, edge_2, edge_3)
3. ¿Puedo llegar de Nodo 1 a Edificio C?
   - Opción 1: Nodo 1 ? Edificio B (edge_2) ? No llega a C
   - Opción 2: Nodo 1 ? Nodo 2 (edge_3) ? Edificio C (edge_4) ? Sí llega

Si encuentras un edificio que NO tiene camino a otro, necesitas agregar más edges.

---

## ?? Errores Comunes

### Error 1: IDs Incorrectos

? **INCORRECTO:**
```json
{
  "source": "Edificio A",  // ? Usar el nombre
  "target": "Nodo 1",
  "distance": 50.0
}
```

? **CORRECTO:**
```json
{
  "source": "ABC123",  // ? Usar el ID del documento
  "target": "DEF456",
  "distance": 50.0
}
```

---

### Error 2: Campo `type` Mal Escrito

? **INCORRECTO:**
```json
{
  "type": "Edificio"  // ? Mayúscula
}
```
```json
{
  "type": "nodo de inflexion"  // ? Espacio en vez de guión bajo
}
```

? **CORRECTO:**
```json
{
  "type": "edificio"  // ? Todo en minúsculas
}
```
```json
{
  "type": "nodo_de_inflexion"  // ? Con guión bajo
}
```

---

### Error 3: Distancia en Kilómetros

? **INCORRECTO:**
```json
{
  "distance": 0.05  // ? 50 metros expresado como 0.05km
}
```

? **CORRECTO:**
```json
{
  "distance": 50.0  // ? 50 metros
}
```

**SIEMPRE usa METROS, no kilómetros.**

---

## ?? Template para Copiar/Pegar

### Template de Edge

```json
{
  "source": "PEGA_AQUI_EL_ID_DEL_PRIMER_NODO",
  "target": "PEGA_AQUI_EL_ID_DEL_SEGUNDO_NODO",
  "distance": DISTANCIA_EN_METROS
}
```

### Template de Nodo de Inflexión

```json
{
  "name": "Nodo X",
  "description": "Descripción del punto",
  "latitude": 13.7200000000000,
  "longitude": -89.2000000000000,
  "type": "nodo_de_inflexion",
  "nearby_places": "Lista de lugares cercanos",
  "arPatternId": "",
  "patternUrl": ""
}
```

---

## ?? Próximos Pasos

Una vez que hayas creado todas las conexiones en Firestore:

1. ? Verifica que todo esté correcto usando el checklist
2. ? Abre Unity
3. ? Crea el GameObject `GraphNavigationManager`
4. ? Asigna las referencias necesarias
5. ? Build and Run
6. ? Revisa los logs en Android Logcat

**Logs esperados:**
```
[FirebaseManager] ?? Consultando colección 'graphEdges'...
[FirebaseManager] ? Se obtuvieron 20 edges del grafo
[GraphNavigationManager] ? Grafo inicializado correctamente
[PathfindingService] ? Inicializado con 15 nodos y 20 aristas
```

---

## ?? Soporte

Si algo no funciona, revisa:

1. **Logs en Unity Console** (Editor)
2. **Android Logcat** (Dispositivo)
   - Window ? Analysis ? Android Logcat
   - Filtrar por: `FirebaseManager` o `GraphNavigation`

3. **Firestore Console**
   - Verificar que las collections existan
   - Verificar que los documentos tengan todos los campos

---

## ? ¡Listo!

Con estos pasos, tu base de datos estará configurada correctamente para el sistema de navegación por grafo. ??
