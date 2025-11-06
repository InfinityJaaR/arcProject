using UnityEngine;

/// <summary>
/// Añade un contorno a la flecha 3D para hacerla más visible en móvil
/// </summary>
[RequireComponent(typeof(Arrow3DGenerator))]
public class Arrow3DOutline : MonoBehaviour
{
    [Header("Contorno")]
    [Tooltip("Activar contorno para mayor visibilidad")]
    [SerializeField] private bool enableOutline = true;
    
    [Tooltip("Color del contorno")]
    [SerializeField] private Color outlineColor = Color.white;
    
    [Tooltip("Grosor del contorno")]
    [SerializeField] private float outlineWidth = 0.03f;

    private GameObject outlineObject;
    private Arrow3DGenerator arrowGenerator;

    void Start()
    {
        if (enableOutline)
        {
            CreateOutline();
        }
    }

    /// <summary>
    /// Crea un contorno alrededor de la flecha
    /// </summary>
    public void CreateOutline()
    {
        arrowGenerator = GetComponent<Arrow3DGenerator>();
        
        // Limpiar outline anterior si existe
        if (outlineObject != null)
        {
            DestroyImmediate(outlineObject);
        }

        // Crear objeto hijo para el outline
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        
        // Escalar ligeramente más grande para crear el efecto de contorno
        float scaleMultiplier = 1f + outlineWidth;
        outlineObject.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);

        // Copiar mesh y material
        MeshFilter meshFilter = outlineObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = outlineObject.AddComponent<MeshRenderer>();
        
        // Copiar el mesh de la flecha principal
        MeshFilter mainMeshFilter = GetComponent<MeshFilter>();
        if (mainMeshFilter != null && mainMeshFilter.sharedMesh != null)
        {
            meshFilter.sharedMesh = mainMeshFilter.sharedMesh;
        }

        // Crear material para el outline
        Material outlineMaterial = new Material(Shader.Find("Standard"));
        outlineMaterial.color = outlineColor;
        outlineMaterial.SetFloat("_Metallic", 0f);
        outlineMaterial.SetFloat("_Glossiness", 0.2f);
        
        // Renderizar detrás de la flecha principal
        meshRenderer.material = outlineMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        // Mover el outline detrás en el render order
        meshRenderer.sortingOrder = -1;
    }

    /// <summary>
    /// Actualiza el contorno
    /// </summary>
    public void UpdateOutline()
    {
        if (enableOutline)
        {
            CreateOutline();
        }
        else if (outlineObject != null)
        {
            DestroyImmediate(outlineObject);
        }
    }

    void OnDestroy()
    {
        if (outlineObject != null)
        {
            DestroyImmediate(outlineObject);
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        outlineWidth = Mathf.Max(0.01f, outlineWidth);
        
        if (Application.isPlaying)
        {
            UpdateOutline();
        }
    }
#endif
}
