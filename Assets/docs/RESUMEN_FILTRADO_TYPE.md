# ? RESUMEN: FILTRADO POR TYPE

## ? IMPLEMENTADO

Campo `type` agregado para filtrar lugares navegables.

---

## ?? FUNCIONAMIENTO

- ? **`type="edificio"`** ? Aparece en lista de navegación
- ? **Otros tipos** ? NO aparecen en navegación (pero sí en InfoPanel AR)

---

## ?? EN FIRESTORE

### Navegable (aparece en lista):
```json
{
  "name": "Biblioteca Central",
  "type": "edificio"  ? NAVEGABLE
}
```

### No navegable (solo tracking AR):
```json
{
  "name": "Monumento X",
  "type": "monumento"  ? NO NAVEGABLE
}
```

---

## ?? AGREGAR EN FIREBASE CONSOLE

1. Firebase Console ? Firestore Database
2. Selecciona un documento
3. **Add field:**
   - Name: `type`
   - Type: `string`
   - Value: `edificio` (para navegable)
4. Save

---

## ??? TIPOS SUGERIDOS

**Navegables:**
- `"edificio"` ? ÚNICO NAVEGABLE

**No navegables:**
- `"monumento"`
- `"punto_referencia"`
- `"estacionamiento"`
- `"zona_verde"`

---

## ?? LOGS

```
[NavigationUIManager] ?? Total: 10, Edificios: 7, Filtrados: 3
[NavigationUIManager] ? Se cargaron 7 edificios navegables
[NavigationUIManager] ?? 'Monumento X' filtrado (type='monumento')
```

---

## ?? RESULTADO

### Panel de Navegación:
```
Solo edificios (type="edificio")
```

### InfoPanel AR:
```
Todos los tipos (se ven al trackear)
```

---

## ? CHECKLIST

Al agregar lugares en Firestore:
- [ ] Campo `type` agregado
- [ ] Si navegable: `type="edificio"`
- [ ] Si no navegable: otro tipo

---

**¡Listo! Solo edificios son navegables.** ???

Ver: `GUIA_FILTRADO_TYPE.md` para más detalles.
