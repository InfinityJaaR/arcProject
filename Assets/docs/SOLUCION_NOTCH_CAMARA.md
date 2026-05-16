# ?? SOLUCIÓN: Ajustar Paneles para Notch/Cámara

## ?? PROBLEMA

Tienes **DOS paneles** que quedan tapados por la cámara/notch:

1. ? **Panel de Selección:** "?? Selecciona un Destino" (al abrir)
2. ? **Panel de Navegación Activa:** "Polideportivo..." con distancia y botón cancelar

Ambos quedan en la parte superior y son tapados por el notch.

---

## ? SOLUCIÓN RÁPIDA (1 minuto)

### **Opción 1: Ajustar TODOS los Paneles** ? RECOMENDADO

```
AR Tools ? Fix Navigation Button ? ?? Ajustar TODOS los Paneles para Notch
```

Esto ajusta **automáticamente**:
- ? Panel de Selección (título y scroll)
- ? Panel de Navegación Activa (movido 50px abajo)

**Resultado:** Ambos paneles visibles debajo del notch.

---

### **Opción 2: Ajustar Solo Uno**

Si solo quieres ajustar un panel específico:

**Panel de Selección:**
```
AR Tools ? Fix Navigation Button ? ?? Ajustar Solo Panel de Selección
```

**Panel de Navegación Activa:**
```
AR Tools ? Fix Navigation Button ? ?? Ajustar Solo Panel de Navegación Activa
```

---

### **Opción 3: Safe Area Automático (Mejor para Producción)** ?

```
AR Tools ? Fix Navigation Button ? ?? Añadir Safe Area a Ambos Paneles
```

Esto añade componentes que:
- ? Detectan automáticamente el Safe Area del dispositivo
- ? Ajustan ambos paneles en runtime
- ? Funcionan en **todos los dispositivos**
- ? Se adaptan automáticamente

---

## ?? CAMBIOS APLICADOS

### **Panel de Selección:**
```
ANTES:
??????????????????????????
? [?? Notch] ? Tapa
? ?? Selecciona... ? Tapado
??????????????????????????

DESPUÉS:
??????????????????????????
? [?? Notch]             ?
?                        ? ? 60px espacio
? ?? Selecciona un Destino? ? Visible ?
??????????????????????????
```

### **Panel de Navegación Activa:**
```
ANTES:
??????????????????????????
? [?? Notch] ? Tapa
? Polideportivo... ? Tapado
? --- m              N(0°)?
? [? Cancelar]           ?

DESPUÉS:
??????????????????????????
? [?? Notch]             ?
?                        ? ? 50px espacio
? Polideportivo...       ? ? Visible ?
? --- m              N(0°)?
? [? Cancelar]           ?
```

---

## ?? VALORES APLICADOS

| Panel | Padding Superior | Descripción |
|-------|------------------|-------------|
| **Selección** | 60px | Más espacio para título |
| **Navegación Activa** | 50px | Panel completo movido abajo |

Estos valores funcionan para la mayoría de dispositivos Android con notch.

---

## ?? AJUSTES PERSONALIZADOS

Si usaste la **Opción 3 (Safe Area)**:

### **Panel de Selección:**
```
1. Selecciona: LocationSelectionPanel
2. Componente: Safe Area Panel
3. Top Padding: 60  ? Ajusta si es necesario
```

### **Panel de Navegación Activa:**
```
1. Selecciona: NavigationActivePanel
2. Componente: Safe Area Panel
3. Top Padding: 50  ? Ajusta si es necesario
```

---

## ?? TROUBLESHOOTING

### **? El panel de navegación sigue tapado:**

**Solución:**
1. Selecciona `NavigationActivePanel` en jerarquía
2. RectTransform:
   - Anchors: Top/Stretch
   - Top: -60 (en lugar de -50)

O si tiene SafeAreaPanel:
- Top Padding: 60-70

### **? El panel queda muy abajo:**

**Solución:**
- Reduce Top Padding a 30-40
- O ajusta el offset manualmente

### **? Solo un panel se arregló:**

**Solución:**
- Ejecuta: **Ajustar TODOS los Paneles para Notch**
- Esto arregla ambos de una vez

---

## ?? TESTING

### **En Unity Editor:**
1. Ejecuta la herramienta
2. **Play** ??
3. **Prueba ambos paneles:**
   - Click "Navegar" ? Panel de selección
   - Selecciona un lugar ? Panel de navegación activa
4. Verifica que ambos estén visibles

### **En Dispositivo Real:**
1. **Build & Run**
2. **Abre el panel de selección** ? ¿Título visible?
3. **Selecciona un lugar** ? ¿Panel de navegación visible?
4. Si alguno sigue tapado:
   - Aumenta Top Padding en el componente SafeAreaPanel
   - O ejecuta de nuevo la herramienta con valores mayores

---

## ?? MEJORES PRÁCTICAS

### **Para Build Final:**
```
? Usa Opción 3: Añadir Safe Area a Ambos Paneles
? Top Padding Panel Selección: 60px
? Top Padding Panel Navegación: 50px
? Update Every Frame: FALSE
? Prueba en múltiples dispositivos
```

### **Para Testing Rápido:**
```
? Usa Opción 1: Ajustar TODOS los Paneles
? Ajusta valores según tu dispositivo
? Migra a Safe Area antes del build final
```

---

## ?? PERSONALIZACIÓN ADICIONAL

### **Cambiar Altura del Panel de Navegación:**

1. Selecciona `NavigationActivePanel`
2. RectTransform ? **Height**: 180 (actual)
3. Cambiar a:
   ```
   Height: 150  (más compacto)
   Height: 180  (actual)
   Height: 200  (más grande)
   ```

### **Mover Botón "Cancelar":**

Si el botón también queda tapado:
1. Selecciona el botón dentro de `NavigationActivePanel`
2. RectTransform ? Ajusta posición Y

---

## ? CHECKLIST FINAL

```
? Ejecutar: Ajustar TODOS los Paneles para Notch
? O ejecutar: Añadir Safe Area a Ambos Paneles
? Play en Unity
? Verificar Panel de Selección (título visible)
? Seleccionar un lugar
? Verificar Panel de Navegación (nombre visible)
? Build & Run en Android
? Probar en dispositivo real
? Ajustar Top Padding si es necesario
```

---

## ?? RESULTADO ESPERADO

Después de aplicar las soluciones:

```
? Panel de Selección:
   - Título visible debajo del notch
   - Scroll funciona correctamente
   - Botones se ven bien

? Panel de Navegación Activa:
   - Nombre del destino visible
   - Distancia y dirección visibles
   - Botón "Cancelar" visible
   - Todo debajo del notch
```

---

## ?? COMPARACIÓN DE OPCIONES

| Característica | Opción 1 (Ajuste Manual) | Opción 3 (Safe Area) |
|----------------|-------------------------|----------------------|
| Ambos paneles | ? Sí | ? Sí |
| Automático | ? Sí | ? Sí |
| Todos los dispositivos | ? No | ? Sí |
| Se adapta a rotaciones | ? No | ? Sí |
| Performance | ? Mejor | ? Bueno |

**Recomendación:** Usa **Opción 1** para testing rápido, **Opción 3** para build final.

---

## ?? ARCHIVOS ACTUALIZADOS

- **`SafeAreaAdjuster.cs`** - Ahora ajusta ambos paneles
  - Función: `AdjustAllPanelsForNotch()`
  - Función: `AdjustSelectionPanel()`
  - Función: `AdjustNavigationPanel()`
  - Función: `AddSafeAreaToAllPanels()`

---

**¡Ambos paneles ahora deberían estar perfectamente visibles!** ???

**Última actualización:** 2025-01-06
