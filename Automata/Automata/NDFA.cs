
using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class NDFA<T, U> : Automaton<T, U>
    {
        // Dictionary<Fromstate, Dictionary<terminal, HashSet<toStates>>>
        public Dictionary<T, Dictionary<U, HashSet<T>>> Transitions { get; private set; } = new Dictionary<T, Dictionary<U, HashSet<T>>>();
        public HashSet<U> Alphabet { get; private set; } = new HashSet<U>();
        public HashSet<T> StartStates { get; private set; } = new HashSet<T>();
        public HashSet<T> EndStates { get; private set; } = new HashSet<T>();
        public U Epsilon { get; private set; }

        public NDFA(U epsilon)
        {
            Epsilon = epsilon;
        }

        public NDFA(U epsilon, NDFA<T, U> ndfa1, NDFA<T, U> ndfa2)
        {
            Epsilon = epsilon;
            if (ndfa1 != null) Alphabet.UnionWith(ndfa1.Alphabet);
            if (ndfa2 != null) Alphabet.UnionWith(ndfa2.Alphabet);
            Transitions
                = ndfa1 != null && ndfa2 != null
                ? ndfa1.Transitions.Concat(ndfa2.Transitions).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
                : ndfa1 != null
                ? ndfa1.Transitions
                : ndfa2 != null
                ? ndfa2.Transitions
                : Transitions;
        }

        public bool addTransition(U input, T fromState, T toState)
        {
            // Add from state if non existant
            if (!Transitions.ContainsKey(fromState))
            {
                Transitions.Add(fromState, new Dictionary<U, HashSet<T>>());
            }

            if (Transitions[fromState].ContainsKey(input))
            {
                // Add to state to existing input.
                return Transitions[fromState][input].Add(toState);
            }
            else
            {
                // Add input to alphabet (except for epsilon) so the alphabet can dynamically be changed
                if (!input.Equals(Epsilon))
                {
                    Alphabet.Add(input);
                }
                // Add input with to state.
                Transitions[fromState].Add(input, new HashSet<T>() { toState });
                return true;
            }
        }

        public bool addStartState(T state)
        {
            return StartStates.Add(state);
        }

        public bool addEndState(T state)
        {
            return EndStates.Add(state);
        }

        public bool isValid()
        {
            bool result = false;
            if (StartStates.Count > 0 && EndStates.Count > 0)
            {
                foreach (Dictionary<U, HashSet<T>> t in Transitions.Values.ToArray())
                {
                    result = t.Keys.Count > 0;
                }
            }
            return result;
        }

        public bool accept(U[] input)
        {
            if (!isValid())
            {
                throw new AutomatonInvalidException();
            }

            var currStates = StartStates;
            foreach (U u in input)
            {
                if (!Alphabet.Contains(u))
                {
                    throw new System.ArgumentException($"{u} is not part of alphabet");
                }
                HashSet<T> newStates = new HashSet<T>();
                foreach (T state in currStates)
                {
                    if (!Transitions.ContainsKey(state))
                    {
                        continue;
                    }
                    if (Transitions[state].ContainsKey(Epsilon))
                    {
                        newStates.UnionWith(Transitions[state][Epsilon]);
                    }
                    if (Transitions[state].ContainsKey(u))
                    {
                        newStates.UnionWith(Transitions[state][u]);
                    }
                }
                currStates = newStates;
            }

            return EndStates.Intersect(currStates).Count() > 0;
        }

        public void forEachTransition(Action<U, T, T> action)
        {
            foreach (var transition in Transitions)
            {
                foreach (var terminalTo in transition.Value)
                {
                    foreach (var toState in terminalTo.Value)
                    {
                        action(terminalTo.Key, transition.Key, toState);
                    }
                }
            }
        }
    }
}
