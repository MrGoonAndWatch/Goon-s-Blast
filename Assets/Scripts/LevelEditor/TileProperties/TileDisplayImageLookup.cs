using System;
using System.Collections.Generic;
using UnityEngine;

public class TileDisplayImageLookup : MonoBehaviour
{
    [SerializeField]
    private BlockImageLookup[] _lookup;

    private Dictionary<TileType, Sprite> _dictLookup;

    private void Start()
    {
        _dictLookup = new Dictionary<TileType, Sprite>();

        for (var i = 0; i < _lookup.Length; i++)
        {
            var lookup = _lookup[i];
            if (_dictLookup.ContainsKey(lookup.Type))
                Debug.LogWarning($"More than one property prefab mapping found for BlockType '{lookup.Type}'! Only mapping the first prefab listed!");
            else
                _dictLookup.Add(lookup.Type, lookup.Image);
        }
    }

    public Sprite GetBlockDisplayImage(TileType type)
    {
        return _dictLookup.ContainsKey(type) ? _dictLookup[type] : null;
    }
}

[Serializable]
public class BlockImageLookup
{
    public TileType Type;
    public Sprite Image;
}
