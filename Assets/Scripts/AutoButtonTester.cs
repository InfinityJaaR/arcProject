using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Script de prueba automática para el botón de navegación
/// Simula clicks y verifica que todo funcione correctamente
/// </summary>
public class AutoButtonTester : MonoBehaviour
{
    [Header("?? Configuración de Testing")]
    [Tooltip("Iniciar test automático al comenzar")]
    public bool autoTestOnStart = false;
    
    [Tooltip("Tiempo de espera antes de iniciar test (segundos)")]
    public float delayBeforeTest = 2f;
    
    [Tooltip("Simular click cada X segundos (0 = solo una vez)")]
    public float testInterval = 0f;
    
    [Header("Referencias")]
    public Button buttonToTest;
    public NavigationUIManager uiManager;
    
    private int testCount = 0;
    private int successCount = 0;
    private int failureCount = 0;
    
    void Start()
    {
        Debug.Log("[AutoButtonTester] ?? Inicializando Auto Button Tester");
        
        if (autoTestOnStart)
        {
            StartCoroutine(RunAutoTest());
        }
    }
    
    void Update()
    {
        // Manual test con tecla T
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[AutoButtonTester] ?? Test manual iniciado con tecla T");
            StartCoroutine(TestButton());
        }
    }
    
    private IEnumerator RunAutoTest()
    {
        Debug.Log($"[AutoButtonTester] ? Esperando {delayBeforeTest} segundos antes de iniciar test...");
        yield return new WaitForSeconds(delayBeforeTest);
        
        // Buscar referencias si no están asignadas
        FindReferences();
        
        if (buttonToTest == null)
        {
            Debug.LogError("[AutoButtonTester] ? No se puede ejecutar test: botón no encontrado");
            yield break;
        }
        
        if (uiManager == null)
        {
            Debug.LogError("[AutoButtonTester] ? No se puede ejecutar test: NavigationUIManager no encontrado");
            yield break;
        }
        
        // Test inicial
        yield return StartCoroutine(TestButton());
        
        // Tests periódicos si está configurado
        if (testInterval > 0)
        {
            Debug.Log($"[AutoButtonTester] ?? Tests periódicos cada {testInterval} segundos");
            
            while (true)
            {
                yield return new WaitForSeconds(testInterval);
                yield return StartCoroutine(TestButton());
            }
        }
    }
    
    private IEnumerator TestButton()
    {
        testCount++;
        
        Debug.Log("[AutoButtonTester] ???????????????????????????????????");
        Debug.Log($"[AutoButtonTester] ?? TEST #{testCount} INICIADO");
        Debug.Log("[AutoButtonTester] ???????????????????????????????????");
        
        // Paso 1: Verificar estado inicial
        Debug.Log("[AutoButtonTester] ?? Paso 1: Verificando estado inicial...");
        
        if (buttonToTest == null)
        {
            Debug.LogError("[AutoButtonTester] ? Botón es NULL");
            failureCount++;
            yield break;
        }
        
        Debug.Log($"[AutoButtonTester] ? Botón encontrado: {buttonToTest.gameObject.name}");
        
        if (uiManager == null)
        {
            Debug.LogError("[AutoButtonTester] ? NavigationUIManager es NULL");
            failureCount++;
            yield break;
        }
        
        Debug.Log($"[AutoButtonTester] ? NavigationUIManager encontrado");
        
        if (uiManager.locationSelectionPanel == null)
        {
            Debug.LogError("[AutoButtonTester] ? locationSelectionPanel es NULL");
            failureCount++;
            yield break;
        }
        
        Debug.Log($"[AutoButtonTester] ? locationSelectionPanel encontrado");
        
        // Paso 2: Verificar estado del panel antes del click
        bool panelWasActive = uiManager.locationSelectionPanel.activeSelf;
        Debug.Log($"[AutoButtonTester] ?? Panel activo antes del click: {panelWasActive}");
        
        // Si el panel ya está activo, cerrarlo primero
        if (panelWasActive)
        {
            Debug.Log("[AutoButtonTester] ?? Panel ya estaba activo, cerrándolo primero...");
            uiManager.locationSelectionPanel.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        
        // Paso 3: Simular click
        Debug.Log("[AutoButtonTester] ?? Paso 2: Simulando click en el botón...");
        
        if (!buttonToTest.interactable)
        {
            Debug.LogError("[AutoButtonTester] ? El botón NO es interactable!");
            failureCount++;
            yield break;
        }
        
        Debug.Log("[AutoButtonTester] ??? Invocando onClick...");
        buttonToTest.onClick.Invoke();
        
        // Esperar un frame para que se procese el click
        yield return null;
        
        // Paso 4: Verificar resultado
        Debug.Log("[AutoButtonTester] ?? Paso 3: Verificando resultado...");
        
        bool panelIsActiveNow = uiManager.locationSelectionPanel.activeSelf;
        Debug.Log($"[AutoButtonTester] ?? Panel activo después del click: {panelIsActiveNow}");
        
        // Paso 5: Evaluar resultado
        if (panelIsActiveNow)
        {
            Debug.Log("[AutoButtonTester] ???????????????????????????????????");
            Debug.Log($"[AutoButtonTester] ? TEST #{testCount} EXITOSO");
            Debug.Log("[AutoButtonTester] ? El panel se abrió correctamente");
            Debug.Log("[AutoButtonTester] ???????????????????????????????????");
            successCount++;
        }
        else
        {
            Debug.Log("[AutoButtonTester] ???????????????????????????????????");
            Debug.LogError($"[AutoButtonTester] ? TEST #{testCount} FALLIDO");
            Debug.LogError("[AutoButtonTester] ? El panel NO se abrió");
            Debug.Log("[AutoButtonTester] ???????????????????????????????????");
            failureCount++;
            
            // Debugging adicional
            Debug.Log("[AutoButtonTester] ?? Información de debugging:");
            Debug.Log($"[AutoButtonTester]    - Botón activo: {buttonToTest.gameObject.activeInHierarchy}");
            Debug.Log($"[AutoButtonTester]    - Botón interactable: {buttonToTest.interactable}");
            Debug.Log($"[AutoButtonTester]    - UIManager activo: {uiManager.gameObject.activeInHierarchy}");
            Debug.Log($"[AutoButtonTester]    - UIManager enabled: {uiManager.enabled}");
            Debug.Log($"[AutoButtonTester]    - Panel activo: {uiManager.locationSelectionPanel.activeSelf}");
            Debug.Log($"[AutoButtonTester]    - Panel en jerarquía: {uiManager.locationSelectionPanel.activeInHierarchy}");
            
            // Intentar abrir manualmente
            Debug.Log("[AutoButtonTester] ?? Intentando abrir panel manualmente...");
            uiManager.locationSelectionPanel.SetActive(true);
            yield return null;
            
            bool manualSuccess = uiManager.locationSelectionPanel.activeSelf;
            Debug.Log($"[AutoButtonTester] ?? Apertura manual: {(manualSuccess ? "? Exitosa" : "? Fallida")}");
        }
        
        // Resumen
        Debug.Log("[AutoButtonTester] ?? RESUMEN DE TESTS:");
        Debug.Log($"[AutoButtonTester]    Total: {testCount}");
        Debug.Log($"[AutoButtonTester]    Exitosos: {successCount}");
        Debug.Log($"[AutoButtonTester]    Fallidos: {failureCount}");
        Debug.Log($"[AutoButtonTester]    Tasa de éxito: {(testCount > 0 ? (successCount * 100f / testCount) : 0):F1}%");
    }
    
    private void FindReferences()
    {
        Debug.Log("[AutoButtonTester] ?? Buscando referencias...");
        
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<NavigationUIManager>();
            if (uiManager != null)
            {
                Debug.Log($"[AutoButtonTester] ? NavigationUIManager encontrado: {uiManager.gameObject.name}");
            }
            else
            {
                Debug.LogError("[AutoButtonTester] ? NavigationUIManager NO encontrado");
                return;
            }
        }
        
        if (buttonToTest == null)
        {
            // Primero buscar en UIManager
            if (uiManager != null && uiManager.openLocationPanelButton != null)
            {
                buttonToTest = uiManager.openLocationPanelButton;
                Debug.Log($"[AutoButtonTester] ? Botón encontrado en UIManager: {buttonToTest.gameObject.name}");
            }
            else
            {
                // Buscar por nombre
                GameObject buttonObj = GameObject.Find("OpenLocationPanelButton");
                if (buttonObj != null)
                {
                    buttonToTest = buttonObj.GetComponent<Button>();
                    if (buttonToTest != null)
                    {
                        Debug.Log($"[AutoButtonTester] ? Botón encontrado por nombre: {buttonToTest.gameObject.name}");
                    }
                }
                else
                {
                    Debug.LogError("[AutoButtonTester] ? Botón NO encontrado");
                }
            }
        }
    }
    
    #region Public Methods
    
    /// <summary>
    /// Ejecuta un test manual del botón
    /// </summary>
    public void RunManualTest()
    {
        StartCoroutine(TestButton());
    }
    
    /// <summary>
    /// Resetea los contadores de test
    /// </summary>
    public void ResetCounters()
    {
        testCount = 0;
        successCount = 0;
        failureCount = 0;
        Debug.Log("[AutoButtonTester] ?? Contadores reseteados");
    }
    
    /// <summary>
    /// Muestra un resumen de los tests ejecutados
    /// </summary>
    public void ShowSummary()
    {
        Debug.Log("[AutoButtonTester] ???????????????????????????????????");
        Debug.Log("[AutoButtonTester] ?? RESUMEN FINAL DE TESTS");
        Debug.Log("[AutoButtonTester] ???????????????????????????????????");
        Debug.Log($"[AutoButtonTester] Total de tests: {testCount}");
        Debug.Log($"[AutoButtonTester] Exitosos: {successCount} ?");
        Debug.Log($"[AutoButtonTester] Fallidos: {failureCount} ?");
        Debug.Log($"[AutoButtonTester] Tasa de éxito: {(testCount > 0 ? (successCount * 100f / testCount) : 0):F1}%");
        Debug.Log("[AutoButtonTester] ???????????????????????????????????");
    }
    
    #endregion
    
    #region On-Screen Display
    
    void OnGUI()
    {
        if (testCount == 0) return;
        
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.fontSize = 18;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperRight;
        
        string status = $"?? Auto Tester\n";
        status += $"Tests: {testCount}\n";
        status += $"? OK: {successCount}\n";
        status += $"? Fail: {failureCount}\n";
        status += $"Rate: {(testCount > 0 ? (successCount * 100f / testCount) : 0):F0}%";
        
        GUI.Box(new Rect(Screen.width - 210, 10, 200, 120), status, style);
        
        // Botón para test manual
        if (GUI.Button(new Rect(Screen.width - 210, 140, 200, 40), "?? Test Manual [T]"))
        {
            StartCoroutine(TestButton());
        }
    }
    
    #endregion
}
