using System.Collections.Generic;
using UnityEngine;

public class CameraWallFader : MonoBehaviour
{
    [Header("Fade Settings")]
    public string wallTag = "Wall";
    
    [Tooltip("Distance where the wall becomes completely invisible (0% opacity).")]
    public float fullTransparentDistance = 3.0f; 
    
    [Tooltip("Distance where the wall becomes completely solid (100% opacity).")]
    public float fullSolidDistance = 8.0f;       

    // A list to keep track of all our wall renderers
    private List<Renderer> wallRenderers = new List<Renderer>();
    
    // Our secret weapon for changing colors without memory leaks
    private MaterialPropertyBlock propertyBlock;

    void Start()
    {
        propertyBlock = new MaterialPropertyBlock();

        // 1. Find every object in the game tagged "Wall" when the game starts
        GameObject[] walls = GameObject.FindGameObjectsWithTag(wallTag);
        
        foreach (GameObject wall in walls)
        {
            Renderer rend = wall.GetComponent<Renderer>();
            if (rend != null)
            {
                wallRenderers.Add(rend); // Save them to our list
            }
        }
    }

    void LateUpdate()
    {
        // 2. Look at where the camera currently is
        Vector3 cameraPosition = transform.position;

        // 3. Check the distance to every single wall in our list
        foreach (Renderer rend in wallRenderers)
        {
            if (rend == null) continue;

            float distance = Vector3.Distance(cameraPosition, rend.transform.position);

            // 4. Calculate the Fade (Alpha)
            // Mathf.InverseLerp takes our distance and turns it into a smooth percentage between 0.0 and 1.0.
            float targetAlpha = Mathf.InverseLerp(fullTransparentDistance, fullSolidDistance, distance);

            // 5. Apply the new transparency to the property block
            rend.GetPropertyBlock(propertyBlock);

            Color currentColor = Color.white;
            
            // Check for URP vs Built-in pipelines just like our Hover script
            if (rend.sharedMaterial.HasProperty("_BaseColor"))
            {
                currentColor = rend.sharedMaterial.GetColor("_BaseColor");
                
                if (HoverManager.Instance != null && HoverManager.Instance.CurrentHoveredRenderer == rend)
                {
                    float darken = HoverManager.Instance.DarkenMultiplier;
                    currentColor = new Color(currentColor.r * darken, currentColor.g * darken, currentColor.b * darken, currentColor.a);
                }

                currentColor.a = targetAlpha; // 'a' stands for Alpha (transparency)
                propertyBlock.SetColor("_BaseColor", currentColor);
            }
            else if (rend.sharedMaterial.HasProperty("_Color"))
            {
                currentColor = rend.sharedMaterial.color;

                if (HoverManager.Instance != null && HoverManager.Instance.CurrentHoveredRenderer == rend)
                {
                    float darken = HoverManager.Instance.DarkenMultiplier;
                    currentColor = new Color(currentColor.r * darken, currentColor.g * darken, currentColor.b * darken, currentColor.a);
                }

                currentColor.a = targetAlpha;
                propertyBlock.SetColor("_Color", currentColor);
            }

            // Apply the block back to the wall
            rend.SetPropertyBlock(propertyBlock);
        }
    }

    void OnDisable()
    {
        ResetWalls();
    }

    public void ResetWalls()
    {
        if (propertyBlock == null) propertyBlock = new MaterialPropertyBlock();

        foreach (Renderer rend in wallRenderers)
        {
            if (rend == null) continue;

            rend.GetPropertyBlock(propertyBlock);

            Color currentColor = Color.white;
            
            if (rend.sharedMaterial.HasProperty("_BaseColor"))
            {
                currentColor = rend.sharedMaterial.GetColor("_BaseColor");
                currentColor.a = 1.0f;
                propertyBlock.SetColor("_BaseColor", currentColor);
            }
            else if (rend.sharedMaterial.HasProperty("_Color"))
            {
                currentColor = rend.sharedMaterial.color;
                currentColor.a = 1.0f;
                propertyBlock.SetColor("_Color", currentColor);
            }

            rend.SetPropertyBlock(propertyBlock);
        }
    }
}