using Photon.Pun;
using UnityEngine;

public class StunBomb : Bomb
{
    protected override void HandleExplosion()
    {
        if (!_exploding) return;

        _currentScale += _explosionIncreaseRatePerSec * Time.deltaTime;
        if (_currentScale > _maxExplosionSize)
            _currentScale = _maxExplosionSize;
        _explosionMesh.transform.localScale = new Vector3(_currentScale, _explosionMesh.transform.localScale.y, _currentScale);
        if (_photonView.IsMine && _currentScale >= _maxExplosionSize)
            PhotonNetwork.Destroy(gameObject);
    }
}
