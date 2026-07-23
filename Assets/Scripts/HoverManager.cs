using UnityEngine;
using UnityEngine.InputSystem;

public class HoverManager : MonoBehaviour
{
    private enum HighlightType { None, Darken }

    [Header("Tag Highlight (Units)")]
    public string targetTag = "Unit";

    [Header("Component Highlight (Metadata)")]
    [Range(0f, 1f)]
    public float darkenMultiplier = 0.7f;

    public BimDataProperties bimDataProperties;
    // Tracking variables
    private Transform currentHoveredObject;
    private Renderer currentRenderer;
    private HighlightType currentHighlightType = HighlightType.None;
    
    private Camera mainCamera;
    private MaterialPropertyBlock propertyBlock;

    public static HoverManager Instance { get; private set; }
    public Renderer CurrentHoveredRenderer => currentRenderer;
    public float DarkenMultiplier => darkenMultiplier;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
        propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (Mouse.current == null || mainCamera == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;

            // If we are looking at a NEW object
            if (hitTransform != currentHoveredObject)
            {
                RemoveHighlight();

                // 1. Check for the "Unit" tag first (Priority 1)
                if (hitTransform.CompareTag(targetTag))
                {
                    ApplyDarken(hitTransform);
                }
                // 2. Check for Metadata component on the object itself
                else 
                {
                    bool hasMetadata = hitTransform.TryGetComponent<Pixyz.ImportSDK.Metadata>(out _);
                    
                    // 3. If no metadata on the object, check 1 level above (its parent)
                    if (!hasMetadata && hitTransform.parent != null)
                    {
                        hasMetadata = hitTransform.parent.TryGetComponent<Pixyz.ImportSDK.Metadata>(out _);
                    }

                    if (hasMetadata && bimDataProperties.GetBIMdata())
                    {
                        ApplyDarken(hitTransform);
                    }
                }
            }
        }
        else
        {
            // Hovering over empty space
            RemoveHighlight();
        }
    }

    private void ApplyDarken(Transform obj)
    {
        currentHoveredObject = obj;
        currentRenderer = obj.GetComponent<Renderer>();

        if (currentRenderer != null)
        {
            currentHighlightType = HighlightType.Darken;

            Color originalColor = Color.white;
            
            if (currentRenderer.sharedMaterial.HasProperty("_BaseColor"))
                originalColor = currentRenderer.sharedMaterial.GetColor("_BaseColor");
            else if (currentRenderer.sharedMaterial.HasProperty("_Color"))
                originalColor = currentRenderer.sharedMaterial.color;

            Color darkenedColor = new Color(
                originalColor.r * darkenMultiplier,
                originalColor.g * darkenMultiplier,
                originalColor.b * darkenMultiplier,
                originalColor.a
            );

            currentRenderer.GetPropertyBlock(propertyBlock);
            
            if (currentRenderer.sharedMaterial.HasProperty("_BaseColor"))
                propertyBlock.SetColor("_BaseColor", darkenedColor);
            else
                propertyBlock.SetColor("_Color", darkenedColor);

            currentRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    private void RemoveHighlight()
    {
        if (currentHoveredObject != null && currentRenderer != null)
        {
            // Undo the specific effect based on what we applied
            if (currentHighlightType == HighlightType.Darken)
            {
                currentRenderer.SetPropertyBlock(null);
            }
        }

        // Wipe variables clean
        currentHoveredObject = null;
        currentRenderer = null;
        currentHighlightType = HighlightType.None;
    }
}