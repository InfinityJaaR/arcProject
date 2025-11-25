# ?? DEBUGGING MEJORADO - Verificación de Desviación

## ?? PROBLEMA DETECTADO EN LOS LOGS

### Síntoma:
El log de verificación aparece pero está **INCOMPLETO**:

```
[GraphNavigationManager] ?? Verificación de desviación:
GraphNavigationManager:CheckRouteDeviation()
GraphNavigationManager:CheckRouteDeviation()
GraphNavigationManager:CheckRouteDeviation()
```

**Faltan:**
- ? Nodo objetivo actual
- ? Distancia al objetivo
- ? Nodo más cercano
- ? Distancia al más cercano
- ? ¿Está en la ruta?

---

## ?? POSIBLES CAUSAS

### 1. **Excepción silenciosa**
El código lanza una excepción después del primer `Debug.Log()` pero no la muestra.

### 2. **`nearestNode` es NULL**
`pathfindingService.FindNearestNode()` devuelve `null` porque:
- Las coordenadas GPS están fuera del área del grafo
- El grafo no tiene nodos cargados
- Error en el cálculo de distancias

### 3. **GPS devuelve coordenadas inválidas**
Aunque las coordenadas parecen válidas (13.722, -89.226), pueden estar muy lejos de los nodos.

---

## ? SOLUCIÓN IMPLEMENTADA

### Añadidos logs de debugging paso a paso:

```csharp
private void CheckRouteDeviation()
{
    // ... throttle ...
    
    try
    {
        // 1. Obtener GPS
        double currentLat = LocationManager.Instance.CurrentLatitude;
        double currentLon = LocationManager.Instance.CurrentLongitude;
        
        Debug.Log($"?? Verificación de desviación:");
        Debug.Log($"   ?? Posición GPS actual: {currentLat:F6}, {currentLon:F6}");
        
        // 2. Buscar nodo cercano
        Debug.Log($"   ?? Buscando nodo más cercano...");
        GraphNode nearestNode = pathfindingService.FindNearestNode(currentLat, currentLon);
        
        // 3. Verificar si es NULL
        if (nearestNode == null)
        {
            Debug.LogWarning($"   ?? No se encontró nodo cercano - Abortando");
            return;
        }
        
        Debug.Log($"   ? Nodo más cercano encontrado: {nearestNode.Name}");
        
        // 4. Cálculos y logs restantes
        // ...
        
        Debug.Log($"  - Nodo objetivo actual: {currentTargetNode?.Name}");
        Debug.Log($"  - Distancia al objetivo: {distanceToCurrentTarget:F1}m");
        Debug.Log($"  - Nodo más cercano: {nearestNode.Name}");
        Debug.Log($"  - Distancia al más cercano: {distanceToNearest:F1}m");
        Debug.Log($"  - ¿Está en la ruta?: {isOnRoute}");
        
        // 5. Decisión de recalculación
        if (shouldRecalculate)
        {
            // ...
        }
        else
        {
            Debug.Log($"   ? Usuario en ruta correcta");
        }
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"? EXCEPCIÓN en CheckRouteDeviation:");
        Debug.LogError($"   Mensaje: {ex.Message}");
        Debug.LogError($"   Stack: {ex.StackTrace}");
    }
}
```

---

## ?? LOGS QUE AHORA VERÁS

### Escenario 1: Todo funciona correctamente
```
[GraphNavigationManager] ?? Verificación de desviación:
[GraphNavigationManager]    ?? Posición GPS actual: 13.722052, -89.226997
[GraphNavigationManager]    ?? Buscando nodo más cercano...
[GraphNavigationManager]    ? Nodo más cercano encontrado: Nodo A
  - Nodo objetivo actual: Nodo B
  - Distancia al objetivo: 125.3m
  - Nodo más cercano: Nodo A
  - Distancia al más cercano: 15.2m
  - ¿Está en la ruta?: true
[GraphNavigationManager]    ? Usuario en ruta correcta
```

---

### Escenario 2: No encuentra nodo cercano (PROBLEMA)
```
[GraphNavigationManager] ?? Verificación de desviación:
[GraphNavigationManager]    ?? Posición GPS actual: 13.722052, -89.226997
[GraphNavigationManager]    ?? Buscando nodo más cercano...
[GraphNavigationManager]    ?? No se encontró nodo cercano - Abortando verificación
```

**Esto indica:**
- El GPS está funcionando
- Pero `FindNearestNode()` devuelve `null`
- **Posible causa:** Coordenadas GPS muy lejos de los nodos del grafo

---

### Escenario 3: Excepción lanzada
```
[GraphNavigationManager] ?? Verificación de desviación:
[GraphNavigationManager]    ?? Posición GPS actual: 13.722052, -89.226997
[GraphNavigationManager]    ?? Buscando nodo más cercano...
[GraphNavigationManager] ? EXCEPCIÓN en CheckRouteDeviation:
[GraphNavigationManager]    Mensaje: Object reference not set to an instance of an object
[GraphNavigationManager]    Stack: at PathfindingService.FindNearestNode...
```

**Esto indica:**
- Hay un bug en `FindNearestNode()`
- O el `pathfindingService` es null

---

### Escenario 4: Desviación detectada
```
[GraphNavigationManager] ?? Verificación de desviación:
[GraphNavigationManager]    ?? Posición GPS actual: 13.722052, -89.226997
[GraphNavigationManager]    ?? Buscando nodo más cercano...
[GraphNavigationManager]    ? Nodo más cercano encontrado: Nodo X
  - Nodo objetivo actual: Nodo B
  - Distancia al objetivo: 145.8m
  - Nodo más cercano: Nodo X
  - Distancia al más cercano: 42.3m
  - ¿Está en la ruta?: false

[GraphNavigationManager] ?? DESVIACIÓN DETECTADA!
[GraphNavigationManager] ?? Recalculando ruta desde Nodo X...
```

---

## ?? QUÉ HACER AHORA

### Paso 1: Build y prueba nuevamente
```bash
# Build en Unity
# Deploy a Android
# Ver logs en tiempo real
adb logcat -s Unity:D | grep GraphNavigationManager
```

### Paso 2: Busca estos logs específicos

#### ? Si ves:
```
?? Posición GPS actual: 13.722052, -89.226997
?? Buscando nodo más cercano...
? Nodo más cercano encontrado: Nodo A
```
**? Todo funciona correctamente**

#### ?? Si ves:
```
?? Posición GPS actual: 13.722052, -89.226997
?? Buscando nodo más cercano...
?? No se encontró nodo cercano - Abortando verificación
```
**? PROBLEMA: GPS está muy lejos de los nodos del grafo**

**Solución:**
- Verifica que estés en el campus donde están tus nodos
- Revisa las coordenadas de tus nodos en Firestore
- Compara tus coordenadas GPS con las coordenadas de los nodos

#### ? Si ves:
```
?? Posición GPS actual: 13.722052, -89.226997
?? Buscando nodo más cercano...
? EXCEPCIÓN en CheckRouteDeviation:
```
**? PROBLEMA: Bug en el código**

**Solución:**
- Copia el mensaje de error completo
- Revisa el método `FindNearestNode()` en `PathfindingService.cs`

---

## ?? ANÁLISIS DE TU LOG ANTERIOR

Basado en tu log:
```
[GraphNavigationManager] ?? Verificación de desviación:
GraphNavigationManager:CheckRouteDeviation()
GraphNavigationManager:CheckRouteDeviation()
```

**Hipótesis más probable:**
- El método `FindNearestNode()` está devolviendo `null`
- Por eso nunca llega a los logs siguientes
- El `if (nearestNode == null) return;` sale silenciosamente

**Con los nuevos logs sabrás EXACTAMENTE:**
1. ? Si el GPS funciona
2. ? Qué coordenadas está leyendo
3. ? Si encuentra o no el nodo cercano
4. ? Por qué no continúa con los demás logs

---

## ?? CAMBIOS REALIZADOS

### Archivo modificado:
- `Assets/Scripts/GraphNavigationManager.cs`
  - Método `CheckRouteDeviation()` (líneas ~367-440)

### Mejoras:
1. ? **Try-catch** completo para capturar excepciones
2. ? **Logs paso a paso** antes de cada operación
3. ? **Verificación explícita** de `nearestNode == null`
4. ? **Mensaje de éxito** cuando todo está OK
5. ? **Stack trace completo** si hay error

---

## ?? SIGUIENTE PASO

**Prueba ahora con el nuevo build y comparte los logs.**

Deberías ver logs mucho más detallados como:
```
?? Posición GPS actual: ...
?? Buscando nodo más cercano...
? Nodo más cercano encontrado: ...
```

**Con esos logs podré decirte exactamente qué está pasando.** ??

---

**Estado:** ? Debugging mejorado  
**Fecha:** $(Get-Date)  
**Prioridad:** ?? Alta (necesario para diagnosticar el problema)
