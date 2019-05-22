using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class NDFA<T, U> : Automaton<T, U>
    {
        public NDFA() : base()
        {
        }

        public NDFA(HashSet<U> alphabet) : base(alphabet)
        {
        }

        public override bool addStartState(T state)
        {
            // Add startstate.
            return base.StartStates.Add(state);
        }

        public bool accept(U[] input)
        {
            if (!isValid())
            {
                throw new AutomatonInvalidException();
            }
            var currStates = base.StartStates.ToList();
            foreach (U u in input)
            {
                for  (int i = 0; i < currStates.Count; i++)
                {
                    currStates[i] = base.Transitions[currStates[i]][u];
                }
                currStates = currStates.Distinct().ToList();
            }

            return EndStates.Intersect(currStates).Count() != 0;
        }

        public override bool isValid()
        {
            bool result = false;
            if (base.StartStates.Count > 0 && base.EndStates.Count > 0)
            {
                foreach (Dictionary<U, T> t in Transitions.Values.ToArray())
                {
                    result = t.Keys.Count > 0;
                }
            }
            return result;
        }
    }
}
