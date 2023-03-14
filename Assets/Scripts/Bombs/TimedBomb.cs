using UnityEngine;

public class TimedBomb : Bomb
{
    private const float BombTimer = 5f;
    private float _bombTimeRemaining;
    
    void Update()
    {
        if (!_initialized) return;

        base.Update();

        if(!_exploding)
            HandleTimer();
    }

    private void HandleTimer()
    {
        _bombTimeRemaining -= Time.deltaTime;
        if (_bombTimeRemaining <= 0)
            Explode();
    }

    protected override void InitBombData()
    {
        _bombTimeRemaining = BombTimer;
    }
}
