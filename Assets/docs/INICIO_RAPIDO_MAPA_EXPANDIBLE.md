# ? INICIO RÁPIDO: MAPA EXPANDIBLE

## ?? ¿Qué Cambia?

**ANTES:** Minimapa pequeño en la esquina ??  
**AHORA:** Minimapa pequeño + Opción de expandir a pantalla completa ??

---

## ?? INSTALACIÓN (1 MINUTO)

### Paso 1: Abrir el Setup
```
Unity ? Menú Superior ? AR Navigation ? Setup Expandable Map System
```

### Paso 2: Configurar Automáticamente
```
Click en el botón grande: "?? Configurar Mapa Expandible Automáticamente"
```

### Paso 3: Confirmar
```
Dialog aparece ? Click "Sí, configurar"
```

### Paso 4: ¡Listo!
```
Mensaje: "¡Configuración Completa! ??"
```

**Tiempo total:** ~30-60 segundos

---

## ?? CÓMO SE USA

### En la App

#### Vista Normal (Minimapa Compacto)
```
???????????????????????
?                     ?
?  [Cámara AR]        ?
?                     ?
?          ??????     ?
?          ???? ? ? Minimapa
?          ?????? ? Botón expandir
?             ?       ?
?        TOCA AQUÍ    ?
???????????????????????
```

#### Vista Expandida (Pantalla Completa)
```
???????????????????????????
? ?  ??? MAPA DEL CAMPUS ? ? Botón cerrar
???????????????????????????
?                         ?
?    [MAPA GRANDE]        ?
?                         ?
?   • Desliza para mover  ?
?   • Slider para zoom    ?
?   • Botón para centrar  ?
?                         ?
???????????????????????????
? ?? [????] Zoom: 1.5x   ?
???????????????????????????
```

---

## ?? FUNCIONALIDADES NUEVAS

### ? En Vista Expandida Puedes:

| Acción | Cómo |
|--------|------|
| **Mover el mapa** | Desliza con un dedo |
| **Hacer zoom** | Usa el slider en el footer |
| **Centrar en ti** | Toca botón ?? |
| **Volver al minimapa** | Toca botón ? |

### ?? Zoom
- **Mínimo:** 0.5x (ver todo el campus)
- **Máximo:** 3.0x (máximo detalle)
- **Inicial:** 1.5x al abrir

### ?? Pan/Arrastre
- Desliza en cualquier dirección
- Límite: ±500 píxeles del centro
- Animación suave al centrar

---

## ? VERIFICACIÓN

### ¿Cómo saber si está bien instalado?

1. **En Unity (Hierarchy):**
   ```
   MiniMapCanvas
   ??? CompactMapPanel (con botón ??)
   ??? ExpandedMapPanel (inicialmente oculto)
   ```

2. **En Inspector (MiniMapCanvas):**
   ```
   Componentes:
   ? MiniMapController
   ? ExpandableMapController ? NUEVO
   ```

3. **Ejecutar verificación automática:**
   ```
   AR Navigation ? Setup Expandable Map System
     ? ?? Verificar Configuración Actual
   ```

---

## ?? CUÁNDO USAR CADA VISTA

### ?? Usa Minimapa Compacto cuando:
- Estás navegando activamente
- Necesitas ver la cámara AR
- Solo quieres referencia rápida

### ??? Usa Mapa Expandido cuando:
- Quieres planificar tu ruta completa
- Necesitas ver todo el campus
- Quieres explorar rutas alternativas
- Necesitas más detalles

---

## ?? SOLUCIÓN DE PROBLEMAS

### ? "Botón expandir no aparece"
```
Solución: 
AR Navigation ? Setup Expandable Map System
  ? Configurar Automáticamente (de nuevo)
```

### ? "Mapa expandido está en blanco"
```
Solución:
1. Verifica que MiniMapController esté funcionando
2. Ejecuta Setup de nuevo
3. Revisa logs en Console
```

### ? "No puedo arrastrar el mapa"
```
Solución:
1. Verifica que estés en vista expandida
2. Inspector ? ExpandableMapController ? Allow Panning ?
3. Usa solo un dedo (no multi-touch)
```

---

## ?? COMPARACIÓN

| Característica | Minimapa | Expandible |
|----------------|----------|------------|
| Tamaño | 260x260 px | Pantalla completa |
| Zoom | Fijo | 0.5x - 3.0x |
| Pan | No | Sí |
| Obstruye AR | No | Sí (temporal) |
| Mejor para | Navegación | Planificación |

---

## ?? MÁS INFORMACIÓN

- **`GUIA_MAPA_EXPANDIBLE.md`** - Guía completa detallada
- **`GUIA_MINIMAPA.md`** - Guía del minimapa base

---

## ?? EJEMPLO DE FLUJO COMPLETO

```
1. Usuario abre la app
   ? Ve el minimapa compacto en la esquina ?

2. Usuario quiere planificar su ruta
   ? Toca el botón ?? en el minimapa
   ? Mapa se expande a pantalla completa ?

3. Usuario explora el mapa
   ? Desliza para ver diferentes áreas
   ? Usa slider para hacer zoom
   ? Toca ?? para centrar en su ubicación ?

4. Usuario está listo para navegar
   ? Toca ? para cerrar
   ? Vuelve a vista AR con minimapa compacto ?

5. Usuario navega
   ? Sigue las instrucciones AR
   ? Consulta minimapa ocasionalmente ?
```

---

**Tiempo de instalación:** 1 minuto  
**Tiempo de aprendizaje:** 2 minutos  
**Mejora de UX:** ? ??

¡Disfruta tu nuevo mapa expandible! ??
