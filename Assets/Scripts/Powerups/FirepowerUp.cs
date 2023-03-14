public class FirepowerUp : Powerup
{
    public override void PickUp(PlayerController playerPickingUp)
    {
        playerPickingUp.IncreaseFirepower();
        base.PickUp(playerPickingUp);
    }
}
