# ?? GUÍA DE CONFIGURACIÓN - FIREBASE + AR

## ?? PROBLEMAS COMUNES AL COMPILAR

### Error: "Failed to get enough keypoints from target image"

Si al hacer **Build and Run** recibes este error:
```
BuildDatabaseFailedException: arcoreimg failed with exit code 1
Failed to get enough keypoints from target image
```

**SOLUCIÓN INMEDIATA:**
1. Ve al menu **AR Tools > Ver Resumen de Solución**
2. O lee: `Assets/README_SOLUCION.md`
3. O usa: **AR Tools > Generar Marcador de Prueba**

**Causa:** Tus imágenes de referencia son muy simples (colores sólidos, pocas características visuales). ARCore necesita imágenes con alto contraste, bordes y detalles distintivos.

**Herramientas instaladas para ayudarte:**
- ?? **AR Tools > Image Tracking Validator** - Analiza tus imágenes
- ??? **AR Tools > Generar Marcador de Prueba** - Crea marcadores garantizados
- ?? **AR Tools > Ver Guía de Imágenes** - Guía visual completa

---

## ? IMPLEMENTACIÓN COMPLETADA

Se han creado los siguientes archivos:
- ? `BuildingData.cs` - Modelo de datos
- ? `FirebaseManager.cs` - Gestor de Firestore
- ? `InfoPanelController.cs` - Controlador de UI AR
- ? `MultiImageSpawner.cs` - Modificado para usar Firebase
- ? `InfoPanel.prefab` - Prefab de panel AR

---

## ?? PASOS DE CONFIGURACIÓN EN UNITY

### **1. Configurar la Escena**

#### **A) Agregar FirebaseManager:**
1. En tu escena AR, crea un GameObject vacío: `Hierarchy > Create Empty`
2. Nómbralo: **"FirebaseManager"**
3. Arrastra el script `FirebaseManager.cs` al GameObject
4. En el Inspector, configura:
   - **Collection Name**: `buildingLocations`
   - **Enable Cache**: ? (activado)
   - **Simulate Data In Editor**: ? (para testing sin build)

#### **B) Configurar MultiImageSpawner:**
1. Selecciona el GameObject que tiene `MultiImageSpawner.cs`
2. En el Inspector:
   - **Use Firebase**: ? (activado)
   - **Info Panel Prefab**: Arrastra `Assets/Prefabs/InfoPanel.prefab` aquí
   - **Show With Limited Tracking**: ? (mantener activado)

---

### **2. Configurar Reference Image Library**

Los nombres de las imágenes deben **coincidir exactamente** con los IDs de documentos en Firestore:

1. Ve a tu **AR Reference Image Library**
2. Para cada imagen, configura el **Name** con el ID de Firestore:
   - Ejemplo: `8YLwj6VOhzT2KPzZDMF9`
   - Ejemplo: `2MpVGjui5ZOxKK1tHzMO`
   - Ejemplo: `Ki03NOqDgcIIUkIe1NnF`

**?? IMPORTANTE:** El nombre debe ser exactamente igual al ID del documento en Firestore.

---

### **3. Verificar google-services.json**

1. Asegúrate de que `google-services.json` esté en `Assets/`
2. Verifica que el archivo tenga la configuración correcta de tu proyecto Firebase

---

## ?? CONFIGURACIÓN EN FIRESTORE

### **Estructura de Datos (Ya existente)**

Tu colección actual ya funciona. Solo asegúrate de que cada documento tenga estos campos:

```json
buildingLocations/
  ?? 8YLwj6VOhzT2KPzZDMF9/  ? Este ID debe coincidir con el nombre en Reference Image Library
      ?? name: "Biblioteca Central"
      ?? description: "Sistema bibliotecario moderno..."
      ?? latitude: 13.7181033
      ?? longitude: -89.2040915
```

### **Reglas de Seguridad**

En Firebase Console, configura las reglas de Firestore para permitir lectura pública:

```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /buildingLocations/{document=**} {
      allow read: if true;  // Permite lectura pública
      allow write: if false; // Solo escritura desde consola/admin
    }
  }
}
```

---

## ?? CÓMO FUNCIONA

### **Flujo de Ejecución:**

1. **Usuario apunta cámara** al patrón (ej: imagen "Biblioteca Central")
2. **ARFoundation detecta** la imagen: `8YLwj6VOhzT2KPzZDMF9`
3. **MultiImageSpawner** recibe el evento
4. **Instancia InfoPanel** en la posición del marcador
5. **Consulta Firebase**: `buildingLocations/8YLwj6VOhzT2KPzZDMF9`
6. **Muestra loading** mientras espera respuesta
7. **Actualiza UI** con datos de Firebase:
   ```
   Biblioteca Central
   
   Sistema bibliotecario moderno que ofrece
   recursos digitales e impresos...
   
   ?? Lat: 13.718103, Lon: -89.204092
   ```

---

## ?? TESTING

### **En Unity Editor (Sin build):**

1. Activa **Simulate Data In Editor** en FirebaseManager
2. Play en Unity
3. Los datos se simularán sin necesidad de Firebase

### **En Android (Build real):**

1. Desactiva **Simulate Data In Editor**
2. Build & Run en dispositivo Android
3. Apunta a los marcadores físicos/impresos
4. Verifica logs en Logcat:
   ```
   [FirebaseManager] ? Firebase inicializado correctamente
   [MultiImageSpawner] ?? Imagen DETECTADA: '8YLwj6VOhzT2KPzZDMF9'
   [FirebaseManager] ?? Consultando Firestore...
   [FirebaseManager] ? Datos obtenidos exitosamente: 'Biblioteca Central'
   ```

---

## ?? TROUBLESHOOTING

### **Problema: "Firebase no está inicializado"**
**Solución:**
- Verifica que `google-services.json` esté en `Assets/`
- Revisa que FirebaseManager esté en la escena
- Espera unos segundos después de iniciar la app

### **Problema: "Documento no existe"**
**Solución:**
- Verifica que el **nombre en Reference Image Library** sea exactamente igual al **ID del documento en Firestore**
- Revisa que el documento exista en la colección `buildingLocations`

### **Problema: "InfoPanelController no encontrado"**
**Solución:**
- Asegúrate de que el prefab `InfoPanel` tenga el script `InfoPanelController.cs` adjunto
- Verifica que el prefab esté asignado en MultiImageSpawner

### **Problema: No se ve el panel AR**
**Solución:**
- Verifica que el tracking esté funcionando (el marcador debe ser visible)
- Revisa que `Show With Limited Tracking` esté activado
- Asegúrate de que el panel no esté detrás del marcador (ajusta `localPosition.y`)

---

## ?? BUILD SETTINGS PARA ANDROID

Asegúrate de tener configurado:

1. **Player Settings:**
   - Minimum API Level: **Android 7.0 (API 24)** o superior
   - Internet Access: **Required**
   - Scripting Backend: **IL2CPP**
   - Target Architectures: **ARM64** ?

2. **XR Plug-in Management:**
   - ARCore: ?

---

## ?? PERSONALIZAR EL PANEL

### **Modificar el diseño del panel:**

1. Abre el prefab `InfoPanel.prefab`
2. Modifica el Canvas/BackgroundPanel:
   - Cambia colores, tamaños, fuentes
   - Agrega nuevos elementos (imágenes, botones)
3. Actualiza `InfoPanelController.cs` si agregas nuevos campos

### **Ajustar posición del panel:**

En `InfoPanel.prefab`, modifica la posición Y del GameObject raíz:
- Actual: `(0, 0.15, 0)` - 15cm arriba del marcador
- Puedes ajustar según necesites

---

## ?? LOGS ÚTILES

El sistema genera logs detallados en consola:

- ?? Firebase: `[FirebaseManager]`
- ?? AR Tracking: `[MultiImageSpawner]`
- ?? UI: `[InfoPanelController]`

**Ejemplo de ejecución exitosa:**
```
[FirebaseManager] ?? Inicializando Firebase...
[FirebaseManager] ? Firebase inicializado correctamente
[MultiImageSpawner] ?? Modo Firebase ACTIVADO
[MultiImageSpawner] ?? Imagen DETECTADA: '8YLwj6VOhzT2KPzZDMF9'
[MultiImageSpawner] ? Consultando Firebase para '8YLwj6VOhzT2KPzZDMF9'...
[FirebaseManager] ?? Consultando Firestore: buildingLocations/8YLwj6VOhzT2KPzZDMF9
[FirebaseManager] ? Datos obtenidos exitosamente: 'Biblioteca Central'
[InfoPanelController] Configurando panel con: Biblioteca Central
[MultiImageSpawner] ? Panel actualizado con datos de 'Biblioteca Central'
```

---

## ?? PRÓXIMOS PASOS (OPCIONAL)

### **Mejoras futuras:**
- [ ] Agregar botón para abrir Google Maps con las coordenadas
- [ ] Mostrar imágenes desde URLs (requiere `UnityWebRequestTexture`)
- [ ] Implementar caché persistente con `PlayerPrefs` para modo offline
- [ ] Agregar animación de rotación al panel (ya tiene billboard opcional)
- [ ] Sistema de favoritos guardados localmente

---

## ?? SOPORTE

Si tienes problemas:
1. Revisa los logs en consola/Logcat
2. Verifica que los nombres en Reference Image Library coincidan exactamente con Firestore
3. Prueba primero en modo "Simulate Data In Editor"

---

**¡Todo listo! ??**

Tu sistema AR ahora consulta Firebase Firestore automáticamente y muestra información dinámica sobre cada patrón detectado.
