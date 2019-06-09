using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class EqualSet<T> : HashSet<T>
    {
        public override bool Equals(object obj)
        {
            return base.SetEquals(obj as IEnumerable<T>);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (T e in this)
            {
                hash ^= e.GetHashCode();
            }
            return hash;
        }
    }
}