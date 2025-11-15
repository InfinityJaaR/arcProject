# ?? GUÍA PASO A PASO: Calibrar Brújula en Android

## ?? ¿Por qué mi brújula muestra 0.0° constante?

El sensor de **magnetómetro** (brújula) en Android necesita:
1. ? **Calibración física** (mover el teléfono)
2. ? **Ambiente sin interferencias** (alejado de metal/electrónica)
3. ? **Permisos correctos** en la app

---

## ?? PASOS PARA SOLUCIONAR

### **Paso 1: Verificar Permisos**

Asegúrate que tu `AndroidManifest.xml` tenga:

```xml
<!-- Archivo: Assets/Plugins/Android/AndroidManifest.xml -->
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    
    <!-- Permisos de ubicación -->
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
    <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
    
    <!-- Android 12+ -->
    <uses-permission android:name="android.permission.HIGH_SAMPLING_RATE_SENSORS" />
    
    <!-- Declarar sensores -->
    <uses-feature android:name="android.hardware.sensor.compass" android:required="false" />
    <uses-feature android:name="android.hardware.sensor.accelerometer" android:required="true" />
    <uses-feature android:name="android.hardware.sensor.gyroscope" android:required="false" />
    
</manifest>
```

### **Paso 2: Calibrar la Brújula Físicamente**

1. **Abre la app nativa "Brújula" o "Compass" de Android**
2. Sigue las instrucciones que aparezcan
3. Mueve el teléfono en **forma de 8** varias veces:

```
      ?
   ?     ?
  ?       ?
   ?     ?
      ?
```

4. Haz movimientos **amplios y continuos**

### **Paso 3: Verificar Ambiente**

? **EVITAR calibrar cerca de:**
- Computadoras portátiles/escritorio
- Mesas metálicas
- Altavoces grandes
- Cables eléctricos gruesos
- Automóviles
- Electrodomésticos

? **MEJOR calibrar en:**
- Exteriores (parques, patios)
- Centro de una habitación (lejos de paredes con cables)
- Lejos de dispositivos electrónicos

### **Paso 4: Reiniciar la App**

Después de calibrar en la app nativa:
1. Cierra completamente tu app AR
2. Vuelve a abrirla
3. Verifica los logs

---

## ??? Código para Testing

Agrega este método temporal a `LocationManager.cs` para verificar el sensor:

```csharp
// MÉTODO DE DEBUG - QUITAR EN PRODUCCIÓN
void OnGUI()
{
    if (!Application.isEditor)
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 300));
        GUILayout.Label("?? DEBUG BRÚJULA");
        GUILayout.Label($"Enabled: {Input.compass.enabled}");
        GUILayout.Label($"Timestamp: {Input.compass.timestamp}");
        GUILayout.Label($"TrueHeading: {Input.compass.trueHeading:F2}°");
        GUILayout.Label($"MagneticHeading: {Input.compass.magneticHeading:F2}°");
        GUILayout.Label($"Accuracy: {Input.compass.headingAccuracy:F2}°");
        GUILayout.Label($"RawVector: {Input.compass.rawVector}");
        
        if (Input.compass.timestamp < 0)
        {
            GUILayout.Label("<color=red>? SIN DATOS</color>");
            GUILayout.Label("Calibra la brújula!");
        }
        else
        {
            GUILayout.Label("<color=green>? FUNCIONANDO</color>");
        }
        
        if (GUILayout.Button("Recalibrar"))
        {
            RecalibrateCompass();
        }
        
        GUILayout.EndArea();
    }
}
```

---

## ?? Testing en Dispositivo Real

### **Checklist:**

- [ ] Compilar APK con permisos correctos
- [ ] Instalar en dispositivo Android real
- [ ] Abrir app nativa de Brújula de Android
- [ ] Calibrar moviendo en forma de 8
- [ ] Salir al exterior si es posible
- [ ] Abrir tu app AR
- [ ] Verificar que `timestamp > 0` en logs

### **Comando ADB para ver logs:**

```bash
adb logcat -s Unity
```

O filtrar solo LocationManager:

```bash
adb logcat -s Unity | grep LocationManager
```

---

## ?? Si NADA Funciona

### **Opción A: Verificar que el dispositivo TENGA magnetómetro**

No todos los Android tienen este sensor. Verifica las especificaciones de tu modelo.

**Código para verificar:**

```csharp
#if UNITY_ANDROID && !UNITY_EDITOR
private bool HasMagnetometer()
{
    try
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject sensorManager = activity.Call<AndroidJavaObject>("getSystemService", "sensor");
        AndroidJavaObject magnetometer = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", 2); // TYPE_MAGNETIC_FIELD
        
        return magnetometer != null;
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error verificando magnetómetro: {e.Message}");
        return false;
    }
}
#endif
```

### **Opción B: Usar GPS Bearing (dirección de movimiento)**

Si la brújula definitivamente no funciona, usa la dirección en la que te mueves:

```csharp
// Solo funciona cuando el usuario se está MOVIENDO
float gpsBearing = Input.location.lastData.course;
float gpsSpeed = Input.location.lastData.speed;

if (gpsSpeed > 0.5f) // 0.5 m/s = caminando lento
{
    currentBearing = gpsBearing;
}
```

**Limitaciones:**
- ?? Solo funciona en **movimiento**
- ?? No funciona si estás **parado**
- ?? Puede tener lag/delay

---

## ?? Solución Implementada en el Proyecto

Ya implementamos:

1. ? **Detección de brújula no funcional** (`compassHasValidData`)
2. ? **Fallback a GPS bearing** cuando estás en movimiento
3. ? **Timeout de inicialización** (10 segundos)
4. ? **UI de calibración** (`CompassCalibrationUI.cs`)
5. ? **Método de recalibración** (`RecalibrateCompass()`)

### **Cómo usar:**

```csharp
// En cualquier script
if (LocationManager.Instance != null)
{
    // Forzar recalibración
    LocationManager.Instance.RecalibrateCompass();
}
```

---

## ?? Contacto / Ayuda

Si después de seguir todos los pasos sigue sin funcionar:

1. Verifica las **especificaciones del dispositivo** (debe tener magnetómetro)
2. Prueba en **otro dispositivo Android** para descartar hardware dañado
3. Considera usar **solo GPS bearing** como alternativa permanente

---

## ?? Referencias

- [Unity Input.compass Documentation](https://docs.unity3d.com/ScriptReference/Input-compass.html)
- [Android Sensor Documentation](https://developer.android.com/guide/topics/sensors/sensors_position)
- [Magnetic Declination (NOAA)](https://www.ngdc.noaa.gov/geomag/declination.shtml)
