using Photon.Pun;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{
    [SerializeField]
    private PhotonView _photonView;
    [SerializeField]
    private float RotationSpeed = 180f;

    private bool _alreadyPickedUp;

    public bool AlreadyPickedUp()
    {
        return _alreadyPickedUp;
    }

    public virtual void PickUp(PlayerController playerPickingUp)
    {
        _alreadyPickedUp = true;
        _photonView.RPC(nameof(DestroyPickup), RpcTarget.All);
    }

    [PunRPC]
    public void DestroyPickup()
    {
        _alreadyPickedUp = true;
        if (_photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        gameObject.transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
    }
}
