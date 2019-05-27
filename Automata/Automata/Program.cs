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
            Automaton<int, char> dfa = new DFA<int, char>();
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

            Automaton<int, char> ndfa = new NDFA<int, char>('$');
            ndfa.addStartState(1);
            ndfa.addEndState(9);
            ndfa.addTransition('a', 1, 1);
            ndfa.addTransition('b', 1, 1);
            ndfa.addTransition('$', 1, 2);
            ndfa.addTransition('$', 1, 9);
            ndfa.addTransition('$', 2, 4);
            ndfa.addTransition('$', 2, 6);
            ndfa.addTransition('a', 4, 5);
            ndfa.addTransition('$', 5, 3);
            ndfa.addTransition('$', 2, 6);
            ndfa.addTransition('b', 6, 7);
            ndfa.addTransition('c', 7, 8);
            ndfa.addTransition('$', 8, 9);
            ndfa.addTransition('$', 3, 2);
            ndfa.addTransition('$', 3, 9);
            ndfa.addTransition('$', 1, 9);
            // Ends with a, or bc
            String ndfaTest = "acbe";
            tester(ndfaTest, ndfa.accept(ndfaTest.ToCharArray()));
            ndfaTest = "bbabaabbbca";
            tester(ndfaTest, ndfa.accept(ndfaTest.ToCharArray()));
        }

        public static void tester(String stringToTest, bool accepted)
        {
            Console.WriteLine(
                "Testing: " + stringToTest + "\n" +
                "result: " + accepted + "\n");
        }
    }
}
