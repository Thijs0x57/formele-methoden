using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Regex
    {
        private enum Operator
        {
            PLUS, STAR, OR, DOT, ONE
        }
        private Operator op;
        private String terminals;
        private Regex left;
        private Regex right;

        public class CompareByLength : IComparer<string>
        {
            public int Compare(string s1, string s2)
            {
                if (s1.Length == s2.Length)
                    return s1.CompareTo(s2);
                else
                    return s1.Length - s2.Length;
            }
        }

        public Regex()
        {
            op = Operator.ONE;
            terminals = "";
            left = null;
            right = null;
        }

        public Regex(String p)
        {
            op = Operator.ONE;
            terminals = p;
            left = null;
            right = null;
        }

        public Regex plus()
        {
            Regex result = new Regex();
            result.op = Operator.PLUS;
            result.left = this;
            return result;
        }

        public Regex star()
        {
            Regex result = new Regex();
            result.op = Operator.STAR;
            result.left = this;
            return result;
        }

        public Regex or(Regex e2)
        {
            Regex result = new Regex();
            result.op = Operator.OR;
            result.left = this;
            result.right = e2;
            return result;
        }

        public Regex dot(Regex e2)
        {
            Regex result = new Regex();
            result.op = Operator.DOT;
            result.left = this;
            result.right = e2;
            return result;
        }

        public SortedSet<String> getLanguageUntilLength(int maxSteps)
        {
            SortedSet<String> emptyLanguage = new SortedSet<String>(new CompareByLength());
            SortedSet<String> languageResult = new SortedSet<String>(new CompareByLength());

            SortedSet<String> languageLeft, languageRight;

            if (maxSteps < 1) return emptyLanguage;

            switch (this.op)
            {
                case Operator.ONE:
                    { languageResult.Add(terminals); }
                    break;

                case Operator.OR:
                    languageLeft = left == null ? emptyLanguage : left.getLanguageUntilLength(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.getLanguageUntilLength(maxSteps - 1);
                    languageResult.UnionWith(languageLeft);
                    languageResult.UnionWith(languageRight);
                    break;


                case Operator.DOT:
                    languageLeft = left == null ? emptyLanguage : left.getLanguageUntilLength(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.getLanguageUntilLength(maxSteps - 1);
                    foreach (String s1 in languageLeft)
                        foreach (String s2 in languageRight)
                        { languageResult.Add(s1 + s2); }
                    break;

                // STAR(*) en PLUS(+) kunnen we bijna op dezelfde manier uitwerken:
                case Operator.STAR:
                case Operator.PLUS:
                    languageLeft = left == null ? emptyLanguage : left.getLanguageUntilLength(maxSteps - 1);
                    languageResult.UnionWith(languageLeft);
                    for (int i = 1; i < maxSteps; i++)
                    {
                        HashSet<String> languageTemp = new HashSet<String>(languageResult);
                        foreach (String s1 in languageLeft)
                        {
                            foreach (String s2 in languageTemp)
                            {
                                languageResult.Add(s1 + s2);
                            }
                        }
                    }
                    if (this.op == Operator.STAR)
                    { languageResult.Add(""); }
                    break;


                default:
                    Console.WriteLine("getLanguageUntilLength is nog niet gedefinieerd voor de operator: " + this.op);
                    break;
            }
            return languageResult;
        }

        public NDFA<string, char> toNDFA()
        {
            NDFA<string, char> ndfaLeft = left?.toNDFA();
            NDFA<string, char> ndfaRight = right?.toNDFA();
            var ndfa = new NDFA<string, char>('$', ndfaLeft, ndfaRight);
            switch (op)
            {
                case Operator.ONE:
                    var currState = Guid.NewGuid().ToString();
                    ndfa.addStartState(currState);
                    foreach (var terminal in terminals)
                    {
                        ndfa.addTransition(terminal, currState, currState = Guid.NewGuid().ToString());
                    }
                    ndfa.addEndState(currState);
                    break;
                case Operator.OR:
                    var orStartState = Guid.NewGuid().ToString();
                    var orEndState = Guid.NewGuid().ToString();
                    ndfa.addStartState(orStartState);
                    ndfa.addTransition(ndfa.Epsilon, orStartState, ndfaLeft.StartStates.First());
                    ndfa.addTransition(ndfa.Epsilon, orStartState, ndfaRight.StartStates.First());
                    ndfa.addTransition(ndfa.Epsilon, ndfaLeft.EndStates.Last(), orEndState);
                    ndfa.addTransition(ndfa.Epsilon, ndfaRight.EndStates.Last(), orEndState);
                    ndfa.addEndState(orEndState);
                    break;
                case Operator.DOT:
                    ndfa.addStartState(ndfaLeft.StartStates.First());
                    ndfa.addEndState(ndfaRight.EndStates.Last());
                    ndfa.addTransition(ndfa.Epsilon, ndfaLeft.EndStates.Last(), ndfaRight.StartStates.First());
                    break;
                case Operator.PLUS:
                    ndfa = plusAndStarOperatorHandlerCalledByToNDFA(ndfa, ndfaLeft);
                    break;
                case Operator.STAR:
                    ndfa = plusAndStarOperatorHandlerCalledByToNDFA(ndfa, ndfaLeft);
                    break;
                default:
                    break;
            }
            return ndfa;
        }

        private NDFA<string, char> plusAndStarOperatorHandlerCalledByToNDFA(NDFA<string, char> ndfa, NDFA<string, char> left)
        {
            var startState = Guid.NewGuid().ToString();
            var endState = Guid.NewGuid().ToString();
            ndfa.addStartState(startState);
            ndfa.addEndState(endState);
            ndfa.addTransition(ndfa.Epsilon, startState, left.StartStates.First());
            ndfa.addTransition(ndfa.Epsilon, left.EndStates.Last(), left.StartStates.First());
            ndfa.addTransition(ndfa.Epsilon, left.EndStates.Last(), endState);
            if (op == Operator.STAR)
            {
                ndfa.addTransition(ndfa.Epsilon, startState, endState);
            }
            return ndfa;
        }
    }
}
