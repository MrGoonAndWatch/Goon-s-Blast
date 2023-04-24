using Photon.Pun;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private const float TimeToExist = 10;
    private const float MoveSpeed = 0.5f;

    private float _timeLeft;
    private Vector3 _moveDir;

    private void Start()
    {
        _timeLeft = TimeToExist;
    }

    public void Init(Vector3 direction)
    {
        Debug.Log($"init'd a cannonball ({direction})!");
        _moveDir = direction;
    }

    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0)
        {
            Debug.Log("Destroyed a cannonball!");
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        gameObject.transform.Translate(_moveDir * MoveSpeed);
    }
}
