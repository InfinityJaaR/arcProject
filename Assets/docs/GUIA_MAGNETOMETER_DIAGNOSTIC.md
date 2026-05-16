# ?? HERRAMIENTA: Diagnóstico de Magnetómetro

## ?? Descripción

`MagnetometerDiagnostic.cs` es una herramienta de debugging que verifica:

? Si el dispositivo tiene magnetómetro (sensor físico)  
? Si Unity puede acceder a la brújula  
? Si los permisos están correctos  
? Estado del GPS  
? Valores actuales de la brújula  

---

## ??? Cómo Usar

### **Paso 1: Agregar el Script a la Escena**

1. En Unity, selecciona cualquier GameObject (ej: Main Camera)
2. Add Component ? `MagnetometerDiagnostic`
3. Configurar opciones:
   - ? **Show On Screen Debug**: activado (muestra panel en pantalla)
   - Ajustar posición/tamaño del panel si es necesario

### **Paso 2: Compilar y Ejecutar en Android**

```bash
# Build Settings
File ? Build Settings
Platform: Android
Build
```

### **Paso 3: Ver Resultados**

#### **En Pantalla (Dispositivo):**

Verás un panel como este:

```
?? DIAGNÓSTICO MAGNETÓMETRO

?? PLATAFORMA
  Sistema: Android OS 13 / API-33
  Dispositivo: Samsung SM-A556E
  Unity: 2022.3.10f1

?? MAGNETÓMETRO
  Disponible: ? SÍ

?? INPUT.COMPASS
  Enabled: True
  Timestamp: 1234567.89
  ? Sensor proporcionando datos
  TrueHeading: 87.34°
  MagneticHeading: 90.12°
  Accuracy: 15.0°
  RawVector: (23.4, -45.2, 18.9)

?? GPS
  Enabled by User: True
  Status: Running
  ? GPS funcionando
  Lat: 13.718103
  Lon: -89.204092
  Accuracy: 12.3m

?? OTROS SENSORES
  Gyroscope: ?
  Accelerometer: ?
  Vibration: ?

?? PERMISOS
  Fine Location: ?
  Coarse Location: ?

?? RECOMENDACIONES
  ? Brújula funcionando correctamente

[?? Actualizar Diagnóstico]
```

#### **En Logs (ADB):**

```bash
# Conectar dispositivo y ejecutar
adb logcat -s Unity | grep MagnetometerDiagnostic
```

Verás:

```
[MagnetometerDiagnostic] ?? INICIANDO DIAGNÓSTICO
[MagnetometerDiagnostic] Plataforma: Android OS 13
[MagnetometerDiagnostic] ? Magnetómetro encontrado:
  Nombre: AK09973 Magnetic field Sensor
  Fabricante: AKM
  Rango máximo: 4900.0 µT
[MagnetometerDiagnostic] Compass enabled: True
[MagnetometerDiagnostic] Timestamp: 1234567.89
[MagnetometerDiagnostic] TrueHeading: 87.34°
```

---

## ?? Interpretación de Resultados

### ? **TODO BIEN:**

```
?? MAGNETÓMETRO
  Disponible: ? SÍ

?? INPUT.COMPASS
  Timestamp: 1234567.89  ? Número positivo
  ? Sensor proporcionando datos
  TrueHeading: 87.34°    ? Valor realista
```

**Acción:** Ninguna, la brújula funciona correctamente.

---

### ? **PROBLEMA: Sin Magnetómetro**

```
?? MAGNETÓMETRO
  Disponible: ? NO
  ?? Este dispositivo NO tiene sensor de brújula
```

**Causa:** El dispositivo NO tiene hardware de magnetómetro.

**Solución:**
- Imposible usar brújula en ese dispositivo
- Activa `useGPSBearingFallback` en `LocationManager`
- El usuario debe moverse para que funcione

```csharp
// En LocationManager
public bool useGPSBearingFallback = true; // ? Activar
```

---

### ?? **PROBLEMA: Brújula Sin Datos**

```
?? MAGNETÓMETRO
  Disponible: ? SÍ  ? Sensor existe

?? INPUT.COMPASS
  Timestamp: -1      ? ? Sin datos
  ?? Sin datos del sensor
  TrueHeading: 0.0°
```

**Causa:** Sensor no inicializado o interferencia magnética.

**Solución:**
1. Calibrar la brújula (mover en figura de 8)
2. Alejarse de objetos metálicos
3. Presionar botón "?? Reintentar Brújula" en el panel
4. Salir al exterior

---

### ?? **PROBLEMA: Sin Permisos**

```
?? PERMISOS
  Fine Location: ?
  Coarse Location: ?
```

**Causa:** Permisos no otorgados.

**Solución:**
1. Verificar `AndroidManifest.xml`
2. Usar `PermissionsManager` para solicitar permisos
3. Reiniciar la app después de otorgar permisos

---

### ?? **PROBLEMA: GPS No Funciona**

```
?? GPS
  Status: Stopped
  ?? GPS no está funcionando
```

**Causa:** GPS no inicializado o deshabilitado.

**Solución:**
- Verificar `LocationManager` está en la escena
- Verificar permisos de ubicación
- Habilitar ubicación en configuración del teléfono

---

## ??? Opciones del Script

### **Variables Públicas:**

```csharp
public bool showOnScreenDebug = true;
// Mostrar panel en pantalla del dispositivo

public Vector2 debugPanelPosition = new Vector2(10, 10);
// Posición del panel (pixeles desde esquina superior izquierda)

public Vector2 debugPanelSize = new Vector2(500, 400);
// Tamaño del panel en pixeles
```

### **Métodos Públicos:**

```csharp
// Ejecutar diagnóstico manualmente
GetComponent<MagnetometerDiagnostic>().RunDiagnostic();
```

---

## ?? Valores Típicos de Referencia

### **Campo Magnético Terrestre:**

```
RawVector típico:
  Magnitud: 25-65 µT (microteslas)
  Dirección: varía según orientación del dispositivo

Ejemplo:
  En El Salvador, apuntando Norte: (0, 40, -20) aprox.
  En El Salvador, apuntando Este: (40, 0, -20) aprox.
```

### **Heading Accuracy:**

```
Accuracy (grados):
  < 10°  ? Excelente ?
  10-25° ? Bueno ?
  25-45° ? Aceptable ??
  > 45°  ? Pobre ? (requiere calibración)
```

---

## ?? Testing Paso a Paso

### **Test 1: Verificar Magnetómetro Existe**

1. Build APK
2. Instalar en dispositivo
3. Ver panel: `?? MAGNETÓMETRO ? Disponible: ? SÍ`

**Si dice NO:** El dispositivo no tiene el sensor, usar GPS bearing.

---

### **Test 2: Verificar Datos Válidos**

1. Ver panel: `Timestamp:` debe ser **> 0**
2. `TrueHeading` debe cambiar al girar el dispositivo

**Si Timestamp = -1:**
- Presionar "?? Reintentar Brújula"
- Mover teléfono en figura de 8
- Alejarse de objetos metálicos

---

### **Test 3: Calibración**

1. Abrir app nativa Brújula de Android
2. Seguir instrucciones de calibración
3. Cerrar app nativa
4. Volver a tu app AR
5. Presionar "?? Actualizar Diagnóstico"

**Resultado esperado:** Timestamp > 0

---

## ?? Integración con LocationManager

El diagnóstico es independiente, pero puedes integrarlo:

```csharp
// En cualquier script
public MagnetometerDiagnostic diagnostic;

void Start()
{
    // Ejecutar diagnóstico al inicio
    if (diagnostic != null)
    {
        diagnostic.RunDiagnostic();
    }
    
    // Verificar si hay magnetómetro
    // Si no, activar fallback
    if (!diagnostic.magnetometerAvailable) // Requiere hacer la variable pública
    {
        LocationManager.Instance.useGPSBearingFallback = true;
    }
}
```

---

## ?? Ejemplo de Uso en Producción

### **Flujo Recomendado:**

```
App Inicio
    ?
??????????????????????
? MagnetometerDiag   ?
? .RunDiagnostic()   ?
??????????????????????
          ?
          ?
     ¿Magnetómetro
      disponible?
          ?
    ?????????????
   SÍ           NO
    ?            ?
    ?            ?
¿Timestamp > 0?  Activar GPS
    ?            Bearing Fallback
?????????
?      ?
SÍ    NO
?      ?
?      ?
?   Mostrar UI
?   Calibración
?      ?
????????????? Continuar
             Navegación
```

---

## ??? Quitar en Build Final

Este script es solo para **debugging**. Para builds finales:

**Opción 1: Deshabilitar**
```csharp
public bool showOnScreenDebug = false; // En el Inspector
```

**Opción 2: Usar Scripting Define Symbols**
```csharp
#if DEVELOPMENT_BUILD
    // Solo en builds de desarrollo
    GetComponent<MagnetometerDiagnostic>().showOnScreenDebug = true;
#else
    GetComponent<MagnetometerDiagnostic>().showOnScreenDebug = false;
#endif
```

**Opción 3: Quitar el componente**
- Simplemente desactivar el GameObject o quitar el componente antes de build final

---

## ?? Resumen

**Uso básico:**
1. Agregar `MagnetometerDiagnostic` a un GameObject
2. Build APK y ejecutar en Android
3. Ver panel en pantalla
4. Verificar que `Timestamp > 0` y `Magnetómetro: ? SÍ`

**Si hay problemas:**
- Calibrar brújula (figura de 8)
- Alejarse de metal
- Verificar permisos
- Si no hay magnetómetro ? usar GPS bearing

**Resultado esperado:**
```
? Magnetómetro disponible
? Timestamp > 0
? TrueHeading cambia al girar dispositivo
```
