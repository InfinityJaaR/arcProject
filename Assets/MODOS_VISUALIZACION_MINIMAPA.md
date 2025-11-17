# ??? MODOS DE VISUALIZACIÓN DEL MINIMAPA

## ?? RESUMEN

El minimapa ahora tiene **2 MODOS** de visualización:

---

## ?? MODO 1: GRAFO COMPLETO (Por defecto)

### ¿Qué muestra?
- ? **TODOS** los nodos del grafo
- ? **TODAS** las conexiones entre nodos
- ? **Ruta activa** resaltada en verde (cuando navegas)
- ? Tu posición en rojo
- ? Próximo objetivo en naranja

### Vista:
```
??????????????????????????
? ??? Mapa Campus        ?
??????????????????????????
?  ????????????????   ?  ? TODO el campus
?   ?   ?   ?   ?   ?   ?  ? TODAS las rutas
?  ?  ??  ?  ?  ?   ?  ? Tu ruta (verde)
?   ?  ???? ?   ?   ?   ?  ? Tú (rojo)
?  ??????????????????   ?  ? Destino marcado
?     Meta              ?
??????????????????????????
```

### Ventajas:
- ? Ves todo el contexto del campus
- ? Puedes ver rutas alternativas
- ? Mejor orientación espacial
- ? Útil para familiarizarte con el campus

### Desventajas:
- ?? Puede verse "saturado" si hay muchos nodos
- ?? Menos focus en tu ruta actual

### Configuración:
```
MiniMapCanvas ? MiniMapController ? Inspector

Toggles:
?? Show Only Active Path: ? (Desmarcado)
```

---

## ?? MODO 2: SOLO RUTA ACTIVA (Nuevo)

### ¿Qué muestra?
- ? **SOLO** los nodos de tu ruta actual
- ? **SOLO** las conexiones de tu ruta
- ? Tu posición en rojo
- ? Próximo objetivo en naranja
- ? NO muestra nodos fuera de tu ruta

### Vista:
```
??????????????????????????
? ??? Mapa - Mi Ruta     ?
??????????????????????????
?                        ?
?  ??                    ?  ? Solo nodos de
?  ????                  ?     tu ruta
?  ??                    ?
?  ???                  ?  ? Camino limpio
?  ??                    ?     y claro
?  ??                    ?
?                        ?
??????????????????????????
```

### Ventajas:
- ? Vista limpia y minimalista
- ? 100% focus en tu ruta
- ? Menos distracciones
- ? Mejor para dispositivos pequeños
- ? Más fácil de leer

### Desventajas:
- ?? No ves el contexto general
- ?? No ves rutas alternativas

### Configuración:
```
MiniMapCanvas ? MiniMapController ? Inspector

Toggles:
?? Show Only Active Path: ?? (Marcado)
```

---

## ?? CAMBIAR ENTRE MODOS

### Durante el Desarrollo (Editor):
```
1. Selecciona: MiniMapCanvas en Hierarchy
2. Inspector ? MiniMapController ? Toggles
3. Show Only Active Path:
   ? = Grafo completo
   ?? = Solo ruta activa
4. Play para ver el cambio
```

### Durante Runtime (Código):
```csharp
MiniMapController miniMap = FindObjectOfType<MiniMapController>();

// Cambiar a "solo ruta"
miniMap.showOnlyActivePath = true;
miniMap.RefreshMap();

// Cambiar a "grafo completo"
miniMap.showOnlyActivePath = false;
miniMap.RefreshMap();
```

### Con Botón UI (Opcional):
Puedes añadir un botón para que el usuario cambie:

```csharp
public void ToggleMapMode()
{
    MiniMapController miniMap = FindObjectOfType<MiniMapController>();
    miniMap.showOnlyActivePath = !miniMap.showOnlyActivePath;
    miniMap.RefreshMap();
}
```

---

## ?? COMPARACIÓN LADO A LADO

| Característica | Grafo Completo | Solo Ruta |
|----------------|----------------|-----------|
| Nodos mostrados | Todos (~10-50) | Solo ruta (~3-8) |
| Conexiones | Todas | Solo ruta |
| Claridad visual | Media | Alta |
| Contexto espacial | Alto | Bajo |
| Performance | Normal | Mejor |
| Zoom automático | No | No (misma escala) |
| Mejor para | Exploración | Navegación |

---

## ?? CASOS DE USO RECOMENDADOS

### Usa GRAFO COMPLETO cuando:
- ? Estás explorando el campus
- ? Quieres ver dónde están todos los edificios
- ? Necesitas conocer rutas alternativas
- ? Estás en un campus pequeño (<20 nodos)
- ? Pantalla grande (tablet)

### Usa SOLO RUTA cuando:
- ? Estás navegando activamente
- ? Quieres concentrarte solo en tu destino
- ? Campus muy grande (>30 nodos)
- ? Pantalla pequeña (móvil)
- ? Usuario casual que solo quiere llegar

---

## ?? PERSONALIZACIÓN AVANZADA

### Modo Híbrido (Personalizado):

Puedes crear un modo intermedio editando `MiniMapController.cs`:

```csharp
// Opción: Mostrar solo edificios + ruta activa
// (Sin nodos de inflexión que no estén en la ruta)

if (showOnlyActivePath)
{
    // Dibujar SOLO edificios + ruta
    foreach (var node in graphNodes.Values)
    {
        if (node.IsBuilding || IsNodeInCurrentPath(node))
        {
            // Dibujar este nodo
        }
    }
}
```

### Auto-Switch (Automático):

Cambiar automáticamente según el estado:

```csharp
void Update()
{
    // Modo "solo ruta" cuando navegas
    if (GraphNavigationManager.Instance.IsNavigating())
    {
        showOnlyActivePath = true;
    }
    else
    {
        // Modo "grafo completo" cuando exploras
        showOnlyActivePath = false;
    }
}
```

---

## ?? RECOMENDACIÓN PERSONAL

### Para tu proyecto:

**Opción A: Grafo Completo (Default actual)**
```
Ventaja: El usuario ve todo el contexto
Ideal si: Tu campus es pequeño-mediano (<25 nodos)
```

**Opción B: Solo Ruta**
```
Ventaja: Vista limpia y clara
Ideal si: Tu campus es grande (>25 nodos)
```

**Opción C: Auto-Switch (Mejor UX)**
```
Grafo completo: Al explorar
Solo ruta: Al navegar
Ideal si: Quieres lo mejor de ambos mundos
```

---

## ?? IMPLEMENTACIÓN RÁPIDA

### Para probar AHORA:

1. **Abre Unity**
2. **Selecciona:** MiniMapCanvas
3. **Inspector ? MiniMapController**
4. **Toggles ? Show Only Active Path:** Marca el checkbox ??
5. **Play ??**
6. **Inicia navegación**
7. **Observa:** Ahora solo ves tu ruta

### Para volver al modo completo:

1. **Desmarca:** Show Only Active Path ?
2. **Stop y Play** de nuevo

---

## ?? NOTAS TÉCNICAS

### Cambio en el código:

**Archivos modificados:**
- `Assets/Scripts/MiniMapController.cs`

**Cambios realizados:**
1. Añadido toggle `showOnlyActivePath`
2. Modificado `DrawGraph()` para respetar el toggle
3. Modificado `DrawActivePath()` para dibujar nodos en modo "solo ruta"

**Retrocompatibilidad:**
- ? Por defecto sigue mostrando el grafo completo
- ? No rompe configuraciones existentes
- ? 100% backward compatible

---

## ?? RESULTADO

Ahora tienes **2 modos de visualización** que puedes cambiar según tus necesidades:

**Modo 1:** ?? Vista completa del campus  
**Modo 2:** ?? Focus total en tu ruta  

**Toggle fácil** en el Inspector para cambiar entre ellos.

---

¿Quieres que añada alguna de las opciones avanzadas (modo híbrido, auto-switch)?
