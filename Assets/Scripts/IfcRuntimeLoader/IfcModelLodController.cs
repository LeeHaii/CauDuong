using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class IfcModelLodController : MonoBehaviour
{
    [SerializeField] private Camera viewingCamera;
    [SerializeField, Min(0.01f)] private float minimumPixelDiameter = 0.05f;
    [SerializeField, Min(0.05f)] private float evaluationInterval = 0.2f;
    [SerializeField, Min(0f)] private float selectedRevealSeconds = 4f;

    private readonly List<Renderer> renderers = new();
    private readonly Dictionary<Renderer, float> revealUntil = new();
    private float nextEvaluationTime;

    public void Rebuild()
    {
        ResetCulling();
        renderers.Clear();
        GetComponentsInChildren(true, renderers);
        viewingCamera ??= Camera.main;
        Evaluate();
    }

    public void Reveal(IReadOnlyList<Renderer> selectedRenderers)
    {
        var expiry = Time.unscaledTime + selectedRevealSeconds;
        foreach (var renderer in selectedRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            revealUntil[renderer] = expiry;
            renderer.forceRenderingOff = false;
        }
    }

    private void OnEnable()
    {
        Rebuild();
    }

    private void OnDisable()
    {
        ResetCulling();
    }

    private void OnDestroy()
    {
        ResetCulling();
    }

    private void Update()
    {
        if (Time.unscaledTime < nextEvaluationTime)
        {
            return;
        }

        nextEvaluationTime = Time.unscaledTime + evaluationInterval;
        Evaluate();
    }

    private void Evaluate()
    {
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return;
        }

        var now = Time.unscaledTime;
        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (revealUntil.TryGetValue(renderer, out var expiry))
            {
                if (expiry > now)
                {
                    renderer.forceRenderingOff = false;
                    continue;
                }

                revealUntil.Remove(renderer);
            }

            renderer.forceRenderingOff =
                CalculateProjectedDiameterPixels(viewingCamera, renderer.bounds) <
                minimumPixelDiameter;
        }
    }

    public static float CalculateProjectedDiameterPixels(Camera camera, Bounds bounds)
    {
        if (camera == null || Screen.height <= 0)
        {
            return float.PositiveInfinity;
        }

        var radius = Mathf.Max(0.001f, bounds.extents.magnitude);
        if (camera.orthographic)
        {
            return radius * Screen.height / Mathf.Max(0.001f, camera.orthographicSize);
        }

        var distance = Mathf.Max(
            0.001f,
            Vector3.Distance(camera.transform.position, bounds.center) - radius);
        var verticalSize = 2f * distance *
                           Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        return radius * 2f * Screen.height / Mathf.Max(0.001f, verticalSize);
    }

    private void ResetCulling()
    {
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.forceRenderingOff = false;
            }
        }

        revealUntil.Clear();
    }
}
