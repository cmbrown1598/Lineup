using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public class Chooser<T>
    {
        private readonly ConcurrentQueue<Func<IEnumerable<T>, IEnumerable<T>>> _rules = new ConcurrentQueue<Func<IEnumerable<T>, IEnumerable<T>>>();

        private bool _isChoosing;
        private readonly object _sync = new object();


        public T Choose()
        {
            lock (_sync)
            {
                if (_isChoosing)
                    throw new SelfReferenceException();

                var choices = AvailableChoices.ToList();

                _isChoosing = true;
                choices = _rules.Aggregate(choices, (current, rule) => rule(current).ToList());
                _isChoosing = false;

                return choices.First();
            }
        }

        public List<T> AvailableChoices { get; set; }


        public void AddRule(Func<IEnumerable<T>, IEnumerable<T>> func)
        {
            _rules.Enqueue(func);
        }
    }
}