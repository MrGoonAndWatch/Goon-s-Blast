using UnityEngine;

public class Explosion : MonoBehaviour
{
    public PlayerController GetPlayerWhoLaidBomb()
    {
        var bomb = transform.parent.parent.GetComponent<Bomb>();
        return bomb._spawnedByPlayer;
    }
}
