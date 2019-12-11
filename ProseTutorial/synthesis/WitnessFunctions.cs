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

        [WitnessFunction(nameof(Semantics.Kth), 1)]
        public DisjunctiveExamplesSpec WitnessDelimiter(GrammarRule rule, ExampleSpec spec) {
            var result = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = (string) example.Value;

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

        [WitnessFunction(nameof(Semantics.Kth), 2, DependsOnParameters = new[] {1})]
        public DisjunctiveExamplesSpec WitnessN(GrammarRule rule, ExampleSpec spec, ExampleSpec dSpec) {

            var nExamples = new Dictionary<State, IEnumerable<object>>();
            foreach (var example in spec.Examples) {
                State inputState = example.Key;
                var v = inputState[rule.Body[0]] as string;
                var s = example.Value as string;
                var d = dSpec.Examples[inputState] as string;

                var ss = v.Split(d);

                var indexes = new List<int>();
                for (int i = 0; i < v.Length; i++) {
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

    }
}