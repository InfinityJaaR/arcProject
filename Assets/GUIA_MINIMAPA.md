# ??? GUÍA COMPLETA: MINIMAPA DEL GRAFO DE NAVEGACIÓN

## ?? ÍNDICE
1. [Introducción](#introducción)
2. [Instalación Automática](#instalación-automática)
3. [¿Cómo Funciona?](#cómo-funciona)
4. [Características](#características)
5. [Personalización](#personalización)
6. [Solución de Problemas](#solución-de-problemas)

---

## ?? INTRODUCCIÓN

El **MiniMap System** es un croquis/mapa 2D que muestra:
- ? Todos los nodos del grafo (edificios y puntos de inflexión)
- ? Todas las conexiones entre nodos
- ? **La ruta activa** que estás siguiendo (en verde)
- ? Tu posición actual en tiempo real (punto rojo)
- ? El nodo objetivo actual (marcado en naranja)

**Ejemplo visual:**
```
???????????????????????????
?    ??? Mapa             ?
???????????????????????????
?                         ?
?    ??????????          ?
?    ?        ?          ?
?    ??????????????     ? ? Edificios (azul)
?         ?              ?   Camino activo (verde)
?         ??             ?   Tu posición (rojo)
?                         ?
???????????????????????????
```

---

## ?? INSTALACIÓN AUTOMÁTICA

### Opción 1: Configuración con 1 Click ? (RECOMENDADO)

```
1. En Unity, ve al menú superior:
   AR Navigation ? Setup MiniMap System

2. Click en: "?? Configurar MiniMap Automáticamente"

3. Espera el mensaje: "MiniMap Configurado ?"

4. ¡Listo! ??
```

**Esto creará automáticamente:**
- ? Canvas del minimapa (esquina superior derecha)
- ? MiniMapController configurado
- ? Prefab de nodos circulares
- ? Botón de toggle "??? Mapa"
- ? Integración con GraphNavigationManager

---

### Opción 2: Configuración Manual ??

Si prefieres hacerlo manualmente:

#### Paso 1: Crear el Canvas

```
1. Hierarchy ? Click derecho ? UI ? Canvas
   Nombre: "MiniMapCanvas"

2. Configurar Canvas:
   - Render Mode: Screen Space - Overlay
   - Canvas Scaler:
     • UI Scale Mode: Scale With Screen Size
     • Reference Resolution: 1920 x 1080
   - Sorting Order: 100

3. Añadir componente: MiniMapController
```

#### Paso 2: Crear Panel del Mapa

```
1. Dentro de MiniMapCanvas:
   Crear ? GameObject ? UI ? Panel
   Nombre: "MapPanel"

2. Configurar RectTransform:
   - Anchor: Top-Right (esquina superior derecha)
   - Pivot: (1, 1)
   - Pos X: -20, Pos Y: -20
   - Width: 300, Height: 300

3. Configurar Image:
   - Color: Negro con Alpha 0.7 (semi-transparente)
```

#### Paso 3: Crear Contenedor del Mapa

```
1. Dentro de MapPanel:
   Crear ? GameObject
   Nombre: "MapContainer"

2. Añadir componente: RectTransform

3. Configurar para llenar el panel:
   - Anchor: Stretch (todos los lados)
   - Left/Right/Top/Bottom: 0
```

#### Paso 4: Crear Prefab de Nodo

```
1. Assets ? Create ? Folder ? "Resources"

2. Hierarchy ? Crear GameObject
   Nombre: "NodeDot"

3. Añadir componentes:
   - RectTransform (size: 8x8)
   - Image (sprite: círculo, color: blanco)

4. Arrastrar a Assets/Resources/NodeDot.prefab

5. Eliminar de Hierarchy
```

#### Paso 5: Conectar Referencias

```
1. Seleccionar MiniMapCanvas en Hierarchy

2. En Inspector, en MiniMapController:
   - Map Container: MapContainer (del paso 3)
   - Node Dot Prefab: NodeDot (del paso 4)

3. Guardar escena (Ctrl+S)
```

---

## ?? ¿CÓMO FUNCIONA?

### Flujo de Operación

```
1. Inicio del Juego
   ?
2. MiniMapController espera a que GraphNavigationManager cargue el grafo
   ?
3. Una vez cargado, lee todos los nodos y edges de Firestore
   ?
4. Convierte coordenadas GPS ? Posiciones 2D en el canvas
   ?
5. Dibuja:
   - Líneas grises para conexiones normales
   - Puntos azules para edificios
   - Puntos grises para nodos de inflexión
   ?
6. Durante la navegación:
   - Actualiza posición del usuario cada 0.5s
   - Dibuja la ruta activa en verde
   - Marca el objetivo actual en naranja
```

### Conversión de Coordenadas

El sistema convierte coordenadas GPS del mundo real a pixeles en el canvas:

```csharp
GPS (lat/lon) ? Normalización [0,1] ? Posición 2D en Canvas
```

**Ejemplo:**
```
Nodo A: (13.6789, -89.2345) ? (50px, 120px) en el mapa
Nodo B: (13.6790, -89.2346) ? (55px, 118px) en el mapa
```

---

## ? CARACTERÍSTICAS

### 1. Visualización de Nodos

| Tipo | Color | Tamaño | Descripción |
|------|-------|--------|-------------|
| ?? Edificio | Azul | 8px | Destinos navegables |
| ?? Inflexión | Gris claro | 8px | Puntos de camino |
| ?? Usuario | Rojo | 12px | Tu posición actual |
| ?? Objetivo | Naranja | 16px | Próximo nodo a alcanzar |

### 2. Visualización de Rutas

| Tipo | Color | Grosor | Descripción |
|------|-------|--------|-------------|
| Conexión Normal | Gris (30% opaco) | 2px | Todas las rutas posibles |
| Ruta Activa | Verde (80% opaco) | 4px | Camino que estás siguiendo |

### 3. Actualización en Tiempo Real

- ? Posición del usuario se actualiza cada **0.5 segundos**
- ? La ruta se redibuja cuando cambias de destino
- ? El nodo objetivo se actualiza al alcanzar cada waypoint
- ? Todo se sincroniza automáticamente con `GraphNavigationManager`

### 4. Eventos Conectados

El minimapa escucha estos eventos:

```csharp
GraphNavigationManager.OnTargetNodeChanged += OnTargetChanged;
```

Cuando el sistema de navegación cambia el nodo objetivo, el minimapa actualiza el marcador naranja.

---

## ?? PERSONALIZACIÓN

### Colores

Puedes cambiar todos los colores desde el Inspector:

```
Selecciona: MiniMapCanvas ? MiniMapController

Configuración Visual:
?? Building Node Color: Color de edificios
?? Inflection Node Color: Color de nodos de camino
?? Edge Color: Color de conexiones
?? Active Path Color: Color de la ruta activa
?? Current Target Color: Color del objetivo
?? User Position Color: Color de tu posición
```

**Colores Recomendados:**

```csharp
// Tema Oscuro (por defecto)
Building: RGB(51, 153, 255)    // Azul brillante
Inflection: RGB(204, 204, 204) // Gris claro
Edge: RGB(128, 128, 128, 77)   // Gris transparente
Active Path: RGB(0, 255, 0, 204) // Verde brillante
Target: RGB(255, 128, 0)       // Naranja
User: RGB(255, 0, 0)           // Rojo

// Tema Claro
Building: RGB(0, 100, 200)
Inflection: RGB(100, 100, 100)
Edge: RGB(50, 50, 50, 128)
Active Path: RGB(0, 200, 0, 230)
Target: RGB(255, 100, 0)
User: RGB(200, 0, 0)
```

### Tamaños

```
Configuración Visual:
?? Node Size: 8 (tamaño de puntos de nodos)
?? Line Width: 2 (grosor de líneas normales)
?? Active Path Width: 4 (grosor de ruta activa)
?? Map Padding: 20 (margen interno del mapa)
```

### Posición y Tamaño del Mapa

```
Selecciona: MiniMapCanvas ? MapPanel

RectTransform:
?? Anchors: Mover para cambiar esquina
?  • Top-Right: Esquina superior derecha
?  • Top-Left: Esquina superior izquierda
?  • Bottom-Right: Esquina inferior derecha
?  • Bottom-Left: Esquina inferior izquierda
?
?? Pos X/Y: Ajustar distancia desde la esquina
?? Width/Height: Cambiar tamaño del mapa
   (Recomendado: 250-400px)
```

### Estilos Alternativos

#### Minimapa Grande

```
MapPanel:
  Width: 500
  Height: 500
  Position: Centro de la pantalla
```

#### Minimapa Compacto

```
MapPanel:
  Width: 200
  Height: 200
  Node Size: 6
  Line Width: 1.5
```

#### Minimapa Transparente

```
MapPanel Image:
  Color: RGB(0, 0, 0, 128) // Más transparente
```

---

## ?? SOLUCIÓN DE PROBLEMAS

### ? El minimapa no aparece

**Posibles causas:**

1. **Grafo no cargado:**
   ```
   Solución:
   - Verifica logs en consola
   - Busca: "[MiniMapController] Esperando a que el grafo esté listo..."
   - Si dice "Timeout", revisa FirebaseManager
   ```

2. **Canvas desactivado:**
   ```
   Solución:
   - Hierarchy ? MiniMapCanvas
   - Verifica que el checkbox esté marcado ?
   ```

3. **MapContainer no asignado:**
   ```
   Solución:
   - Selecciona MiniMapCanvas
   - Inspector ? MiniMapController ? Map Container
   - Debe estar asignado a MapPanel/MapContainer
   ```

---

### ? Los nodos no se ven

**Posibles causas:**

1. **Prefab no asignado:**
   ```
   Solución:
   AR Navigation ? Setup MiniMap System
   ? Configurar MiniMap Automáticamente
   ```

2. **Colores con alpha = 0:**
   ```
   Solución:
   - Selecciona MiniMapCanvas
   - Verifica que los colores tengan Alpha > 0
   - Recomendado: Alpha entre 0.7 - 1.0
   ```

---

### ? La ruta activa no se dibuja

**Verificaciones:**

```
1. ¿Está navegando?
   Console ? Busca: "[GraphNavigationManager] Navegación iniciada"

2. ¿El color es visible?
   MiniMapController ? Active Path Color
   Debe ser verde con Alpha alto

3. ¿Hay una ruta válida?
   Console ? Busca: "[PathfindingService] Camino encontrado"
```

---

### ? La posición del usuario no se actualiza

**Verificaciones:**

```
1. ¿GPS está activo?
   Console ? Busca: "[LocationManager] GPS Ready"

2. ¿Permisos otorgados?
   - Android: Location permission
   - Build Settings ? Player Settings ? Permissions

3. ¿Update Interval muy alto?
   MiniMapController ? Update Interval
   Recomendado: 0.5 segundos
```

---

### ?? Verificar Configuración

```
AR Navigation ? Setup MiniMap System
? ?? Verificar Configuración Actual

Esto mostrará en consola:
? MiniMapController encontrado
? mapContainer asignado
? nodeDotPrefab asignado
? GraphNavigationManager encontrado
```

---

## ?? LOGS IMPORTANTES

### Durante la Inicialización

```
[MiniMapController] ??? Inicializando minimapa...
[MiniMapController] ? Esperando a que el grafo esté listo...
[MiniMapController] ? Grafo listo, inicializando mapa...
[MiniMapController] ?? Cargados 15 nodos y 22 edges
[MiniMapController] ?? Bounds: Lat[13.678, 13.682] Lon[-89.234, -89.230]
[MiniMapController] ?? Map size: 260x260
[MiniMapController] ?? Dibujando grafo...
[MiniMapController] ?? Dibujadas 22 conexiones
[MiniMapController] ?? Dibujados 15 nodos
[MiniMapController] ? Grafo dibujado
[MiniMapController] ?? Minimapa dibujado exitosamente
```

### Durante la Navegación

```
[MiniMapController] ??? Ruta activa dibujada (5 segmentos)
[MiniMapController] ?? Objetivo actualizado: Edificio A3
```

---

## ?? USO EN RUNTIME

### Mostrar/Ocultar el Minimapa

**Opción 1: Botón UI**
```
Click en el botón "??? Mapa"
(Creado automáticamente)
```

**Opción 2: Código**
```csharp
MiniMapController miniMap = FindObjectOfType<MiniMapController>();
miniMap.ToggleMiniMap();
```

**Opción 3: Inspector**
```
Durante Play Mode:
Selecciona MiniMapCanvas ? MiniMapController
? Show Mini Map (toggle en tiempo real)
```

### Refrescar el Mapa

Si el grafo cambia en Firestore:

```csharp
MiniMapController miniMap = FindObjectOfType<MiniMapController>();
miniMap.RefreshMap();
```

---

## ?? INTEGRACIÓN CON TU PROYECTO

### El minimapa funciona automáticamente con:

? **GraphNavigationManager** - Sincroniza la navegación
? **PathfindingService** - Lee las rutas calculadas
? **LocationManager** - Obtiene posición GPS
? **FirebaseManager** - Lee el grafo de Firestore

### No requiere configuración adicional si ya tienes:

- ? Sistema de navegación por grafo funcionando
- ? Firestore con `graphEdges` y `buildingLocations`
- ? GPS activado en el dispositivo

---

## ?? CHECKLIST DE CONFIGURACIÓN

- [ ] Ejecutado `AR Navigation ? Setup MiniMap System`
- [ ] Verificado que aparece "MiniMap Configurado ?"
- [ ] Presionado Play y esperado a que cargue el grafo
- [ ] Iniciado navegación a un edificio
- [ ] Verificado que se ve el minimapa en esquina superior derecha
- [ ] Comprobado que la ruta activa se dibuja en verde
- [ ] Verificado que tu posición (punto rojo) se actualiza
- [ ] Probado el botón "??? Mapa" para mostrar/ocultar

---

## ?? ¡LISTO!

El minimapa está configurado y funcionando. Ahora cada vez que navegues, podrás ver:

- ??? El croquis completo del campus
- ??? La ruta que estás siguiendo
- ?? Tu ubicación en tiempo real
- ?? El próximo punto al que debes llegar

**¡Disfruta de la navegación mejorada con visualización!** ??
