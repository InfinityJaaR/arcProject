# ??? SETUP MANUAL DEL MAPA EXPANDIBLE

## Si los menús no aparecen, sigue estos pasos:

### PASO 1: Abrir la Escena
1. Abre tu escena principal donde está el `MiniMapCanvas`

### PASO 2: Seleccionar el Canvas
1. En Hierarchy, busca y selecciona `MiniMapCanvas`

### PASO 3: Agregar el Componente
1. Con `MiniMapCanvas` seleccionado
2. En el Inspector, click "Add Component"
3. Busca: `ExpandableMapController`
4. Click para agregarlo

### PASO 4: Ejecutar Script de Setup desde Inspector
1. Con `MiniMapCanvas` aún seleccionado
2. En el Inspector, busca el menú de 3 puntos (?) arriba a la derecha
3. Selecciona "Assets ? Create ? AR Navigation ? Setup Mapa Expandible"

### PASO 5: Alternativa - Ejecutar desde Console

Si los pasos anteriores no funcionan, copia y pega este código en un nuevo archivo C#:

1. **Crear archivo temporal:**
   - Assets ? Create ? C# Script
   - Nombre: `SetupMapaNow`

2. **Pegar este código:**

```csharp
using UnityEngine;
using UnityEditor;

public class SetupMapaNow
{
    [MenuItem("Tools/Setup Mapa Expandible AHORA")]
    public static void SetupNow()
    {
        // Llamar a la utilidad
        ExpandableMapSetupUtility.SetupExpandableMapSystem();
    }
}
```

3. **Guardar** el archivo

4. **Esperar** a que Unity compile (~5 segundos)

5. **Ejecutar:**
   - Unity ? Menú Superior ? Tools ? Setup Mapa Expandible AHORA

6. **Eliminar** el archivo `SetupMapaNow.cs` cuando termines

---

## ?? VERIFICAR QUE FUNCIONÓ

Después de cualquier método, deberías ver en Hierarchy:

```
MiniMapCanvas
??? CompactMapPanel (renombrado desde MapPanel)
?   ??? ExpandButton ?? (NUEVO)
??? ExpandedMapPanel (NUEVO)
    ??? Header
    ?   ??? CloseButton ?
    ?   ??? Title
    ??? ExpandedMapContainer
    ??? Footer
        ??? CenterButton ??
        ??? ZoomSlider
        ??? ZoomText
```

Y en Inspector de `MiniMapCanvas`:
```
? MiniMapController (existente)
? ExpandableMapController (NUEVO)
```

---

## ?? Si Nada Funciona

Contacta y comparte:
1. Screenshot de tu Hierarchy
2. Screenshot de tu carpeta Assets/Editor
3. Logs de la Console de Unity

¡Te ayudaré directamente!
