using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class SimpleAlgorithm : ISolveGamesAlgorithm
    {
        public Game SolveGame(ScheduleItem item, GamePlayer[] playersAvailableForThisGame)
        {
            var game = new Game { Name = $"{item.GameNumber} - {item.Opponent}" };

            if (playersAvailableForThisGame.Length < 8)
            {
                game.IsForfiet = true;
            }
            else
            {
                var j = 0;
                foreach (var inning in game.Innings)
                {
                    var i = j;
                    var added = new List<string>();
                    foreach (var position in LineupSolver.AvailablePositions())
                    {
                        inning[position] = playersAvailableForThisGame[i].PlayerName;
                        added.Add(inning[position]);
                        i++;
                        if (i == playersAvailableForThisGame.Length) i = 0;
                    }

                    inning.SittingOut = playersAvailableForThisGame.Select(a => a.PlayerName)
                        .Except(added)
                        .ToArray();

                    j++;
                }
            }
            return game;
        }
    }
}
