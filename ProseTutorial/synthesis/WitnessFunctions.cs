using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ProgramSynthesis;
using System.Threading.Tasks;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;
using Microsoft.ProgramSynthesis.Learning;

namespace ProseTutorial {
    public class WitnessFunctions : DomainLearningLogic {
        public WitnessFunctions(Grammar grammar) : base(grammar) { }

        // We will use this set of regular expressions in this tutorial 
        public static Regex[] UsefulRegexes = {
    new Regex(@"\w+"),  // Word
	new Regex(@"\d+"),  // Number
    new Regex(@"\s+"),  // Space
    new Regex(@".+"),  // Anything
    new Regex(@"$")  // End of line
};

        [WitnessFunction(nameof(Semantics.Append), 0)]
        public DisjunctiveExamplesSpec WitnessPrefix(GrammarRule rule, ExampleSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();

            Console.WriteLine("[Prefix spec {0}", spec.Examples.Count);
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var output = example.Value as string;
                // Console.WriteLine("Prefix for {0}", output);
                var substrings = new List<string>();
                for (int i = 1; i <= output.Length - 1; ++i) {
                    substrings.Add(output.Substring(0, i));
                }
                if (substrings.Count == 0) return null;
                result[inputState] = substrings.Cast<object>();
                Console.WriteLine("Prefix o: {0}\tp: {1}", output, String.Join(", ", substrings));
            }
            return new DisjunctiveExamplesSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Append), 1, DependsOnParameters = new []{0})]
        public ExampleSpec WitnessSuffix(GrammarRule rule, ExampleSpec spec, ExampleSpec prefixSpec) {
            var result = new Dictionary<State, object>();
            Console.WriteLine("[Suffix spec {0} startSpec {1}", spec.Examples.Count, prefixSpec.Examples.Count);
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var output = example.Value as string;
                // Console.WriteLine("Suffix for {0}", output);
                var prefix = (string) prefixSpec.Examples[inputState];
                result[inputState] = output.Substring(prefix.Length);
                Console.WriteLine("Suffix o: {0}\tp: {1}\ts: {2}", output, prefix, output.Substring(prefix.Length));
            }
            return new ExampleSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Substring), 1)]
        public DisjunctiveExamplesSpec WitnessStartPosition(GrammarRule rule, ExampleSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();

            Console.WriteLine("[Start spec {0}", spec.Examples.Count);
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;
                var occurrences = new List<int>();

                for (int i = input.IndexOf(output); i >= 0; i = input.IndexOf(output, i + 1)) {
                    occurrences.Add(i);
                }

                if (occurrences.Count == 0) return null;
                result[inputState] = occurrences.Cast<object>();
                Console.WriteLine("Start o: {0}\ti: {1}\ts: {2}", output, input, String.Join(", ", occurrences));
            }
            return new DisjunctiveExamplesSpec(result);

        }

        [WitnessFunction(nameof(Semantics.Substring), 2, DependsOnParameters = new []{1})]
        public ExampleSpec WitnessEndPosition(GrammarRule rule, ExampleSpec spec, ExampleSpec startSpec) {
            var result = new Dictionary<State, object>();
            Console.WriteLine("[End spec {0} startSpec {1}", spec.Examples.Count, startSpec.Examples.Count);
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var output = example.Value as string;
                var start = (int) startSpec.Examples[inputState];
                result[inputState] = start + output.Length;
                Console.WriteLine("End o: {0}\ts: {1}\te: {2}", output, start, start + output.Length);
            }
            return new ExampleSpec(result);
        }

        [WitnessFunction(nameof(Semantics.AbsPos), 1)]
        public DisjunctiveExamplesSpec WitnessK(GrammarRule rule, DisjunctiveExamplesSpec spec) {

            var kExamples = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.DisjunctiveExamples) {
                State inputState = example.Key;
                var v = inputState[rule.Body[0]] as string;

                var positions = new List<int>();
                foreach (int pos in example.Value) {
                    positions.Add((int)pos + 1);
                    positions.Add((int)pos - v.Length - 1);
                }
                if (positions.Count == 0) return null;
                kExamples[inputState] = positions.Cast<object>();
            }
            return DisjunctiveExamplesSpec.From(kExamples);
        }

        [WitnessFunction(nameof(Semantics.RelPos), 1)]
        public DisjunctiveExamplesSpec WitnessRegexPair(GrammarRule rule, DisjunctiveExamplesSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.DisjunctiveExamples) {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;

                var regexes = new List<Tuple<Regex, Regex>>();
                foreach (int output in example.Value) {
                    List<Regex>[] leftMatches, rightMatches;
                    BuildStringMatches(input, out leftMatches, out rightMatches);


                    var leftRegex = leftMatches[output];
                    var rightRegex = rightMatches[output];
                    if (leftRegex.Count == 0 || rightRegex.Count == 0)
                        return null;
                    regexes.AddRange(from l in leftRegex
                                     from r in rightRegex
                                     select Tuple.Create(l, r));
                }
                if (regexes.Count == 0) return null;
                result[inputState] = regexes;
            }
            return DisjunctiveExamplesSpec.From(result);
        }

        static void BuildStringMatches(string inp, out List<Regex>[] leftMatches,
                                       out List<Regex>[] rightMatches) {
            leftMatches = new List<Regex>[inp.Length + 1];
            rightMatches = new List<Regex>[inp.Length + 1];
            for (int p = 0; p <= inp.Length; ++p) {
                leftMatches[p] = new List<Regex>();
                rightMatches[p] = new List<Regex>();
            }
            foreach (Regex r in UsefulRegexes) {
                foreach (Match m in r.Matches(inp)) {
                    leftMatches[m.Index + m.Length].Add(r);
                    rightMatches[m.Index].Add(r);
                }
            }
        }

    }
}