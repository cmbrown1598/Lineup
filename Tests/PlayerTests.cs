using Algorithms;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PlayerTests
    {

        [SetUp]
        public void BeforeEach()
        {
            this.Subject = new PlayerItem()
            {
                Name = "FredBob",
            };
        }

        public PlayerItem Subject { get; set; }

        [Test]
        public void CanPlayInGame_WhenAvailable()
        {
            var availability = new[]
            {
                new PlayerGameAvailabilityItem() {GameNumber = 1, Name = "FredBob",},
                new PlayerGameAvailabilityItem() {GameNumber = 3, Name = "FredBob",},
            };
            var result = Subject.CanPlayInGame(2, availability);

            Assert.That(result, Is.True);
        }

        [Test]
        public void CanPlayInGame_WhenNotAvailable()
        {
            var availability = new[]
            {
                new PlayerGameAvailabilityItem() {GameNumber = 1, Name = "FredBob",},
                new PlayerGameAvailabilityItem() {GameNumber = 3, Name = "FredBob",},
            };

            var result = Subject.CanPlayInGame(1, availability);

            Assert.That(result, Is.False);
        }

        [Test]
        public void CanPlayInGame_WhenReallyAvailable()
        {
            var availability = new[]
            {
                new PlayerGameAvailabilityItem() {GameNumber = 1, Name = "FredBob",},
                new PlayerGameAvailabilityItem() {GameNumber = 3, Name = "FredBob",},
            };
            Subject.Name = "Jackson";
            var result = Subject.CanPlayInGame(1, availability);

            Assert.That(result, Is.True);
        }
    }
}