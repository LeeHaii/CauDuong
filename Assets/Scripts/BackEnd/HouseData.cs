using System;
using UnityEngine;

[Serializable]
public class HouseData
{
    public int id;
    public string title;
    public long price;
    public string description;
    public string image_url;
    public string addressable_str;
    public float area_m2;
    public string status;
    public int residential_number;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
