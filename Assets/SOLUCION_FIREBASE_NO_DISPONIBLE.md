# ?? SOLUCIÓN: "Información no disponible" - Firebase

## ? Problema Actual

El panel muestra:
```
Información no disponible
No se pudo cargar la información para el marcador:
N0UxoKqwo98gRg3cuZUZ

Verifica tu conexión a internet.
```

**Causa:** El documento `N0UxoKqwo98gRg3cuZUZ` **NO existe** en tu base de datos Firestore.

---

## ? SOLUCIÓN: 2 Opciones

### **Opción 1: Crear el Documento en Firebase (RÁPIDO)**

#### **Paso 1: Ve a Firebase Console**
1. Abre: https://console.firebase.google.com/
2. Selecciona tu proyecto
3. En el menú lateral: **Firestore Database**

#### **Paso 2: Crear la colección (si no existe)**
1. Si no existe la colección `buildingLocations`, créala:
   - Click en: **"Start collection"**
   - Collection ID: `buildingLocations`

#### **Paso 3: Crear el documento**
1. Click en: **"Add document"**
2. **Document ID:** `N0UxoKqwo98gRg3cuZUZ` (exactamente igual, ¡sin espacios!)
3. Agrega estos campos:

| Field name | Field type | Field value |
|------------|-----------|-------------|
| `name` | string | `"Edificio de Prueba"` |
| `description` | string | `"Este es un edificio de prueba para validar el sistema AR con Firebase Firestore."` |
| `latitude` | number | `13.7181033` |
| `longitude` | number | `-89.2040915` |

4. Click en: **"Save"**

#### **Paso 4: Probar**
1. Vuelve a la app en tu dispositivo
2. Apunta al marcador
3. Ahora **SÍ** debería cargar los datos

---

### **Opción 2: Usar IDs que Ya Existen en Firebase**

Si ya tienes documentos en Firebase con otros IDs:

#### **Paso 1: Ver qué IDs tienes**
1. Ve a Firebase Console ? Firestore Database
2. Abre la colección `buildingLocations`
3. Anota los IDs de los documentos que existen

Ejemplo:
```
buildingLocations/
?? Ki03NOqDgcIIUkIelNnF/
?? UrvKdEj96fv3VKVQxk2H/
?? Yub2zI6C8A0pxtHzbFKk/
?? ZOYQRDxBUw76kLfvQFkL/
```

#### **Paso 2: Generar marcadores con esos nombres**

En Unity:
1. **Unity ? AR Tools ? Generar Marcador de Prueba**
2. **Nombre del Marcador:** `Ki03NOqDgcIIUkIelNnF` (el ID exacto de Firebase)
3. **Resolución:** `512`
4. Click: **"Generar Marcador"**
5. Repite para cada ID que tengas en Firebase

#### **Paso 3: Reemplazar en Reference Image Library**

1. Abre: `Assets/Markers/ReferenceImageLibrary`
2. Elimina el marcador actual
3. Click: **"Add Image"**
4. Arrastra los marcadores generados desde `Assets/Markers/Generated/`
5. **Importante:** El **Name** debe coincidir exactamente con el ID de Firebase
6. Especifica tamaño: **0.2 x 0.2**

#### **Paso 4: Build and Run**
Ahora al detectar los marcadores, Firebase encontrará los documentos.

---

## ?? Verificar Conexión Firebase

### **Verificar google-services.json**

1. Verifica que existe: `Assets/google-services.json`
2. Debe ser el archivo descargado de tu proyecto Firebase
3. Si no lo tienes:
   - Firebase Console ? Project Settings ? General
   - Scroll hasta "Your apps"
   - Click en el ícono de Android
   - Download `google-services.json`
   - Colócalo en `Assets/`

### **Verificar Firestore está habilitado**

1. Firebase Console ? Firestore Database
2. Si ves "Create database", significa que Firestore NO está activado
3. Click en "Create database"
4. Elige modo: **"Start in test mode"** (para desarrollo)
5. Location: Elige la más cercana
6. Click: "Enable"

---

## ?? Estructura Correcta en Firestore

Tu base de datos debe verse así:

```
buildingLocations (collection)
?
?? N0UxoKqwo98gRg3cuZUZ (document) ? El ID debe coincidir con el nombre del marcador
?  ?? name: "Edificio de Prueba"
?  ?? description: "Descripción..."
?  ?? latitude: 13.7181033
?  ?? longitude: -89.2040915
?
?? Ki03NOqDgcIIUkIelNnF (document)
?  ?? name: "Biblioteca Central"
?  ?? description: "..."
?  ?? latitude: ...
?  ?? longitude: ...
?
?? ... más documentos
```

**?? IMPORTANTE:**
- El **Document ID** en Firestore DEBE ser exactamente igual al **Name** del marcador en Reference Image Library
- Son **case-sensitive** (mayúsculas y minúsculas importan)
- Sin espacios adicionales

---

## ?? Modo de Prueba (Sin Firebase)

Si quieres probar sin conectar a Firebase:

1. Abre: `Assets/Scripts/FirebaseManager.cs`
2. En el Inspector (cuando selecciones el GameObject con FirebaseManager):
   - Marca: ? **Simulate Data In Editor**
3. Esto retornará datos de prueba sin consultar Firebase

---

## ?? Checklist de Verificación

Verifica estos puntos:

- [ ] `google-services.json` está en `Assets/`
- [ ] Firestore está habilitado en Firebase Console
- [ ] La colección se llama exactamente: `buildingLocations`
- [ ] El documento existe con ID: `N0UxoKqwo98gRg3cuZUZ`
- [ ] El marcador en Unity se llama exactamente igual (sin espacios)
- [ ] Los campos `name`, `description`, `latitude`, `longitude` existen

---

## ?? Verificar en Logcat

Para ver qué está pasando:

1. Conecta el dispositivo por USB
2. **Window ? Analysis ? Android Logcat**
3. Filtra por: `FirebaseManager`
4. Busca estos mensajes:

**Si Firebase no se inicializó:**
```
? Error al inicializar Firebase: ...
```

**Si el documento no existe:**
```
?? Documento 'N0UxoKqwo98gRg3cuZUZ' NO EXISTE en Firestore
```

**Si todo está bien:**
```
? Datos obtenidos exitosamente: 'Edificio de Prueba'
```

---

## ?? Resumen Rápido

**El problema es simple:**
El marcador `N0UxoKqwo98gRg3cuZUZ` existe en Unity, pero **NO existe en Firebase**.

**Solución más rápida:**
1. Ve a Firebase Console
2. Crea el documento con ese ID
3. Agrega los campos necesarios
4. Vuelve a probar

**O:**
1. Usa IDs que ya existen en Firebase
2. Genera marcadores con esos nombres

---

## ? Después de Crear el Documento

Una vez que crees el documento en Firebase:

1. NO necesitas hacer rebuild
2. Solo cierra y abre la app en el dispositivo
3. Apunta al marcador
4. Debería cargar los datos correctamente

---

**El panel YA funciona correctamente (se ve sólido). Solo falta sincronizar los IDs con Firebase.** ??
