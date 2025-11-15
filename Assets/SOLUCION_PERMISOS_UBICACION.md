# ?? SOLUCIÓN: Permisos de Ubicación No Se Piden

## ?? PROBLEMA IDENTIFICADO

La app **NO pide permisos de ubicación** al usuario cuando inicia, solo pide permisos de cámara.

**Resultado:**
- ? GPS nunca inicializa
- ? `Input.location.isEnabledByUser` es **false**
- ? La flecha no puede calcular dirección
- ? Siempre apunta hacia adelante

---

## ? SOLUCIÓN

He creado un **sistema de permisos en runtime** que pide correctamente todos los permisos necesarios.

### **Archivo Creado:**
- ? **`PermissionsManager.cs`** - Gestor de permisos para Android

---

## ?? CÓMO IMPLEMENTAR

### **Paso 1: Añadir PermissionsManager a la Escena**

1. En Unity, **jerarquía** ? Click derecho ? **Create Empty**
2. Nombrar: **"PermissionsManager"**
3. **Add Component** ? Busca **`PermissionsManager`**
4. En el Inspector:
   ```
   ? Request On Start: TRUE
   ? Show Rationale: TRUE
   ```

5. **Importante:** Debe estar **ANTES** que `LocationManager` en la jerarquía o con **Script Execution Order** configurado

---

### **Paso 2: Configurar Script Execution Order**

Para asegurar que los permisos se pidan **ANTES** de inicializar GPS:

1. **Edit** ? **Project Settings** ? **Script Execution Order**
2. Click **+** (añadir)
3. Selecciona **`PermissionsManager`**
4. Valor: **-100** (se ejecuta primero)
5. Click **+** de nuevo
6. Selecciona **`LocationManager`**
7. Valor: **0** (se ejecuta después)
8. **Apply**

Esto asegura que `PermissionsManager` pida permisos **ANTES** de que `LocationManager` intente inicializar GPS.

---

### **Paso 3: Build & Run**

1. **File** ? **Build Settings**
2. **Build And Run**
3. Instala en tu dispositivo

---

## ?? QUÉ VERÁS AL INICIAR LA APP

### **Primera Vez que Abres la App:**

1. **Diálogo de la app aparece:**
```
???????????????????????????????????
? Permisos Necesarios             ?
???????????????????????????????????
? Esta app necesita:              ?
?                                 ?
? ?? UBICACIÓN:                   ?
? Para navegación AR y mostrar    ?
? tu posición                     ?
?                                 ?
? ?? CÁMARA:                      ?
? Para detectar marcadores AR     ?
?                                 ?
? ¿Conceder permisos?             ?
???????????????????????????????????
?  [Cancelar]       [Aceptar]     ?
???????????????????????????????????
```

2. **Click "Aceptar"**

3. **Diálogo de Android aparece (Ubicación):**
```
???????????????????????????????????
? ¿Permitir que ArcProject        ?
? acceda a la ubicación de este   ?
? dispositivo?                    ?
???????????????????????????????????
? ? Permitir solo mientras se usa?
? ? Permitir siempre              ?
? ? No permitir                   ?
???????????????????????????????????
```

4. **Selecciona**: **"Permitir siempre"** o **"Permitir solo mientras se usa"**

5. **Diálogo de Android aparece (Cámara):**
```
???????????????????????????????????
? ¿Permitir que ArcProject        ?
? acceda a la cámara?             ?
???????????????????????????????????
?    [Denegar]    [Permitir]      ?
???????????????????????????????????
```

6. **Click "Permitir"**

7. **La app continúa y GPS inicializa** ?

---

## ?? LOGS ESPERADOS

### **En Logcat, después de conceder permisos:**

```
[PermissionsManager] ?? Verificando permisos...
[PermissionsManager] ?? Ubicación (Fine): False
[PermissionsManager] ?? Ubicación (Coarse): False
[PermissionsManager] ?? Cámara: True
[PermissionsManager] ?? Pidiendo permisos al usuario...
[PermissionsManager] ?? Pidiendo permisos:
   - android.permission.ACCESS_FINE_LOCATION
   - android.permission.ACCESS_COARSE_LOCATION
   - android.permission.CAMERA
[PermissionsManager] ?? Resultado de permisos:
[PermissionsManager] ? Concedido: android.permission.ACCESS_FINE_LOCATION
[PermissionsManager] ? Concedido: android.permission.ACCESS_COARSE_LOCATION
[PermissionsManager] ? Concedido: android.permission.CAMERA
[PermissionsManager] ? Todos los permisos concedidos!
[PermissionsManager] ?? Reiniciando LocationManager...
[LocationManager] ?? Reinicializando GPS después de obtener permisos...
[LocationManager] ?? Inicializando servicios de ubicación...
[LocationManager] ? GPS está habilitado por el usuario
[LocationManager] ?? Iniciando servicio GPS...
[LocationManager] ? Inicializando GPS... 1/20
[LocationManager] ? Inicializando GPS... 2/20
...
[LocationManager] ? GPS inicializado correctamente
[LocationManager] ?? Ubicación inicial: 13.718103, -89.204092
```

---

## ?? CONFIGURACIÓN ADICIONAL

### **Opción 1: Sin Diálogo Explicativo**

Si quieres que pida permisos **directamente** sin mostrar explicación:

1. Selecciona `PermissionsManager` en la jerarquía
2. Inspector:
   ```
   ? Request On Start: TRUE
   ? Show Rationale: FALSE  ? Desactivar
   ```

### **Opción 2: Pedir Permisos Manualmente**

Si quieres pedir permisos **solo cuando el usuario hace algo específico** (ej: al presionar "Navegar"):

1. En el Inspector:
   ```
   ? Request On Start: FALSE  ? Desactivar
   ? Show Rationale: TRUE
   ```

2. En tu código, llama:
   ```csharp
   PermissionsManager pm = FindObjectOfType<PermissionsManager>();
   if (pm != null)
   {
       pm.CheckAndRequestPermissions();
   }
   ```

---

## ?? SI EL USUARIO DENIEGA PERMISOS

Si el usuario hace click en **"No permitir"** o **"Denegar"**:

1. **Toast aparece:**
```
"Permisos necesarios para usar la app. 
Ve a Configuración para activarlos."
```

2. **Para activarlos manualmente:**
   - Configuración ? Apps ? ArcProject
   - Permisos ? Ubicación ? Permitir
   - Reiniciar la app

---

## ?? TROUBLESHOOTING

### **? Los permisos no se piden:**

**Causa:** `PermissionsManager` no está en la escena o está desactivado.

**Solución:**
- Verifica que el GameObject `PermissionsManager` existe
- Verifica que el componente está **enabled**
- Verifica que **Request On Start = TRUE**

---

### **? Se piden permisos pero GPS no inicializa:**

**Causa:** `LocationManager` intenta inicializar **antes** de que se concedan permisos.

**Solución:**
- Configura **Script Execution Order** (ver Paso 2 arriba)
- O mueve `PermissionsManager` **arriba** de `LocationManager` en la jerarquía

---

### **? Error: "AndroidJavaObject not found":**

**Causa:** Código Android ejecutándose en Unity Editor.

**Solución:**
- Es normal, el código está protegido con `#if PLATFORM_ANDROID`
- Solo funciona en build de Android
- En Editor, asume que los permisos están concedidos

---

### **? La app crashea al pedir permisos:**

**Causa:** Posible problema con la versión de Android o Unity.

**Solución:**
- Verifica que `Minimum API Level` sea **Android 6.0 (API 23)** o superior
- `Edit` ? `Project Settings` ? `Player` ? `Android` ? `Minimum API Level`

---

## ?? FUNCIONALIDADES ADICIONALES

### **Callbacks Personalizados:**

Puedes ejecutar código cuando se conceden/deniegan permisos:

```csharp
PermissionsManager pm = FindObjectOfType<PermissionsManager>();

pm.OnAllPermissionsGranted += () => {
    Debug.Log("¡Todos los permisos concedidos!");
    // Tu código aquí
};

pm.OnPermissionsDenied += () => {
    Debug.LogWarning("Permisos denegados");
    // Mostrar mensaje al usuario
};
```

### **Verificar Estado de Permisos:**

```csharp
PermissionsManager pm = FindObjectOfType<PermissionsManager>();

if (pm.HasAllPermissions)
{
    // Iniciar navegación
}
else
{
    // Pedir permisos
    pm.CheckAndRequestPermissions();
}
```

---

## ? RESULTADO FINAL

Después de implementar esto:

```
? La app pide permisos de UBICACIÓN al iniciar
? La app pide permisos de CÁMARA al iniciar
? Usuario puede conceder/denegar cada uno
? GPS inicializa DESPUÉS de conceder permisos
? Flecha puede calcular dirección correcta
? Navegación AR funciona perfectamente
```

---

## ?? ARCHIVOS CREADOS/MODIFICADOS

- ? **`PermissionsManager.cs`** (nuevo) - Gestor de permisos
- ? **`LocationManager.cs`** (modificado) - Añadido `ReinitializeLocation()`
- ? **`SOLUCION_PERMISOS_UBICACION.md`** (este documento)

---

## ?? EXPLICACIÓN TÉCNICA

### **¿Por qué antes no pedía permisos?**

Unity **automáticamente** pide permisos de cámara porque usa ARCore, pero **NO pide automáticamente** permisos de ubicación porque `Input.location` no lo hace por defecto.

### **¿Qué hace PermissionsManager?**

1. Al iniciar, verifica qué permisos tiene la app
2. Si faltan permisos, usa la API de Android (`Permission.RequestUserPermissions`)
3. Muestra diálogos nativos de Android
4. Recibe la respuesta del usuario
5. Si se conceden permisos, reinicia `LocationManager`
6. GPS ahora puede inicializar correctamente

---

**¡Con esto, la app debería pedir permisos de ubicación y el GPS debería funcionar!** ??
