using System;
using UnityEngine;

/// <summary>
/// Modelo de datos que representa la información de un edificio/lugar en Firestore
/// Estructura coincide con los documentos en la colección "buildingLocations"
/// </summary>
[Serializable]
public class BuildingData
{
    public string name;            // Nombre del edificio (ej: "Biblioteca Central")
    public string description;     // Descripción detallada del lugar
    public double latitude;        // Coordenada de latitud
    public double longitude;       // Coordenada de longitud
    
    // Constructor vacío para deserialización de Firebase
    public BuildingData() { }
    
    /// <summary>
    /// Constructor para crear datos de prueba o fallback
    /// </summary>
    public BuildingData(string name, string description, double latitude = 0, double longitude = 0)
    {
        this.name = name;
        this.description = description;
        this.latitude = latitude;
        this.longitude = longitude;
    }
    
    /// <summary>
    /// Retorna un objeto BuildingData por defecto cuando no se puede cargar la información
    /// </summary>
    public static BuildingData GetFallback(string documentId)
    {
        return new BuildingData(
            "?? Error de Conexión Firebase",
            $"No se pudo cargar la información para:\n<b>{documentId}</b>\n\n" +
            $"<b>Posibles causas:</b>\n" +
            $"• Reglas de Firestore muy restrictivas\n" +
            $"• Sin conexión a internet\n" +
            $"• google-services.json incorrecto\n\n" +
            $"<b>Solución rápida:</b>\n" +
            $"1. Verifica tu conexión a internet\n" +
            $"2. Ve a Firebase Console ? Firestore ? Reglas\n" +
            $"3. Cambia a: allow read: if true\n\n" +
            $"Ver logs para más detalles.",
            0,
            0
        );
    }
    
    /// <summary>
    /// Retorna un objeto BuildingData cuando el documento no existe en Firestore
    /// </summary>
    public static BuildingData GetNotFound(string documentId)
    {
        return new BuildingData(
            "?? Documento No Encontrado",
            $"El documento con ID:\n<b>{documentId}</b>\n\n" +
            $"No existe en la colección 'buildingLocations'.\n\n" +
            $"<b>Soluciones:</b>\n" +
            $"• Crea el documento en Firebase Console\n" +
            $"• O usa un marcador con un ID que exista\n\n" +
            $"Documentos disponibles:\n" +
            $"• N0UxoKqwo98gRg3cuZUZ\n" +
            $"• 2MpVGjui5ZOxKK1tHzMO\n" +
            $"• 8YLwj6VOhzT2KPzZDMF9\n" +
            $"• Ki03NOqDgcIIUkIelNnF\n" +
            $"• UrvKdEj96fv3VKVQxk2H\n" +
            $"• Yub2zI6C8A0pxtHzbFKk\n" +
            $"• ZBXZWnw3XWFc1ounohc6\n" +
            $"• ZOYQRDxBUw76kLfvQFkL\n" +
            $"• astgWzn9wMViim4d2lmR\n" +
            $"• vQg2RXAAToj6jKwip0mn",
            0,
            0
        );
    }
    
    /// <summary>
    /// Retorna un objeto BuildingData cuando hay error de permisos
    /// </summary>
    public static BuildingData GetPermissionDenied(string documentId)
    {
        return new BuildingData(
            "?? Permisos Denegados",
            $"Firebase rechazó la consulta para:\n<b>{documentId}</b>\n\n" +
            $"<b>Causa:</b>\n" +
            $"Las reglas de seguridad de Firestore están bloqueando las lecturas.\n\n" +
            $"<b>Solución:</b>\n" +
            $"1. Ve a Firebase Console\n" +
            $"2. Firestore Database ? Reglas\n" +
            $"3. Cambia a:\n" +
            $"   allow read: if true;\n" +
            $"4. Publica los cambios\n" +
            $"5. Reinicia la app\n\n" +
            $"Esto es para desarrollo. Usa reglas más seguras en producción.",
            0,
            0
        );
    }
    
    /// <summary>
    /// Retorna las coordenadas formateadas como string
    /// </summary>
    public string GetFormattedCoordinates()
    {
        if (latitude == 0 && longitude == 0)
            return "Coordenadas no disponibles";
        
        return $"?? Lat: {latitude:F6}, Lon: {longitude:F6}";
    }
}
