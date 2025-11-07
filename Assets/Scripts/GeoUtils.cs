using System;
using UnityEngine;

/// <summary>
/// Utilidades para cálculos geográficos (GPS, distancias, bearings)
/// </summary>
public static class GeoUtils
{
    private const double EARTH_RADIUS_METERS = 6371000.0; // Radio de la Tierra en metros
    
    /// <summary>
    /// Calcula la distancia en metros entre dos coordenadas GPS usando la fórmula de Haversine
    /// </summary>
    /// <param name="lat1">Latitud del punto 1</param>
    /// <param name="lon1">Longitud del punto 1</param>
    /// <param name="lat2">Latitud del punto 2</param>
    /// <param name="lon2">Longitud del punto 2</param>
    /// <returns>Distancia en metros</returns>
    public static float CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);
        
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return (float)(EARTH_RADIUS_METERS * c);
    }
    
    /// <summary>
    /// Calcula el bearing (dirección) en grados desde el punto 1 hacia el punto 2
    /// El resultado está en el rango [0, 360) donde 0° = Norte, 90° = Este, etc.
    /// </summary>
    /// <param name="lat1">Latitud del punto actual</param>
    /// <param name="lon1">Longitud del punto actual</param>
    /// <param name="lat2">Latitud del destino</param>
    /// <param name="lon2">Longitud del destino</param>
    /// <returns>Bearing en grados [0, 360)</returns>
    public static float CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        double dLon = ToRadians(lon2 - lon1);
        double lat1Rad = ToRadians(lat1);
        double lat2Rad = ToRadians(lat2);
        
        double y = Math.Sin(dLon) * Math.Cos(lat2Rad);
        double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                   Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);
        
        double bearing = Math.Atan2(y, x);
        double bearingDegrees = ToDegrees(bearing);
        
        // Normalizar a [0, 360)
        return (float)((bearingDegrees + 360) % 360);
    }
    
    /// <summary>
    /// Formatea una distancia en metros a un string legible
    /// </summary>
    public static string FormatDistance(float meters)
    {
        if (meters < 1000)
        {
            return $"{meters:F0} m";
        }
        else
        {
            return $"{(meters / 1000f):F2} km";
        }
    }
    
    /// <summary>
    /// Formatea un bearing en una dirección cardinal (N, NE, E, SE, S, SW, W, NW)
    /// </summary>
    public static string BearingToCardinal(float bearing)
    {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        int index = (int)Math.Round(bearing / 45.0) % 8;
        return directions[index];
    }
    
    /// <summary>
    /// Convierte grados a radianes
    /// </summary>
    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
    
    /// <summary>
    /// Convierte radianes a grados
    /// </summary>
    private static double ToDegrees(double radians)
    {
        return radians * 180.0 / Math.PI;
    }
    
    /// <summary>
    /// Normaliza un ángulo al rango [-180, 180)
    /// Útil para calcular la diferencia más corta entre dos ángulos
    /// </summary>
    public static float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
