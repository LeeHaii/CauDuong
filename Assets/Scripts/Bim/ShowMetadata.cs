using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;   
using UnityEngine.EventSystems;  
// Added Enhanced Touch
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class ShowMetadata : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private ScrollRect entriesPanel;
    private List<GameObject> entries = new List<GameObject>();

    [Header("Position")]
    [SerializeField] private TextMeshProUGUI corHeader;
    [SerializeField] private TextMeshProUGUI xcor;
    [SerializeField] private TextMeshProUGUI ycor;
    [SerializeField] private TextMeshProUGUI zcor;

    [Header("Metadata Filtering")]
    [Tooltip("If key or value contains these strings, the entire entry is hidden.")]
    public List<string> keysToIgnore = new List<string>();
    
    [Tooltip("These strings will be removed from keys and values (e.g. 'IFCLIST/' -> '').")]
    public List<string> stringsToRemoveFromKeys = new List<string>();

    private GameObject lastSelectedObject; 
    private Camera mainCamera;

    // Enable touch tracking
    private void OnEnable() { EnhancedTouchSupport.Enable(); }
    private void OnDisable() { EnhancedTouchSupport.Disable(); }

    void Start()
    {
        mainCamera = Camera.main;
        
        if (entryPrefab != null) 
            entryPrefab.SetActive(false);
            
        ClearUI(); 
    }

    void Update()
    {
        if (mainCamera == null) return;

        bool isClicking = false;
        Vector2 clickPosition = Vector2.zero;
        bool isOverUI = false;

        // 1. Check for Touch (Mobile)
        if (Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            // Only trigger on the exact frame the finger touches the screen
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                isClicking = true;
                clickPosition = touch.screenPosition;
                
                // Touch-specific UI check using the finger's unique ID
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.touchId))
                    isOverUI = true;
            }
        }
        // 2. Check for Mouse (PC)
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            isClicking = true;
            clickPosition = Mouse.current.position.ReadValue();
            
            // Mouse-specific UI check
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                isOverUI = true;
        }

        // 3. Process the Raycast if a valid click/tap happened
        if (isClicking)
        {
            if (isOverUI) return; // Prevent raycast if touching the scrollbar or text

            Ray ray = mainCamera.ScreenPointToRay(clickPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;

                if (clickedObject != lastSelectedObject)
                {
                    lastSelectedObject = clickedObject;
                    OnObjectSelected(clickedObject);
                }
            }
            else
            {
                if (lastSelectedObject != null)
                {
                    lastSelectedObject = null;
                    ClearUI();
                }
            }
        }
    }

    private void OnObjectSelected(GameObject selectedObj)
    {
        CleanEntries();
        if (selectedObj != null)
        {
            ShowingMetaData(selectedObj);
        }
    }

    private void ShowingMetaData(GameObject obj)
    {
        GameObject targetObj = obj;
        bool hasHouseComp = targetObj.TryGetComponent(out HouseComponent houseComp) && houseComp.Data != null;
        bool hasPixyz = targetObj.TryGetComponent(out Pixyz.ImportSDK.Metadata pixyzMetadata);

        if (!hasHouseComp && !hasPixyz && targetObj.transform.parent != null)
        {
            targetObj = targetObj.transform.parent.gameObject;
            hasHouseComp = targetObj.TryGetComponent(out houseComp) && houseComp.Data != null;
            hasPixyz = targetObj.TryGetComponent(out pixyzMetadata);
        }

        corHeader.text = targetObj.name;
        xcor.text = "X: " + Mathf.Round(targetObj.transform.position.x);
        ycor.text = "Y: " + Mathf.Round(targetObj.transform.position.y);
        zcor.text = "Z: " + Mathf.Round(targetObj.transform.position.z);

        // Try getting House data first (Priority)
        if (hasHouseComp)
        {
            corHeader.text = houseComp.Data.title; // Override header with House title
            
            AddUIEntry("ID", houseComp.Data.id.ToString());
            AddUIEntry("Price", string.Format("{0:N0} VND", houseComp.Data.price));
            AddUIEntry("Description", houseComp.Data.description);
            AddUIEntry("Area", houseComp.Data.area_m2 + " m²");
            AddUIEntry("Status", houseComp.Data.status);
            AddUIEntry("Residents", houseComp.Data.residential_number.ToString());
        }
        // Fallback to Pixyz Metadata if no HouseComponent is found or if both exist
        else if (hasPixyz)
        {
            foreach (var property in pixyzMetadata.getProperties())
            {
                string key = property.Key;
                string value = property.Value;

                bool shouldIgnore = false;
                foreach (string ignoreStr in keysToIgnore)
                {
                    if (key.Contains(ignoreStr) || value.Contains(ignoreStr))
                    {
                        shouldIgnore = true;
                        break;
                    }
                }
                if (shouldIgnore) continue;

                foreach (string removeStr in stringsToRemoveFromKeys)
                {
                    key = key.Replace(removeStr, "");
                    value = value.Replace(removeStr, "");
                }

                AddUIEntry(key, value);
            }
        }
    }

    private void AddUIEntry(string key, string value)
    {
        GameObject newEntry = Instantiate(entryPrefab, entriesPanel.content);
        newEntry.SetActive(true);
        newEntry.transform.Find("Key").GetComponentInChildren<TextMeshProUGUI>().SetText(key);
        newEntry.transform.Find("Value").GetComponentInChildren<TextMeshProUGUI>().SetText(value);
        entries.Add(newEntry);
    }

    private void CleanEntries()
    {
        if (entries.Count == 0) return;
        foreach (var entry in entries)
        {
            Destroy(entry);
        }
        entries.Clear();
    }

    private void ClearUI()
    {
        CleanEntries();
        corHeader.text = "No Object Selected";
        xcor.text = "X: --";
        ycor.text = "Y: --";
        zcor.text = "Z: --";
    }
}