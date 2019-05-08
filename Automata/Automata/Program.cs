using System;

namespace Automata
{
    class Program
    {
        static void Main(string[] args)
        {
            /**
             *  
             *  bevat een even aantal b’s of bevat een oneven aantal a’s
             *  bevat een even aantal b’s en eindigt op aab
             *  begint met abb en bevat baab
             */

            // Begint met abb of eindigt op baab
            DFA<int, char> dfa = new DFA<int, char>();
            dfa.addStartState(0);
            dfa.addEndState(3);
            dfa.addEndState(8);
            dfa.addTransition('a', 0, 1);
            dfa.addTransition('b', 0, 4);
            dfa.addTransition('a', 1, 4);
            dfa.addTransition('b', 1, 2);
            dfa.addTransition('a', 2, 4);
            dfa.addTransition('b', 2, 3);
            dfa.addTransition('a', 3, 3);
            dfa.addTransition('b', 3, 3);
            dfa.addTransition('a', 4, 4);
            dfa.addTransition('b', 4, 5);
            dfa.addTransition('a', 5, 6);
            dfa.addTransition('b', 5, 5);
            dfa.addTransition('a', 6, 7);
            dfa.addTransition('b', 6, 5);
            dfa.addTransition('a', 7, 4);
            dfa.addTransition('b', 7, 8);
            dfa.addTransition('a', 8, 6);
            dfa.addTransition('b', 8, 5);
            // Begint met abb of eindigt op baab
            String test = "abbabaab";
            tester(test, dfa.accept(test.ToCharArray()));
            test = "abbaaaaaa";
            tester(test, dfa.accept(test.ToCharArray()));
            test = "abaaaaaabbbbbbabaab";
            tester(test, dfa.accept(test.ToCharArray()));
            test = "abbaab";
            tester(test, dfa.accept(test.ToCharArray()));
            test = "abaa";
            tester(test, dfa.accept(test.ToCharArray()));
            test = "aaaaaaabbbbbb";
            tester(test, dfa.accept(test.ToCharArray()));
        }

        public static void tester(String stringToTest, bool accepted)
        {
            Console.WriteLine(
                "Testing: " + stringToTest + "\n" +
                "result: " + accepted + "\n");
        }
    }
}
