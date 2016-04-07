using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Algorithms;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OptimizeForSkillAlgorithmTest
    {

        [SetUp]
        public void BeforeEach()
        {
            SystemUnderTest = new OptimizeForSkillAlgorithm();
        }

        
        public OptimizeForSkillAlgorithm SystemUnderTest { get; set; }


        [Test]
        public void SolveGameMaximizesUtilizationAndTakesSecondaryUtilizationInOffInnings()
        {
            // Each player is named what his best skill is.
            // Each player has a secondary skill position.
            // Test should have 5 innings (1, 2, 4, 5, 7) of all top players.
            // and 2 innings (3, 6) with secondary skill

            var scheduleItem = new ScheduleItem {GameNumber = 1, Opponent = "Newts"};
            var gamePlayers = Enumerable.Range(1, 10).Select((a) =>
            {
                var position = (Position) (a - 1);
                var f = new GamePlayer
                {
                    PlayerName = $"{position}",
                    PositionRanks = (from Position p in Enum.GetValues(typeof(Position)) select p).ToDictionary(b => b, c => 0)
                };
                f.PositionRanks[position] = 10;
                if((a - 2) == -1)
                {
                    f.PositionRanks[Position.Rover] = 9;
                }
                else
                {
                    f.PositionRanks[position - 1] = 9;
                }
                return f;

            }).ToArray();

            var game = SystemUnderTest.SolveGame(scheduleItem, gamePlayers);

            foreach (var i in Enumerable.Range(0, 9))
            {
                var position = (Position) i;
                var secondaryPosition = i - 1 == -1 ? Position.Rover : (Position) i - 1 ;
                var player = $"{position}";
                var count = game.Innings.Count(inning => string.Equals(inning[position], player));
                var secondarycount = game.Innings.Count(inning => string.Equals(inning[secondaryPosition], player));
                Assert.That(count, Is.EqualTo(5), "There should be 5 innings where the 'best player' is playing a particular position.");
                Assert.That(secondarycount, Is.EqualTo(2), "There should be 2 innings where the 'second best player' is playing a particular position.");
            }
        }

        [Test]
        public void SolveGameMaximizesSimpleUtilization()
        {
            // Each player is named what his best skill is, and have skills of 10 at that spot.
            // All other skills are zerod out
            // Test should have 5 innings (1, 2, 4, 5, 7) of all top players.
            // and 2 have non top players.
            var scheduleItem = new ScheduleItem { GameNumber = 1, Opponent = "Newts" };
            var gamePlayers = Enumerable.Range(1, 10).Select((a) =>
            {
                var position = (Position)(a - 1);
                var f = new GamePlayer
                {
                    PlayerName = $"{position}",
                    PositionRanks = (from Position p in Enum.GetValues(typeof(Position)) select p).ToDictionary(b => b, c => 0)
                };
                f.PositionRanks[position] = 10;
                return f;

            }).ToArray();

            var game = SystemUnderTest.SolveGame(scheduleItem, gamePlayers);

            foreach (var i in Enumerable.Range(0, 9))
            {
                var position = (Position)i;
                var player = $"{position}";
                var count = game.Innings.Count(inning => string.Equals(inning[position], player));
                Assert.That(count, Is.EqualTo(5), "There should only be 5 innings where the 'best player' is playing a particular position.");
            }
        }
        

    }


}