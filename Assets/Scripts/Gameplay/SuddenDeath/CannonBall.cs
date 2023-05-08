using Photon.Pun;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField]
    private const float TimeToExist = 10;

    private float _timeLeft;

    private void Start()
    {
        _timeLeft = TimeToExist;
    }

    public void Init(Vector3 direction)
    {
        Debug.Log($"init'd a cannonball ({direction})!");
        var cannonBallBall = GetComponentInChildren<CannonBallBall>();
        cannonBallBall.Init(direction);
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
}
