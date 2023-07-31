using Assets.Scripts.Constants;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animationController;

    private bool _isBusy;

    public bool CanMove()
    {
        return !_isBusy;
    }

    public bool IsBusy()
    {
        return _isBusy;
    }

    public void StartPickUp()
    {
        TryStartAnimation(GameConstants.AnimationVariables.PickingUp);
    }

    public void OnPickupEnd()
    {
        OnAnimationEnd(GameConstants.AnimationVariables.PickingUp);
    }

    public void StartPlacingBomb()
    {
        TryStartAnimation(GameConstants.AnimationVariables.PlacingBomb);
    }

    public void OnPlacingBombEnd()
    {
        OnAnimationEnd(GameConstants.AnimationVariables.PlacingBomb);
    }

    public void StartSpawnHeldBomb()
    {
        TryStartAnimation(GameConstants.AnimationVariables.SpawningHeldBomb);
    }

    public void OnSpawnHeldBombEnd()
    {
        OnAnimationEnd(GameConstants.AnimationVariables.SpawningHeldBomb);
    }

    private void TryStartAnimation(string animationVarName)
    {
        if (IsBusy()) return;

        _isBusy = true;
        _animationController.SetBool(animationVarName, true);
    }

    private void OnAnimationEnd(string animationVarName)
    {
        _animationController.SetBool(animationVarName, false);
        _isBusy = false;
    }
}
