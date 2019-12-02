using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ProgramSynthesis.Utils;

/*
 * PROSE documentation on Semantics:
 *  https://microsoft.github.io/prose/documentation/prose/tutorial/#semantics
 * and Black-Box Operators:
 *  https://microsoft.github.io/prose/documentation/prose/usage/#black-box-operators
 */

namespace ProseTutorial {
    public static class Semantics {
        public static string Substring(string v, int start, int end) => v.Substring(start, end - start);

        public static int? AbsPos(string v, int k) {
            // TODO update the return statement to consider the case where k is a negative number
            // if k is positive, you should return k-1 but it k is negative, it represents the one-based index from the right to the left,
            // so you should return the length of v + k + 1. 
            return k - 1;
        }

        public static int? RelPos(string v, Tuple<Regex, Regex> rr) {
            Regex left = rr.Item1;
            Regex right = rr.Item2;
            var rightMatches = right.Matches(v);

            foreach (Match leftMatch in left.Matches(v)) {
                foreach (Match rightMatch in rightMatches) {
                    if (rightMatch.Index == leftMatch.Index + leftMatch.Length)
                        return leftMatch.Index + leftMatch.Length;
                }
            }
            return null;
        }
    }
}
