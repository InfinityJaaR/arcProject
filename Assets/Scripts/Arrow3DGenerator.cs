using UnityEngine;

/// <summary>
/// Genera una flecha 3D proceduralmente que puede usarse como compass
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Arrow3DGenerator : MonoBehaviour
{
    [Header("Dimensiones de la Flecha")]
    [Tooltip("Longitud del cuerpo de la flecha")]
    [SerializeField] private float shaftLength = 1.2f;
    
    [Tooltip("Radio del cuerpo de la flecha")]
    [SerializeField] private float shaftRadius = 0.12f;
    
    [Tooltip("Longitud de la punta de la flecha")]
    [SerializeField] private float headLength = 0.5f;
    
    [Tooltip("Radio de la base de la punta")]
    [SerializeField] private float headRadius = 0.25f;
    
    [Tooltip("Número de lados del cilindro (más = más suave)")]
    [SerializeField] private int segments = 16;

    [Header("Material")]
    [Tooltip("Color de la flecha")]
    [SerializeField] private Color arrowColor = new Color(1f, 0.2f, 0.2f); // Rojo brillante
    
    [Header("Opciones Visuales")]
    [Tooltip("Añadir brillo emisivo para mejor visibilidad")]
    [SerializeField] private bool useEmission = true;
    
    [Tooltip("Intensidad del brillo emisivo")]
    [SerializeField] private float emissionIntensity = 0.3f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        GenerateArrow();
    }

    /// <summary>
    /// Genera la geometría de la flecha
    /// </summary>
    public void GenerateArrow()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        Mesh mesh = CreateArrowMesh();
        meshFilter.mesh = mesh;

        // Crear y aplicar material con mejor visibilidad
        Material material = new Material(Shader.Find("Standard"));
        material.color = arrowColor;
        material.SetFloat("_Metallic", 0.3f);
        material.SetFloat("_Glossiness", 0.8f);
        
        // Añadir emisión para mejor visibilidad en AR
        if (useEmission)
        {
            material.EnableKeyword("_EMISSION");
            Color emissionColor = arrowColor * emissionIntensity;
            material.SetColor("_EmissionColor", emissionColor);
        }
        
        meshRenderer.material = material;
    }

    /// <summary>
    /// Crea el mesh de la flecha completa
    /// </summary>
    private Mesh CreateArrowMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Arrow3D";

        // Calcular número total de vértices
        int shaftVertices = segments * 4; // 2 círculos para el cilindro
        int headVertices = segments * 2 + 1; // Base del cono + vértice superior
        int totalVertices = shaftVertices + headVertices;

        Vector3[] vertices = new Vector3[totalVertices];
        Vector3[] normals = new Vector3[totalVertices];
        Vector2[] uvs = new Vector2[totalVertices];
        
        int vertexIndex = 0;

        // 1. CREAR CUERPO (CILINDRO)
        float angleStep = 360f / segments;
        
        // Círculo inferior del cuerpo (en el origen)
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * shaftRadius;
            float z = Mathf.Sin(angle) * shaftRadius;
            
            vertices[vertexIndex] = new Vector3(x, 0, z);
            normals[vertexIndex] = new Vector3(x, 0, z).normalized;
            uvs[vertexIndex] = new Vector2((float)i / segments, 0);
            vertexIndex++;
        }

        // Círculo superior del cuerpo
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * shaftRadius;
            float z = Mathf.Sin(angle) * shaftRadius;
            
            vertices[vertexIndex] = new Vector3(x, shaftLength, z);
            normals[vertexIndex] = new Vector3(x, 0, z).normalized;
            uvs[vertexIndex] = new Vector2((float)i / segments, 0.5f);
            vertexIndex++;
        }

        // Círculo inferior del cuerpo (para tapa)
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * shaftRadius;
            float z = Mathf.Sin(angle) * shaftRadius;
            
            vertices[vertexIndex] = new Vector3(x, 0, z);
            normals[vertexIndex] = Vector3.down;
            uvs[vertexIndex] = new Vector2((float)i / segments, 0);
            vertexIndex++;
        }

        // Círculo superior del cuerpo (para unión con cabeza)
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * shaftRadius;
            float z = Mathf.Sin(angle) * shaftRadius;
            
            vertices[vertexIndex] = new Vector3(x, shaftLength, z);
            normals[vertexIndex] = Vector3.up;
            uvs[vertexIndex] = new Vector2((float)i / segments, 0.5f);
            vertexIndex++;
        }

        // 2. CREAR CABEZA (CONO)
        int headBaseStart = vertexIndex;
        
        // Base del cono
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * headRadius;
            float z = Mathf.Sin(angle) * headRadius;
            
            Vector3 vertexPos = new Vector3(x, shaftLength, z);
            vertices[vertexIndex] = vertexPos;
            
            // Calcular normal del cono
            Vector3 toTip = new Vector3(0, shaftLength + headLength, 0) - vertexPos;
            Vector3 tangent = new Vector3(-z, 0, x);
            normals[vertexIndex] = Vector3.Cross(tangent, toTip).normalized;
            
            uvs[vertexIndex] = new Vector2((float)i / segments, 0.75f);
            vertexIndex++;
        }

        // Punta del cono
        for (int i = 0; i < segments; i++)
        {
            vertices[vertexIndex] = new Vector3(0, shaftLength + headLength, 0);
            
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * headRadius;
            float z = Mathf.Sin(angle) * headRadius;
            Vector3 basePos = new Vector3(x, shaftLength, z);
            Vector3 toTip = vertices[vertexIndex] - basePos;
            Vector3 tangent = new Vector3(-z, 0, x);
            normals[vertexIndex] = Vector3.Cross(tangent, toTip).normalized;
            
            uvs[vertexIndex] = new Vector2((float)i / segments, 1);
            vertexIndex++;
        }

        // Vértice central para la base del cono (tapa)
        vertices[vertexIndex] = new Vector3(0, shaftLength, 0);
        normals[vertexIndex] = Vector3.down;
        uvs[vertexIndex] = new Vector2(0.5f, 0.5f);
        int coneCenterIndex = vertexIndex;
        vertexIndex++;

        // CREAR TRIÁNGULOS
        int[] triangles = new int[segments * 18]; // 6 caras por segmento * 3 vértices
        int triIndex = 0;

        // Triángulos del cuerpo (lateral del cilindro)
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            
            // Primer triángulo
            triangles[triIndex++] = i;
            triangles[triIndex++] = segments + i;
            triangles[triIndex++] = next;
            
            // Segundo triángulo
            triangles[triIndex++] = next;
            triangles[triIndex++] = segments + i;
            triangles[triIndex++] = segments + next;
        }

        // Tapa inferior del cuerpo
        int bottomCapStart = segments * 2;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            triangles[triIndex++] = bottomCapStart + i;
            triangles[triIndex++] = bottomCapStart + next;
            triangles[triIndex++] = bottomCapStart + i; // Centro en el mismo índice para simplicidad
        }

        // Tapa superior del cuerpo (base de transición)
        int topCapStart = segments * 3;
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            triangles[triIndex++] = topCapStart + i;
            triangles[triIndex++] = topCapStart + i; // Centro
            triangles[triIndex++] = topCapStart + next;
        }

        // Triángulos del cono (lateral)
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            
            triangles[triIndex++] = headBaseStart + i;
            triangles[triIndex++] = headBaseStart + segments + i;
            triangles[triIndex++] = headBaseStart + next;
        }

        // Base del cono (tapa inferior)
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            triangles[triIndex++] = headBaseStart + i;
            triangles[triIndex++] = headBaseStart + next;
            triangles[triIndex++] = coneCenterIndex;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals(); // Recalcular para mejorar el sombreado

        return mesh;
    }

    // Método para regenerar la flecha en el editor
    #if UNITY_EDITOR
    private void OnValidate()
    {
        // Evitar valores negativos
        shaftLength = Mathf.Max(0.1f, shaftLength);
        shaftRadius = Mathf.Max(0.01f, shaftRadius);
        headLength = Mathf.Max(0.1f, headLength);
        headRadius = Mathf.Max(0.01f, headRadius);
        segments = Mathf.Max(3, segments);
        emissionIntensity = Mathf.Max(0f, emissionIntensity);
        
        if (Application.isPlaying && meshFilter != null)
        {
            GenerateArrow();
        }
    }
    #endif
}
