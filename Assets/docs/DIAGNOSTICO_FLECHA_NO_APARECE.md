# ?? DIAGNÓSTICO: Por Qué No Aparece la Flecha

## ?? Logs Actuales (Lo que ves)

```
? [NavigationArrowController] ?? Suscrito a LocationManager
? [NavigationArrowController] ?? Suscrito a GraphNavigationManager (x2)
? [AppModeManager] ?? NavigationArrowController habilitado y listo
```

**PERO FALTA:**
```
? [GraphNavigationManager] ?? Nuevo nodo objetivo: ...
? [NavigationArrowController] ?? Nuevo nodo objetivo: ...
? [NavigationArrowController] ? Flecha de navegación activa
```

---

## ?? Problema Diagnosticado

El `NavigationArrowController` **SÍ está funcionando correctamente**:
- ? Se suscribe a eventos
- ? Está listo para recibir eventos

**EL PROBLEMA:** `GraphNavigationManager` **NO está emitiendo** el evento `OnTargetNodeChanged`.

---

## ?? Posibles Causas

### Causa 1: El Grafo No Se Inicializó

**Firestore está vacío** o **no hay conexión**:
- El grafo necesita datos de Firestore
- Sin datos ? No hay nodos ? No se puede calcular ruta

**Logs a buscar:**
```
[GraphNavigationManager] ?? Inicializando grafo de navegación...
[GraphNavigationManager] ? FirebaseManager encontrado
[GraphNavigationManager] ? Firebase listo
[GraphNavigationManager] ?? Descargando grafo desde Firestore...
[GraphNavigationManager] ?? Nodos descargados: 0  ? ?? PROBLEMA
[GraphNavigationManager] ? Grafo vacío - verifica Firestore
```

**Solución:**
1. Configura Firestore siguiendo: `Assets/docs/INSTRUCCIONES_FIRESTORE_GRAFO.md`
2. Crea la colección `graphEdges` en Firestore
3. Agrega campo `type` a documentos en `buildingLocations`

---

### Causa 2: GPS No Está Listo

**LocationManager no tiene posición GPS** cuando intentas navegar:
- El sistema necesita tu posición para calcular la ruta
- Sin GPS ? No puede encontrar nodo cercano ? No puede calcular ruta

**Logs a buscar:**
```
[GraphNavigationManager] ?? StartNavigationToBuilding llamado: Biblioteca Central
[GraphNavigationManager] ? GPS no está listo  ? ?? PROBLEMA
[GraphNavigationManager] ?? Lat: 0.000000, Lon: 0.000000  ? No tiene posición
```

**Solución:**
1. Espera 10-30 segundos después de abrir la app
2. Verifica logs de LocationManager:
   ```
   [LocationManager] ? GPS Inicializado correctamente
   [LocationManager] ?? Nueva ubicación: 13.xxxxxx, -89.xxxxxx
   ```
3. Sal al exterior (GPS funciona mal en interiores)

---

### Causa 3: No Se Encuentra el Nodo Destino

**El edificio seleccionado no existe en el grafo**:
- El grafo solo conoce los nodos que descargó de Firestore
- Si seleccionas un edificio que no tiene nodo ? Error

**Logs a buscar:**
```
[GraphNavigationManager] ?? Buscando nodo para: Biblioteca Central
[GraphNavigationManager] ? No se encontró el nodo para Biblioteca Central  ? ?? PROBLEMA
[GraphNavigationManager] ?? Nodos disponibles en el grafo: 10
[GraphNavigationManager] ?? Lista de nodos en el grafo:
   0. Edificio A (13.718xxx, -89.204xxx)
   1. Edificio B (13.719xxx, -89.203xxx)
   ...
```

**Solución:**
1. Verifica que el edificio esté en Firestore
2. Las coordenadas deben coincidir exactamente
3. Agrega el edificio si falta

---

## ?? Cómo Diagnosticar (HAZLO AHORA)

### Paso 1: Build and Run

```bash
File ? Build and Run
```

### Paso 2: Abre Android Logcat

En Unity:
```
Window ? Analysis ? Android Logcat
```

### Paso 3: Filtra por GraphNavigationManager

En el filtro de búsqueda:
```
GraphNavigationManager
```

### Paso 4: Observa los Logs al Iniciar

**Deberías ver:**
```
[GraphNavigationManager] ?? Inicializando grafo de navegación...
[GraphNavigationManager] ? FirebaseManager encontrado
[GraphNavigationManager] ? Esperando Firebase... (2s)
[GraphNavigationManager] ? Esperando Firebase... (4s)
[GraphNavigationManager] ? Firebase listo
[GraphNavigationManager] ?? Descargando grafo desde Firestore...
[GraphNavigationManager] ?? Nodos descargados: 10  ? Debe ser > 0
[GraphNavigationManager] ?? Edges descargados: 15  ? Debe ser > 0
[GraphNavigationManager] ? Grafo inicializado correctamente
```

**Si ves esto:**
```
[GraphNavigationManager] ?? Nodos descargados: 0
[GraphNavigationManager] ? Grafo vacío
```
? **Problema: Firestore no está configurado**

---

### Paso 5: Espera a que GPS Inicialice

Filtra por `LocationManager`:
```
[LocationManager] ?? Inicializando GPS...
[LocationManager] ? GPS Inicializado correctamente
[LocationManager] ?? Nueva ubicación: 13.xxxxxx, -89.xxxxxx
```

**Espera hasta ver "Nueva ubicación"** antes de navegar.

---

### Paso 6: Selecciona un Destino

En la app, abre el panel de navegación y selecciona un edificio.

**Deberías ver:**
```
[AppModeManager] ?? Iniciando navegación hacia: Biblioteca Central
[GraphNavigationManager] ?? StartNavigationToBuilding llamado: Biblioteca Central
[GraphNavigationManager] ? Todos los requisitos OK
[GraphNavigationManager] ?? Buscando nodo para: Biblioteca Central
[GraphNavigationManager] ? Nodo encontrado: Biblioteca Central
[GraphNavigationManager] ?? StartNavigationToNode: biblioteca_central
[GraphNavigationManager] ?? Posición actual: 13.718xxx, -89.204xxx
[GraphNavigationManager] ?? Nodo más cercano a tu posición: Nodo_Entrada
[GraphNavigationManager] ? Navegación iniciada
[GraphNavigationManager] ??? Ruta calculada: 3 nodos
[GraphNavigationManager] ?? Nuevo nodo objetivo: Nodo_Entrada

[NavigationArrowController] ?? Nuevo nodo objetivo: Nodo_Entrada
[NavigationArrowController] ?? Iniciando flecha de navegación
[NavigationArrowController] ? Flecha de navegación activa
```

---

## ? Diagnóstico por Logs

### Escenario 1: No Hay Logs de GraphNavigationManager

```
? NO aparece: [GraphNavigationManager] ?? Inicializando grafo...
```

**Problema:** GraphNavigationManager no existe o está deshabilitado.

**Solución:**
```
Unity ? AR Navigation ? Configurar Sistema Completo
```

---

### Escenario 2: Grafo Vacío

```
[GraphNavigationManager] ?? Nodos descargados: 0
[GraphNavigationManager] ? Grafo vacío
```

**Problema:** Firestore no está configurado.

**Solución:**
1. Abre `Assets/docs/INSTRUCCIONES_FIRESTORE_GRAFO.md`
2. Sigue las instrucciones paso a paso
3. Crea `graphEdges` collection
4. Agrega campo `type` a `buildingLocations`

---

### Escenario 3: GPS No Listo

```
[GraphNavigationManager] ? GPS no está listo
[GraphNavigationManager] ?? Lat: 0.000000, Lon: 0.000000
```

**Problema:** GPS aún no tiene fix.

**Solución:**
1. Espera 10-30 segundos más
2. Sal al exterior (GPS no funciona en interiores)
3. Verifica permisos de ubicación en Android

---

### Escenario 4: Nodo No Encontrado

```
[GraphNavigationManager] ? No se encontró el nodo para Biblioteca Central
```

**Problema:** El edificio no está en Firestore o coordenadas no coinciden.

**Solución:**
1. Verifica que el documento exista en `buildingLocations`
2. Verifica que tenga campo `type = "edificio"`
3. Verifica que las coordenadas sean exactas

---

## ?? Checklist de Verificación

Marca cada uno cuando lo confirmes:

```
? GraphNavigationManager existe en la escena
? FirebaseManager existe en la escena
? Firestore tiene collection 'buildingLocations'
? Firestore tiene collection 'graphEdges'
? Documentos en 'buildingLocations' tienen campo 'type'
? Hay al menos 1 documento con type="edificio"
? GPS está habilitado en el dispositivo
? App tiene permisos de ubicación
? Esperaste 10-30 seg para que GPS inicialice
? Logs muestran "Nodos descargados: X" donde X > 0
```

---

## ?? Próximo Paso

**Ejecuta la app y comparte estos logs:**

1. Logs al iniciar (GraphNavigationManager inicializando)
2. Logs al seleccionar destino (StartNavigationToBuilding)
3. Cualquier error que aparezca en rojo

Con esos logs podré decirte exactamente cuál es el problema.

---

## ?? Logs Mejorados

He agregado **logs detallados** en `GraphNavigationManager` para que veas exactamente qué está pasando:

- ? Inicialización del grafo
- ? Descarga de nodos y edges
- ? Estado del GPS
- ? Búsqueda de nodo destino
- ? Cálculo de ruta
- ? Actualización de objetivo

**Guarda (`Ctrl+S`) y haz Build and Run** para ver los nuevos logs.
