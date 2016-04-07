using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class OptimizeForSkillAlgorithm : ISolveGamesAlgorithm
    {
        public Game SolveGame(ScheduleItem item, GamePlayer[] playersAvailableForThisGame)
        {
            var game = new Game { Name = $"{item.GameNumber} - {item.Opponent}" };

            var playersByPosition = LineupSolver.AvailablePositions()
                .ToDictionary(position => position,
                    pos =>
                        playersAvailableForThisGame.OrderByDescending(v => v.PositionRanks[pos])
                            .Select(b => b.PlayerName)
                            .ToArray());

            foreach (var inning in game.Innings)
            {
                var added = new List<string>();
                foreach (var position in LineupSolver.AvailablePositions())
                {
                    inning[position] = playersByPosition[position].First(a => !added.Contains(a));
                    added.Add(inning[position]);
                }
            }

            foreach (var position in LineupSolver.AvailablePositions())
            {

            }
            
            return game;
        }

        
    }
}