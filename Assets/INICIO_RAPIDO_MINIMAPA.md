# ?? INICIO RÁPIDO - MINIMAPA

## ? 30 SEGUNDOS PARA TENER EL MINIMAPA FUNCIONANDO

### Paso 1: Configurar (10 segundos)
```
Unity ? Menú superior ? AR Navigation ? Setup MiniMap System
```

### Paso 2: Click (5 segundos)
```
Click en: "?? Configurar MiniMap Automáticamente"
```

### Paso 3: Confirmar (5 segundos)
```
Espera el mensaje: "MiniMap Configurado ?"
Click: "Excelente!"
```

### Paso 4: Probar (10 segundos)
```
Presiona Play ??
Espera ~5 segundos (carga del grafo)
Inicia navegación a cualquier edificio
```

### Resultado:
```
??????????????????
? ??? Mapa       ?
??????????????????
?  ?????????   ?  ? Edificios y caminos
?   ?   ?   ?   ?
?  ??????????   ?  ? Ruta activa (verde)
?   ?           ?
?  ?? (Tú)      ?  ? Tu posición
?               ?
??????????????????
```

---

## ? VERIFICACIÓN RÁPIDA

### ¿Funciona?
- [ ] ¿Ves el panel negro en la esquina superior derecha? ? ?
- [ ] ¿Hay puntos azules y grises? ? ? (Nodos)
- [ ] ¿Hay líneas grises conectándolos? ? ? (Conexiones)
- [ ] Al navegar, ¿aparece línea verde? ? ? (Ruta activa)
- [ ] ¿Hay un punto rojo que se mueve? ? ? (Tu posición)

### ¿No funciona?
```
AR Navigation ? Setup MiniMap System ? Verificar Configuración
```

Revisa los logs en Console:
```
[MiniMapController] ??? Inicializando minimapa...
[MiniMapController] ? Grafo listo, inicializando mapa...
```

---

## ?? PERSONALIZACIÓN RÁPIDA (OPCIONAL)

### Cambiar Colores (30 segundos)
```
1. Hierarchy ? MiniMapCanvas
2. Inspector ? MiniMapController ? Configuración Visual
3. Modifica los colores que quieras
```

### Cambiar Tamaño (15 segundos)
```
1. Hierarchy ? MiniMapCanvas ? MapPanel
2. Inspector ? RectTransform
3. Width/Height ? Cambia a 250-500
```

### Cambiar Posición (15 segundos)
```
1. Hierarchy ? MiniMapCanvas ? MapPanel
2. Inspector ? RectTransform ? Anchors
3. Click en la esquina que prefieras
```

---

## ?? ¿QUIERES SABER MÁS?

### Lectura Recomendada (por orden):

1. **README_MINIMAPA.md** ? Empieza aquí
   - Visión general
   - Características
   - FAQ

2. **RESUMEN_MINIMAPA.md**
   - Guía condensada
   - Personalización detallada
   - Solución de problemas

3. **GUIA_MINIMAPA.md**
   - Tutorial completo
   - Instalación manual
   - Personalización avanzada

4. **EJEMPLOS_VISUALES_MINIMAPA.md**
   - Diagramas y mockups
   - Casos de uso visuales

5. **INDICE_MINIMAPA.md**
   - Referencia técnica
   - Arquitectura
   - Debugging avanzado

---

## ?? SOLUCIÓN RÁPIDA

| Problema | Solución Inmediata |
|----------|-------------------|
| No aparece | Re-ejecuta Setup ? Configurar Automáticamente |
| No hay nodos | Verifica que el grafo esté en Firestore |
| No hay ruta verde | Inicia navegación primero |
| Posición no actualiza | Verifica GPS activo (LocationManager) |
| Todo gris | Aumenta el Alpha de los colores |

---

## ?? TIPS

### Para Mejor Experiencia:
- ? Usa el minimapa mientras navegas (no en exploración)
- ? Ajusta el tamaño según tu pantalla (móvil más pequeño)
- ? Usa colores contrastantes para visibilidad
- ? Prueba en exterior con GPS real

### Para Desarrollo:
- ? Activa logs detallados en Console
- ? Usa "Verificar Configuración" frecuentemente
- ? Prueba en Play Mode antes de Build
- ? Documenta tus personalizaciones

---

## ?? SIGUIENTE NIVEL

### Cuando domines lo básico:

**Personalización Avanzada:**
- Modifica `MiniMapController.cs` para añadir features
- Crea tus propios esquemas de color
- Añade etiquetas a los edificios
- Implementa zoom y pan interactivo

**Integración:**
- Sincroniza con otros elementos de tu UI
- Añade notificaciones en el mapa
- Crea modos de visualización (2D/3D)

**Optimización:**
- Ajusta `updateInterval` según dispositivo
- Implementa object pooling para nodos
- Añade culling para grafos grandes

---

## ?? AYUDA

### Si te atoras:

1. **Documentación:**
   - Lee `GUIA_MINIMAPA.md` ? Solución de Problemas

2. **Herramientas:**
   - Usa `AR Navigation ? Verificar Configuración`

3. **Logs:**
   - Console ? Busca `[MiniMapController]`
   - Busca mensajes de error (rojo)

4. **Community:**
   - Revisa los issues en GitHub (si aplica)
   - Consulta con tu equipo

---

## ?? ¡ESO ES TODO!

Con solo 30 segundos de setup, ahora tienes:

? Un minimapa completamente funcional  
? Visualización del grafo en 2D  
? Ruta activa resaltada  
? Posición en tiempo real  
? Sistema personalizable  

**¡Disfruta de la navegación mejorada!** ??

---

**Próximo paso:** Presiona Play ?? y navega a un edificio para ver el minimapa en acción.
