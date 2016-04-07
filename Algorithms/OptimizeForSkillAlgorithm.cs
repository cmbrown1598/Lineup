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
                inning.SittingOut =
                    playersAvailableForThisGame.Where(a => !added.Contains(a.PlayerName))
                        .Select(b => b.PlayerName)
                        .ToArray();
            }

            var inningsToSkip = new [] {2, 5};

            foreach (var i in inningsToSkip)
            {
                var added = new List<string>();
                foreach (var position in LineupSolver.AvailablePositions())
                {
                    var currentPlayer = game.Innings.Skip(i).First()[position];
                    var desiredPlayer = playersByPosition[position].First(a => !added.Contains(a) && a != currentPlayer);
                    game.Innings.Skip(i).First()[position] = desiredPlayer;
                    added.Add(desiredPlayer);
                }
                game.Innings.Skip(i).First().SittingOut =
                    playersAvailableForThisGame.Where(a => !added.Contains(a.PlayerName))
                        .Select(b => b.PlayerName)
                        .ToArray();
            }

            return game;
        }

        
    }
}