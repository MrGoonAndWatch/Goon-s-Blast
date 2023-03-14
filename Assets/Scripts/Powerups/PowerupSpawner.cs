using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public void SpawnPowerup(Destructable spawnFrom)
    {
        var prefabToSpawn = GetContentsPrefabPath(spawnFrom);
        if (string.IsNullOrEmpty(prefabToSpawn) || !CheckChanceToSpawn(spawnFrom))
            return;
        
        PhotonNetwork.Instantiate(prefabToSpawn, spawnFrom.gameObject.transform.position, Quaternion.identity);
    }

    private static string GetContentsPrefabPath(Destructable destructable)
    {
        switch (destructable.Contains)
        {
            case GameConstants.DestructableContents.Random:
                var selectedPowerupIndex = Random.Range(0, GameConstants.SpawnablePrefabs.Powerups.Length);
                return GameConstants.SpawnablePrefabs.Powerups[selectedPowerupIndex];
            case GameConstants.DestructableContents.Nothing:
                return null;
            default:
                var index = (int)destructable.Contains - 2;
                return GameConstants.SpawnablePrefabs.Powerups[index];
        }
    }

    private static bool CheckChanceToSpawn(Destructable destructable)
    {
        if (destructable.SpawnPowerupChance <= 0.0f)
            return false;
        var randomNum = Random.Range(0.0f, 1.0f);
        return randomNum <= destructable.SpawnPowerupChance;
    }
}
