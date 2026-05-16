# arcProject

Este repositorio contiene un proyecto AR mobile desarrollado en Unity como un proyecto para la asignatura de Arquitectura de Computadoras en la Universidad de El Salvador.

---

## ?? CORRECCIÓN RECIENTE (2025-01-07)

### **Errores de compilación corregidos:**
- ? Error `LocationInfo.course` eliminado
- ? Error `LocationInfo.speed` eliminado

Ver detalles: [RESUMEN_CORRECCION_ERRORES.md](./Assets/docs/RESUMEN_CORRECCION_ERRORES.md)

### **?? FIX: Flecha Apunta al Revés (180°)**

Si la flecha apunta al lado opuesto del destino:
- ? Fix implementado con checkbox `invertArrowModel`
- ?? Por defecto MARCADO (basado en reportes)
- ?? Probar en dispositivo y ajustar si necesario

Ver guía: [RESUMEN_FIX_FLECHA_INVERTIDA.md](./Assets/docs/RESUMEN_FIX_FLECHA_INVERTIDA.md)

---

## ?? Sistema de Navegación AR

El proyecto incluye un sistema completo de navegación AR que utiliza GPS y brújula para guiar al usuario hacia destinos seleccionados.

### ? Características Principales

- ?? **GPS de Alta Precisión**: Tracking en tiempo real de la ubicación del usuario
- ?? **Brújula Magnética**: Detección del Norte geográfico real
- ?? **Flecha AR Direccional**: Apunta visualmente hacia el destino
- ?? **Interfaz de Navegación**: Muestra distancia y dirección
- ?? **Sistema de Fallback**: Usa GPS bearing si la brújula no funciona
- ??? **Herramientas de Diagnóstico**: Verificación de sensores

---

## ?? Documentación de Brújula

### ?? ¿Problema con la Brújula? (timestamp: -1)

Si ves en los logs:
```
[LocationManager] ?? Brújula - timestamp: -1
```

**Lee primero:**
- ?? [RESPUESTA_RAPIDA.md](./Assets/docs/RESPUESTA_RAPIDA.md) - Solución inmediata
- ?? [INDICE_DOCUMENTACION_BRUJULA.md](./Assets/docs/INDICE_DOCUMENTACION_BRUJULA.md) - Navegación completa

### ?? Documentación Disponible

| Documento | Descripción |
|-----------|-------------|
| [RESPUESTA_RAPIDA.md](./Assets/docs/RESPUESTA_RAPIDA.md) | Respuestas a preguntas frecuentes |
| [DIAGRAMA_VISUAL_BRUJULA.md](./Assets/docs/DIAGRAMA_VISUAL_BRUJULA.md) | Diagramas visuales del sistema |
| [SOLUCION_BRUJULA_NO_VALORES.md](./Assets/docs/SOLUCION_BRUJULA_NO_VALORES.md) | Soluciones técnicas |
| [GUIA_CALIBRAR_BRUJULA.md](./Assets/docs/GUIA_CALIBRAR_BRUJULA.md) | Calibración paso a paso |
| [GUIA_MAGNETOMETER_DIAGNOSTIC.md](./Assets/docs/GUIA_MAGNETOMETER_DIAGNOSTIC.md) | Herramienta de diagnóstico |
| [EXPLICACION_NORTE_UNITY.md](./Assets/docs/EXPLICACION_NORTE_UNITY.md) | Teoría completa |

---

## ??? Scripts Principales

### Navegación
- **LocationManager.cs** - Gestión de GPS y brújula
- **NavigationArrowController.cs** - Control de flecha AR
- **NavigationUIManager.cs** - Interfaz de usuario
- **GeoUtils.cs** - Cálculos geográficos

### Diagnóstico
- **MagnetometerDiagnostic.cs** - Verificación de sensores
- **CompassCalibrationUI.cs** - UI de calibración
- **CompassDebugPanel.cs** - Panel de debug

### Firebase & Datos
- **FirebaseManager.cs** - Conexión con Firebase
- **BuildingData.cs** - Estructura de datos de edificios

---

## ?? Inicio Rápido

### Prerrequisitos
- Unity 2022.3+ 
- Android SDK
- AR Foundation
- Firebase SDK

### Instalación

1. Clonar el repositorio:
```bash
git clone https://github.com/InfinityJaaR/arcProject.git
cd arcProject
```

2. Abrir en Unity

3. Configurar Firebase:
   - Agregar `google-services.json` en `Assets/`
   - Configurar en Firebase Console

4. Build para Android:
   - File ? Build Settings
   - Platform: Android
   - Build

### Testing en Dispositivo

1. Instalar APK en Android
2. Otorgar permisos de ubicación
3. Calibrar brújula (mover en figura de 8)
4. Salir al exterior para mejor GPS
5. Seleccionar destino y navegar

---

## ?? Sistema de Brújula

### ¿Cómo Funciona?

Unity usa el **magnetómetro** del dispositivo para detectar el campo magnético de la Tierra:

```
?? Magnetómetro ? Norte Magnético
     +
?? GPS ? Declinación magnética
     ?
?? Norte Verdadero
```

### Calibración

Para que la brújula funcione correctamente:

1. Abrir app nativa "Brújula" de Android
2. Mover el teléfono en **forma de 8**
3. Alejarse de objetos metálicos
4. Preferiblemente al aire libre

Ver guía completa: [GUIA_CALIBRAR_BRUJULA.md](./Assets/docs/GUIA_CALIBRAR_BRUJULA.md)

---

## ?? Permisos Requeridos (Android)

```xml
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.INTERNET" />
```

---

## ?? Solución de Problemas

### Brújula muestra 0.0° constante
? Ver [SOLUCION_BRUJULA_NO_VALORES.md](./Assets/docs/SOLUCION_BRUJULA_NO_VALORES.md)

### GPS no inicializa
? Ver [SOLUCION_GPS_NO_INICIALIZA.md](./Assets/docs/SOLUCION_GPS_NO_INICIALIZA.md)

### Flecha AR no apunta correctamente
? Ver [SOLUCION_FLECHA_DIRECCION.md](./Assets/docs/SOLUCION_FLECHA_DIRECCION.md)

### Permisos no funcionan
? Ver [SOLUCION_PERMISOS_UBICACION.md](./Assets/docs/SOLUCION_PERMISOS_UBICACION.md)

---

## ?? Funcionalidades

- ? Navegación AR con flecha direccional
- ? Cálculo de distancia y bearing
- ? Detección automática de Norte geográfico
- ? Fallback a GPS bearing (en movimiento)
- ? Panel de depuración en tiempo real
- ? Sistema de permisos automatizado
- ? Calibración de brújula guiada
- ? Diagnóstico de sensores

---

## ?? Estructura del Proyecto

```
Assets/
??? Scripts/
?   ??? LocationManager.cs
?   ??? NavigationArrowController.cs
?   ??? NavigationUIManager.cs
?   ??? GeoUtils.cs
?   ??? FirebaseManager.cs
?   ??? MagnetometerDiagnostic.cs
?   ??? CompassCalibrationUI.cs
?
??? Prefabs/
?   ??? NavigationCanvas.prefab
?   ??? InfoPanel.prefab
?   ??? LocationButton.prefab
?
??? Documentación/
    ??? INDICE_DOCUMENTACION_BRUJULA.md
    ??? RESPUESTA_RAPIDA.md
    ??? GUIA_CALIBRAR_BRUJULA.md
    ??? ... (ver índice completo)
```

---

## ?? Contribuidores

Proyecto desarrollado para la asignatura de Arquitectura de Computadoras - Universidad de El Salvador

---

## ?? Licencia

[Especificar licencia del proyecto]

---

## ?? Enlaces Útiles

- [Documentación Unity AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@5.0/manual/index.html)
- [Input.compass API](https://docs.unity3d.com/ScriptReference/Input-compass.html)
- [Firebase Unity](https://firebase.google.com/docs/unity/setup)

---

## ?? Soporte

Para problemas con la brújula o navegación, consulta primero:
- [INDICE_DOCUMENTACION_BRUJULA.md](./Assets/docs/INDICE_DOCUMENTACION_BRUJULA.md)
- [RESPUESTA_RAPIDA.md](./Assets/docs/RESPUESTA_RAPIDA.md)

---

**Desarrollado con ?? en la Universidad de El Salvador**
