using UnityEngine;

public class IceExplosion : Explosion
{
    public override bool DestroysBlocks()
    {
        return false;
    }

    public override void HitPlayer(PlayerController targetPlayer, Collider collision)
    {
        targetPlayer.Freeze();
    }
}
