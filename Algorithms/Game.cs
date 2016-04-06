using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Algorithms
{
    public class Game
    {
        private readonly ReadOnlyCollection<Inning> _innings = new ReadOnlyCollection<Inning>(Enumerable.Range(1, 7).Select(a => new Inning { Number = a}).ToArray());

        public IReadOnlyCollection<Inning> Innings => _innings;

        public bool IsForfiet { get; set; }
        public string Name { get; set; }
    }
}