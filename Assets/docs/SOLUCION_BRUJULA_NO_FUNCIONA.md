# ?? SOLUCIÓN: Brújula No Funciona (Bearing Siempre 0°)

## ?? PROBLEMA IDENTIFICADO

En los logs veo:

```
?? Bearing del dispositivo: 0.0° (hacia dónde miras)  ? SIEMPRE 0°
```

**Esto significa que la BRÚJULA NO ESTÁ FUNCIONANDO.**

### **Resultado:**
- ? GPS funciona (obtiene coordenadas correctas)
- ? Calcula bearing correcto al destino (91.7°)
- ? **Bearing del dispositivo SIEMPRE es 0°** 
- ? **La flecha apunta en dirección INCORRECTA**

---

## ? SOLUCIONES

### **SOLUCIÓN 1: Calibrar la Brújula Física** ? MÁS COMÚN

La brújula necesita calibración después de:
- Primera vez que usas la app
- Cambio de ubicación
- Interferencia magnética

**Pasos:**

1. **Sal al aire libre** (lejos de edificios metálicos)

2. **Mueve el teléfono haciendo FIGURA DE 8** en el aire:
```
     ?
   /   \
  |     |
   \   /
     ?
```

3. **Repite 3-5 veces** en diferentes direcciones

4. **Reinicia la app**

5. **Build & Run** de nuevo

6. **Observa Logcat:**
```
[LocationManager] ?? Brújula - trueHeading: 45.3° ? (debe cambiar)
[LocationManager] ?? Brújula - magneticHeading: 47.1° ?
```

**Si sigue siendo 0.0°, pasa a la siguiente solución.**

---

### **SOLUCIÓN 2: Verificar Permisos de Sensores**

En algunos dispositivos Android, los sensores también requieren permisos.

**Pasos:**

1. En el dispositivo:
```
Configuración ? Apps ? ArcProject
? Permisos ? Sensores corporales
? Permitir
```

2. O en **Configuración del dispositivo:**
```
Configuración ? Privacidad ? Gestor de permisos
? Sensores ? ArcProject ? Permitir
```

3. **Reinicia la app**

---

### **SOLUCIÓN 3: Usar Magnetic Heading en lugar de True Heading**

Si `trueHeading` siempre es 0 pero `magneticHeading` funciona, podemos usar ese.

**Ya he actualizado el código** para hacer esto automáticamente:

```csharp
// Si trueHeading es inválido, usar magneticHeading
if (float.IsNaN(newBearing) || newBearing < 0)
{
    newBearing = Input.compass.magneticHeading;
}
```

**No necesitas hacer nada, solo Build & Run de nuevo.**

---

### **SOLUCIÓN 4: Añadir Panel de Debug de Brújula**

He creado un script para ver exactamente qué está pasando con la brújula.

**Implementación:**

Crea un Canvas con este UI:

```
Canvas
?? CompassDebugPanel (Panel)
    ?? StatusText (TextMeshProUGUI): "Estado Brújula"
    ?? TrueHeadingText (TextMeshProUGUI): "True Heading"
    ?? MagneticHeadingText (TextMeshProUGUI): "Magnetic Heading"
    ?? TimestampText (TextMeshProUGUI): "Timestamp"
    ?? RawQuaternionText (TextMeshProUGUI): "Raw Vector"
    ?? RecalibrateButton (Button): "?? Recalibrar"
```

**Script:** `CompassDebugPanel.cs` (ya creado)

**Esto te mostrará en pantalla:**
- ? Si la brújula está activa
- ? Valores de trueHeading y magneticHeading en tiempo real
- ? Si los valores son válidos o inválidos
- ? Botón para recalibrar

---

### **SOLUCIÓN 5: Reiniciar Brújula Programáticamente**

Puedes forzar la recalibración desde código.

**En Unity Console o mediante un botón:**

```csharp
LocationManager.Instance.RecalibrateCompass();
```

**Esto:**
1. Desactiva la brújula
2. La vuelve a activar
3. Te pide calibrar físicamente

---

## ?? DEBUGGING

### **Verificar Estado de la Brújula:**

Después de hacer Build & Run, observa estos logs:

**? BRÚJULA FUNCIONANDO:**
```
[LocationManager] ?? Brújula - trueHeading: 45.3°
[LocationManager] ?? Brújula - magneticHeading: 47.1°
[LocationManager] ?? Brújula - timestamp: 123.456

[NavigationArrowController] ?? Bearing del dispositivo: 45.3° ?
```

**? BRÚJULA NO FUNCIONANDO:**
```
[LocationManager] ?? Brújula - trueHeading: 0.0° ? Siempre 0
[LocationManager] ?? Brújula - magneticHeading: 0.0° ? Siempre 0
[LocationManager] ?? trueHeading inválido, usando magneticHeading

[NavigationArrowController] ?? Bearing del dispositivo: 0.0° ?
```

---

## ?? PRUEBA DE FUNCIONAMIENTO

### **Test Simple:**

1. **Build & Run**
2. **Selecciona un destino**
3. **Gira tu cuerpo 360°** (da una vuelta completa)
4. **Observa Logcat**

**Comportamiento CORRECTO:**
```
Mirando al Norte: Bearing = 0°
Mirando al Este:  Bearing = 90°
Mirando al Sur:   Bearing = 180°
Mirando al Oeste: Bearing = 270°
Mirando al Norte: Bearing = 0° (de vuelta)
```

**Comportamiento INCORRECTO:**
```
Mirando al Norte: Bearing = 0°
Mirando al Este:  Bearing = 0° ? NO CAMBIA ?
Mirando al Sur:   Bearing = 0° ? NO CAMBIA ?
```

---

## ?? CAUSAS COMUNES DEL PROBLEMA

### **1. Interferencia Magnética**

**Causas:**
- Edificios con estructura metálica
- Cerca de electrodomésticos
- Fundas de teléfono con imán
- Auriculares magnéticos cerca

**Solución:**
- Sal al aire libre
- Quita funda metálica
- Aléjate de objetos metálicos

---

### **2. Sensor de Brújula Dañado**

**Verificación:**
- Descarga app "GPS Status & Toolbox" de Play Store
- Verifica si muestra valores de brújula
- Si no funciona ahí tampoco ? Hardware dañado

---

### **3. Android API/Permisos**

**Algunos dispositivos Samsung/Xiaomi:**
- Requieren permisos adicionales de sensores
- Configuración ? Privacidad ? Gestor de permisos ? Sensores

---

### **4. Modo Ahorro de Energía**

**Algunos dispositivos desactivan sensores en modo ahorro:**
- Desactiva modo ahorro de energía
- Reinicia la app

---

## ?? ALTERNATIVA: Usar Rotación de la Cámara

Si la brújula NO funciona en absoluto, podemos usar la rotación de la cámara AR.

**Implementación alternativa:**

```csharp
// En NavigationArrowController.UpdateArrowRotation()
float deviceBearing;

#if PLATFORM_ANDROID
// Intentar usar brújula
deviceBearing = LocationManager.Instance.CurrentBearing;

// Si brújula no funciona (siempre 0), usar rotación de cámara AR
if (Mathf.Approximately(deviceBearing, 0f))
{
    // Usar rotación Y de la cámara como aproximación
    deviceBearing = arCamera.transform.eulerAngles.y;
    Debug.LogWarning("[NavigationArrowController] ?? Usando rotación de cámara (brújula no funciona)");
}
#else
deviceBearing = LocationManager.Instance.CurrentBearing;
#endif
```

**Nota:** Esto es menos preciso pero funciona si la brújula está rota.

---

## ? CHECKLIST DE VERIFICACIÓN

```
? Calibré la brújula (figura de 8)
? Estoy al aire libre
? No hay objetos metálicos cerca
? Permisos de sensores activados
? Modo ahorro de energía desactivado
? Build & Run ejecutado
? Logcat muestra valores de brújula cambiantes
? Bearing del dispositivo NO es siempre 0°
? La flecha rota cuando giro el dispositivo
```

---

## ?? RESULTADO ESPERADO

Después de calibrar la brújula:

```
? trueHeading cambia al rotar el dispositivo (0°-360°)
? magneticHeading cambia al rotar el dispositivo
? Bearing del dispositivo se actualiza en tiempo real
? La flecha apunta SIEMPRE hacia el destino
? Al girar 360°, la flecha mantiene dirección al destino
? Navegación AR funciona correctamente
```

---

## ?? ARCHIVOS MODIFICADOS/CREADOS

- ? **`LocationManager.cs`** - Mejor manejo de brújula + método RecalibrateCompass()
- ? **`CompassDebugPanel.cs`** (nuevo) - Panel de debugging para brújula
- ? **`SOLUCION_BRUJULA_NO_FUNCIONA.md`** - Esta documentación

---

## ?? PRÓXIMOS PASOS

1. **Build & Run** con el código actualizado
2. **Sal al exterior**
3. **Calibra la brújula** (figura de 8)
4. **Observa Logcat:**
   - ¿El bearing cambia al girar?
   - ? SÍ ? ¡Funcionando!
   - ? NO ? Pasa a solución alternativa

5. **Prueba navegación:**
   - Selecciona destino
   - Gira 360°
   - La flecha debe apuntar siempre al destino

---

**La calibración de la brújula es el paso MÁS IMPORTANTE.** ??

Si después de calibrar sigue sin funcionar, es probable que el sensor esté dañado o el dispositivo no lo soporte correctamente.

**Última actualización:** 2025-01-07
