# ?? EJEMPLO DE ESTRUCTURA FIRESTORE CON NEARBY_PLACES

## ?? Estructura de Documento Completa

### Colección: `buildingLocations`

#### Documento 1: Biblioteca Central
```json
{
  "name": "Biblioteca Central",
  "description": "Sistema bibliotecario moderno que ofrece recursos digitales e impresos para la comunidad universitaria.",
  "latitude": 13.7181033,
  "longitude": -89.2040915,
  "nearby_places": "Cafetería Central, Auditorio Principal, Facultad de Ingeniería, Rectoría"
}
```

#### Documento 2: Facultad de Ingeniería
```json
{
  "name": "Facultad de Ingeniería",
  "description": "Edificio principal de la Facultad de Ingeniería y Arquitectura. Cuenta con laboratorios especializados y aulas equipadas.",
  "latitude": 13.7185000,
  "longitude": -89.2045000,
  "nearby_places": "Biblioteca Central, Laboratorio de Computación, Cafetería de Ingeniería, Estacionamiento Este"
}
```

#### Documento 3: Rectoría
```json
{
  "name": "Rectoría",
  "description": "Edificio administrativo central de la universidad. Oficinas de autoridades y gestión administrativa.",
  "latitude": 13.7178000,
  "longitude": -89.2038000,
  "nearby_places": "Biblioteca Central, Plaza Central, Oficina de Registro Académico"
}
```

#### Documento 4: Sin Lugares Cercanos
```json
{
  "name": "Cafetería del Campus",
  "description": "Principal área de comidas del campus universitario.",
  "latitude": 13.7183000,
  "longitude": -89.2042000,
  "nearby_places": ""
}
```

**NOTA:** Si `nearby_places` está vacío `""`, el sistema lo ocultará automáticamente.

---

## ??? CÓMO AGREGARLO EN FIREBASE CONSOLE

### Método 1: Crear Documento Nuevo

1. **Firebase Console** ? **Firestore Database**
2. Click en **"Add document"**
3. **Document ID:** `8YLwj6VOhzT2KPzZDMF9` (o "Auto-ID")
4. **Agregar campos:**

   | Campo         | Tipo    | Valor                                      |
   |---------------|---------|-------------------------------------------|
   | name          | string  | Biblioteca Central                        |
   | description   | string  | Sistema bibliotecario moderno...          |
   | latitude      | number  | 13.7181033                                |
   | longitude     | number  | -89.2040915                               |
   | nearby_places | string  | Cafetería Central, Auditorio Principal, Facultad de Ingeniería |

5. Click **"Save"**

---

### Método 2: Editar Documento Existente

1. **Firebase Console** ? **Firestore Database**
2. Selecciona la colección: `buildingLocations`
3. Click en un documento (ej: `8YLwj6VOhzT2KPzZDMF9`)
4. Click en **"Add field"**
5. **Field name:** `nearby_places`
6. **Field type:** `string`
7. **Value:** `Cafetería Central, Auditorio Principal, Facultad de Ingeniería`
8. Click **"Save"**

---

## ? VALIDACIÓN

### Verificar que funciona:

1. **Build and Run** en Android
2. **Apunta** a un marcador (ej: Biblioteca Central)
3. **Verifica** en **Logcat**:
   ```
   [FirebaseManager] ?? Lugares cercanos encontrados: Cafetería Central, Auditorio Principal...
   [FirebaseManager] ? Documento parseado: Biblioteca Central
   [InfoPanelController] ? Mostrando lugares cercanos: Cafetería Central, Auditorio Principal...
   ```

4. **En el panel AR** debes ver:
   ```
   ?? Lugares Cercanos:
   Cafetería Central, Auditorio Principal, Facultad de Ingeniería, Rectoría
   ```

---

## ?? TIPS

### Formato Recomendado
- Separa con comas: `"Lugar 1, Lugar 2, Lugar 3"`
- O con saltos de línea: `"Lugar 1\nLugar 2\nLugar 3"`
- Puedes usar emojis: `"??? Cafetería, ?? Biblioteca, ?? Auditorio"`

### Nombres Cortos
- Usa nombres concisos para mejor visualización:
  - ? "Cafetería Central"
  - ? "Cafetería Central del Campus Universitario - Edificio B"

### Orden Lógico
- Ordena por proximidad real (más cercano primero)
- O por importancia/relevancia

### Con Distancias
- Puedes incluir distancias:
  ```
  "Cafetería Central (50m), Auditorio (100m), Biblioteca (150m)"
  ```

---

## ?? RESUMEN

1. ? Campo debe ser tipo **string**
2. ? Puede contener texto separado por comas, saltos de línea, etc.
3. ? Puede estar vacío `""`
4. ? El sistema oculta automáticamente si está vacío
5. ? Se muestra tal cual está escrito en Firestore

**¡Listo para usar! ??**
