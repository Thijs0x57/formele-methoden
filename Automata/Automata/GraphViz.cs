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
        public static void PrintGraph<T>(Automaat<T> data, string filename) where T : IComparable
        {
            var s = "digraph{ ";
            s += GetFinalStatesData(data);
            s += GetStartStatesData(data);

            s += "node [shape = circle];";

            foreach (var t in data._transitions)
            {
                //s += " " + ("S" + t.FromState) + " -> " + ("S" + t.ToState) + " " + "[ label = " + "\"" + t.Symbol + "\"" + " ];";
                if (t.Symbol.Equals('$'))
                {
                    s += $" S{t.FromState} -> S{t.ToState} [ label = \"{t.Symbol}\" ]; ";
                }
                else
                {
                    s += $" S{t.FromState} -> S{t.ToState} [ label = {t.Symbol} ]; ";
                }

            }
            s += " }";

            //Console.WriteLine(s);

            GenerateGraphFile(s, filename);
        }

        private static string GetFinalStatesData<T>(Automaat<T> a) where T : IComparable
        {
            if (a._finalStates.Count == 0) return "";

            var s = "node [shape = doublecircle];";

            foreach (var t in a._finalStates)
            {
                s += " " + ("S" + t) + " ";
            }
            s += ";  ";

            return s;
        }

        private static string GetStartStatesData<T>(Automaat<T> a) where T : IComparable
        {
            if (a._startStates.Count == 0) return "";

            var s = "node [shape=point]";
            s += "node0 [label=\"\"];";

            s += "node [shape = circle];";
            foreach (var state in a._startStates)
            {
                s += $" node0:\"\" -> S{state} ";
            }

            return s;
        }

        static void GenerateGraphFile(string data, string filename)
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand =
                new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand);

            byte[] output = wrapper.GenerateGraph(data, Enums.GraphReturnType.Jpg);
            System.IO.File.WriteAllBytes(filename + ".jpg", output);
            System.Diagnostics.Process.Start($"{filename}.jpg");
        }
    }
}
