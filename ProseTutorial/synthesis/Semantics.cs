using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ProgramSynthesis.Utils;


namespace ProseTutorial {
    public static class Semantics {
        public static string Kth(string v, string d, int n) {
            var ss = v.Split(d);
            if (n < ss.Length && n >= 0) return ss[n];
            return null;
        }
    }
}
