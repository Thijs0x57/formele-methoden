using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    /**
     * <typeparam name="T" State type
     * <typeparam name="U" Alphabet type
     * 
     */
    abstract class Automaton<T, U>
    {
        protected Dictionary<T, Dictionary<U, T>> automaton = new Dictionary<T, Dictionary<U, T>>();
        private List<U> alphabet;

        public Automaton(List<U> alphabet)
        {
            this.alphabet = alphabet;
        }

        public abstract void addTransition(U input, T fromState, T toState);
    }
}
