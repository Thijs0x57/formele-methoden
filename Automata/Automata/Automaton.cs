using System.Collections.Generic;

namespace Automata
{
    /**
     * <typeparam name="T" State type
     * <typeparam name="U" Alphabet type
     * 
     */
    interface Automaton<T, U>
    {
        bool addTransition(U input, T fromState, T toState);

        bool addStartState(T state);

        bool addEndState(T state);

        bool isValid();

        bool accept(U[] input);

    }
}
