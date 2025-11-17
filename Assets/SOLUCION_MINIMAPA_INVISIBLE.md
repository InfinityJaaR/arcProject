# ?? Solución: Minimapa Invisible o Muy Pequeño

## ?? Problema Detectado

Según tus logs, el minimapa tiene estos bounds:
```
Bounds: Lat[13.690253, 13.841611] Lon[-89.271201, -89.151105]
```

Pero tus nodos del campus están en:
```
Edificio D:    (13.720701, -89.200618)
Nodo N:        (13.720225, -89.200383)
Arquitectura:  (13.720900, -89.200424)
```

**El problema:** Hay nodos "outliers" (con coordenadas incorrectas) que están causando que el mapa se haga muy grande y los nodos del campus se vean minúsculos.

---

## ? Solución Rápida (Recomendada)

### Paso 1: Configurar Bounds Manuales

1. En Unity, selecciona el GameObject `MiniMapCanvas` en la jerarquía
2. Busca el componente `MiniMapController` en el Inspector
3. En la sección **"Configuración de Área"**:
   - ? **Desactiva** `Use Auto Bounds` (quítale el check)
   - Configura estos valores para el campus UES:
     ```
     Manual Min Lat: 13.7195
     Manual Max Lat: 13.7215
     Manual Min Lon: -89.2015
     Manual Max Lon: -89.1995
     ```

### Paso 2: Ejecutar y Verificar

1. Ejecuta la app en tu dispositivo Android
2. Revisa los nuevos logs - deberías ver:
   ```
   [MiniMapController] ?? Usando BOUNDS MANUALES:
   [MiniMapController]    Lat[13.7195, 13.7215]
   [MiniMapController]    Lon[-89.2015, -89.1995]
   ```
3. Revisa también la sección de **DIAGNÓSTICO DE OUTLIERS** para identificar qué nodos tienen coordenadas incorrectas

---

## ?? Diagnóstico Avanzado

Los nuevos logs incluirán:

### 1. Diagnóstico de Outliers
```
[MiniMapController] ?? DIAGNÓSTICO DE OUTLIERS
[MiniMapController] ?? Centro del campus: (13.7205, -89.2005)
[MiniMapController] ?? NODOS MÁS ALEJADOS:
   1. Nodo Problemático
      ?? (13.6902, -89.2712)
      ?? ~15000 metros del centro
      ?? POSIBLE OUTLIER
```

### 2. Nodos Fuera de Bounds
```
[MiniMapController] ?? Dibujados 30 nodos:
   ? 28 dentro de bounds
   ?? 2 fuera de bounds
```

---

## ?? Solución Permanente: Corregir Datos en Firestore

Una vez que identifiques los nodos outliers con los logs:

1. Abre Firebase Console
2. Ve a Firestore Database
3. Navega a la colección `buildingLocations`
4. Busca los nodos problemáticos (aparecerán en los logs)
5. Corrige sus coordenadas `latitude` y `longitude`

### Coordenadas de Referencia UES
El campus de la Universidad de El Salvador está aproximadamente en:
- **Latitud:** 13.7195 - 13.7215 (rango ~200 metros)
- **Longitud:** -89.2015 - -89.1995 (rango ~200 metros)

Si un nodo tiene valores fuera de este rango, probablemente tiene un error de escritura.

---

## ?? Ajustes Visuales Adicionales

En el Inspector del `MiniMapController` también puedes ajustar:

### Tamaños
- **Node Size:** 8 (predeterminado) - Tamaño de los puntos de nodos
- **Line Width:** 2 (predeterminado) - Grosor de las conexiones
- **Active Path Width:** 4 (predeterminado) - Grosor de la ruta activa

### Colores
- **Building Node Color:** Azul - Nodos de edificios
- **Inflection Node Color:** Gris - Nodos intermedios
- **Edge Color:** Gris translúcido - Conexiones
- **Active Path Color:** Verde - Ruta activa
- **User Position Color:** Rojo - Tu ubicación

### Zoom y Área
- **Map Padding:** 20 píxeles de margen alrededor del mapa
- **Update Interval:** 0.5s entre actualizaciones de posición

---

## ?? Valores Óptimos de Bounds

### Para el Campus UES Completo
```
Min Lat: 13.7195
Max Lat: 13.7215
Min Lon: -89.2015
Max Lon: -89.1995
```

### Para un Área más Grande (con margen)
```
Min Lat: 13.7190
Max Lat: 13.7220
Min Lon: -89.2020
Max Lon: -89.1990
```

### Para Solo la Zona Central
```
Min Lat: 13.7200
Max Lat: 13.7210
Min Lon: -89.2010
Max Lon: -89.2000
```

---

## ?? Si el Problema Persiste

1. **Verifica el tamaño del MapContainer:**
   - Selecciona `MapContainer` en la jerarquía
   - En el Inspector, verifica que `RectTransform` tenga un tamaño razonable
   - Recomendado: Width=260, Height=260 píxeles mínimo

2. **Verifica que los nodos se estén dibujando:**
   - Revisa los logs para `?? Dibujados X nodos`
   - Si dice "0 dentro de bounds", tus bounds están mal configurados

3. **Prueba con Auto Bounds mejorado:**
   - ? Activa `Use Auto Bounds`
   - Esto ahora filtra automáticamente los outliers usando percentiles

4. **Revisa el Canvas:**
   - El MiniMapCanvas debe tener un `Canvas Scaler`
   - Recomendado: UI Scale Mode = "Scale With Screen Size"
   - Reference Resolution: 1080 x 1920

---

## ?? Captura de Pantalla Esperada

Después de aplicar estos cambios, deberías ver:
- ? Minimapa visible en la esquina superior derecha
- ? Nodos del campus distribuidos uniformemente
- ? Conexiones entre nodos visibles
- ? Punto rojo (tu posición) visible cuando hay GPS
- ? Ruta verde visible durante navegación

---

## ?? Contacto

Si después de estos pasos el minimapa aún no se ve correctamente, comparte:
1. Screenshot del Inspector de `MiniMapController`
2. Logs completos desde que inicia hasta que dibuja el grafo
3. Screenshot de la app mostrando el minimapa

¡Buena suerte! ??
