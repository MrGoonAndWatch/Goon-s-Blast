using Photon.Pun;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    private TilePrefabLookup _tilePrefabLookup;
    [SerializeField]
    private Transform _parentObject;

    public void LoadLevel(LevelData levelData)
    {
        // TODO: Set Song

        // TODO: Set Background/Lighting/Skin Settings

        for (var i = 0; i < levelData.Tiles.Count; i++)
            CreateTile(levelData.Tiles[i]);
    }

    private void CreateTile(TileData tile)
    {
        var tilePos = new Vector3(tile.X, tile.Y, tile.Z);
        var tilePrefab = _tilePrefabLookup.GetPrefab(tile.Type);
        if (tilePrefab.GetComponent<GenerateHostSide>() == null)
            Instantiate(tilePrefab, tilePos, Quaternion.identity, _parentObject);
        // TODO: Implement this (need to figure out how to find full prefab path name for PUN.
        //else if (PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.Instantiate(_tilePrefabLookup.GetPrefabPath(), tilePos, Quaternion.identity);
    }
}
