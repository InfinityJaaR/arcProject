# ??? MINIMAPA DEL GRAFO - README

## ?? ¿QUÉ ES ESTO?

Un **sistema completo de minimapa 2D** que muestra un croquis del grafo de navegación de tu campus/edificio en tiempo real durante la navegación AR.

---

## ? INSTALACIÓN ULTRA RÁPIDA

```bash
1. Unity ? Menú superior ? AR Navigation ? Setup MiniMap System
2. Click en: "?? Configurar MiniMap Automáticamente"
3. Espera: "MiniMap Configurado ?"
4. Play ??
```

**¡Eso es todo!** El minimapa aparecerá automáticamente en la esquina superior derecha.

---

## ?? VISTA PREVIA

```
????????????????????????????????????????
?                   ?????????????????? ?
?  [Tu App AR]      ? ??? Mapa       ? ?
?                   ?????????????????? ?
?  (Cámara con      ?                ? ?
?   flecha 3D)      ?  ??????????   ? ?
?                   ?   ?       ?    ? ?
?                   ?  ??????????   ? ?
?  Destino: A3      ?   ?   ?       ? ?
?  45m restantes    ?  ??  ??      ? ?
?                   ? (Tú)(Meta)     ? ?
?  [Cancelar]       ?????????????????? ?
?                   [??? Mapa]        ?
????????????????????????????????????????
```

**Leyenda:**
- ?? = Edificios (azul)
- ? = Puntos de camino (gris)
- ?? = Ruta activa (verde)
- ?? = Tu posición (rojo)
- ?? = Próximo objetivo (naranja)

---

## ? CARACTERÍSTICAS

| Característica | Estado |
|----------------|--------|
| Visualiza todos los nodos del grafo | ? |
| Muestra todas las conexiones | ? |
| Resalta la ruta activa | ? |
| Posición del usuario en tiempo real | ? |
| Marca el próximo waypoint | ? |
| Actualización automática | ? |
| Configuración con 1 click | ? |
| Totalmente personalizable | ? |
| Compatible con móviles | ? |
| No interfiere con AR | ? |

---

## ?? DOCUMENTACIÓN

### Para Empezar
- **[RESUMEN_MINIMAPA.md](RESUMEN_MINIMAPA.md)** - Lee esto primero (5 min)

### Documentación Completa
- **[GUIA_MINIMAPA.md](GUIA_MINIMAPA.md)** - Guía paso a paso completa
- **[EJEMPLOS_VISUALES_MINIMAPA.md](EJEMPLOS_VISUALES_MINIMAPA.md)** - Casos de uso visuales
- **[INDICE_MINIMAPA.md](INDICE_MINIMAPA.md)** - Referencia técnica completa

---

## ?? PERSONALIZACIÓN RÁPIDA

### Cambiar Colores
```
1. Hierarchy ? MiniMapCanvas
2. Inspector ? MiniMapController
3. Configuración Visual ? Modificar colores
```

### Cambiar Tamaño
```
1. Hierarchy ? MiniMapCanvas ? MapPanel
2. Inspector ? RectTransform
3. Width/Height (Recomendado: 250-400)
```

### Cambiar Posición
```
1. Hierarchy ? MiniMapCanvas ? MapPanel
2. Inspector ? RectTransform
3. Anchors ? Seleccionar esquina deseada
```

---

## ?? SOLUCIÓN DE PROBLEMAS

| Problema | Solución Rápida |
|----------|-----------------|
| No aparece el mapa | Ejecuta Setup MiniMap ? Configurar Automáticamente |
| No se ven nodos | Verifica colores con Alpha > 0.5 |
| Ruta no se dibuja | Inicia navegación primero |
| Posición no actualiza | Verifica GPS activo |

**Verificación completa:**
```
AR Navigation ? Setup MiniMap System ? Verificar Configuración
```

---

## ??? ARQUITECTURA

### Archivos Creados

```
Assets/
??? Scripts/
?   ??? MiniMapController.cs          # Componente principal
?
??? Editor/
?   ??? SetupMiniMap.cs                # Herramienta de setup
?
??? Resources/
?   ??? NodeDot.prefab                 # Prefab de nodos
?
??? Docs/
    ??? GUIA_MINIMAPA.md               # Guía completa
    ??? RESUMEN_MINIMAPA.md            # Resumen rápido
    ??? EJEMPLOS_VISUALES_MINIMAPA.md  # Casos visuales
    ??? INDICE_MINIMAPA.md             # Índice técnico
```

### Integración

El minimapa se integra automáticamente con:
- ? `GraphNavigationManager` - Datos del grafo y navegación
- ? `PathfindingService` - Rutas calculadas
- ? `LocationManager` - Posición GPS
- ? `FirebaseManager` - Carga de datos

**No requiere código adicional.**

---

## ?? USO

### Automático
El minimapa:
1. Se inicializa al cargar el grafo
2. Se actualiza durante la navegación
3. Muestra tu posición en tiempo real
4. Resalta la ruta activa

**No requiere interacción manual.**

### Toggle Manual

**Opción 1:** Botón UI
```
Click en "??? Mapa" (esquina inferior derecha)
```

**Opción 2:** Código
```csharp
FindObjectOfType<MiniMapController>().ToggleMiniMap();
```

---

## ?? RENDIMIENTO

| Métrica | Valor |
|---------|-------|
| GameObjects | ~50-100 |
| Updates/seg | 2 |
| Draw calls | +1-3 |
| Memoria | <5 MB |
| **Impacto** | **Mínimo** |

---

## ?? PRÓXIMOS PASOS

1. **Pruébalo:**
   ```
   Play ?? ? Inicia navegación ? Observa el minimapa
   ```

2. **Personalízalo:**
   ```
   Cambia colores, tamaño, posición según tu app
   ```

3. **Compártelo:**
   ```
   Build and Run en dispositivo real ??
   ```

---

## ? FAQ

**¿Funciona sin GPS?**  
? No. Requiere GPS para mostrar posición del usuario.

**¿Funciona offline?**  
?? Parcialmente. Necesita cargar el grafo una vez.

**¿Compatible con AR Foundation?**  
? Sí, completamente compatible.

**¿Puedo tener varios minimapas?**  
?? No recomendado (solo uno por escena).

**¿Funciona en dispositivos móviles?**  
? Sí, optimizado para móviles.

---

## ?? SOPORTE

**Si tienes problemas:**

1. Lee: [GUIA_MINIMAPA.md - Solución de Problemas](GUIA_MINIMAPA.md#solución-de-problemas)
2. Verifica: `AR Navigation ? Setup MiniMap ? Verificar Configuración`
3. Revisa logs en Console (busca `[MiniMapController]`)

---

## ?? RESUMEN

### Implementación Completada:
- ? Sistema de minimapa 2D funcional
- ? Integración automática con navegación
- ? Visualización en tiempo real
- ? Herramienta de configuración automática
- ? Documentación completa

### Características:
- ??? Muestra todo el grafo
- ??? Resalta ruta activa
- ?? Posición en tiempo real
- ?? Marca objetivos
- ?? Personalizable
- ? Setup con 1 click

---

## ?? VENTAJAS

? **Fácil instalación** - 1 click  
? **Cero configuración** - Todo automático  
? **Bajo impacto** - Rendimiento óptimo  
? **Documentación completa** - 4 archivos MD  
? **Personalizable** - Colores, tamaño, posición  
? **Producción ready** - Listo para deploy  

---

## ?? LICENCIA

[Ajustar según tu proyecto]

---

## ????? AUTOR

Sistema de Navegación AR - 2024

---

**¡Disfruta del minimapa!** ??

Para más detalles, lee la [Guía Completa](GUIA_MINIMAPA.md).
