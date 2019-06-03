﻿using System;

namespace Automata
{
    class Program
    {
        private static Regex expr1, expr2, expr3, expr4, expr5, a, b, all;
        static void Main(string[] args)
        {
            TestDFA();
            TestNDFA();
            TestRegExp();
            testLanguage();
        }

        public static void tester(String stringToTest, bool accepted)
        {
            Console.WriteLine(
                "Testing: " + stringToTest + "\n" +
                "result: " + accepted + "\n");
        }

        public static void TestDFA()
        {
            /**
             *  bevat een even aantal b’s of bevat een oneven aantal a’s
             *  bevat een even aantal b’s en eindigt op aab
             *  begint met abb en bevat baab
             */
            // Begint met abb of eindigt op baab
            Console.WriteLine("--------------DFA--------------");
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
        }

        public static void TestNDFA()
        {
            Console.WriteLine("--------------NDFA--------------");
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
            string ndfaTest = "acb";
            tester(ndfaTest, ndfa.accept(ndfaTest.ToCharArray()));
            ndfaTest = "bbabaabbbca";
            tester(ndfaTest, ndfa.accept(ndfaTest.ToCharArray()));
        }

        public static void TestRegExp()
        {
            a = new Regex("a");
            b = new Regex("b");

            // expr1: "baa"
            expr1 = new Regex("baa");
            // expr2: "bb"
            expr2 = new Regex("bb");
            // expr3: "baa | baa"
            expr3 = expr1.or(expr2);

            // all: "(a|b)*"
            all = (a.or(b)).star();

            // expr4: "(baa | baa)+"
            expr4 = expr3.plus();
            // expr5: "(baa | baa)+ (a|b)*"
            expr5 = expr4.dot(all);
            // converting to NDFA
            NDFA<string, char> ndfa = expr5.toNDFA();
            string ndfaTest = "baab";
            tester(ndfaTest, ndfa.accept(ndfaTest.ToCharArray()));
        }

        public static void testLanguage()
        {
            Console.WriteLine("--------------Regex--------------");
            Console.WriteLine("1. taal van (baa):\n" + string.Join(" ", expr1.getLanguageUntilLength(1)));
            Console.WriteLine("2. taal van (bb):\n" + string.Join(" ", expr2.getLanguageUntilLength(1)));
            Console.WriteLine("3. taal van (baa | bb):\n" + string.Join(" ", expr3.getLanguageUntilLength(2)));
            
            Console.WriteLine("4. taal van (a|b)*:\n" + string.Join(" ", all.getLanguageUntilLength(3)));
            Console.WriteLine("5. taal van (baa | bb)+:\n" + string.Join(" ", expr4.getLanguageUntilLength(4)));
            Console.WriteLine("6. taal van (baa | bb)+ (a|b)*:\n" + string.Join(" ", expr5.getLanguageUntilLength(4)));
        }
    }
}
