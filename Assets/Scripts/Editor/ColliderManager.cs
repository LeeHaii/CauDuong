using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

// The marker component has been removed per user request.

public class ColliderManager : MonoBehaviour
{
    // Allows you to run this from the component's context menu in the Inspector
    [ContextMenu("Add Convex Mesh Colliders")]
    public void AddConvexMeshColliders()
    {
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>(true);
        int addedCount = 0;
        int modifiedCount = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject go = renderer.gameObject;
            
            MeshCollider collider = go.GetComponent<MeshCollider>();
            if (collider == null)
            {
                collider = go.AddComponent<MeshCollider>();
                collider.convex = true;
                collider.convex = true;
                addedCount++;
            }
            else if (!collider.convex)
            {
                // We just set it to convex but didn't create it, so we DON'T add the marker here.
                collider.convex = true;
                modifiedCount++;
            }
        }

        Debug.Log($"Added {addedCount} new MeshColliders and set {modifiedCount} existing ones to convex.");
    }

    // RemoveGeneratedColliders has been removed because it relied on the marker.

    void Start()
    {
        // Uncomment the line below if you want this to happen automatically when the game starts.
        // NOTE: Doing this at runtime can be performance intensive for large scenes!
        // AddConvexMeshColliders();
    }

#if UNITY_EDITOR
    // Adds a top menu item in the Unity Editor for convenience
    [MenuItem("Tools/Colliders/Add Convex Mesh Colliders to All Renderers")]
    public static void AddCollidersMenu()
    {
        MeshRenderer[] renderers = FindObjectsOfType<MeshRenderer>(true);
        int addedCount = 0;
        int modifiedCount = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            GameObject go = renderer.gameObject;
            
            MeshCollider collider = go.GetComponent<MeshCollider>();
            if (collider == null)
            {
                Undo.AddComponent<MeshCollider>(go);
                collider = go.GetComponent<MeshCollider>();
                //collider.convex = true;
                //collider.convex = true;
                addedCount++;
            }
            else if (!collider.convex)
            {
                Undo.RecordObject(collider, "Set Convex");
                //collider.convex = true;
                modifiedCount++;
            }
        }

        Debug.Log($"[Editor] Added {addedCount} new MeshColliders and set {modifiedCount} existing ones to convex.");
    }

    // RemoveCollidersMenu has been removed because it relied on the marker.
#endif
}
