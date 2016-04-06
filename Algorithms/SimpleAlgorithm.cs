using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    foreach (var position in LineupSolver.AvailablePositions())
                    {
                        inning[position] = playersAvailableForThisGame[i].PlayerName;
                        i++;
                        if (i == playersAvailableForThisGame.Length) i = 0;
                    }
                    j++;
                }
            }
            return game;
        }
    }
}
