using System;
using UnityEngine;

public class SurvivorMatchRulesManager : MatchRulesManager
{
    private bool _isTimed;
    private TimeSpan _remainingTime;
    private MatchTimerDisplay _timerDisplay;
    private bool _stopTimer;

    public void Init(bool isTimed, TimeSpan? timeLimit = null)
    {
        _remainingTime = timeLimit ?? TimeSpan.Zero;
        _isTimed = isTimed;
        _timerDisplay = FindObjectOfType<MatchTimerDisplay>();
        if (!_isTimed) _timerDisplay.HideDisplay();
        Players = new PlayerController[0];
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (!_isTimed || _stopTimer) return;

        _remainingTime = _remainingTime.Add(-TimeSpan.FromSeconds(Time.deltaTime));

        if (_remainingTime <= TimeSpan.Zero)
        {
            _remainingTime = TimeSpan.Zero;
            _stopTimer = true;
        }

        _timerDisplay.SetTimerText(_remainingTime.ToString(@"mm\:ss"));
    }

    private bool IsTimeUp()
    {
        return _isTimed && _remainingTime.TotalMilliseconds <= 0;
    }

    public override bool HasMatchEnded()
    {
        if (!PlayersInitialized) return false;

        var playersRemaining = 0;
        for (var i = 0; i < Players.Length; i++)
            if (Players[i].IsAlive())
                playersRemaining++;

        return IsTimeUp() || (Players.Length > 1 && playersRemaining <= 1) || playersRemaining == 0;
    }

    public override string ProcessMatchEnd()
    {
        _stopTimer = true;
        _timerDisplay.HideDisplay();

        var lastAliveName = "";
        var playersRemaining = 0;
        for (var i = 0; i < Players.Length; i++)
        {
            if (Players[i].IsAlive())
            {
                playersRemaining++;
                lastAliveName = Players[i].GetName();
            }
        }
        var winningMessage = playersRemaining > 1 || playersRemaining == 0 || Players.Length == 1 ? "DRAW" : $"{lastAliveName} WINS!";
        return winningMessage;
    }
}
