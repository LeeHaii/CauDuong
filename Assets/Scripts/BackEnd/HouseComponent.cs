using UnityEngine;

public class HouseComponent : MonoBehaviour
{
    public HouseData Data { get; private set; }

    public void SetData(HouseData data)
    {
        this.Data = data;
    }
}
