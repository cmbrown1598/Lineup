using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public LineupSolver SystemUnderTest { get; set; }
        public Domain Domain { get; set; }

        public PlayerItem[] GetPlayers(int numberOfPlayers)
        {
            return Enumerable.Range(1, numberOfPlayers)
                             .Select(a => new PlayerItem {Name = $"Player {a}"})
                             .ToArray();
        }

        public PlayerPositionItem[] GetPlayerPositions(int numberOfPlayers)
        {
            var random = new Random();
            return Enumerable.Range(1, numberOfPlayers)
                             .SelectMany(a => (int[]) Enum.GetValues(typeof (Position)), (a, n) => new PlayerPositionItem
                             {
                                Name = $"Player {a}",
                                Position = (Position) n,
                                Rank = random.Next(1, 10) 
                             }).ToArray();
        }

        [SetUp]
        public void Setup()
        {
            SystemUnderTest = new LineupSolver();
            Domain = new Domain(
                    new[] { new ScheduleItem { GameNumber = 1, Opponent = "No One" } }, 
                    GetPlayers(10), 
                    new PlayerGameAvailabilityItem[0], 
                    GetPlayerPositions(10));
        }

        [Test]
        public void SolverCreatesAGameForOneScheduleEntry()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.Count, Is.EqualTo(1));
            Assert.That(result.Games.First().Name, Is.EqualTo("1 - No One"));
        }

        [Test]
        public void SolverCreatesAGameForMultipleScheduleEntries()
        {

            Domain.ScheduleItems = new[]
            {
                new ScheduleItem {GameNumber = 1, Opponent = "No One"},
                new ScheduleItem {GameNumber = 2, Opponent = "Someone"},
                new ScheduleItem {GameNumber = 3, Opponent = "Everyone"}
            };

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.Count, Is.EqualTo(3));
            Assert.That(result.Games.First().Name, Is.EqualTo("1 - No One"));
            Assert.That(result.Games.Skip(1).First().Name, Is.EqualTo("2 - Someone"));
            Assert.That(result.Games.Last().Name, Is.EqualTo("3 - Everyone"));

        }

        [Test]
        public void SolveCreatesAGameWith7Innings()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.First().Innings.Count, Is.EqualTo(7));
        }

        [Test]
        public void EachInningIsNumberedAppropriately()
        {
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.Games.First().Innings.First().Number, Is.EqualTo(1));
            Assert.That(result.Games.First().Innings.Skip(3).First().Number, Is.EqualTo(4));
            Assert.That(result.Games.First().Innings.Last().Number, Is.EqualTo(7));
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
            Domain.PlayerGameAvailabilityItems = new [] { new PlayerGameAvailabilityItem { GameNumber = 1, Name = "Player 1"}, new PlayerGameAvailabilityItem { GameNumber = 1, Name = "Player 2" }, new PlayerGameAvailabilityItem { GameNumber = 1, Name = "Player 3" }, };
          
            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.True);
            Assert.That(result.Games.First().IsForfiet, Is.True);
        }

        [Test]
        public void WhenMoreThanEnoughPlayersAreAvailableEachInningHasSomeoneSittingOut()
        {
            var positions = Enum.GetValues(typeof(Position));
            Domain.PlayerItems = GetPlayers(11);
            Domain.PlayerPositionItems = GetPlayerPositions(11);

            var result = SystemUnderTest.Solve(Domain);

            Assert.That(result.IsSolvable, Is.True);
            Assert.That(result.Games.First().IsForfiet, Is.False);

            foreach (var inning in result.Games.First().Innings)
            {
                Assert.That(inning.SittingOut.Length, Is.EqualTo(1));
                foreach (Position position in positions)
                {
                    Assert.That(inning[position], Is.Not.EqualTo(inning.SittingOut[0]));
                }
            }
        }

        [TestCase(SolvingMode.Simple)]
        [TestCase(SolvingMode.OptimizeForSkill)]
        public void SolveSolvesSuccessfullyForBasicSetups(SolvingMode solvingMode)
        {
            SystemUnderTest.SolvingMode = solvingMode;

            var positions = Enum.GetValues(typeof (Position));
            var result = SystemUnderTest.Solve(Domain);

            var game = result.Games.First();
            // The game is not forfiet
            Assert.That(game.IsForfiet, Is.False, "Game should not be a forfeit.");

            foreach (
                var playersList in
                    game.Innings.Select(
                        inning => (from Position position in positions select inning[position]).ToList())
                )
            {
                // all players fielded are unique
                Assert.That(playersList.Distinct().Count(), Is.EqualTo(10),
                    "There are not enough unique players on the field.");
            }

            foreach (Position position in positions)
            {
                var playerList = game.Innings.Select(inning => inning[position]).ToList();
                for (var i = 0; i < playerList.Count - 2; i++)
                {
                    var isInvalid = (playerList[i] == playerList[i + 1]) && (playerList[i + 1] == playerList[i + 2]);
                    // no player is set to the same position more than twice.
                    Assert.That(isInvalid, Is.False, "Players repeat positions more than acceptably.");
                }
            }

            var sittingOut = game.Innings.SelectMany(b => b.SittingOut).ToList();

            for (var i = 0; i < sittingOut.Count - 1; i++)
            {
                var isInvalid = sittingOut[i] == sittingOut[i + 1];
                Assert.That(isInvalid, Is.False, "Players sit out more than acceptably.");
            }
        }
    }
}
