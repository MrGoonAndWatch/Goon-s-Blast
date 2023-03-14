public class BombsUp : Powerup
{
    public override void PickUp(PlayerController playerPickingUp)
    {
        playerPickingUp.IncreaseMaxBombs();
        base.PickUp(playerPickingUp);
    }
}
