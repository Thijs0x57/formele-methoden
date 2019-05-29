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
                { return s1.CompareTo(s2); }
                else
                { return s1.Length - s2.Length; }
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

        public SortedSet<String> getLanguage(int maxSteps)
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
                    languageLeft = left == null ? emptyLanguage : left.getLanguage(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.getLanguage(maxSteps - 1);
                    languageResult.UnionWith(languageLeft);
                    languageResult.UnionWith(languageRight);
                    break;


                case Operator.DOT:
                    languageLeft = left == null ? emptyLanguage : left.getLanguage(maxSteps - 1);
                    languageRight = right == null ? emptyLanguage : right.getLanguage(maxSteps - 1);
                    foreach (String s1 in languageLeft)
                        foreach (String s2 in languageRight)
                        { languageResult.Add(s1 + s2); }
                    break;

                // STAR(*) en PLUS(+) kunnen we bijna op dezelfde manier uitwerken:
                case Operator.STAR:
                case Operator.PLUS:
                    languageLeft = left == null ? emptyLanguage : left.getLanguage(maxSteps - 1);
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
                    Console.WriteLine("getLanguage is nog niet gedefinieerd voor de operator: " + this.op);
                    break;
            }


            return languageResult;
        }


        public NDFA<int, char> toNDFA()
        {
            return null;
        }
    }
}
