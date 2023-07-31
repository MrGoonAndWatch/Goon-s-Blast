using UnityEngine;

public class PlayerAnimationEventHook : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimationManager _animationManager;
    [SerializeField]
    private PlayerController _playerController;

    public void PickUpObject()
    {
        _playerController.TryToPickUp();
    }

    public void OnPickUpEnd()
    {
        _animationManager.OnPickupEnd();
    }

    public void PlaceBomb()
    {
        _playerController.PlaceBomb(false);
    }

    public void OnPlaceBombEnd()
    {
        _animationManager.OnPlacingBombEnd();
    }

    public void SpawnHeldBomb()
    {
        _playerController.PlaceBomb(true);
    }

    public void OnSpawnHeldBombEnd()
    {
        _animationManager.OnSpawnHeldBombEnd();
    }
}
