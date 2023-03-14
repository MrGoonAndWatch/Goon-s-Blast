using Photon.Pun;
using UnityEngine;

public abstract class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject _bombMesh;
    [SerializeField] private GameObject _explosionMesh;
    [SerializeField] private float _baseExplosionEndSize = 3.0f;
    [SerializeField] private float _explosionIncreaseRatePerSec = 0.5f;

    protected bool _initialized;
    private float _maxExplosionSize;

    private PlayerController _spawnedByPlayer;
    [SerializeField]
    private PhotonView _photonView;

    protected bool _exploding;
    private float _currentScale = 1.0f;

    protected bool IsMine()
    {
        return _photonView.IsMine;
    }

    public void Initialize(PlayerController spawnedByPlayer, int firePower)
    {
        _spawnedByPlayer = spawnedByPlayer;
        _photonView.RPC(nameof(InitParams), RpcTarget.All, firePower);
    }

    [PunRPC]
    protected void InitParams(int firePower)
    {
        _maxExplosionSize = _baseExplosionEndSize * firePower;
        InitBombData();
        _initialized = true;
    }

    protected abstract void InitBombData();

    protected void Update()
    {
        if (!_initialized) return;

        if (_exploding)
            HandleExplosion();
    }

    private void HandleExplosion()
    {
        _currentScale += _explosionIncreaseRatePerSec * Time.deltaTime;
        if (_currentScale > _maxExplosionSize)
            _currentScale = _maxExplosionSize;
        _explosionMesh.transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
        if (_photonView.IsMine && _currentScale >= _maxExplosionSize)
            PhotonNetwork.Destroy(gameObject);
    }

    public void Explode()
    {
        _photonView.RPC(nameof(StartExplosion), RpcTarget.All);
        if (_spawnedByPlayer != null && _photonView.IsMine)
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
}
