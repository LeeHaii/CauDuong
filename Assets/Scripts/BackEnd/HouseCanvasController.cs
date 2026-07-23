using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class HouseCanvasController : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject uiPanel;

    [Header("Text References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI areaText;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI residentsText;

    private RawImage houseImage;
    private Button goToApartmentButton;

    private Camera mainCamera;
    private HouseData currentHouseData;
    private GameObject currentSelectedUnit;

    void Start()
    {
        mainCamera = Camera.main;
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            
            // Auto-bind newly created UI elements via code
            houseImage = uiPanel.GetComponentInChildren<RawImage>();
            goToApartmentButton = uiPanel.GetComponentInChildren<Button>();
            
            if (goToApartmentButton != null)
            {
                goToApartmentButton.onClick.AddListener(OnGoToApartmentClicked);
            }
        }
    }

    void Update()
    {
        bool wasPressed = false;
        Vector2 screenPos = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            wasPressed = true;
            screenPos = Mouse.current.position.ReadValue();
        }
        else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            wasPressed = true;
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        if (wasPressed)
        {
            HandleClick(screenPos);
        }

        // Auto-update if data arrives while Skeleton UI is showing
        if (uiPanel != null && uiPanel.activeSelf && currentHouseData == null && currentSelectedUnit != null)
        {
            if (currentSelectedUnit.TryGetComponent(out HouseComponent houseComp) && houseComp.Data != null)
            {
                ShowHouseData(houseComp.Data);
            }
        }
    }

    private void HandleClick(Vector2 screenPos)
    {
        // Ignore click if the pointer is over a UI element
        if (EventSystem.current != null)
        {
            bool isOverUI = false;
            
            // Check for touch UI hit
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                isOverUI = EventSystem.current.IsPointerOverGameObject(Touchscreen.current.primaryTouch.touchId.ReadValue());
            }
            
            // Fallback to general pointer check (Mouse)
            if (!isOverUI)
            {
                isOverUI = EventSystem.current.IsPointerOverGameObject();
            }

            if (isOverUI) return;
        }

        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (clickedObject.CompareTag("Unit"))
            {
                currentSelectedUnit = clickedObject;
                if (clickedObject.TryGetComponent(out HouseComponent houseComp) && houseComp.Data != null)
                {
                    ShowHouseData(houseComp.Data);
                }
                else
                {
                    ShowSkeletonUI();
                }
                return;
            }
        }

        // If we click nothing or something else, hide the UI
        HideUI();
    }

    private void ShowSkeletonUI()
    {
        currentHouseData = null;

        if (titleText != null) titleText.text = "ĐANG TẢI...";
        if (priceText != null) priceText.text = "GIÁ BÁN: ---";
        if (areaText != null) areaText.text = "DIỆN TÍCH: ---";
        if (descText != null) descText.text = "MÔ TẢ: ĐANG TẢI...";
        if (statusText != null) statusText.text = "TRẠNG THÁI: ---";
        if (residentsText != null) residentsText.text = "SỐ NGƯỜI Ở: ---";

        if (houseImage != null)
        {
            houseImage.texture = null; // Clear image while loading
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }

    private void ShowHouseData(HouseData data)
    {
        currentHouseData = data;

        if (titleText != null) titleText.text = data.title;
        if (priceText != null) priceText.text = $"GIÁ BÁN: {data.price:N0} VND";
        if (areaText != null) areaText.text = $"DIỆN TÍCH: {data.area_m2} m²";
        if (descText != null) descText.text = $"MÔ TẢ: {data.description}";
        if (statusText != null) statusText.text = $"TRẠNG THÁI: {data.status}";
        if (residentsText != null) residentsText.text = $"SỐ NGƯỜI Ở: {data.residential_number}";

        if (houseImage != null && !string.IsNullOrEmpty(data.image_url))
        {
            StartCoroutine(LoadImageCoroutine(data.image_url));
        }

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }
    }

    private IEnumerator LoadImageCoroutine(string url)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                if (houseImage != null)
                {
                    houseImage.texture = texture;
                }
            }
            else
            {
                Debug.LogError("Failed to download image: " + uwr.error);
            }
        }
    }

    public void OnGoToApartmentClicked()
    {
        if (currentHouseData != null && !string.IsNullOrEmpty(currentHouseData.addressable_str))
        {
            PlayerPrefs.SetString("SelectedApartment", currentHouseData.addressable_str);
            PlayerPrefs.Save();
            SceneManager.LoadScene("ApartmentScene");
        }
        else
        {
            Debug.LogWarning("No apartment selected or missing addressable_str.");
        }
    }

    private void HideUI()
    {
        currentSelectedUnit = null;
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }
}
