using UnityEngine;

public class Explosion : MonoBehaviour
{
    public virtual bool CausesChainExplosions()
    {
        return true;
    }

    public virtual bool DestroysBlocks()
    {
        return true;
    }

    public PlayerController GetPlayerWhoLaidBomb()
    {
        var bomb = transform.parent.parent.GetComponent<Bomb>();
        return bomb._spawnedByPlayer;
    }
}
