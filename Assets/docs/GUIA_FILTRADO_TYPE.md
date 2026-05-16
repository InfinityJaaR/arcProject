# ?? FILTRADO POR TIPO: CAMPO `type` EN FIRESTORE

## ? IMPLEMENTACIÓN COMPLETADA

Se ha agregado el campo `type` para clasificar los lugares y filtrar cuáles son navegables.

---

## ?? FUNCIONALIDAD

### **Filtro de Navegación:**
- Solo los lugares con `type="edificio"` aparecen en la lista de navegación
- Los demás lugares (monumentos, puntos de referencia, etc.) se siguen mostrando al trackear patrones AR
- Pero NO son seleccionables para navegación por brújula

---

## ?? ESTRUCTURA EN FIRESTORE

### Ejemplo de Edificio (Navegable):
```json
{
  "name": "Biblioteca Central",
  "description": "Sistema bibliotecario moderno...",
  "latitude": 13.7181033,
  "longitude": -89.2040915,
  "nearby_places": "Cafetería Central, Auditorio",
  "type": "edificio"  ? NAVEGABLE
}
```

### Ejemplo de Monumento (No Navegable):
```json
{
  "name": "Monumento a la Libertad",
  "description": "Monumento conmemorativo...",
  "latitude": 13.7182000,
  "longitude": -89.2041000,
  "nearby_places": "Plaza Central, Biblioteca",
  "type": "monumento"  ? NO NAVEGABLE
}
```

### Ejemplo de Punto de Referencia (No Navegable):
```json
{
  "name": "Fuente Principal",
  "description": "Fuente decorativa en el centro...",
  "latitude": 13.7179000,
  "longitude": -89.2039000,
  "nearby_places": "Rectoría, Plaza",
  "type": "punto_referencia"  ? NO NAVEGABLE
}
```

---

## ?? CÓMO AGREGAR EL CAMPO EN FIREBASE CONSOLE

### Para Documentos Nuevos:
1. Firebase Console ? Firestore Database
2. Selecciona la colección `buildingLocations`
3. Click en **"Add document"**
4. Agrega los campos:
   ```
   name: "Edificio X"
   description: "..."
   latitude: 13.xxx
   longitude: -89.xxx
   nearby_places: "..."
   type: "edificio"  ? IMPORTANTE
   ```

### Para Documentos Existentes:
1. Firebase Console ? Firestore Database
2. Selecciona un documento de `buildingLocations`
3. Click en **"Add field"**
4. Configurar:
   - **Field name:** `type`
   - **Field type:** `string`
   - **Value:** `edificio` (para que sea navegable)
5. Click **"Save"**

---

## ??? TIPOS RECOMENDADOS

### Navegables (aparecen en lista de navegación):
- `"edificio"` - Edificios principales, facultades, administrativos

### No Navegables (solo se ven al trackear):
- `"monumento"` - Monumentos, estatuas
- `"punto_referencia"` - Fuentes, plazas, jardines
- `"estacionamiento"` - Áreas de parking
- `"zona_verde"` - Parques, áreas verdes
- `"servicio"` - Baños, puntos de información

**NOTA:** Solo `"edificio"` (en minúsculas) hará que el lugar sea navegable.

---

## ?? LOGS DEL SISTEMA

### Cuando se cargan lugares:
```
[NavigationUIManager] ?? Cargando lugares desde Firebase...
[FirebaseManager] ??? Tipo: edificio
[FirebaseManager] ? Documento parseado: Biblioteca Central
[FirebaseManager] ??? Tipo: monumento
[FirebaseManager] ? Documento parseado: Monumento a la Libertad
[NavigationUIManager] ?? 'Monumento a la Libertad' filtrado (type='monumento')
[NavigationUIManager] ?? Total cargados: 10, Edificios navegables: 7, Filtrados: 3
[NavigationUIManager] ? Se cargaron 7 edificios navegables
```

---

## ?? COMPORTAMIENTO DEL SISTEMA

### 1. Panel de Navegación (Selección de Destinos):
```
???????????????????????????????
? ?? Selecciona un Destino    ?
???????????????????????????????
? Biblioteca Central          ? ? type="edificio" ?
? Facultad de Ingeniería      ? ? type="edificio" ?
? Rectoría                    ? ? type="edificio" ?
? Cafetería Central           ? ? type="edificio" ?
?                             ?
? (Monumento NO aparece aquí) ? ? type="monumento" ?
???????????????????????????????
```

### 2. InfoPanel AR (Tracking de Patrones):
- **Edificios:** Se muestran normalmente ?
- **Monumentos:** Se muestran normalmente ?
- **Todos los tipos:** Aparecen al trackear su patrón ?

**El filtro solo aplica a la lista de navegación, NO al tracking AR.**

---

## ? VENTAJAS DEL SISTEMA

1. **Organización:** Separa edificios navegables de puntos de referencia
2. **Mejor UX:** El usuario no ve lugares irrelevantes para navegación
3. **Flexibilidad:** Puedes agregar más tipos según necesites
4. **Escalable:** Fácil agregar categorías nuevas

---

## ?? PERSONALIZACIÓN

### Cambiar el tipo navegable:

Si quieres que otros tipos también sean navegables, edita `BuildingData.cs`:

```csharp
public bool IsEdificio()
{
    if (string.IsNullOrEmpty(type)) return false;
    
    string lowerType = type.ToLower();
    
    // Permitir múltiples tipos navegables
    return lowerType == "edificio" 
        || lowerType == "facultad" 
        || lowerType == "administrativo";
}
```

### Agregar categorías personalizadas:

Puedes crear tus propios tipos:
```
"biblioteca" - Bibliotecas específicas
"laboratorio" - Laboratorios especializados
"aula" - Aulas o salones
"deportivo" - Instalaciones deportivas
```

Y filtrar según necesites en `NavigationUIManager.cs`.

---

## ?? CHECKLIST PARA FIRESTORE

Al agregar o actualizar lugares en Firestore:

- [ ] Campo `name` agregado
- [ ] Campo `description` agregado
- [ ] Campo `latitude` agregado (number)
- [ ] Campo `longitude` agregado (number)
- [ ] Campo `nearby_places` agregado (string, opcional)
- [ ] Campo `type` agregado (string)
  - [ ] Si es navegable: `type="edificio"`
  - [ ] Si no es navegable: `type="monumento"`, `type="punto_referencia"`, etc.

---

## ?? TROUBLESHOOTING

### ? "No hay edificios disponibles para navegación"

**Causa:** Ningún documento tiene `type="edificio"`

**Solución:**
1. Verifica en Firestore que al menos un documento tenga el campo `type`
2. Asegúrate de que el valor sea exactamente `"edificio"` (en minúsculas)
3. Revisa los logs para ver qué tipos se están filtrando

---

### ? Un lugar no aparece en la lista de navegación

**Verifica:**
1. El campo `type` existe en Firestore
2. El valor es exactamente `"edificio"` (minúsculas, sin espacios)
3. Los logs muestran si fue filtrado:
   ```
   [NavigationUIManager] ?? 'Nombre del lugar' filtrado (type='xxx')
   ```

---

### ? Un lugar aparece pero no debería

**Causa:** El campo `type` es `"edificio"` o está vacío

**Solución:**
1. Cambia el `type` a otro valor: `"monumento"`, `"punto_referencia"`, etc.
2. Verifica que se aplicó el cambio en Firestore

---

## ?? RESUMEN

- ? Campo `type` agregado a `BuildingData`
- ? Parser de Firestore actualizado
- ? Filtrado implementado en `NavigationUIManager`
- ? Solo `type="edificio"` es navegable
- ? Otros tipos se siguen mostrando en InfoPanel AR
- ? Logs detallados para debugging

**¡Sistema de filtrado completamente funcional! ??**

---

## ?? ARCHIVOS MODIFICADOS

1. `BuildingData.cs` - Campo `type` y método `IsEdificio()`
2. `FirebaseManager.cs` - Parseo del campo `type`
3. `NavigationUIManager.cs` - Filtrado por tipo
4. Datos simulados actualizados con `type="edificio"`

**Todo listo para usar!** ??
