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

        public static string[] UsefulDelimeters = {@",", @" ", @"", @"\t"};

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

        [WitnessFunction(nameof(Semantics.Split), 1)]
        public DisjunctiveExamplesSpec WitnessDelimiter(GrammarRule rule, ExampleSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = (string[]) example.Value;

                var delimiters = new List<string>();
                foreach (string d in UsefulDelimeters) {
                    if (input.Split(d).Equals(d)) {
                        delimiters.Append(d);
                    }
                }
                if (delimiters.Count == 0) return null;
                result[inputState] = delimiters.Cast<object>();
                Console.WriteLine("Delimiter o: {0}\ti: {1}\td: {2}", output, input, String.Join(", ", delimiters));
            }
            return DisjunctiveExamplesSpec.From(result);
        }

        [WitnessFunction(nameof(Semantics.Kth), 1)]
        public DisjunctiveExamplesSpec WitnessN(GrammarRule rule, ExampleSpec spec) {

            var nExamples = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var ss = inputState[rule.Body[0]] as string[]; // todo switch to actual SS
                var s = example.Value as string;

                var indexes = new List<int>();
                for (int i = 0; i < ss.Length; i++) {
                    if (ss[i] == s) indexes.Append(i);
                }
                if (indexes.Count == 0) return null;
                nExamples[inputState] = indexes.Cast<object>();
                Console.WriteLine("N ss: {0}\ts: {1}\ti: {2}", ss, s, String.Join(", ", indexes));
            }
            return DisjunctiveExamplesSpec.From(nExamples);
        }

        // https://github.com/microsoft/prose/blob/72b5e03a/ProgramSynthesis/ProseSample.TextExtraction/WitnessFunctions.cs
        // [WitnessFunction(nameof(Semantics.Kth), 1)]
        // public DisjunctiveExamplesSpec WitnessSS(GrammarRule rule, PrefixSpec spec) {
        //     var ssExample = new Dictionary<State, IEnumerable<object>>();

        //     foreach (var input in spec.ProvidedInputs) {
        //         var v = (string) input[rule.Grammar.InputSymbol];
        //         var ss = spec.PositiveExamples[input].Cast<string[]>();

        //         var indexesWithStr = new List<int>();
        //         foreach ()

        //         State inputState = example.Key;
        //         var output = example.Value as string;
        //         var occurrences = new List<int>();

        //         for (int i = input.IndexOf(output); i >= 0; i = input.IndexOf(output, i + 1)) {
        //             occurrences.Add(i);
        //         }

        //         if (occurrences.Count == 0) return null;
        //         result[inputState] = occurrences.Cast<object>();
        //         Console.WriteLine("Start o: {0}\ti: {1}\ts: {2}", output, input, String.Join(", ", occurrences));
        //     }
        //     return new DisjunctiveExamplesSpec(result);
        // }

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