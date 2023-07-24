using Assets.Scripts.Constants;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField]
    private Animator _animationController;

    private bool _isPickingUp;

    private const float PickupCancelTime = 1.1f;
    private float _remainingPickupTime;

    private void Update()
    {
        // TODO: Sometimes the pickup animation doesn't play and without this code the player gets stuck (seems to happen when spamming the pickup button?)
        if (_remainingPickupTime > 0)
        {
            _remainingPickupTime -= Time.deltaTime;
            if(_remainingPickupTime <= 0)
                OnPickupEnd();
        }
    }

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
        _remainingPickupTime = PickupCancelTime;
    }

    public void OnPickupEnd()
    {
        _remainingPickupTime = 0;
        _animationController.SetBool(GameConstants.AnimationVariables.PickingUp, false);
        _isPickingUp = false;
    }
}
