using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class IfcMetadataEntry
{
    [SerializeField] private string key;
    [SerializeField] private string value;

    public string Key => key;
    public string Value => value;

    public IfcMetadataEntry(string propertyKey, string propertyValue)
    {
        key = propertyKey;
        value = propertyValue;
    }
}

public class IfcElementMetadata : MonoBehaviour
{
    [SerializeField] private string ifcType;
    [SerializeField] private string globalId;
    [SerializeField] private int entityLabel;
    [SerializeField] private List<IfcMetadataEntry> properties = new();

    private Dictionary<string, string> propertyMap;

    public string IfcType => ifcType;
    public string GlobalId => globalId;
    public int EntityLabel => entityLabel;
    public IReadOnlyDictionary<string, string> Properties
    {
        get
        {
            EnsurePropertyMap();
            return propertyMap;
        }
    }

    public void Initialize(string typeName, string id, int label)
    {
        Initialize(typeName, id, label, null);
    }

    public void Initialize(
        string typeName,
        string id,
        int label,
        IEnumerable<KeyValuePair<string, string>> values)
    {
        ifcType = typeName;
        globalId = id;
        entityLabel = label;
        properties.Clear();

        if (values != null)
        {
            foreach (var pair in values)
            {
                if (!string.IsNullOrWhiteSpace(pair.Key))
                {
                    properties.Add(new IfcMetadataEntry(pair.Key, pair.Value ?? string.Empty));
                }
            }
        }

        propertyMap = null;
    }

    public IEnumerable<KeyValuePair<string, string>> GetProperties()
    {
        yield return new KeyValuePair<string, string>("IFC Type", ifcType);
        yield return new KeyValuePair<string, string>("Global ID", globalId);
        yield return new KeyValuePair<string, string>("Entity Label", entityLabel.ToString());

        foreach (var property in properties)
        {
            yield return new KeyValuePair<string, string>(property.Key, property.Value);
        }
    }

    private void EnsurePropertyMap()
    {
        if (propertyMap != null)
        {
            return;
        }

        propertyMap = new Dictionary<string, string>(properties.Count);
        foreach (var property in properties)
        {
            propertyMap[property.Key] = property.Value;
        }
    }
}
