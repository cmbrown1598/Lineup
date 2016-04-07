﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Algorithms
{
    public class Solution
    {
        private Game[] _games = new Game[0];
        public IReadOnlyCollection<Game> Games => new ReadOnlyCollection<Game>(_games);

        public bool IsSolvable { get; set; }

        public Solution AddGame(Game game)
        {
            var array = new Game[_games.Length + 1];
            Array.Copy(_games, 0, array, 0, _games.Length);
            array[_games.Length] = game;
            return new Solution {_games = array, IsSolvable = this.IsSolvable};
        }

    }
}