# ?? AJUSTAR POSICIÓN DE BOTONES EN PANEL LATERAL

## ?? Problema
Los botones están todos agrupados arriba del panel lateral.

## ? Soluciones

### Solución 1: Centrado Automático (IMPLEMENTADA)

**Resultado:** Los botones se centran verticalmente en el panel

```
???????????????
?             ?
?   (vacío)   ? ? Espaciador flexible
?             ?
???????????????
?  CONTROLES  ?
?             ?
?  ? CERRAR   ?
?             ?
?  ?? CENTRAR ?
?             ?
?  ?? ZOOM    ?
?   [???]    ?
?    1.5x     ?
???????????????
?             ?
?   (vacío)   ? ? Espaciador flexible
?             ?
???????????????
```

**Para activar:**
```
Unity ? Tools ? Actualizar Diseño Mapa AHORA
```

El nuevo código agrega espaciadores flexibles arriba y abajo que centran automáticamente todos los botones.

---

### Solución 2: Ajuste Manual en Unity (SIN CÓDIGO)

Si prefieres ajustar la posición tú mismo:

#### Paso 1: Seleccionar el Panel
```
Hierarchy ? MiniMapCanvas ? ExpandedMapPanel ? ControlPanel
```

#### Paso 2: Ajustar el Layout
En el **Inspector**, busca el componente `VerticalLayoutGroup`:

```
VerticalLayoutGroup:
?? Padding
?  ?? Left: 15
?  ?? Right: 15
?  ?? Top: 20    ? AJUSTAR ESTO (aumenta para bajar botones)
?  ?? Bottom: 20
?
?? Child Alignment: Middle Center ? CAMBIAR ESTO
```

**Opciones de Child Alignment:**

| Valor | Resultado |
|-------|-----------|
| Upper Center | Botones arriba |
| Middle Center | Botones centrados (RECOMENDADO) |
| Lower Center | Botones abajo |

**Ejemplos de Padding Top:**
- `Top: 20` ? Cerca del borde superior
- `Top: 100` ? Más abajo
- `Top: 200` ? Muy abajo
- `Top: 300` ? Casi al fondo

---

### Solución 3: Espaciador Fijo (Personalizado)

Si quieres un espacio fijo arriba (no flexible):

#### En el código (`ExpandableMapSetupUtility.cs`), busca:

```csharp
// NUEVO: Agregar espaciador flexible arriba
CreateFlexibleSpacer(controlPanel.transform);
```

#### Reemplázalo por:

```csharp
// Espaciador fijo de 100px
CreateFixedSpacer(controlPanel.transform, 100); // Cambiar 100 al tamaño deseado
```

#### Y agrega esta función:

```csharp
private static void CreateFixedSpacer(Transform parent, float height)
{
    GameObject spacer = new GameObject("FixedSpacer");
    RectTransform spacerRect = spacer.AddComponent<RectTransform>();
    spacerRect.SetParent(parent, false);
    spacerRect.sizeDelta = new Vector2(0, height);
    
    LayoutElement layoutElement = spacer.AddComponent<LayoutElement>();
    layoutElement.preferredHeight = height;
    layoutElement.minHeight = height;
}
```

---

### Solución 4: Distribuir Uniformemente

Si quieres que los botones ocupen todo el alto disponible:

#### En el código, cambia:

```csharp
layout.childAlignment = TextAnchor.MiddleCenter;
```

#### Por:

```csharp
layout.childAlignment = TextAnchor.UpperCenter;
layout.spacing = 30; // Más espacio entre elementos
```

---

## ?? Presets Recomendados

### Preset 1: Centrado (Implementado)
```
Espaciador flexible arriba
Botones en el centro
Espaciador flexible abajo

? Uso: Diseño balanceado
```

### Preset 2: Arriba
```
Sin espaciador arriba
Botones cerca del tope
Espaciador flexible abajo

? Uso: Fácil acceso desde arriba
```

### Preset 3: Abajo
```
Espaciador flexible arriba
Botones cerca del fondo
Sin espaciador abajo

? Uso: Fácil acceso desde abajo
```

### Preset 4: Distribuido
```
Espaciador pequeño arriba
Botones con mucho spacing (30-50px)
Espaciador pequeño abajo

? Uso: Máxima separación
```

---

## ?? Cómo Aplicar un Preset

### Para Preset 2 (Arriba):

En `CreateControlPanel`, elimina o comenta:

```csharp
// CreateFlexibleSpacer(controlPanel.transform); // ? COMENTAR
CreatePanelTitle(controlPanel.transform);
// ... resto de botones ...
CreateFlexibleSpacer(controlPanel.transform); // ? DEJAR
```

### Para Preset 3 (Abajo):

```csharp
CreateFlexibleSpacer(controlPanel.transform); // ? DEJAR
CreatePanelTitle(controlPanel.transform);
// ... resto de botones ...
// CreateFlexibleSpacer(controlPanel.transform); // ? COMENTAR
```

### Para Preset 4 (Distribuido):

```csharp
CreateFixedSpacer(controlPanel.transform, 50); // 50px arriba
CreatePanelTitle(controlPanel.transform);
// ... resto de botones ...
CreateFixedSpacer(controlPanel.transform, 50); // 50px abajo

// Y cambiar:
layout.spacing = 40; // Más espacio entre botones
```

---

## ?? Vista Previa de Cada Preset

### Preset 1 - Centrado (Actual):
```
???????????????
?             ?
?             ?
?  ??????     ? ? Inicio de botones (centro)
?  CONTROLES  ?
?  ? CERRAR   ?
?  ?? CENTRAR ?
?  ?? ZOOM    ?
?  ??????     ? ? Fin de botones
?             ?
?             ?
???????????????
```

### Preset 2 - Arriba:
```
???????????????
?  ??????     ? ? Inicio (cerca del tope)
?  CONTROLES  ?
?  ? CERRAR   ?
?  ?? CENTRAR ?
?  ?? ZOOM    ?
?  ??????     ?
?             ?
?             ?
?             ?
?             ?
???????????????
```

### Preset 3 - Abajo:
```
???????????????
?             ?
?             ?
?             ?
?             ?
?  ??????     ?
?  CONTROLES  ?
?  ? CERRAR   ?
?  ?? CENTRAR ?
?  ?? ZOOM    ?
?  ??????     ? ? Fin (cerca del fondo)
???????????????
```

### Preset 4 - Distribuido:
```
???????????????
?             ?
?  CONTROLES  ?
?             ?
?  ? CERRAR   ?
?             ?
?  ?? CENTRAR ?
?             ?
?  ?? ZOOM    ?
?             ?
???????????????
```

---

## ? Cambio Rápido Sin Código

**Método más rápido (sin recompilar):**

1. Ejecuta la app o Play Mode
2. Selecciona `ControlPanel` en Hierarchy
3. En Inspector ? `VerticalLayoutGroup`
4. Ajusta `Padding ? Top` en tiempo real
5. Cuando te guste, copia el valor
6. Para en Play Mode
7. Aplica el mismo valor en el prefab/escena

---

## ?? Recomendación

Para dispositivos móviles, te recomiendo:

**Preset 1 (Centrado)** ? Ya implementado ?
- Fácil acceso con el pulgar
- Balanceado visualmente
- No requiere estirar mucho

O ajuste manual:
```
Padding Top: 150
Child Alignment: Upper Center
```

---

¿Quieres que implemente algún preset específico o prefieres ajustarlo manualmente en Unity? ??
