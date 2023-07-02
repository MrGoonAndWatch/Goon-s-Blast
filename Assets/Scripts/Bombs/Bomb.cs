using System.Linq;
using Photon.Pun;
using UnityEngine;

public abstract class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _bombMesh;
    [SerializeField] private GameObject _explosionMesh;
    [SerializeField] private float _baseExplosionEndSize = 3.0f;
    [SerializeField] private float _explosionIncreaseRatePerSec = 0.5f;
    
    private const float HeldDistanceInFrontOfPlayer = 1.5f;
    private const float HeldDistanceAbovePlayerCenter = 1.0f;

    private const float ThrowForceHorizontal = 2.5f;
    private const float ThrowForceVerticalAddition = 150.0f;
    private const float ThrowForceVerticalMomentum = 15.0f;

    public int BombNumber;

    protected bool _initialized;
    private float _maxExplosionSize;

    public PlayerController _spawnedByPlayer;
    [SerializeField]
    protected PhotonView _photonView;

    protected bool _exploding;
    private float _currentScale = 1.0f;

    private bool _isHeld;
    private PlayerController _heldByPlayer;
    private bool _isThrown;
    private Vector3 _throwDir;

    [SerializeField]
    private Rigidbody _bombRigidBody;

    public void Pickup(PlayerController playerPickingUp)
    {
        _isHeld = true;
        _photonView.RPC(nameof(DisableGravity), RpcTarget.All);
        if(!_photonView.IsMine)
            _photonView.TransferOwnership(playerPickingUp.GetPhotonViewOwner());
        _isHeld = true;
        _heldByPlayer = playerPickingUp;
    }

    [PunRPC]
    public void DisableGravity()
    {
        if(_bombRigidBody != null)
            _bombRigidBody.useGravity = false;
    }

    public bool IsHeld()
    {
        return _isHeld;
    }

    public void OnOwnershipRequest(object[] viewAndPlayer)
    {
        Debug.Log($"OnOwnershipRequest(): Player {viewAndPlayer[1]}, view {viewAndPlayer[0]}");
    }

    public void Throw(Vector3 direction)
    {
        if (_exploding) return;

        _photonView.RPC(nameof(TossBomb), RpcTarget.All, direction.x, direction.y, direction.z);
    }

    [PunRPC]
    public void TossBomb(float xVelocity, float yVelocity, float zVelocity)
    {
        if (_photonView.IsMine)
        {
            // TODO: Don't bother transferring ownership here if player holding bomb is the player that spawned it.
            _photonView.TransferOwnership(_spawnedByPlayer.GetPhotonViewOwner());
        }

        if (_bombRigidBody == null) return;
        
        _isThrown = true;
        _throwDir = new Vector3(xVelocity * ThrowForceHorizontal, 0, zVelocity * ThrowForceHorizontal);
        Debug.Log($"Throwing bomb at dir {_throwDir}");
        Debug.Log($"Throw Y dir = {yVelocity} * {ThrowForceVerticalMomentum}");
        _isHeld = false;
        _heldByPlayer = null;
        _bombRigidBody.useGravity = true;
        _bombRigidBody.AddForce(0, yVelocity * ThrowForceVerticalMomentum + ThrowForceVerticalAddition, 0);
        // Reset timer on timed bombs.
        if (!RoomManager.GetMatchSettings().RunBombTimerWhenHeld)
            InitBombData();
    }

    public void Drop()
    {
        if (_exploding) return;

        _photonView.RPC(nameof(DropBomb), RpcTarget.All);
    }

    [PunRPC]
    public void DropBomb()
    {
        if (_photonView.IsMine)
        {
            // TODO: Don't bother transferring ownership here if player holding bomb is the player that spawned it.
            _photonView.TransferOwnership(_spawnedByPlayer.GetPhotonViewOwner());
        }

        if (_bombRigidBody == null) return;

        _isHeld = false;
        _heldByPlayer = null;
        _bombRigidBody.useGravity = true;
        // Reset timer on timed bombs.
        if (!RoomManager.GetMatchSettings().RunBombTimerWhenHeld)
            InitBombData();
    }

    public bool IsMine()
    {
        return _photonView.IsMine;
    }

    public void Initialize(PlayerController spawnedByPlayer, int firePower, int bombNumber)
    {
        _spawnedByPlayer = spawnedByPlayer;
        _photonView.RPC(nameof(InitParams), RpcTarget.All, firePower, bombNumber, spawnedByPlayer.GetPhotonViewId());
    }

    [PunRPC]
    protected void InitParams(int firePower, int bombNumber, int spawnedByPlayerPvId)
    {
        if (_spawnedByPlayer == null)
            _spawnedByPlayer = FindObjectsOfType<PlayerController>()
                .FirstOrDefault(pc => pc.GetPhotonViewId() == spawnedByPlayerPvId);
        _maxExplosionSize = _baseExplosionEndSize * firePower;
        BombNumber = bombNumber;
        InitBombData();
        _initialized = true;
    }

    protected abstract void InitBombData();

    protected void Update()
    {
        if (!_initialized) return;

        SyncWithMeshYPos();
        HandleBombCarry();
        HandleBombThrow();
        HandleExplosion();
    }

    private void SyncWithMeshYPos()
    {
        if (!_photonView.IsMine || _exploding) return;

        if (_bombMesh.transform.localPosition.y != 0)
        {
            var offset = _bombMesh.transform.localPosition.y;
            transform.position += new Vector3(0, offset, 0);
            _bombMesh.transform.position -= new Vector3(0, offset, 0);
        }
    }

    private void HandleBombCarry()
    {
        if (!_isHeld || _exploding || !_photonView.IsMine) return;
        
        // TODO: smooth this movement out instead of instantly moving to this position.
        transform.position = _heldByPlayer.transform.position + (_heldByPlayer.transform.forward * HeldDistanceInFrontOfPlayer) + new Vector3(0 ,HeldDistanceAbovePlayerCenter, 0);
    }

    private void HandleBombThrow()
    {
        if (!_photonView.IsMine) return;

        if (_exploding)
        {
            StopThrow();
            return;
        }

        transform.position += new Vector3(_throwDir.x * Time.deltaTime, 0, _throwDir.z * Time.deltaTime);
    }

    private void HandleExplosion()
    {
        if (!_exploding) return;

        _currentScale += _explosionIncreaseRatePerSec * Time.deltaTime;
        if (_currentScale > _maxExplosionSize)
            _currentScale = _maxExplosionSize;
        _explosionMesh.transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
        if (_photonView.IsMine && _currentScale >= _maxExplosionSize)
            PhotonNetwork.Destroy(gameObject);
    }

    public void Explode()
    {
        GoonsBlastAudioManager.PlayOneShot(GoonsBlastFmodAudioEvents.ExplosionSound, transform.position);
        _photonView.RPC(nameof(StartExplosion), RpcTarget.All);
        if (_spawnedByPlayer != null)
            _spawnedByPlayer.IncrementBombCount(this);
    }

    [PunRPC]
    protected void StartExplosion()
    {
        _exploding = true;
        // TODO: Play sound effect.
        _explosionMesh.transform.position = _bombMesh.transform.position;
        _bombMesh.SetActive(false);
        _explosionMesh.SetActive(true);
    }

    public void StopThrow()
    {
        _isThrown = false;
        _throwDir = Vector3.zero;
    }
}
