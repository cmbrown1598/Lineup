using System;
using System.Linq;
using Algorithms;

namespace Tests
{
    public class Solver
    {
        public Solution Solve(Domain domain)
        {
            var solution = new Solution
            {
                IsSolvable = DomainIsSolvable(domain)
            };

            if (!solution.IsSolvable) return solution;


            // solve the damn thing
            foreach (var item in domain.ScheduleItems)
            {
                var availablePlayers = domain.PlayerItems.Except(
                    domain.PlayerGameAvailabilityItems
                        .Where(a => a.GameNumber == item.GameNumber)
                        .Select(b => new PlayerItem { Name = b.Name})
                    ).ToArray();

                var game = new Game { Name = $"{item.GameNumber} - {item.Opponent}" };

                if (availablePlayers.Length < 10)
                {
                    game.IsForfiet = true;
                }
                else
                {
                    var i = 0;
                    foreach (var inning in game.Innings)
                    {
                        i = 0;
                        foreach (Position position in Enum.GetValues(typeof (Position)))
                        {
                            inning[position] = availablePlayers[i].Name;
                            i++;
                        }
                    }

                }

                solution = solution.AddGame(game);
            }

            return solution;
        }

        private bool DomainIsSolvable(Domain domain) 
        {
            var solvable = domain.PlayerItems?.Any() ?? false;
            solvable &= domain.ScheduleItems?.Any() ?? false;
            solvable &= domain.PlayerPositionItems?.Any() ?? false;
            solvable &= domain.PlayerGameAvailabilityItems != null; 


            return solvable;
        }
    }
}