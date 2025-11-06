using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

/// <summary>
/// Gestor singleton para todas las operaciones con Firebase Firestore
/// Maneja consultas directas por Document ID sin necesidad de queries complejas
/// </summary>
public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    
    [Header("Configuración de Firestore")]
    [Tooltip("Nombre de la colección en Firestore donde están los edificios")]
    public string collectionName = "buildingLocations";
    
    [Tooltip("Habilitar caché local para evitar consultas repetidas al mismo documento")]
    public bool enableCache = true;
    
    [Header("Debug")]
    [Tooltip("Simular datos en Unity Editor (sin necesidad de Firebase)")]
    public bool simulateDataInEditor = false;
    
    private FirebaseFirestore db;
    private bool isInitialized = false;
    private Dictionary<string, BuildingData> cache = new Dictionary<string, BuildingData>();
    
    void Awake()
    {
        // Patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeFirebase();
    }
    
    /// <summary>
    /// Inicializa Firebase y verifica que todas las dependencias estén disponibles
    /// </summary>
    private void InitializeFirebase()
    {
        Debug.Log("[FirebaseManager] ?? Inicializando Firebase...");
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            
            if (dependencyStatus == DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                isInitialized = true;
                Debug.Log("[FirebaseManager] ? Firebase inicializado correctamente");
                Debug.Log($"[FirebaseManager] ?? Colección: '{collectionName}'");
                Debug.Log($"[FirebaseManager] ?? Caché: {(enableCache ? "Activado" : "Desactivado")}");
            }
            else
            {
                Debug.LogError($"[FirebaseManager] ? Error al inicializar Firebase: {dependencyStatus}");
                Debug.LogError("[FirebaseManager] Verifica que google-services.json esté en Assets/ y sea válido");
            }
        });
    }
    
    /// <summary>
    /// Obtiene datos de un edificio/lugar desde Firestore usando el ID del documento
    /// Este método es asíncrono y debe usarse con await
    /// </summary>
    /// <param name="documentId">ID del documento en Firestore (ej: "8YLwj6VOhzT2KPzZDMF9")</param>
    /// <returns>BuildingData con la información del lugar</returns>
    public async Task<BuildingData> GetBuildingDataAsync(string documentId)
    {
        if (string.IsNullOrEmpty(documentId))
        {
            Debug.LogWarning("[FirebaseManager] ?? documentId está vacío");
            return BuildingData.GetFallback(documentId);
        }
        
        // Simular datos en Unity Editor para testing sin Firebase
        #if UNITY_EDITOR
        if (simulateDataInEditor)
        {
            Debug.Log($"[FirebaseManager] ?? Modo simulación activo - Retornando datos de prueba");
            await Task.Delay(500); // Simular latencia de red
            return new BuildingData(
                $"Edificio de Prueba ({documentId.Substring(0, Math.Min(5, documentId.Length))}...)",
                "Este es un dato simulado para testing en Unity Editor.\n\nEn build real se consultará Firebase.",
                13.7181033,
                -89.2040915
            );
        }
        #endif
        
        // Revisar caché primero
        if (enableCache && cache.ContainsKey(documentId))
        {
            Debug.Log($"[FirebaseManager] ? Datos de '{documentId}' obtenidos desde CACHÉ");
            return cache[documentId];
        }
        
        // Verificar que Firebase esté listo
        if (!isInitialized)
        {
            Debug.LogWarning("[FirebaseManager] ?? Firebase no está inicializado aún, esperando...");
            await Task.Delay(1000); // Esperar un segundo
            
            if (!isInitialized)
            {
                Debug.LogError("[FirebaseManager] ? Firebase no se pudo inicializar");
                return BuildingData.GetFallback(documentId);
            }
        }
        
        try
        {
            Debug.Log($"[FirebaseManager] ?? Consultando Firestore: {collectionName}/{documentId}");
            
            // Consulta DIRECTA por ID de documento (no se usa Where())
            DocumentReference docRef = db.Collection(collectionName).Document(documentId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            
            if (!snapshot.Exists)
            {
                Debug.LogWarning($"[FirebaseManager] ?? Documento '{documentId}' NO EXISTE en Firestore");
                Debug.LogWarning($"[FirebaseManager] Verifica que el ID coincida con el nombre en Reference Image Library");
                return BuildingData.GetNotFound(documentId);
            }
            
            // Parsear datos del documento
            BuildingData buildingData = ParseDocument(snapshot);
            
            // Guardar en caché
            if (enableCache)
            {
                cache[documentId] = buildingData;
                Debug.Log($"[FirebaseManager] ?? Datos guardados en caché");
            }
            
            Debug.Log($"[FirebaseManager] ? Datos obtenidos exitosamente: '{buildingData.name}'");
            return buildingData;
        }
        catch (FirebaseException ex)
        {
            // Manejar errores específicos de Firebase
            Debug.LogError($"[FirebaseManager] ? Error Firebase: {ex.ErrorCode}");
            Debug.LogError($"[FirebaseManager] Mensaje: {ex.Message}");
            
            // Error de permisos
            if (ex.ErrorCode == 7) // PERMISSION_DENIED
            {
                Debug.LogError("[FirebaseManager] ?? PERMISSION_DENIED - Las reglas de Firestore están bloqueando la lectura");
                Debug.LogError("[FirebaseManager] Solución: Firebase Console ? Firestore ? Reglas ? Cambiar a 'allow read: if true'");
                return BuildingData.GetPermissionDenied(documentId);
            }
            
            return BuildingData.GetFallback(documentId);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] ? Error al consultar Firestore: {ex.Message}");
            Debug.LogError($"[FirebaseManager] Stack: {ex.StackTrace}");
            return BuildingData.GetFallback(documentId);
        }
    }
    
    /// <summary>
    /// Parsea un DocumentSnapshot de Firestore a un objeto BuildingData
    /// Maneja campos opcionales con valores por defecto
    /// </summary>
    private BuildingData ParseDocument(DocumentSnapshot snapshot)
    {
        var data = new BuildingData();
        
        try
        {
            // Parsear campos - usar valores por defecto si no existen
            if (snapshot.ContainsField("name"))
                data.name = snapshot.GetValue<string>("name");
            else
                data.name = "Sin nombre";
            
            if (snapshot.ContainsField("description"))
                data.description = snapshot.GetValue<string>("description");
            else
                data.description = "Sin descripción disponible";
            
            if (snapshot.ContainsField("latitude"))
                data.latitude = snapshot.GetValue<double>("latitude");
            
            if (snapshot.ContainsField("longitude"))
                data.longitude = snapshot.GetValue<double>("longitude");
            
            Debug.Log($"[FirebaseManager] ?? Documento parseado: {data.name}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FirebaseManager] ? Error al parsear documento: {ex.Message}");
        }
        
        return data;
    }
    
    /// <summary>
    /// Limpia la caché de datos (útil para refrescar información)
    /// </summary>
    public void ClearCache()
    {
        cache.Clear();
        Debug.Log("[FirebaseManager] ??? Caché limpiada");
    }
    
    /// <summary>
    /// Verifica si Firebase está listo para consultas
    /// </summary>
    public bool IsReady()
    {
        return isInitialized;
    }
    
    /// <summary>
    /// Obtiene el número de elementos en caché
    /// </summary>
    public int GetCacheCount()
    {
        return cache.Count;
    }
}
