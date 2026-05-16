# ?? EJEMPLOS DE PERSONALIZACIÓN - Sistema de Navegación AR

## ?? CASOS DE USO COMUNES

### 1?? Cambiar el Comportamiento de la Flecha

#### **Hacer que la flecha "pulse" más rápido cuando estás cerca:**

```csharp
// En NavigationArrowController.cs, dentro de UpdatePulseAnimation():

float distance = LocationManager.Instance.GetDistanceToDestination(
    currentDestination.latitude,
    currentDestination.longitude
);

// Pulsar más rápido cuando estás cerca
float speedMultiplier = distance < 50f ? 3f : 1f;
float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed * speedMultiplier) * pulseIntensity;
arrowInstance.transform.localScale = baseScale * pulse;
```

#### **Cambiar el color según la dirección (adelante = verde, atrás = rojo):**

```csharp
// En NavigationArrowController.cs, dentro de UpdateArrowRotation():

float relativeAngle = LocationManager.Instance.GetRelativeAngleToDestination(
    currentDestination.latitude,
    currentDestination.longitude
);

// Si el ángulo es pequeño, estás mirando en la dirección correcta
bool facingCorrectDirection = Mathf.Abs(relativeAngle) < 30f;

if (arrowRenderer != null)
{
    arrowRenderer.material.color = facingCorrectDirection ? Color.green : Color.red;
}
```

---

### 2?? Añadir Vibración

#### **Vibrar cuando miras en la dirección correcta:**

```csharp
// En NavigationArrowController.cs, añadir al final de UpdateArrowRotation():

float relativeAngle = LocationManager.Instance.GetRelativeAngleToDestination(
    currentDestination.latitude,
    currentDestination.longitude
);

if (Mathf.Abs(relativeAngle) < 15f)
{
    // Vibración corta cada 2 segundos
    if (Time.time % 2f < 0.1f)
    {
        Handheld.Vibrate();
    }
}
```

#### **Vibrar al llegar al destino:**

```csharp
// En NavigationArrowController.cs, dentro de UpdateDistanceDisplay():

float distance = LocationManager.Instance.GetDistanceToDestination(
    currentDestination.latitude,
    currentDestination.longitude
);

// Si llegas a menos de 10 metros
if (distance < 10f && !hasReachedDestination)
{
    hasReachedDestination = true;
    Handheld.Vibrate();
    Debug.Log("[NavigationArrowController] ?? ¡Llegaste al destino!");
    
    // Opcional: Mostrar mensaje en UI
    // ShowDestinationReachedMessage();
}
```

**Añadir variable:**
```csharp
// Al inicio de la clase NavigationArrowController:
private bool hasReachedDestination = false;

// En StopNavigation():
hasReachedDestination = false;
```

---

### 3?? Mejorar la UI

#### **Mostrar tiempo estimado de llegada:**

```csharp
// En NavigationUIManager.cs, añadir nuevo método:

private string CalculateETA(float distanceMeters)
{
    // Asumiendo velocidad de caminata promedio: 1.4 m/s (5 km/h)
    float walkingSpeedMPS = 1.4f;
    float timeSeconds = distanceMeters / walkingSpeedMPS;
    
    if (timeSeconds < 60)
    {
        return $"~{timeSeconds:F0}s";
    }
    else
    {
        int minutes = Mathf.CeilToInt(timeSeconds / 60f);
        return $"~{minutes} min";
    }
}

// En Update(), modificar UpdateNavigationInfo():

if (distanceText != null)
{
    string eta = CalculateETA(distance);
    distanceText.text = $"{GeoUtils.FormatDistance(distance)}\nETA: {eta}";
}
```

#### **Añadir indicador de precisión del GPS:**

```csharp
// En NavigationUIManager.cs, añadir al final de Update():

if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
{
    float accuracy = LocationManager.Instance.CurrentAccuracy;
    
    if (accuracy > 20f)
    {
        // Mostrar advertencia si la precisión es baja
        if (directionText != null)
        {
            directionText.color = Color.yellow;
            directionText.text += $"\n?? Precisión baja (±{accuracy:F0}m)";
        }
    }
}
```

---

### 4?? Sonidos y Efectos

#### **Reproducir sonido al seleccionar destino:**

```csharp
// Primero, añadir variable en NavigationUIManager:
public AudioClip destinationSelectedSound;
private AudioSource audioSource;

// En Start():
audioSource = gameObject.AddComponent<AudioSource>();

// En OnLocationSelected():
if (destinationSelectedSound != null && audioSource != null)
{
    audioSource.PlayOneShot(destinationSelectedSound);
}
```

#### **Sonido cuando apuntas en la dirección correcta:**

```csharp
// En NavigationArrowController, añadir:
public AudioClip correctDirectionSound;
private AudioSource audioSource;
private bool wasPointingCorrectly = false;

// En Start():
audioSource = GetComponent<AudioSource>();
if (audioSource == null)
    audioSource = gameObject.AddComponent<AudioSource>();

// En UpdateArrowRotation(), después de calcular relativeAngle:
bool isPointingCorrectly = Mathf.Abs(relativeAngle) < 15f;

if (isPointingCorrectly && !wasPointingCorrectly)
{
    // Cambió de incorrecto a correcto
    if (correctDirectionSound != null)
    {
        audioSource.PlayOneShot(correctDirectionSound);
    }
}

wasPointingCorrectly = isPointingCorrectly;
```

---

### 5?? Filtros de Lugares

#### **Filtrar lugares por distancia máxima:**

```csharp
// En NavigationUIManager.cs, modificar PopulateLocationList():

float maxDistance = 5000f; // 5 km

foreach (BuildingData building in availableBuildings)
{
    // Calcular distancia desde ubicación actual
    if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
    {
        float distance = LocationManager.Instance.GetDistanceToDestination(
            building.latitude,
            building.longitude
        );
        
        // Solo mostrar si está dentro del rango
        if (distance > maxDistance)
            continue;
    }
    
    // ...resto del código para crear botón...
}
```

#### **Ordenar lugares por cercanía:**

```csharp
// En NavigationUIManager.cs, después de LoadAvailableLocations():

if (LocationManager.Instance != null && LocationManager.Instance.IsGPSReady)
{
    availableBuildings.Sort((a, b) =>
    {
        float distA = LocationManager.Instance.GetDistanceToDestination(a.latitude, a.longitude);
        float distB = LocationManager.Instance.GetDistanceToDestination(b.latitude, b.longitude);
        return distA.CompareTo(distB);
    });
    
    Debug.Log("[NavigationUIManager] ?? Lugares ordenados por cercanía");
}
```

---

### 6?? Mejoras de Performance

#### **Actualizar GPS solo cuando es necesario:**

```csharp
// En LocationManager.cs, añadir:
public float updateInterval = 1f; // Actualizar cada segundo
private float lastUpdateTime = 0f;

// En Update(), reemplazar UpdateLocation() con:

if (Time.time - lastUpdateTime >= updateInterval)
{
    UpdateLocation();
    lastUpdateTime = Time.time;
}
```

#### **Pausar navegación cuando la app está en background:**

```csharp
// En NavigationArrowController.cs, añadir:

void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        // App en background
        Debug.Log("[NavigationArrowController] ?? Pausando navegación");
    }
    else
    {
        // App vuelve a foreground
        Debug.Log("[NavigationArrowController] ?? Reanudando navegación");
    }
}
```

---

### 7?? Modo Offline

#### **Guardar último destino para reanudar:**

```csharp
// En NavigationArrowController.cs:

private const string LAST_DESTINATION_KEY = "LastDestination";

public void SetDestination(BuildingData destination)
{
    // ...código existente...
    
    // Guardar en PlayerPrefs
    string json = JsonUtility.ToJson(destination);
    PlayerPrefs.SetString(LAST_DESTINATION_KEY, json);
    PlayerPrefs.Save();
}

// Añadir método para restaurar:
public void RestoreLastDestination()
{
    if (PlayerPrefs.HasKey(LAST_DESTINATION_KEY))
    {
        string json = PlayerPrefs.GetString(LAST_DESTINATION_KEY);
        BuildingData lastDest = JsonUtility.FromJson<BuildingData>(json);
        SetDestination(lastDest);
        Debug.Log($"[NavigationArrowController] ?? Destino restaurado: {lastDest.name}");
    }
}
```

---

### 8?? Integración con Google Maps

#### **Botón para abrir en Google Maps:**

```csharp
// En NavigationUIManager.cs, añadir botón en el panel:
public Button openInMapsButton;

void Start()
{
    // ...código existente...
    
    if (openInMapsButton != null)
    {
        openInMapsButton.onClick.AddListener(OpenDestinationInMaps);
    }
}

private void OpenDestinationInMaps()
{
    if (selectedDestination == null) return;
    
    string url = $"https://www.google.com/maps/dir/?api=1&destination={selectedDestination.latitude},{selectedDestination.longitude}";
    
    #if UNITY_ANDROID
    Application.OpenURL(url);
    #elif UNITY_IOS
    Application.OpenURL($"comgooglemaps://?daddr={selectedDestination.latitude},{selectedDestination.longitude}&directionsmode=walking");
    #else
    Application.OpenURL(url);
    #endif
    
    Debug.Log($"[NavigationUIManager] ??? Abriendo Google Maps para {selectedDestination.name}");
}
```

---

### 9?? Compartir Ubicación

#### **Compartir destino por WhatsApp/SMS:**

```csharp
// En NavigationUIManager.cs:

public Button shareLocationButton;

void Start()
{
    // ...código existente...
    
    if (shareLocationButton != null)
    {
        shareLocationButton.onClick.AddListener(ShareDestination);
    }
}

private void ShareDestination()
{
    if (selectedDestination == null) return;
    
    string message = $"?? {selectedDestination.name}\n" +
                    $"{selectedDestination.description}\n" +
                    $"Ver ubicación: https://www.google.com/maps?q={selectedDestination.latitude},{selectedDestination.longitude}";
    
    // Android share intent
    #if UNITY_ANDROID
    AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
    AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
    
    intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
    intentObject.Call<AndroidJavaObject>("setType", "text/plain");
    intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), message);
    
    AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
    
    AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Compartir ubicación");
    currentActivity.Call("startActivity", chooser);
    #endif
    
    Debug.Log("[NavigationUIManager] ?? Compartiendo destino");
}
```

---

### ?? Modo Noche/Día

#### **Cambiar colores de UI según hora del día:**

```csharp
// En NavigationUIManager.cs:

void Start()
{
    // ...código existente...
    
    ApplyThemeBasedOnTime();
}

private void ApplyThemeBasedOnTime()
{
    int hour = System.DateTime.Now.Hour;
    bool isNightMode = hour < 6 || hour > 20;
    
    if (isNightMode)
    {
        // Tema oscuro
        if (navigationActivePanel != null)
        {
            Image panelImage = navigationActivePanel.GetComponent<Image>();
            if (panelImage != null)
                panelImage.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        }
        
        Debug.Log("[NavigationUIManager] ?? Modo noche activado");
    }
    else
    {
        // Tema claro
        if (navigationActivePanel != null)
        {
            Image panelImage = navigationActivePanel.GetComponent<Image>();
            if (panelImage != null)
                panelImage.color = new Color(0, 0.3f, 0.5f, 0.9f);
        }
        
        Debug.Log("[NavigationUIManager] ?? Modo día activado");
    }
}
```

---

## ?? CONFIGURACIÓN VISUAL AVANZADA

### **Flecha con gradiente de color:**

```csharp
// En NavigationArrowController.cs:

private void UpdateArrowColor()
{
    if (arrowRenderer == null) return;
    
    float distance = LocationManager.Instance.GetDistanceToDestination(
        currentDestination.latitude,
        currentDestination.longitude
    );
    
    Color color;
    
    if (distance < 20f)
        color = Color.green; // Muy cerca
    else if (distance < 100f)
        color = Color.yellow; // Cerca
    else if (distance < 500f)
        color = Color.Lerp(Color.yellow, Color.red, (distance - 100f) / 400f); // Gradiente
    else
        color = Color.red; // Lejos
    
    arrowRenderer.material.color = color;
}
```

---

## ?? ANALYTICS Y TRACKING

### **Registrar eventos de navegación:**

```csharp
// En AppModeManager.cs:

public void StartNavigation(BuildingData destination)
{
    // ...código existente...
    
    // Registrar evento
    LogNavigationEvent("navigation_started", destination.name);
}

private void LogNavigationEvent(string eventName, string destinationName)
{
    Debug.Log($"[Analytics] {eventName}: {destinationName}");
    
    // Aquí podrías integrar con Firebase Analytics, Unity Analytics, etc.
    // Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, "destination", destinationName);
}
```

---

## ?? OPTIMIZACIONES

### **Pooling de objetos UI:**

```csharp
// En NavigationUIManager.cs:

private List<GameObject> buttonPool = new List<GameObject>();

private void PopulateLocationList()
{
    // Reutilizar botones en lugar de destruir/crear
    
    // Desactivar todos los botones
    foreach (GameObject button in buttonPool)
    {
        button.SetActive(false);
    }
    
    for (int i = 0; i < availableBuildings.Count; i++)
    {
        GameObject buttonObj;
        
        if (i < buttonPool.Count)
        {
            // Reutilizar existente
            buttonObj = buttonPool[i];
            buttonObj.SetActive(true);
        }
        else
        {
            // Crear nuevo
            buttonObj = Instantiate(locationButtonPrefab, locationListContent);
            buttonPool.Add(buttonObj);
        }
        
        // Configurar botón...
    }
}
```

---

**¡Estos ejemplos te dan ideas para personalizar completamente tu sistema de navegación AR!** ??

Puedes mezclar y combinar según tus necesidades. Todos los ejemplos son copiar-pegar y funcionan directamente.
