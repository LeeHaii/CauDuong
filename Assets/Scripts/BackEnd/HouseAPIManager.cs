using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

public class HouseAPIManager : MonoBehaviour
{
    [Header("API Settings")]
    public string apiUrl = "https://nhadatabase.onrender.com/houses/";

    [Header("Scene References")]
    [Tooltip("The parent GameObject containing the 8 floor children")]
    public Transform floorsParent;

    void Start()
    {
        if (floorsParent == null)
        {
            GameObject floorsObj = GameObject.Find("Floors");
            if (floorsObj != null)
                floorsParent = floorsObj.transform;
        }

        StartCoroutine(FetchHousesData());
    }

    private IEnumerator FetchHousesData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl))    
        {
            webRequest.timeout = 120; // 120 seconds timeout for Render cold start

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.DataProcessingError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching houses: " + webRequest.error);
            }
            else
            {
                string json = webRequest.downloadHandler.text;
                ProcessHouseData(json);
            }
        }
    }

    private void ProcessHouseData(string json)
    {
        HouseData[] houses;
        try
        {
            houses = JsonHelper.FromJson<HouseData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse House JSON: " + e.Message);
            return;
        }

        if (houses == null || houses.Length == 0)
        {
            Debug.LogWarning("No houses received from API.");
            return;
        }

        if (floorsParent == null)
        {
            Debug.LogError("Floors parent object is not assigned or found.");
            return;
        }

        // Gather all unit game objects with tag "Unit" in the correct order
        // Assuming floors are children of floorsParent, and units are children of floors
        // We will just iterate sequentially through the hierarchy.
        
        int houseIndex = 0;

        foreach (Transform floorTransform in floorsParent)
        {
            // Sort or just use hierarchy order? Usually hierarchy order is fine.
            // But let's only consider children with tag "Unit" if specified, or all children if they are the units.
            foreach (Transform unitTransform in floorTransform)
            {
                if (unitTransform.CompareTag("Unit"))
                {
                    if (houseIndex < houses.Length)
                    {
                        HouseComponent comp = unitTransform.gameObject.GetComponent<HouseComponent>();
                        if (comp == null)
                        {
                            comp = unitTransform.gameObject.AddComponent<HouseComponent>();
                        }
                        
                        comp.SetData(houses[houseIndex]);
                        houseIndex++;
                    }
                    else
                    {
                        Debug.LogWarning("More Unit GameObjects in scene than houses from API.");
                        return; // Done assigning
                    }
                }
            }
        }

        Debug.Log($"Successfully assigned {houseIndex} houses to Unit GameObjects.");
    }
}
