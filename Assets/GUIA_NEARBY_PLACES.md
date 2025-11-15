# ?? GUÍA: CONFIGURACIÓN DE NEARBY PLACES EN INFOPANEL

## ? CAMBIOS REALIZADOS

Se han modificado los siguientes archivos para soportar el campo `nearby_places` de Firestore:

### ?? Scripts Modificados:
- ? `BuildingData.cs` - Agregado campo `nearby_places` (string)
- ? `FirebaseManager.cs` - Actualizado parser para leer `nearby_places` desde Firestore
- ? `InfoPanelController.cs` - Agregadas referencias UI y lógica para mostrar lugares cercanos

---

## ?? CONFIGURACIÓN DEL PREFAB (REQUERIDO)

Necesitas agregar nuevos elementos UI al prefab `InfoPanel.prefab` para mostrar los lugares cercanos.

### **OPCIÓN 1: Configuración Manual en Unity**

#### Paso 1: Abrir el Prefab
1. En Unity, ve a `Assets/Prefabs/InfoPanel.prefab`
2. Haz doble clic para abrirlo en modo de edición de prefab

#### Paso 2: Crear el Contenedor de Lugares Cercanos
1. En la jerarquía del prefab, busca: `InfoPanel/Canvas/BackgroundPanel`
2. **Right Click** en `BackgroundPanel` ? `Create Empty`
3. Nómbralo: **"NearbyPlacesContainer"**
4. Configura su RectTransform:
   ```
   ?? Anchors: Bottom/Stretch
   ?? Position X: 0, Y: 60
   ?? Size: Width: -40, Height: 80
   ?? Pivot: X: 0.5, Y: 0
   ```

#### Paso 3: Crear el Título
1. **Right Click** en `NearbyPlacesContainer` ? `UI` ? `Text - TextMeshPro`
2. Nómbralo: **"NearbyPlacesTitle"**
3. Configura:
   ```
   ?? Text: "?? Lugares Cercanos:"
   ?? Font Size: 16
   ?? Font Style: Bold
   ?? Color: Amarillo/Naranja claro (R: 1, G: 0.9, B: 0.5)
   ?? Anchors: Top/Stretch
   ?? Position Y: -5
   ?? Height: 20
   ?? Alignment: Left/Top
   ```

#### Paso 4: Crear el Texto de Lugares Cercanos
1. **Right Click** en `NearbyPlacesContainer` ? `UI` ? `Text - TextMeshPro`
2. Nómbralo: **"NearbyPlacesText"**
3. Configura:
   ```
   ?? Text: "Cafetería Central, Auditorio Principal, Biblioteca"
   ?? Font Size: 14
   ?? Color: Blanco (R: 1, G: 1, B: 1)
   ?? Anchors: Stretch/Stretch
   ?? Offsets: Left: 5, Right: -5, Top: -25, Bottom: 5
   ?? Alignment: Left/Top
   ? Word Wrapping: Enabled
   ? Auto Size: Opcional
   ```

#### Paso 5: Ajustar el Panel de Fondo (Opcional)
Si el panel se ve muy pequeño, puedes agrandarlo:
1. Selecciona `BackgroundPanel`
2. Cambia el **Height** a `400` o más (actualmente es `330`)

#### Paso 6: Conectar Referencias en el Script
1. Selecciona el GameObject raíz: `InfoPanel`
2. En el Inspector, busca el componente `InfoPanelController`
3. Arrastra las referencias:
   ```
   ?? Nearby Places Text ? [Arrastra "NearbyPlacesText"]
   ?? Nearby Places Container ? [Arrastra "NearbyPlacesContainer"]
   ```

#### Paso 7: Guardar el Prefab
1. Presiona `Ctrl + S` o **File ? Save**
2. Cierra el modo de edición de prefab

---

### **OPCIÓN 2: Configuración Rápida por Script (Avanzado)**

Si prefieres que Unity configure automáticamente el prefab:

1. En Unity, ve al menú: **AR Tools ? Configurar Nearby Places UI**
2. ¡Listo! El prefab se configurará automáticamente.

---

## ?? ESTRUCTURA DE DATOS EN FIRESTORE

### Formato del Campo `nearby_places`:

En Firestore, el campo `nearby_places` debe ser un **String** (texto simple):

```javascript
buildingLocations/
  ?? 8YLwj6VOhzT2KPzZDMF9/
      ?? name: "Biblioteca Central"
      ?? description: "Sistema bibliotecario moderno..."
      ?? latitude: 13.7181033
      ?? longitude: -89.2040915
      ?? nearby_places: "Cafetería Central, Auditorio Principal, Facultad de Ingeniería"
```

### Cómo Agregar el Campo en Firebase Console:

1. Ve a **Firebase Console ? Firestore Database**
2. Selecciona un documento de la colección `buildingLocations`
3. Click en **"Add field"**
4. Configura:
   ```
   ?? Field name: nearby_places
   ?? Field type: string
   ?? Value: Cafetería Central, Auditorio Principal, Facultad de Ingeniería
   ```
5. Click en **"Save"**

---

## ?? COMPORTAMIENTO DEL SISTEMA

### ? Si hay lugares cercanos:
```
El panel mostrará:

????????????????????????????????????
?   Biblioteca Central             ?
?                                  ?
?   Sistema bibliotecario...       ?
?                                  ?
?   ?? Lugares Cercanos:          ?
?   Cafetería Central,            ?
?   Auditorio Principal,          ?
?   Facultad de Ingeniería        ?
?                                  ?
?   ?? Lat: 13.718103, ...        ?
????????????????????????????????????
```

### ?? Si NO hay lugares cercanos:
```
El panel mostrará:

????????????????????????????????????
?   Biblioteca Central             ?
?                                  ?
?   Sistema bibliotecario...       ?
?                                  ?
?   (Sección de lugares cercanos   ?
?    oculta automáticamente)       ?
?                                  ?
?   ?? Lat: 13.718103, ...        ?
????????????????????????????????????
```

**NOTA:** El contenedor `NearbyPlacesContainer` se oculta automáticamente si no hay datos.

---

## ?? FORMATOS RECOMENDADOS

Puedes escribir el texto de varias formas en Firestore:

### Separado por comas:
```
"Cafetería Central, Auditorio Principal, Facultad de Ingeniería"
```

### Con saltos de línea:
```
"Cafetería Central
Auditorio Principal
Facultad de Ingeniería"
```

### Con viñetas:
```
"• Cafetería Central
• Auditorio Principal
• Facultad de Ingeniería"
```

### Con emojis y distancias:
```
"??? Cafetería Central (50m)
?? Biblioteca (100m)
?? Auditorio (150m)"
```

**El texto se mostrará exactamente como lo escribas en Firestore.**

---

## ?? LOGS ÚTILES

Al trackear un patrón con lugares cercanos, verás:

```
[FirebaseManager] ?? Lugares cercanos encontrados: Cafetería Central, Auditorio...
[InfoPanelController] ? Mostrando lugares cercanos: Cafetería Central, Auditorio...
```

Si no hay lugares cercanos:

```
[InfoPanelController] ?? No hay lugares cercanos para mostrar
```

---

## ?? TROUBLESHOOTING

### ? "No se muestran los lugares cercanos"

**Posibles causas:**
1. El campo `nearby_places` no existe en Firestore
2. El campo está vacío (`""`)
3. Las referencias UI no están conectadas en `InfoPanelController`
4. El contenedor `NearbyPlacesContainer` está desactivado manualmente

**Solución:**
1. Verifica en Firestore que el campo `nearby_places` existe y tiene contenido
2. En Unity, selecciona el prefab `InfoPanel`
3. Verifica que `Nearby Places Text` y `Nearby Places Container` están asignados
4. Revisa los logs en consola

---

### ? "El texto se ve cortado"

**Solución:**
1. Abre el prefab `InfoPanel.prefab`
2. Selecciona `NearbyPlacesContainer`
3. Aumenta el **Height** a `100` o más
4. O ajusta el **Height** de `BackgroundPanel` para dar más espacio
5. Asegúrate de que `NearbyPlacesText` tenga **Word Wrapping** habilitado

---

## ? PERSONALIZACIÓN

### Cambiar el formato en el panel:
Puedes modificar cómo se muestra el texto en `InfoPanelController.cs`:

```csharp
// Agregar un título antes del texto:
if (nearbyPlacesText != null)
{
    nearbyPlacesText.text = "?? Lugares Cercanos:\n" + data.nearby_places;
}
```

### Convertir comas en viñetas:
```csharp
// Reemplazar comas por saltos de línea con viñetas:
if (nearbyPlacesText != null)
{
    string formatted = data.nearby_places.Replace(", ", "\n• ");
    nearbyPlacesText.text = "• " + formatted;
}
```

---

## ?? CHECKLIST FINAL

Antes de probar:
- [ ] Prefab `InfoPanel.prefab` tiene `NearbyPlacesContainer` y `NearbyPlacesText`
- [ ] Referencias UI están conectadas en `InfoPanelController`
- [ ] Campo `nearby_places` existe en Firestore como **string**
- [ ] Al menos un documento tiene texto en `nearby_places`
- [ ] Build and Run en dispositivo Android
- [ ] Trackear un patrón que tenga `nearby_places`

---

## ?? RESULTADO FINAL

Al apuntar la cámara a un marcador, el panel AR mostrará:
1. ? Nombre del lugar
2. ? Descripción
3. ? **Lugares cercanos** (si existen)
4. ? Coordenadas GPS

**¡Sistema de Nearby Places completamente funcional! ??**

Si tienes dudas, revisa los logs en consola o usa Logcat en Android.
