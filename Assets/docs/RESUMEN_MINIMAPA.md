# ??? MINIMAPA - RESUMEN EJECUTIVO

## ? ¿QUÉ ES?

Un **croquis 2D en tiempo real** del grafo de navegación que muestra:
- Todos los nodos y conexiones del campus
- La ruta activa que estás siguiendo (verde)
- Tu posición actual (rojo)
- El próximo nodo objetivo (naranja)

---

## ?? INSTALACIÓN RÁPIDA (30 SEGUNDOS)

```
1. Unity ? AR Navigation ? Setup MiniMap System
2. Click: "?? Configurar MiniMap Automáticamente"
3. Espera: "MiniMap Configurado ?"
4. ¡Listo!
```

---

## ?? CARACTERÍSTICAS PRINCIPALES

| Característica | Descripción |
|----------------|-------------|
| ?? Nodos de Edificios | Azul - Destinos navegables |
| ? Nodos de Camino | Gris - Puntos de inflexión |
| ?? Ruta Activa | Verde - Camino que sigues |
| ?? Tu Posición | Rojo - Actualizada en tiempo real |
| ?? Objetivo Actual | Naranja - Próximo waypoint |
| ?? Conexiones | Gris - Todas las rutas posibles |

---

## ?? UBICACIÓN

**Por defecto:** Esquina superior derecha (300x300 px)

```
????????????????????????????????????????
?                     ????????????     ?
?                     ? ??? Mapa  ?     ?
?                     ?          ?     ?
?                     ?  ?????  ?     ?
?                     ?  ?   ?   ?     ?
?                     ?  ??????  ?     ?
?                     ????????????     ?
?                                      ?
?                                      ?
?            (Tu app aquí)             ?
?                                      ?
????????????????????????????????????????
```

---

## ?? PERSONALIZACIÓN RÁPIDA

### Cambiar Colores
```
Selecciona: MiniMapCanvas ? Inspector ? MiniMapController
? Configuración Visual ? Modificar colores
```

### Cambiar Posición
```
Selecciona: MiniMapCanvas ? MapPanel ? RectTransform
? Anchor Presets ? Elegir esquina
```

### Cambiar Tamaño
```
MapPanel ? RectTransform
? Width/Height (Recomendado: 250-400)
```

---

## ?? VERIFICACIÓN

```bash
AR Navigation ? Setup MiniMap System ? Verificar Configuración

Debe mostrar:
? MiniMapController encontrado
? mapContainer asignado
? nodeDotPrefab asignado
? GraphNavigationManager encontrado
```

---

## ? USO

### Automático
- Se activa al cargar el grafo
- Se actualiza durante la navegación
- No requiere interacción

### Toggle Manual
- Botón "??? Mapa" (creado automáticamente)
- O en código:
  ```csharp
  FindObjectOfType<MiniMapController>().ToggleMiniMap();
  ```

---

## ?? SOLUCIÓN RÁPIDA DE PROBLEMAS

| Problema | Solución |
|----------|----------|
| No aparece el mapa | Ejecuta Setup MiniMap ? Configurar Automáticamente |
| No se ven nodos | Verifica que los colores tengan Alpha > 0.5 |
| No se dibuja la ruta | Inicia navegación primero |
| Posición no actualiza | Verifica GPS activo en LocationManager |

---

## ?? DOCUMENTACIÓN COMPLETA

Ver: `Assets/docs/GUIA_MINIMAPA.md`

---

## ?? VENTAJAS

? **No invasivo** - Solo 300x300px en la esquina  
? **Automático** - Se sincroniza solo con la navegación  
? **Personalizable** - Colores, tamaño, posición  
? **Eficiente** - Actualización cada 0.5s  
? **Sin configuración** - Setup con 1 click  

---

## ?? CHECKLIST

- [ ] Configurado con Setup MiniMap
- [ ] Verificado en Play Mode
- [ ] Probado durante navegación
- [ ] Personalizado colores (opcional)
- [ ] Ajustado posición/tamaño (opcional)

---

## ?? SIGUIENTE PASO

1. Presiona **Play** ??
2. Inicia navegación a un edificio
3. Observa el minimapa en acción

¡Eso es todo! ??
