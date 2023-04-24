using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class CannonBallSuddenDeath : SuddenDeath
{
    private const float TimeBetweenSpawnsInSeconds = 2.0f;

    private float _nextSpawnInSeconds;

    private void Update()
    {
        if (!SuddenDeathStarted) return;

        _nextSpawnInSeconds -= Time.deltaTime;
        if(_nextSpawnInSeconds <= 0)
            SpawnCannonBall();
    }

    private void SpawnCannonBall()
    {
        // TODO: Spawn at a random location along the axis you spawn in!
        // TODO: Actually knock the player around when they get hit by a cannonball!
        var chosenDir = Random.Range(0, 4);
        Vector3 direction;
        Vector3 spawnPos;
        // TODO: Take map in to consideration when picking lifespan & spawnPos!
        switch (chosenDir)
        {
            case 0:
                spawnPos = new Vector3(0, 2, 20);
                direction = new Vector3(0, 0, -1);
                break;
            case 1:
                spawnPos = new Vector3(20, 2, 0);
                direction = new Vector3(-1, 0, 0);
                break;
            case 2:
                spawnPos = new Vector3(0, 2, -20);
                direction = new Vector3(0, 0, 1);
                break;
            default:
                spawnPos = new Vector3(-20, 2, 0);
                direction = new Vector3(1, 0, 0);
                break;
        }

        var cannonBall = PhotonNetwork.Instantiate(GameConstants.SpawnablePrefabs.CannonBall, spawnPos, Quaternion.identity).GetComponent<CannonBall>();
        cannonBall.Init(direction);
        _nextSpawnInSeconds = TimeBetweenSpawnsInSeconds;
    }
}
