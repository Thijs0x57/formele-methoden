using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class DFA<T, U> : Automaton<T, U>
    {
        public DFA(List<U> alphabet) : base(alphabet)
        {
        }

        public override void addTransition(U input, T fromState, T toState)
        {
            if (base.automaton.ContainsKey(fromState))
            {
                
            }
        }
    }
}
