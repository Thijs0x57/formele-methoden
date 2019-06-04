using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class DFA<T, U> : Automaton<T, U>
    {
        public Dictionary<T, Dictionary<U, T>> Transitions { get; protected set; } = new Dictionary<T, Dictionary<U, T>>();
        public HashSet<U> Alphabet { get; protected set; } = new HashSet<U>();
        public T StartState { get; protected set; }
        public HashSet<T> EndStates { get; protected set; } = new HashSet<T>();

        public virtual bool addTransition(U input, T fromState, T toState)
        {
            // Check if state and input already exists
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

        public bool addStartState(T state)
        {
            // Only add startstate when there are 0 startStates, otherwise return false.
            return StartState == null
                ? addStartState(state)
                : false;
        }

        public bool addEndState(T state)
        {
            return EndStates.Add(state);
        }
        public bool isValid()
        {
            if (StartState == null || EndStates.Count == 0)
            {
                return false;
            }
            foreach (Dictionary<U, T> t in Transitions.Values.ToArray())
            {
                if (!t.Keys.ToHashSet().SetEquals(Alphabet))
                {
                    return false;
                }
            }
            return true;
        }

        public bool accept(U[] input)
        {
            if (!isValid())
            {
                throw new AutomatonInvalidException();
            }
            var currState = StartState;
            foreach (U u in input)
            {
                if (!Alphabet.Contains(u))
                {
                    throw new System.ArgumentException($"{u} is not part of alphabet");
                }
                currState = Transitions[currState][u];
            }

            return EndStates.Contains(currState);
        }

        public DFA<T[], U> and(DFA<T, U> other)
        {
            if (Alphabet != other.Alphabet)
                return null;
            var curr = new T[] { StartState, other.StartState };
            var result = new DFA<T[], U>();
            result.addStartState(curr);
            while (result.Transitions.Count() < Transitions.Count * other.Transitions.Count())
            {
                foreach (var terminal in Alphabet)
                {
                    var next = new T[2];
                    if (Transitions.ContainsKey(curr[0]))
                    {
                        next[0] = Transitions[curr[0]][terminal];
                    }
                    if (other.Transitions.ContainsKey(curr[1]))
                    {
                        next[1] = other.Transitions[curr[1]][terminal];
                    }
                    if (EndStates.Contains(curr[0]) && other.EndStates.Contains(curr[1]))
                    {
                        result.addEndState(curr);
                    }
                    result.addTransition(terminal, curr, next);
                    curr = next;
                }
            }
            return result;
        }

        public DFA<T, U> or(DFA<T, U> other)
        {
            return null;
        }

        public DFA<T, U> negative()
        {
            DFA<T, U> result = new DFA<T, U>();
            result.addStartState(StartState);
            result.Transitions = Transitions;


            return null;
        }
    }
}
