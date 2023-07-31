using Assets.Scripts.Constants;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animationController;

    private bool _isPickingUp;

    public bool CanMove()
    {
        return !_isPickingUp;
    }

    public bool CanPickUp()
    {
        return !_isPickingUp;
    }

    public void StartPickUp()
    {
        if (!CanPickUp()) return;

        _isPickingUp = true;
        _animationController.SetBool(GameConstants.AnimationVariables.PickingUp, true);
    }

    public void OnPickupEnd()
    {
        _animationController.SetBool(GameConstants.AnimationVariables.PickingUp, false);
        _isPickingUp = false;
    }
}
