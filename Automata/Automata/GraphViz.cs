using System;
using System.Collections.Generic;
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

            foreach (var transition in ndfa.Transitions)
                foreach (var terminal in transition.Value.Keys)
                    foreach (var toStateHashset in transition.Value.Values)
                        foreach (var toState in toStateHashset)
                        {
                            if (terminal.Equals(ndfa.Epsilon))
                            {
                                // fromstate -> toState = terminal
                                s += $" S{transition.Key} -> S{toState} [ label = \"{terminal}\" ]; ";
                            }
                            else
                            {
                                s += $" S{transition.Key} -> S{toState} [ label = {terminal} ]; ";
                            }
                        }
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

            foreach (var transition in dfa.Transitions)
                foreach (var terminal in transition.Value.Keys)
                    foreach (var toState in transition.Value.Values)
                    {
                        s += $" S{transition.Key} -> S{toState} [ label = {terminal} ]; ";
                    }
            s += " }";

            Console.WriteLine(s);

            GenerateGraphFile(s, filename, Enums.GraphReturnType.Svg);
        }

        //public static void PrintGraph<T, U>(Automaton<T, U> automaton, string filename) where T : IComparable
        //{
        //    var s = "digraph{ ";
        //    s += GetFinalStatesData(automaton);
        //    s += GetStartStatesData(automaton);

        //    s += "node [shape = circle];";

        //    foreach (var t in automaton.Transitions)
        //    {
        //        //s += " " + ("S" + t.FromState) + " -> " + ("S" + t.ToState) + " " + "[ label = " + "\"" + t.Symbol + "\"" + " ];";
        //        if (t.Symbol.Equals('$'))
        //        {
        //            s += $" S{t.FromState} -> S{t.ToState} [ label = \"{t.Symbol}\" ]; ";
        //        }
        //        else
        //        {
        //            s += $" S{t.FromState} -> S{t.ToState} [ label = {t.Symbol} ]; ";
        //        }

        //    }
        //    s += " }";

        //    Console.WriteLine(s);

        //    GenerateGraphFile(s, filename);
        //}

        private static string GetFinalStatesData<T>(HashSet<T> endStates)//Automaton<T,U> a) where T : IComparable
        {
            if (endStates.Count == 0) return "";

            var s = "node [shape = doublecircle];";

            foreach (var t in endStates)
            {
                s += " " + ("S" + t) + " ";
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
                s += $" node0:\"\" -> S{state} ";
            }

            return s;
        }

        static void GenerateGraphFile(string data, string filename, Enums.GraphReturnType filetype)
        {
            Console.WriteLine($"filetype: {filetype}");
            System.IO.File.WriteAllText("./fsm.gv", data);
        }
    }
}
