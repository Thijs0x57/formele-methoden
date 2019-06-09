
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

        public DFA<HashSet<T>, U> toDFA()
        {
            DFA<HashSet<T>, U> result = new DFA<HashSet<T>, U>();
            HashSet<EqualSet<T>> curr = new HashSet<EqualSet<T>>();

            // Add start state
            EqualSet<T> start = new EqualSet<T>();
            foreach (T startState in StartStates)
            {
                start.UnionWith(epsilonClosure(startState));
            }
            curr.Add(start);
            result.addStartState(start);

            // Add trap state
            HashSet<T> trap = new EqualSet<T>();
            foreach (U terminal in Alphabet)
            {
                result.addTransition(terminal, trap, trap);
            }

            // Add transitions
            bool done = false;
            while (!done)
            {
                bool addedNew = false;
                HashSet<EqualSet<T>> next = new HashSet<EqualSet<T>>();
                // For each current state in set of current states.
                foreach (EqualSet<T> currState in curr)
                {
                    // For each terminal in the alphabet.
                    foreach (U terminal in Alphabet)
                    {
                        // nextState will become the actual dfa to state.
                        EqualSet<T> nextState = new EqualSet<T>();
                        // For each ndfa state of which the dfa state is made.
                        foreach (T subState in currState)
                        {
                            // Get the epsilon closure of the sub state.
                            HashSet<T> preClosure = epsilonClosure(subState);
                            // For each state in the epsilon closure.
                            foreach (T preClosureState in preClosure)
                            {
                                // Check if exists.
                                if (!Transitions.ContainsKey(preClosureState) || !Transitions[preClosureState].ContainsKey(terminal))
                                {
                                    continue;
                                }
                                // Get all the to states for this terminal.
                                HashSet<T> follow = Transitions[preClosureState][terminal];
                                // Accumulate the epsilon closure of each followed state.
                                foreach (T followedState in follow)
                                {
                                    HashSet<T> postClosure = epsilonClosure(followedState);
                                    nextState.UnionWith(postClosure);
                                }

                                next.Add(nextState);
                            }
                        }

                        if (nextState.Count() > 0)
                        {
                            // Add transition.
                            if (result.addTransition(terminal, currState, nextState))
                            {
                                addedNew = true;
                            }
                            // Add end state
                            if (EndStates.Intersect(nextState).Count() > 0)
                            {
                                result.addEndState(nextState);
                            }
                        }
                        else
                        {
                            /*
. . . . . . . . . . . . . . . .   ____________
. . . . . . . . . . . . . . . . / It’s a trap! \
. . . . . . . . . . . . . . . . \ _____________/
                            __...------------._
                         ,-'                   `-.
                      ,-'                         `.
                    ,'                            ,-`.
                   ;                              `-' `.
                  ;                                 .-. \
                 ;                           .-.    `-'  \
                ;                            `-'          \
               ;                                          `.
               ;                                           :
              ;                                            |
             ;                                             ;
            ;                            ___              ;
           ;                        ,-;-','.`.__          |
       _..;                      ,-' ;`,'.`,'.--`.        |
      ///;           ,-'   `. ,-'   ;` ;`,','_.--=:      /
     |'':          ,'        :     ;` ;,;,,-'_.-._`.   ,'
     '  :         ;_.-.      `.    :' ;;;'.ee.    \|  /
      \.'    _..-'/8o. `.     :    :! ' ':8888)   || /
       ||`-''    \\88o\ :     :    :! :  :`""'    ;;/
       ||         \"88o\;     `.    \ `. `.      ;,'
       /)   ___    `."'/(--.._ `.    `.`.  `-..-' ;--.
       \(.="""""==.. `'-'     `.|      `-`-..__.-' `. `.
        |          `"==.__      )                    )  ;
        |   ||           `"=== '                   .'  .'
        /\,,||||  | |           \                .'   .'
        | |||'|' |'|'           \|             .'   _.' \
        | |\' |  |           || ||           .'    .'    \
        ' | \ ' |'  .   ``-- `| ||         .'    .'       \
          '  |  ' |  .    ``-.._ |  ;    .'    .'          `.
       _.--,;`.       .  --  ...._,'   .'    .'              `.__
     ,'  ,';   `.     .   --..__..--'.'    .'                __/_\
   ,'   ; ;     |    .   --..__.._.'     .'                ,'     `.
  /    ; :     ;     .    -.. _.'     _.'                 /         `
 /     :  `-._ |    .    _.--'     _.'                   |
/       `.    `--....--''       _.'                      |
          `._              _..-'                         |
             `-..____...-''                              |
                                                         |
                                                         |
                             */
                            result.addTransition(terminal, currState, trap);
                        }
                    }
                }
                curr = next;
                done = !addedNew;
            }

            return result;
        }

        public HashSet<T> epsilonClosure(T state)
        {
            HashSet<T> eClosure = new HashSet<T>();
            if (!Transitions.ContainsKey(state))
            {
                return eClosure;
            }
            if (Transitions[state].ContainsKey(Epsilon))
            {
                eClosure.UnionWith(Transitions[state][Epsilon]);
            }
            HashSet<T> eClosure2 = new HashSet<T>();
            do
            {
                foreach (T eState in eClosure)
                {
                    eClosure2.UnionWith(epsilonClosure(eState));
                }
                eClosure.UnionWith(eClosure2);
            }
            while (eClosure2.Except(eClosure).Count() > 0);
            eClosure.Add(state);
            return eClosure;
        }
    }
}
