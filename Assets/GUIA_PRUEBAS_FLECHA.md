# ?? GUÍA DE PRUEBAS - Sistema de Flecha de Navegación

## ?? PREPARACIÓN

### **Antes de empezar:**
- [ ] Build instalado en dispositivo Android
- [ ] Permisos de ubicación activados
- [ ] Estar en área abierta (campus)
- [ ] `adb logcat` corriendo para ver logs

---

## ? TEST 1: VERIFICAR QUE LA FLECHA APUNTA CORRECTAMENTE

### **Objetivo:** Confirmar que la flecha apunta en la dirección geográfica correcta.

### **Pasos:**

#### **1.1 Prueba básica - Norte**
```
1. Abre la app
2. Inicia navegación a cualquier edificio
3. Usa una brújula física o app de brújula
4. Párate mirando al NORTE (0°)
5. Observa hacia dónde apunta la flecha AR

Resultado esperado:
? La flecha apunta en la dirección GEOGRÁFICA del nodo
? NO debe girar cuando giras la cámara
? Debe mantener su orientación mundial
```

#### **1.2 Prueba de rotación - 360°**
```
1. Mantén la navegación activa
2. Gira tu cuerpo 90° a la derecha (mirando Este)
3. Observa la flecha

Resultado esperado:
? La flecha mantiene la misma orientación MUNDIAL
? Desde tu perspectiva, la flecha se movió a la IZQUIERDA
```

```
4. Gira otros 90° a la derecha (mirando Sur)
5. Observa la flecha

Resultado esperado:
? La flecha sigue apuntando en la misma dirección mundial
? Desde tu perspectiva, puede estar a tu IZQUIERDA o ATRÁS
```

```
6. Gira 180° (mirando Norte otra vez)
7. Observa la flecha

Resultado esperado:
? La flecha vuelve a estar en la MISMA posición que al inicio
```

### **Logs a buscar:**
```
[NavigationArrowController] ????????????????????
[NavigationArrowController] ?? Objetivo: [Nombre del nodo]
[NavigationArrowController] ?? Bearing al objetivo: 45.3° (desde Norte)
[NavigationArrowController] ?? Bearing dispositivo: 10.5°
[NavigationArrowController] ?? Ángulo relativo: 34.8°
[NavigationArrowController]    ? Objetivo está ADELANTE
```

### **? ÉXITO si:**
- La flecha mantiene orientación mundial
- No gira con la cámara
- Apunta consistentemente al nodo

### **? FALLA si:**
- La flecha gira con la cámara
- Apunta en dirección opuesta (180°)
- Cambia de dirección erráticamente

---

## ? TEST 2: VERIFICAR ACTUALIZACIÓN AL CAMBIAR NODO

### **Objetivo:** Confirmar que la flecha se actualiza cuando llegas a un nodo.

### **Pasos:**

```
1. Inicia navegación a un edificio lejano (>100m)
2. Observa el primer nodo en la ruta
   Log: "[GraphNavigationManager] ?? Nuevo nodo objetivo: Nodo A"
3. Camina hacia el nodo A
4. Cuando llegues a <10m del nodo:
   
Resultado esperado:
? Log: "[GraphNavigationManager] ? Llegaste al nodo: Nodo A"
? Log: "[NavigationArrowController] ?? Nuevo nodo objetivo: Nodo B"
? La flecha cambia de dirección inmediatamente
? Apunta al siguiente nodo (Nodo B)
```

### **Logs a buscar:**
```
[GraphNavigationManager] ? Llegaste al nodo: Nodo A
[GraphNavigationManager] ?? Nuevo nodo objetivo: Nodo B
[GraphNavigationManager] ?? Progreso: 2/7
[NavigationArrowController] ?? Nuevo nodo objetivo: Nodo B
[NavigationArrowController] ?? Coordenadas: 13.722100, -89.226500
```

### **? ÉXITO si:**
- Flecha cambia automáticamente al llegar al nodo
- Apunta al nuevo nodo correcto
- Progreso se actualiza (2/7, 3/7, etc.)

### **? FALLA si:**
- Flecha sigue apuntando al nodo anterior
- No hay log de "Nuevo nodo objetivo"
- Progreso no cambia

---

## ? TEST 3: VERIFICAR RECALCULACIÓN DE RUTA

### **Objetivo:** Confirmar que la flecha se actualiza cuando se recalcula la ruta.

### **Pasos:**

```
1. Inicia navegación
2. Camina en dirección OPUESTA a la ruta sugerida
3. Aléjate más de 50 metros de cualquier nodo de la ruta
4. Acércate a otro nodo del grafo (fuera de tu ruta original)

Resultado esperado:
? Log: "[GraphNavigationManager] ?? DESVIACIÓN DETECTADA!"
? Log: "[GraphNavigationManager] ?? Recalculando ruta desde Nodo X..."
? Log: "[NavigationArrowController] ?? RUTA RECALCULADA"
? La flecha cambia de dirección
? Apunta al nuevo primer nodo de la ruta recalculada
```

### **Logs a buscar:**
```
[GraphNavigationManager] ?? Verificación de desviación:
   ?? Posición GPS actual: 13.722500, -89.227000
   ?? Usuario lejos de la ruta (>30m)
   ?? Buscando nodo más cercano en todo el grafo...
   ? Nodo más cercano encontrado: Nodo X
  - Distancia al más cercano: 25.3m
  - Distancia mínima a ruta: 85.7m
  - ?? ¿Debe recalcular? true

[GraphNavigationManager] ?? DESVIACIÓN DETECTADA!
[GraphNavigationManager] ?? Recalculando ruta desde Nodo X...
[GraphNavigationManager] ? Ruta recalculada: 5 nodos
[NavigationArrowController] ?? RUTA RECALCULADA - Actualizando dirección
```

### **? ÉXITO si:**
- Sistema detecta desviación (>30m de ruta)
- Recalcula ruta automáticamente
- Flecha apunta al nuevo nodo
- Logs muestran el proceso completo

### **? FALLA si:**
- No detecta desviación
- No recalcula aunque estés lejos
- Flecha no se actualiza

---

## ? TEST 4: VERIFICAR QUE NO RECALCULA FALSAMENTE

### **Objetivo:** Confirmar que NO recalcula cuando estás siguiendo la ruta correctamente.

### **Pasos:**

```
1. Inicia navegación
2. Camina hacia el primer nodo siguiendo la flecha
3. Mantente dentro de 30 metros de algún nodo de la ruta
4. Observa los logs cada 2 segundos

Resultado esperado:
? Log: "[GraphNavigationManager] ?? Verificación de desviación:"
? Log: "   ? Usuario cerca de la ruta (dentro de 30m)"
? Log: "   ? Usuario en ruta correcta - No recalcular"
? NO debe aparecer: "?? DESVIACIÓN DETECTADA"
? NO debe recalcular
```

### **Logs a buscar:**
```
[GraphNavigationManager] ?? Verificación de desviación:
   ?? Posición GPS actual: 13.722052, -89.226997
   ?? Verificando distancia a nodos de la ruta actual...
   ?? Distancia mínima a ruta: 18.5m (nodo: Nodo B)
   ? ¿Está cerca de la ruta?: true
   ? Usuario cerca de la ruta (dentro de 30m)
  - Nodo objetivo actual: Nodo B
  - Distancia al objetivo: 18.5m
  - Nodo más cercano en ruta: Nodo B
  - Distancia al más cercano: 18.5m
  - ¿Está en la ruta?: true
   ? Usuario en ruta correcta - No recalcular
```

### **? ÉXITO si:**
- Logs muestran "Usuario en ruta correcta"
- NO recalcula mientras sigues la ruta
- Flecha mantiene dirección estable

### **? FALLA si:**
- Recalcula cuando estás cerca del nodo correcto
- Aparece "DESVIACIÓN DETECTADA" innecesariamente
- Flecha cambia de dirección erráticamente

---

## ? TEST 5: VERIFICAR CALIBRACIÓN DE BRÚJULA

### **Objetivo:** Confirmar que la brújula del dispositivo funciona correctamente.

### **Pasos:**

```
1. Inicia la app (sin navegación)
2. Busca en los logs iniciales:
   "[LocationManager] ? Brújula inicializada correctamente"
   "[LocationManager] ?? TrueHeading: 45.3°"
   
3. Compara con una brújula física o app de brújula
4. Gira el dispositivo 90°
5. Verifica que el heading cambia ~90°

Resultado esperado:
? Brújula inicializada
? TrueHeading actualiza al girar
? Valores coinciden con brújula física (±10°)
```

### **Si la brújula NO funciona:**
```
?? Log: "[LocationManager] ?? Brújula no proporcionó datos"
?? Log: "Calibra la brújula moviendo el teléfono en forma de 8"

Solución:
1. Calibrar brújula (figura de 8 en el aire)
2. Alejar de objetos metálicos
3. Reiniciar la app
```

---

## ? TEST 6: VERIFICAR PRECISIÓN DE GPS

### **Objetivo:** Confirmar que el GPS tiene suficiente precisión.

### **Pasos:**

```
1. Estar en área ABIERTA (no cerca de edificios altos)
2. Esperar 30 segundos para que GPS se estabilice
3. Buscar en logs:
   "[LocationManager] ?? Ubicación actualizada: 13.722052, -89.226997 (±5.3m)"

Resultado esperado:
? Precisión < 10 metros (±5m ideal)
? Coordenadas estables (no saltan >20m)
```

### **Si GPS es impreciso:**
```
?? Precisión > 20 metros
?? Coordenadas saltan erráticamente

Solución:
1. Esperar más tiempo (hasta 2 minutos)
2. Moverse a área más abierta
3. Verificar que GPS del dispositivo esté activado
```

---

## ?? TABLA DE RESULTADOS

| Test | Estado | Notas |
|------|--------|-------|
| ? Test 1: Flecha apunta correctamente | ? | |
| ? Test 2: Actualiza al cambiar nodo | ? | |
| ? Test 3: Recalcula cuando desvías | ? | |
| ? Test 4: NO recalcula falsamente | ? | |
| ? Test 5: Brújula calibrada | ? | |
| ? Test 6: GPS preciso | ? | |

---

## ?? TROUBLESHOOTING

### **Problema: Flecha apunta en dirección opuesta**

**Verificar:**
```csharp
// En Inspector de Unity, NavigationArrowController:
invertArrowModel = ?

// Si flecha apunta 180° al revés:
invertArrowModel = true;  // Cambiar a true
```

**Prueba:**
1. Cambiar valor en Inspector
2. Build nuevamente
3. Probar

---

### **Problema: Flecha gira con la cámara**

**Verificar en código:**
```csharp
// Línea 200 - Debe ser:
arrowInstance.transform.SetParent(null); // ? NULL

// NO debe ser:
arrowInstance.transform.SetParent(arCamera.transform); // ? INCORRECTO
```

---

### **Problema: Flecha no se actualiza al cambiar nodo**

**Verificar en logs:**
```
¿Aparece? "[NavigationArrowController] ?? Nuevo nodo objetivo:"
```

**Si NO aparece:**
```csharp
// Verificar suscripción (línea 538):
GraphNavigationManager.Instance.OnTargetNodeChanged += OnTargetNodeChanged;
```

**Solución:**
1. Verificar que `OnTargetNodeChanged` se llama
2. Verificar que `currentDestination` se actualiza
3. Reiniciar la app

---

### **Problema: Sistema no detecta desviación**

**Verificar en logs cada 2 segundos:**
```
¿Aparece? "[GraphNavigationManager] ?? Verificación de desviación:"
```

**Si NO aparece:**
```csharp
// Verificar en Inspector:
enableRouteRecalculation = true  // ? Debe estar marcado
```

**Si aparece pero no recalcula:**
```
Ver logs:
- "Distancia mínima a ruta: X m"
- "Distancia al más cercano: Y m"

Si X - Y < 10m ? No recalcula (correcto)
Si X - Y > 10m ? Debería recalcular
```

---

## ?? COMANDO PARA VER LOGS

### **Ver todos los logs relevantes:**
```bash
adb logcat -s Unity:D | grep -E "NavigationArrow|GraphNavigation|LocationManager"
```

### **Ver solo verificaciones de desviación:**
```bash
adb logcat -s Unity:D | grep "Verificación de desviación"
```

### **Ver solo cambios de nodo:**
```bash
adb logcat -s Unity:D | grep "Nuevo nodo objetivo"
```

---

## ? CHECKLIST FINAL

### **Antes de reportar problema:**
- [ ] GPS tiene precisión < 10m
- [ ] Brújula está calibrada
- [ ] `invertArrowModel` configurado correctamente
- [ ] Logs muestran actualizaciones cada 2 segundos
- [ ] Probado en área abierta (no entre edificios)
- [ ] Esperado al menos 1 minuto para estabilización

### **Si todo está OK pero sigue fallando:**
- [ ] Copiar logs completos de una sesión de navegación
- [ ] Tomar screenshot de Inspector (NavigationArrowController)
- [ ] Describir comportamiento exacto observado
- [ ] Indicar en qué test específico falla

---

**Fecha:** $(Get-Date)  
**Versión del sistema:** 1.0  
**Estado:** ? Guía de pruebas lista
