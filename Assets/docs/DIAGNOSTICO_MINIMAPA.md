# ?? DIAGNÓSTICO: MINIMAPA NO SE VE

## ? PROBLEMA

El minimapa aparece como un **panel negro vacío** sin nodos ni rutas.

---

## ?? PASOS DE DIAGNÓSTICO

### 1. Verifica en Console

**Presiona Play y busca estos logs:**

```
[MiniMapController] ??? INICIALIZANDO MINIMAPA
[MiniMapController] ? Esperando a que el grafo esté listo...
[MiniMapController] ? Grafo listo
[MiniMapController] ?? MINIMAPA DIBUJADO EXITOSAMENTE
```

### 2. Si NO ves esos logs:

**El minimapa no se está inicializando.**

**Solución:**
```
AR Navigation ? Setup MiniMap System ? Configurar Automáticamente
```

### 3. Si ves "TIMEOUT esperando datos del grafo":

**El problema es FirebaseManager o Firestore.**

**Verifica:**
- Firestore tiene datos en `graphEdges` y `buildingLocations`
- FirebaseManager está configurado
- Internet conectado

### 4. Si ves "MINIMAPA DIBUJADO" pero no aparece:

**El problema son los colores o el tamaño.**

**Solución:**
```
MiniMapCanvas ? Inspector ? MiniMapController ? Configuración Visual

Cambia:
- Node Size: 15 (más grande)
- Building Node Color: Alpha = 1.0
- Inflection Node Color: Alpha = 1.0
- Edge Color: Alpha = 0.8
```

---

## ?? SOLUCIÓN RÁPIDA

```
1. Para Play Mode
2. Hierarchy ? Elimina "MiniMapCanvas" (si existe)
3. AR Navigation ? Setup MiniMap System
4. Click "Configurar Automáticamente"
5. Play ??
6. Espera 10 segundos
7. Revisa Console
```

---

## ?? COPIAR ESTOS LOGS

Si sigue sin funcionar, copia TODOS los logs que empiecen con `[MiniMapController]` y compártelos.
