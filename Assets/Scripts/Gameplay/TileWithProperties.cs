using Newtonsoft.Json;
using UnityEngine;

public abstract class TileWithProperties : MonoBehaviour
{
    public abstract void SetDefaultProperties();
    public abstract void LoadProperties(string propertyJson);

    protected T LoadProperties<T>(string propertyJson)
    {
        return JsonConvert.DeserializeObject<T>(propertyJson);
    }
}
