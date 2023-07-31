using Assets.Scripts.Constants;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animationController;

    private bool _isPickingUp;
    private bool _isPlacingBomb;

    public bool CanMove()
    {
        return !_isPickingUp && !_isPlacingBomb;
    }

    public bool CanPickUp()
    {
        return !_isPickingUp && !_isPlacingBomb;
    }

    public bool IsBusy()
    {
        return _isPickingUp || _isPlacingBomb;
    }

    public void StartPickUp()
    {
        if (IsBusy()) return;

        _isPickingUp = true;
        _animationController.SetBool(GameConstants.AnimationVariables.PickingUp, true);
    }

    public void OnPickupEnd()
    {
        _animationController.SetBool(GameConstants.AnimationVariables.PickingUp, false);
        _isPickingUp = false;
    }

    public void StartPlacingBomb()
    {
        if (IsBusy()) return;

        _isPlacingBomb = true;
        _animationController.SetBool(GameConstants.AnimationVariables.PlacingBomb, true);
    }

    public void OnPlacingBombEnd()
    {
        _animationController.SetBool(GameConstants.AnimationVariables.PlacingBomb, false);
        _isPlacingBomb = false;
    }
}
