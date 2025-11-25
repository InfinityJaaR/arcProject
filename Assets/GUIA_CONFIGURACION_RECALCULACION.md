# ?? GUÍA DE CONFIGURACIÓN Y USO - RECALCULACIÓN DE RUTA

## ?? CONFIGURACIÓN EN UNITY INSPECTOR

### GraphNavigationManager - Configuración Recomendada

```
???????????????????????????????????????????????
? Graph Navigation Manager (Script)          ?
???????????????????????????????????????????????
? ? Configuración                             ?
?   Node Reached Distance: 10                 ?
?   Update Interval: 1                        ?
?                                             ?
? ? Recalculación de Ruta                     ?
?   ? Enable Route Recalculation             ?
?   Max Deviation Distance: 30                ?
?   Recalculation Cooldown: 5                 ?
?   Deviation Check Interval: 2               ?
???????????????????????????????????????????????
```

---

## ?? AJUSTES POR TIPO DE ENTORNO

### Campus Universitario (Recomendado)
```csharp
enableRouteRecalculation = true
maxDeviationDistance = 30f      // 30 metros
recalculationCooldown = 5f       // 5 segundos
deviationCheckInterval = 2f      // 2 segundos
```

### Ciudad (Calles estrechas)
```csharp
enableRouteRecalculation = true
maxDeviationDistance = 15f      // 15 metros (más sensible)
recalculationCooldown = 3f       // 3 segundos (más frecuente)
deviationCheckInterval = 1.5f    // 1.5 segundos
```

### Área Rural (Caminos amplios)
```csharp
enableRouteRecalculation = true
maxDeviationDistance = 50f      // 50 metros (más tolerante)
recalculationCooldown = 10f      // 10 segundos (menos frecuente)
deviationCheckInterval = 3f      // 3 segundos
```

### Modo Ahorro de Batería
```csharp
enableRouteRecalculation = true
maxDeviationDistance = 40f      
recalculationCooldown = 10f      // Más tiempo entre recalculaciones
deviationCheckInterval = 5f      // Verificaciones menos frecuentes
```

### Modo Desarrollo/Debug
```csharp
enableRouteRecalculation = true
maxDeviationDistance = 10f      // Muy sensible para testing
recalculationCooldown = 2f       // Recalcula rápido
deviationCheckInterval = 1f      // Verifica cada segundo
```

---

## ?? ESCENARIOS DE USO

### Escenario 1: Usuario toma un atajo
```
Situación:
- Usuario navegando de A ? B ? C ? D
- En el nodo B, decide tomar un atajo hacia E
- E no está en la ruta original

Comportamiento del sistema:
1. [t+0s]  Usuario en B, se desvía hacia E
2. [t+2s]  Verificación: Distancia a nodo más cercano (E) = 35m
3. [t+2s]  ?? DESVIACIÓN DETECTADA (35m > 30m y E no está en ruta)
4. [t+2s]  ?? Recalcula: Nueva ruta E ? F ? D
5. [t+2s]  ?? Objetivo actualizado: Nodo E
6. [t+2s]  Flecha apunta a E

Resultado:
? El usuario puede tomar atajos y el sistema se adapta
```

### Escenario 2: Usuario se pierde
```
Situación:
- Usuario navegando de A ? B ? C
- Se confunde y camina en dirección opuesta
- Llega a un nodo X fuera de la ruta

Comportamiento del sistema:
1. [t+0s]  Usuario debería ir a B, pero camina hacia X
2. [t+2s]  Verificación: Nodo más cercano = X (50m de distancia)
3. [t+2s]  ?? DESVIACIÓN DETECTADA
4. [t+2s]  ?? Recalcula: Nueva ruta X ? Y ? C
5. [t+2s]  ?? Nuevo objetivo: Nodo X

Resultado:
? El sistema guía al usuario desde donde está, no desde donde "debería estar"
```

### Escenario 3: Desviación dentro de tolerancia
```
Situación:
- Usuario navegando hacia B
- Camina 15m fuera del camino ideal (evitando obstáculo)

Comportamiento del sistema:
1. [t+0s]  Usuario se desvía ligeramente
2. [t+2s]  Verificación: Distancia al nodo más cercano = 15m
3. [t+2s]  ? OK (15m < 30m tolerancia)
4. [t+2s]  Continúa apuntando a B

Resultado:
? No recalcula por desviaciones pequeñas (evita recalculaciones innecesarias)
```

### Escenario 4: Cooldown activo
```
Situación:
- Sistema recalculó hace 2 segundos
- Usuario se desvía otra vez inmediatamente

Comportamiento del sistema:
1. [t+0s]  Recalculación exitosa
2. [t+2s]  Usuario se desvía de nuevo
3. [t+2s]  Verificación: COOLDOWN ACTIVO (2s < 5s)
4. [t+2s]  ?? No recalcula (esperando cooldown)
5. [t+5s]  Cooldown terminado
6. [t+6s]  Nueva verificación: Puede recalcular si es necesario

Resultado:
? Evita spam de recalculaciones (eficiencia y batería)
```

---

## ?? INTERPRETACIÓN DE LOGS

### Log Normal (Sin desviación)
```
[GraphNavigationManager] ?? Verificación de desviación:
  - Nodo objetivo actual: Edificio A
  - Distancia al objetivo: 45.2m
  - Nodo más cercano: Edificio A
  - Distancia al más cercano: 15.3m
  - ¿Está en la ruta?: true
```
**Interpretación:** ? Todo OK, usuario en camino correcto


### Log de Desviación Detectada
```
[GraphNavigationManager] ?? Verificación de desviación:
  - Nodo objetivo actual: Nodo B
  - Distancia al objetivo: 68.5m
  - Nodo más cercano: Nodo X
  - Distancia al más cercano: 42.1m
  - ¿Está en la ruta?: false

[GraphNavigationManager] ?? DESVIACIÓN DETECTADA!
[GraphNavigationManager] ?? Recalculando ruta desde Nodo X...
```
**Interpretación:** ?? Usuario se desvió, iniciando recalculación


### Log de Recalculación Exitosa
```
[GraphNavigationManager] ?? RECALCULANDO RUTA:
  - Desde: Nodo X
  - Hasta: Edificio Destino

[PathfindingService] ?? Buscando camino: Nodo X ? Edificio Destino
[PathfindingService] ? Camino encontrado (3 nodos, 124.5m):
  1. Nodo X ?
  2. Nodo Y ?
  3. Edificio Destino

[GraphNavigationManager] ? Ruta recalculada: 3 nodos
[GraphNavigationManager] ?? Nueva ruta:
  1. Nodo X
  2. Nodo Y
  3. Edificio Destino
[GraphNavigationManager] ?? Nuevo objetivo: Nodo X
```
**Interpretación:** ? Nueva ruta calculada exitosamente


### Log de Cooldown Activo
```
[GraphNavigationManager] ?? Verificación de desviación:
  - Nodo objetivo actual: Nodo A
  - Distancia al objetivo: 55.0m
  - Nodo más cercano: Nodo B
  - Distancia al más cercano: 38.2m
  - ¿Está en la ruta?: false

(No aparece mensaje de recalculación)
```
**Interpretación:** ?? Desviación detectada pero cooldown activo (esperando)

---

## ?? DEBUGGING Y TROUBLESHOOTING

### Problema: No recalcula cuando debería

#### Checklist:
- [ ] `enableRouteRecalculation` está en **true**
- [ ] La desviación es mayor que `maxDeviationDistance`
- [ ] El cooldown ha expirado (> `recalculationCooldown` segundos)
- [ ] El nodo más cercano realmente NO está en la ruta actual
- [ ] GPS está funcionando correctamente

#### Solución temporal para testing:
```csharp
// En Inspector, reducir para testing:
maxDeviationDistance = 5f        // Muy sensible
recalculationCooldown = 1f       // Cooldown corto
deviationCheckInterval = 1f      // Verifica cada segundo
```


### Problema: Recalcula demasiado frecuentemente

#### Causa probable:
- `maxDeviationDistance` muy bajo
- `recalculationCooldown` muy corto
- GPS con mucho ruido/imprecisión

#### Solución:
```csharp
maxDeviationDistance = 50f       // Más tolerante
recalculationCooldown = 10f      // Cooldown más largo
deviationCheckInterval = 3f      // Verificaciones menos frecuentes
```


### Problema: GPS impreciso causa falsas detecciones

#### Síntomas:
```
[GraphNavigationManager] ?? Verificación de desviación:
  - Distancia al más cercano: 35.2m  (GPS dice que estás aquí)
  - Distancia al más cercano: 12.8m  (2 segundos después)
  - Distancia al más cercano: 41.3m  (4 segundos después)
```

#### Solución:
```csharp
// Aumentar tolerancia
maxDeviationDistance = 50f

// Verificar menos frecuentemente
deviationCheckInterval = 5f

// Cooldown más largo
recalculationCooldown = 10f
```


### Problema: No aparecen logs de verificación

#### Causa:
- La navegación no está activa
- LocationManager no está listo

#### Verificar:
```csharp
Debug.Log($"IsNavigating: {GraphNavigationManager.Instance.IsNavigating()}");
Debug.Log($"GPS Ready: {LocationManager.Instance.IsGPSReady}");
Debug.Log($"Enable Recalculation: {enableRouteRecalculation}");
```

---

## ?? MÉTRICAS DE RENDIMIENTO

### Consumo de Recursos

#### Verificación cada 2 segundos:
- **CPU:** ~1-2% (cálculos de distancia)
- **Memoria:** Despreciable (~100 bytes por verificación)
- **Batería:** Mínimo (solo cálculos, no queries GPS adicionales)

#### Recalculación (Dijkstra):
- **CPU:** ~5-10% durante 0.1-0.5 segundos
- **Memoria:** ~1-5 KB (dependiendo del tamaño del grafo)
- **Batería:** Bajo impacto (operación poco frecuente)

### Recomendaciones de optimización:

#### Para dispositivos de gama baja:
```csharp
deviationCheckInterval = 3f      // Verificar cada 3 segundos
recalculationCooldown = 8f       // Cooldown más largo
```

#### Para maximizar duración de batería:
```csharp
deviationCheckInterval = 5f      
recalculationCooldown = 15f      
maxDeviationDistance = 50f       // Menos recalculaciones
```

---

## ?? TESTS AUTOMATIZADOS (OPCIONAL)

### Test 1: Desviación grande debe recalcular
```csharp
[Test]
public void TestRecalculationOnLargeDeviation()
{
    // Arrange
    SetupNavigationToDestination("BuildingA");
    SimulateUserPosition(outsideRoute: true, distance: 35f);
    
    // Act
    WaitForSeconds(3f); // Esperar verificación
    
    // Assert
    Assert.IsTrue(wasRouteRecalculated);
}
```

### Test 2: Desviación pequeña no debe recalcular
```csharp
[Test]
public void TestNoRecalculationOnSmallDeviation()
{
    // Arrange
    SetupNavigationToDestination("BuildingA");
    SimulateUserPosition(outsideRoute: true, distance: 15f);
    
    // Act
    WaitForSeconds(3f);
    
    // Assert
    Assert.IsFalse(wasRouteRecalculated);
}
```

---

## ?? CASOS DE ÉXITO ESPERADOS

### ? Usuario puede tomar atajos
- El sistema detecta y recalcula automáticamente
- La flecha se actualiza para guiar desde la nueva posición

### ? Usuario se pierde
- No importa dónde esté, el sistema encuentra una ruta desde ahí
- Siempre llega al destino final

### ? Múltiples desviaciones
- El cooldown previene spam de recalculaciones
- El sistema es estable y predecible

### ? Navegación fluida
- No interrumpe la experiencia del usuario
- Las actualizaciones son suaves y continuas

---

**Última actualización:** $(Get-Date)  
**Versión:** 1.0  
**Estado:** ? Producción
