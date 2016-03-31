using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Tests
{
    [TestFixture]
    public class GameTests
    {
        [SetUp]
        public void BeforeEach()
        {
            this.Subject = new Game();
        }

        public Game Subject { get; set; }

        [Test]
        public void HasSevenInnings()
        {
            this.Subject.Innings.Count.ShouldBe(7);
        }
    }

    public class Game
    {
        private readonly List<Inning> _innings;
        public ReadOnlyCollection<Inning> Innings => _innings.AsReadOnly();

        public Game()
        {
            this._innings = new List<Inning>();

            for (var i = 1; i <= 7; i++)
            {
                this._innings.Add(new Inning());
            }
        }
    }

    public class Inning
    {
    }
}
