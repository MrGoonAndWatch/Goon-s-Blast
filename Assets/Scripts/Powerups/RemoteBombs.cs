public class RemoteBombs : Powerup
{
    public override void PickUp(PlayerController playerPickingUp)
    {
        playerPickingUp.SetRemoteBombs();
        base.PickUp(playerPickingUp);
    }
}
