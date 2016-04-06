using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class Solver
    {

        private class GamePlayer
        {
            public GamePlayer()
            {
                PositionsInOrderOfPreference = new List<Position>();
            }

            public string PlayerName { get; set; }

            public List<Position> PositionsInOrderOfPreference { get; private set; }
        }




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
                    foreach (var inning in game.Innings)
                    {
                        var i = 0;
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