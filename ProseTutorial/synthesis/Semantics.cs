﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ProgramSynthesis.Utils;


namespace ProseTutorial {
    public static class Semantics {
        public static string Substring(string v, int start, int end) => v.Substring(start, end - start);

        public static string Kth(string[] ss, int k) {
            if (k < ss.Length && k >= 0) return ss[k];
            return null;
        }

        public static string[] Split(string v, string d) => v.Split(d);

        public static int? AbsPos(string v, int k) {
            return k > 0 ? k - 1 : v.Length + k + 1;
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
