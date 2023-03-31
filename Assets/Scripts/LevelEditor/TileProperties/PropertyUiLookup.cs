using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertyUiLookup : MonoBehaviour
{
    [SerializeField]
    private TileLookup[] _lookup;

    private Dictionary<TileType, GameObject> _dictLookup;

    private void Start()
    {
        _dictLookup = new Dictionary<TileType, GameObject>();

        for (var i = 0; i < _lookup.Length; i++)
        {
            var lookup = _lookup[i];
            if (_dictLookup.ContainsKey(lookup.Type))
                Debug.LogWarning($"More than one property prefab mapping found for BlockType '{lookup.Type}'! Only mapping the first prefab listed!");
            else
                _dictLookup.Add(lookup.Type, lookup.Prefab);
        }
    }

    public GameObject GetPropertyUiPrefab(TileType type)
    {
        return _dictLookup.ContainsKey(type) ? _dictLookup[type] : null;
    }
}

[Serializable]
public class PropertyUi
{
    public TileType Type;
    public GameObject Prefab;
}