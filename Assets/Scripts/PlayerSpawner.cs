using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("The Addressable key/address for the Player prefab")]
    public string playerAddress = "Player";

    void Start()
    {
        if (!string.IsNullOrEmpty(playerAddress))
        {
            // Spawn the player using Addressables string key
            Addressables.InstantiateAsync(playerAddress, transform.position, transform.rotation).Completed += (handle) =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"PlayerSpawner: Failed to instantiate Addressable with key '{playerAddress}'.");
                }
            };
        }
        else
        {
            Debug.LogError("PlayerSpawner: Player Address string is empty.");
        }
    }
}
