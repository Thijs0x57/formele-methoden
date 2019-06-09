using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Automata
{
    class GraphViz
    {
        public static void PrintNDFA<T, U>(NDFA<T, U> ndfa, string filename)
        {
            var s = "digraph{ ";
            s += GetFinalStatesData(ndfa.EndStates);
            s += GetStartStatesData(ndfa.StartStates);

            s += "node [shape = circle];";
            ndfa.forEachTransition((terminal, fromState, toState) => s += TransitionToString(terminal, fromState, toState));
            s += " }";

            Console.WriteLine(s);

            GenerateGraphFile(s, filename, Enums.GraphReturnType.Svg);
        }

        public static void PrintDFA<T, U>(DFA<T, U> dfa, string filename)
        {
            var s = "digraph{ ";
            s += GetFinalStatesData(dfa.EndStates);
            s += GetStartStatesData(new HashSet<T> { dfa.StartState });

            s += "node [shape = circle];";
            dfa.forEachTransition((terminal, fromState, toState) => s += TransitionToString(terminal, fromState, toState));
            s += " }";

            Console.WriteLine(s);

            GenerateGraphFile(s, filename, Enums.GraphReturnType.Svg);
        }

        private static string TransitionToString<T, U>(U terminal, T fromState, T toState)
        {
                return $" \"S{stateToString(fromState)}\" -> \"S{stateToString(toState)}\" [ label = \"{terminal}\" ]; ";
        }

        private static string stateToString<T>(T state)
        {
            if (state is IEnumerable || typeof(T).GetInterfaces().Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                IEnumerable enumState = (IEnumerable)state;
                return '{' + string.Join(",", enumState.Cast<object>()) + '}';
            }
            else
            {
                return state.ToString();
            }
        }

        private static string GetFinalStatesData<T>(HashSet<T> endStates)//Automaton<T,U> a) where T : IComparable
        {
            if (endStates.Count == 0) return "";

            var s = "node [shape = doublecircle];";

            foreach (var name in endStates)
            {
                s += " " + ("\"S" + stateToString(name)) + "\" ";
            }
            s += ";  ";

            return s;
        }

        private static string GetStartStatesData<T>(HashSet<T> startStates)// where T : IComparable
        {
            if (startStates.Count == 0) return "";

            var s = "node [shape=point]";
            s += "node0 [label=\"\"];";

            s += "node [shape = circle];";
            foreach (var state in startStates)
            {
                s += $" node0:\"\" -> \"S{stateToString(state)}\" ";
            }

            return s;
        }

        static void GenerateGraphFile(string data, string filename, Enums.GraphReturnType filetype)
        {
            Console.WriteLine($"filetype: {filetype}\n");
            System.IO.File.WriteAllText($"./{filename}.gv", data);
        }
    }
}
