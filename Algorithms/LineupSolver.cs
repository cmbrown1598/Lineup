using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;

namespace Algorithms
{
    public class LineupSolver
    {
        private static ReadOnlyCollection<Position> _availablePositions; 

        public static ReadOnlyCollection<Position> AvailablePositions()
        {
            return _availablePositions ?? (_availablePositions = new ReadOnlyCollection<Position>(new[]
            {
                Position.Pitcher,
                Position.Catcher,
                Position.FirstBase,
                Position.SecondBase,
                Position.ShortStop,
                Position.ThirdBase,
                Position.RightField,
                Position.CenterField,
                Position.LeftField,
                Position.Rover
            }));
        }

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
                    var j = 0;
                    foreach (var inning in game.Innings)
                    {
                        var i = j;
                        foreach (var position in AvailablePositions())
                        {
                            inning[position] = availablePlayers[i].Name;
                            i++;
                            if (i == availablePlayers.Length) i = 0;
                        }
                        j++;
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