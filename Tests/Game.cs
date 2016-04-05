using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Tests
{
    public class Game
    {
        private readonly ReadOnlyCollection<Inning> _innings = new ReadOnlyCollection<Inning>(Enumerable.Range(1, 7).Select(a => new Inning()).ToArray());

        public IReadOnlyCollection<Inning> Innings => _innings;

        public bool IsForfiet { get; set; }
        public string Name { get; set; }
    }
}