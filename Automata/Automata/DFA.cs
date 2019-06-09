using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class DFA<T, U> : Automaton<T, U>
    {
        // Dictionary<Fromstate, Dictionary<terminal, toStates>>
        public Dictionary<T, Dictionary<U, T>> Transitions { get; protected set; } = new Dictionary<T, Dictionary<U, T>>();
        public HashSet<U> Alphabet { get; protected set; } = new HashSet<U>();
        public T StartState { get; protected set; }
        public HashSet<T> EndStates { get; protected set; } = new HashSet<T>();
        public HashSet<T> States { get; protected set; } = new HashSet<T>();

        public bool addTransition(U input, T fromState, T toState)
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
            States.Add(fromState);
            States.Add(toState);
            return true;
        }

        public bool addStartState(T state)
        {
            StartState = state;
            States.Add(state);
            return true;
        }

        public bool addEndState(T state)
        {
            States.Add(state);
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

        public DFA<Tuple<T, T>, U> and(DFA<T, U> other)
        {
            return andor(other, true);
        }

        public DFA<Tuple<T, T>, U> or(DFA<T, U> other)
        {
            return andor(other, false);
        }

        private DFA<Tuple<T, T>, U> andor(DFA<T, U> other, bool and)
        {
            if (!Alphabet.SetEquals(other.Alphabet))
                return null;
            var curr = new HashSet<Tuple<T, T>>(Transitions.Count * other.Transitions.Count());
            curr.Add(Tuple.Create(StartState, other.StartState));
            var result = new DFA<Tuple<T, T>, U>();
            result.addStartState(curr.First());
            while (result.Transitions.Count() < Transitions.Count * other.Transitions.Count())
            {
                var next = new HashSet<Tuple<T, T>>(Transitions.Count * other.Transitions.Count());
                foreach (var currState in curr)
                {
                    if (and ? (EndStates.Contains(currState.Item1) && other.EndStates.Contains(currState.Item2)) 
                        : (EndStates.Contains(currState.Item1) || other.EndStates.Contains(currState.Item2)))
                    {
                        result.addEndState(currState);
                    }
                    foreach (var terminal in Alphabet)
                    {
                        var nextState = Tuple.Create(Transitions[currState.Item1][terminal], other.Transitions[currState.Item2][terminal]);
                        next.Add(nextState);
                        result.addTransition(terminal, currState, nextState);
                    }
                }
                curr = next;
            }
            return result;
        }

        public DFA<T, U> negative()
        {
            DFA<T, U> result = new DFA<T, U>();
            result.addStartState(StartState);
            result.Transitions = Transitions;
            result.States = States;
            foreach (var state in States)
            {
                if (!EndStates.Contains(state))
                    result.addEndState(state);
            }
            return result;
        }

        public NDFA<T, U> reverse(U epsilon)
        {
            NDFA<T, U> result = new NDFA<T, U>(epsilon);
            // Add DFA's startstate as endstate
            result.addEndState(StartState);
            foreach (T endState in EndStates)
            {
                result.addStartState(endState);
            }

            forEachTransition((terminal, fromState, toState) 
                => result.addTransition(terminal, toState, fromState));
            return result;
        }

        public DFA<int, string> minimise()
        {
            string epsilon = Guid.NewGuid().ToString();
            return squash().reverse(epsilon).toDFA().squash().reverse(epsilon).toDFA().squash();
        }

        public DFA<int, string> squash()
        {
            DFA<int, string> result = new DFA<int, string>();
            List<T> states = States.ToList();
            result.addStartState(states.IndexOf(StartState));
            forEachTransition((terminal, fromState, toState) => result.addTransition(terminal.ToString(), states.IndexOf(fromState), states.IndexOf(toState)));
            foreach (T endState in EndStates)
            {
                result.addEndState(states.IndexOf(endState));
            }
            return result;
        }

        public void forEachTransition(Action<U, T, T> action)
        {
            foreach (KeyValuePair<T, Dictionary<U, T>> transition in Transitions)
            {
                foreach (KeyValuePair<U, T> terminalTo in transition.Value)
                {
                    action(terminalTo.Key, transition.Key, terminalTo.Value);
                }
            }
        }


    }
}
