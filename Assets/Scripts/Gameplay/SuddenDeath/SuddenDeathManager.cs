using Assets.Scripts.Constants;
using Photon.Pun;
using UnityEngine;

public class SuddenDeathManager : MonoBehaviour
{
    private PhotonView _photonView;

    private bool _initialized;
    private bool _suddenDeathTimerStarted;
    private SuddenDeath _suddenDeath;
    private float _secondsUntilSuddenDeath;
    
    public void Init(GameConstants.MatchSettings matchSettings)
    {
        _photonView = GetComponent<PhotonView>();

        if (matchSettings.SuddenDeathType == GameConstants.SuddenDeathType.None || matchSettings.SuddenDeathStartsAt <= 0) return;

        switch (matchSettings.SuddenDeathType)
        {
            case GameConstants.SuddenDeathType.CannonBalls:
                _suddenDeath = gameObject.AddComponent<CannonBallSuddenDeath>();
                break;
            case GameConstants.SuddenDeathType.BombRain:
                _suddenDeath = gameObject.AddComponent<BombRainSuddenDeath>();
                break;
            default:
                Debug.LogWarning($"Unknown sudden death type '{matchSettings.SuddenDeathType}', skipping sudden death init!");
                return;
        }

        _secondsUntilSuddenDeath = matchSettings.TimerSeconds - matchSettings.SuddenDeathStartsAt;
        _initialized = true;
    }
    
    private void Update()
    {
        if (!_initialized || _suddenDeathTimerStarted) return;
        if (!_photonView.IsMine) return;

        _secondsUntilSuddenDeath -= Time.deltaTime;
        if (_secondsUntilSuddenDeath <= 0)
        {
            _suddenDeath.StartSuddenDeath();
            _suddenDeathTimerStarted = true;
        }
    }
}
