using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

[RequireComponent(typeof(Camera))]
public sealed class OrbitCamera : MonoBehaviour
{
    [Header("Target Pivot")]
    public Vector3 pivotPoint = Vector3.zero;

    [Header("Distance & Zoom")]
    [Min(0.01f)] public float distance = 100f;
    [Min(0.01f)] public float minDistance = 1f;
    [Min(0.01f)] public float maxDistance = 5000f;

    [Header("Mouse Speeds")]
    [Min(0f)] public float mouseZoomSpeed = 0.2f;
    [Min(0f)] public float mouseXSpeed = 0.2f;
    [Min(0f)] public float mouseYSpeed = 0.2f;
    [Min(0f)] public float mousePanSpeed = 1f;

    [Header("Touch Speeds")]
    [Min(0f)] public float touchZoomSpeed = 0.1f;
    [Min(0f)] public float touchXSpeed = 0.1f;
    [Min(0f)] public float touchYSpeed = 0.1f;
    [Min(0f)] public float touchPanSpeed = 1f;

    [Header("Rotation Limits")]
    [SerializeField, Range(0f, 90f)] private float minPitch = 0f;
    [SerializeField, Range(0f, 90f)] private float maxPitch = 90f;
    [SerializeField] private UnityEngine.UIElements.UIDocument dashboardDocument;

    private Camera controlledCamera;
    private Button resetButton;
    private float yaw;
    private float pitch;

    private Vector3 initialPivotPoint;
    private float initialDistance;
    private float initialYaw;
    private float initialPitch;

    private void Awake()
    {
        controlledCamera = GetComponent<Camera>();
        var dashboard = FindFirstObjectByType<IfcOperationsDashboard>();
        dashboardDocument ??= dashboard != null
            ? dashboard.GetComponent<UnityEngine.UIElements.UIDocument>()
            : null;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = ClampPitch(NormalizeSignedAngle(angles.x));

        initialPivotPoint = pivotPoint;
        initialDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        initialYaw = yaw;
        initialPitch = pitch;

        var resetButtonObject = GameObject.Find("ButtonResetVIew") ??
                                GameObject.Find("ButtonResetView");
        if (resetButtonObject != null &&
            resetButtonObject.TryGetComponent<Button>(out var foundResetButton))
        {
            resetButton = foundResetButton;
            resetButton.onClick.AddListener(ResetView);
        }

        distance = initialDistance;
        UpdateCameraPosition();
    }

    private void OnDestroy()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(ResetView);
        }
    }

    private void LateUpdate()
    {
        var panDelta = Vector2.zero;
        var orbitDelta = Vector2.zero;

        if (Touch.activeTouches.Count > 0)
        {
            ReadTouchInput(ref panDelta, ref orbitDelta);
        }
        else
        {
            ReadMouseInput(ref panDelta, ref orbitDelta);
        }

        if (panDelta.sqrMagnitude > 0f)
        {
            Pan(panDelta, Touch.activeTouches.Count > 0
                ? touchPanSpeed
                : mousePanSpeed);
        }

        if (orbitDelta.sqrMagnitude > 0f)
        {
            Orbit(orbitDelta);
        }

        UpdateCameraPosition();
    }

    public void ResetView()
    {
        pivotPoint = initialPivotPoint;
        distance = initialDistance;
        yaw = initialYaw;
        pitch = initialPitch;
        UpdateCameraPosition();
    }

    private void ReadMouseInput(ref Vector2 panDelta, ref Vector2 orbitDelta)
    {
        if (Mouse.current == null)
        {
            return;
        }

        var pointerOverUi = IfcUiHitTest.IsPointerOverInteractiveUi(
            dashboardDocument,
            Mouse.current.position.ReadValue());
        var measurementActive = IfcMeasurementController.IsCapturingInput;

        if (!pointerOverUi &&
            !measurementActive &&
            Mouse.current.leftButton.isPressed)
        {
            panDelta = Mouse.current.delta.ReadValue();
        }

        if (!pointerOverUi &&
            !measurementActive &&
            Mouse.current.rightButton.isPressed)
        {
            var delta = Mouse.current.delta.ReadValue();
            orbitDelta = new Vector2(
                delta.x * mouseXSpeed,
                delta.y * mouseYSpeed);
        }

        var scroll = Mouse.current.scroll.y.ReadValue();
        if (!pointerOverUi && scroll != 0f)
        {
            var scrollNotches = scroll / 120f;
            distance = Mathf.Clamp(
                distance * Mathf.Exp(-scrollNotches * mouseZoomSpeed),
                minDistance,
                maxDistance);
        }
    }

    private void ReadTouchInput(ref Vector2 panDelta, ref Vector2 orbitDelta)
    {
        if (Touch.activeTouches.Count == 1)
        {
            var touch = Touch.activeTouches[0];
            if (!IsTouchOverUi(touch))
            {
                panDelta = touch.delta;
            }

            return;
        }

        var first = Touch.activeTouches[0];
        var second = Touch.activeTouches[1];
        if (IsTouchOverUi(first) || IsTouchOverUi(second))
        {
            return;
        }

        var previousFirstPosition = first.screenPosition - first.delta;
        var previousSecondPosition = second.screenPosition - second.delta;
        var previousSpacing = Vector2.Distance(
            previousFirstPosition,
            previousSecondPosition);
        var currentSpacing = Vector2.Distance(
            first.screenPosition,
            second.screenPosition);
        var pinchDelta = currentSpacing - previousSpacing;

        distance = Mathf.Clamp(
            distance - pinchDelta * touchZoomSpeed,
            minDistance,
            maxDistance);

        var averageDelta = (first.delta + second.delta) * 0.5f;
        orbitDelta = new Vector2(
            averageDelta.x * touchXSpeed,
            averageDelta.y * touchYSpeed);
    }

    private void Pan(Vector2 screenDelta, float speed)
    {
        var yawRotation = Quaternion.Euler(0f, yaw, 0f);
        var right = yawRotation * Vector3.right;
        var forward = yawRotation * Vector3.forward;
        var worldUnitsPerPixel = GetWorldUnitsPerPixel();

        pivotPoint += (-right * screenDelta.x - forward * screenDelta.y) *
                      (worldUnitsPerPixel * speed);
    }

    private void Orbit(Vector2 delta)
    {
        yaw += delta.x;
        pitch = ClampPitch(pitch - delta.y);
    }

    private float GetWorldUnitsPerPixel()
    {
        var screenHeight = Mathf.Max(1f, Screen.height);
        if (controlledCamera != null && controlledCamera.orthographic)
        {
            return controlledCamera.orthographicSize * 2f / screenHeight;
        }

        var fieldOfView = controlledCamera != null
            ? controlledCamera.fieldOfView
            : 60f;
        return 2f * distance *
               Mathf.Tan(fieldOfView * 0.5f * Mathf.Deg2Rad) /
               screenHeight;
    }

    private void UpdateCameraPosition()
    {
        var rotation = Quaternion.Euler(pitch, yaw, 0f);
        transform.SetPositionAndRotation(
            pivotPoint + rotation * new Vector3(0f, 0f, -distance),
            rotation);
    }

    private float ClampPitch(float angle)
    {
        var lower = Mathf.Clamp(minPitch, 0f, 90f);
        var upper = Mathf.Clamp(maxPitch, lower, 90f);
        return Mathf.Clamp(angle, lower, upper);
    }

    private static float NormalizeSignedAngle(float angle)
    {
        angle %= 360f;
        return angle > 180f ? angle - 360f : angle;
    }

    private static bool IsTouchOverUi(Touch touch)
    {
        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject(touch.touchId);
    }

    private void OnValidate()
    {
        minDistance = Mathf.Max(0.01f, minDistance);
        maxDistance = Mathf.Max(minDistance, maxDistance);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        minPitch = Mathf.Clamp(minPitch, 0f, 90f);
        maxPitch = Mathf.Clamp(maxPitch, minPitch, 90f);
    }
}
