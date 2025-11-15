# ?? GUÍA RÁPIDA: Añadir Permissions Manager

## ?? USO RÁPIDO (1 CLICK)

### **En Unity Editor:**

```
AR Tools ? Permissions ? ?? Añadir Permissions Manager
```

**Eso es todo.** El script hace automáticamente:

1. ? Crea GameObject "PermissionsManager"
2. ? Añade componente `PermissionsManager`
3. ? Configura `Request On Start: TRUE`
4. ? Configura `Show Rationale: TRUE`
5. ? Configura Script Execution Order (-100)
6. ? Configura LocationManager Execution Order (0)
7. ? Selecciona el GameObject creado

---

## ?? MENÚ COMPLETO

En Unity Editor ? **AR Tools ? Permissions**:

### **?? Añadir Permissions Manager** ?
- Añade y configura PermissionsManager automáticamente
- Configura Script Execution Order
- Listo para usar

### **?? Configurar Script Execution Order**
- Solo configura el orden de ejecución
- Útil si ya tienes PermissionsManager pero no funciona

### **?? Verificar Permisos en AndroidManifest**
- Verifica que AndroidManifest.xml tenga los permisos
- Si no existe, ofrece crearlo
- Si faltan permisos, ofrece añadirlos

### **?? Abrir Guía de Permisos**
- Abre `SOLUCION_PERMISOS_UBICACION.md`
- Documentación completa

---

## ? CHECKLIST

Después de usar el script:

```
? GameObject "PermissionsManager" existe en la escena
? Componente PermissionsManager está añadido
? Request On Start: TRUE
? Show Rationale: TRUE
? Script Execution Order configurado
? Build & Run en Android
? App pide permisos de UBICACIÓN ?
? App pide permisos de CÁMARA
? GPS inicializa correctamente
```

---

## ?? PRÓXIMOS PASOS

1. **Ejecutar el script** (AR Tools ? Permissions ? Añadir Permissions Manager)
2. **Verificar configuración** en Inspector
3. **Build & Run** en Android
4. **Conceder permisos** cuando la app los pida
5. **Probar navegación** - GPS debería funcionar

---

## ?? TROUBLESHOOTING

### **? El script no aparece en el menú:**

**Solución:**
- Espera a que Unity compile
- Verifica que `PermissionsManagerSetup.cs` esté en `Assets/Scripts/Editor/`

### **? Script Execution Order no se configura:**

**Solución:**
- Usa: AR Tools ? Permissions ? ?? Configurar Script Execution Order
- O configúralo manualmente: Edit ? Project Settings ? Script Execution Order

### **? AndroidManifest.xml no tiene permisos:**

**Solución:**
- Usa: AR Tools ? Permissions ? ?? Verificar Permisos en AndroidManifest
- El script ofrecerá crear/actualizar el manifest

---

## ?? CARACTERÍSTICAS

### **Detección Inteligente:**
- Si PermissionsManager ya existe, solo lo selecciona
- No crea duplicados

### **Configuración Automática:**
- Todos los valores óptimos por defecto
- Script Execution Order configurado automáticamente

### **Verificación de AndroidManifest:**
- Verifica permisos existentes
- Ofrece crear/actualizar automáticamente

### **Feedback Visual:**
- Diálogos de confirmación
- Logs detallados en Console
- Selecciona automáticamente el objeto creado

---

## ?? FLUJO COMPLETO

```
1. AR Tools ? Permissions ? Añadir Permissions Manager
   ?
2. Script crea y configura todo automáticamente
   ?
3. Diálogo de confirmación aparece ?
   ?
4. GameObject seleccionado en jerarquía
   ?
5. (Opcional) Verificar AndroidManifest
   ?
6. Build & Run
   ?
7. App pide permisos ?
   ?
8. GPS funciona ?
```

---

## ?? COMPARACIÓN

| Método | Tiempo | Errores |
|--------|--------|---------|
| **Manual** | 5-10 min | Fácil olvidar algo |
| **Con Script** | 10 segundos | Cero |

---

## ? RESULTADO

Después de usar el script:

```
? PermissionsManager añadido y configurado
? Script Execution Order correcto
? Listo para Build & Run
? La app pedirá permisos correctamente
? GPS funcionará
```

---

**¡Usa el script y haz Build & Run para probar!** ??

**Última actualización:** 2025-01-06
