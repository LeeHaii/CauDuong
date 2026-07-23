using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ApartmentLoader : MonoBehaviour
{
    void Start()
    {
        string addressableStr = PlayerPrefs.GetString("SelectedApartment", "");
        if (!string.IsNullOrEmpty(addressableStr))
        {
            LoadApartment(addressableStr);
        }
        else
        {
            Debug.LogError("No addressable string found in PlayerPrefs.");
        }
    }

    private void LoadApartment(string addressableKey)
    {
        Addressables.InstantiateAsync(addressableKey).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($"Successfully loaded apartment: {addressableKey}");
            }
            else
            {
                Debug.LogError($"Failed to load apartment: {addressableKey}");
            }
        };
    }
}
