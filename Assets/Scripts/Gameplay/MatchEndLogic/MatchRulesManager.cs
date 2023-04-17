using UnityEngine;

public abstract class MatchRulesManager : MonoBehaviour
{
    protected PlayerController[] Players;
    protected bool PlayersInitialized;

    public void UpdatePlayerList(PlayerController[] players)
    {
        Players = players;
        PlayersInitialized = true;
    }

    public abstract bool HasMatchEnded();
    public abstract string ProcessMatchEnd();
}
