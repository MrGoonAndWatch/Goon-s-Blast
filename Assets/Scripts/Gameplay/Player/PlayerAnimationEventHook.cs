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
        _playerController.PlaceBomb();
    }

    public void OnPlaceBombEnd()
    {
        _animationManager.OnPlacingBombEnd();
    }
}
