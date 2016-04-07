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
        public void SolveGameMaximizesUtilizationOnInningsSimpleAllOn2InningsAllOffOneInningStructureWithAllOnData()
        {
            var scheduleItem = new ScheduleItem {GameNumber = 1, Opponent = "Newts"};
            var gamePlayers = Enumerable.Range(1, 10).Select((a) =>
            {
                var f = new GamePlayer
                {
                    PlayerName = $"Player {a}",
                    PositionRanks = (from Position p in Enum.GetValues(typeof(Position)) select p).ToDictionary(b => b, c => 0)
                };
                f.PositionRanks[((Position)(a - 1))] = 10;
                return f;
            }).ToArray();

            var game = SystemUnderTest.SolveGame(scheduleItem, gamePlayers);

            foreach (var i in Enumerable.Range(1, 10))
            {
                var position = ((Position) (i - 1));
                var player = $"Player {i}";
                var count = game.Innings.Count(inning => string.Equals(inning[position], player));
                Assert.That(count, Is.EqualTo(5));
            }
        }


    }
}