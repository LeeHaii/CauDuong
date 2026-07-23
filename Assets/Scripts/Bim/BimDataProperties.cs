using UnityEngine;

public class BimDataProperties : MonoBehaviour
{
    private bool bimData;

    private void Awake()
    {
        bimData = false;
    }   
    
    public bool GetBIMdata()
    {
        return bimData;
    }

    public void EnableBIMData()
    {
        bimData = true;
    }

    public void DisableBIMData()
    {
        bimData = false;
    }
}
