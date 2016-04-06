using System.Collections.Generic;

namespace Algorithms
{
    public class GamePlayer
    {
        public string PlayerName { get; set; }

        public Dictionary<Position, int> PositionRanks { get; set; }
    }
}