using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class IfcModelLodController : MonoBehaviour
{
    [SerializeField] private Camera viewingCamera;
    [SerializeField, Min(0.01f)] private float minimumPixelDiameter = 0.5f;
    [SerializeField, Min(0.05f)] private float evaluationInterval = 0.25f;
    [SerializeField, Min(0f)] private float selectedRevealSeconds = 4f;

    private readonly List<Renderer> renderers = new();
    private readonly Dictionary<Renderer, float> revealUntil = new();
    private readonly Dictionary<Renderer, bool> cullingStates = new();
    private float nextEvaluationTime;
    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;
    private float lastCameraProjection;
    private bool evaluationRequired = true;

    public void Rebuild()
    {
        ResetCulling();
        renderers.Clear();
        cullingStates.Clear();
        GetComponentsInChildren(true, renderers);
        viewingCamera ??= Camera.main;
        evaluationRequired = true;
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
            SetCullingState(renderer, false);
        }

        evaluationRequired = true;
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
        if (!evaluationRequired &&
            revealUntil.Count == 0 &&
            !HasCameraChanged())
        {
            return;
        }

        Evaluate();
    }

    private void Evaluate()
    {
        viewingCamera ??= Camera.main;
        if (viewingCamera == null)
        {
            return;
        }

        RememberCameraState();
        evaluationRequired = false;
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
                    SetCullingState(renderer, false);
                    continue;
                }

                revealUntil.Remove(renderer);
            }

            SetCullingState(
                renderer,
                CalculateProjectedDiameterPixels(viewingCamera, renderer.bounds) <
                minimumPixelDiameter);
        }
    }

    private bool HasCameraChanged()
    {
        if (viewingCamera == null)
        {
            return false;
        }

        var projection = viewingCamera.orthographic
            ? viewingCamera.orthographicSize
            : viewingCamera.fieldOfView;
        return (viewingCamera.transform.position - lastCameraPosition).sqrMagnitude > 0.01f ||
               Quaternion.Angle(viewingCamera.transform.rotation, lastCameraRotation) > 0.05f ||
               Mathf.Abs(projection - lastCameraProjection) > 0.01f;
    }

    private void RememberCameraState()
    {
        lastCameraPosition = viewingCamera.transform.position;
        lastCameraRotation = viewingCamera.transform.rotation;
        lastCameraProjection = viewingCamera.orthographic
            ? viewingCamera.orthographicSize
            : viewingCamera.fieldOfView;
    }

    private void SetCullingState(Renderer renderer, bool culled)
    {
        if (cullingStates.TryGetValue(renderer, out var previous) && previous == culled)
        {
            return;
        }

        cullingStates[renderer] = culled;
        renderer.forceRenderingOff = culled;
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
        cullingStates.Clear();
    }
}
