using System.Collections.Generic;
using System.Linq;

public class KillMatchRulesManager : MatchRulesManager
{
    private int _killsToWin;

    public void SetKillsToWin(int killsToWin)
    {
        _killsToWin = killsToWin;
    }

    public override bool HasMatchEnded()
    {
        if (!PlayersInitialized) return false;

        var playersWithMostKills = GetPlayersWithMostKills();
        var maxKills = playersWithMostKills.Length > 0 ? playersWithMostKills[0].GetKillCount() : -1;

        return Timer.IsTimeUp() || maxKills >= _killsToWin;
    }

    private PlayerController[] GetPlayersWithMostKills()
    {
        var topPlayers = new List<PlayerController>();
        var mostKills = 0;
        for (var i = 0; i < Players.Length; i++)
        {
            var kills = Players[i].GetKillCount();
            if (kills > mostKills)
            {
                topPlayers.Clear();
                mostKills = kills;
            }

            if (kills >= mostKills)
            {
                topPlayers.Add(Players[i]);
            }
        }

        return topPlayers.ToArray();
    }

    public override string ProcessMatchEnd()
    {
        Timer.StopTimer();

        var winningPlayers = GetPlayersWithMostKills();

        if (winningPlayers.Length == 0)
            return "Draw";
        
        return $"{string.Join("/", winningPlayers.Select(p => p.GetName()))} WIN!";
    }
}
