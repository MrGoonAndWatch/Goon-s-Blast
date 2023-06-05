using UnityEngine;

public abstract class MatchRulesManager : MonoBehaviour
{
    protected PlayerController[] Players = new PlayerController[0];
    protected bool PlayersInitialized;
    protected MatchTimer Timer;

    public void Init(MatchTimer timer)
    {
        Timer = timer;
    }

    public void UpdatePlayerList(PlayerController[] players)
    {
        Debug.Log($"UpdatePlayerList w/ {players.Length} players");

        if (players.Length == 0) return;

        Players = players;
        PlayersInitialized = true;
    }

    public abstract bool HasMatchEnded();
    public abstract string ProcessMatchEnd();
}
