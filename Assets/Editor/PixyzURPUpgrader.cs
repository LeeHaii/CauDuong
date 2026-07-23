using UnityEngine;
using UnityEditor;

public class PixyzURPUpgrader
{
    [MenuItem("Pixyz/Upgrade Selected Models to URP")]
    public static void UpgradeSelected()
    {
        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
        if (urpLit == null)
        {
            Debug.LogError("URP Lit shader not found. Make sure URP is installed.");
            return;
        }

        int count = 0;
        
        // Handle selected materials directly
        foreach (Object obj in Selection.objects)
        {
            if (obj is Material m)
            {
                if (UpgradeMaterial(m, urpLit)) count++;
            }
            else if (obj is GameObject go)
            {
                Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer r in renderers)
                {
                    Material[] materials = r.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (materials[i] != null && UpgradeMaterial(materials[i], urpLit))
                        {
                            count++;
                        }
                    }
                }
            }
        }
        
        Debug.Log($"Upgraded {count} materials to URP.");
    }

    private static bool UpgradeMaterial(Material m, Shader urpLit)
    {
        if (m.shader.name == urpLit.name) return false;

        // Save old properties
        Color color = m.HasProperty("_Color") ? m.GetColor("_Color") : Color.white;
        Texture mainTex = m.HasProperty("_MainTex") ? m.GetTexture("_MainTex") : null;
        Vector2 texOffset = mainTex != null && m.HasProperty("_MainTex") ? m.GetTextureOffset("_MainTex") : Vector2.zero;
        Vector2 texScale = mainTex != null && m.HasProperty("_MainTex") ? m.GetTextureScale("_MainTex") : Vector2.one;

        // Change shader
        m.shader = urpLit;
        
        // Apply properties to URP material
        m.SetColor("_BaseColor", color);
        if (mainTex != null)
        {
            m.SetTexture("_BaseMap", mainTex);
            m.SetTextureOffset("_BaseMap", texOffset);
            m.SetTextureScale("_BaseMap", texScale);
        }
        
        EditorUtility.SetDirty(m);
        return true;
    }
}
