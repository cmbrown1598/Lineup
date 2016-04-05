using System.Collections.Generic;
using Algorithms;

namespace Tests
{
    public class Inning
    {
        private readonly Dictionary<Position, string> _positionDictionary = new Dictionary<Position, string>
        {
            { Position.Pitcher, string.Empty },
            { Position.Catcher, string.Empty },
            { Position.CenterField, string.Empty },
            { Position.FirstBase, string.Empty },
            { Position.LeftField, string.Empty },
            { Position.RightField, string.Empty },
            { Position.Rover, string.Empty },
            { Position.SecondBase, string.Empty },
            { Position.ShortStop, string.Empty },
            { Position.ThirdBase, string.Empty },
        };

        public string this[Position indexer] {
            get { return _positionDictionary[indexer]; }
            set { _positionDictionary[indexer] = value; }
        }
    }
}