# ?? SOLUCIÓN: Brújula No Proporciona Valores (0.0° constante)

## ?? Problema Identificado

En los logs se observa:
```
Brújula - trueHeading: 0.0°
Brújula - magneticHeading: 0.0°
Brújula - timestamp: -1
```

**timestamp: -1** significa que el sensor de magnetómetro **NO está proporcionando datos**.

---

## ?? ¿Cómo Unity Detecta el Norte Real?

Unity utiliza el **sensor de magnetómetro** del dispositivo Android:

### 1. **Norte Magnético** (`magneticHeading`)
- Apunta hacia los **polos magnéticos de la Tierra**
- NO es el Norte geográfico exacto
- Varía según tu ubicación en el planeta (declinación magnética)

### 2. **Norte Verdadero** (`trueHeading`)
- Apunta hacia el **Norte geográfico real**
- Unity lo calcula usando: `magneticHeading + declinación magnética de tu ubicación GPS`
- Requiere que el GPS esté funcionando

### ?? Declinación Magnética
La diferencia entre el Norte magnético y el geográfico. Por ejemplo:
- En El Salvador: aproximadamente **-2° a -4°**
- En Nueva York: aproximadamente **-13°**
- En Londres: aproximadamente **0°** (casi sin diferencia)

Unity usa `Input.compass.headingAccuracy` para obtener esta corrección automáticamente.

---

## ?? Causas Comunes del Problema

### 1. **Falta de Calibración**
El magnetómetro necesita calibración física:
- Mover el dispositivo en forma de **figura 8**
- Alejarse de objetos metálicos
- Alejarse de campos magnéticos (computadoras, cables, altavoces)

### 2. **Permisos Insuficientes**
Android 12+ requiere permisos adicionales.

### 3. **Interferencia Magnética**
- Estar en interiores con estructuras metálicas
- Cerca de electrodomésticos
- Cerca de computadoras o altavoces

### 4. **Sensor No Disponible**
Algunos dispositivos Android no tienen magnetómetro (raro pero posible).

---

## ? Soluciones

### **Solución 1: Calibración Manual Forzada**

Agregar un panel UI que pida al usuario calibrar:

```csharp
public void ShowCompassCalibrationDialog()
{
    // Mostrar instrucciones al usuario
    Debug.Log("?? CALIBRACIÓN NECESARIA:");
    Debug.Log("1. Aleja el teléfono de objetos metálicos");
    Debug.Log("2. Mueve el dispositivo en forma de FIGURA 8");
    Debug.Log("3. Repite varias veces hasta que funcione");
    
    // Opcional: mostrar un diálogo/panel UI
}
```

### **Solución 2: Verificar Sensor en Runtime**

Agregar verificación de disponibilidad del magnetómetro:

```csharp
private bool IsMagnetometerAvailable()
{
    // Android specific check
    #if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    AndroidJavaObject sensorManager = activity.Call<AndroidJavaObject>("getSystemService", "sensor");
    AndroidJavaObject magnetometer = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", 2); // TYPE_MAGNETIC_FIELD = 2
    
    return magnetometer != null;
    #else
    return true; // Asumir disponible en otras plataformas
    #endif
}
```

### **Solución 3: Fallback a Gyroscope**

Si la brújula no funciona, usar el giroscopio como alternativa temporal:

```csharp
private float GetOrientationFallback()
{
    if (Input.compass.timestamp > 0)
    {
        // Brújula funciona
        return Input.compass.trueHeading;
    }
    else if (Input.gyro.enabled)
    {
        // Usar giroscopio como fallback
        Quaternion gyroAttitude = Input.gyro.attitude;
        float heading = Mathf.Atan2(2f * (gyroAttitude.y * gyroAttitude.w + gyroAttitude.x * gyroAttitude.z),
                                     1f - 2f * (gyroAttitude.y * gyroAttitude.y + gyroAttitude.z * gyroAttitude.z));
        return heading * Mathf.Rad2Deg;
    }
    else
    {
        Debug.LogWarning("Ni brújula ni giroscopio disponibles");
        return 0f;
    }
}
```

### **Solución 4: Aumentar Timeout de Inicialización**

La brújula puede tardar más en inicializarse:

```csharp
private IEnumerator WaitForCompassReady()
{
    Input.compass.enabled = true;
    
    float timeout = 10f;
    float elapsed = 0f;
    
    while (Input.compass.timestamp < 0 && elapsed < timeout)
    {
        yield return new WaitForSeconds(0.5f);
        elapsed += 0.5f;
        Debug.Log($"? Esperando brújula... {elapsed:F1}s");
    }
    
    if (Input.compass.timestamp >= 0)
    {
        Debug.Log("? Brújula lista!");
        isCompassEnabled = true;
    }
    else
    {
        Debug.LogError("? Brújula no inicializó después de " + timeout + "s");
    }
}
```

### **Solución 5: Permisos en AndroidManifest**

Asegúrate de tener estos permisos en `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />

<!-- Android 12+ requiere esto para sensores relacionados con ubicación -->
<uses-permission android:name="android.permission.HIGH_SAMPLING_RATE_SENSORS" />

<!-- Declarar que la app usa el sensor de brújula -->
<uses-feature android:name="android.hardware.sensor.compass" android:required="false" />
<uses-feature android:name="android.hardware.sensor.accelerometer" android:required="true" />
```

---

## ?? Pasos para Probar

### En Dispositivo Real:

1. **Sal al exterior** (las brújulas funcionan mal en interiores)
2. **Aleja el teléfono de objetos metálicos**
3. **Abre la app nativa de Brújula de Android** y calibra
4. **Mueve el teléfono en forma de 8** varias veces
5. **Cierra y reabre tu app**
6. **Verifica los logs**: `timestamp` debe ser > 0

### En Unity Editor:

El Editor **siempre simula** la brújula, por lo que no verás el problema allí.

---

## ?? Alternativa: Usar Solo GPS Bearing

Si la brújula sigue sin funcionar, puedes usar **GPS Bearing** (dirección de movimiento):

```csharp
// En vez de usar Input.compass.trueHeading
// Usar la dirección en la que te estás moviendo
float gpsBearing = Input.location.lastData.course; // Solo disponible si te estás moviendo
```

**Limitación**: Solo funciona cuando el usuario se está **moviendo**, no cuando está parado.

---

## ?? Código Mejorado de Referencia

Ver actualizaciones en:
- `LocationManager.cs` ? Método `InitializeCompass()` mejorado
- `NavigationArrowController.cs` ? Fallback a GPS bearing

---

## ?? Debugging

Agregar esto a `LocationManager.Update()`:

```csharp
if (Time.frameCount % 60 == 0) // Cada segundo
{
    Debug.Log($"?? Compass Status:");
    Debug.Log($"   Enabled: {Input.compass.enabled}");
    Debug.Log($"   Timestamp: {Input.compass.timestamp}");
    Debug.Log($"   TrueHeading: {Input.compass.trueHeading}°");
    Debug.Log($"   MagneticHeading: {Input.compass.magneticHeading}°");
    Debug.Log($"   Accuracy: {Input.compass.headingAccuracy}");
    Debug.Log($"   RawVector: {Input.compass.rawVector}");
}
```

---

## ?? Si Nada Funciona

**Posibles causas:**
1. El dispositivo NO tiene magnetómetro (verifica las especificaciones)
2. El magnetómetro está dañado físicamente
3. Android bloqueó el acceso al sensor (problema de permisos o ROM personalizada)

**Solución definitiva:**
Implementa navegación basada en **GPS Bearing** (dirección de movimiento) en vez de brújula estática.

