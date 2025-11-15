# ??? GUÍA: Solución de Errores de Compilación

## ?? Errores Corregidos (2025-01-07)

? `LocationInfo.course` - Eliminado  
? `LocationInfo.speed` - Eliminado  

**Estado:** Proyecto debería compilar sin errores ahora.

---

## ?? Si Aún Ves Errores

### **Paso 1: Limpiar Caché de Unity**

Unity a veces mantiene archivos en caché que causan problemas:

```
1. Cerrar Unity completamente
2. Ir a la carpeta del proyecto
3. Eliminar estas carpetas:
   - Library/
   - Temp/
   - obj/
   - .vs/ (si existe)
4. Reabrir Unity
5. Esperar a que reimporte todo
6. Intentar compilar de nuevo
```

---

### **Paso 2: Verificar Version de Unity**

Este proyecto usa **Unity 2022.3.62f2**.

**Si usas otra versión:**
- Puede haber incompatibilidades de API
- Considera usar la misma versión
- O adaptar el código a tu versión

**Verificar versión:**
```
Unity Hub ? Projects ? [ArcProject] ? Ver versión
```

---

### **Paso 3: Verificar Paquetes Instalados**

**Paquetes necesarios:**
```
? AR Foundation 5.2.0+
? ARCore XR Plugin 5.2.0+
? XR Plugin Management
? Firebase SDK (opcional)
? TextMeshPro
```

**Cómo verificar:**
```
Window ? Package Manager
```

---

## ?? Tipos de Errores Comunes

### **Error Tipo 1: API Inexistente**

```
CS1061: 'SomeType' does not contain a definition for 'someProperty'
```

**Causa:** Intentas acceder a una propiedad/método que no existe.

**Solución:**
1. Verificar documentación de Unity para tu versión
2. Buscar alternativas
3. Eliminar o comentar el código problemático

**Ejemplo de este proyecto:**
```csharp
// ? NO existe en Unity estándar
float course = Input.location.lastData.course;

// ? Alternativa: usar brújula
float heading = Input.compass.trueHeading;
```

---

### **Error Tipo 2: Namespace Faltante**

```
CS0246: The type or namespace name 'SomeClass' could not be found
```

**Causa:** Falta un `using` statement.

**Solución:**
```csharp
// Agregar al inicio del archivo
using UnityEngine;
using System.Collections;
using TMPro; // Para TextMeshPro
```

**Namespaces comunes:**
```csharp
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARCore;
using Firebase;
using Firebase.Database;
```

---

### **Error Tipo 3: Referencia Null**

```
CS0103: The name 'variableName' does not exist in the current context
```

**Causa:** Variable no declarada o mal escrita.

**Solución:**
1. Verificar declaración de variable
2. Revisar typos (mayúsculas/minúsculas)
3. Verificar scope (privado/público)

---

### **Error Tipo 4: Incompatibilidad de Tipos**

```
CS0029: Cannot implicitly convert type 'X' to 'Y'
```

**Causa:** Intentas asignar un tipo a otro incompatible.

**Solución:**
```csharp
// ? Error
float value = "123"; 

// ? Corregido
float value = float.Parse("123");
```

---

## ?? Cómo Reportar Errores

Si encuentras un error que no puedes solucionar:

### **Información Necesaria:**

1. **Mensaje de error completo:**
   ```
   [Línea] [Archivo] error CS####: Mensaje completo del error
   ```

2. **Archivo afectado:**
   ```
   Assets/Scripts/NombreDelScript.cs
   ```

3. **Línea del error:**
   ```
   Línea 123, columna 45
   ```

4. **Código alrededor del error:**
   ```csharp
   // 3-5 líneas antes
   // ... línea con error ...
   // 3-5 líneas después
   ```

5. **Versión de Unity:**
   ```
   Unity 2022.3.62f2
   ```

---

## ?? Soluciones Rápidas

### **1. Errores de Input System**

Si ves errores relacionados con Input System:

```
Assets ? Create ? Input Actions
```

O desactiva el nuevo Input System:
```
Edit ? Project Settings ? Player ? Other Settings
Active Input Handling: Input Manager (Old)
```

---

### **2. Errores de AR Foundation**

Si AR Foundation da problemas:

```
Window ? Package Manager ? AR Foundation ? Reinstalar
```

También verifica:
```
Edit ? Project Settings ? XR Plug-in Management
? ARCore (Android)
```

---

### **3. Errores de TextMeshPro**

Si faltan referencias de TMP:

```
Window ? TextMeshPro ? Import TMP Essential Resources
```

---

### **4. Errores de Firebase**

Si Firebase da problemas:

```
1. Eliminar carpetas:
   - Assets/ExternalDependencyManager
   - Assets/Firebase
   - Assets/Plugins/Android

2. Reimportar Firebase SDK

3. Agregar google-services.json

4. Resolver dependencias:
   Assets ? External Dependency Manager ? Android Resolver ? Resolve
```

---

## ?? Testing de Compilación

### **Compilar en Unity Editor:**

```
File ? Build Settings
Platform: Android
? Switch Platform
Build
```

**Resultado esperado:** ? Build successful

---

### **Compilar desde Línea de Comandos:**

```bash
# Windows
"C:\Program Files\Unity\Hub\Editor\2022.3.62f2\Editor\Unity.exe" ^
  -quit ^
  -batchmode ^
  -projectPath "C:\path\to\ArcProject" ^
  -buildTarget Android ^
  -executeMethod BuildScript.Build ^
  -logFile build.log

# Ver errores
type build.log | findstr "error"
```

---

## ?? Recursos Útiles

### **Documentación Oficial:**

- [Unity Scripting API](https://docs.unity3d.com/ScriptReference/)
- [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/index.html)
- [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html)

### **Errores Comunes:**

- [Unity Forum](https://forum.unity.com/)
- [Stack Overflow - Unity Tag](https://stackoverflow.com/questions/tagged/unity3d)

---

## ?? Checklist de Verificación

Antes de compilar, verifica:

- [ ] Unity versión correcta (2022.3.62f2)
- [ ] Paquetes instalados (AR Foundation, ARCore)
- [ ] Caché limpiada (si hubo cambios grandes)
- [ ] Scripts sin errores en consola
- [ ] Build Settings configurados (Android)
- [ ] Platform switched a Android
- [ ] Minimum API Level: 24 (Android 7.0)
- [ ] Target API Level: 30+ (Android 11+)

---

## ?? Última Opción: Rollback

Si nada funciona, puedes volver a una versión anterior:

```bash
# Si usas Git
git log --oneline  # Ver commits
git checkout <commit-hash>  # Volver a versión anterior
```

**Commits importantes:**
- Antes de errores de `LocationInfo`
- Última versión estable conocida

---

## ?? Contacto

Si después de seguir esta guía sigues con errores:

1. **Revisar documentación del proyecto:**
   - [INDICE_DOCUMENTACION_BRUJULA.md](./INDICE_DOCUMENTACION_BRUJULA.md)
   - [RESUMEN_CORRECCION_ERRORES.md](./RESUMEN_CORRECCION_ERRORES.md)

2. **Compartir información completa del error** (ver formato arriba)

3. **Indicar qué intentaste hacer** para solucionarlo

---

**¡Buena suerte! ??**
