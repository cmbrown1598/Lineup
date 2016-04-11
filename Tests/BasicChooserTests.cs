using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class BasicChooserTests
    {
        public Chooser<string> SystemUnderTest
        {
            get; set;
        }

        [SetUp]
        public void BeforeEach()
        {
            var set = new List<string> { "Adam", "Ben", "Charlie", "David" };

            SystemUnderTest = new Chooser<string> {AvailableChoices = set};

        }

        [Test]
        public void ChooserReturnsNonNull()
        {
            var result = SystemUnderTest.Choose();

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void ChooserPicksItemFromProvidedSet()
        {
            var set = new List<string> {"Angel", "Barry", "Curt", "Derrick"};

            SystemUnderTest.AvailableChoices = set;

            var result = SystemUnderTest.Choose();

            Assert.That(set.Contains(result), Is.True);
        }

        [Test]
        public void ChooserPicksItemFromProvidedSetThatFollowsProvidedRule()
        {
            var rule = new Func<IEnumerable<string>, IEnumerable<string>>(a =>
            {
                return a.OrderByDescending(f => f.Length);
            });
            
            SystemUnderTest.AddRule(rule);

            var result = SystemUnderTest.Choose();

            Assert.That(result, Is.EqualTo("Charlie"));
        }

        [Test]
        public void ChooserAppliesRulesSequentially()
        {
            SystemUnderTest.AddRule(a => a.OrderByDescending(f => f.Length));
            SystemUnderTest.AddRule(a => a.OrderBy(f => f.Length));

            var result = SystemUnderTest.Choose();

            Assert.That(result, Is.EqualTo("Ben"));
        }

        [Test]
        public void ChooserCalledTwiceChoosesTheSameThing()
        {
            SystemUnderTest.AddRule(a => a.OrderByDescending(f => f.Length));
            SystemUnderTest.AddRule(a => a.OrderBy(f => f.Length));

            var firstResult = SystemUnderTest.Choose();
            var secondResult = SystemUnderTest.Choose();

            Assert.That(firstResult, Is.EqualTo("Ben"));
            Assert.That(secondResult, Is.EqualTo("Ben"));
        }


        [Test]
        public void ChooserCannotReferenceItself()
        {
            SystemUnderTest.AddRule(a => a.OrderByDescending(f => f.Length));
            SystemUnderTest.AddRule(a => a.OrderBy(f => f.Length));
            SystemUnderTest.AddRule(a => a.Where(b => b != SystemUnderTest.Choose()));

            Assert.Throws<SelfReferenceException>(() => { SystemUnderTest.Choose(); });
        }

        [Test]
        public void ChooserCanReferenceResultsOfItself()
        {
            SystemUnderTest.AddRule(a => a.OrderByDescending(f => f.Length));
            SystemUnderTest.AddRule(a => a.OrderBy(f => f.Length));

            var firstResult = SystemUnderTest.Choose();
            SystemUnderTest.AddRule(a => a.Where(b => b != firstResult));

            var secondResult = SystemUnderTest.Choose();
            Assert.That(secondResult, Is.EqualTo("Adam"));
        }
    }
}