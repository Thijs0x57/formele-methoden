using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class DFA<T, U> : Automaton<T, U>
    {
        public DFA() : base()
        {
        }

        public DFA(HashSet<U> alphabet) : base(alphabet)
        {
        }

        public override bool addStartState(T state)
        {
            // Only add startstate when there are 0 startStates, otherwise return false.
            return base.StartStates.Count == 0
                ? base.addStartState(state)
                : false;
        }

        public bool accept(U[] input)
        {
            if (!base.IsValid)
            {
                throw new AutomatonInvalidException();
            }
            var currState = base.StartStates.First();
            foreach (U u in input)
            {
                currState = base.Transitions[currState][u];
            }

            return EndStates.Contains(currState);
        }
    }
}
