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

    public virtual void HitPlayer(PlayerController targetPlayer, Collider collision)
    {
        var playerWhoLaidBomb = GetPlayerWhoLaidBomb();
        string causeOfDeath;
        if (playerWhoLaidBomb == null)
            causeOfDeath = "an explosion";
        else if (playerWhoLaidBomb.GetPhotonViewId() == targetPlayer.GetPhotonViewId())
            causeOfDeath = $"{targetPlayer.GetName()} blew themselves up!";
        else
            causeOfDeath = $"{playerWhoLaidBomb.GetName()}'s explosion";

        targetPlayer.DamagePlayer(causeOfDeath, playerWhoLaidBomb);
    }

    private PlayerController GetPlayerWhoLaidBomb()
    {
        var bomb = transform.parent.parent.GetComponent<Bomb>();
        return bomb._spawnedByPlayer;
    }
}
