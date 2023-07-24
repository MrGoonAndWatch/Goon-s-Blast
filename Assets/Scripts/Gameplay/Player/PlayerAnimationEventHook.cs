using System.Collections;
using UnityEngine;

public class PlayerAnimationEventHook : MonoBehaviour
{
    [SerializeField]
    private PlayerAnimationManager _animationManager;
    [SerializeField]
    private PlayerController _playerController;

    public IEnumerator PickUpObject()
    {
        _playerController.TryToPickUp();
        yield return null;
    }

    public IEnumerator OnPickUpEnd()
    {
        _animationManager.OnPickupEnd();
        yield return null;
    }
}
