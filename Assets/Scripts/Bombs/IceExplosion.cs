public class IceExplosion : Explosion
{
    public override bool DestroysBlocks()
    {
        return false;
    }

    public override void HitPlayer(PlayerController targetPlayer)
    {
        targetPlayer.Freeze();
    }
}
