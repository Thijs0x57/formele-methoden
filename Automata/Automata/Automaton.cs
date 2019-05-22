using System.Collections.Generic;

namespace Automata
{
    /**
     * <typeparam name="T" State type
     * <typeparam name="U" Alphabet type
     * 
     */
    abstract class Automaton<T, U>
    {
        public Dictionary<T, Dictionary<U, T>> Transitions { get; protected set; } = new Dictionary<T, Dictionary<U, T>>();
        public HashSet<U> Alphabet { get; protected set; }
        public HashSet<T> StartStates { get; protected set; } = new HashSet<T>();
        public HashSet<T> EndStates { get; protected set; } = new HashSet<T>();

        public Automaton()
        {
            this.Alphabet = new HashSet<U>();
        }

        public Automaton(HashSet<U> alphabet)
        {
            this.Alphabet = alphabet;
        }

        public abstract bool isValid();

        public bool addTransition(U input, T fromState, T toState)
        {
            // Check if state and input already exists (it shouldn't)
            if (!Transitions.ContainsKey(fromState))
            {
                // Add the transition to the automaton
                Transitions.Add(fromState, new Dictionary<U, T>());
            }
            // Check if input already exists
            if (Transitions[fromState].ContainsKey(input))
            {
                // if it already exists we don't need to add it, so the method returns false
                return false;
            }
            // Add input to alphabet so the alphabet can dynamically be changed
            Alphabet.Add(input);
            Transitions[fromState].Add(input, toState);
            return true;
        }

        public virtual bool addStartState(T state)
        {
            return StartStates.Add(state);
        }

        public bool addEndState(T state)
        {
            return EndStates.Add(state);
        }
    }
}
