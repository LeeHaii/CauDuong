using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems; // Added to prevent clicking through UI
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class UnitSceneTransition : MonoBehaviour
{
    [Header("Scene & Spawn Settings")]
    public string sceneToLoad;
    public GameObject prefabToSpawn;

    private Camera mainCamera;

    // Enable touch tracking
    private void OnEnable() { EnhancedTouchSupport.Enable(); }
    private void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) return;

        bool isClicking = false;
        Vector2 clickPosition = Vector2.zero;

        // 1. Check for Touch (Mobile)
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                isClicking = true;
                clickPosition = touch.screenPosition;
                
                // UI Protection for Touch
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId))
                    return;
            }
        }
        // 2. Check for Mouse (PC)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isClicking = true;
            clickPosition = Mouse.current.position.ReadValue();
            
            // UI Protection for Mouse
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
        }

        // 3. Process Raycast
        if (isClicking)
        {
            Ray ray = mainCamera.ScreenPointToRay(clickPosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    LoadSceneAndSpawn();
                }
            }
        }
    }

    private void LoadSceneAndSpawn()
    {
        GameObject messenger = new GameObject("SpawnMessenger");
        DontDestroyOnLoad(messenger);

        SceneSpawnMessenger helper = messenger.AddComponent<SceneSpawnMessenger>();
        helper.Setup(sceneToLoad, prefabToSpawn);
    }
}

// =====================================================================
// HELPER SCRIPT 
// =====================================================================
public class SceneSpawnMessenger : MonoBehaviour
{
    private string targetScene;
    private GameObject targetPrefab;

    public void Setup(string sceneName, GameObject prefab)
    {
        targetScene = sceneName;
        targetPrefab = prefab;

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (targetPrefab != null)
        {
            Instantiate(targetPrefab, Vector3.zero, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}