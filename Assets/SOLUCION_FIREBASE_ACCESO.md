# ?? SOLUCIÓN: Firebase conecta pero no carga datos

## ? Verificado
- ? El documento **SÍ existe** en Firebase: `N0UxoKqwo98gRg3cuZUZ`
- ? El documento tiene datos correctos
- ? El panel funciona (se ve sólido)
- ? La app detecta el marcador

## ? Problema
La app no puede **leer** los datos desde Firestore.

---

## ?? SOLUCIÓN: Verificar Reglas de Firestore

### **Paso 1: Verificar Reglas de Seguridad**

1. **Firebase Console** ? Tu proyecto
2. **Firestore Database** (en el menú lateral)
3. Click en la pestaña: **"Reglas"** (Rules)

### **Paso 2: Verificar las Reglas Actuales**

Deberías ver algo como esto:

#### ? **Si tus reglas son restrictivas:**
```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /{document=**} {
      allow read, write: if false; // ? BLOQUEA TODO
    }
  }
}
```

#### ? **Cambiar a modo desarrollo (temporalmente):**
```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /{document=**} {
      allow read, write: if true; // ? PERMITE TODO (solo para desarrollo)
    }
  }
}
```

### **Paso 3: Publicar las Reglas**
1. Click en: **"Publicar"** (Publish)
2. Espera unos segundos a que se apliquen

### **Paso 4: Probar de Nuevo**
1. Cierra la app en el dispositivo
2. Vuelve a abrirla
3. Apunta al marcador
4. Debería cargar los datos

---

## ?? Reglas Recomendadas para Producción

Una vez que funcione, cambia a reglas más seguras:

```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    // Permitir lectura pública, escritura solo autenticada
    match /buildingLocations/{document} {
      allow read: if true;  // Cualquiera puede leer
      allow write: if request.auth != null;  // Solo usuarios autenticados pueden escribir
    }
  }
}
```

---

## ?? Verificar google-services.json

### **Verificación 1: Archivo existe**
1. Verifica que existe: `Assets/google-services.json`
2. NO debe estar en subcarpetas
3. Debe estar directamente en `Assets/`

### **Verificación 2: Es el archivo correcto**
1. Abre `google-services.json` con un editor de texto
2. Busca: `"project_id"`
3. Verifica que sea el ID de tu proyecto Firebase

**Ejemplo:**
```json
{
  "project_info": {
    "project_number": "123456789",
    "project_id": "tu-proyecto-firebase",  // ? Debe coincidir
    "storage_bucket": "..."
  },
  ...
}
```

### **Verificación 3: Package Name coincide**
1. En `google-services.json`, busca:
```json
"client": [
  {
    "client_info": {
      ...
      "android_client_info": {
        "package_name": "com.tuempresa.arcproject"  // ? Este debe coincidir
      }
    }
  }
]
```

2. En Unity: **Edit ? Project Settings ? Player ? Android**
3. Verifica que **Package Name** sea exactamente el mismo

---

## ?? Verificar Permisos de Internet en Android

### **Paso 1: Verificar AndroidManifest.xml**

Si tienes un archivo `Assets/Plugins/Android/AndroidManifest.xml`, verifica que tenga:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

### **Paso 2: Si no tienes AndroidManifest.xml**

No te preocupes, Unity lo genera automáticamente con los permisos correctos.

---

## ?? Verificar Logs de Firebase

Para ver qué está pasando exactamente:

### **Opción 1: Android Logcat en Unity**

1. **Window ? Analysis ? Android Logcat**
2. Conecta el dispositivo por USB
3. Filtra por: `FirebaseManager`
4. Busca estos mensajes:

**Si Firebase no se inicializó:**
```
? Error al inicializar Firebase: [código de error]
```

**Si hay problema de permisos:**
```
? Error al consultar Firestore: PERMISSION_DENIED
```

**Si el documento no se encuentra:**
```
?? Documento 'N0UxoKqwo98gRg3cuZUZ' NO EXISTE en Firestore
```

### **Opción 2: Logs desde el Dispositivo**

1. Conecta por USB
2. Abre una terminal/PowerShell
3. Ejecuta:
```powershell
adb logcat | Select-String "FirebaseManager"
```

---

## ?? Modo de Prueba Forzado

Para verificar que el panel funciona sin Firebase:

### **Paso 1: Activar Simulación**

1. En Unity, selecciona el GameObject con `FirebaseManager`
2. En el Inspector:
   - ? Marca: **Simulate Data In Editor**

### **Paso 2: Build and Run**

Ahora la app mostrará datos de prueba sin consultar Firebase.

Si funciona con simulación pero no con Firebase real, el problema es definitivamente de conexión/permisos.

---

## ?? Pasos de Resolución en Orden

Sigue estos pasos en orden:

### ? **Paso 1: Verificar Reglas de Firestore**
- Firebase Console ? Firestore ? Reglas
- Cambiar a `allow read, write: if true;` temporalmente
- Publicar

### ? **Paso 2: Verificar google-services.json**
- Ubicación: `Assets/google-services.json`
- `project_id` correcto
- `package_name` coincide con Unity

### ? **Paso 3: Verificar Package Name**
- Unity ? Player Settings ? Package Name
- Debe coincidir con google-services.json

### ? **Paso 4: Rebuild**
- Guarda todo: Ctrl+S
- File ? Build and Run
- Espera a que se instale

### ? **Paso 5: Revisar Logs**
- Android Logcat
- Filtrar por `FirebaseManager`
- Ver qué error específico aparece

---

## ?? Tabla de Diagnóstico

| Síntoma | Causa Probable | Solución |
|---------|----------------|----------|
| "Información no disponible" | Reglas de Firestore restrictivas | Cambiar reglas a `allow read: if true` |
| "Verifica tu conexión" | google-services.json incorrecto | Descargar nuevo desde Firebase Console |
| Panel se queda en "Cargando..." | Firebase no se inicializó | Verificar logs de inicialización |
| Error PERMISSION_DENIED | Reglas de Firestore | Cambiar reglas |
| Error 404 / NOT_FOUND | Document ID no coincide | Verificar nombre exacto del marcador |

---

## ? Checklist Completo

Verifica TODOS estos puntos:

- [ ] Reglas de Firestore: `allow read: if true`
- [ ] `google-services.json` en `Assets/` (no en subcarpetas)
- [ ] `project_id` en google-services.json es correcto
- [ ] Package Name en Unity coincide con google-services.json
- [ ] Dispositivo tiene conexión a internet
- [ ] Firebase está habilitado en el proyecto
- [ ] La colección se llama: `buildingLocations`
- [ ] El documento existe: `N0UxoKqwo98gRg3cuZUZ`
- [ ] El marcador en Unity se llama exactamente igual

---

## ?? Solución Más Probable

Basándome en tu caso específico, la causa MÁS PROBABLE es:

**Las reglas de Firestore están bloqueando las lecturas.**

**Solución:**
1. Firebase Console ? Firestore Database ? Reglas
2. Cambiar a:
```javascript
rules_version = '2';
service cloud.firestore {
  match /databases/{database}/documents {
    match /{document=**} {
      allow read: if true;
      allow write: if request.auth != null;
    }
  }
}
```
3. Publicar
4. Reiniciar la app

---

## ?? Mensaje Temporal

Si quieres un mensaje más descriptivo mientras arreglas Firebase, puedo modificar `BuildingData.cs` para mostrar el error específico en lugar de "Información no disponible".

¿Quieres que haga ese cambio?

---

**¡El documento existe! Solo necesitamos arreglar el acceso a Firebase.** ??
