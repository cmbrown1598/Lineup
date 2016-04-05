using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Algorithms;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class SolverTest
    {
        public Solver SystemUnderTest { get; set; }
        public Domain Domain { get; set; }

        public PlayerItem[] GetPlayers(int numberOfPlayers)
        {
            return Enumerable.Range(1, numberOfPlayers)
                             .Select(a => new PlayerItem {Name = $"Player {a}"})
                             .ToArray();
        }

        public PlayerPositionItem[] GetPlayerPositions(int numberOfPlayers)
        {
            return Enumerable.Range(1, numberOfPlayers)
                             .SelectMany(a => (int[]) Enum.GetValues(typeof (Position)), (a, n) => new PlayerPositionItem
                             {
                                Name = $"Player {a}",
                                Position = (Position) n,
                                Rank = a
                             }).ToArray();
        }

        [SetUp]
        public void Setup()
        {
            SystemUnderTest = new Solver();
            Domain = new Domain(
                    new[] { new ScheduleItem { GameNumber = 1, Opponent = "No One" } }, 
                    GetPlayers(10), 
                    new PlayerGameAvailabilityItem[0], 
                    GetPlayerPositions(10));
        }

        [Test]
        public void SolverCreatesAGameForEveryScheduleEntry()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.Count, Is.EqualTo(1));
            Assert.That(result.Games.First().Name, Is.EqualTo("1 - No One"));
        }

        [Test]
        public void SolveCreatesAGameWith7Innings()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.First().Innings.Count, Is.EqualTo(7));
        }

        [Test]
        public void WhenNoGamesAreSubmittedSolutionIsSetAsUnsolvable()
        {
            Domain.ScheduleItems = null;

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.False);
        }

        [Test]
        public void WhenNoPlayerPositionsAreSubmittedSolutionIsSetAsUnsolvable()
        {
            Domain.PlayerPositionItems = null;

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.False);
        }

        [Test]
        public void WhenNoPlayersAreSubmittedSolutionIsSetAsUnsolvable()
        {
            Domain.PlayerItems = null;

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.False);
        }

        [Test]
        public void WhenNoPlayerAvailabilityItemsAreSubmittedSolutionIsSetAsUnsolvable()
        {
            Domain.PlayerGameAvailabilityItems = null;

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.False);
        }

        [Test]
        public void WhenNotEnoughPlayersItemsAreSubmittedSolutionIsSetAsSolvableAndGameIsListedAsForfeit()
        {
            Domain.PlayerGameAvailabilityItems = new [] { new PlayerGameAvailabilityItem { GameNumber = 1, Name = "Player 1"}, };

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.True);
            Assert.That(result.Games.First().IsForfiet, Is.True);
        }

        [Test]
        public void SolveSolvesSuccessfullyForValidSetups()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.First().IsForfiet, Is.False);

        }
    }

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

    public class Solution
    {
        private Game[] _games = new Game[0];
        public IReadOnlyCollection<Game> Games => new ReadOnlyCollection<Game>(_games);

        public bool IsSolvable { get; set; }

        public Solution AddGame(Game game)
        {
            var array = new Game[_games.Length + 1];
            Array.Copy(_games, 0, array, 1, _games.Length);
            array[0] = game;
            return new Solution {_games = array, IsSolvable = this.IsSolvable};
        }

    }
    
    public class Game
    {
        private readonly Inning[] _innings = Enumerable.Range(1, 7).Select(a => new Inning()).ToArray();


        public IReadOnlyCollection<Inning> Innings => new ReadOnlyCollection<Inning>(_innings);

        public bool IsForfiet { get; set; }
        public string Name { get; set; }
    }

    public class Inning
    {
        private Dictionary<Position, string> _positionDictionary = new Dictionary<Position, string>();

        
    }
}
