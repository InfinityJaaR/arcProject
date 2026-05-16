# ?? SISTEMA DE NAVEGACIÓN AR - GUÍA DE CONFIGURACIÓN

## ? ARCHIVOS CREADOS

Se han creado los siguientes componentes para el sistema de navegación:

### ?? Scripts Principales:
- ? `GeoUtils.cs` - Utilidades para cálculos geográficos (distancias, bearings)
- ? `LocationManager.cs` - Gestor de GPS y brújula del dispositivo
- ? `AppModeManager.cs` - Gestor de modos (Tracking vs Navigation)
- ? `NavigationArrowController.cs` - Controlador de la flecha AR
- ? `NavigationUIManager.cs` - Gestor de la interfaz de usuario

### ?? Scripts Modificados:
- ? `FirebaseManager.cs` - Añadido método `GetAllBuildingsAsync()`
- ? `MultiImageSpawner.cs` - Añadido método `SetTrackingEnabled()`

### ?? Prefabs:
- ? `LocationButton.prefab` - Botón para seleccionar lugares

---

## ?? CONFIGURACIÓN EN UNITY

### **PASO 1: Configurar la Escena**

#### A) Crear LocationManager:
1. En la jerarquía: `Create Empty` ? Nombrar: **"LocationManager"**
2. Añadir componente: `LocationManager.cs`
3. Configurar en el Inspector:
   ```
   ? Desired Accuracy In Meters: 10
   ? Update Distance In Meters: 5
   ? Max Wait Time: 20
   
   ?? PARA TESTING EN UNITY EDITOR:
   ? Simulate In Editor: TRUE
   ? Simulated Latitude: 13.7181033 (tu ubicación de prueba)
   ? Simulated Longitude: -89.2040915
   ? Simulated Bearing: 0
   ```

#### B) Crear AppModeManager:
1. En la jerarquía: `Create Empty` ? Nombrar: **"AppModeManager"**
2. Añadir componente: `AppModeManager.cs`
3. Configurar en el Inspector:
   ```
   ? AR Tracked Image Manager: [Arrastra el GameObject que tiene ARTrackedImageManager]
   ? Multi Image Spawner: [Arrastra el GameObject que tiene MultiImageSpawner]
   ? Navigation Controller: [Se configurará en el siguiente paso]
   ? Navigation UI Manager: [Se configurará más adelante]
   ```

#### C) Crear NavigationArrowController:
1. En la jerarquía: `Create Empty` ? Nombrar: **"NavigationController"**
2. Añadir componente: `NavigationArrowController.cs`
3. Configurar en el Inspector:
   ```
   ? Arrow Prefab: [Arrastra Assets/Prefabs/Flecha-Prefab.prefab]
   ? AR Camera: [Arrastra la Main Camera / AR Camera]
   
   ?? Configuración de Posicionamiento:
   ? Arrow Distance: 2 (metros frente a la cámara)
   ? Arrow Height Offset: -0.5 (0.5m abajo del nivel de ojos)
   ? Arrow Scale: 0.3
   
   ?? Configuración de Rotación:
   ? Rotation Smooth Speed: 8
   ? Enable Vertical Tilt: TRUE
   
   ?? Feedback Visual:
   ? Enable Distance Color Feedback: TRUE
   ? Far Color: Rojo (R:1, G:0, B:0)
   ? Near Color: Verde (R:0, G:1, B:0)
   
   ? Animación:
   ? Enable Pulse Animation: TRUE
   ? Pulse Speed: 2
   ? Pulse Intensity: 0.1
   ```

4. **IMPORTANTE:** Vuelve a **AppModeManager** y asigna este objeto en **Navigation Controller**

---

### **PASO 2: Crear la UI de Navegación**

#### A) Crear Canvas Principal (si no existe):
1. `Hierarchy` ? `Right Click` ? `UI` ? `Canvas`
2. Nombrar: **"NavigationCanvas"**
3. Configurar Canvas:
   ```
   ? Render Mode: Screen Space - Overlay
   ? Canvas Scaler:
      - UI Scale Mode: Scale With Screen Size
      - Reference Resolution: 1080 x 1920 (portrait)
      - Match: 0.5
   ```

#### B) Crear Panel de Selección de Lugares:
1. Dentro del Canvas: `Right Click` ? `UI` ? `Panel`
2. Nombrar: **"LocationSelectionPanel"**
3. Configurar:
   ```
   ? Color: Semi-transparente (R:0, G:0, B:0, A:0.8)
   ? Anchors: Stretch/Stretch (ocupa toda la pantalla)
   ```

4. Añadir título:
   - `Right Click` en LocationSelectionPanel ? `UI` ? `Text - TextMeshPro`
   - Nombrar: **"TitleText"**
   - Texto: "?? Selecciona un Destino"
   - Configurar:
     ```
     ? Font Size: 32
     ? Alignment: Center/Top
     ? Position: Top (Y: -50)
     ```

5. Añadir ScrollView para lista de lugares:
   - `Right Click` en LocationSelectionPanel ? `UI` ? `Scroll View`
   - Nombrar: **"LocationScrollView"**
   - Configurar:
     ```
     ? Anchors: Stretch/Stretch
     ? Offsets: Left: 20, Right: -20, Top: -100, Bottom: 100
     ```

6. Configurar el Content del ScrollView:
   - Seleccionar: **LocationScrollView/Viewport/Content**
   - Añadir componente: **Vertical Layout Group**
   - Configurar:
     ```
     ? Child Force Expand: Width = TRUE, Height = FALSE
     ? Spacing: 10
     ? Padding: 10 en todos lados
     ```
   - Añadir componente: **Content Size Fitter**
   - Configurar:
     ```
     ? Vertical Fit: Preferred Size
     ```

7. Añadir texto de loading:
   - `Right Click` en LocationSelectionPanel ? `UI` ? `Text - TextMeshPro`
   - Nombrar: **"LoadingText"**
   - Texto: "Cargando lugares..."
   - Configurar:
     ```
     ? Font Size: 24
     ? Alignment: Center/Middle
     ? Anchors: Center/Center
     ```

8. Añadir botón de cerrar:
   - `Right Click` en LocationSelectionPanel ? `UI` ? `Button - TextMeshPro`
   - Nombrar: **"CloseButton"**
   - Configurar:
     ```
     ? Position: Top-Right corner
     ? Text: "?"
     ? Font Size: 36
     ```

#### C) Crear Panel de Navegación Activa:
1. Dentro del Canvas: `Right Click` ? `UI` ? `Panel`
2. Nombrar: **"NavigationActivePanel"**
3. Configurar:
   ```
   ? Anchors: Top/Stretch
   ? Height: 150
   ? Color: Semi-transparente (R:0, G:0.2, B:0.4, A:0.9)
   ```

4. Añadir nombre del destino:
   - `Right Click` en NavigationActivePanel ? `UI` ? `Text - TextMeshPro`
   - Nombrar: **"DestinationNameText"**
   - Texto: "Destino"
   - Configurar:
     ```
     ? Font Size: 28
     ? Alignment: Center/Top
     ? Position: Y: -10
     ? Font Style: Bold
     ```

5. Añadir texto de distancia:
   - `Right Click` en NavigationActivePanel ? `UI` ? `Text - TextMeshPro`
   - Nombrar: **"DistanceText"**
   - Texto: "--- m"
   - Configurar:
     ```
     ? Font Size: 36
     ? Alignment: Center/Middle
     ? Font Style: Bold
     ? Color: Verde brillante
     ```

6. Añadir texto de dirección:
   - `Right Click` en NavigationActivePanel ? `UI` ? `Text - TextMeshPro`
   - Nombrar: **"DirectionText"**
   - Texto: "N (0°)"
   - Configurar:
     ```
     ? Font Size: 20
     ? Alignment: Center/Bottom
     ? Position: Y: 10
     ```

7. Añadir botón de cancelar:
   - `Right Click` en NavigationActivePanel ? `UI` ? `Button - TextMeshPro`
   - Nombrar: **"CancelNavigationButton"**
   - Configurar:
     ```
     ? Position: Bottom-Center
     ? Text: "? Cancelar Navegación"
     ? Font Size: 18
     ? Color: Rojo
     ```

#### D) Crear Botón para Abrir Panel de Lugares:
1. Dentro del Canvas: `Right Click` ? `UI` ? `Button - TextMeshPro`
2. Nombrar: **"OpenLocationPanelButton"**
3. Configurar:
   ```
   ? Anchors: Bottom/Right
   ? Position: X: -100, Y: 100
   ? Size: 180 x 180 (botón grande circular)
   ? Text: "??\nNavegar"
   ? Font Size: 24
   ? Color: Azul brillante
   ```

#### E) Configurar NavigationUIManager:
1. Selecciona el GameObject **NavigationCanvas**
2. Añadir componente: `NavigationUIManager.cs`
3. Configurar en el Inspector:
   ```
   ?? Paneles UI:
   ? Location Selection Panel: [Arrastra LocationSelectionPanel]
   ? Navigation Active Panel: [Arrastra NavigationActivePanel]
   
   ?? Lista de Lugares:
   ? Location List Content: [Arrastra LocationScrollView/Viewport/Content]
   ? Location Button Prefab: [Arrastra Assets/Prefabs/LocationButton.prefab]
   ? Loading Text: [Arrastra LoadingText]
   
   ?? Panel de Navegación Activa:
   ? Destination Name Text: [Arrastra DestinationNameText]
   ? Distance Text: [Arrastra DistanceText]
   ? Direction Text: [Arrastra DirectionText]
   ? Cancel Navigation Button: [Arrastra CancelNavigationButton]
   
   ?? Botones Principales:
   ? Open Location Panel Button: [Arrastra OpenLocationPanelButton]
   
   ?? Configuración:
   ? Load Locations On Start: TRUE
   ```

4. **IMPORTANTE:** Vuelve a **AppModeManager** y asigna **NavigationCanvas** en **Navigation UI Manager**

---

### **PASO 3: Configurar Permisos Android**

#### A) Player Settings:
1. `Edit` ? `Project Settings` ? `Player`
2. En la sección **Android**:
   ```
   ? Minimum API Level: Android 7.0 (API 24) o superior
   ? Target API Level: Automatic (Highest Installed)
   ? Internet Access: Require
   ```

#### B) Permisos de Ubicación:
Unity debería añadir automáticamente los permisos de ubicación cuando uses `Input.location`, pero si no funciona:

1. Busca o crea: `Assets/Plugins/Android/AndroidManifest.xml`
2. Añade dentro de `<manifest>`:
   ```xml
   <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
   <uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
   ```

---

### **PASO 4: Testing**

#### ?? Testing en Unity Editor:
1. **Play** en Unity Editor
2. Verifica en Console:
   ```
   [LocationManager] ?? Modo simulación activado (Editor)
   [FirebaseManager] ?? Modo simulación - Retornando datos de prueba
   ```
3. Click en el botón **"?? Navegar"**
4. Selecciona un lugar de la lista
5. Deberías ver:
   - La flecha aparece frente a la cámara
   - Los textos de distancia y dirección se actualizan
   - Puedes rotar la vista con las flechas del teclado (? ?) para simular cambio de orientación

#### ?? Testing en Android:
1. **Build and Run**
2. **IMPORTANTE:** Al abrir la app, acepta los permisos de ubicación
3. Espera a que el GPS inicialice (puede tomar 10-30 segundos)
4. Verifica en Logcat:
   ```
   [LocationManager] ? GPS inicializado correctamente
   [LocationManager] ?? Ubicación inicial: ...
   ```
5. Click en **"?? Navegar"**
6. Selecciona un lugar
7. Mueve el dispositivo y observa cómo la flecha rota

---

## ?? FLUJO DE USO

### **Modo 1: Tracking de Patrones**
1. Usuario abre la app
2. Apunta la cámara a un marcador AR
3. Ve información del lugar en el panel AR

### **Modo 2: Navegación con Brújula**
1. Usuario hace click en **"?? Navegar"**
2. Se abre panel con lista de lugares
3. Selecciona un destino
4. Aparece flecha AR apuntando al destino
5. Panel superior muestra:
   - Nombre del destino
   - Distancia en metros/km
   - Dirección cardinal (N, NE, E, etc.)
6. Usuario mueve el dispositivo y la flecha rota automáticamente
7. Para cancelar: Click en **"? Cancelar Navegación"**

---

## ?? PERSONALIZACIÓN

### Ajustar distancia de la flecha:
```csharp
// En NavigationArrowController:
public float arrowDistance = 2f; // Cambiar a 1.5f o 3f
```

### Cambiar colores según distancia:
```csharp
// En NavigationArrowController Inspector:
Far Color: Rojo (lejos)
Near Color: Verde (cerca)
```

### Ajustar velocidad de rotación:
```csharp
// En NavigationArrowController:
public float rotationSmoothSpeed = 8f; // Mayor = más rápido
```

### Cambiar precisión del GPS:
```csharp
// En LocationManager:
public float desiredAccuracyInMeters = 10f; // Menor = más batería
```

---

## ?? TROUBLESHOOTING

### ? "GPS no está habilitado por el usuario"
**Solución:** Activa la ubicación en los ajustes del dispositivo

### ? "Timeout al inicializar GPS"
**Solución:** 
- Sal al exterior (GPS no funciona bien en interiores)
- Espera más tiempo (puede tardar 1-2 minutos la primera vez)
- Aumenta `maxWaitTime` en LocationManager

### ? La flecha no apunta correctamente
**Solución:**
- Calibra la brújula del dispositivo (hacer figura de 8 en el aire)
- Aléjate de objetos metálicos o imanes
- Verifica que `Input.compass.enabled` esté true

### ? No se cargan los lugares
**Solución:**
- Verifica que FirebaseManager esté en la escena
- Revisa que las reglas de Firestore permitan lectura
- En modo simulación, verifica que `simulateDataInEditor = true`

### ? La flecha es muy grande/pequeña
**Solución:**
- Ajusta `arrowScale` en NavigationArrowController (prueba 0.2 a 0.5)

### ? La UI no se ve
**Solución:**
- Verifica que Canvas esté en modo Screen Space - Overlay
- Asegúrate de que EventSystem existe en la escena
- Revisa que los paneles tengan alpha > 0

---

## ?? LOGS ÚTILES

### Sistema Funcionando Correctamente:
```
[LocationManager] ?? Inicializando servicios de ubicación...
[LocationManager] ? GPS inicializado correctamente
[LocationManager] ?? Ubicación inicial: 13.718103, -89.204092
[LocationManager] ?? Brújula inicializada
[NavigationUIManager] ? Inicializado
[NavigationUIManager] ?? Cargando lugares desde Firebase...
[FirebaseManager] ?? Obteniendo lista de todos los edificios...
[FirebaseManager] ? Se obtuvieron 5 edificios
[NavigationUIManager] ? Se cargaron 5 lugares
[AppModeManager] ?? Iniciando navegación hacia: Biblioteca Central
[NavigationArrowController] ?? Destino establecido: Biblioteca Central
[NavigationArrowController] ? Flecha de navegación activa
```

---

## ?? PRÓXIMAS MEJORAS (OPCIONAL)

- [ ] Vibración cuando el usuario está mirando en la dirección correcta
- [ ] Notificación cuando llegas al destino (< 10 metros)
- [ ] Historial de lugares visitados
- [ ] Compartir ubicación de destino
- [ ] Modo AR Cloud Anchors para interiores
- [ ] Ruta con múltiples waypoints
- [ ] Integración con Google Maps para vista de mapa

---

## ? CHECKLIST FINAL

Antes de hacer build:
- [ ] LocationManager configurado y en la escena
- [ ] AppModeManager configurado con todas las referencias
- [ ] NavigationArrowController configurado con el prefab de flecha
- [ ] NavigationUIManager configurado con todos los elementos UI
- [ ] FirebaseManager con colección correcta
- [ ] Permisos de ubicación en AndroidManifest.xml
- [ ] Testing en Unity Editor (modo simulación)
- [ ] Build and Run en dispositivo Android
- [ ] Aceptar permisos de ubicación en dispositivo
- [ ] Verificar que GPS inicializa correctamente
- [ ] Probar navegación a varios destinos

---

**¡Sistema de Navegación AR completo! ??**

Tu app ahora tiene dos modos:
1. **?? Tracking de Patrones** - Detecta marcadores y muestra información
2. **?? Navegación AR** - Flecha que apunta a destinos seleccionados

Cualquier duda o problema, revisa los logs en consola/Logcat.
