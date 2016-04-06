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
        public static Dictionary<SolvingMode, ISolveGamesAlgorithm> SolvingModeAlgorithms { get; } = new Dictionary
            <SolvingMode, ISolveGamesAlgorithm>()
        {
            {SolvingMode.Simple, new SimpleAlgorithm()}
        };


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


        public SolvingMode SolvingMode { get; set; }

        public Solution Solve(Domain domain)
        {
            var solution = new Solution
            {
                IsSolvable = DomainIsSolvable(domain)
            };

            if (!solution.IsSolvable) return solution;

            var availablePlayers = GetAvailablePlayers(domain);

            // solve the damn thing
            foreach (var item in domain.ScheduleItems)
            {
                var playersAvailableForThisGame = availablePlayers[item.GameNumber];
                var game = SolvingModeAlgorithms[SolvingMode].SolveGame(item, playersAvailableForThisGame);

                solution = solution.AddGame(game);
            }

            return solution;
        }
        
        private Dictionary<int, GamePlayer[]> GetAvailablePlayers(Domain domain)
        {
            var retValue = new Dictionary<int, GamePlayer[]>();

            foreach (var item in domain.ScheduleItems)
            {
                var availablePlayers = domain.PlayerItems
                    .Except(
                        domain.PlayerGameAvailabilityItems
                            .Where(a => a.GameNumber == item.GameNumber)
                            .Select(b => new PlayerItem {Name = b.Name})
                        )
                    .Select(c => new GamePlayer
                    {
                        PlayerName = c.Name,
                        PositionRanks = domain.PlayerPositionItems
                                              .Where(d => d.Name == c.Name)
                                              .Select(e => new { e.Position, e.Rank })
                                              .ToDictionary(f => f.Position, g => g.Rank)
                    }).ToArray();

                retValue.Add(item.GameNumber, availablePlayers);
            }


            return retValue;
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