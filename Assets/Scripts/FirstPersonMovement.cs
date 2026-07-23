using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float mouseSensitivity = 0.5f; 
    public float touchSensitivity = 0.2f;
    
    private float verticalRotation = 0f;
    private Camera playerCamera;
    private AudioListener playerAudioListener;

    private Camera orbitCamera;
    private AudioListener orbitAudioListener;
    private CameraWallFader wallFader;
    private List<Collider> doorColliders = new List<Collider>();
    
    // Touch controls
    private bool useTouchControls = false;
    private GameObject touchCanvas;
    private Image joystickBase;
    private Image joystickKnob;
    private int movementTouchId = -1;
    private int lookTouchId = -1;
    private Vector2 joystickStartPos;
    private Vector2 joystickCurrentPos;
    private float joystickMaxRadius = 150f;
    private Vector2 touchLookDelta = Vector2.zero;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        
        if (playerCamera != null)
        {
            playerAudioListener = playerCamera.GetComponent<AudioListener>();
            playerCamera.enabled = false;
            if (playerAudioListener != null) playerAudioListener.enabled = false;
        }

        GameObject mainCamObj = GameObject.Find("Main Camera");
        if (mainCamObj != null)
        {
            orbitCamera = mainCamObj.GetComponent<Camera>();
            orbitAudioListener = mainCamObj.GetComponent<AudioListener>();
            wallFader = mainCamObj.GetComponent<CameraWallFader>();
        }

        GameObject spawnPoint = GameObject.Find("Spawnpoint");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }

        GameObject btnObj = GameObject.Find("ButtonPlayer");
        if (btnObj != null)
        {
            Button btn = btnObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(EnableMovement);
            }
        }

        CreateTouchUI();

        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Doors");
        foreach (GameObject door in doorObjects)
        {
            Collider[] colliders = door.GetComponentsInChildren<Collider>(true);
            doorColliders.AddRange(colliders);
        }

        this.enabled = false;
    }

    void CreateTouchUI()
    {
        touchCanvas = new GameObject("TouchCanvas");
        Canvas canvas = touchCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        touchCanvas.AddComponent<CanvasScaler>();
        touchCanvas.AddComponent<GraphicRaycaster>();
        
        GameObject baseGO = new GameObject("JoystickBase");
        baseGO.transform.SetParent(touchCanvas.transform);
        joystickBase = baseGO.AddComponent<Image>();
        joystickBase.color = new Color(0, 0, 0, 0.3f);
        joystickBase.rectTransform.sizeDelta = new Vector2(joystickMaxRadius * 2, joystickMaxRadius * 2);
        joystickBase.rectTransform.anchorMin = Vector2.zero;
        joystickBase.rectTransform.anchorMax = Vector2.zero;
        joystickBase.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        
        GameObject knobGO = new GameObject("JoystickKnob");
        knobGO.transform.SetParent(baseGO.transform);
        joystickKnob = knobGO.AddComponent<Image>();
        joystickKnob.color = new Color(1, 1, 1, 0.5f);
        joystickKnob.rectTransform.sizeDelta = new Vector2(joystickMaxRadius, joystickMaxRadius);
        
        // --- Create Return Button ---
        GameObject btnObj = new GameObject("ReturnButton");
        btnObj.transform.SetParent(touchCanvas.transform, false);
        
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.1f, 0.1f, 0.1f, 0.7f); 
        
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(1, 1);
        btnRect.anchorMax = new Vector2(1, 1);
        btnRect.pivot = new Vector2(1, 1);
        btnRect.anchoredPosition = new Vector2(-40, -40); // Top Right corner
        btnRect.sizeDelta = new Vector2(200, 80);
        
        Button exitBtn = btnObj.AddComponent<Button>();
        exitBtn.onClick.AddListener(() => {
            this.enabled = false; // Disabling script triggers OnDisable() to exit first person
        });

        // Add Text to Return Button
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        Text btnText = textObj.AddComponent<Text>();
        btnText.text = "QUAY LẠI"; // Return
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
        btnText.fontSize = 32;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        touchCanvas.SetActive(false);
    }

    void EnableMovement()
    {
        this.enabled = true;
    }

    void OnEnable()
    {
        if (orbitCamera != null) orbitCamera.enabled = false;
        if (orbitAudioListener != null) orbitAudioListener.enabled = false;
        if (wallFader != null) wallFader.enabled = false;

        if (playerCamera != null) playerCamera.enabled = true;
        if (playerAudioListener != null) playerAudioListener.enabled = true;

        if (!useTouchControls)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        foreach (Collider col in doorColliders)
        {
            if (col != null) col.enabled = false;
        }
    }

    void OnDisable()
    {
        if (playerCamera != null) playerCamera.enabled = false;
        if (playerAudioListener != null) playerAudioListener.enabled = false;

        if (orbitCamera != null) orbitCamera.enabled = true;
        if (orbitAudioListener != null) orbitAudioListener.enabled = true;
        if (wallFader != null) wallFader.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (touchCanvas != null) touchCanvas.SetActive(false);

        foreach (Collider col in doorColliders)
        {
            if (col != null) col.enabled = true;
        }
    }

    void Update()
    {
        // Detect touch to enable touch controls
        if (!useTouchControls && Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            foreach (var t in Touchscreen.current.touches)
            {
                if (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    useTouchControls = true;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    break;
                }
            }
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            this.enabled = false;
            return;
        }

        float moveForward = 0f;
        float moveSide = 0f;
        float mouseX = 0f;
        float mouseY = 0f;

        if (useTouchControls)
        {
            ProcessTouchInput(ref moveForward, ref moveSide, ref mouseX, ref mouseY);
        }
        else
        {
            ProcessKeyboardMouseInput(ref moveForward, ref moveSide, ref mouseX, ref mouseY);
        }

        Vector3 move = transform.forward * moveForward + transform.right * moveSide;
        
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.SimpleMove(move * speed);
        }
        else
        {
            transform.Translate(new Vector3(moveSide, 0, moveForward) * speed * Time.deltaTime, Space.Self);
        }

        transform.Rotate(0, mouseX, 0);

        if (playerCamera != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }

    void ProcessKeyboardMouseInput(ref float moveForward, ref float moveSide, ref float mouseX, ref float mouseY)
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveForward += 1f;
            if (Keyboard.current.sKey.isPressed) moveForward -= 1f;
            if (Keyboard.current.aKey.isPressed) moveSide -= 1f;
            if (Keyboard.current.dKey.isPressed) moveSide += 1f;
        }

        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX = mouseDelta.x * mouseSensitivity;
            mouseY = mouseDelta.y * mouseSensitivity;
        }
    }

    void ProcessTouchInput(ref float moveForward, ref float moveSide, ref float mouseX, ref float mouseY)
    {
        if (Touchscreen.current == null) return;
        
        touchCanvas.SetActive(true);
        bool movementTouchFound = false;

        foreach (var touch in Touchscreen.current.touches)
        {
            var phase = touch.phase.ReadValue();
            if (phase == UnityEngine.InputSystem.TouchPhase.None || phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                if (touch.touchId.ReadValue() == movementTouchId)
                {
                    movementTouchId = -1;
                    joystickBase.gameObject.SetActive(false);
                }
                if (touch.touchId.ReadValue() == lookTouchId)
                {
                    lookTouchId = -1;
                }
                continue;
            }

            Vector2 pos = touch.position.ReadValue();
            int id = touch.touchId.ReadValue();

            // Left side of screen = movement
            if (pos.x < Screen.width / 2)
            {
                if (movementTouchId == -1 && phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    movementTouchId = id;
                    joystickStartPos = pos;
                    joystickBase.rectTransform.anchoredPosition = pos;
                    joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
                    joystickBase.gameObject.SetActive(true);
                }
                
                if (movementTouchId == id)
                {
                    movementTouchFound = true;
                    joystickCurrentPos = pos;
                    Vector2 offset = joystickCurrentPos - joystickStartPos;
                    if (offset.magnitude > joystickMaxRadius)
                    {
                        offset = offset.normalized * joystickMaxRadius;
                    }
                    joystickKnob.rectTransform.anchoredPosition = offset;
                    
                    Vector2 inputDir = offset / joystickMaxRadius;
                    moveSide = inputDir.x;
                    moveForward = inputDir.y;
                }
            }
            // Right side of screen = look
            else
            {
                if (lookTouchId == -1 && phase == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    lookTouchId = id;
                }

                if (lookTouchId == id)
                {
                    Vector2 delta = touch.delta.ReadValue();
                    mouseX = delta.x * touchSensitivity;
                    mouseY = delta.y * touchSensitivity;
                }
            }
        }

        if (!movementTouchFound && movementTouchId != -1)
        {
            movementTouchId = -1;
            joystickBase.gameObject.SetActive(false);
        }
    }
}
