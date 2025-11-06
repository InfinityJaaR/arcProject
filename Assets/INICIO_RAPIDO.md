# ?? INICIO RÁPIDO - Solución Error ARCore

## ? Tu Error
```
BuildDatabaseFailedException: arcoreimg failed with exit code 1
Failed to get enough keypoints from target image
```

## ? Solución en 3 Pasos (5 minutos)

### PASO 1: Generar marcador "biblioteca"
1. Unity ? Menu ? **AR Tools**
2. Click ? **Generar Marcador de Prueba**
3. Configura:
   - Nombre: `biblioteca`
   - Resolución: `512`
   - Dejar todo lo demás por defecto
4. Click: **"Generar Marcador"**

### PASO 2: Generar marcador "cafeteria"
1. Unity ? Menu ? **AR Tools**
2. Click ? **Generar Marcador de Prueba**
3. Configura:
   - Nombre: `cafeteria`
   - Resolución: `512`
   - Dejar todo lo demás por defecto
4. Click: **"Generar Marcador"**

### PASO 3: Reemplazar en Reference Image Library
1. Ve a: `Assets/Markers/ReferenceImageLibrary`
2. Selecciónalo en el Inspector
3. **Elimina** las imágenes antiguas:
   - `biblioteca` (la antigua)
   - `cafeteria` (la antigua)
4. Click: **"Add Image"**
5. Arrastra: `Assets/Markers/Generated/biblioteca_512x512.png`
6. Configura:
   - Name: `biblioteca`
   - Specify Size: ?
   - Size: `0.2` x `0.2`
7. Click: **"Add Image"** de nuevo
8. Arrastra: `Assets/Markers/Generated/cafeteria_512x512.png`
9. Configura:
   - Name: `cafeteria`
   - Specify Size: ?
   - Size: `0.2` x `0.2`

## ? ¡Listo!

Ahora puedes hacer **Build and Run** sin errores.

---

## ?? Si necesitas más ayuda:

### Documentación completa:
```
Assets/
??? README_SOLUCION.md              ? Resumen ejecutivo
??? SOLUCION_ERROR_KEYPOINTS.md     ? Guía detallada
??? GUIA_IMAGENES_AR.md             ? Guía visual
??? INSTALACION_COMPLETA.md         ? Índice completo
```

### Menu AR Tools:
```
Unity ? AR Tools ?
  ?? Ver Resumen de Solución
  ?? Ver Guía de Imágenes
  ?? Image Tracking Validator
  ?? Ayuda Rápida
```

---

## ?? Importante:

### Tamaño físico de los marcadores:
- **0.2 x 0.2 metros** = 20cm x 20cm
- Este es el tamaño que deben tener cuando los imprimas
- O el tamaño en pantalla si los muestras digitalmente

### Para Firebase:
- Si usas Firebase, asegúrate de que los **nombres** en Reference Image Library coincidan con los **IDs de documentos** en Firestore
- Si los generaste con el generador, actualiza Firebase con los nuevos nombres

---

## ?? Tu próximo build FUNCIONARÁ

**¡Éxito con tu proyecto AR!**
