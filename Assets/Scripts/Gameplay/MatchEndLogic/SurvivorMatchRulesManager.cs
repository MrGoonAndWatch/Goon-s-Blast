public class SurvivorMatchRulesManager : MatchRulesManager
{
    public override bool HasMatchEnded()
    {
        if (!PlayersInitialized) return false;

        var playersRemaining = 0;
        for (var i = 0; i < Players.Length; i++)
            if (Players[i].IsAlive())
                playersRemaining++;

        return Timer.IsTimeUp() || (Players.Length > 1 && playersRemaining <= 1) || playersRemaining == 0;
    }

    public override string ProcessMatchEnd()
    {
        Timer.StopTimer();

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
