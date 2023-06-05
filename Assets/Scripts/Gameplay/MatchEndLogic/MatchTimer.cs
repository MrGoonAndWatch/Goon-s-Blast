using System;
using UnityEngine;

public class MatchTimer : MonoBehaviour
{
    private bool _initialized;
    private bool _isTimed;
    private TimeSpan _remainingTime;
    private MatchTimerDisplay _timerDisplay;
    private bool _stopTimer;

    public virtual void Init(bool isTimed, TimeSpan? timeLimit = null)
    {
        _remainingTime = timeLimit ?? TimeSpan.Zero;
        _isTimed = isTimed;
        _timerDisplay = FindObjectOfType<MatchTimerDisplay>();
        if (!_isTimed) _timerDisplay.HideDisplay();
        _initialized = true;
    }

    public bool IsTimeUp()
    {
        return _initialized && _isTimed && _remainingTime.TotalMilliseconds <= 0;
    }

    public void StopTimer()
    {
        _stopTimer = true;
        _timerDisplay.HideDisplay();
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
}
